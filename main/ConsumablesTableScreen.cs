using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class ConsumablesTableScreen : TableScreen
{
	private const int CONSUMABLE_COLUMNS_BEFORE_SCROLL = 12;

	protected override void OnActivate()
	{
		title = UI.CONSUMABLESSCREEN.TITLE;
		base.OnActivate();
		AddPortraitColumn("Portrait", base.on_load_portrait, null, true);
		AddButtonLabelColumn("Names", base.on_load_name_label, base.get_value_name_label, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, base.compare_rows_alphabetical, on_tooltip_name, base.on_tooltip_sort_alphabetically, false);
		AddLabelColumn("QOLExpectations", on_load_qualityoflife_expectations, get_value_qualityoflife_label, compare_rows_qualityoflife_expectations, on_tooltip_qualityoflife_expectations, on_tooltip_sort_qualityoflife_expectations, 96, true);
		List<IConsumableUIItem> list = new List<IConsumableUIItem>();
		for (int i = 0; i < FOOD.FOOD_TYPES_LIST.Count; i++)
		{
			list.Add(FOOD.FOOD_TYPES_LIST[i]);
		}
		List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(GameTags.Medicine);
		for (int j = 0; j < prefabsWithTag.Count; j++)
		{
			MedicinalPill component = prefabsWithTag[j].GetComponent<MedicinalPill>();
			if ((bool)component)
			{
				list.Add(component);
			}
			else
			{
				DebugUtil.DevLogErrorFormat("Prefab tagged Medicine does not have MedicinalPill component: {0}", prefabsWithTag[j]);
			}
		}
		list.Sort(delegate(IConsumableUIItem a, IConsumableUIItem b)
		{
			int num2 = a.MajorOrder.CompareTo(b.MajorOrder);
			if (num2 == 0)
			{
				num2 = a.MinorOrder.CompareTo(b.MinorOrder);
			}
			return num2;
		});
		ConsumerManager.instance.OnDiscover += OnConsumableDiscovered;
		List<ConsumableInfoTableColumn> list2 = new List<ConsumableInfoTableColumn>();
		List<DividerColumn> list3 = new List<DividerColumn>();
		List<ConsumableInfoTableColumn> list4 = new List<ConsumableInfoTableColumn>();
		StartScrollableContent("consumableScroller");
		int num = 0;
		for (int k = 0; k < list.Count; k++)
		{
			if (list[k].Display)
			{
				if (list[k].MajorOrder != num && k != 0)
				{
					string id = "QualityDivider_" + list[k].MajorOrder;
					ConsumableInfoTableColumn[] quality_group_columns = list4.ToArray();
					DividerColumn dividerColumn = new DividerColumn(delegate
					{
						if (quality_group_columns == null || quality_group_columns.Length == 0)
						{
							return true;
						}
						ConsumableInfoTableColumn[] array = quality_group_columns;
						foreach (ConsumableInfoTableColumn consumableInfoTableColumn in array)
						{
							if (consumableInfoTableColumn.isRevealed)
							{
								return true;
							}
						}
						return false;
					}, "consumableScroller");
					list3.Add(dividerColumn);
					RegisterColumn(id, dividerColumn);
					list4.Clear();
				}
				ConsumableInfoTableColumn item = AddConsumableInfoColumn(list[k].ConsumableId, list[k], on_load_consumable_info, get_value_consumable_info, on_click_consumable_info, set_value_consumable_info, compare_consumable_info, on_tooltip_consumable_info, on_tooltip_sort_consumable_info);
				list2.Add(item);
				num = list[k].MajorOrder;
				list4.Add(item);
			}
		}
		AddSuperCheckboxColumn("SuperCheckConsumable", list2.ToArray(), base.on_load_value_checkbox_column_super, get_value_checkbox_column_super, base.on_press_checkbox_column_super, base.set_value_checkbox_column_super, null, on_tooltip_consumable_info_super);
	}

	private void refresh_scrollers()
	{
		int num = 0;
		foreach (EdiblesManager.FoodInfo item in FOOD.FOOD_TYPES_LIST)
		{
			if (DebugHandler.InstantBuildMode || ConsumerManager.instance.isDiscovered(item.ConsumableId.ToTag()))
			{
				num++;
			}
		}
		foreach (TableRow row in rows)
		{
			GameObject scroller = row.GetScroller("consumableScroller");
			if ((UnityEngine.Object)scroller != (UnityEngine.Object)null)
			{
				KScrollRect component = scroller.transform.parent.GetComponent<KScrollRect>();
				if ((UnityEngine.Object)component.horizontalScrollbar != (UnityEngine.Object)null)
				{
					component.horizontalScrollbar.gameObject.SetActive(num >= 12);
					row.GetScrollerBorder("consumableScroller").gameObject.SetActive(num >= 12);
				}
				component.horizontal = (num >= 12);
				component.enabled = (num >= 12);
			}
		}
	}

	private void on_load_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else
		{
			componentInChildren.text = ((!widgetRow.isDefault) ? UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS.ToString() : string.Empty);
		}
	}

	private string get_value_qualityoflife_label(IAssignableIdentity minion, GameObject widget_go)
	{
		string result = string.Empty;
		TableRow widgetRow = GetWidgetRow(widget_go);
		if (widgetRow.rowType == TableRow.RowType.Minion)
		{
			result = Db.Get().Attributes.QualityOfLife.Lookup(minion as MinionIdentity).GetFormattedValue();
		}
		else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
		{
			result = UI.TABLESCREENS.NA;
		}
		return result;
	}

	private int compare_rows_qualityoflife_expectations(IAssignableIdentity a, IAssignableIdentity b)
	{
		MinionIdentity minionIdentity = a as MinionIdentity;
		MinionIdentity minionIdentity2 = b as MinionIdentity;
		if ((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null && (UnityEngine.Object)minionIdentity2 == (UnityEngine.Object)null)
		{
			return 0;
		}
		if ((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null)
		{
			return -1;
		}
		if ((UnityEngine.Object)minionIdentity2 == (UnityEngine.Object)null)
		{
			return 1;
		}
		float totalValue = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(minionIdentity).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(minionIdentity2).GetTotalValue();
		return totalValue.CompareTo(totalValue2);
	}

	protected void on_tooltip_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			break;
		case TableRow.RowType.Default:
			break;
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = minion as MinionIdentity;
			if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minionIdentity.GetProperName()), null);
				tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS_TOOLTIP, Db.Get().Attributes.QualityOfLifeExpectation.Lookup(minionIdentity).GetFormattedValue()), null);
				tooltip.AddMultiStringTooltip(UI.HORIZONTAL_RULE, null);
				tooltip.AddMultiStringTooltip(Db.Get().Attributes.QualityOfLife.Lookup(minionIdentity).GetAttributeValueTooltip(), null);
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			StoredMinionTooltip(minion, tooltip);
			break;
		}
	}

	protected void on_tooltip_sort_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Default:
			break;
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			break;
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, null);
			break;
		}
	}

	private ResultValues get_value_food_info_super(MinionIdentity minion, GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		bool flag4 = false;
		CheckboxTableColumn[] columns_affected = superCheckboxTableColumn.columns_affected;
		foreach (CheckboxTableColumn checkboxTableColumn in columns_affected)
		{
			switch (checkboxTableColumn.get_value_action(widgetRow.GetIdentity(), widgetRow.GetWidget(checkboxTableColumn)))
			{
			case ResultValues.False:
				flag2 = false;
				if (!flag)
				{
					flag4 = true;
				}
				break;
			case ResultValues.Partial:
				flag3 = true;
				flag4 = true;
				break;
			case ResultValues.True:
				flag = false;
				if (!flag2)
				{
					flag4 = true;
				}
				break;
			}
			if (flag4)
			{
				break;
			}
		}
		if (flag3)
		{
			return ResultValues.Partial;
		}
		if (flag2)
		{
			return ResultValues.True;
		}
		if (flag)
		{
			return ResultValues.False;
		}
		return ResultValues.Partial;
	}

	private void set_value_consumable_info(GameObject widget_go, ResultValues new_value)
	{
		ConsumableConsumer consumableConsumer = null;
		IConsumableUIItem consumableUIItem = null;
		TableRow widgetRow = GetWidgetRow(widget_go);
		if ((UnityEngine.Object)widgetRow == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Row is null");
		}
		else
		{
			ConsumableInfoTableColumn consumableInfoTableColumn = GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
			IAssignableIdentity identity = widgetRow.GetIdentity();
			consumableUIItem = consumableInfoTableColumn.consumable_info;
			switch (widgetRow.rowType)
			{
			case TableRow.RowType.StoredMinon:
				break;
			case TableRow.RowType.Header:
				set_value_consumable_info(default_row.GetComponent<TableRow>().GetWidget(consumableInfoTableColumn), new_value);
				StartCoroutine(CascadeSetColumnCheckBoxes(sortable_rows, consumableInfoTableColumn, new_value, widget_go));
				break;
			case TableRow.RowType.Default:
				if (new_value == ResultValues.True)
				{
					ConsumerManager.instance.DefaultForbiddenTagsList.Remove(consumableUIItem.ConsumableId.ToTag());
				}
				else
				{
					ConsumerManager.instance.DefaultForbiddenTagsList.Add(consumableUIItem.ConsumableId.ToTag());
				}
				consumableInfoTableColumn.on_load_action(identity, widget_go);
				foreach (KeyValuePair<TableRow, GameObject> item in consumableInfoTableColumn.widgets_by_row)
				{
					if (item.Key.rowType == TableRow.RowType.Header)
					{
						consumableInfoTableColumn.on_load_action(null, item.Value);
						break;
					}
				}
				break;
			case TableRow.RowType.Minion:
			{
				MinionIdentity minionIdentity = identity as MinionIdentity;
				if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
				{
					consumableConsumer = minionIdentity.GetComponent<ConsumableConsumer>();
					if ((UnityEngine.Object)consumableConsumer == (UnityEngine.Object)null)
					{
						Debug.LogError("Could not find minion identity / row associated with the widget");
					}
					else
					{
						switch (new_value)
						{
						case ResultValues.True:
						case ResultValues.ConditionalGroup:
							consumableConsumer.SetPermitted(consumableUIItem.ConsumableId, true);
							break;
						case ResultValues.False:
						case ResultValues.Partial:
							consumableConsumer.SetPermitted(consumableUIItem.ConsumableId, false);
							break;
						}
						consumableInfoTableColumn.on_load_action(widgetRow.GetIdentity(), widget_go);
						foreach (KeyValuePair<TableRow, GameObject> item2 in consumableInfoTableColumn.widgets_by_row)
						{
							if (item2.Key.rowType == TableRow.RowType.Header)
							{
								consumableInfoTableColumn.on_load_action(null, item2.Value);
								break;
							}
						}
					}
				}
				break;
			}
			}
		}
	}

	private void on_click_consumable_info(GameObject widget_go)
	{
		ConsumableConsumer consumableConsumer = null;
		IConsumableUIItem consumableUIItem = null;
		TableRow widgetRow = GetWidgetRow(widget_go);
		IAssignableIdentity identity = widgetRow.GetIdentity();
		ConsumableInfoTableColumn consumableInfoTableColumn = GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			switch (get_value_consumable_info(null, widget_go))
			{
			case ResultValues.True:
				consumableInfoTableColumn.on_set_action(widget_go, ResultValues.False);
				break;
			case ResultValues.False:
			case ResultValues.Partial:
			case ResultValues.ConditionalGroup:
				consumableInfoTableColumn.on_set_action(widget_go, ResultValues.True);
				break;
			}
			consumableInfoTableColumn.on_load_action(null, widget_go);
			break;
		case TableRow.RowType.Default:
		{
			consumableUIItem = consumableInfoTableColumn.consumable_info;
			bool flag3 = !ConsumerManager.instance.DefaultForbiddenTagsList.Contains(consumableUIItem.ConsumableId.ToTag());
			consumableInfoTableColumn.on_set_action(widget_go, (!flag3) ? ResultValues.True : ResultValues.False);
			break;
		}
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = identity as MinionIdentity;
			if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
			{
				consumableUIItem = consumableInfoTableColumn.consumable_info;
				consumableConsumer = minionIdentity.GetComponent<ConsumableConsumer>();
				if ((UnityEngine.Object)consumableConsumer == (UnityEngine.Object)null)
				{
					Debug.LogError("Could not find minion identity / row associated with the widget");
				}
				else
				{
					bool flag2 = consumableConsumer.IsPermitted(consumableUIItem.ConsumableId);
					consumableInfoTableColumn.on_set_action(widget_go, (!flag2) ? ResultValues.True : ResultValues.False);
				}
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
		{
			StoredMinionIdentity storedMinionIdentity = identity as StoredMinionIdentity;
			if ((UnityEngine.Object)storedMinionIdentity != (UnityEngine.Object)null)
			{
				consumableUIItem = consumableInfoTableColumn.consumable_info;
				bool flag = storedMinionIdentity.IsPermittedToConsume(consumableUIItem.ConsumableId);
				consumableInfoTableColumn.on_set_action(widget_go, (!flag) ? ResultValues.True : ResultValues.False);
			}
			break;
		}
		}
	}

	private void on_tooltip_consumable_info(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		ConsumableInfoTableColumn consumableInfoTableColumn = GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
		TableRow widgetRow = GetWidgetRow(widget_go);
		EdiblesManager.FoodInfo foodInfo = consumableInfoTableColumn.consumable_info as EdiblesManager.FoodInfo;
		int num = 0;
		if (foodInfo != null)
		{
			int num2 = foodInfo.Quality;
			MinionIdentity minionIdentity = minion as MinionIdentity;
			if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
			{
				AttributeInstance attributeInstance = minionIdentity.GetAttributes().Get(Db.Get().Attributes.FoodExpectation);
				num2 += Mathf.RoundToInt(attributeInstance.GetTotalValue());
			}
			string effectForFoodQuality = Edible.GetEffectForFoodQuality(num2);
			Effect effect = Db.Get().effects.Get(effectForFoodQuality);
			foreach (AttributeModifier selfModifier in effect.SelfModifiers)
			{
				if (selfModifier.AttributeId == Db.Get().Attributes.QualityOfLife.Id)
				{
					num += Mathf.RoundToInt(selfModifier.Value);
				}
			}
		}
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(consumableInfoTableColumn.consumable_info.ConsumableName, null);
			if (foodInfo != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, GameUtil.GetFormattedCalories(WorldInventory.Instance.GetAmount(consumableInfoTableColumn.consumable_info.ConsumableId.ToTag()) * foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), null);
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_QUALITY, GameUtil.AddPositiveSign(num.ToString(), num > 0)), null);
			}
			else
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, GameUtil.GetFormattedUnits(WorldInventory.Instance.GetAmount(consumableInfoTableColumn.consumable_info.ConsumableId.ToTag()), GameUtil.TimeSlice.None, true)), null);
			}
			break;
		case TableRow.RowType.Default:
			if (consumableInfoTableColumn.get_value_action(minion, widget_go) == ResultValues.True)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_ON, consumableInfoTableColumn.consumable_info.ConsumableName), null);
			}
			else
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_OFF, consumableInfoTableColumn.consumable_info.ConsumableName), null);
			}
			break;
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			if (minion != null)
			{
				if (consumableInfoTableColumn.get_value_action(minion, widget_go) == ResultValues.True)
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_PERMISSION_ON, minion.GetProperName(), consumableInfoTableColumn.consumable_info.ConsumableName), null);
				}
				else
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_PERMISSION_OFF, minion.GetProperName(), consumableInfoTableColumn.consumable_info.ConsumableName), null);
				}
				if (foodInfo != null && (UnityEngine.Object)(minion as MinionIdentity) != (UnityEngine.Object)null)
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_QUALITY_VS_EXPECTATION, GameUtil.AddPositiveSign(num.ToString(), num > 0), minion.GetProperName()), null);
				}
				else if ((UnityEngine.Object)(minion as StoredMinionIdentity) != (UnityEngine.Object)null)
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.CANNOT_ADJUST_PERMISSIONS, (minion as StoredMinionIdentity).GetStorageReason()), null);
				}
			}
			break;
		}
	}

	private void on_tooltip_sort_consumable_info(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
	}

	private void on_tooltip_consumable_info_super(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.StoredMinon:
			break;
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ALL.text, null);
			break;
		case TableRow.RowType.Default:
			tooltip.AddMultiStringTooltip(UI.CONSUMABLESSCREEN.NEW_MINIONS_TOOLTIP_TOGGLE_ROW, null);
			break;
		case TableRow.RowType.Minion:
			if ((UnityEngine.Object)(minion as MinionIdentity) != (UnityEngine.Object)null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ROW.text, (minion as MinionIdentity).gameObject.GetProperName()), null);
			}
			break;
		}
	}

	private void on_load_consumable_info(IAssignableIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = GetWidgetRow(widget_go);
		TableColumn widgetColumn = GetWidgetColumn(widget_go);
		IConsumableUIItem consumable_info = (widgetColumn as ConsumableInfoTableColumn).consumable_info;
		EdiblesManager.FoodInfo foodInfo = consumable_info as EdiblesManager.FoodInfo;
		MultiToggle component = widget_go.GetComponent<MultiToggle>();
		if (!widgetColumn.isRevealed)
		{
			widget_go.SetActive(false);
		}
		else
		{
			if (!widget_go.activeSelf)
			{
				widget_go.SetActive(true);
			}
			switch (widgetRow.rowType)
			{
			case TableRow.RowType.Header:
			{
				GameObject prefab = Assets.GetPrefab(consumable_info.ConsumableId.ToTag());
				if ((UnityEngine.Object)prefab == (UnityEngine.Object)null)
				{
					return;
				}
				KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
				Image image2 = widget_go.GetComponent<HierarchyReferences>().GetReference("PortraitImage") as Image;
				if (component2.AnimFiles.Length > 0)
				{
					Sprite sprite = image2.sprite = Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], "ui", false);
				}
				image2.color = Color.white;
				image2.material = ((!(WorldInventory.Instance.GetAmount(consumable_info.ConsumableId.ToTag()) > 0f)) ? Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial);
				break;
			}
			case TableRow.RowType.Default:
				switch (get_value_consumable_info(minion, widget_go))
				{
				case ResultValues.False:
					component.ChangeState(0);
					break;
				case ResultValues.True:
					component.ChangeState(1);
					break;
				case ResultValues.ConditionalGroup:
					component.ChangeState(2);
					break;
				}
				break;
			case TableRow.RowType.Minion:
			case TableRow.RowType.StoredMinon:
				switch (get_value_consumable_info(minion, widget_go))
				{
				case ResultValues.False:
					component.ChangeState(0);
					break;
				case ResultValues.True:
					component.ChangeState(1);
					break;
				case ResultValues.ConditionalGroup:
					component.ChangeState(2);
					break;
				}
				if (foodInfo != null && (UnityEngine.Object)(minion as MinionIdentity) != (UnityEngine.Object)null)
				{
					Image image = widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image;
					Color color2 = image.color = new Color(0.721568644f, 0.443137258f, 0.5803922f, Mathf.Max((float)foodInfo.Quality - Db.Get().Attributes.FoodExpectation.Lookup(minion as MinionIdentity).GetTotalValue() + 1f, 0f) * 0.25f);
				}
				break;
			}
			refresh_scrollers();
		}
	}

	private int compare_consumable_info(IAssignableIdentity a, IAssignableIdentity b)
	{
		return 0;
	}

	private ResultValues get_value_consumable_info(IAssignableIdentity minion, GameObject widget_go)
	{
		ConsumableInfoTableColumn consumableInfoTableColumn = GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
		IConsumableUIItem consumable_info = consumableInfoTableColumn.consumable_info;
		TableRow widgetRow = GetWidgetRow(widget_go);
		ResultValues result = ResultValues.Partial;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			foreach (KeyValuePair<TableRow, GameObject> item in consumableInfoTableColumn.widgets_by_row)
			{
				GameObject value = item.Value;
				if (!((UnityEngine.Object)value == (UnityEngine.Object)widget_go) && !((UnityEngine.Object)value == (UnityEngine.Object)null))
				{
					switch (consumableInfoTableColumn.get_value_action(item.Key.GetIdentity(), value))
					{
					case ResultValues.False:
						flag2 = false;
						if (!flag)
						{
							flag4 = true;
						}
						break;
					case ResultValues.Partial:
						flag3 = true;
						flag4 = true;
						break;
					case ResultValues.True:
						flag = false;
						if (!flag2)
						{
							flag4 = true;
						}
						break;
					}
					if (flag4)
					{
						break;
					}
				}
			}
			result = (flag3 ? ResultValues.Partial : (flag2 ? ResultValues.True : ((!flag) ? ResultValues.Partial : ResultValues.False)));
			break;
		}
		case TableRow.RowType.Default:
			result = ((!ConsumerManager.instance.DefaultForbiddenTagsList.Contains(consumable_info.ConsumableId.ToTag())) ? ResultValues.True : ResultValues.False);
			break;
		case TableRow.RowType.Minion:
			if ((UnityEngine.Object)(minion as MinionIdentity) != (UnityEngine.Object)null)
			{
				ConsumableConsumer component = ((MinionIdentity)minion).GetComponent<ConsumableConsumer>();
				result = (component.IsPermitted(consumable_info.ConsumableId) ? ResultValues.True : ResultValues.False);
			}
			else
			{
				result = ResultValues.True;
			}
			break;
		case TableRow.RowType.StoredMinon:
			result = ((!((UnityEngine.Object)(minion as StoredMinionIdentity) != (UnityEngine.Object)null)) ? ResultValues.True : (((StoredMinionIdentity)minion).IsPermittedToConsume(consumable_info.ConsumableId) ? ResultValues.True : ResultValues.False));
			break;
		}
		return result;
	}

	protected void on_tooltip_name(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			break;
		case TableRow.RowType.Default:
			break;
		case TableRow.RowType.StoredMinon:
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, minion.GetProperName()), null);
			}
			break;
		}
	}

	protected ConsumableInfoTableColumn AddConsumableInfoColumn(string id, IConsumableUIItem consumable_info, Action<IAssignableIdentity, GameObject> load_value_action, Func<IAssignableIdentity, GameObject, ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, ResultValues> set_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		ConsumableInfoTableColumn consumableInfoTableColumn = new ConsumableInfoTableColumn(consumable_info, load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, (GameObject widget_go) => string.Empty);
		consumableInfoTableColumn.scrollerID = "consumableScroller";
		if (RegisterColumn(id, consumableInfoTableColumn))
		{
			return consumableInfoTableColumn;
		}
		return null;
	}

	private void OnConsumableDiscovered(Tag tag)
	{
		MarkRowsDirty();
	}

	private void StoredMinionTooltip(IAssignableIdentity minion, ToolTip tooltip)
	{
		StoredMinionIdentity storedMinionIdentity = minion as StoredMinionIdentity;
		if ((UnityEngine.Object)storedMinionIdentity != (UnityEngine.Object)null)
		{
			tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, storedMinionIdentity.GetStorageReason(), storedMinionIdentity.GetProperName()), null);
		}
	}
}
