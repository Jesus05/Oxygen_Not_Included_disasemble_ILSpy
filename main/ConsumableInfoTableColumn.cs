using System;
using UnityEngine;

public class ConsumableInfoTableColumn : CheckboxTableColumn
{
	public IConsumableUIItem consumable_info;

	public Func<GameObject, string> get_header_label;

	public ConsumableInfoTableColumn(IConsumableUIItem consumable_info, Action<IAssignableIdentity, GameObject> load_value_action, Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, TableScreen.ResultValues> set_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, Func<GameObject, string> get_header_label)
		: base(load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, () => DebugHandler.InstantBuildMode || ConsumerManager.instance.isDiscovered(consumable_info.ConsumableId.ToTag()))
	{
		this.consumable_info = consumable_info;
		this.get_header_label = get_header_label;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		GameObject headerWidget = base.GetHeaderWidget(parent);
		LocText componentInChildren = headerWidget.GetComponentInChildren<LocText>();
		if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
		{
			headerWidget.GetComponentInChildren<LocText>().text = get_header_label(headerWidget);
		}
		headerWidget.GetComponentInChildren<MultiToggle>().gameObject.SetActive(false);
		return headerWidget;
	}
}
