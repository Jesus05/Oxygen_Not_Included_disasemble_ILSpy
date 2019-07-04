using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using VoronoiTree;

public class OfflineWorldGen : KMonoBehaviour
{
	public struct ErrorInfo
	{
		public string errorDesc;

		public Exception exception;
	}

	[Serializable]
	private struct ValidDimensions
	{
		public int width;

		public int height;

		public StringKey name;
	}

	[SerializeField]
	private RectTransform buttonRoot;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private RectTransform chooseLocationPanel;

	[SerializeField]
	private GameObject locationButtonPrefab;

	private const float baseScale = 0.005f;

	private Mutex errorMutex = new Mutex();

	private List<ErrorInfo> errors = new List<ErrorInfo>();

	private ValidDimensions[] validDimensions = new ValidDimensions[1]
	{
		new ValidDimensions
		{
			width = 256,
			height = 384,
			name = UI.FRONTEND.WORLDGENSCREEN.SIZES.STANDARD.key
		}
	};

	public string frontendGameLevel = "frontend";

	public string mainGameLevel = "backend";

	private bool shouldStop = false;

	private StringKey currentConvertedCurrentStage;

	private float currentPercent = 0f;

	public bool debug = false;

	private bool trackProgress = true;

	private bool doWorldGen = false;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText mainText;

	[SerializeField]
	private LocText updateText;

	[SerializeField]
	private LocText percentText;

	[SerializeField]
	private LocText seedText;

	[SerializeField]
	private KBatchedAnimController meterAnim;

	[SerializeField]
	private KBatchedAnimController asteriodAnim;

	private WorldGen worldGen;

	private List<VoronoiTree.Node> startNodes = null;

	private StringKey currentStringKeyRoot;

	private List<LocString> convertList = new List<LocString>
	{
		UI.WORLDGEN.SETTLESIM,
		UI.WORLDGEN.BORDERS,
		UI.WORLDGEN.PROCESSING,
		UI.WORLDGEN.COMPLETELAYOUT,
		UI.WORLDGEN.WORLDLAYOUT,
		UI.WORLDGEN.GENERATENOISE,
		UI.WORLDGEN.BUILDNOISESOURCE,
		UI.WORLDGEN.GENERATESOLARSYSTEM
	};

	private WorldGenProgressStages.Stages currentStage = WorldGenProgressStages.Stages.Failure;

	private bool loadTriggered = false;

	private bool shownStartingLocations = false;

	private bool startedExitFlow = false;

	private bool generateThreadComplete = false;

	private bool renderThreadComplete = false;

	private bool firstPassGeneration = false;

	private bool secondPassGeneration = false;

	private int seed = 0;

	private void TrackProgress(string text)
	{
		if (trackProgress)
		{
			Debug.Log(text);
		}
	}

	public static bool CanLoadSave()
	{
		bool flag = true;
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		flag = WorldGen.CanLoad(activeSaveFilePath);
		if (!flag)
		{
			SaveLoader.SetActiveSaveFilePath(null);
			flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
		}
		return flag;
	}

