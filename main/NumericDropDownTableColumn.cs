using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericDropDownTableColumn : TableColumn
{
	public class ToolTipCallbacks
	{
		public Action<IAssignableIdentity, GameObject, ToolTip> headerTooltip;

		public Action<IAssignableIdentity, GameObject, ToolTip> headerSortTooltip;

		public Action<IAssignableIdentity, GameObject, ToolTip> headerDropdownTooltip;
	}

	public object userData;

	private ToolTipCallbacks callbacks;

	private Action<GameObject, int> set_value_action;

	private List<TMP_Dropdown.OptionData> options;

	public NumericDropDownTableColumn(object user_data, List<TMP_Dropdown.OptionData> options, Action<IAssignableIdentity, GameObject> on_load_action, Action<GameObject, int> set_value_action, Comparison<IAssignableIdentity> sort_comparer, ToolTipCallbacks callbacks, Func<bool> revealed = null)
		: base(on_load_action, sort_comparer, callbacks.headerTooltip, callbacks.headerSortTooltip, revealed, false, string.Empty)
	{
		userData = user_data;
		this.set_value_action = set_value_action;
		this.options = options;
		this.callbacks = callbacks;
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
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.NumericDropDown, parent, true);
		TMP_Dropdown componentInChildren = widget_go.transform.GetComponentInChildren<TMP_Dropdown>();
		componentInChildren.options = options;
		componentInChildren.onValueChanged.AddListener(delegate(int new_value)
		{
			set_value_action(widget_go, new_value);
		});
		ToolTip tt = widget_go.transform.GetComponentInChildren<ToolTip>();
		if ((UnityEngine.Object)tt != (UnityEngine.Object)null)
		{
			tt.OnToolTip = (() => GetTooltip(tt));
		}
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.DropDownHeader, parent, true);
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		Component reference = component.GetReference("Label");
		MultiToggle multiToggle = column_sort_toggle = reference.GetComponentInChildren<MultiToggle>(true);
		MultiToggle multiToggle2 = multiToggle;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			screen.SetSortComparison(sort_comparer, this);
			screen.SortRows();
		});
		ToolTip tt = reference.GetComponent<ToolTip>();
		tt.enabled = true;
		tt.OnToolTip = delegate
		{
			callbacks.headerTooltip(null, widget_go, tt);
			return string.Empty;
		};
		ToolTip tt2 = multiToggle.transform.GetComponent<ToolTip>();
		tt2.OnToolTip = delegate
		{
			callbacks.headerSortTooltip(null, widget_go, tt2);
			return string.Empty;
		};
		Component reference2 = component.GetReference("DropDown");
		TMP_Dropdown componentInChildren = reference2.GetComponentInChildren<TMP_Dropdown>();
		componentInChildren.options = options;
		componentInChildren.onValueChanged.AddListener(delegate(int new_value)
		{
			set_value_action(widget_go, new_value);
		});
		ToolTip tt3 = reference2.GetComponent<ToolTip>();
		tt3.OnToolTip = delegate
		{
			callbacks.headerDropdownTooltip(null, widget_go, tt3);
			return string.Empty;
		};
		LayoutElement component2 = widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		float num3 = component2.preferredWidth = (component2.minWidth = 83f);
		return widget_go;
	}
}
