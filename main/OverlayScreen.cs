using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OverlayScreen : KMonoBehaviour
{
	private struct ModeInfo
	{
		public OverlayModes.Mode mode;
	}

	public static HashSet<Tag> WireIDs = new HashSet<Tag>();

	public static HashSet<Tag> GasVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> LiquidVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> HarvestableIDs = new HashSet<Tag>();

	public static HashSet<Tag> DiseaseIDs = new HashSet<Tag>();

	public static HashSet<Tag> SuitIDs = new HashSet<Tag>();

	public static HashSet<Tag> SolidConveyorIDs = new HashSet<Tag>();

	[EventRef]
	[SerializeField]
	public string techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	public static OverlayScreen Instance;

	[Header("Power")]
	[SerializeField]
	private Canvas powerLabelParent;

	[SerializeField]
	private LocText powerLabelPrefab;

	[SerializeField]
	private BatteryUI batUIPrefab;

	[SerializeField]
	private Vector3 powerLabelOffset;

	[SerializeField]
	private Vector3 batteryUIOffset;

	[SerializeField]
	private Vector3 batteryUITransformerOffset;

	[SerializeField]
	private Vector3 batteryUISmallTransformerOffset;

	[SerializeField]
	private Color consumerColour;

	[SerializeField]
	private Color generatorColour;

	[SerializeField]
	private Color buildingDisabledColour = Color.gray;

	[Header("Circuits")]
	[SerializeField]
	private Color32 circuitUnpoweredColour;

	[SerializeField]
	private Color32 circuitSafeColour;

	[SerializeField]
	private Color32 circuitStrainingColour;

	[Header("Crops")]
	[SerializeField]
	private GameObject harvestableNotificationPrefab;

	[Header("Disease")]
	[SerializeField]
	private GameObject diseaseOverlayPrefab;

	[Header("Suit")]
	[SerializeField]
	private GameObject suitOverlayPrefab;

	[Header("ToolTip")]
	[SerializeField]
	private TextStyleSetting TooltipHeader;

	[SerializeField]
	private TextStyleSetting TooltipDescription;

	[Header("Logic")]
	[SerializeField]
	private LogicModeUI logicModeUIPrefab;

	public Action<HashedString> OnOverlayChanged;

	private ModeInfo currentModeInfo;

	private Dictionary<HashedString, ModeInfo> modeInfos = new Dictionary<HashedString, ModeInfo>();

	public HashedString mode => currentModeInfo.mode.ViewMode();

	protected override void OnPrefabInit()
	{
		Debug.Assert((UnityEngine.Object)Instance == (UnityEngine.Object)null);
		Instance = this;
		powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
	}

	protected override void OnLoadLevel()
	{
		harvestableNotificationPrefab = null;
		powerLabelParent = null;
		Instance = null;
		OverlayModes.Mode.Clear();
		modeInfos = null;
		currentModeInfo = default(ModeInfo);
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		techViewSound = KFMOD.CreateInstance(techViewSoundPath);
		techViewSoundPlaying = false;
		Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
		RegisterModes();
		currentModeInfo = modeInfos[OverlayModes.None.ID];
	}

	private void RegisterModes()
	{
		modeInfos.Clear();
		OverlayModes.None mode = new OverlayModes.None();
		RegisterMode(mode);
		RegisterMode(new OverlayModes.Oxygen());
		RegisterMode(new OverlayModes.Power(powerLabelParent, powerLabelPrefab, batUIPrefab, powerLabelOffset, batteryUIOffset, batteryUITransformerOffset, batteryUISmallTransformerOffset, consumerColour, generatorColour, buildingDisabledColour, circuitUnpoweredColour, circuitSafeColour, circuitStrainingColour));
		RegisterMode(new OverlayModes.Temperature());
		RegisterMode(new OverlayModes.ThermalConductivity());
		RegisterMode(new OverlayModes.Light());
		RegisterMode(new OverlayModes.LiquidConduits());
		RegisterMode(new OverlayModes.GasConduits());
		RegisterMode(new OverlayModes.Decor());
		RegisterMode(new OverlayModes.Disease(powerLabelParent, diseaseOverlayPrefab));
		RegisterMode(new OverlayModes.Crop(powerLabelParent, harvestableNotificationPrefab));
		RegisterMode(new OverlayModes.Harvest());
		RegisterMode(new OverlayModes.Priorities());
		RegisterMode(new OverlayModes.HeatFlow());
		RegisterMode(new OverlayModes.Rooms());
		RegisterMode(new OverlayModes.Suit(powerLabelParent, suitOverlayPrefab));
		RegisterMode(new OverlayModes.Logic(logicModeUIPrefab));
		RegisterMode(new OverlayModes.SolidConveyor());
		RegisterMode(new OverlayModes.TileMode());
	}

	private void RegisterMode(OverlayModes.Mode mode)
	{
		modeInfos[mode.ViewMode()] = new ModeInfo
		{
			mode = mode
		};
	}

	private void LateUpdate()
	{
		currentModeInfo.mode.Update();
	}

	public void ToggleOverlay(HashedString newMode)
	{
		bool flag = (!(currentModeInfo.mode.ViewMode() == newMode)) ? true : false;
		if (newMode != OverlayModes.None.ID)
		{
			ManagementMenu.Instance.CloseAll();
		}
		currentModeInfo.mode.Disable();
		if (newMode != currentModeInfo.mode.ViewMode() && newMode == OverlayModes.None.ID)
		{
			ManagementMenu.Instance.CloseAll();
		}
		ResourceCategoryScreen.Instance.Show(newMode == OverlayModes.None.ID);
		SimDebugView.Instance.SetMode(newMode);
		if (!modeInfos.TryGetValue(newMode, out currentModeInfo))
		{
			currentModeInfo = modeInfos[OverlayModes.None.ID];
		}
		currentModeInfo.mode.Enable();
		if (flag)
		{
			UpdateOverlaySounds();
		}
		if (OverlayModes.None.ID == currentModeInfo.mode.ViewMode())
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterOnMigrated, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicOverlayInactive();
			techViewSound.stop(STOP_MODE.ALLOWFADEOUT);
			techViewSoundPlaying = false;
		}
		else if (!techViewSoundPlaying)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterOnMigrated);
			MusicManager.instance.SetDynamicMusicOverlayActive();
			techViewSound.start();
			techViewSoundPlaying = true;
		}
		if (OnOverlayChanged != null)
		{
			OnOverlayChanged(currentModeInfo.mode.ViewMode());
		}
		ActivateLegend();
	}

	private void ActivateLegend()
	{
		if (!((UnityEngine.Object)OverlayLegend.Instance == (UnityEngine.Object)null))
		{
			OverlayLegend.Instance.SetLegend(currentModeInfo.mode, false);
		}
	}

	public void Refresh()
	{
		LateUpdate();
	}

	public HashedString GetMode()
	{
		return (currentModeInfo.mode == null) ? OverlayModes.None.ID : currentModeInfo.mode.ViewMode();
	}

	private void UpdateOverlaySounds()
	{
		string soundName = currentModeInfo.mode.GetSoundName();
		if (soundName != string.Empty)
		{
			soundName = GlobalAssets.GetSound(soundName, false);
			KMonoBehaviour.PlaySound(soundName);
		}
	}
}
