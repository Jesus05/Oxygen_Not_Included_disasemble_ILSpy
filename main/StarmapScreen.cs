using Database;
using FMOD.Studio;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarmapScreen : KModalScreen
{
	public GameObject listPanel;

	public GameObject rocketPanel;

	public LocText listHeaderLabel;

	public LocText listHeaderStatusLabel;

	public HierarchyReferences listRocketTemplate;

	public LocText listNoRocketText;

	public RectTransform rocketListContainer;

	private Dictionary<Spacecraft, HierarchyReferences> listRocketRows = new Dictionary<Spacecraft, HierarchyReferences>();

	[Header("Shared References")]
	public BreakdownList breakdownListPrefab;

	public GameObject progressBarPrefab;

	[Header("Selected Rocket References")]
	public LocText rocketHeaderLabel;

	public LocText rocketHeaderStatusLabel;

	private BreakdownList rocketDetailsStatus;

	public Sprite rocketDetailsStatusIcon;

	private BreakdownList rocketDetailsChecklist;

	public Sprite rocketDetailsChecklistIcon;

	private BreakdownList rocketDetailsMass;

	public Sprite rocketDetailsMassIcon;

	private BreakdownList rocketDetailsRange;

	public Sprite rocketDetailsRangeIcon;

	public RocketThrustWidget rocketThrustWidget;

	private BreakdownList rocketDetailsStorage;

	public Sprite rocketDetailsStorageIcon;

	private BreakdownList rocketDetailsDupes;

	public Sprite rocketDetailsDupesIcon;

	private BreakdownList rocketDetailsFuel;

	public Sprite rocketDetailsFuelIcon;

	private BreakdownList rocketDetailsOxidizer;

	public Sprite rocketDetailsOxidizerIcon;

	public RectTransform rocketDetailsContainer;

	[Header("Selected Destination References")]
	public LocText destinationHeaderLabel;

	public LocText destinationStatusLabel;

	public LocText destinationNameLabel;

	public LocText destinationTypeNameLabel;

	public LocText destinationTypeValueLabel;

	public LocText destinationDistanceNameLabel;

	public LocText destinationDistanceValueLabel;

	public LocText destinationDescriptionLabel;

	private BreakdownList destinationDetailsAnalysis;

	private GenericUIProgressBar destinationAnalysisProgressBar;

	public Sprite destinationDetailsAnalysisIcon;

	private BreakdownList destinationDetailsResearch;

	public Sprite destinationDetailsResearchIcon;

	private BreakdownList destinationDetailsComposition;

	public Sprite destinationDetailsCompositionIcon;

	private BreakdownList destinationDetailsResources;

	public Sprite destinationDetailsResourcesIcon;

	public RectTransform destinationDetailsContainer;

	public MultiToggle showRocketsButton;

	public MultiToggle launchButton;

	public MultiToggle analyzeButton;

	private int rocketConditionEventHandler = -1;

	[Header("Map References")]
	public RectTransform Map;

	public RectTransform rowsContiner;

	public GameObject rowPrefab;

	public StarmapPlanet planetPrefab;

	public GameObject rocketIconPrefab;

	private List<GameObject> planetRows = new List<GameObject>();

	private Dictionary<SpaceDestination, StarmapPlanet> planetWidgets = new Dictionary<SpaceDestination, StarmapPlanet>();

	private float planetsMaxDistance = 1f;

	public Image distanceOverlay;

	private int distanceOverlayVerticalOffset = 500;

	private int distanceOverlayYOffset = 24;

	public Image visualizeRocketImage;

	public Image visualizeRocketTrajectory;

	public LocText visualizeRocketLabel;

	public LocText visualizeRocketProgress;

	public Color[] distanceColors;

	public LocText titleBarLabel;

	public KButton button;

	private const int DESTINATION_ICON_SCALE = 2;

	public static StarmapScreen Instance;

	private int selectionUpdateHandle = -1;

	private SpaceDestination selectedDestination = null;

	private KSelectable currentSelectable;

	private CommandModule currentCommandModule;

	private LaunchConditionManager currentLaunchConditionManager;

	private bool currentRocketHasGasContainer = false;

	private bool currentRocketHasLiquidContainer = false;

	private bool currentRocketHasSolidContainer = false;

	private bool currentRocketHasEntitiesContainer = false;

	private bool forceScrollDown = true;

	private Coroutine animateAnalysisRoutine;

	private Coroutine animateSelectedPlanetRoutine;

	private BreakdownListRow rangeRowTotal;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		rocketDetailsStatus = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsStatus.SetTitle(UI.STARMAP.LISTTITLES.MISSIONSTATUS);
		rocketDetailsStatus.SetIcon(rocketDetailsStatusIcon);
		rocketDetailsStatus.gameObject.name = "rocketDetailsStatus";
		rocketDetailsChecklist = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsChecklist.SetTitle(UI.STARMAP.LISTTITLES.LAUNCHCHECKLIST);
		rocketDetailsChecklist.SetIcon(rocketDetailsChecklistIcon);
		rocketDetailsChecklist.gameObject.name = "rocketDetailsChecklist";
		rocketDetailsRange = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsRange.SetTitle(UI.STARMAP.LISTTITLES.MAXRANGE);
		rocketDetailsRange.SetIcon(rocketDetailsRangeIcon);
		rocketDetailsRange.gameObject.name = "rocketDetailsRange";
		rocketDetailsMass = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsMass.SetTitle(UI.STARMAP.LISTTITLES.MASS);
		rocketDetailsMass.SetIcon(rocketDetailsMassIcon);
		rocketDetailsMass.gameObject.name = "rocketDetailsMass";
		rocketThrustWidget = UnityEngine.Object.Instantiate(rocketThrustWidget, rocketDetailsContainer);
		rocketDetailsStorage = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsStorage.SetTitle(UI.STARMAP.LISTTITLES.STORAGE);
		rocketDetailsStorage.SetIcon(rocketDetailsStorageIcon);
		rocketDetailsStorage.gameObject.name = "rocketDetailsStorage";
		rocketDetailsFuel = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsFuel.SetTitle(UI.STARMAP.LISTTITLES.FUEL);
		rocketDetailsFuel.SetIcon(rocketDetailsFuelIcon);
		rocketDetailsFuel.gameObject.name = "rocketDetailsFuel";
		rocketDetailsOxidizer = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsOxidizer.SetTitle(UI.STARMAP.LISTTITLES.OXIDIZER);
		rocketDetailsOxidizer.SetIcon(rocketDetailsOxidizerIcon);
		rocketDetailsOxidizer.gameObject.name = "rocketDetailsOxidizer";
		rocketDetailsDupes = UnityEngine.Object.Instantiate(breakdownListPrefab, rocketDetailsContainer);
		rocketDetailsDupes.SetTitle(UI.STARMAP.LISTTITLES.PASSENGERS);
		rocketDetailsDupes.SetIcon(rocketDetailsDupesIcon);
		rocketDetailsDupes.gameObject.name = "rocketDetailsDupes";
		destinationDetailsAnalysis = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsAnalysis.SetTitle(UI.STARMAP.LISTTITLES.ANALYSIS);
		destinationDetailsAnalysis.SetIcon(destinationDetailsAnalysisIcon);
		destinationDetailsAnalysis.gameObject.name = "destinationDetailsAnalysis";
		destinationDetailsAnalysis.SetDescription(string.Format(UI.STARMAP.ANALYSIS_DESCRIPTION, 0));
		destinationAnalysisProgressBar = UnityEngine.Object.Instantiate(progressBarPrefab.gameObject, destinationDetailsContainer).GetComponent<GenericUIProgressBar>();
		destinationAnalysisProgressBar.SetMaxValue((float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
		destinationDetailsResearch = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsResearch.SetTitle(UI.STARMAP.LISTTITLES.RESEARCH);
		destinationDetailsResearch.SetIcon(destinationDetailsResearchIcon);
		destinationDetailsResearch.gameObject.name = "destinationDetailsResearch";
		destinationDetailsResearch.SetDescription(string.Format(UI.STARMAP.RESEARCH_DESCRIPTION, 0));
		destinationDetailsComposition = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsComposition.SetTitle(UI.STARMAP.LISTTITLES.WORLDCOMPOSITION);
		destinationDetailsComposition.SetIcon(destinationDetailsCompositionIcon);
		destinationDetailsComposition.gameObject.name = "destinationDetailsComposition";
		destinationDetailsResources = UnityEngine.Object.Instantiate(breakdownListPrefab, destinationDetailsContainer);
		destinationDetailsResources.SetTitle(UI.STARMAP.LISTTITLES.RESOURCES);
		destinationDetailsResources.SetIcon(destinationDetailsResourcesIcon);
		destinationDetailsResources.gameObject.name = "destinationDetailsResources";
		LoadPlanets();
		selectionUpdateHandle = Game.Instance.Subscribe(-1503271301, OnSelectableChanged);
		titleBarLabel.text = UI.STARMAP.TITLE;
		button.onClick += delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		launchButton.play_sound_on_click = false;
		MultiToggle multiToggle = launchButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			if ((UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null && selectedDestination != null)
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
				LaunchRocket(currentLaunchConditionManager);
			}
			else
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Negative", false));
			}
		});
		launchButton.ChangeState(1);
		MultiToggle multiToggle2 = showRocketsButton;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			OnSelectableChanged(null);
		});
		SelectDestination(null);
		SpacecraftManager.instance.Subscribe(532901469, delegate
		{
			RefreshAnalyzeButton();
			UpdateDestinationStates();
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (selectionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(selectionUpdateHandle);
		}
		StopAllCoroutines();
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
			MusicManager.instance.PlaySong("Music_Starmap", false);
			UpdateDestinationStates();
			Refresh(null);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.StopSong("Music_Starmap", true, STOP_MODE.ALLOWFADEOUT);
		}
		OnSelectableChanged((!((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)null)) ? SelectTool.Instance.selected.gameObject : null);
		forceScrollDown = true;
	}

	private void UpdateDestinationStates()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		foreach (SpaceDestination destination in SpacecraftManager.instance.destinations)
		{
			num = Mathf.Max(num, destination.OneBasedDistance);
			if (destination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				num2 = Mathf.Max(num2, destination.OneBasedDistance);
			}
		}
		for (int i = num2; i < num; i++)
		{
			bool flag = false;
			foreach (SpaceDestination destination2 in SpacecraftManager.instance.destinations)
			{
				if (destination2.distance == i)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			num3++;
		}
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in planetWidgets)
		{
			SpaceDestination key = planetWidget.Key;
			StarmapPlanet planet = planetWidget.Value;
			Color color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
			Color color2 = new Color(0.75f, 0.75f, 0.75f, 0.75f);
			if (planetWidget.Key.distance >= num2 + num3)
			{
				planet.SetUnknownBGActive(false, Color.white);
				planet.SetSprite(Assets.GetSprite("unknown_far"), color);
			}
			else
			{
				planet.SetAnalysisActive(SpacecraftManager.instance.GetStarmapAnalysisDestinationID() == planetWidget.Key.id);
				bool flag2 = SpacecraftManager.instance.GetDestinationAnalysisState(key) == SpacecraftManager.DestinationAnalysisState.Complete;
				SpaceDestinationType destinationType = key.GetDestinationType();
				planet.SetLabel((!flag2) ? (UI.STARMAP.UNKNOWN_DESTINATION + "\n" + string.Format(UI.STARMAP.ANALYSIS_AMOUNT.text, GameUtil.GetFormattedPercent(100f * (SpacecraftManager.instance.GetDestinationAnalysisScore(planetWidget.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE), GameUtil.TimeSlice.None))) : (destinationType.Name + "\n<color=#979798> " + GameUtil.GetFormattedDistance((float)planetWidget.Key.OneBasedDistance * 10000f * 1000f) + "</color>"));
				planet.SetSprite((!flag2) ? Assets.GetSprite("unknown") : Assets.GetSprite(destinationType.spriteName), (!flag2) ? color2 : Color.white);
				planet.SetUnknownBGActive(SpacecraftManager.instance.GetDestinationAnalysisState(planetWidget.Key) != SpacecraftManager.DestinationAnalysisState.Complete, color2);
				planet.SetFillAmount(SpacecraftManager.instance.GetDestinationAnalysisScore(planetWidget.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
				List<int> spacecraftsForDestination = SpacecraftManager.instance.GetSpacecraftsForDestination(key);
				planet.SetRocketIcons(spacecraftsForDestination.Count, rocketIconPrefab);
				bool show = (UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null && key == SpacecraftManager.instance.GetSpacecraftDestination(currentLaunchConditionManager);
				planet.ShowAsCurrentRocketDestination(show);
				planet.SetOnClick(delegate
				{
					if ((UnityEngine.Object)currentLaunchConditionManager == (UnityEngine.Object)null)
					{
						SelectDestination(planetWidget.Key);
					}
					else
					{
						Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(currentLaunchConditionManager);
						if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded)
						{
							SelectDestination(planetWidget.Key);
						}
					}
				});
				planet.SetOnEnter(delegate
				{
					planet.ShowLabel(true);
				});
				planet.SetOnExit(delegate
				{
					planet.ShowLabel(false);
				});
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
	}

	private string DisplayDistance(float distance)
	{
		return Util.FormatWholeNumber(distance) + " " + UI.UNITSUFFIXES.DISTANCE.KILOMETER;
	}

	private void LoadPlanets()
	{
		foreach (SpaceDestination destination in Game.Instance.spacecraftManager.destinations)
		{
			if ((float)destination.OneBasedDistance * 10000f > planetsMaxDistance)
			{
				planetsMaxDistance = (float)destination.OneBasedDistance * 10000f;
			}
			while (planetRows.Count < destination.distance + 1)
			{
				GameObject gameObject = Util.KInstantiateUI(rowPrefab, rowsContiner.gameObject, true);
				gameObject.rectTransform().SetAsFirstSibling();
				planetRows.Add(gameObject);
				gameObject.GetComponentInChildren<Image>().color = distanceColors[planetRows.Count % distanceColors.Length];
				gameObject.GetComponentInChildren<LocText>().text = DisplayDistance((float)(planetRows.Count + 1) * 10000f);
			}
			GameObject gameObject2 = Util.KInstantiateUI(planetPrefab.gameObject, planetRows[destination.distance], true);
			planetWidgets.Add(destination, gameObject2.GetComponent<StarmapPlanet>());
		}
		UpdateDestinationStates();
	}

	private void UnselectAllPlanets()
	{
		if (animateSelectedPlanetRoutine != null)
		{
			StopCoroutine(animateSelectedPlanetRoutine);
		}
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in planetWidgets)
		{
			planetWidget.Value.SetSelectionActive(false);
			planetWidget.Value.ShowAsCurrentRocketDestination(false);
		}
	}

	private void SelectPlanet(StarmapPlanet planet)
	{
		planet.SetSelectionActive(true);
		if (animateSelectedPlanetRoutine != null)
		{
			StopCoroutine(animateSelectedPlanetRoutine);
		}
		animateSelectedPlanetRoutine = StartCoroutine(AnimatePlanetSelection(planet));
	}

	private IEnumerator AnimatePlanetSelection(StarmapPlanet planet)
	{
		planet.AnimateSelector(Time.unscaledTime);
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void Update()
	{
		PositionPlanetWidgets();
		if (forceScrollDown)
		{
			ScrollToBottom();
			forceScrollDown = false;
		}
	}

	private void ScrollToBottom()
	{
		RectTransform rectTransform = Map.GetComponentInChildren<VerticalLayoutGroup>().rectTransform();
		RectTransform transform = rectTransform;
		Vector3 localPosition = rectTransform.localPosition;
		float x = localPosition.x;
		float y = rectTransform.rect.height - Map.rect.height;
		Vector3 localPosition2 = rectTransform.localPosition;
		transform.SetLocalPosition(new Vector3(x, y, localPosition2.z));
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && (e.TryConsume(Action.MouseRight) || e.TryConsume(Action.Escape)))
		{
			ManagementMenu.Instance.CloseAll();
		}
		else if (CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		if ((UnityEngine.Object)UnityEngine.EventSystems.EventSystem.current != (UnityEngine.Object)null)
		{
			GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if ((UnityEngine.Object)currentSelectedGameObject != (UnityEngine.Object)null)
			{
				foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in listRocketRows)
				{
					HierarchyReferences value = listRocketRow.Value;
					EditableTitleBar component = value.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
					if ((UnityEngine.Object)currentSelectedGameObject == (UnityEngine.Object)component.inputField.gameObject)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void PositionPlanetWidgets()
	{
		float num = rowPrefab.GetComponent<RectTransform>().rect.height / 2f;
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in planetWidgets)
		{
			RectTransform rectTransform = planetWidget.Value.rectTransform();
			Vector2 sizeDelta = planetWidget.Value.transform.parent.rectTransform().sizeDelta;
			rectTransform.anchoredPosition = new Vector2(sizeDelta.x * planetWidget.Key.startingOrbitPercentage, 0f - num);
		}
	}

	private void OnSelectableChanged(object data)
	{
		if (base.gameObject.activeSelf)
		{
			if (rocketConditionEventHandler != -1)
			{
				Unsubscribe(rocketConditionEventHandler);
			}
			if (data != null)
			{
				currentSelectable = ((GameObject)data).GetComponent<KSelectable>();
				currentCommandModule = currentSelectable.GetComponent<CommandModule>();
				currentLaunchConditionManager = currentSelectable.GetComponent<LaunchConditionManager>();
				if ((UnityEngine.Object)currentCommandModule != (UnityEngine.Object)null && (UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null)
				{
					SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(currentLaunchConditionManager);
					SelectDestination(spacecraftDestination);
					rocketConditionEventHandler = currentLaunchConditionManager.Subscribe(1655598572, Refresh);
					ShowRocketDetailsPanel();
				}
				else
				{
					currentSelectable = null;
					currentCommandModule = null;
					currentLaunchConditionManager = null;
					ShowRocketListPanel();
				}
			}
			else
			{
				currentSelectable = null;
				currentCommandModule = null;
				currentLaunchConditionManager = null;
				ShowRocketListPanel();
			}
			Refresh(null);
		}
	}

	private void ShowRocketListPanel()
	{
		listPanel.SetActive(true);
		rocketPanel.SetActive(false);
		launchButton.ChangeState(1);
		UpdateDistanceOverlay(null);
		UpdateMissionOverlay(null);
	}

	private void ShowRocketDetailsPanel()
	{
		listPanel.SetActive(false);
		rocketPanel.SetActive(true);
		ValidateTravelAbility();
		UpdateDistanceOverlay(null);
		UpdateMissionOverlay(null);
	}

	private void LaunchRocket(LaunchConditionManager lcm)
	{
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcm);
		lcm.Launch(spacecraftDestination);
		ClearRocketListPanel();
		FillRocketListPanel();
		ShowRocketListPanel();
		Refresh(null);
	}

	private void FillRocketListPanel()
	{
		ClearRocketListPanel();
		List<Spacecraft> spacecraft = SpacecraftManager.instance.GetSpacecraft();
		if (spacecraft.Count == 0)
		{
			listHeaderStatusLabel.text = UI.STARMAP.NO_ROCKETS_TITLE;
			listNoRocketText.gameObject.SetActive(true);
		}
		else
		{
			listHeaderStatusLabel.text = string.Format(UI.STARMAP.ROCKET_COUNT, spacecraft.Count);
			listNoRocketText.gameObject.SetActive(false);
		}
		foreach (Spacecraft item in spacecraft)
		{
			HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(listRocketTemplate.gameObject, rocketListContainer.gameObject, true);
			BreakdownList component = hierarchyReferences.GetComponent<BreakdownList>();
			MultiToggle component2 = hierarchyReferences.GetComponent<MultiToggle>();
			EditableTitleBar component3 = hierarchyReferences.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
			MultiToggle component4 = hierarchyReferences.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
			MultiToggle component5 = hierarchyReferences.GetReference<RectTransform>("LandRocketButton").GetComponent<MultiToggle>();
			HierarchyReferences component6 = hierarchyReferences.GetReference<RectTransform>("ProgressBar").GetComponent<HierarchyReferences>();
			LaunchConditionManager launchConditionManager = item.launchConditions;
			CommandModule component7 = launchConditionManager.GetComponent<CommandModule>();
			MinionStorage component8 = launchConditionManager.GetComponent<MinionStorage>();
			component3.SetTitle(item.rocketName);
			component3.OnNameChanged += delegate(string newName)
			{
				item.SetRocketName(newName);
			};
			MultiToggle multiToggle = component2;
			multiToggle.onEnter = (System.Action)Delegate.Combine(multiToggle.onEnter, (System.Action)delegate
			{
				LaunchConditionManager launchConditions = item.launchConditions;
				UpdateDistanceOverlay(launchConditions);
				UpdateMissionOverlay(launchConditions);
			});
			MultiToggle multiToggle2 = component2;
			multiToggle2.onExit = (System.Action)Delegate.Combine(multiToggle2.onExit, (System.Action)delegate
			{
				UpdateDistanceOverlay(null);
				UpdateMissionOverlay(null);
			});
			MultiToggle multiToggle3 = component2;
			multiToggle3.onClick = (System.Action)Delegate.Combine(multiToggle3.onClick, (System.Action)delegate
			{
				OnSelectableChanged(item.launchConditions.gameObject);
			});
			component4.play_sound_on_click = false;
			MultiToggle multiToggle4 = component4;
			multiToggle4.onClick = (System.Action)Delegate.Combine(multiToggle4.onClick, (System.Action)delegate
			{
				if ((UnityEngine.Object)launchConditionManager != (UnityEngine.Object)null && selectedDestination != null)
				{
					KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
					LaunchRocket(launchConditionManager);
				}
				else
				{
					KFMOD.PlayOneShot(GlobalAssets.GetSound("Negative", false));
				}
			});
			if ((DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).state != 0)
			{
				component5.gameObject.SetActive(true);
				component5.transform.SetAsLastSibling();
				component5.play_sound_on_click = false;
				MultiToggle multiToggle5 = component5;
				multiToggle5.onClick = (System.Action)Delegate.Combine(multiToggle5.onClick, (System.Action)delegate
				{
					if ((UnityEngine.Object)launchConditionManager != (UnityEngine.Object)null && selectedDestination != null)
					{
						KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
						SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).ForceComplete();
						ClearRocketListPanel();
						FillRocketListPanel();
						ShowRocketListPanel();
						Refresh(null);
					}
					else
					{
						KFMOD.PlayOneShot(GlobalAssets.GetSound("Negative", false));
					}
				});
			}
			else
			{
				component5.gameObject.SetActive(false);
			}
			BreakdownListRow breakdownListRow = component.AddRow();
			string value = UI.STARMAP.MISSION_STATUS.GROUNDED;
			Color dotColor = Color.green;
			switch (item.state)
			{
			case Spacecraft.MissionState.Grounded:
				dotColor = Color.green;
				value = UI.STARMAP.MISSION_STATUS.GROUNDED;
				break;
			case Spacecraft.MissionState.Launching:
				value = UI.STARMAP.MISSION_STATUS.LAUNCHING;
				dotColor = Color.yellow;
				break;
			case Spacecraft.MissionState.WaitingToLand:
				dotColor = Color.yellow;
				value = UI.STARMAP.MISSION_STATUS.WAITING_TO_LAND;
				break;
			case Spacecraft.MissionState.Landing:
				dotColor = Color.yellow;
				value = UI.STARMAP.MISSION_STATUS.LANDING;
				break;
			case Spacecraft.MissionState.Underway:
				dotColor = Color.red;
				value = UI.STARMAP.MISSION_STATUS.UNDERWAY;
				break;
			}
			breakdownListRow.ShowStatusData(UI.STARMAP.ROCKETSTATUS.STATUS, value, dotColor);
			breakdownListRow.SetHighlighted(true);
			if ((UnityEngine.Object)component8 != (UnityEngine.Object)null)
			{
				List<MinionStorage.Info> storedMinionInfo = component8.GetStoredMinionInfo();
				BreakdownListRow breakdownListRow2 = component.AddRow();
				int count = storedMinionInfo.Count;
				breakdownListRow2.ShowStatusData(UI.STARMAP.LISTTITLES.PASSENGERS, count.ToString(), (count != 0) ? Color.green : Color.red);
			}
			if (item.state == Spacecraft.MissionState.Grounded)
			{
				string text = "";
				List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchConditionManager.GetComponent<AttachableBuilding>());
				foreach (GameObject item2 in attachedNetwork)
				{
					text = text + item2.GetProperName() + "\n";
				}
				BreakdownListRow breakdownListRow3 = component.AddRow();
				breakdownListRow3.ShowData(UI.STARMAP.LISTTITLES.MODULES, attachedNetwork.Count.ToString());
				breakdownListRow3.AddTooltip(text);
				BreakdownListRow breakdownListRow4 = component.AddRow();
				breakdownListRow4.ShowData(UI.STARMAP.LISTTITLES.MAXRANGE, DisplayDistance(component7.rocketStats.GetRocketMaxDistance()));
				component4.GetComponent<RectTransform>().SetAsLastSibling();
				component4.gameObject.SetActive(true);
				component6.gameObject.SetActive(false);
			}
			else
			{
				float duration = item.GetDuration();
				float timeLeft = item.GetTimeLeft();
				float num = (duration != 0f) ? (1f - timeLeft / duration) : 0f;
				BreakdownListRow breakdownListRow5 = component.AddRow();
				breakdownListRow5.ShowData(UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration, "F1"));
				component6.gameObject.SetActive(true);
				RectTransform reference = component6.GetReference<RectTransform>("ProgressImage");
				LocText component9 = component6.GetReference<RectTransform>("ProgressText").GetComponent<LocText>();
				reference.transform.localScale = new Vector3(num, 1f, 1f);
				component9.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
				component6.GetComponent<RectTransform>().SetAsLastSibling();
				component4.gameObject.SetActive(false);
			}
			listRocketRows.Add(item, hierarchyReferences);
		}
		UpdateRocketRowsTravelAbility();
	}

	private void ClearRocketListPanel()
	{
		listHeaderStatusLabel.text = UI.STARMAP.NO_ROCKETS_TITLE;
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in listRocketRows)
		{
			UnityEngine.Object.Destroy(listRocketRow.Value.gameObject);
		}
		listRocketRows.Clear();
	}

	private void FillChecklist(LaunchConditionManager launchConditionManager)
	{
		foreach (RocketLaunchCondition launchCondition in launchConditionManager.GetLaunchConditionList())
		{
			BreakdownListRow breakdownListRow = rocketDetailsChecklist.AddRow();
			string launchStatusMessage = launchCondition.GetLaunchStatusMessage(true);
			bool flag = launchCondition.EvaluateLaunchCondition();
			breakdownListRow.ShowCheckmarkData(launchStatusMessage, "", flag);
			if (!flag)
			{
				breakdownListRow.SetHighlighted(true);
			}
			breakdownListRow.AddTooltip(launchCondition.GetLaunchStatusTooltip(flag));
		}
	}

	private void SelectDestination(SpaceDestination destination)
	{
		selectedDestination = destination;
		UnselectAllPlanets();
		if (selectedDestination != null)
		{
			SelectPlanet(planetWidgets[selectedDestination]);
			if ((UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null)
			{
				SpacecraftManager.instance.SetSpacecraftDestination(currentLaunchConditionManager, selectedDestination);
			}
			ShowDestinationPanel();
			UpdateRocketRowsTravelAbility();
		}
		else
		{
			ClearDestinationPanel();
		}
		if ((UnityEngine.Object)rangeRowTotal != (UnityEngine.Object)null && selectedDestination != null && (UnityEngine.Object)currentCommandModule != (UnityEngine.Object)null)
		{
			rangeRowTotal.SetStatusColor((!currentCommandModule.reachable.CanReachDestination(selectedDestination)) ? Color.red : Color.green);
		}
		UpdateDestinationStates();
		Refresh(null);
	}

	private void UpdateRocketRowsTravelAbility()
	{
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in listRocketRows)
		{
			Spacecraft key = listRocketRow.Key;
			LaunchConditionManager launchConditions = key.launchConditions;
			CommandModule component = launchConditions.GetComponent<CommandModule>();
			HierarchyReferences value = listRocketRow.Value;
			MultiToggle component2 = value.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
			bool flag = key.state == Spacecraft.MissionState.Grounded;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(launchConditions);
			bool flag2 = spacecraftDestination != null && component.reachable.CanReachDestination(spacecraftDestination);
			bool flag3 = launchConditions.CheckReadyToLaunch();
			component2.ChangeState((!flag || !flag2 || !flag3) ? 1 : 0);
		}
	}

	private void RefreshAnalyzeButton()
	{
		if (selectedDestination == null)
		{
			analyzeButton.ChangeState(1);
			analyzeButton.onClick = null;
			analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.NO_ANALYZABLE_DESTINATION_SELECTED;
		}
		else if (selectedDestination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			if (DebugHandler.InstantBuildMode)
			{
				analyzeButton.ChangeState(0);
				analyzeButton.onClick = delegate
				{
					selectedDestination.TryCompleteResearchOpportunity();
					ShowDestinationPanel();
				};
				analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYSIS_COMPLETE + " (debug research)";
			}
			else
			{
				analyzeButton.ChangeState(1);
				analyzeButton.onClick = null;
				analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYSIS_COMPLETE;
			}
		}
		else
		{
			analyzeButton.ChangeState(0);
			if (selectedDestination.id == SpacecraftManager.instance.GetStarmapAnalysisDestinationID())
			{
				analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.SUSPEND_DESTINATION_ANALYSIS;
				analyzeButton.onClick = delegate
				{
					SpacecraftManager.instance.SetStarmapAnalysisDestinationID(-1);
				};
			}
			else
			{
				analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYZE_DESTINATION;
				analyzeButton.onClick = delegate
				{
					if (DebugHandler.InstantBuildMode)
					{
						SpacecraftManager.instance.SetStarmapAnalysisDestinationID(selectedDestination.id);
						SpacecraftManager.instance.EarnDestinationAnalysisPoints(selectedDestination.id, 99999f);
						ShowDestinationPanel();
					}
					else
					{
						SpacecraftManager.instance.SetStarmapAnalysisDestinationID(selectedDestination.id);
					}
				};
			}
		}
	}

	private void Refresh(object data = null)
	{
		FillRocketListPanel();
		RefreshAnalyzeButton();
		if ((UnityEngine.Object)currentCommandModule != (UnityEngine.Object)null && (UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null)
		{
			FillRocketPanel();
			if (selectedDestination != null)
			{
				ValidateTravelAbility();
			}
		}
		else
		{
			ClearRocketPanel();
		}
	}

	private void ClearRocketPanel()
	{
		rocketHeaderStatusLabel.text = UI.STARMAP.ROCKETSTATUS.NONE;
		rocketDetailsChecklist.ClearRows();
		rocketDetailsMass.ClearRows();
		rocketDetailsRange.ClearRows();
		rocketThrustWidget.gameObject.SetActive(false);
		rocketDetailsStorage.ClearRows();
		rocketDetailsFuel.ClearRows();
		rocketDetailsOxidizer.ClearRows();
		rocketDetailsDupes.ClearRows();
		rocketDetailsStatus.ClearRows();
		currentRocketHasLiquidContainer = false;
		currentRocketHasGasContainer = false;
		currentRocketHasSolidContainer = false;
		currentRocketHasEntitiesContainer = false;
		LayoutRebuilder.ForceRebuildLayoutImmediate(rocketDetailsContainer);
	}

	private void FillRocketPanel()
	{
		ClearRocketPanel();
		rocketHeaderStatusLabel.text = UI.STARMAP.STATUS;
		UpdateDistanceOverlay(null);
		UpdateMissionOverlay(null);
		FillChecklist(currentLaunchConditionManager);
		UpdateRangeDisplay();
		UpdateMassDisplay();
		UpdateOxidizerDisplay();
		UpdateStorageDisplay();
		UpdateFuelDisplay();
		LayoutRebuilder.ForceRebuildLayoutImmediate(rocketDetailsContainer);
	}

	private void UpdateRangeDisplay()
	{
		BreakdownListRow breakdownListRow = rocketDetailsRange.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZABLE_FUEL, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalOxidizableFuel(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow2 = rocketDetailsRange.AddRow();
		breakdownListRow2.ShowData(UI.STARMAP.ROCKETSTATS.ENGINE_EFFICIENCY, GameUtil.GetFormattedEngineEfficiency(currentCommandModule.rocketStats.GetEngineEfficiency()));
		BreakdownListRow breakdownListRow3 = rocketDetailsRange.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.OXIDIZER_EFFICIENCY, GameUtil.GetFormattedPercent(currentCommandModule.rocketStats.GetAverageOxidizerEfficiency(), GameUtil.TimeSlice.None));
		float num = currentCommandModule.rocketStats.GetBoosterThrust() * 1000f;
		if (num != 0f)
		{
			BreakdownListRow breakdownListRow4 = rocketDetailsRange.AddRow();
			breakdownListRow4.ShowData(UI.STARMAP.ROCKETSTATS.SOLID_BOOSTER, GameUtil.GetFormattedDistance(num));
		}
		BreakdownListRow breakdownListRow5 = rocketDetailsRange.AddRow();
		breakdownListRow5.ShowStatusData(UI.STARMAP.ROCKETSTATS.TOTAL_THRUST, GameUtil.GetFormattedDistance(currentCommandModule.rocketStats.GetTotalThrust() * 1000f), Color.green);
		breakdownListRow5.SetImportant(true);
		float distance = 0f - (currentCommandModule.rocketStats.GetTotalThrust() - currentCommandModule.rocketStats.GetRocketMaxDistance());
		rocketThrustWidget.gameObject.SetActive(true);
		BreakdownListRow breakdownListRow6 = rocketDetailsRange.AddRow();
		breakdownListRow6.ShowStatusData(UI.STARMAP.ROCKETSTATUS.WEIGHTPENALTY, DisplayDistance(distance), Color.red);
		breakdownListRow6.SetHighlighted(true);
		rocketDetailsRange.AddCustomRow(rocketThrustWidget.gameObject);
		rocketThrustWidget.Draw(currentCommandModule);
		BreakdownListRow breakdownListRow7 = rocketDetailsRange.AddRow();
		breakdownListRow7.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_RANGE, GameUtil.GetFormattedDistance(currentCommandModule.rocketStats.GetRocketMaxDistance() * 1000f));
		breakdownListRow7.SetImportant(true);
	}

	private void UpdateMassDisplay()
	{
		BreakdownListRow breakdownListRow = rocketDetailsMass.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATS.DRY_MASS, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetDryMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow2 = rocketDetailsMass.AddRow();
		breakdownListRow2.ShowData(UI.STARMAP.ROCKETSTATS.WET_MASS, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetWetMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow3 = rocketDetailsMass.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow3.SetImportant(true);
	}

	private void UpdateFuelDisplay()
	{
		Tag engineFuelTag = currentCommandModule.rocketStats.GetEngineFuelTag();
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			FuelTank component = item.GetComponent<FuelTank>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				BreakdownListRow breakdownListRow = rocketDetailsFuel.AddRow();
				breakdownListRow.ShowData(item.gameObject.GetProperName() + " (" + ElementLoader.GetElement(engineFuelTag).name + ")", GameUtil.GetFormattedMass(component.GetAmountAvailable(engineFuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
			SolidBooster component2 = item.GetComponent<SolidBooster>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				BreakdownListRow breakdownListRow2 = rocketDetailsFuel.AddRow();
				breakdownListRow2.ShowData(item.gameObject.GetProperName() + " (" + ElementLoader.GetElement(component2.fuelTag).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(component2.fuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
		BreakdownListRow breakdownListRow3 = rocketDetailsFuel.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_FUEL, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalFuel(true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow3.SetImportant(true);
	}

	private void UpdateOxidizerDisplay()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = item.GetComponent<OxidizerTank>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				Dictionary<Tag, float> oxidizersAvailable = component.GetOxidizersAvailable();
				foreach (KeyValuePair<Tag, float> item2 in oxidizersAvailable)
				{
					if (item2.Value != 0f)
					{
						BreakdownListRow breakdownListRow = rocketDetailsOxidizer.AddRow();
						breakdownListRow.ShowData(item.gameObject.GetProperName() + " (" + item2.Key.ProperName() + ")", GameUtil.GetFormattedMass(item2.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
					}
				}
			}
			SolidBooster component2 = item.GetComponent<SolidBooster>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				BreakdownListRow breakdownListRow2 = rocketDetailsOxidizer.AddRow();
				breakdownListRow2.ShowData(item.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.OxyRock).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
		BreakdownListRow breakdownListRow3 = rocketDetailsOxidizer.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZER, GameUtil.GetFormattedMass(currentCommandModule.rocketStats.GetTotalOxidizer(true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow3.SetImportant(true);
	}

	private void UpdateStorageDisplay()
	{
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = item.GetComponent<CargoBay>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				BreakdownListRow breakdownListRow = rocketDetailsStorage.AddRow();
				breakdownListRow.ShowData(item.gameObject.GetProperName(), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
	}

	private void ClearDestinationPanel()
	{
		destinationDetailsContainer.gameObject.SetActive(false);
		destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.NONE;
	}

	private void ShowDestinationPanel()
	{
		destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.SELECTED;
		if ((UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(currentLaunchConditionManager);
			if (spacecraftFromLaunchConditionManager.state != 0)
			{
				destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.LOCKEDIN;
			}
		}
		SpaceDestinationType destinationType = selectedDestination.GetDestinationType();
		destinationNameLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) != SpacecraftManager.DestinationAnalysisState.Complete) ? UI.STARMAP.UNKNOWN_DESTINATION.text : destinationType.Name);
		destinationTypeValueLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) != SpacecraftManager.DestinationAnalysisState.Complete) ? UI.STARMAP.UNKNOWN_TYPE.text : destinationType.typeName);
		destinationDistanceValueLabel.text = DisplayDistance((float)selectedDestination.OneBasedDistance * 10000f);
		destinationDescriptionLabel.text = destinationType.description;
		destinationDetailsComposition.ClearRows();
		float num = 0f;
		if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (KeyValuePair<SimHashes, float> recoverableElement in selectedDestination.recoverableElements)
			{
				num += selectedDestination.GetResourceValue(recoverableElement.Key, recoverableElement.Value);
			}
		}
		destinationDetailsResearch.ClearRows();
		if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (SpaceDestination.ResearchOpportunity researchOpportunity in selectedDestination.researchOpportunities)
			{
				BreakdownListRow breakdownListRow = destinationDetailsResearch.AddRow();
				string name = (researchOpportunity.discoveredRareResource == SimHashes.Void) ? researchOpportunity.description : $"(!!) {researchOpportunity.description}";
				breakdownListRow.ShowCheckmarkData(name, researchOpportunity.dataValue.ToString(), researchOpportunity.completed);
			}
		}
		destinationAnalysisProgressBar.SetFillPercentage(SpacecraftManager.instance.GetDestinationAnalysisScore(selectedDestination.id) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
		if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (KeyValuePair<SimHashes, float> recoverableElement2 in selectedDestination.recoverableElements)
			{
				BreakdownListRow breakdownListRow2 = destinationDetailsComposition.AddRow();
				float num2 = selectedDestination.GetResourceValue(recoverableElement2.Key, recoverableElement2.Value) / num * 100f;
				Element element = ElementLoader.FindElementByHash(recoverableElement2.Key);
				Tuple<Sprite, Color> uISprite = Def.GetUISprite(element, "ui", false);
				if (num2 <= 1f)
				{
					breakdownListRow2.ShowIconData(element.name, UI.STARMAP.COMPOSITION_SMALL_AMOUNT, uISprite.first, uISprite.second);
				}
				else
				{
					breakdownListRow2.ShowIconData(element.name, GameUtil.GetFormattedPercent(num2, GameUtil.TimeSlice.None), uISprite.first, uISprite.second);
				}
				if (element.IsGas)
				{
					string properName = Assets.GetPrefab("GasCargoBay".ToTag()).GetProperName();
					if (currentRocketHasGasContainer)
					{
						breakdownListRow2.SetHighlighted(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName));
					}
					else
					{
						breakdownListRow2.SetDisabled(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName));
					}
				}
				if (element.IsLiquid)
				{
					string properName2 = Assets.GetPrefab("LiquidCargoBay".ToTag()).GetProperName();
					if (currentRocketHasLiquidContainer)
					{
						breakdownListRow2.SetHighlighted(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName2));
					}
					else
					{
						breakdownListRow2.SetDisabled(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName2));
					}
				}
				if (element.IsSolid)
				{
					string properName3 = Assets.GetPrefab("CargoBay".ToTag()).GetProperName();
					if (currentRocketHasSolidContainer)
					{
						breakdownListRow2.SetHighlighted(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName3));
					}
					else
					{
						breakdownListRow2.SetDisabled(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName3));
					}
				}
			}
			foreach (SpaceDestination.ResearchOpportunity researchOpportunity2 in selectedDestination.researchOpportunities)
			{
				if (!researchOpportunity2.completed && researchOpportunity2.discoveredRareResource != SimHashes.Void)
				{
					BreakdownListRow breakdownListRow3 = destinationDetailsComposition.AddRow();
					breakdownListRow3.ShowData(UI.STARMAP.COMPOSITION_UNDISCOVERED, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
					breakdownListRow3.SetDisabled(true);
					breakdownListRow3.AddTooltip(UI.STARMAP.COMPOSITION_UNDISCOVERED_TOOLTIP);
				}
			}
		}
		destinationDetailsResources.ClearRows();
		if (SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (KeyValuePair<Tag, int> recoverableEntity in selectedDestination.GetRecoverableEntities())
			{
				BreakdownListRow breakdownListRow4 = destinationDetailsResources.AddRow();
				GameObject prefab = Assets.GetPrefab(recoverableEntity.Key);
				Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(prefab, "ui", false);
				breakdownListRow4.ShowIconData(prefab.GetProperName(), "", uISprite2.first, uISprite2.second);
				string properName4 = Assets.GetPrefab("SpecialCargoBay".ToTag()).GetProperName();
				if (currentRocketHasEntitiesContainer)
				{
					breakdownListRow4.SetHighlighted(true);
					breakdownListRow4.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, prefab.GetProperName(), properName4));
				}
				else
				{
					breakdownListRow4.SetDisabled(true);
					breakdownListRow4.AddTooltip(string.Format(UI.STARMAP.CANT_CARRY_ELEMENT, properName4, prefab.GetProperName()));
				}
			}
		}
		destinationDetailsContainer.gameObject.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(destinationDetailsContainer);
	}

	private void ValidateTravelAbility()
	{
		if (selectedDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete && (UnityEngine.Object)currentCommandModule != (UnityEngine.Object)null && (UnityEngine.Object)currentLaunchConditionManager != (UnityEngine.Object)null)
		{
			launchButton.ChangeState((!currentLaunchConditionManager.CheckReadyToLaunch()) ? 1 : 0);
		}
	}

	private void UpdateDistanceOverlay(LaunchConditionManager lcmToVisualize = null)
	{
		if ((UnityEngine.Object)lcmToVisualize == (UnityEngine.Object)null)
		{
			lcmToVisualize = currentLaunchConditionManager;
		}
		Spacecraft spacecraft = null;
		if ((UnityEngine.Object)lcmToVisualize != (UnityEngine.Object)null)
		{
			spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
		}
		if ((UnityEngine.Object)lcmToVisualize != (UnityEngine.Object)null && spacecraft != null && spacecraft.state == Spacecraft.MissionState.Grounded)
		{
			distanceOverlay.gameObject.SetActive(true);
			CommandModule component = lcmToVisualize.GetComponent<CommandModule>();
			float rocketMaxDistance = component.rocketStats.GetRocketMaxDistance();
			rocketMaxDistance = (float)(int)(rocketMaxDistance / 10000f) * 10000f;
			Vector2 sizeDelta = distanceOverlay.rectTransform.sizeDelta;
			sizeDelta.x = rowsContiner.rect.width;
			sizeDelta.y = (1f - rocketMaxDistance / planetsMaxDistance) * rowsContiner.rect.height + (float)distanceOverlayYOffset + (float)distanceOverlayVerticalOffset;
			distanceOverlay.rectTransform.sizeDelta = sizeDelta;
			distanceOverlay.rectTransform.anchoredPosition = new Vector3(0f, (float)distanceOverlayVerticalOffset, 0f);
		}
		else
		{
			distanceOverlay.gameObject.SetActive(false);
		}
	}

	private void UpdateMissionOverlay(LaunchConditionManager lcmToVisualize = null)
	{
		if ((UnityEngine.Object)lcmToVisualize == (UnityEngine.Object)null)
		{
			lcmToVisualize = currentLaunchConditionManager;
		}
		Spacecraft spacecraft = null;
		if ((UnityEngine.Object)lcmToVisualize != (UnityEngine.Object)null)
		{
			spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
		}
		if ((UnityEngine.Object)lcmToVisualize != (UnityEngine.Object)null && spacecraft != null)
		{
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcmToVisualize);
			if (spacecraftDestination == null)
			{
				Debug.Log("destination is null", null);
			}
			else
			{
				StarmapPlanet starmapPlanet = planetWidgets[spacecraftDestination];
				if (spacecraft == null)
				{
					Debug.Log("craft is null", null);
				}
				else if ((UnityEngine.Object)starmapPlanet == (UnityEngine.Object)null)
				{
					Debug.Log("planet is null", null);
				}
				else
				{
					UnselectAllPlanets();
					SelectPlanet(starmapPlanet);
					starmapPlanet.ShowAsCurrentRocketDestination(true);
					if (spacecraft.state != 0)
					{
						visualizeRocketImage.gameObject.SetActive(true);
						visualizeRocketTrajectory.gameObject.SetActive(true);
						visualizeRocketLabel.gameObject.SetActive(true);
						visualizeRocketProgress.gameObject.SetActive(true);
						float duration = spacecraft.GetDuration();
						float timeLeft = spacecraft.GetTimeLeft();
						float num = (duration != 0f) ? (1f - timeLeft / duration) : 0f;
						bool flag = num > 0.5f;
						Vector2 size = rowsContiner.rect.size;
						Vector2 vector = new Vector2(0f, 0f - size.y);
						Vector3 localPosition = starmapPlanet.rectTransform().localPosition;
						Vector2 sizeDelta = starmapPlanet.rectTransform().sizeDelta;
						Vector3 b = localPosition + new Vector3(sizeDelta.x * 0.5f, 0f, 0f);
						b = starmapPlanet.transform.parent.rectTransform().localPosition + b;
						Vector2 vector2 = new Vector2(b.x, b.y);
						float num2 = Vector2.Distance(vector, vector2);
						Vector2 vector3 = vector2 - vector;
						float num3 = Mathf.Atan2(vector3.y, vector3.x);
						float z = num3 * 57.29578f;
						Vector2 v = flag ? new Vector2(Mathf.Lerp(vector.x, vector2.x, 1f - num * 2f + 1f), Mathf.Lerp(vector.y, vector2.y, 1f - num * 2f + 1f)) : new Vector2(Mathf.Lerp(vector.x, vector2.x, num * 2f), Mathf.Lerp(vector.y, vector2.y, num * 2f));
						visualizeRocketLabel.text = spacecraft.state.ToString();
						visualizeRocketProgress.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
						visualizeRocketTrajectory.transform.SetLocalPosition(vector);
						RectTransform rectTransform = visualizeRocketTrajectory.rectTransform;
						float x = num2;
						Vector2 sizeDelta2 = visualizeRocketTrajectory.rectTransform.sizeDelta;
						rectTransform.sizeDelta = new Vector2(x, sizeDelta2.y);
						visualizeRocketTrajectory.rectTransform.localRotation = Quaternion.Euler(0f, 0f, z);
						visualizeRocketImage.transform.SetLocalPosition(v);
					}
				}
			}
		}
		else
		{
			if (selectedDestination != null && planetWidgets.ContainsKey(selectedDestination))
			{
				UnselectAllPlanets();
				StarmapPlanet planet = planetWidgets[selectedDestination];
				SelectPlanet(planet);
			}
			else
			{
				UnselectAllPlanets();
			}
			visualizeRocketImage.gameObject.SetActive(false);
			visualizeRocketTrajectory.gameObject.SetActive(false);
			visualizeRocketLabel.gameObject.SetActive(false);
			visualizeRocketProgress.gameObject.SetActive(false);
		}
	}
}
