using System;
using UnityEngine;

public class SuperCheckboxTableColumn : CheckboxTableColumn
{
	public GameObject prefab_super_checkbox = Assets.UIPrefabs.TableScreenWidgets.SuperCheckbox_Horizontal;

	public CheckboxTableColumn[] columns_affected;

	public SuperCheckboxTableColumn(CheckboxTableColumn[] columns_affected, Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, TableScreen.ResultValues> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip)
		: base(on_load_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, null, null)
	{
		this.columns_affected = columns_affected;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(prefab_super_checkbox, parent, true);
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
		GameObject widget_go = Util.KInstantiateUI(prefab_super_checkbox, parent, true);
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

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(prefab_super_checkbox, parent, true);
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
}
