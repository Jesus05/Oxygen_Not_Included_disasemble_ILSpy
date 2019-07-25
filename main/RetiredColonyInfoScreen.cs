using Database;
using FMOD.Studio;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetiredColonyInfoScreen : KModalScreen
{
	public static RetiredColonyInfoScreen Instance;

	private bool wasPixelPerfect;

	[Header("Screen")]
	[SerializeField]
	private KButton closeButton;

	[Header("Header References")]
	[SerializeField]
	private GameObject explorerHeaderContainer;

	[SerializeField]
	private GameObject colonyHeaderContainer;

	[SerializeField]
	private LocText colonyName;

	[SerializeField]
	private LocText cycleCount;

	[Header("Timelapse References")]
	[SerializeField]
	private Slideshow slideshow;

	[Header("Main Layout")]
	[SerializeField]
	private GameObject coloniesSection;

	[SerializeField]
	private GameObject achievementsSection;

	[Header("Achievement References")]
	[SerializeField]
	private GameObject achievementsContainer;

	[SerializeField]
	private GameObject achievementsPrefab;

	[SerializeField]
	private GameObject victoryAchievementsPrefab;

	[SerializeField]
	private TMP_InputField achievementSearch;

	[SerializeField]
	private KButton clearAchievementSearchButton;

	[SerializeField]
	private GameObject[] achievementVeils;

	[Header("Duplicant References")]
	[SerializeField]
	private GameObject duplicantPrefab;

	[Header("Building References")]
	[SerializeField]
	private GameObject buildingPrefab;

	[Header("Colony Stat References")]
	[SerializeField]
	private GameObject statsContainer;

	[SerializeField]
	private GameObject specialMediaBlock;

	[SerializeField]
	private GameObject tallFeatureBlock;

	[SerializeField]
	private GameObject standardStatBlock;

	[SerializeField]
	private GameObject lineGraphPrefab;

	public RetiredColonyData[] retiredColonyData;

	[Header("Explorer References")]
	[SerializeField]
	private GameObject colonyScroll;

	[SerializeField]
	private GameObject explorerRoot;

	[SerializeField]
	private GameObject explorerGrid;

	[SerializeField]
	private GameObject colonyDataRoot;

	[SerializeField]
	private GameObject colonyButtonPrefab;

	[SerializeField]
	private TMP_InputField explorerSearch;

	[SerializeField]
	private KButton clearExplorerSearchButton;

	[Header("Navigation Buttons")]
	[SerializeField]
	private KButton closeScreenButton;

	[SerializeField]
	private KButton viewOtherColoniesButton;

	[SerializeField]
	private KButton quitToMainMenuButton;

	private bool explorerGridConfigured = false;

	private Dictionary<string, GameObject> achievementEntries = new Dictionary<string, GameObject>();

	private List<GameObject> activeColonyWidgetContainers = new List<GameObject>();

	private Dictionary<string, GameObject> activeColonyWidgets = new Dictionary<string, GameObject>();

	private const float maxAchievementWidth = 830f;

	private Canvas canvasRef;

	private Dictionary<string, GameObject> explorerColonyWidgets = new Dictionary<string, GameObject>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		ConfigButtons();
		LoadExplorer();
		PopulateAchievements();
		ConsumeMouseScroll = true;
		explorerSearch.text = "";
		explorerSearch.onValueChanged.AddListener(delegate
		{
			if (colonyDataRoot.activeSelf)
			{
				FilterColonyData(explorerSearch.text);
			}
			else
			{
				FilterExplorer(explorerSearch.text);
			}
		});
		clearExplorerSearchButton.onClick += delegate
		{
			explorerSearch.text = "";
		};
		achievementSearch.text = "";
		achievementSearch.onValueChanged.AddListener(delegate
		{
			FilterAchievements(achievementSearch.text);
		});
		clearAchievementSearchButton.onClick += delegate
		{
			achievementSearch.text = "";
		};
		RefreshUIScale(null);
		Subscribe(-810220474, RefreshUIScale);
	}

	private void RefreshUIScale(object data = null)
	{
		StartCoroutine(DelayedRefreshScale());
	}

	private IEnumerator DelayedRefreshScale()
	{
		int i = 0;
		if (i < 3)
		{
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		float spacingBuffer = 36f;
		GameObject parent = GameObject.Find("ScreenSpaceOverlayCanvas");
		if ((UnityEngine.Object)parent != (UnityEngine.Object)null)
		{
			explorerRoot.transform.parent.localScale = Vector3.one * ((colonyScroll.rectTransform().rect.width - spacingBuffer) / explorerRoot.transform.parent.rectTransform().rect.width);
		}
		else
		{
			explorerRoot.transform.parent.localScale = Vector3.one * ((colonyScroll.rectTransform().rect.width - spacingBuffer) / explorerRoot.transform.parent.rectTransform().rect.width);
		}
	}

	private void ConfigButtons()
	{
		closeButton.ClearOnClick();
		closeButton.onClick += delegate
		{
			Show(false);
		};
		viewOtherColoniesButton.ClearOnClick();
		viewOtherColoniesButton.onClick += delegate
		{
			ToggleExplorer(true);
		};
		quitToMainMenuButton.ClearOnClick();
		quitToMainMenuButton.onClick += delegate
		{
			ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, OnQuitConfirm);
		};
		closeScreenButton.ClearOnClick();
		closeScreenButton.onClick += delegate
		{
			Show(false);
		};
		viewOtherColoniesButton.gameObject.SetActive(false);
		if ((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
		{
			closeScreenButton.gameObject.SetActive(true);
			closeScreenButton.GetComponentInChildren<LocText>().SetText(UI.RETIRED_COLONY_INFO_SCREEN.BUTTONS.RETURN_TO_GAME);
			quitToMainMenuButton.gameObject.SetActive(true);
		}
		else
		{
			closeScreenButton.gameObject.SetActive(true);
			closeScreenButton.GetComponentInChildren<LocText>().SetText(UI.RETIRED_COLONY_INFO_SCREEN.BUTTONS.CLOSE);
			quitToMainMenuButton.gameObject.SetActive(false);
		}
	}

	private void ConfirmDecision(string text, System.Action onConfirm)
	{
		base.gameObject.SetActive(false);
		ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		confirmDialogScreen.PopupConfirmDialog(text, onConfirm, OnCancelPopup, null, null, null, null, null, null, true);
	}

	private void OnCancelPopup()
	{
		base.gameObject.SetActive(true);
	}

	private void OnQuitConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			Deactivate();
			PauseScreen.TriggerQuitGame();
		});
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		GetCanvasRef();
		wasPixelPerfect = canvasRef.pixelPerfect;
		canvasRef.pixelPerfect = false;
	}

	private void GetCanvasRef()
	{
		if ((UnityEngine.Object)base.transform.parent.GetComponent<Canvas>() != (UnityEngine.Object)null)
		{
			canvasRef = base.transform.parent.GetComponent<Canvas>();
		}
		else
		{
			canvasRef = base.transform.parent.parent.GetComponent<Canvas>();
		}
	}

	protected override void OnCmpDisable()
	{
		canvasRef.pixelPerfect = wasPixelPerfect;
		base.OnCmpDisable();
	}

	public RetiredColonyData GetColonyDataByBaseName(string name)
	{
		name = RetireColonyUtility.StripInvalidCharacters(name);
		for (int i = 0; i < retiredColonyData.Length; i++)
		{
			if (RetireColonyUtility.StripInvalidCharacters(retiredColonyData[i].colonyName) == name)
			{
				return retiredColonyData[i];
			}
		}
		return null;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			RefreshUIScale(null);
		}
		if ((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
		{
			if (!show)
			{
				if (MusicManager.instance.SongIsPlaying("Music_Victory_03_StoryAndSummary"))
				{
					MusicManager.instance.StopSong("Music_Victory_03_StoryAndSummary", true, STOP_MODE.ALLOWFADEOUT);
				}
			}
			else if (MusicManager.instance.SongIsPlaying("Music_Victory_03_StoryAndSummary"))
			{
				MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 2f, true);
			}
		}
		else if ((UnityEngine.Object)Game.Instance == (UnityEngine.Object)null)
		{
			ToggleExplorer(true);
		}
	}

	public void LoadColony(RetiredColonyData data)
	{
		colonyName.text = data.colonyName.ToUpper();
		cycleCount.text = string.Format(UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, data.cycleCount.ToString());
		ToggleExplorer(false);
		if ((UnityEngine.Object)Game.Instance == (UnityEngine.Object)null)
		{
			viewOtherColoniesButton.gameObject.SetActive(true);
		}
		ClearColony();
		if ((UnityEngine.Object)SaveGame.Instance != (UnityEngine.Object)null)
		{
			UpdateAchievementData(data, SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievementsToDisplay.ToArray());
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().ClearDisplayAchievements();
		}
		else
		{
			UpdateAchievementData(data, null);
		}
		DisplayStatistics(data);
		RectTransform transform = colonyDataRoot.transform.parent.rectTransform();
		Vector3 position = colonyDataRoot.transform.parent.rectTransform().position;
		transform.SetPosition(new Vector3(position.x, 0f, 0f));
	}

	private void LoadSlideshow(RetiredColonyData data)
	{
		slideshow.SetSprites(RetireColonyUtility.LoadColonySlideshow(data.colonyName));
	}

	private void LoadScreenshot(RetiredColonyData data)
	{
		slideshow.SetSprites(new Sprite[1]
		{
			RetireColonyUtility.LoadColonyPreview(data.colonyName)
		});
	}

	private void ClearColony()
	{
		foreach (GameObject activeColonyWidgetContainer in activeColonyWidgetContainers)
		{
			UnityEngine.Object.Destroy(activeColonyWidgetContainer);
		}
		activeColonyWidgetContainers.Clear();
		activeColonyWidgets.Clear();
		UpdateAchievementData(null, null);
	}

	private void PopulateAchievements()
	{
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			GameObject gameObject = Util.KInstantiateUI((!resource.isVictoryCondition) ? achievementsPrefab : victoryAchievementsPrefab, achievementsContainer, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("nameLabel").SetText(resource.Name);
			component.GetReference<LocText>("descriptionLabel").SetText(resource.description);
			if (resource.isVictoryCondition)
			{
				gameObject.transform.SetAsFirstSibling();
			}
			gameObject.GetComponent<MultiToggle>().ChangeState(2);
			achievementEntries.Add(resource.Id, gameObject);
		}
		UpdateAchievementData(null, null);
	}

	private IEnumerator ClearAchievementVeil(float delay = 0f)
	{
		yield return (object)new WaitForSecondsRealtime(delay);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private IEnumerator ShowAchievementVeil()
	{
		float targetAlpha = 0.7f;
		GameObject[] array = achievementVeils;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(true);
		}
		float i = 0f;
		if (i <= targetAlpha)
		{
			GameObject[] array2 = achievementVeils;
			foreach (GameObject gameObject2 in array2)
			{
				gameObject2.GetComponent<Image>().color = new Color(0f, 0f, 0f, i);
			}
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		for (float num = 0f; num <= targetAlpha; num += Time.unscaledDeltaTime)
		{
			GameObject[] array3 = achievementVeils;
			foreach (GameObject gameObject3 in array3)
			{
				gameObject3.GetComponent<Image>().color = new Color(0f, 0f, 0f, targetAlpha);
			}
		}
	}

	private void UpdateAchievementData(RetiredColonyData data, string[] newlyAchieved = null)
	{
		int num = 1;
		float num2 = 1f;
		foreach (KeyValuePair<string, GameObject> achievementEntry in achievementEntries)
		{
			bool flag = false;
			bool flag2 = false;
			if (data != null)
			{
				string[] achievements = data.achievements;
				foreach (string a in achievements)
				{
					if (a == achievementEntry.Key)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag && data == null)
			{
				RetiredColonyData[] array = this.retiredColonyData;
				foreach (RetiredColonyData retiredColonyData in array)
				{
					string[] achievements2 = retiredColonyData.achievements;
					foreach (string a2 in achievements2)
					{
						if (a2 == achievementEntry.Key)
						{
							flag2 = true;
						}
					}
				}
			}
			if (flag)
			{
				bool flag3 = false;
				if (newlyAchieved != null)
				{
					for (int l = 0; l < newlyAchieved.Length; l++)
					{
						if (newlyAchieved[l] == achievementEntry.Key)
						{
							flag3 = true;
							achievementEntry.Value.GetComponent<AchievementWidget>().ActivateNewlyAchievedFlourish(num2 + (float)num * 1f);
							num++;
						}
					}
				}
				if (!flag3)
				{
					achievementEntry.Value.GetComponent<AchievementWidget>().SetAchievedNow();
				}
			}
			else if (flag2)
			{
				achievementEntry.Value.GetComponent<AchievementWidget>().SetAchievedBefore();
			}
			else if (data == null)
			{
				achievementEntry.Value.GetComponent<AchievementWidget>().SetNeverAchieved();
			}
			else
			{
				achievementEntry.Value.GetComponent<AchievementWidget>().SetNotAchieved();
			}
		}
		if (newlyAchieved != null && newlyAchieved.Length > 0)
		{
			StartCoroutine(ShowAchievementVeil());
			StartCoroutine(ClearAchievementVeil(num2 + (float)num * 1f));
		}
	}

	private void DisplayInfoBlock(RetiredColonyData data, GameObject container)
	{
		container.GetComponent<HierarchyReferences>().GetReference<LocText>("ColonyNameLabel").SetText(data.colonyName);
		container.GetComponent<HierarchyReferences>().GetReference<LocText>("CycleCountLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, data.cycleCount.ToString()));
	}

	private void DisplayTimelapse(RetiredColonyData data, GameObject container)
	{
		slideshow = container.GetComponent<HierarchyReferences>().GetReference<Slideshow>("Slideshow");
		LoadSlideshow(data);
	}

	private void DisplayScreenshotBlock(RetiredColonyData data, GameObject container)
	{
		slideshow = container.GetComponent<HierarchyReferences>().GetReference<Slideshow>("Screenshot");
		LoadScreenshot(data);
	}

	private void DisplayDuplicants(RetiredColonyData data, GameObject container)
	{
		for (int num = container.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(container.transform.GetChild(num).gameObject);
		}
		RetiredColonyData.RetiredDuplicantData[] duplicants = data.Duplicants;
		foreach (RetiredColonyData.RetiredDuplicantData retiredDuplicantData in duplicants)
		{
			GameObject gameObject = Util.KInstantiateUI(duplicantPrefab, container, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("NameLabel").SetText(retiredDuplicantData.name);
			component.GetReference<LocText>("AgeLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.DUPLICANT_AGE, retiredDuplicantData.age.ToString()));
			component.GetReference<LocText>("SkillLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.SKILL_LEVEL, retiredDuplicantData.skillPointsGained.ToString()));
			SymbolOverrideController reference = component.GetReference<SymbolOverrideController>("SymbolOverrideController");
			reference.RemoveAllSymbolOverrides(0);
			KBatchedAnimController componentInChildren = gameObject.GetComponentInChildren<KBatchedAnimController>();
			componentInChildren.SetSymbolVisiblity("snapTo_neck", false);
			componentInChildren.SetSymbolVisiblity("snapTo_goggles", false);
			componentInChildren.SetSymbolVisiblity("snapTo_hat", false);
			componentInChildren.SetSymbolVisiblity("snapTo_hat_hair", false);
			foreach (KeyValuePair<string, string> accessory in retiredDuplicantData.accessories)
			{
				KAnim.Build.Symbol symbol = Db.Get().Accessories.Get(accessory.Value).symbol;
				AccessorySlot accessorySlot = Db.Get().AccessorySlots.Get(accessory.Key);
				reference.AddSymbolOverride(accessorySlot.targetSymbolId, symbol, 0);
				gameObject.GetComponentInChildren<KBatchedAnimController>().SetSymbolVisiblity(accessory.Key, true);
			}
			reference.ApplyOverrides();
		}
	}

	private void DisplayBuildings(RetiredColonyData data, GameObject container)
	{
		for (int num = container.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(container.transform.GetChild(num).gameObject);
		}
		data.buildings.Sort(delegate(Tuple<string, int> a, Tuple<string, int> b)
		{
			if (a.second <= b.second)
			{
				if (a.second != b.second)
				{
					return -1;
				}
				return 0;
			}
			return 1;
		});
		data.buildings.Reverse();
		foreach (Tuple<string, int> building in data.buildings)
		{
			GameObject prefab = Assets.GetPrefab(building.first);
			GameObject gameObject = Util.KInstantiateUI(buildingPrefab, container, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("NameLabel").SetText(GameUtil.ApplyBoldString(prefab.GetProperName()));
			component.GetReference<LocText>("CountLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.BUILDING_COUNT, building.second.ToString()));
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(prefab, "ui", false);
			component.GetReference<Image>("Portrait").sprite = uISprite.first;
		}
	}

	private IEnumerator ComputeSizeStatGrid()
	{
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private IEnumerator ComputeSizeExplorerGrid()
	{
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void DisplayStatistics(RetiredColonyData data)
	{
		GameObject gameObject = Util.KInstantiateUI(specialMediaBlock, statsContainer, true);
		activeColonyWidgetContainers.Add(gameObject);
		activeColonyWidgets.Add("timelapse", gameObject);
		DisplayTimelapse(data, gameObject);
		DisplayScreenshotBlock(data, gameObject);
		GameObject gameObject2 = Util.KInstantiateUI(tallFeatureBlock, statsContainer, true);
		activeColonyWidgetContainers.Add(gameObject2);
		activeColonyWidgets.Add("duplicants", gameObject2);
		gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.RETIRED_COLONY_INFO_SCREEN.TITLES.DUPLICANTS);
		DisplayDuplicants(data, gameObject2.GetComponentInChildren<KScrollRect>().content.gameObject);
		GameObject gameObject3 = Util.KInstantiateUI(tallFeatureBlock, statsContainer, true);
		activeColonyWidgetContainers.Add(gameObject3);
		activeColonyWidgets.Add("buildings", gameObject3);
		gameObject3.GetComponent<HierarchyReferences>().GetReference<LocText>("Title").SetText(UI.RETIRED_COLONY_INFO_SCREEN.TITLES.BUILDINGS);
		DisplayBuildings(data, gameObject3.GetComponentInChildren<KScrollRect>().content.gameObject);
		int num = 2;
		for (int i = 0; i < data.Stats.Length; i += num)
		{
			GameObject gameObject4 = Util.KInstantiateUI(standardStatBlock, statsContainer, true);
			activeColonyWidgetContainers.Add(gameObject4);
			for (int j = 0; j < num; j++)
			{
				if (i + j <= data.Stats.Length - 1)
				{
					RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic = data.Stats[i + j];
					ConfigureGraph(GetStatistic(retiredColonyStatistic.id, data), gameObject4);
				}
			}
		}
		StartCoroutine(ComputeSizeStatGrid());
	}

	private void ConfigureGraph(RetiredColonyData.RetiredColonyStatistic statistic, GameObject layoutBlockGameObject)
	{
		GameObject gameObject = Util.KInstantiateUI(lineGraphPrefab, layoutBlockGameObject, true);
		activeColonyWidgets.Add(statistic.name, gameObject);
		GraphBase componentInChildren = gameObject.GetComponentInChildren<GraphBase>();
		componentInChildren.graphName = statistic.name;
		componentInChildren.label_title.SetText(componentInChildren.graphName);
		componentInChildren.axis_x.name = statistic.nameX;
		componentInChildren.axis_y.name = statistic.nameY;
		componentInChildren.label_x.SetText(componentInChildren.axis_x.name);
		componentInChildren.label_y.SetText(componentInChildren.axis_y.name);
		LineLayer componentInChildren2 = gameObject.GetComponentInChildren<LineLayer>();
		componentInChildren.axis_y.min_value = 0f;
		componentInChildren.axis_y.max_value = statistic.GetByMaxValue().second * 1.2f;
		componentInChildren.axis_x.min_value = 0f;
		componentInChildren.axis_x.max_value = statistic.GetByMaxKey().first;
		componentInChildren.axis_x.guide_frequency = (componentInChildren.axis_x.max_value - componentInChildren.axis_x.min_value) / 10f;
		componentInChildren.axis_y.guide_frequency = (componentInChildren.axis_y.max_value - componentInChildren.axis_y.min_value) / 10f;
		componentInChildren.RefreshGuides();
		Tuple<float, float>[] value = statistic.value;
		componentInChildren2.NewLine(value, statistic.id);
	}

	private RetiredColonyData.RetiredColonyStatistic GetStatistic(string id, RetiredColonyData data)
	{
		RetiredColonyData.RetiredColonyStatistic[] stats = data.Stats;
		foreach (RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic in stats)
		{
			if (retiredColonyStatistic.id == id)
			{
				return retiredColonyStatistic;
			}
		}
		return null;
	}

	private void ToggleExplorer(bool active)
	{
		ConfigButtons();
		explorerRoot.SetActive(active);
		colonyDataRoot.SetActive(!active);
		if (!explorerGridConfigured)
		{
			explorerGridConfigured = true;
			StartCoroutine(ComputeSizeExplorerGrid());
		}
		explorerHeaderContainer.SetActive(active);
		colonyHeaderContainer.SetActive(!active);
		if (active)
		{
			RectTransform transform = colonyDataRoot.transform.parent.rectTransform();
			Vector3 position = colonyDataRoot.transform.parent.rectTransform().position;
			transform.SetPosition(new Vector3(position.x, 0f, 0f));
		}
		UpdateAchievementData(null, null);
		explorerSearch.text = "";
	}

	private void LoadExplorer()
	{
		ToggleExplorer(true);
		this.retiredColonyData = RetireColonyUtility.LoadRetiredColonies();
		RetiredColonyData[] array = this.retiredColonyData;
		foreach (RetiredColonyData retiredColonyData in array)
		{
			RetiredColonyData data = retiredColonyData;
			GameObject gameObject = Util.KInstantiateUI(colonyButtonPrefab, explorerGrid, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			string text = RetireColonyUtility.StripInvalidCharacters(data.colonyName);
			Sprite sprite = RetireColonyUtility.LoadColonyPreview(text);
			Image reference = component.GetReference<Image>("ColonyImage");
			if ((UnityEngine.Object)sprite != (UnityEngine.Object)null)
			{
				reference.enabled = true;
				reference.sprite = sprite;
			}
			else
			{
				reference.enabled = false;
			}
			component.GetReference<LocText>("ColonyNameLabel").SetText(retiredColonyData.colonyName);
			component.GetReference<LocText>("CycleCountLabel").SetText(string.Format(UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, retiredColonyData.cycleCount.ToString()));
			component.GetReference<LocText>("DateLabel").SetText(retiredColonyData.date);
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				LoadColony(data);
			};
			explorerColonyWidgets.Add(retiredColonyData.colonyName, gameObject);
		}
	}

	private void FilterExplorer(string search)
	{
		foreach (KeyValuePair<string, GameObject> explorerColonyWidget in explorerColonyWidgets)
		{
			if (string.IsNullOrEmpty(search) || explorerColonyWidget.Key.ToUpper().Contains(search.ToUpper()))
			{
				explorerColonyWidget.Value.SetActive(true);
			}
			else
			{
				explorerColonyWidget.Value.SetActive(false);
			}
		}
	}

	private void FilterColonyData(string search)
	{
		foreach (KeyValuePair<string, GameObject> activeColonyWidget in activeColonyWidgets)
		{
			if (string.IsNullOrEmpty(search) || activeColonyWidget.Key.ToUpper().Contains(search.ToUpper()))
			{
				activeColonyWidget.Value.SetActive(true);
			}
			else
			{
				activeColonyWidget.Value.SetActive(false);
			}
		}
	}

	private void FilterAchievements(string search)
	{
		foreach (KeyValuePair<string, GameObject> achievementEntry in achievementEntries)
		{
			if (string.IsNullOrEmpty(search) || Db.Get().ColonyAchievements.Get(achievementEntry.Key).Name.ToUpper().Contains(search.ToUpper()))
			{
				achievementEntry.Value.SetActive(true);
			}
			else
			{
				achievementEntry.Value.SetActive(false);
			}
		}
	}
}
