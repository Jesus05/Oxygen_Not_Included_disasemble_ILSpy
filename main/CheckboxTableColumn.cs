using System;
using UnityEngine;

public class CheckboxTableColumn : TableColumn
{
	public GameObject prefab_header_portrait_checkbox = Assets.UIPrefabs.TableScreenWidgets.TogglePortrait;

	public GameObject prefab_checkbox = Assets.UIPrefabs.TableScreenWidgets.Checkbox;

	public Action<GameObject> on_press_action;

	public Action<GameObject, TableScreen.ResultValues> on_set_action;

	public Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action;

	public CheckboxTableColumn(Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, TableScreen.ResultValues> set_value_action, Comparison<IAssignableIdentity> sort_comparer, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, Func<bool> revealed = null)
		: base(on_load_action, sort_comparer, on_tooltip, on_sort_tooltip, revealed, false, string.Empty)
	{
		this.get_value_action = get_value_action;
		this.on_press_action = on_press_action;
		on_set_action = set_value_action;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(prefab_checkbox, parent, true);
		if ((UnityEngine.Object)widget_go.GetComponent<ToolTip>() != (UnityEngine.Object)null)
		{
			widget_go.GetComponent<ToolTip>().OnToolTip = (() => GetTooltip(widget_go.GetComponent<ToolTip>()));
		}
		MultiToggle component = widget_go.GetComponent<MultiToggle>();
		component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
		{
			on_press_action(widget_go);
		});
		return widget_go;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(prefab_checkbox, parent, true);
		if ((UnityEngine.Object)widget_go.GetComponent<ToolTip>() != (UnityEngine.Object)null)
		{
			widget_go.GetComponent<ToolTip>().OnToolTip = (() => GetTooltip(widget_go.GetComponent<ToolTip>()));
		}
		MultiToggle component = widget_go.GetComponent<MultiToggle>();
		component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
		{
			on_press_action(widget_go);
		});
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		ToolTip tooltip = null;
		GameObject widget_go = Util.KInstantiateUI(prefab_header_portrait_checkbox, parent, true);
		tooltip = widget_go.GetComponent<ToolTip>();
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		if ((UnityEngine.Object)tooltip == (UnityEngine.Object)null && (UnityEngine.Object)component != (UnityEngine.Object)null && component.HasReference("ToolTip"))
		{
			tooltip = (component.GetReference("ToolTip") as ToolTip);
		}
		tooltip.OnToolTip = (() => GetTooltip(tooltip));
		MultiToggle obj = component.GetReference("Toggle") as MultiToggle;
		obj.onClick = (System.Action)Delegate.Combine(obj.onClick, (System.Action)delegate
		{
			on_press_action(widget_go);
		});
		MultiToggle componentInChildren = widget_go.GetComponentInChildren<MultiToggle>();
		MultiToggle multiToggle = componentInChildren;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			screen.SetSortComparison(sort_comparer, this);
			screen.SortRows();
		});
		ToolTip sort_tooltip = componentInChildren.GetComponent<ToolTip>();
		sort_tooltip.OnToolTip = (() => GetSortTooltip(sort_tooltip));
		column_sort_toggle = widget_go.GetComponentInChildren<MultiToggle>();
		return widget_go;
	}
}
