using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableScreen : KScreen
{
	public enum ResultValues
	{
		False,
		Partial,
		True,
		ConditionalGroup
	}

	protected string title;

	protected bool has_default_duplicant_row = true;

	private bool rows_dirty;

	protected Comparison<IAssignableIdentity> active_sort_method;

	protected TableColumn active_sort_column;

	protected bool sort_is_reversed;

	private int active_cascade_coroutine_count;

	private HandleVector<int>.Handle current_looping_sound = HandleVector<int>.InvalidHandle;

	private bool incubating;

	protected Dictionary<string, TableColumn> columns = new Dictionary<string, TableColumn>();

	public List<TableRow> rows = new List<TableRow>();

	public List<TableRow> sortable_rows = new List<TableRow>();

	public List<string> column_scrollers = new List<string>();

	private Dictionary<GameObject, TableRow> known_widget_rows = new Dictionary<GameObject, TableRow>();

	private Dictionary<GameObject, TableColumn> known_widget_columns = new Dictionary<GameObject, TableColumn>();

	public GameObject prefab_row_empty;

	public GameObject prefab_row_header;

	public GameObject prefab_scroller_border;

	private string cascade_sound_path = GlobalAssets.GetSound("Placers_Unfurl_LP", false);

	public KButton CloseButton;

	[MyCmpGet]
	private VerticalLayoutGroup VLG;

	protected GameObject header_row;

	protected GameObject default_row;

	public LocText title_bar;

	public Transform header_content_transform;

	public Transform scroll_content_transform;

	public Transform scroller_borders_transform;

	protected override void OnActivate()
	{
		base.OnActivate();
		title_bar.text = title;
		ConsumeMouseScroll = true;
		CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		incubating = true;
		base.transform.rectTransform().localScale = Vector3.zero;
		Components.LiveMinionIdentities.OnAdd += delegate
		{
			MarkRowsDirty();
		};
		Components.LiveMinionIdentities.OnRemove += delegate
		{
			MarkRowsDirty();
		};
	}

	protected override void OnShow(bool show)
	{
		if (!show)
		{
			active_cascade_coroutine_count = 0;
			StopAllCoroutines();
			StopLoopingCascadeSound();
		}
		ZeroScrollers();
		base.OnShow(show);
	}

	private void ZeroScrollers()
	{
		if (rows.Count > 0)
		{
			foreach (string column_scroller in column_scrollers)
			{
				foreach (TableRow row in rows)
				{
					row.GetScroller(column_scroller).transform.parent.GetComponent<KScrollRect>().horizontalNormalizedPosition = 0f;
				}
			}
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (incubating)
		{
			ZeroScrollers();
			base.transform.rectTransform().localScale = Vector3.one;
			incubating = false;
		}
		if (rows_dirty)
		{
			RefreshRows();
		}
		foreach (TableRow row in rows)
		{
			row.RefreshScrollers();
		}
		foreach (TableColumn value in columns.Values)
		{
			if (value.isDirty)
			{
				foreach (KeyValuePair<TableRow, GameObject> item in value.widgets_by_row)
				{
					value.on_load_action(item.Key.GetIdentity(), item.Value);
					value.MarkClean();
				}
			}
		}
	}

	protected void MarkRowsDirty()
	{
		rows_dirty = true;
	}

	protected virtual void RefreshRows()
	{
		ClearRows();
		AddRow(null);
		if (has_default_duplicant_row)
		{
			AddDefaultRow();
		}
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			AddRow(Components.LiveMinionIdentities[i]);
		}
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info item2 in item.GetStoredMinionInfo())
			{
				MinionStorage.Info current2 = item2;
				if (current2.serializedMinion != null)
				{
					StoredMinionIdentity minion = current2.serializedMinion.Get<StoredMinionIdentity>();
					AddRow(minion);
				}
			}
		}
		SortRows();
		rows_dirty = false;
	}

	public virtual void SetSortComparison(Comparison<IAssignableIdentity> comparison, TableColumn sort_column)
	{
		if (comparison != null)
		{
			if (active_sort_column == sort_column)
			{
				if (sort_is_reversed)
				{
					sort_is_reversed = false;
					active_sort_method = null;
					active_sort_column = null;
				}
				else
				{
					sort_is_reversed = true;
				}
			}
			else
			{
				active_sort_column = sort_column;
				active_sort_method = comparison;
				sort_is_reversed = false;
			}
		}
	}

	public void SortRows()
	{
		foreach (TableColumn value in columns.Values)
		{
			if (!((UnityEngine.Object)value.column_sort_toggle == (UnityEngine.Object)null))
			{
				if (value == active_sort_column)
				{
					if (sort_is_reversed)
					{
						value.column_sort_toggle.ChangeState(2);
					}
					else
					{
						value.column_sort_toggle.ChangeState(1);
					}
				}
				else
				{
					value.column_sort_toggle.ChangeState(0);
				}
			}
		}
		if (active_sort_method != null)
		{
			Dictionary<IAssignableIdentity, TableRow> dictionary = new Dictionary<IAssignableIdentity, TableRow>();
			foreach (TableRow sortable_row in sortable_rows)
			{
				dictionary.Add(sortable_row.GetIdentity(), sortable_row);
			}
			List<IAssignableIdentity> list = new List<IAssignableIdentity>();
			foreach (KeyValuePair<IAssignableIdentity, TableRow> item in dictionary)
			{
				list.Add(item.Key);
			}
			list.Sort(active_sort_method);
			if (sort_is_reversed)
			{
				list.Reverse();
			}
			sortable_rows.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				sortable_rows.Add(dictionary[list[i]]);
			}
			for (int j = 0; j < sortable_rows.Count; j++)
			{
				sortable_rows[j].gameObject.transform.SetSiblingIndex(j);
			}
			if (has_default_duplicant_row)
			{
				default_row.transform.SetAsFirstSibling();
			}
		}
	}

	protected int compare_rows_alphabetical(IAssignableIdentity a, IAssignableIdentity b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		return a.GetProperName().CompareTo(b.GetProperName());
	}

	protected int default_sort(TableRow a, TableRow b)
	{
		return 0;
	}

	protected void ClearRows()
	{
		for (int num = rows.Count - 1; num >= 0; num--)
		{
			rows[num].Clear();
		}
		rows.Clear();
		sortable_rows.Clear();
	}

	protected void AddRow(IAssignableIdentity minion)
	{
		bool flag = minion == null;
		GameObject gameObject = Util.KInstantiateUI((!flag) ? prefab_row_empty : prefab_row_header, (minion != null) ? scroll_content_transform.gameObject : header_content_transform.gameObject, true);
		TableRow component = gameObject.GetComponent<TableRow>();
		component.rowType = ((!flag) ? ((!((UnityEngine.Object)(minion as MinionIdentity) != (UnityEngine.Object)null)) ? TableRow.RowType.StoredMinon : TableRow.RowType.Minion) : TableRow.RowType.Header);
		rows.Add(component);
		component.ConfigureContent(minion, columns);
		if (!flag)
		{
			sortable_rows.Add(component);
		}
		else
		{
			header_row = gameObject;
		}
	}

	protected void AddDefaultRow()
	{
		TableRow component = (default_row = Util.KInstantiateUI(prefab_row_empty, scroll_content_transform.gameObject, true)).GetComponent<TableRow>();
		component.rowType = TableRow.RowType.Default;
		component.isDefault = true;
		rows.Add(component);
		component.ConfigureContent(null, columns);
	}

	protected TableRow GetWidgetRow(GameObject widget_go)
	{
		if ((UnityEngine.Object)widget_go == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Widget is null");
			return null;
		}
		if (known_widget_rows.ContainsKey(widget_go))
		{
			return known_widget_rows[widget_go];
		}
		foreach (TableRow row in rows)
		{
			if (row.ContainsWidget(widget_go))
			{
				known_widget_rows.Add(widget_go, row);
				return row;
			}
		}
		Debug.LogWarning("Row is null for widget: " + widget_go.name + " parent is " + widget_go.transform.parent.name);
		return null;
	}

	protected void StartScrollableContent(string scrollablePanelID)
	{
		if (!column_scrollers.Contains(scrollablePanelID))
		{
			DividerColumn new_column = new DividerColumn(() => true, string.Empty);
			RegisterColumn("scroller_spacer_" + scrollablePanelID, new_column);
			column_scrollers.Add(scrollablePanelID);
		}
	}

	protected PortraitTableColumn AddPortraitColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Comparison<IAssignableIdentity> sort_comparison, bool double_click_to_target = true)
	{
		PortraitTableColumn portraitTableColumn = new PortraitTableColumn(on_load_action, sort_comparison, double_click_to_target);
		if (RegisterColumn(id, portraitTableColumn))
		{
			return portraitTableColumn;
		}
		return null;
	}

	protected ButtonLabelColumn AddButtonLabelColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, string> get_value_action, Action<GameObject> on_click_action, Action<GameObject> on_double_click_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, bool whiteText = false)
	{
		ButtonLabelColumn buttonLabelColumn = new ButtonLabelColumn(on_load_action, get_value_action, on_click_action, on_double_click_action, sort_comparison, on_tooltip, on_sort_tooltip, whiteText);
		if (RegisterColumn(id, buttonLabelColumn))
		{
			return buttonLabelColumn;
		}
		return null;
	}

	protected LabelTableColumn AddLabelColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, string> get_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, int widget_width = 128, bool should_refresh_columns = false)
	{
		LabelTableColumn labelTableColumn = new LabelTableColumn(on_load_action, get_value_action, sort_comparison, on_tooltip, on_sort_tooltip, widget_width, should_refresh_columns);
		if (RegisterColumn(id, labelTableColumn))
		{
			return labelTableColumn;
		}
		return null;
	}

	protected CheckboxTableColumn AddCheckboxColumn(string id, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, ResultValues> set_value_function, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		CheckboxTableColumn checkboxTableColumn = new CheckboxTableColumn(on_load_action, get_value_action, on_press_action, set_value_function, sort_comparison, on_tooltip, on_sort_tooltip, null);
		if (RegisterColumn(id, checkboxTableColumn))
		{
			return checkboxTableColumn;
		}
		return null;
	}

	protected SuperCheckboxTableColumn AddSuperCheckboxColumn(string id, CheckboxTableColumn[] columns_affected, Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, ResultValues> set_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = new SuperCheckboxTableColumn(columns_affected, on_load_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip);
		if (RegisterColumn(id, superCheckboxTableColumn))
		{
			foreach (CheckboxTableColumn checkboxTableColumn in columns_affected)
			{
				CheckboxTableColumn checkboxTableColumn2 = checkboxTableColumn;
				checkboxTableColumn2.on_set_action = (Action<GameObject, ResultValues>)Delegate.Combine(checkboxTableColumn2.on_set_action, new Action<GameObject, ResultValues>(superCheckboxTableColumn.MarkDirty));
			}
			superCheckboxTableColumn.MarkDirty(null, ResultValues.False);
			return superCheckboxTableColumn;
		}
		Debug.LogWarning("SuperCheckbox column registration failed");
		return null;
	}

	protected NumericDropDownTableColumn AddNumericDropDownColumn(string id, object user_data, List<TMP_Dropdown.OptionData> options, Action<IAssignableIdentity, GameObject> on_load_action, Action<GameObject, int> set_value_action, Comparison<IAssignableIdentity> sort_comparison, NumericDropDownTableColumn.ToolTipCallbacks tooltip_callbacks)
	{
		NumericDropDownTableColumn numericDropDownTableColumn = new NumericDropDownTableColumn(user_data, options, on_load_action, set_value_action, sort_comparison, tooltip_callbacks, null);
		if (RegisterColumn(id, numericDropDownTableColumn))
		{
			return numericDropDownTableColumn;
		}
		return null;
	}

	protected bool RegisterColumn(string id, TableColumn new_column)
	{
		if (columns.ContainsKey(id))
		{
			Debug.LogWarning($"Column with id {id} already in dictionary");
			return false;
		}
		new_column.screen = this;
		columns.Add(id, new_column);
		MarkRowsDirty();
		return true;
	}

	protected TableColumn GetWidgetColumn(GameObject widget_go)
	{
		if (known_widget_columns.ContainsKey(widget_go))
		{
			return known_widget_columns[widget_go];
		}
		foreach (KeyValuePair<string, TableColumn> column in columns)
		{
			if (column.Value.ContainsWidget(widget_go))
			{
				known_widget_columns.Add(widget_go, column.Value);
				return column.Value;
			}
		}
		Debug.LogWarning("No column found for widget gameobject " + widget_go.name);
		return null;
	}

	protected void on_load_portrait(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		CrewPortrait component = widget_go.GetComponent<CrewPortrait>();
		if (minion != null)
		{
			component.SetIdentityObject(minion, false);
		}
		else
		{
			component.targetImage.enabled = (widgetRow.rowType == TableRow.RowType.Default);
		}
	}

	protected void on_load_name_label(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText locText = null;
		LocText locText2 = null;
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		locText = (component.GetReference("Label") as LocText);
		if (component.HasReference("SubLabel"))
		{
			locText2 = (component.GetReference("SubLabel") as LocText);
		}
		if (minion != null)
		{
			locText.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			if ((UnityEngine.Object)locText2 != (UnityEngine.Object)null)
			{
				MinionIdentity minionIdentity = minion as MinionIdentity;
				if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
				{
					locText2.text = minionIdentity.gameObject.GetComponent<MinionResume>().GetSkillsSubtitle();
				}
				else
				{
					locText2.text = string.Empty;
				}
				locText2.enableWordWrapping = false;
			}
		}
		else
		{
			if (widgetRow.isDefault)
			{
				locText.text = UI.JOBSCREEN_DEFAULT;
				if ((UnityEngine.Object)locText2 != (UnityEngine.Object)null)
				{
					locText2.gameObject.SetActive(false);
				}
			}
			else
			{
				locText.text = UI.JOBSCREEN_EVERYONE;
			}
			if ((UnityEngine.Object)locText2 != (UnityEngine.Object)null)
			{
				locText2.text = string.Empty;
			}
		}
	}

	protected string get_value_name_label(IAssignableIdentity minion, GameObject widget_go)
	{
		return minion.GetProperName();
	}

	protected void on_load_value_checkbox_column_super(IAssignableIdentity minion, GameObject widget_go)
	{
		MultiToggle multiToggle = null;
		multiToggle = widget_go.GetComponent<MultiToggle>();
		TableRow widgetRow = GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType == TableRow.RowType.Header || rowType == TableRow.RowType.Default || rowType == TableRow.RowType.Minion)
		{
			multiToggle.ChangeState((int)get_value_checkbox_column_super(minion, widget_go));
		}
	}

	public virtual ResultValues get_value_checkbox_column_super(IAssignableIdentity minion, GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		CheckboxTableColumn[] columns_affected = superCheckboxTableColumn.columns_affected;
		foreach (CheckboxTableColumn checkboxTableColumn in columns_affected)
		{
			if (checkboxTableColumn.isRevealed)
			{
				switch (checkboxTableColumn.get_value_action(widgetRow.GetIdentity(), widgetRow.GetWidget(checkboxTableColumn)))
				{
				case ResultValues.False:
					flag2 = false;
					if (!flag)
					{
						flag5 = true;
					}
					break;
				case ResultValues.Partial:
					flag4 = true;
					flag5 = true;
					break;
				case ResultValues.True:
					flag4 = true;
					flag = false;
					if (!flag2)
					{
						flag5 = true;
					}
					break;
				case ResultValues.ConditionalGroup:
					flag3 = true;
					flag2 = false;
					flag = false;
					break;
				}
				if (!flag5)
				{
					continue;
				}
			}
		}
		ResultValues result = ResultValues.Partial;
		if (flag3 && !flag4 && !flag2 && !flag)
		{
			result = ResultValues.ConditionalGroup;
		}
		else if (flag2)
		{
			result = ResultValues.True;
		}
		else if (flag)
		{
			result = ResultValues.False;
		}
		else if (flag4)
		{
			result = ResultValues.Partial;
		}
		return result;
	}

	protected void set_value_checkbox_column_super(GameObject widget_go, ResultValues new_value)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			StartCoroutine(CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, default_row.GetComponent<TableRow>(), new_value, widget_go));
			StartCoroutine(CascadeSetColumnCheckBoxes(sortable_rows, superCheckboxTableColumn, new_value, widget_go));
			break;
		case TableRow.RowType.Default:
			StartCoroutine(CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, widgetRow, new_value, widget_go));
			break;
		case TableRow.RowType.Minion:
			StartCoroutine(CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, widgetRow, new_value, widget_go));
			break;
		}
	}

	protected IEnumerator CascadeSetRowCheckBoxes(CheckboxTableColumn[] checkBoxToggleColumns, TableRow row, ResultValues state, GameObject ignore_widget = null)
	{
		if (active_cascade_coroutine_count == 0)
		{
			current_looping_sound = LoopingSoundManager.StartSound(cascade_sound_path, Vector3.zero, false, false);
		}
		active_cascade_coroutine_count++;
		for (int i = 0; i < checkBoxToggleColumns.Length; i++)
		{
			if (checkBoxToggleColumns[i].widgets_by_row.ContainsKey(row))
			{
				GameObject widget = checkBoxToggleColumns[i].widgets_by_row[row];
				if (!((UnityEngine.Object)widget == (UnityEngine.Object)ignore_widget) && checkBoxToggleColumns[i].isRevealed)
				{
					bool needsSetting = false;
					switch ((GetWidgetColumn(widget) as CheckboxTableColumn).get_value_action(row.GetIdentity(), widget))
					{
					case ResultValues.False:
						needsSetting = ((state != 0) ? true : false);
						break;
					case ResultValues.Partial:
					case ResultValues.ConditionalGroup:
						needsSetting = true;
						break;
					case ResultValues.True:
						needsSetting = ((state != ResultValues.True) ? true : false);
						break;
					}
					if (needsSetting)
					{
						(GetWidgetColumn(widget) as CheckboxTableColumn).on_set_action(widget, state);
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
		}
		active_cascade_coroutine_count--;
		if (active_cascade_coroutine_count <= 0)
		{
			StopLoopingCascadeSound();
		}
	}

	protected IEnumerator CascadeSetColumnCheckBoxes(List<TableRow> rows, CheckboxTableColumn checkBoxToggleColumn, ResultValues state, GameObject header_widget_go = null)
	{
		if (active_cascade_coroutine_count == 0)
		{
			current_looping_sound = LoopingSoundManager.StartSound(cascade_sound_path, Vector3.zero, false, true);
		}
		active_cascade_coroutine_count++;
		for (int i = 0; i < rows.Count; i++)
		{
			GameObject widget = rows[i].GetWidget(checkBoxToggleColumn);
			if (!((UnityEngine.Object)widget == (UnityEngine.Object)header_widget_go))
			{
				bool needsSetting = false;
				switch ((GetWidgetColumn(widget) as CheckboxTableColumn).get_value_action(rows[i].GetIdentity(), widget))
				{
				case ResultValues.False:
					needsSetting = ((state != 0) ? true : false);
					break;
				case ResultValues.Partial:
				case ResultValues.ConditionalGroup:
					needsSetting = true;
					break;
				case ResultValues.True:
					needsSetting = ((state != ResultValues.True) ? true : false);
					break;
				}
				if (needsSetting)
				{
					(GetWidgetColumn(widget) as CheckboxTableColumn).on_set_action(widget, state);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}
		if ((UnityEngine.Object)header_widget_go != (UnityEngine.Object)null)
		{
			(GetWidgetColumn(header_widget_go) as CheckboxTableColumn).on_load_action(null, header_widget_go);
		}
		active_cascade_coroutine_count--;
		if (active_cascade_coroutine_count <= 0)
		{
			StopLoopingCascadeSound();
		}
	}

	private void StopLoopingCascadeSound()
	{
		if (current_looping_sound.IsValid())
		{
			LoopingSoundManager.StopSound(current_looping_sound);
			current_looping_sound.Clear();
		}
	}

	protected void on_press_checkbox_column_super(GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (get_value_checkbox_column_super(widgetRow.GetIdentity(), widget_go))
		{
		case ResultValues.True:
			superCheckboxTableColumn.on_set_action(widget_go, ResultValues.False);
			break;
		case ResultValues.Partial:
		case ResultValues.ConditionalGroup:
			superCheckboxTableColumn.on_set_action(widget_go, ResultValues.True);
			break;
		case ResultValues.False:
			superCheckboxTableColumn.on_set_action(widget_go, ResultValues.True);
			break;
		}
		superCheckboxTableColumn.on_load_action(widgetRow.GetIdentity(), widget_go);
	}

	protected void on_tooltip_sort_alphabetically(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_NAME, null);
			break;
		}
	}
}
