using Klei.AI;
using STRINGS;
using UnityEngine;

public class VitalsTableScreen : TableScreen
{
	protected override void OnActivate()
	{
		has_default_duplicant_row = false;
		title = UI.VITALS;
		base.OnActivate();
		AddPortraitColumn("Portrait", base.on_load_portrait, null, true);
		AddButtonLabelColumn("Names", base.on_load_name_label, base.get_value_name_label, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, base.compare_rows_alphabetical, on_tooltip_name, base.on_tooltip_sort_alphabetically, false);
		AddLabelColumn("Stress", on_load_stress, get_value_stress_label, compare_rows_stress, on_tooltip_stress, on_tooltip_sort_stress, 64, true);
		AddLabelColumn("QOLExpectations", on_load_qualityoflife_expectations, get_value_qualityoflife_expectations_label, compare_rows_qualityoflife_expectations, on_tooltip_qualityoflife_expectations, on_tooltip_sort_qualityoflife_expectations, 128, true);
		AddLabelColumn("Fullness", on_load_fullness, get_value_fullness_label, compare_rows_fullness, on_tooltip_fullness, on_tooltip_sort_fullness, 96, true);
		AddLabelColumn("EatenToday", on_load_eaten_today, get_value_eaten_today_label, compare_rows_eaten_today, on_tooltip_eaten_today, on_tooltip_sort_eaten_today, 96, true);
		AddLabelColumn("Health", on_load_health, get_value_health_label, compare_rows_health, on_tooltip_health, on_tooltip_sort_health, 64, true);
		AddLabelColumn("Immunity", on_load_immunity, get_value_immunity_label, compare_rows_immunity, on_tooltip_immunity, on_tooltip_sort_immunity, 192, true);
	}

	private void on_load_stress(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if ((Object)minion != (Object)null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = ((!widgetRow.isDefault) ? UI.VITALSSCREEN.STRESS.ToString() : "");
		}
	}

