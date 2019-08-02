using FMOD.Studio;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DetailsScreen : KTabMenu
{
	[Serializable]
	private struct Screens
	{
		public string name;

		public string displayName;

		public string tooltip;

		public Sprite icon;

		public TargetScreen screen;

		public int displayOrderPriority;

		public bool hideWhenDead;

		public HashedString focusInViewMode;

		[HideInInspector]
		public int tabIdx;
	}

	[Serializable]
	public class SideScreenRef
	{
		public string name;

		public SideScreenContent screenPrefab;

		public Vector2 offset;

		[HideInInspector]
		public SideScreenContent screenInstance;
	}

	public static DetailsScreen Instance;

	[SerializeField]
	private KButton CodexEntryButton;

	[Header("Panels")]
	public Transform UserMenuPanel;

	[Header("Name Editing (disabled)")]
	[SerializeField]
	private KButton CloseButton;

	[Header("Tabs")]
	[SerializeField]
	private EditableTitleBar TabTitle;

	[SerializeField]
	private Screens[] screens;

	[SerializeField]
	private GameObject tabHeaderContainer;

	[Header("Side Screens")]
	[SerializeField]
	private GameObject sideScreenContentBody;

	[SerializeField]
	private GameObject sideScreen;

	[SerializeField]
	private LocText sideScreenTitle;

	[SerializeField]
	private List<SideScreenRef> sideScreens;

	[Header("Secondary Side Screens")]
	[SerializeField]
	private GameObject sideScreen2ContentBody;

	[SerializeField]
	private GameObject sideScreen2;

	[SerializeField]
	private LocText sideScreen2Title;

	private KScreen activeSideScreen2;

	private bool HasActivated;

	private bool isEditing;

	private SideScreenContent currentSideScreen;

	private static readonly EventSystem.IntraObjectHandler<DetailsScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DetailsScreen>(delegate(DetailsScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	public GameObject target
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public override float GetSortKey()
	{
		if (isEditing)
		{
			return 10f;
		}
		return base.GetSortKey();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SortScreenOrder();
		ConsumeMouseScroll = true;
		Debug.Assert((UnityEngine.Object)Instance == (UnityEngine.Object)null);
		Instance = this;
		UIRegistry.detailsScreen = this;
		DeactivateSideContent();
		Show(false);
		Subscribe(Game.Instance.gameObject, -1503271301, OnSelectObject);
	}

	private void OnSelectObject(object data)
	{
		if (data == null)
		{
			previouslyActiveTab = -1;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CodexEntryButton.onClick += OpenCodexEntry;
		CloseButton.onClick += DeselectAndClose;
		TabTitle.OnNameChanged += OnNameChanged;
		TabTitle.OnStartedEditing += OnStartedEditing;
		sideScreen2.SetActive(false);
		Subscribe(-1514841199, OnRefreshDataDelegate);
	}

	private void OnStartedEditing()
	{
		isEditing = true;
		KScreenManager.Instance.RefreshStack();
	}

	private void OnNameChanged(string newName)
	{
		isEditing = false;
		if (!string.IsNullOrEmpty(newName))
		{
			MinionIdentity component = target.GetComponent<MinionIdentity>();
			StorageLocker component2 = target.GetComponent<StorageLocker>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.SetName(newName);
			}
			else if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				component2.SetName(newName);
			}
		}
	}

	protected override void OnDeactivate()
	{
		DeactivateSideContent();
		base.OnDeactivate();
	}

	protected override void OnShow(bool show)
	{
		if (!show)
		{
			DeactivateSideContent();
		}
		else
		{
			MaskSideContent(false);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenHalfEffect);
		}
		base.OnShow(show);
	}

	protected override void OnCmpDisable()
	{
		DeactivateSideContent();
		base.OnCmpDisable();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!isEditing && (UnityEngine.Object)target != (UnityEngine.Object)null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			DeselectAndClose();
		}
	}

	private static Component GetComponent(GameObject go, string name)
	{
		Component component = null;
		Type type = Type.GetType(name);
		if (type == null)
		{
			return go.GetComponent(name);
		}
		return go.GetComponent(type);
	}

	private static bool IsExcludedPrefabTag(GameObject go, Tag[] excluded_tags)
	{
		if (excluded_tags == null || excluded_tags.Length == 0)
		{
			return false;
		}
		bool result = false;
		KPrefabID component = go.GetComponent<KPrefabID>();
		foreach (Tag b in excluded_tags)
		{
			if (component.PrefabTag == b)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void UpdateCodexButton()
	{
		string selectedObjectCodexID = GetSelectedObjectCodexID();
		CodexEntryButton.isInteractable = (selectedObjectCodexID != string.Empty);
		CodexEntryButton.GetComponent<ToolTip>().SetSimpleTooltip((!CodexEntryButton.isInteractable) ? UI.TOOLTIPS.NO_CODEX_ENTRY : UI.TOOLTIPS.OPEN_CODEX_ENTRY);
	}

	public void OnRefreshData(object obj)
	{
		SetTitle(base.PreviousActiveTab);
		for (int i = 0; i < tabs.Count; i++)
		{
			if (tabs[i].gameObject.activeInHierarchy)
			{
				tabs[i].Trigger(-1514841199, obj);
			}
		}
	}

	public void Refresh(GameObject go)
	{
		if (screens != null)
		{
			target = go;
			CellSelectionObject component = target.GetComponent<CellSelectionObject>();
			if ((bool)component)
			{
				component.OnObjectSelected(null);
			}
			if (!HasActivated)
			{
				if (screens != null)
				{
					for (int i = 0; i < screens.Length; i++)
					{
						GameObject gameObject = null;
						gameObject = KScreenManager.Instance.InstantiateScreen(screens[i].screen.gameObject, body.gameObject).gameObject;
						screens[i].screen = gameObject.GetComponent<TargetScreen>();
						screens[i].tabIdx = AddTab(screens[i].icon, Strings.Get(screens[i].displayName), screens[i].screen, Strings.Get(screens[i].tooltip));
					}
				}
				base.onTabActivated += OnTabActivated;
				HasActivated = true;
			}
			int num = -1;
			int num2 = 0;
			for (int j = 0; j < screens.Length; j++)
			{
				bool flag = screens[j].screen.IsValidForTarget(go);
				bool flag2 = screens[j].hideWhenDead && base.gameObject.HasTag(GameTags.Dead);
				bool flag3 = flag && !flag2;
				SetTabEnabled(screens[j].tabIdx, flag3);
				if (flag3)
				{
					num2++;
					if (num == -1)
					{
						if (SimDebugView.Instance.GetMode() != OverlayModes.None.ID)
						{
							if (SimDebugView.Instance.GetMode() == screens[j].focusInViewMode)
							{
								num = j;
							}
						}
						else if (flag3 && previouslyActiveTab >= 0 && previouslyActiveTab < screens.Length && screens[j].name == screens[previouslyActiveTab].name)
						{
							num = screens[j].tabIdx;
						}
					}
				}
			}
			if (num != -1)
			{
				ActivateTab(num);
			}
			else
			{
				ActivateTab(0);
			}
			tabHeaderContainer.gameObject.SetActive((CountTabs() > 1) ? true : false);
			if (sideScreens != null && sideScreens.Count > 0)
			{
				sideScreens.ForEach(delegate(SideScreenRef scn)
				{
					if (scn.screenPrefab.IsValidForTarget(target))
					{
						if ((UnityEngine.Object)scn.screenInstance == (UnityEngine.Object)null)
						{
							scn.screenInstance = Util.KInstantiateUI<SideScreenContent>(scn.screenPrefab.gameObject, sideScreenContentBody, false);
						}
						if (!sideScreen.activeInHierarchy)
						{
							sideScreen.SetActive(true);
						}
						scn.screenInstance.transform.SetAsFirstSibling();
						scn.screenInstance.SetTarget(target);
						scn.screenInstance.Show(true);
						currentSideScreen = scn.screenInstance;
						RefreshTitle();
					}
				});
			}
		}
	}

	public void RefreshTitle()
	{
		if ((bool)currentSideScreen)
		{
			sideScreenTitle.SetText(currentSideScreen.GetTitle());
		}
	}

	private void OnTabActivated(int newTab, int oldTab)
	{
		SetTitle(newTab);
		if (oldTab != -1)
		{
			screens[oldTab].screen.SetTarget(null);
		}
		if (newTab != -1)
		{
			screens[newTab].screen.SetTarget(target);
		}
	}

	public KScreen SetSecondarySideScreen(KScreen secondaryPrefab, string title)
	{
		ClearSecondarySideScreen();
		activeSideScreen2 = KScreenManager.Instance.InstantiateScreen(secondaryPrefab.gameObject, sideScreen2ContentBody);
		activeSideScreen2.Activate();
		sideScreen2Title.text = title;
		sideScreen2.SetActive(true);
		return activeSideScreen2;
	}

	public void ClearSecondarySideScreen()
	{
		if ((UnityEngine.Object)activeSideScreen2 != (UnityEngine.Object)null)
		{
			activeSideScreen2.Deactivate();
			activeSideScreen2 = null;
		}
		sideScreen2.SetActive(false);
	}

	public void DeactivateSideContent()
	{
		if ((UnityEngine.Object)SideDetailsScreen.Instance != (UnityEngine.Object)null && SideDetailsScreen.Instance.gameObject.activeInHierarchy)
		{
			SideDetailsScreen.Instance.Show(false);
		}
		if (sideScreens != null && sideScreens.Count > 0)
		{
			sideScreens.ForEach(delegate(SideScreenRef scn)
			{
				if ((UnityEngine.Object)scn.screenInstance != (UnityEngine.Object)null)
				{
					scn.screenInstance.ClearTarget();
					scn.screenInstance.Show(false);
				}
			});
		}
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenHalfEffect, STOP_MODE.ALLOWFADEOUT);
		sideScreen.SetActive(false);
	}

	public void MaskSideContent(bool hide)
	{
		if (hide)
		{
			sideScreen.transform.localScale = Vector3.zero;
		}
		else
		{
			sideScreen.transform.localScale = Vector3.one;
		}
	}

	private string GetSelectedObjectCodexID()
	{
		string text = string.Empty;
		CellSelectionObject component = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
		BuildingUnderConstruction component2 = SelectTool.Instance.selected.GetComponent<BuildingUnderConstruction>();
		CreatureBrain component3 = SelectTool.Instance.selected.GetComponent<CreatureBrain>();
		PlantableSeed component4 = SelectTool.Instance.selected.GetComponent<PlantableSeed>();
		BudUprootedMonitor component5 = SelectTool.Instance.selected.GetComponent<BudUprootedMonitor>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			text = CodexCache.FormatLinkID(component.element.id.ToString());
		}
		else if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			text = CodexCache.FormatLinkID(component2.Def.PrefabID);
		}
		else if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
		{
			text = CodexCache.FormatLinkID(SelectTool.Instance.selected.PrefabID().ToString());
			text = text.Replace("BABY", string.Empty);
		}
		else if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			text = CodexCache.FormatLinkID(SelectTool.Instance.selected.PrefabID().ToString());
			text = text.Replace("SEED", string.Empty);
		}
		else if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)component5.parentObject.Get() != (UnityEngine.Object)null)
			{
				text = CodexCache.FormatLinkID(component5.parentObject.Get().PrefabID().ToString());
			}
			else if ((UnityEngine.Object)component5.GetComponent<TreeBud>() != (UnityEngine.Object)null)
			{
				text = CodexCache.FormatLinkID(component5.GetComponent<TreeBud>().buddingTrunk.Get().PrefabID().ToString());
			}
		}
		else
		{
			text = CodexCache.FormatLinkID(SelectTool.Instance.selected.PrefabID().ToString());
		}
		if (CodexCache.entries.ContainsKey(text) || CodexCache.FindSubEntry(text) != null)
		{
			return text;
		}
		return string.Empty;
	}

	public void OpenCodexEntry()
	{
		string selectedObjectCodexID = GetSelectedObjectCodexID();
		if (selectedObjectCodexID != string.Empty)
		{
			ManagementMenu.Instance.OpenCodexToEntry(selectedObjectCodexID);
		}
	}

	public void DeselectAndClose()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back", false));
		SelectTool.Instance.Select(null, false);
		if (!((UnityEngine.Object)target == (UnityEngine.Object)null))
		{
			target = null;
			DeactivateSideContent();
			Show(false);
		}
	}

	private void SortScreenOrder()
	{
		Array.Sort(screens, (Screens x, Screens y) => x.displayOrderPriority.CompareTo(y.displayOrderPriority));
	}

	public void UpdatePortrait(GameObject target)
	{
		KSelectable component = target.GetComponent<KSelectable>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			TabTitle.portrait.ClearPortrait();
			Building component2 = component.GetComponent<Building>();
			if ((bool)component2)
			{
				Sprite sprite = null;
				sprite = component2.Def.GetUISprite("ui", false);
				if ((UnityEngine.Object)sprite != (UnityEngine.Object)null)
				{
					TabTitle.portrait.SetPortrait(sprite);
					return;
				}
			}
			MinionIdentity component3 = target.GetComponent<MinionIdentity>();
			if ((bool)component3)
			{
				TabTitle.SetPortrait(component.gameObject);
			}
			else
			{
				Edible component4 = target.GetComponent<Edible>();
				if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
				{
					KBatchedAnimController component5 = component4.GetComponent<KBatchedAnimController>();
					Sprite uISpriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component5.AnimFiles[0], "ui", false, string.Empty);
					TabTitle.portrait.SetPortrait(uISpriteFromMultiObjectAnim);
				}
				else
				{
					PrimaryElement component6 = target.GetComponent<PrimaryElement>();
					if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
					{
						TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(ElementLoader.FindElementByHash(component6.ElementID).substance.anim, "ui", false, string.Empty));
					}
					else
					{
						CellSelectionObject component7 = target.GetComponent<CellSelectionObject>();
						if ((UnityEngine.Object)component7 != (UnityEngine.Object)null)
						{
							string animName = (!component7.element.IsSolid) ? component7.element.substance.name : "ui";
							Sprite uISpriteFromMultiObjectAnim2 = Def.GetUISpriteFromMultiObjectAnim(component7.element.substance.anim, animName, false, string.Empty);
							TabTitle.portrait.SetPortrait(uISpriteFromMultiObjectAnim2);
						}
					}
				}
			}
		}
	}

	public bool CompareTargetWith(GameObject compare)
	{
		return (UnityEngine.Object)target == (UnityEngine.Object)compare;
	}

	public void SetTitle(int selectedTabIndex)
	{
		UpdateCodexButton();
		if ((UnityEngine.Object)TabTitle != (UnityEngine.Object)null)
		{
			TabTitle.SetTitle(target.GetProperName());
			MinionIdentity minionIdentity = null;
			StorageLocker x = null;
			if ((UnityEngine.Object)target != (UnityEngine.Object)null)
			{
				minionIdentity = target.gameObject.GetComponent<MinionIdentity>();
				x = target.gameObject.GetComponent<StorageLocker>();
			}
			if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
			{
				TabTitle.SetSubText(minionIdentity.GetComponent<MinionResume>().GetSkillsSubtitle(), string.Empty);
				TabTitle.SetUserEditable(true);
			}
			else if ((UnityEngine.Object)x != (UnityEngine.Object)null)
			{
				TabTitle.SetSubText(string.Empty, string.Empty);
				TabTitle.SetUserEditable(true);
			}
			else
			{
				TabTitle.SetSubText(string.Empty, string.Empty);
				TabTitle.SetUserEditable(false);
			}
		}
	}

	public void SetTitle(string title)
	{
		TabTitle.SetTitle(title);
	}

	public TargetScreen GetActiveTab()
	{
		if (previouslyActiveTab >= 0 && previouslyActiveTab < screens.Length)
		{
			return screens[previouslyActiveTab].screen;
		}
		return null;
	}
}
