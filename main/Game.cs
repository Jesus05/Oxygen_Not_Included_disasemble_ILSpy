using FMOD.Studio;
using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGenGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Game : KMonoBehaviour
{
	[Serializable]
	public struct SavedInfo
	{
		public bool discoveredSurface;
	}

	public struct CallbackInfo
	{
		public System.Action cb;

		public bool manuallyRelease;

		public CallbackInfo(System.Action cb, bool manually_release = false)
		{
			this.cb = cb;
			manuallyRelease = manually_release;
		}
	}

	public struct ComplexCallbackInfo<DataType>
	{
		public Action<DataType, object> cb;

		public object callbackData;

		public string debugInfo;

		public ComplexCallbackInfo(Action<DataType, object> cb, object callback_data, string debug_info)
		{
			this.cb = cb;
			debugInfo = debug_info;
			callbackData = callback_data;
		}
	}

	public class ComplexCallbackHandleVector<DataType>
	{
		private HandleVector<ComplexCallbackInfo<DataType>> baseMgr;

		private Dictionary<int, string> releaseInfo = new Dictionary<int, string>();

		public ComplexCallbackHandleVector(int initial_size)
		{
			baseMgr = new HandleVector<ComplexCallbackInfo<DataType>>(initial_size);
		}

		public HandleVector<ComplexCallbackInfo<DataType>>.Handle Add(Action<DataType, object> cb, object callback_data, string debug_info)
		{
			return baseMgr.Add(new ComplexCallbackInfo<DataType>(cb, callback_data, debug_info));
		}

		public ComplexCallbackInfo<DataType> GetItem(HandleVector<ComplexCallbackInfo<DataType>>.Handle handle)
		{
			try
			{
				return baseMgr.GetItem(handle);
			}
			catch (Exception ex)
			{
				baseMgr.UnpackHandleUnchecked(handle, out byte _, out int index);
				string value = null;
				if (releaseInfo.TryGetValue(index, out value))
				{
					KCrashReporter.Assert(false, "Trying to get data for handle that was already released by " + value);
				}
				else
				{
					KCrashReporter.Assert(false, "Trying to get data for handle that was released ...... magically");
				}
				throw ex;
			}
		}

		public ComplexCallbackInfo<DataType> Release(HandleVector<ComplexCallbackInfo<DataType>>.Handle handle, string release_info)
		{
			byte version;
			int index;
			try
			{
				baseMgr.UnpackHandle(handle, out version, out index);
				releaseInfo[index] = release_info;
				return baseMgr.Release(handle);
			}
			catch (Exception ex)
			{
				baseMgr.UnpackHandleUnchecked(handle, out version, out index);
				string value = null;
				if (releaseInfo.TryGetValue(index, out value))
				{
					KCrashReporter.Assert(false, release_info + "is trying to release handle but it was already released by " + value);
				}
				else
				{
					KCrashReporter.Assert(false, release_info + "is trying to release a handle that was already released by some unknown thing");
				}
				throw ex;
			}
		}

		public void Clear()
		{
			baseMgr.Clear();
		}

		public bool IsVersionValid(HandleVector<ComplexCallbackInfo<DataType>>.Handle handle)
		{
			return baseMgr.IsVersionValid(handle);
		}
	}

	[Serializable]
	public class ConduitVisInfo
	{
		public GameObject prefab;

		[Header("Main View")]
		public Color32 tint;

		public Color32 insulatedTint;

		public Color32 radiantTint;

		[Header("Overlay")]
		public Color32 overlayTint;

		public Color32 overlayInsulatedTint;

		public Color32 overlayRadiantTint;

		public Vector2 overlayMassScaleRange = new Vector2f(1f, 1000f);

		public Vector2 overlayMassScaleValues = new Vector2f(0.1f, 1f);
	}

	private enum SpawnRotationConfig
	{
		Normal,
		StringName
	}

	[Serializable]
	private struct SpawnRotationData
	{
		public string animName;

		public bool flip;
	}

	[Serializable]
	private struct SpawnPoolData
	{
		[HashedEnum]
		public SpawnFXHashes id;

		public int initialCount;

		public Color32 colour;

		public GameObject fxPrefab;

		public string initialAnim;

		public Vector3 spawnOffset;

		public Vector2 spawnRandomOffset;

		public SpawnRotationConfig rotationConfig;

		public SpawnRotationData[] rotationData;
	}

	[Serializable]
	private class Settings
	{
		public int nextUniqueID;

		public int gameID;

		public Settings(Game game)
		{
			nextUniqueID = KPrefabID.NextUniqueID;
			gameID = KleiMetrics.GameID();
		}

		public Settings()
		{
		}
	}

	public class GameSaveData
	{
		public ConduitFlow gasConduitFlow;

		public ConduitFlow liquidConduitFlow;

		public Vector2I simActiveRegionMin;

		public Vector2I simActiveRegionMax;

		public FallingWater fallingWater;

		public UnstableGroundManager unstableGround;

		public WorldDetailSave worldDetail;

		public CustomGameSettings customGameSettings;

		public bool debugWasUsed;

		public bool autoPrioritizeRoles;

		public bool advancedPersonalPriorities;

		public SavedInfo savedInfo;
	}

	public delegate void CansaveCB();

	public delegate void SavingPreCB(CansaveCB cb);

	public delegate void SavingActiveCB();

	public delegate void SavingPostCB();

	[Serializable]
	public struct LocationColours
	{
		public Color unreachable;

		public Color invalidLocation;

		public Color validLocation;

		public Color requiresRole;

		public Color unreachable_requiresRole;
	}

	[Serializable]
	public class UIColours
	{
		[SerializeField]
		private LocationColours digColours;

		[SerializeField]
		private LocationColours buildColours;

		public LocationColours Dig => digColours;

		public LocationColours Build => buildColours;
	}

	private static readonly string NextUniqueIDKey = "NextUniqueID";

	public static string worldID = null;

	public static List<ModError> modLoadErrors;

	private PlayerController playerController;

	private CameraController cameraController;

	public Action<GameSaveData> OnSave;

	public Action<GameSaveData> OnLoad;

	[NonSerialized]
	public bool baseAlreadyCreated = false;

	[NonSerialized]
	public bool autoPrioritizeRoles = false;

	[NonSerialized]
	public bool advancedPersonalPriorities = false;

	public SavedInfo savedInfo;

	public static bool quitting = false;

	public AssignmentManager assignmentManager;

	public GameObject playerPrefab;

	public GameObject screenManagerPrefab;

	public GameObject cameraControllerPrefab;

	public GameObject tempIntroScreenPrefab;

	public static int BlockSelectionLayerMask;

	public static int PickupableLayer;

	public Element VisualTunerElement;

	public float currentSunlightIntensity = 0f;

	public RoomProber roomProber;

	public RoleManager roleManager;

	public FetchManager fetchManager;

	public EdiblesManager ediblesManager;

	public SpacecraftManager spacecraftManager;

	public UserMenu userMenu;

	public Unlocks unlocks;

	private bool sandboxModeActive = false;

	public HandleVector<CallbackInfo> callbackManager = new HandleVector<CallbackInfo>(256);

	public ComplexCallbackHandleVector<int> simComponentCallbackManager = new ComplexCallbackHandleVector<int>(256);

	public ComplexCallbackHandleVector<Sim.MassConsumedCallback> massConsumedCallbackManager = new ComplexCallbackHandleVector<Sim.MassConsumedCallback>(64);

	public ComplexCallbackHandleVector<Sim.MassEmittedCallback> massEmitCallbackManager = new ComplexCallbackHandleVector<Sim.MassEmittedCallback>(64);

	public ComplexCallbackHandleVector<Sim.DiseaseConsumptionCallback> diseaseConsumptionCallbackManager = new ComplexCallbackHandleVector<Sim.DiseaseConsumptionCallback>(64);

	[NonSerialized]
	public Player LocalPlayer;

	[SerializeField]
	public TextAsset maleNamesFile;

	[SerializeField]
	public TextAsset femaleNamesFile;

	[NonSerialized]
	public World world;

	[NonSerialized]
	public CircuitManager circuitManager;

	[NonSerialized]
	public EnergySim energySim;

	[NonSerialized]
	public LogicCircuitManager logicCircuitManager;

	private GameScreenManager screenMgr;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> gasConduitSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> liquidConduitSystem;

	public UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem;

	public UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem;

	public UtilityNetworkTubesManager travelTubeSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, SolidConduit> solidConduitSystem;

	public ConduitFlow gasConduitFlow;

	public ConduitFlow liquidConduitFlow;

	public SolidConduitFlow solidConduitFlow;

	public Accumulators accumulators;

	public PlantElementAbsorbers plantElementAbsorbers;

	public bool showGasConduitDisease;

	public bool showLiquidConduitDisease;

	public ConduitFlowVisualizer gasFlowVisualizer;

	public ConduitFlowVisualizer liquidFlowVisualizer;

	public SolidConduitFlowVisualizer solidFlowVisualizer;

	public ConduitTemperatureManager conduitTemperatureManager;

	public ConduitDiseaseManager conduitDiseaseManager;

	public MingleCellTracker mingleCellTracker;

	private int simSubTick;

	private bool hasFirstSimTickRun;

	private float simDt;

	[SerializeField]
	public ConduitVisInfo liquidConduitVisInfo;

	[SerializeField]
	public ConduitVisInfo gasConduitVisInfo;

	[SerializeField]
	public ConduitVisInfo solidConduitVisInfo;

	[SerializeField]
	private Material liquidFlowMaterial;

	[SerializeField]
	private Material gasFlowMaterial;

	[SerializeField]
	private Color flowColour;

	private Vector3 gasFlowPos;

	private Vector3 liquidFlowPos;

	private Vector3 solidFlowPos;

	public bool drawStatusItems = true;

	private List<SolidInfo> solidInfo = new List<SolidInfo>();

	private List<Klei.CallbackInfo> callbackInfo = new List<Klei.CallbackInfo>();

	private List<SolidInfo> gameSolidInfo = new List<SolidInfo>();

	private bool IsPaused = false;

	private HashSet<int> solidChangedFilter = new HashSet<int>();

	public SafetyConditions safetyConditions = new SafetyConditions();

	public SimData simData = new SimData();

	[MyCmpGet]
	private GameScenePartitioner gameScenePartitioner;

	private bool gameStarted = false;

	private static readonly EventSystem.IntraObjectHandler<Game> MarkStatusItemRendererDirtyDelegate = new EventSystem.IntraObjectHandler<Game>(delegate(Game component, object data)
	{
		component.MarkStatusItemRendererDirty(data);
	});

	private ushort[] activeFX;

	private Vector2I simActiveRegionMin;

	private Vector2I simActiveRegionMax;

	public bool debugWasUsed = false;

	[SerializeField]
	private bool forceActiveArea = false;

	[SerializeField]
	private Vector2 minForcedActiveArea = new Vector2(0f, 0f);

	[SerializeField]
	private Vector2 maxForcedActiveArea = new Vector2(128f, 128f);

	private bool isLoading = false;

	private HashedString previousOverlayMode = OverlayModes.None.ID;

	private float previousGasConduitFlowDiscreteLerpPercent = -1f;

	private float previousLiquidConduitFlowDiscreteLerpPercent = -1f;

	private float previousSolidConduitFlowDiscreteLerpPercent = -1f;

	[SerializeField]
	private SpawnPoolData[] fxSpawnData;

	private Dictionary<int, Action<Vector3, float>> fxSpawner = new Dictionary<int, Action<Vector3, float>>();

	private Dictionary<int, ObjectPool> fxPools = new Dictionary<int, ObjectPool>();

	private SavingPreCB activatePreCB = null;

	private SavingActiveCB activateActiveCB = null;

	private SavingPostCB activatePostCB = null;

	[SerializeField]
	public UIColours uiColours = new UIColours();

	private float lastTimeWorkStarted = float.NegativeInfinity;

	public KInputHandler inputHandler
	{
		get;
		set;
	}

	public static Game Instance
	{
		get;
		private set;
	}

	public bool SandboxModeActive
	{
		get
		{
			return sandboxModeActive;
		}
		set
		{
			sandboxModeActive = value;
			Trigger(-1948169901, null);
			if ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null)
			{
				PlanScreen.Instance.Refresh();
			}
			if ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null)
			{
				BuildMenu.Instance.Refresh();
			}
		}
	}

	public StatusItemRenderer statusItemRenderer
	{
		get;
		private set;
	}

	public PrioritizableRenderer prioritizableRenderer
	{
		get;
		private set;
	}

	public float LastTimeWorkStarted => lastTimeWorkStarted;

	public static bool IsQuitting()
	{
		return quitting;
	}

	protected override void OnPrefabInit()
	{
		Output.Log(Time.realtimeSinceStartup, "Level Loaded....", SceneManager.GetActiveScene().name);
		Singleton<KBatchedAnimUpdater>.CreateInstance();
		Singleton<CellChangeMonitor>.CreateInstance();
		userMenu = new UserMenu();
		SimTemperatureTransfer.ClearInstanceMap();
		StructureTemperatureComponents.ClearInstanceMap();
		ElementConsumer.ClearInstanceMap();
		App.OnPreLoadScene = (System.Action)Delegate.Combine(App.OnPreLoadScene, new System.Action(StopBE));
		Instance = this;
		statusItemRenderer = new StatusItemRenderer();
		prioritizableRenderer = new PrioritizableRenderer();
		LoadEventHashes();
		gasFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.GasConduits) - 0.4f);
		liquidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.LiquidConduits) - 0.4f);
		solidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.SolidConduitContents) - 0.4f);
		Shader.WarmupAllShaders();
		Db.Get();
		quitting = false;
		PickupableLayer = LayerMask.NameToLayer("Pickupable");
		BlockSelectionLayerMask = LayerMask.GetMask("BlockSelection");
		world = World.Instance;
		KPrefabID.NextUniqueID = KPlayerPrefs.GetInt(NextUniqueIDKey, 0);
		circuitManager = new CircuitManager();
		energySim = new EnergySim();
		gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 13);
		liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 17);
		electricalConduitSystem = new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(Grid.WidthInCells, Grid.HeightInCells, 25);
		logicCircuitSystem = new UtilityNetworkManager<LogicCircuitNetwork, LogicWire>(Grid.WidthInCells, Grid.HeightInCells, 30);
		logicCircuitManager = new LogicCircuitManager(logicCircuitSystem);
		travelTubeSystem = new UtilityNetworkTubesManager(Grid.WidthInCells, Grid.HeightInCells, 32);
		solidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, SolidConduit>(Grid.WidthInCells, Grid.HeightInCells, 21);
		conduitTemperatureManager = new ConduitTemperatureManager();
		conduitDiseaseManager = new ConduitDiseaseManager(conduitTemperatureManager);
		gasConduitFlow = new ConduitFlow(ConduitType.Gas, Grid.CellCount, gasConduitSystem, 1f, 0.25f);
		liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, Grid.CellCount, liquidConduitSystem, 10f, 0.75f);
		solidConduitFlow = new SolidConduitFlow(Grid.CellCount, solidConduitSystem, 0.75f);
		gasFlowVisualizer = new ConduitFlowVisualizer(gasConduitFlow, gasConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundGas, Lighting.Instance.Settings.GasConduit);
		liquidFlowVisualizer = new ConduitFlowVisualizer(liquidConduitFlow, liquidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundLiquid, Lighting.Instance.Settings.LiquidConduit);
		solidFlowVisualizer = new SolidConduitFlowVisualizer(solidConduitFlow, solidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundSolid, Lighting.Instance.Settings.SolidConduit);
		accumulators = new Accumulators();
		plantElementAbsorbers = new PlantElementAbsorbers();
		activeFX = new ushort[Grid.CellCount];
		simActiveRegionMax = new Vector2I(0, 0);
		simActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		UnsafePrefabInit();
		Shader.SetGlobalVector("_MetalParameters", new Vector4(0f, 0f, 0f, 0f));
		Shader.SetGlobalVector("_WaterParameters", new Vector4(0f, 0f, 0f, 0f));
		InitializeFXSpawners();
		PathFinder.Initialize();
		new GameNavGrids(Pathfinding.Instance);
		screenMgr = Util.KInstantiate(screenManagerPrefab, null, null).GetComponent<GameScreenManager>();
		roleManager = new RoleManager();
		roomProber = new RoomProber();
		fetchManager = base.gameObject.AddComponent<FetchManager>();
		ediblesManager = base.gameObject.AddComponent<EdiblesManager>();
		Singleton<CellChangeMonitor>.Instance.SetGridSize(Grid.WidthInCells, Grid.HeightInCells);
		unlocks = GetComponent<Unlocks>();
	}

	public void SetGameStarted()
	{
		gameStarted = true;
	}

	public bool GameStarted()
	{
		return gameStarted;
	}

	private unsafe void UnsafePrefabInit()
	{
		StepTheSim(0f);
	}

	protected override void OnLoadLevel()
	{
		Unsubscribe(1798162660, MarkStatusItemRendererDirtyDelegate, false);
		base.OnLoadLevel();
	}

	private void MarkStatusItemRendererDirty(object data)
	{
		statusItemRenderer.MarkAllDirty();
	}

	protected override void OnForcedCleanUp()
	{
		if (prioritizableRenderer != null)
		{
			prioritizableRenderer.Cleanup();
			prioritizableRenderer = null;
		}
		if (statusItemRenderer != null)
		{
			statusItemRenderer.Destroy();
			statusItemRenderer = null;
		}
		if (conduitTemperatureManager != null)
		{
			conduitTemperatureManager.Shutdown();
		}
		gasFlowVisualizer.FreeResources();
		liquidFlowVisualizer.FreeResources();
		solidFlowVisualizer.FreeResources();
		LightGridManager.Shutdown();
		App.OnPreLoadScene = (System.Action)Delegate.Remove(App.OnPreLoadScene, new System.Action(StopBE));
		base.OnForcedCleanUp();
	}

	protected override void OnSpawn()
	{
		Debug.Log("-- GAME --", null);
		PropertyTextures.FogOfWarScale = 0f;
		if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null)
		{
			CameraController.Instance.FreeCameraEnabled = false;
		}
		LocalPlayer = SpawnPlayer();
		WaterCubes.Instance.Init();
		SpeedControlScreen.Instance.Pause(false);
		LightGridManager.Initialise();
		UnsafeOnSpawn();
		Time.timeScale = 0f;
		if ((UnityEngine.Object)tempIntroScreenPrefab != (UnityEngine.Object)null)
		{
			Util.KInstantiate(tempIntroScreenPrefab, null, null);
		}
		if (SaveLoader.Instance.cachedGSD != null)
		{
			Reset(SaveLoader.Instance.cachedGSD);
			NewBaseScreen.SetInitialCamera();
		}
		TagManager.FillMissingProperNames();
		CameraController.Instance.SetOrthographicsSize(20f);
		if (SaveLoader.Instance.loadedFromSave)
		{
			baseAlreadyCreated = true;
			Trigger(-1992507039, null);
			Trigger(-838649377, null);
		}
		KScreen kScreen = LocalPlayer.ScreenManager.StartScreen(ScreenPrefabs.Instance.ResourceCategoryScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		kScreen.transform.SetSiblingIndex(1);
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(MeshRenderer));
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer meshRenderer = (MeshRenderer)array[i];
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		Subscribe(1798162660, MarkStatusItemRendererDirtyDelegate);
		solidConduitFlow.Initialize();
		SimAndRenderScheduler.instance.Add(roomProber, false);
		SimAndRenderScheduler.instance.Add(KComponentSpawn.instance, false);
		if (!SaveLoader.Instance.loadedFromSave)
		{
			SettingConfig settingConfig = CustomGameSettings.Instance.QualitySettings[CustomGameSettingConfigs.SandboxMode.id];
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SandboxMode);
			SaveGame.Instance.sandboxEnabled = !settingConfig.IsDefaultLevel(currentQualitySetting.id);
		}
		mingleCellTracker = base.gameObject.AddComponent<MingleCellTracker>();
		if ((UnityEngine.Object)Global.Instance != (UnityEngine.Object)null)
		{
			Global.Instance.GetComponent<PerformanceMonitor>().Reset();
		}
		if (modLoadErrors != null)
		{
			ModErrorsScreen.ShowErrors(modLoadErrors);
			modLoadErrors = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		SimAndRenderScheduler.instance.Remove(KComponentSpawn.instance);
		DestroyInstances();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		DestroyInstances();
	}

	private void UnsafeOnSpawn()
	{
		world.UpdateCellInfo(gameSolidInfo, callbackInfo, 0, null, 0, null);
	}

	public void SetMusicEnabled(bool enabled)
	{
		if (enabled)
		{
			MusicManager.instance.PlaySong("Music_FrontEnd", false);
		}
		else
		{
			MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	private Player SpawnPlayer()
	{
		GameObject gameObject = Util.KInstantiate(playerPrefab, base.gameObject, null);
		Player component = gameObject.GetComponent<Player>();
		component.ScreenManager = screenMgr;
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.HudScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.HoverTextScreen.gameObject, null, GameScreenManager.UIRenderTarget.HoverTextScreen);
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.ToolTipScreen.gameObject, null, GameScreenManager.UIRenderTarget.HoverTextScreen);
		cameraController = Util.KInstantiate(cameraControllerPrefab, null, null).GetComponent<CameraController>();
		component.CameraController = cameraController;
		KInputHandler.Add(Global.Instance.GetInputManager().GetDefaultController(), cameraController, 1);
		playerController = component.GetComponent<PlayerController>();
		KInputHandler.Add(Global.Instance.GetInputManager().GetDefaultController(), playerController, 20);
		return component;
	}

	public void SetForceField(int cell, bool force_field, bool solid)
	{
		Grid.ForceField[cell] = force_field;
		gameSolidInfo.Add(new SolidInfo(cell, solid));
	}

	private unsafe Sim.GameDataUpdate* StepTheSim(float dt)
	{
		using (new KProfiler.Region("StepTheSim", null))
		{
			IntPtr intPtr = IntPtr.Zero;
			using (new KProfiler.Region("WaitingForSim", null))
			{
				if (Grid.Visible == null || Grid.Visible.Length == 0)
				{
					Output.LogError("Invalid Grid.Visible, what have you done?!");
					return null;
				}
				intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, Grid.Visible.Length, Grid.Visible);
			}
			if (!(intPtr == IntPtr.Zero))
			{
				Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
				Grid.elementIdx = ptr->elementIdx;
				Grid.temperature = ptr->temperature;
				Grid.mass = ptr->mass;
				Grid.properties = ptr->properties;
				Grid.strengthInfo = ptr->strengthInfo;
				Grid.insulation = ptr->insulation;
				Grid.diseaseIdx = ptr->diseaseIdx;
				Grid.diseaseCount = ptr->diseaseCount;
				Grid.AccumulatedFlowValues = ptr->accumulatedFlow;
				Grid.exposedToSunlight = (byte*)(void*)ptr->propertyTextureExposedToSunlight;
				PropertyTextures.externalFlowTex = ptr->propertyTextureFlow;
				PropertyTextures.externalLiquidTex = ptr->propertyTextureLiquid;
				PropertyTextures.externalExposedToSunlight = ptr->propertyTextureExposedToSunlight;
				List<Element> elements = ElementLoader.elements;
				simData.emittedMassEntries = ptr->emittedMassEntries;
				simData.elementChunks = ptr->elementChunkInfos;
				simData.buildingTemperatures = ptr->buildingTemperatures;
				simData.diseaseEmittedInfos = ptr->diseaseEmittedInfos;
				simData.diseaseConsumedInfos = ptr->diseaseConsumedInfos;
				for (int i = 0; i < ptr->numSubstanceChangeInfo; i++)
				{
					Sim.SubstanceChangeInfo substanceChangeInfo = ptr->substanceChangeInfo[i];
					Element element = elements[substanceChangeInfo.newElemIdx];
					Grid.Element[substanceChangeInfo.cellIdx] = element;
				}
				for (int j = 0; j < ptr->numSolidInfo; j++)
				{
					Sim.SolidInfo solidInfo = ptr->solidInfo[j];
					if (!solidChangedFilter.Contains(solidInfo.cellIdx))
					{
						this.solidInfo.Add(new SolidInfo(solidInfo.cellIdx, solidInfo.isSolid != 0));
						Grid.PreviousSolid[solidInfo.cellIdx] = Grid.Solid[solidInfo.cellIdx];
						bool solid = solidInfo.isSolid != 0;
						Grid.SetSolid(solidInfo.cellIdx, solid, CellEventLogger.Instance.SimMessagesSolid);
					}
				}
				for (int k = 0; k < ptr->numCallbackInfo; k++)
				{
					Sim.CallbackInfo callbackInfo = ptr->callbackInfo[k];
					HandleVector<CallbackInfo>.Handle handle = default(HandleVector<CallbackInfo>.Handle);
					handle.index = callbackInfo.callbackIdx;
					HandleVector<CallbackInfo>.Handle h = handle;
					this.callbackInfo.Add(new Klei.CallbackInfo(h));
				}
				int numSpawnFallingLiquidInfo = ptr->numSpawnFallingLiquidInfo;
				for (int l = 0; l < numSpawnFallingLiquidInfo; l++)
				{
					Sim.SpawnFallingLiquidInfo spawnFallingLiquidInfo = ptr->spawnFallingLiquidInfo[l];
					FallingWater.instance.AddParticle(spawnFallingLiquidInfo.cellIdx, spawnFallingLiquidInfo.elemIdx, spawnFallingLiquidInfo.mass, spawnFallingLiquidInfo.temperature, spawnFallingLiquidInfo.diseaseIdx, spawnFallingLiquidInfo.diseaseCount, false, false, false, false);
				}
				int numDigInfo = ptr->numDigInfo;
				WorldDamage component = world.GetComponent<WorldDamage>();
				for (int m = 0; m < numDigInfo; m++)
				{
					Sim.SpawnOreInfo spawnOreInfo = ptr->digInfo[m];
					if (spawnOreInfo.temperature <= 0f && spawnOreInfo.mass > 0f)
					{
						Output.LogError("Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this....");
					}
					component.OnDigComplete(spawnOreInfo.cellIdx, spawnOreInfo.mass, spawnOreInfo.temperature, spawnOreInfo.elemIdx, spawnOreInfo.diseaseIdx, spawnOreInfo.diseaseCount);
				}
				int numSpawnOreInfo = ptr->numSpawnOreInfo;
				for (int n = 0; n < numSpawnOreInfo; n++)
				{
					Sim.SpawnOreInfo spawnOreInfo2 = ptr->spawnOreInfo[n];
					Vector3 position = Grid.CellToPosCCC(spawnOreInfo2.cellIdx, Grid.SceneLayer.Ore);
					Element element2 = ElementLoader.elements[spawnOreInfo2.elemIdx];
					if (spawnOreInfo2.temperature <= 0f && spawnOreInfo2.mass > 0f)
					{
						Output.LogError("Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this....");
					}
					element2.substance.SpawnResource(position, spawnOreInfo2.mass, spawnOreInfo2.temperature, spawnOreInfo2.diseaseIdx, spawnOreInfo2.diseaseCount, false, false);
				}
				int numSpawnFXInfo = ptr->numSpawnFXInfo;
				for (int num = 0; num < numSpawnFXInfo; num++)
				{
					Sim.SpawnFXInfo spawnFXInfo = ptr->spawnFXInfo[num];
					SpawnFX((SpawnFXHashes)spawnFXInfo.fxHash, spawnFXInfo.cellIdx, spawnFXInfo.rotation);
				}
				UnstableGroundManager component2 = world.GetComponent<UnstableGroundManager>();
				int numUnstableCellInfo = ptr->numUnstableCellInfo;
				for (int num2 = 0; num2 < numUnstableCellInfo; num2++)
				{
					Sim.UnstableCellInfo unstableCellInfo = ptr->unstableCellInfo[num2];
					if (unstableCellInfo.fallingInfo == 0)
					{
						component2.Spawn(unstableCellInfo.cellIdx, ElementLoader.elements[unstableCellInfo.elemIdx], unstableCellInfo.mass, unstableCellInfo.temperature, unstableCellInfo.diseaseIdx, unstableCellInfo.diseaseCount);
					}
				}
				int numWorldDamageInfo = ptr->numWorldDamageInfo;
				for (int num3 = 0; num3 < numWorldDamageInfo; num3++)
				{
					Sim.WorldDamageInfo damage_info = ptr->worldDamageInfo[num3];
					WorldDamage.Instance.ApplyDamage(damage_info);
				}
				for (int num4 = 0; num4 < ptr->numRemovedMassEntries; num4++)
				{
					Sim.ConsumedMassInfo consumed_info = ptr->removedMassEntries[num4];
					ElementConsumer.AddMass(consumed_info);
				}
				int numMassConsumedCallbacks = ptr->numMassConsumedCallbacks;
				HandleVector<ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle2 = default(HandleVector<ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle);
				for (int num5 = 0; num5 < numMassConsumedCallbacks; num5++)
				{
					Sim.MassConsumedCallback arg = ptr->massConsumedCallbacks[num5];
					handle2.index = arg.callbackIdx;
					ComplexCallbackInfo<Sim.MassConsumedCallback> complexCallbackInfo = massConsumedCallbackManager.Release(handle2, "massConsumedCB");
					if (complexCallbackInfo.cb != null)
					{
						complexCallbackInfo.cb(arg, complexCallbackInfo.callbackData);
					}
				}
				int numMassEmittedCallbacks = ptr->numMassEmittedCallbacks;
				HandleVector<ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle handle3 = default(HandleVector<ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle);
				for (int num6 = 0; num6 < numMassEmittedCallbacks; num6++)
				{
					Sim.MassEmittedCallback arg2 = ptr->massEmittedCallbacks[num6];
					handle3.index = arg2.callbackIdx;
					if (massEmitCallbackManager.IsVersionValid(handle3))
					{
						ComplexCallbackInfo<Sim.MassEmittedCallback> item = massEmitCallbackManager.GetItem(handle3);
						if (item.cb != null)
						{
							item.cb(arg2, item.callbackData);
						}
					}
				}
				int numDiseaseConsumptionCallbacks = ptr->numDiseaseConsumptionCallbacks;
				HandleVector<ComplexCallbackInfo<Sim.DiseaseConsumptionCallback>>.Handle handle4 = default(HandleVector<ComplexCallbackInfo<Sim.DiseaseConsumptionCallback>>.Handle);
				for (int num7 = 0; num7 < numDiseaseConsumptionCallbacks; num7++)
				{
					Sim.DiseaseConsumptionCallback arg3 = ptr->diseaseConsumptionCallbacks[num7];
					handle4.index = arg3.callbackIdx;
					if (diseaseConsumptionCallbackManager.IsVersionValid(handle4))
					{
						ComplexCallbackInfo<Sim.DiseaseConsumptionCallback> item2 = diseaseConsumptionCallbackManager.GetItem(handle4);
						if (item2.cb != null)
						{
							item2.cb(arg3, item2.callbackData);
						}
					}
				}
				int numComponentStateChangedMessages = ptr->numComponentStateChangedMessages;
				HandleVector<ComplexCallbackInfo<int>>.Handle handle5 = default(HandleVector<ComplexCallbackInfo<int>>.Handle);
				for (int num8 = 0; num8 < numComponentStateChangedMessages; num8++)
				{
					Sim.ComponentStateChangedMessage componentStateChangedMessage = ptr->componentStateChangedMessages[num8];
					handle5.index = componentStateChangedMessage.callbackIdx;
					if (simComponentCallbackManager.IsVersionValid(handle5))
					{
						ComplexCallbackInfo<int> complexCallbackInfo2 = simComponentCallbackManager.Release(handle5, "component state changed cb");
						if (complexCallbackInfo2.cb != null)
						{
							complexCallbackInfo2.cb(componentStateChangedMessage.simHandle, complexCallbackInfo2.callbackData);
						}
					}
				}
				int numElementChunkMeltedInfos = ptr->numElementChunkMeltedInfos;
				for (int num9 = 0; num9 < numElementChunkMeltedInfos; num9++)
				{
					Sim.MeltedInfo meltedInfo = ptr->elementChunkMeltedInfos[num9];
					SimTemperatureTransfer.DoStateTransition(meltedInfo.handle);
				}
				int numBuildingOverheatInfos = ptr->numBuildingOverheatInfos;
				for (int num10 = 0; num10 < numBuildingOverheatInfos; num10++)
				{
					Sim.MeltedInfo meltedInfo2 = ptr->buildingOverheatInfos[num10];
					StructureTemperatureComponents.DoOverheat(meltedInfo2.handle);
				}
				int numBuildingNoLongerOverheatedInfos = ptr->numBuildingNoLongerOverheatedInfos;
				for (int num11 = 0; num11 < numBuildingNoLongerOverheatedInfos; num11++)
				{
					Sim.MeltedInfo meltedInfo3 = ptr->buildingNoLongerOverheatedInfos[num11];
					StructureTemperatureComponents.DoNoLongerOverheated(meltedInfo3.handle);
				}
				int numBuildingMeltedInfos = ptr->numBuildingMeltedInfos;
				for (int num12 = 0; num12 < numBuildingMeltedInfos; num12++)
				{
					Sim.MeltedInfo meltedInfo4 = ptr->buildingMeltedInfos[num12];
					StructureTemperatureComponents.DoStateTransition(meltedInfo4.handle);
				}
				int numCellMeltedInfos = ptr->numCellMeltedInfos;
				for (int num13 = 0; num13 < numCellMeltedInfos; num13++)
				{
					Sim.CellMeltedInfo cellMeltedInfo = ptr->cellMeltedInfos[num13];
					int gameCell = cellMeltedInfo.gameCell;
					GameObject gameObject = Grid.Objects[gameCell, 9];
					if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
					{
						Util.KDestroyGameObject(gameObject);
					}
				}
				if (dt > 0f)
				{
					conduitTemperatureManager.Sim200ms(0.2f);
					conduitDiseaseManager.Sim200ms(0.2f);
					gasConduitFlow.Sim200ms(0.2f);
					liquidConduitFlow.Sim200ms(0.2f);
					solidConduitFlow.Sim200ms(0.2f);
					accumulators.Sim200ms(0.2f);
					plantElementAbsorbers.Sim200ms(0.2f);
				}
				Sim.DebugProperties debugProperties = default(Sim.DebugProperties);
				debugProperties.buildingTemperatureScale = 100f;
				debugProperties.contaminatedOxygenEmitProbability = 0.001f;
				debugProperties.contaminatedOxygenConversionPercent = 0.001f;
				debugProperties.biomeTemperatureLerpRate = 0.001f;
				debugProperties.isDebugEditing = (byte)(((UnityEngine.Object)DebugPaintElementScreen.Instance != (UnityEngine.Object)null && DebugPaintElementScreen.Instance.gameObject.activeSelf) ? 1 : 0);
				debugProperties.pad0 = (debugProperties.pad1 = (debugProperties.pad2 = 0));
				SimMessages.SetDebugProperties(debugProperties);
				if (dt > 0f)
				{
					if (circuitManager != null)
					{
						circuitManager.Sim200msFirst(dt);
					}
					if (energySim != null)
					{
						energySim.EnergySim200ms(dt);
					}
					if (logicCircuitManager != null)
					{
						logicCircuitManager.Sim200ms(dt);
					}
					if (circuitManager != null)
					{
						circuitManager.Sim200msLast(dt);
					}
				}
				return ptr;
			}
			return null;
		}
	}

	public void AddSolidChangedFilter(int cell)
	{
		solidChangedFilter.Add(cell);
	}

	public void RemoveSolidChangedFilter(int cell)
	{
		solidChangedFilter.Remove(cell);
	}

	public void UpdateGameActiveRegion(int x0, int y0, int x1, int y1)
	{
		simActiveRegionMin.x = Mathf.Max(0, Mathf.Min(x0, simActiveRegionMin.x));
		simActiveRegionMin.y = Mathf.Max(0, Mathf.Min(y0, simActiveRegionMin.y));
		simActiveRegionMax.x = Mathf.Min(Grid.WidthInCells - 1, Mathf.Max(x1, simActiveRegionMax.x));
		simActiveRegionMax.y = Mathf.Min(Grid.HeightInCells - 1, Mathf.Max(y1, simActiveRegionMax.y));
	}

	public void SetIsLoading()
	{
		isLoading = true;
	}

	public bool IsLoading()
	{
		return isLoading;
	}

	private void ShowDebugCellInfo()
	{
		int mouseCell = DebugHandler.GetMouseCell();
		int x = 0;
		int y = 0;
		Grid.CellToXY(mouseCell, out x, out y);
		string text = mouseCell.ToString() + " (" + x + ", " + y + ")";
		DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
	}

	public void ForceSimStep()
	{
		Output.Log("Force-stepping the sim");
		simDt = 0.2f;
	}

	private void Update()
	{
		if (!isLoading)
		{
			float deltaTime = Time.deltaTime;
			if (Debug.developerConsoleVisible)
			{
				Debug.developerConsoleVisible = false;
			}
			if (DebugHandler.DebugCellInfo)
			{
				ShowDebugCellInfo();
			}
			gasConduitSystem.Update();
			liquidConduitSystem.Update();
			solidConduitSystem.Update();
			circuitManager.RenderEveryTick(deltaTime);
			logicCircuitManager.RenderEveryTick(deltaTime);
			solidConduitFlow.RenderEveryTick(deltaTime);
			if (forceActiveArea)
			{
				simActiveRegionMin = new Vector2I((int)Mathf.Max(0f, minForcedActiveArea.x), (int)Mathf.Max(0f, minForcedActiveArea.y));
				simActiveRegionMax = new Vector2I((int)Mathf.Min((float)(Grid.WidthInCells - 1), maxForcedActiveArea.x), (int)Mathf.Min((float)(Grid.HeightInCells - 1), maxForcedActiveArea.y));
			}
			simActiveRegionMin = new Vector2I(0, 0);
			simActiveRegionMax = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			Pathfinding.Instance.RenderEveryTick();
			Singleton<CellChangeMonitor>.Instance.RenderEveryTick();
			SimEveryTick(deltaTime);
		}
	}

	private void SimEveryTick(float dt)
	{
		dt = Mathf.Min(dt, 0.2f);
		simDt += dt;
		if (simDt >= 0.0166666675f)
		{
			while (simDt >= 0.0166666675f)
			{
				simSubTick++;
				simSubTick %= 12;
				if (simSubTick == 0)
				{
					hasFirstSimTickRun = true;
					UnsafeSim200ms(0.2f);
				}
				if (hasFirstSimTickRun)
				{
					Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
				}
				simDt -= 0.0166666675f;
			}
		}
		else
		{
			UnsafeSim200ms(0f);
		}
	}

	private unsafe void UnsafeSim200ms(float dt)
	{
		SimMessages.NewGameFrame(dt, simActiveRegionMin, simActiveRegionMax);
		Sim.GameDataUpdate* ptr = StepTheSim(dt);
		if (ptr == null)
		{
			Debug.LogError("UNEXPECTED!", null);
		}
		else if (ptr->numFramesProcessed > 0)
		{
			gameSolidInfo.AddRange(solidInfo);
			world.UpdateCellInfo(gameSolidInfo, callbackInfo, ptr->numSolidSubstanceChangeInfo, ptr->solidSubstanceChangeInfo, ptr->numLiquidChangeInfo, ptr->liquidChangeInfo);
			gameSolidInfo.Clear();
			solidInfo.Clear();
			callbackInfo.Clear();
			Pathfinding.Instance.UpdateNavGrids(false);
		}
	}

	private void LateUpdateComponents()
	{
		if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null)
		{
			HashedString mode = OverlayScreen.Instance.GetMode();
			foreach (BuildingCellVisualizer item in Components.BuildingCellVisualizers.Items)
			{
				item.Tick(mode);
			}
		}
	}

	public void ForceOverlayUpdate()
	{
		previousOverlayMode = OverlayModes.None.ID;
	}

	private void LateUpdate()
	{
		if (Time.timeScale == 0f && !IsPaused)
		{
			IsPaused = true;
			Trigger(-1788536802, IsPaused);
		}
		else if (Time.timeScale != 0f && IsPaused)
		{
			IsPaused = false;
			Trigger(-1788536802, IsPaused);
		}
		if (Input.GetMouseButton(0))
		{
			VisualTunerElement = null;
			int mouseCell = DebugHandler.GetMouseCell();
			if (Grid.IsValidCell(mouseCell))
			{
				Element element = VisualTunerElement = Grid.Element[mouseCell];
			}
		}
		gasConduitSystem.Update();
		liquidConduitSystem.Update();
		solidConduitSystem.Update();
		HashedString mode = SimDebugView.Instance.GetMode();
		if (mode != previousOverlayMode)
		{
			previousOverlayMode = mode;
			if (mode == OverlayModes.LiquidConduits.ID)
			{
				liquidFlowVisualizer.ColourizePipeContents(true, true);
				gasFlowVisualizer.ColourizePipeContents(false, true);
				solidFlowVisualizer.ColourizePipeContents(false, true);
			}
			else if (mode == OverlayModes.GasConduits.ID)
			{
				liquidFlowVisualizer.ColourizePipeContents(false, true);
				gasFlowVisualizer.ColourizePipeContents(true, true);
				solidFlowVisualizer.ColourizePipeContents(false, true);
			}
			else if (mode == OverlayModes.SolidConveyor.ID)
			{
				liquidFlowVisualizer.ColourizePipeContents(false, true);
				gasFlowVisualizer.ColourizePipeContents(false, true);
				solidFlowVisualizer.ColourizePipeContents(true, true);
			}
			else
			{
				liquidFlowVisualizer.ColourizePipeContents(false, false);
				gasFlowVisualizer.ColourizePipeContents(false, false);
				solidFlowVisualizer.ColourizePipeContents(false, false);
			}
		}
		gasFlowVisualizer.Render(gasFlowPos.z, 0, gasConduitFlow.ContinuousLerpPercent, mode == OverlayModes.GasConduits.ID && gasConduitFlow.DiscreteLerpPercent != previousGasConduitFlowDiscreteLerpPercent);
		liquidFlowVisualizer.Render(liquidFlowPos.z, 0, liquidConduitFlow.ContinuousLerpPercent, mode == OverlayModes.LiquidConduits.ID && liquidConduitFlow.DiscreteLerpPercent != previousLiquidConduitFlowDiscreteLerpPercent);
		solidFlowVisualizer.Render(solidFlowPos.z, 0, solidConduitFlow.ContinuousLerpPercent, mode == OverlayModes.SolidConveyor.ID && solidConduitFlow.DiscreteLerpPercent != previousSolidConduitFlowDiscreteLerpPercent);
		previousGasConduitFlowDiscreteLerpPercent = ((!(mode == OverlayModes.GasConduits.ID)) ? (-1f) : gasConduitFlow.DiscreteLerpPercent);
		previousLiquidConduitFlowDiscreteLerpPercent = ((!(mode == OverlayModes.LiquidConduits.ID)) ? (-1f) : liquidConduitFlow.DiscreteLerpPercent);
		previousSolidConduitFlowDiscreteLerpPercent = ((!(mode == OverlayModes.SolidConveyor.ID)) ? (-1f) : solidConduitFlow.DiscreteLerpPercent);
		Camera main = Camera.main;
		Vector3 position = Camera.main.transform.GetPosition();
		Vector3 vector = main.ViewportToWorldPoint(new Vector3(1f, 1f, position.z));
		Camera main2 = Camera.main;
		Vector3 position2 = Camera.main.transform.GetPosition();
		Vector3 vector2 = main2.ViewportToWorldPoint(new Vector3(0f, 0f, position2.z));
		Shader.SetGlobalVector("_WsToCs", new Vector4(vector.x / (float)Grid.WidthInCells, vector.y / (float)Grid.HeightInCells, (vector2.x - vector.x) / (float)Grid.WidthInCells, (vector2.y - vector.y) / (float)Grid.HeightInCells));
		if (drawStatusItems)
		{
			statusItemRenderer.RenderEveryTick();
			prioritizableRenderer.RenderEveryTick();
		}
		LateUpdateComponents();
		Singleton<StateMachineUpdater>.Instance.Render(Time.unscaledDeltaTime);
		Singleton<StateMachineUpdater>.Instance.RenderEveryTick(Time.unscaledDeltaTime);
		if ((UnityEngine.Object)SelectTool.Instance != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.selected != (UnityEngine.Object)null)
		{
			Navigator component = SelectTool.Instance.selected.GetComponent<Navigator>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.DrawPath();
			}
		}
		KFMOD.RenderEveryTick(Time.deltaTime);
		if (GenericGameSettings.instance.developerDebugEnable)
		{
			UpdateGCProfileCapture();
		}
	}

	private void UpdateGCProfileCapture()
	{
		if (GenericGameSettings.instance.developerCaptureGCStatsTime != 0f)
		{
			if (IsPaused && (UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
			{
				SpeedControlScreen.Instance.Unpause(true);
			}
			if (!(Time.timeSinceLevelLoad < GenericGameSettings.instance.developerCaptureGCStatsTime))
			{
				float fPS = Global.Instance.GetComponent<PerformanceMonitor>().FPS;
				Debug.Log("Begin GC profiling...", null);
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				GC.Collect();
				float num = Time.realtimeSinceStartup - realtimeSinceStartup;
				Debug.Log("\tGC.Collect() took " + num.ToString() + " seconds", null);
				uint num2 = 299241u;
				string text = System.DateTime.Now.ToShortDateString();
				string text2 = System.DateTime.Now.ToShortTimeString();
				string fileName = Path.GetFileName(SaveLoader.GetLatestSaveFile());
				string text3 = "Version,Date,Time,SaveGame";
				string text4 = $"{num2},{text},{text2},{fileName}";
				using (StreamWriter streamWriter = new StreamWriter("./memory/GeneralMetrics.csv"))
				{
					string format = "{0},{1},{2}";
					streamWriter.WriteLine(string.Format(format, text3, "GCDuration", "FPS"));
					streamWriter.WriteLine(string.Format(format, text4, num, fPS));
				}
				MemorySnapshot memorySnapshot = new MemorySnapshot();
				using (StreamWriter streamWriter2 = new StreamWriter("./memory/GCTypeMetrics.csv"))
				{
					string format2 = "{0},{1},{2},{3}";
					streamWriter2.WriteLine(string.Format(format2, text3, "Type", "Instances", "References"));
					foreach (MemorySnapshot.TypeData value in memorySnapshot.types.Values)
					{
						streamWriter2.WriteLine(string.Format(format2, text4, "\"" + value.type.ToString() + "\"", value.instanceCount, value.refCount));
					}
				}
				GenericGameSettings.instance.developerCaptureGCStatsTime = 0f;
				Debug.Log("...end GC profiling", null);
				Application.Quit();
			}
		}
	}

	public void Reset(GameSpawnData gsd)
	{
		using (new KProfiler.Region("World.Reset", null))
		{
			if (gsd != null)
			{
				foreach (KeyValuePair<Vector2I, bool> item in gsd.preventFoWReveal)
				{
					if (item.Value)
					{
						Grid.PreventFogOfWarReveal[Grid.PosToCell(item.Key)] = item.Value;
					}
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		quitting = true;
		Sim.Shutdown();
		AudioMixer.Destroy();
		if ((UnityEngine.Object)screenMgr != (UnityEngine.Object)null && (UnityEngine.Object)screenMgr.gameObject != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(screenMgr.gameObject);
		}
		Console.WriteLine("Game.OnApplicationQuit()");
	}

	private void InitializeFXSpawners()
	{
		for (int i = 0; i < fxSpawnData.Length; i++)
		{
			int fx_idx = i;
			fxSpawnData[fx_idx].fxPrefab.SetActive(false);
			ushort fx_mask = (ushort)(1 << fx_idx);
			Action<SpawnFXHashes, GameObject> destroyer = delegate(SpawnFXHashes fxid, GameObject go)
			{
				if (!IsQuitting())
				{
					int num3 = Grid.PosToCell(go);
					activeFX[num3] &= (ushort)(~fx_mask);
					go.GetComponent<KAnimControllerBase>().enabled = false;
					fxPools[(int)fxid].ReleaseInstance(go);
				}
			};
			Func<GameObject> instantiator = delegate
			{
				GameObject gameObject = GameUtil.KInstantiate(fxSpawnData[fx_idx].fxPrefab, Grid.SceneLayer.Front, null, 0);
				KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
				component2.enabled = false;
				gameObject.SetActive(true);
				component2.onDestroySelf = delegate(GameObject go)
				{
					destroyer(fxSpawnData[fx_idx].id, go);
				};
				return gameObject;
			};
			ObjectPool pool = new ObjectPool(instantiator, fxSpawnData[fx_idx].initialCount);
			fxPools[(int)fxSpawnData[fx_idx].id] = pool;
			fxSpawner[(int)fxSpawnData[fx_idx].id] = delegate(Vector3 pos, float rotation)
			{
				GameScheduler.Instance.Schedule("SpawnFX", 0f, delegate
				{
					int num = Grid.PosToCell(pos);
					if ((activeFX[num] & fx_mask) == 0)
					{
						activeFX[num] |= fx_mask;
						GameObject instance = pool.GetInstance();
						SpawnPoolData spawnPoolData = fxSpawnData[fx_idx];
						Quaternion rotation2 = Quaternion.identity;
						bool flipX = false;
						string s = spawnPoolData.initialAnim;
						switch (spawnPoolData.rotationConfig)
						{
						case SpawnRotationConfig.Normal:
							rotation2 = Quaternion.Euler(0f, 0f, rotation);
							break;
						case SpawnRotationConfig.StringName:
						{
							int num2 = (int)(rotation / 90f);
							if (num2 < 0)
							{
								num2 += spawnPoolData.rotationData.Length;
							}
							s = spawnPoolData.rotationData[num2].animName;
							flipX = spawnPoolData.rotationData[num2].flip;
							break;
						}
						}
						pos += spawnPoolData.spawnOffset;
						Vector2 v = UnityEngine.Random.insideUnitCircle;
						v.x *= spawnPoolData.spawnRandomOffset.x;
						v.y *= spawnPoolData.spawnRandomOffset.y;
						v = rotation2 * (Vector3)v;
						pos.x += v.x;
						pos.y += v.y;
						instance.transform.SetPosition(pos);
						instance.transform.rotation = rotation2;
						KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
						component.FlipX = flipX;
						component.TintColour = spawnPoolData.colour;
						component.Play(s, KAnim.PlayMode.Once, 1f, 0f);
						component.enabled = true;
					}
				}, null, null);
			};
		}
	}

	public void SpawnFX(SpawnFXHashes fx_id, int cell, float rotation)
	{
		Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Front);
		if (CameraController.Instance.IsVisiblePos(vector))
		{
			fxSpawner[(int)fx_id](vector, rotation);
		}
	}

	public void SpawnFX(SpawnFXHashes fx_id, Vector3 pos, float rotation)
	{
		fxSpawner[(int)fx_id](pos, rotation);
	}

	public static void SaveSettings(BinaryWriter writer)
	{
		Settings obj = new Settings(Instance);
		Serializer.Serialize(obj, writer);
	}

	public static void LoadSettings(Deserializer deserializer)
	{
		Settings settings = new Settings();
		deserializer.Deserialize(settings);
		KPlayerPrefs.SetInt(NextUniqueIDKey, settings.nextUniqueID);
		KleiMetrics.SetGameID(settings.gameID);
	}

	public void Save(BinaryWriter writer)
	{
		GameSaveData gameSaveData = new GameSaveData();
		gameSaveData.gasConduitFlow = gasConduitFlow;
		gameSaveData.liquidConduitFlow = liquidConduitFlow;
		gameSaveData.simActiveRegionMin = simActiveRegionMin;
		gameSaveData.simActiveRegionMax = simActiveRegionMax;
		gameSaveData.fallingWater = world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = SaveLoader.Instance.worldDetailSave;
		gameSaveData.debugWasUsed = debugWasUsed;
		gameSaveData.customGameSettings = CustomGameSettings.Instance;
		gameSaveData.autoPrioritizeRoles = autoPrioritizeRoles;
		gameSaveData.advancedPersonalPriorities = advancedPersonalPriorities;
		gameSaveData.savedInfo = savedInfo;
		if (OnSave != null)
		{
			OnSave(gameSaveData);
		}
		Serializer.Serialize(gameSaveData, writer);
	}

	public void Load(Deserializer deserializer)
	{
		GameSaveData gameSaveData = new GameSaveData();
		gameSaveData.gasConduitFlow = gasConduitFlow;
		gameSaveData.liquidConduitFlow = liquidConduitFlow;
		gameSaveData.simActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		gameSaveData.simActiveRegionMax = new Vector2I(0, 0);
		gameSaveData.fallingWater = world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = new WorldDetailSave();
		gameSaveData.customGameSettings = CustomGameSettings.Instance;
		deserializer.Deserialize(gameSaveData);
		gasConduitFlow = gameSaveData.gasConduitFlow;
		liquidConduitFlow = gameSaveData.liquidConduitFlow;
		simActiveRegionMin = gameSaveData.simActiveRegionMin;
		simActiveRegionMax = gameSaveData.simActiveRegionMax;
		debugWasUsed = gameSaveData.debugWasUsed;
		autoPrioritizeRoles = gameSaveData.autoPrioritizeRoles;
		advancedPersonalPriorities = gameSaveData.advancedPersonalPriorities;
		savedInfo = gameSaveData.savedInfo;
		CustomGameSettings.Instance.Print();
		KCrashReporter.debugWasUsed = debugWasUsed;
		SaveLoader.Instance.SetWorldDetail(gameSaveData.worldDetail);
		if (OnLoad != null)
		{
			OnLoad(gameSaveData);
		}
	}

	public void SetAutoSaveCallbacks(SavingPreCB activatePreCB, SavingActiveCB activateActiveCB, SavingPostCB activatePostCB)
	{
		this.activatePreCB = activatePreCB;
		this.activateActiveCB = activateActiveCB;
		this.activatePostCB = activatePostCB;
	}

	public void StartDelayedInitialSave()
	{
		StartCoroutine(DelayedInitialSave());
	}

	private IEnumerator DelayedInitialSave()
	{
		int i = 0;
		if (i < 1)
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		SaveLoader.Instance.InitialSave();
	}

	public void StartDelayedSave(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		if (activatePreCB != null)
		{
			activatePreCB(delegate
			{
				StartCoroutine(DelayedSave(filename, isAutoSave, updateSavePointer));
			});
		}
		else
		{
			StartCoroutine(DelayedSave(filename, isAutoSave, updateSavePointer));
		}
	}

	private IEnumerator DelayedSave(string filename, bool isAutoSave, bool updateSavePointer)
	{
		int j = 0;
		if (j < 1)
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		if (activateActiveCB != null)
		{
			activateActiveCB();
			int i = 0;
			if (i < 1)
			{
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}
		SaveLoader.Instance.Save(filename, isAutoSave, updateSavePointer);
		if (activatePostCB != null)
		{
			activatePostCB();
		}
	}

	public void StartDelayed(int tick_delay, System.Action action)
	{
		StartCoroutine(DelayedExecutor(tick_delay, action));
	}

	private IEnumerator DelayedExecutor(int tick_delay, System.Action action)
	{
		int i = 0;
		if (i < tick_delay)
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		action();
	}

	private void LoadEventHashes()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(GameHashes)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				GameHashes hash = (GameHashes)enumerator.Current;
				HashCache.Get().Add((int)hash, hash.ToString());
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		IEnumerator enumerator2 = Enum.GetValues(typeof(UtilHashes)).GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				UtilHashes hash2 = (UtilHashes)enumerator2.Current;
				HashCache.Get().Add((int)hash2, hash2.ToString());
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = (enumerator2 as IDisposable)) != null)
			{
				disposable2.Dispose();
			}
		}
		IEnumerator enumerator3 = Enum.GetValues(typeof(UIHashes)).GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				UIHashes hash3 = (UIHashes)enumerator3.Current;
				HashCache.Get().Add((int)hash3, hash3.ToString());
			}
		}
		finally
		{
			IDisposable disposable3;
			if ((disposable3 = (enumerator3 as IDisposable)) != null)
			{
				disposable3.Dispose();
			}
		}
	}

	public void StopFE()
	{
		if ((bool)SteamUGCService.Instance)
		{
			SteamUGCService.Instance.enabled = false;
		}
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, STOP_MODE.ALLOWFADEOUT);
		if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		}
		if (MusicManager.instance.SongIsPlaying("Music_TitleTheme"))
		{
			MusicManager.instance.StopSong("Music_TitleTheme", true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void StartBE()
	{
		Resources.UnloadUnusedAssets();
		if ((UnityEngine.Object)TimeOfDay.Instance != (UnityEngine.Object)null && !MusicManager.instance.SongIsPlaying("Stinger_Loop_Night") && TimeOfDay.Instance.GetCurrentTimeRegion() == TimeOfDay.TimeRegion.Night)
		{
			MusicManager.instance.PlaySong("Stinger_Loop_Night", false);
			MusicManager.instance.SetSongParameter("Stinger_Loop_Night", "Music_PlayStinger", 0f, true);
		}
		AudioMixer.instance.Reset();
		AudioMixer.instance.StartPersistentSnapshots();
		if (MusicManager.instance.ShouldPlayDynamicMusicLoadedGame())
		{
			MusicManager.instance.PlayDynamicMusic();
		}
	}

	public void StopBE()
	{
		if ((bool)SteamUGCService.Instance)
		{
			SteamUGCService.Instance.enabled = true;
		}
		LoopingSoundManager loopingSoundManager = LoopingSoundManager.Get();
		if ((UnityEngine.Object)loopingSoundManager != (UnityEngine.Object)null)
		{
			loopingSoundManager.StopAllSounds();
		}
		MusicManager.instance.KillAllSongs(STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.StopPersistentSnapshots();
		Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
		foreach (List<SaveLoadRoot> value in lists.Values)
		{
			foreach (SaveLoadRoot item in value)
			{
				if ((UnityEngine.Object)item.gameObject != (UnityEngine.Object)null)
				{
					Util.KDestroyGameObject(item.gameObject);
				}
			}
		}
		GetComponent<EntombedItemVisualizer>().Clear();
		SimTemperatureTransfer.ClearInstanceMap();
		StructureTemperatureComponents.ClearInstanceMap();
		ElementConsumer.ClearInstanceMap();
		KComponentSpawn.instance.comps.Clear();
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), cameraController);
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), playerController);
		Sim.Shutdown();
		SimAndRenderScheduler.instance.Reset();
		Resources.UnloadUnusedAssets();
	}

	public void SetStatusItemOffset(Transform transform, Vector3 offset)
	{
		statusItemRenderer.SetOffset(transform, offset);
	}

	public void AddStatusItem(Transform transform, StatusItem status_item)
	{
		statusItemRenderer.Add(transform, status_item);
	}

	public void RemoveStatusItem(Transform transform, StatusItem status_item)
	{
		statusItemRenderer.Remove(transform, status_item);
	}

	public void StartedWork()
	{
		lastTimeWorkStarted = Time.time;
	}

	private void SpawnOxygenBubbles(Vector3 position, float angle)
	{
	}

	[ContextMenu("Print")]
	private void Print()
	{
		Console.WriteLine("This is a console writeline test");
		Debug.Log("This is a debug log test", null);
	}

	private void DestroyInstances()
	{
		KMonoBehaviour.lastGameObject = null;
		KMonoBehaviour.lastObj = null;
		GridSettings.ClearGrid();
		StateMachineManager.ResetParameters();
		ChoreTable.Instance.ResetParameters();
		BubbleManager.DestroyInstance();
		AmbientSoundManager.Destroy();
		AutoDisinfectableManager.DestroyInstance();
		BuildMenu.DestroyInstance();
		CancelTool.DestroyInstance();
		ClearTool.DestroyInstance();
		ChoreGroupManager.DestroyInstance();
		CO2Manager.DestroyInstance();
		ConsumerManager.DestroyInstance();
		CopySettingsTool.DestroyInstance();
		DateTime.DestroyInstance();
		DebugBaseTemplateButton.DestroyInstance();
		DebugPaintElementScreen.DestroyInstance();
		DetailsScreen.DestroyInstance();
		DietManager.DestroyInstance();
		DebugText.DestroyInstance();
		FactionManager.DestroyInstance();
		EmptyPipeTool.DestroyInstance();
		FetchListStatusItemUpdater.DestroyInstance();
		FishOvercrowingManager.DestroyInstance();
		FallingWater.DestroyInstance();
		GridCompositor.DestroyInstance();
		Infrared.DestroyInstance();
		KPrefabIDTracker.DestroyInstance();
		ManagementMenu.DestroyInstance();
		MaterialNeeds.DestroyInstance();
		Messenger.DestroyInstance();
		LoopingSoundManager.DestroyInstance();
		MeterScreen.DestroyInstance();
		MinionGroupProber.DestroyInstance();
		NavPathDrawer.DestroyInstance();
		MinionIdentity.DestroyStatics();
		PathFinder.PathGrid = null;
		Pathfinding.DestroyInstance();
		PrebuildTool.DestroyInstance();
		PrioritizeTool.DestroyInstance();
		SelectTool.DestroyInstance();
		PopFXManager.DestroyInstance();
		ProgressBarsConfig.DestroyInstance();
		PropertyTextures.DestroyInstance();
		RationTracker.DestroyInstance();
		ReportManager.DestroyInstance();
		RedAlertManager.Instance.DestroyInstance();
		Research.DestroyInstance();
		RootMenu.DestroyInstance();
		SaveLoader.DestroyInstance();
		Scenario.DestroyInstance();
		SimDebugView.DestroyInstance();
		SpriteSheetAnimManager.DestroyInstance();
		ScheduleManager.DestroyInstance();
		Sounds.DestroyInstance();
		ToolMenu.DestroyInstance();
		WorldDamage.DestroyInstance();
		WaterCubes.DestroyInstance();
		WireBuildTool.DestroyInstance();
		VisibilityTester.DestroyInstance();
		Traces.DestroyInstance();
		TopLeftControlScreen.DestroyInstance();
		UtilityBuildTool.DestroyInstance();
		ReportScreen.DestroyInstance();
		ChorePreconditions.DestroyInstance();
		SandboxBrushTool.DestroyInstance();
		SandboxHeatTool.DestroyInstance();
		SandboxClearFloorTool.DestroyInstance();
		GameScreenManager.DestroyInstance();
		GameScheduler.DestroyInstance();
		NavigationReservations.DestroyInstance();
		Tutorial.DestroyInstance();
		CameraController.DestroyInstance();
		CellEventLogger.DestroyInstance();
		GameFlowManager.DestroyInstance();
		Immigration.DestroyInstance();
		BuildTool.DestroyInstance();
		DebugTool.DestroyInstance();
		DeconstructTool.DestroyInstance();
		DigTool.DestroyInstance();
		DisinfectTool.DestroyInstance();
		HarvestTool.DestroyInstance();
		MopTool.DestroyInstance();
		MoveToLocationTool.DestroyInstance();
		PlaceTool.DestroyInstance();
		SpacecraftManager.DestroyInstance();
		SandboxDestroyerTool.DestroyInstance();
		SandboxFOWTool.DestroyInstance();
		SandboxFloodTool.DestroyInstance();
		SandboxSprinkleTool.DestroyInstance();
		StampTool.DestroyInstance();
		OnDemandUpdater.DestroyInstance();
		HoverTextScreen.DestroyInstance();
		ImmigrantScreen.DestroyInstance();
		OverlayMenu.DestroyInstance();
		NameDisplayScreen.DestroyInstance();
		PlanScreen.DestroyInstance();
		ResourceCategoryScreen.DestroyInstance();
		ResourceRemainingDisplayScreen.DestroyInstance();
		SandboxToolParameterMenu.DestroyInstance();
		SpeedControlScreen.DestroyInstance();
		Vignette.DestroyInstance();
		PlayerController.DestroyInstance();
		NotificationScreen.DestroyInstance();
		BuildingCellVisualizerResources.DestroyInstance();
		PauseScreen.DestroyInstance();
		SaveLoadRoot.DestroyStatics();
		KTime.DestroyInstance();
		DemoTimer.DestroyInstance();
		UIScheduler.DestroyInstance();
		SaveGame.DestroyInstance();
		GameClock.DestroyInstance();
		TimeOfDay.DestroyInstance();
		DeserializeWarnings.DestroyInstance();
		UISounds.DestroyInstance();
		RenderTextureDestroyer.DestroyInstance();
		WorldInspector.DestroyStatics();
		LoadScreen.DestroyInstance();
		LoadingOverlay.DestroyInstance();
		SimAndRenderScheduler.DestroyInstance();
		Singleton<CellChangeMonitor>.DestroyInstance();
		Singleton<StateMachineManager>.Instance.Clear();
		Singleton<StateMachineUpdater>.Instance.Clear();
		UpdateObjectCountParameter.Clear();
		MaterialSelectionPanel.ClearStatics();
		StarmapScreen.DestroyInstance();
		SpacecraftManager.DestroyInstance();
		Instance = null;
		Grid.OnReveal = null;
		VisualTunerElement = null;
		Assets.ClearOnAddPrefab();
		KMonoBehaviour.lastGameObject = null;
		KMonoBehaviour.lastObj = null;
		GameComps gameComps = KComponentSpawn.instance.comps as GameComps;
		gameComps.Clear();
	}
}