	public void Generate()
	{
		doWorldGen = !CanLoadSave();
		updateText.gameObject.SetActive(false);
		percentText.gameObject.SetActive(false);
		doWorldGen |= debug;
		if (doWorldGen)
		{
			seedText.text = string.Format(UI.WORLDGEN.USING_PLAYER_SEED, seed);
			titleText.text = UI.FRONTEND.WORLDGENSCREEN.TITLE.ToString();
			mainText.text = UI.WORLDGEN.CHOOSEWORLDSIZE.ToString();
			for (int i = 0; i < this.validDimensions.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab);
				gameObject.SetActive(true);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.SetParent(buttonRoot);
				component.localScale = Vector3.one;
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				ValidDimensions validDimensions = this.validDimensions[i];
				componentInChildren.text = validDimensions.name.ToString();
				int idx = i;
				KButton component2 = gameObject.GetComponent<KButton>();
				component2.onClick += delegate
				{
					DoWorldGen(idx);
					ToggleGenerationUI();
				};
			}
			if (this.validDimensions.Length == 1)
			{
				DoWorldGen(0);
				ToggleGenerationUI();
			}
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
			OnResize();
		}
		else
		{
			titleText.text = UI.FRONTEND.WORLDGENSCREEN.LOADINGGAME.ToString();
			mainText.gameObject.SetActive(false);
			currentConvertedCurrentStage = UI.WORLDGEN.COMPLETE.key;
			currentPercent = 100f;
			updateText.gameObject.SetActive(false);
			percentText.gameObject.SetActive(false);
			RemoveButtons();
		}
		buttonPrefab.SetActive(false);
	}

	private void OnResize()
	{
		float num = 1f;
		num = GetComponentInParent<KCanvasScaler>().GetCanvasScale();
		if ((UnityEngine.Object)asteriodAnim != (UnityEngine.Object)null)
		{
			asteriodAnim.animScale = 0.005f * (1f / num);
		}
	}

	private void ToggleGenerationUI()
	{
		percentText.gameObject.SetActive(false);
		updateText.gameObject.SetActive(true);
		titleText.text = UI.FRONTEND.WORLDGENSCREEN.GENERATINGWORLD.ToString();
		if ((UnityEngine.Object)titleText != (UnityEngine.Object)null && (UnityEngine.Object)titleText.gameObject != (UnityEngine.Object)null)
		{
			titleText.gameObject.SetActive(false);
		}
		if ((UnityEngine.Object)buttonRoot != (UnityEngine.Object)null && (UnityEngine.Object)buttonRoot.gameObject != (UnityEngine.Object)null)
		{
			buttonRoot.gameObject.SetActive(false);
		}
	}

	private void ChooseBaseLocation(VoronoiTree.Node startNode)
	{
		worldGen.ChooseBaseLocation(startNode);
		DoRenderWorld();
		RemoveLocationButtons();
	}

	private void ShowStartingLocationChoices()
	{
		if ((UnityEngine.Object)titleText != (UnityEngine.Object)null)
		{
			titleText.text = "Choose Starting Location";
		}
		startNodes = worldGen.WorldLayout.GetStartNodes();
		startNodes.Shuffle();
		if (startNodes.Count > 0)
		{
			ChooseBaseLocation(startNodes[0]);
		}
		else
		{
			List<SubWorld> list = new List<SubWorld>();
			for (int i = 0; i < startNodes.Count; i++)
			{
				Tree tree = startNodes[i] as Tree;
				if (tree == null)
				{
					tree = worldGen.GetOverworldForNode(startNodes[i] as Leaf);
					if (tree == null)
					{
						continue;
					}
				}
				SubWorld subWorldForNode = worldGen.GetSubWorldForNode(tree);
				if (subWorldForNode != null && !list.Contains(subWorldForNode))
				{
					list.Add(subWorldForNode);
					GameObject gameObject = UnityEngine.Object.Instantiate(locationButtonPrefab);
					RectTransform component = gameObject.GetComponent<RectTransform>();
					component.SetParent(chooseLocationPanel);
					component.localScale = Vector3.one;
					Text componentInChildren = gameObject.GetComponentInChildren<Text>();
					SubWorld subWorld = null;
					Tree parent = startNodes[i].parent;
					while (subWorld == null && parent != null)
					{
						subWorld = worldGen.GetSubWorldForNode(parent);
						if (subWorld == null)
						{
							parent = parent.parent;
						}
					}
					TagSet tagSet = new TagSet(startNodes[i].tags);
					tagSet.Remove(WorldGenTags.Feature);
					tagSet.Remove(WorldGenTags.StartLocation);
					tagSet.Remove(WorldGenTags.IgnoreCaveOverride);
					componentInChildren.text = tagSet.ToString();
					int idx = i;
					Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
					buttonClickedEvent.AddListener(delegate
					{
						ChooseBaseLocation(startNodes[idx]);
					});
					Button component2 = gameObject.GetComponent<Button>();
					component2.onClick = buttonClickedEvent;
				}
			}
		}
	}

	private void RemoveLocationButtons()
	{
		int childCount = chooseLocationPanel.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			Transform child = chooseLocationPanel.GetChild(num);
			UnityEngine.Object.Destroy(child.gameObject);
		}
		if ((UnityEngine.Object)titleText != (UnityEngine.Object)null && (UnityEngine.Object)titleText.gameObject != (UnityEngine.Object)null)
		{
			UnityEngine.Object.DestroyImmediate(titleText.gameObject);
		}
	}

	private bool UpdateProgress(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage)
	{
		if (currentStage != stage)
		{
			currentStage = stage;
		}
		if (currentStringKeyRoot.Hash != stringKeyRoot.Hash)
		{
			currentConvertedCurrentStage = stringKeyRoot;
			currentStringKeyRoot = stringKeyRoot;
		}
		else
		{
			int num = (int)completePercent / 10;
			LocString locString = convertList.Find(delegate(LocString s)
			{
				StringKey key2 = s.key;
				return key2.Hash == stringKeyRoot.Hash;
			});
			if (num != 0 && locString != null)
			{
				StringKey key = locString.key;
				currentConvertedCurrentStage = new StringKey(key.String + num.ToString());
			}
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = WorldGenProgressStages.StageWeights[(int)stage].Value * completePercent;
		for (int i = 0; i < WorldGenProgressStages.StageWeights.Length; i++)
		{
			num3 += WorldGenProgressStages.StageWeights[i].Value * 100f;
			if (i < (int)currentStage)
			{
				num2 += WorldGenProgressStages.StageWeights[i].Value * 100f;
			}
		}
		float num5 = currentPercent = 100f * ((num2 + num4) / num3);
		return !shouldStop;
	}

	private void Update()
	{
		if (!loadTriggered && currentConvertedCurrentStage.String != null)
		{
			errorMutex.WaitOne();
			int count = errors.Count;
			errorMutex.ReleaseMutex();
			if (count > 0)
			{
				DoExitFlow();
			}
			else
			{
				updateText.text = Strings.Get(currentConvertedCurrentStage.String);
				if (!debug)
				{
					int hash = currentConvertedCurrentStage.Hash;
					StringKey key = UI.WORLDGEN.COMPLETE.key;
					if (hash == key.Hash && currentPercent >= 100f)
					{
						if (!KCrashReporter.terminateOnError || !ReportErrorDialog.hasCrash)
						{
							percentText.text = "";
							loadTriggered = true;
							App.LoadScene(mainGameLevel);
						}
						return;
					}
				}
				if (currentPercent < 0f)
				{
					DoExitFlow();
				}
				else
				{
					if (currentPercent > 0f && !percentText.gameObject.activeSelf)
					{
						percentText.gameObject.SetActive(false);
					}
					percentText.text = GameUtil.GetFormattedPercent(currentPercent, GameUtil.TimeSlice.None);
					meterAnim.SetPositionPercent(currentPercent / 100f);
					if (firstPassGeneration)
					{
						generateThreadComplete = worldGen.IsGenerateComplete();
						if (!generateThreadComplete)
						{
							renderThreadComplete = false;
						}
					}
					if (secondPassGeneration)
					{
						renderThreadComplete = worldGen.IsRenderComplete();
					}
					if (!shownStartingLocations && firstPassGeneration && generateThreadComplete)
					{
						shownStartingLocations = true;
						ShowStartingLocationChoices();
					}
					if (renderThreadComplete)
					{
						int num = 0;
						num++;
					}
				}
			}
		}
	}

	private void DisplayErrors()
	{
		errorMutex.WaitOne();
		if (errors.Count > 0)
		{
			foreach (ErrorInfo error in errors)
			{
				ErrorInfo current = error;
				ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, FrontEndManager.Instance.gameObject, true);
				confirmDialogScreen.PopupConfirmDialog(current.errorDesc, OnConfirmExit, null, null, null, null, null, null, null, true);
			}
		}
		errorMutex.ReleaseMutex();
	}

	private void DoExitFlow()
	{
		if (!startedExitFlow)
		{
			startedExitFlow = true;
			percentText.text = UI.WORLDGEN.RESTARTING.ToString();
			loadTriggered = true;
			Sim.Shutdown();
			DisplayErrors();
		}
	}

	private void OnConfirmExit()
	{
		App.LoadScene(frontendGameLevel);
	}

	private void RemoveButtons()
	{
		int childCount = buttonRoot.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			Transform child = buttonRoot.GetChild(num);
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}

	private void DoWorldGen(int selectedDimension)
	{
		RemoveButtons();
		DoWorldGenInitialize();
	}

	private void DoWorldGenInitialize()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World);
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		seed = int.Parse(currentQualitySetting2.id);
		List<string> randomTraits = SettingsCache.GetRandomTraits(seed);
		worldGen = new WorldGen(currentQualitySetting.id, randomTraits);
		Vector2I worldsize = worldGen.Settings.world.worldsize;
		GridSettings.Reset(worldsize.x, worldsize.y);
		worldGen.Initialise(UpdateProgress, OnError, seed, seed, seed, seed);
		firstPassGeneration = true;
		worldGen.GenerateOfflineThreaded();
	}

	private void DoRenderWorld()
	{
		firstPassGeneration = false;
		secondPassGeneration = true;
		worldGen.RenderWorldThreaded();
	}

	private void OnError(ErrorInfo error)
	{
		errorMutex.WaitOne();
		errors.Add(error);
		errorMutex.ReleaseMutex();
	}
}
