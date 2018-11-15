using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RolesScreen : KModalScreen
{
	public new const float SCREEN_SORT_KEY = 101f;

	private Dictionary<string, GameObject> roleWidgets = new Dictionary<string, GameObject>();

	private List<GameObject> tierColumns = new List<GameObject>();

	[SerializeField]
	private GameObject unassignedMinionList;

	[SerializeField]
	private GameObject assignedMinionList;

	[SerializeField]
	private GameObject tierTableHorizontalLayout;

	[SerializeField]
	private GameObject rootContent;

	[SerializeField]
	private GameObject Prefab_MinionWidget;

	[SerializeField]
	private GameObject Prefab_RoleWidget;

	[SerializeField]
	private GameObject Prefab_RoleSlot;

	[SerializeField]
	private GameObject Prefab_tierColumn;

	[SerializeField]
	private GameObject Prefab_Slot;

	[SerializeField]
	private KButton CloseButton;

	[SerializeField]
	private GameObject SlotWidgetPool;

	[SerializeField]
	private MultiToggle toggleAutoPrioritize;

	private List<GameObject> freeWidgetSlots = new List<GameObject>();

	private int HEADER_HEIGHT = 192;

	private bool dirty;

	private bool layoutRoles = true;

	private bool linesPending;

	[HideInInspector]
	public MinionResume activeResume;

	public Sprite[] TierIcons;

	public static readonly string[] tierNames = new string[10]
	{
		UI.ROLES_SCREEN.TIER_NAMES.ZERO,
		UI.ROLES_SCREEN.TIER_NAMES.ONE,
		UI.ROLES_SCREEN.TIER_NAMES.TWO,
		UI.ROLES_SCREEN.TIER_NAMES.THREE,
		UI.ROLES_SCREEN.TIER_NAMES.FOUR,
		UI.ROLES_SCREEN.TIER_NAMES.FIVE,
		UI.ROLES_SCREEN.TIER_NAMES.SIX,
		UI.ROLES_SCREEN.TIER_NAMES.SEVEN,
		UI.ROLES_SCREEN.TIER_NAMES.EIGHT,
		UI.ROLES_SCREEN.TIER_NAMES.NINE
	};

	private int layoutRowHeight = 96;

	private Coroutine expandRoutine;

	public GameObject GetSlotWidget(GameObject parent)
	{
		GameObject gameObject;
		if (freeWidgetSlots.Count > 0)
		{
			gameObject = freeWidgetSlots[0];
			freeWidgetSlots.Remove(gameObject);
			gameObject.transform.SetParent(parent.transform);
		}
		else
		{
			gameObject = Util.KInstantiateUI(Prefab_Slot, parent, true);
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	public void RecycleSlotWidget(GameObject slotWidget)
	{
		freeWidgetSlots.Add(slotWidget);
		slotWidget.SetActive(false);
		slotWidget.transform.SetParent(SlotWidgetPool.transform);
	}

	protected override void OnActivate()
	{
		ConsumeMouseScroll = true;
		base.OnActivate();
		RefreshAll(null);
		Components.LiveMinionIdentities.OnAdd += MarkDirty;
		Components.LiveMinionIdentities.OnRemove += MarkDirty;
		CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		MultiToggle multiToggle = toggleAutoPrioritize;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(OnToggleAutoPrioritize));
		toggleAutoPrioritize.GetComponent<ToolTip>().OnToolTip = OnHoverToggleAutoPrioritize;
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			RefreshAll(null);
		}
		base.OnShow(show);
	}

	private void RefreshAll(object eventData = null)
	{
		dirty = false;
		RefreshRoleWidgets();
		RefreshSideBar();
		linesPending = true;
		toggleAutoPrioritize.ChangeState(Game.Instance.autoPrioritizeRoles ? 1 : 0);
	}

	public void MarkDirty(object eventData = null)
	{
		dirty = true;
	}

	private void Update()
	{
		if (dirty)
		{
			RefreshAll(null);
		}
		if (linesPending)
		{
			foreach (GameObject value in roleWidgets.Values)
			{
				value.GetComponent<RoleWidget>().RefreshLines();
			}
			linesPending = false;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && (e.TryConsume(Action.MouseRight) || e.TryConsume(Action.Escape)))
		{
			ManagementMenu.Instance.CloseAll();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public void RefreshRoleWidgets()
	{
		foreach (RoleConfig rolesConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (!roleWidgets.ContainsKey(rolesConfig.id))
			{
				while (Mathf.Max(rolesConfig.tier, 6) >= tierColumns.Count)
				{
					GameObject gameObject = Util.KInstantiateUI(Prefab_tierColumn, tierTableHorizontalLayout, true);
					gameObject.GetComponent<VerticalLayoutGroup>().enabled = !layoutRoles;
					tierColumns.Add(gameObject);
					HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
					if (tierColumns.Count % 2 == 0)
					{
						component.GetReference("BG").gameObject.SetActive(false);
					}
					component.GetReference<LocText>("Label").text = tierNames[tierColumns.Count - 1];
					component.GetReference<Image>("TierIcon").sprite = TierIcons[tierColumns.Count - 1];
					string text = "<b>" + tierNames[tierColumns.Count] + "</b> \n\n";
					if (Expectations.ExpectationsByTier[tierColumns.Count - 1].Length == 0)
					{
						text = UI.ROLES_SCREEN.EXPECTATIONS.NO_EXPECTATIONS;
					}
					Expectation[] array = Expectations.ExpectationsByTier[tierColumns.Count - 1];
					foreach (Expectation expectation in array)
					{
						text = "<b>" + expectation.name + "</b>: " + expectation.description + "\n\n";
						HierarchyReferences component2 = gameObject.GetComponent<HierarchyReferences>();
						GameObject gameObject2 = Util.KInstantiate(component2.GetReference("ExpectationPrefab").gameObject, component2.GetReference("ExpectationContainer").gameObject, expectation.name);
						gameObject2.SetActive(true);
						if (expectation is AttributeModifierExpectation)
						{
							gameObject2.GetComponentsInChildren<Image>()[1].sprite = (expectation as AttributeModifierExpectation).icon;
							gameObject2.GetComponentInChildren<LocText>().text = "+ " + (expectation as AttributeModifierExpectation).modifier.Value.ToString();
						}
						gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(text);
					}
				}
				GameObject value = Util.KInstantiateUI(Prefab_RoleWidget, tierColumns[rolesConfig.tier], true);
				roleWidgets.Add(rolesConfig.id, value);
			}
			roleWidgets[rolesConfig.id].GetComponent<RoleWidget>().Refresh(rolesConfig.id);
		}
		if (layoutRoles)
		{
			RefreshWidgetPositions();
		}
	}

	public void RefreshWidgetPositions()
	{
		float num = 0f;
		foreach (KeyValuePair<string, GameObject> roleWidget in roleWidgets)
		{
			float rowPosition = GetRowPosition(Game.Instance.roleManager.GetRowIndex(roleWidget.Key));
			num = Mathf.Max(rowPosition, num);
			roleWidget.Value.rectTransform().anchoredPosition = Vector2.down * rowPosition;
		}
		num = Mathf.Max(num, GetRowHeight(0) + (float)HEADER_HEIGHT);
		float rowHeight = GetRowHeight(Game.Instance.roleManager.NumberOfRows);
		foreach (GameObject tierColumn in tierColumns)
		{
			tierColumn.GetComponent<LayoutElement>().minHeight = num + rowHeight;
		}
		linesPending = true;
	}

	public float GetRowPosition(int rowIndex)
	{
		float num = 0f;
		for (int i = 1; i < rowIndex; i++)
		{
			num += GetRowHeight(i);
		}
		return num + (float)HEADER_HEIGHT;
	}

	public float GetRowHeight(int rowIndex)
	{
		float num = 32f;
		int num2 = 0;
		foreach (RoleConfig rolesConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (Game.Instance.roleManager.GetRowIndex(rolesConfig.id) == rowIndex)
			{
				num2 = Math.Max(num2, Game.Instance.roleManager.GetRoleAssignees(rolesConfig.id).Count);
			}
		}
		return Math.Max((float)layoutRowHeight, 72f + (float)num2 * num);
	}

	public RoleWidget GetRoleWidget(string roleID)
	{
		return roleWidgets[roleID].GetComponent<RoleWidget>();
	}

	public Vector2 GetRoleWidgetLineTargetPosition(string roleID)
	{
		return roleWidgets[roleID].GetComponent<RoleWidget>().lines_right.GetPosition();
	}

	public void RefreshSideBar()
	{
		roleWidgets["NoRole"].GetComponent<RoleWidget>().Refresh("NoRole");
	}

	private void OnToggleAutoPrioritize()
	{
		Game.Instance.autoPrioritizeRoles = !Game.Instance.autoPrioritizeRoles;
		toggleAutoPrioritize.ChangeState(Game.Instance.autoPrioritizeRoles ? 1 : 0);
		toggleAutoPrioritize.GetComponent<ToolTip>().forceRefresh = true;
	}

	private string OnHoverToggleAutoPrioritize()
	{
		return (!Game.Instance.autoPrioritizeRoles) ? UI.ROLES_SCREEN.AUTO_PRIORITIZE_DISABLED : UI.ROLES_SCREEN.AUTO_PRIORITIZE_ENABLED;
	}
}
