using System;
using UnityEngine;
using UnityEngine.UI;

public class PrioritizationGroupTableColumn : TableColumn
{
	public object userData;

	private Action<object, int> onChangePriority;

	private Func<object, string> onHoverWidget;

	private Func<object, string> onHoverHeaderOptionSelector;

	private Action<object> onSortClicked;

	private Func<object, string> onSortHovered;

	public PrioritizationGroupTableColumn(object user_data, Action<MinionIdentity, GameObject> on_load_action, Action<object, int> on_change_priority, Func<object, string> on_hover_widget, Action<object, int> on_change_header_priority, Func<object, string> on_hover_header_option_selector, Action<object> on_sort_clicked, Func<object, string> on_sort_hovered)
		: base(on_load_action, null, null, null, null, false, "")
	{
		userData = user_data;
		onChangePriority = on_change_priority;
		onHoverWidget = on_hover_widget;
		onHoverHeaderOptionSelector = on_hover_header_option_selector;
		onSortClicked = on_sort_clicked;
		onSortHovered = on_sort_hovered;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		return GetWidget(parent);
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		return GetWidget(parent);
	}

	private GameObject GetWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PriorityGroupSelector, parent, true);
		OptionSelector component = widget_go.GetComponent<OptionSelector>();
		component.Initialize(widget_go);
		component.OnChangePriority = delegate(object widget, int delta)
		{
			onChangePriority(widget, delta);
		};
		ToolTip[] componentsInChildren = widget_go.transform.GetComponentsInChildren<ToolTip>();
		if (componentsInChildren != null)
		{
			ToolTip[] array = componentsInChildren;
			foreach (ToolTip toolTip in array)
			{
				toolTip.OnToolTip = (() => onHoverWidget(widget_go));
			}
		}
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PriorityGroupSelectorHeader, parent, true);
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		LayoutElement component2 = widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		float num3 = component2.preferredWidth = (component2.minWidth = 63f);
		Component reference = component.GetReference("Label");
		LocText component3 = reference.GetComponent<LocText>();
		component3.raycastTarget = true;
		ToolTip component4 = reference.GetComponent<ToolTip>();
		if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			component4.OnToolTip = (() => onHoverWidget(widget_go));
		}
		MultiToggle multiToggle = column_sort_toggle = widget_go.GetComponentInChildren<MultiToggle>(true);
		MultiToggle multiToggle2 = multiToggle;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			onSortClicked(widget_go);
		});
		ToolTip component5 = multiToggle.GetComponent<ToolTip>();
		if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
		{
			component5.OnToolTip = (() => onSortHovered(widget_go));
		}
		KButton kButton = component.GetReference("PrioritizeButton") as KButton;
		ToolTip component6 = kButton.GetComponent<ToolTip>();
		if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
		{
			component6.OnToolTip = (() => onHoverHeaderOptionSelector(widget_go));
		}
		return widget_go;
	}
}