	private string get_value_stress_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Amounts.Stress.Lookup(minion).GetValueString();
	}

	private int compare_rows_stress(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.Stress.Lookup(a).value;
		float value2 = Db.Get().Amounts.Stress.Lookup(b).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_stress(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.Stress.Lookup(minion).GetTooltip(), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_stress(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_STRESS, null);
			break;
		}
	}

	private void on_load_qualityoflife_expectations(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if ((Object)minion != (Object)null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = ((!widgetRow.isDefault) ? UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS.ToString() : "");
		}
	}

	private string get_value_qualityoflife_expectations_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Attributes.QualityOfLife.Lookup(minion).GetFormattedValue();
	}

	private int compare_rows_qualityoflife_expectations(MinionIdentity a, MinionIdentity b)
	{
		float totalValue = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(a).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(b).GetTotalValue();
		return totalValue.CompareTo(totalValue2);
	}

	protected void on_tooltip_qualityoflife_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
				tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS_TOOLTIP, Db.Get().Attributes.QualityOfLifeExpectation.Lookup(minion).GetFormattedValue()), null);
				tooltip.AddMultiStringTooltip(UI.HORIZONTAL_RULE, null);
				tooltip.AddMultiStringTooltip(Db.Get().Attributes.QualityOfLife.Lookup(minion).GetAttributeValueTooltip(), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_qualityoflife_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, null);
			break;
		}
	}

	private void on_load_health(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if ((Object)minion != (Object)null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			LocText locText = componentInChildren;
			object text;
			if (widgetRow.isDefault)
			{
				text = "";
			}
			else
			{
				string text3 = componentInChildren.text = UI.VITALSSCREEN_HEALTH.ToString();
				text = text3;
			}
			locText.text = (string)text;
		}
	}

	private string get_value_health_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Amounts.HitPoints.Lookup(minion).GetValueString();
	}

	private int compare_rows_health(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.HitPoints.Lookup(a).value;
		float value2 = Db.Get().Amounts.HitPoints.Lookup(b).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_health(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.HitPoints.Lookup(minion).GetTooltip(), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_health(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_HITPOINTS, null);
			break;
		}
	}

	private void on_load_immunity(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if ((Object)minion != (Object)null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = ((!widgetRow.isDefault) ? UI.VITALSSCREEN_IMMUNITY.ToString() : "");
		}
	}

	private string get_value_immunity_label(MinionIdentity minion, GameObject widget_go)
	{
		Diseases diseases = minion.GetComponent<MinionModifiers>().diseases;
		if (!diseases.IsInfected())
		{
			return Db.Get().Amounts.ImmuneLevel.Lookup(minion).GetValueString();
		}
		string text = "";
		if (diseases.Count > 1)
		{
			float seconds = 0f;
			foreach (DiseaseInstance item in diseases)
			{
				seconds = Mathf.Min(item.GetInfectedTimeRemaining());
			}
			text += string.Format(UI.VITALSSCREEN.IMMUNITY_MULTIPLE_DISEASES, GameUtil.GetFormattedCycles(seconds, "F1"));
		}
		else
		{
			foreach (DiseaseInstance item2 in diseases)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "\n";
				}
				text += string.Format(UI.VITALSSCREEN.IMMUNITY_DISEASE, item2.modifier.Name, GameUtil.GetFormattedCycles(item2.GetInfectedTimeRemaining(), "F1"));
			}
		}
		return text;
	}

	private int compare_rows_immunity(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.ImmuneLevel.Lookup(a).value;
		float value2 = Db.Get().Amounts.ImmuneLevel.Lookup(b).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_immunity(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
				Diseases diseases = minion.GetComponent<MinionModifiers>().diseases;
				if (diseases.IsInfected())
				{
					foreach (DiseaseInstance item in diseases)
					{
						tooltip.AddMultiStringTooltip(UI.HORIZONTAL_RULE, null);
						tooltip.AddMultiStringTooltip(item.modifier.Name, null);
						StatusItem statusItem = item.GetStatusItem();
						tooltip.AddMultiStringTooltip(statusItem.GetTooltip(item.ExposureInfo), null);
					}
				}
				else
				{
					tooltip.AddMultiStringTooltip(Db.Get().Amounts.ImmuneLevel.Lookup(minion).GetTooltip(), null);
				}
			}
			break;
		}
	}

	protected void on_tooltip_sort_immunity(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_IMMUNEPOINTS, null);
			break;
		}
	}

	private void on_load_fullness(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if ((Object)minion != (Object)null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = ((!widgetRow.isDefault) ? UI.VITALSSCREEN_CALORIES.ToString() : "");
		}
	}

	private string get_value_fullness_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Amounts.Calories.Lookup(minion).GetValueString();
	}

	private int compare_rows_fullness(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.Calories.Lookup(a).value;
		float value2 = Db.Get().Amounts.Calories.Lookup(b).value;
		return value2.CompareTo(value);
	}

	protected void on_tooltip_fullness(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.Calories.Lookup(minion).GetTooltip(), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_fullness(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_FULLNESS, null);
			break;
		}
	}

	protected void on_tooltip_name(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, minion.GetProperName()), null);
			}
			break;
		}
	}

	private void on_load_eaten_today(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if ((Object)minion != (Object)null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = ((!widgetRow.isDefault) ? UI.VITALSSCREEN_EATENTODAY.ToString() : "");
		}
	}

	private static float RationsEatenToday(MinionIdentity minion)
	{
		float result = 0f;
		if ((Object)minion != (Object)null)
		{
			RationMonitor.Instance sMI = minion.GetSMI<RationMonitor.Instance>();
			if (sMI != null)
			{
				result = sMI.GetRationsAteToday();
			}
		}
		return result;
	}

	private string get_value_eaten_today_label(MinionIdentity minion, GameObject widget_go)
	{
		float calories = RationsEatenToday(minion);
		return GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true);
	}

	private int compare_rows_eaten_today(MinionIdentity a, MinionIdentity b)
	{
		float value = RationsEatenToday(a);
		return RationsEatenToday(b).CompareTo(value);
	}

	protected void on_tooltip_eaten_today(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if ((Object)minion != (Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
				float calories = RationsEatenToday(minion);
				tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.EATEN_TODAY_TOOLTIP, GameUtil.GetFormattedCalories(calories, GameUtil.TimeSlice.None, true)), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_eaten_today(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EATEN_TODAY, null);
			break;
		}
	}
}
