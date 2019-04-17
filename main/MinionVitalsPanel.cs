using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class MinionVitalsPanel : KMonoBehaviour
{
	[DebuggerDisplay("{amount.Name}")]
	public struct AmountLine
	{
		public Amount amount;

		public GameObject go;

		public ValueTrendImageToggle imageToggle;

		public LocText locText;

		public ToolTip toolTip;

		public Func<AmountInstance, string> toolTipFunc;

		public bool TryUpdate(Amounts amounts)
		{
			foreach (AmountInstance amount2 in amounts)
			{
				if (amount == amount2.amount && !amount2.hide)
				{
					locText.SetText(amount.GetDescription(amount2));
					toolTip.toolTip = toolTipFunc(amount2);
					imageToggle.SetValue(amount2);
					return true;
				}
			}
			return false;
		}
	}

	[DebuggerDisplay("{attribute.Name}")]
	public struct AttributeLine
	{
		public Klei.AI.Attribute attribute;

		public GameObject go;

		public LocText locText;

		public ToolTip toolTip;

		public Func<AttributeInstance, string> toolTipFunc;

		public bool TryUpdate(Attributes attributes)
		{
			foreach (AttributeInstance attribute2 in attributes)
			{
				if (attribute == attribute2.modifier && !attribute2.hide)
				{
					locText.SetText(attribute.GetDescription(attribute2));
					toolTip.toolTip = toolTipFunc(attribute2);
					return true;
				}
			}
			return false;
		}
	}

	public struct CheckboxLine
	{
		public Amount amount;

		public GameObject go;

		public LocText locText;

		public Func<GameObject, string> tooltip;

		public Func<GameObject, bool> get_value;

		public Func<GameObject, CheckboxLineDisplayType> display_condition;

		public Func<GameObject, string> label_text_func;

		public Transform parentContainer;
	}

	public enum CheckboxLineDisplayType
	{
		Normal,
		Diminished,
		Hidden
	}

	public GameObject LineItemPrefab;

	public GameObject CheckboxLinePrefab;

	public GameObject selectedEntity;

	public List<AmountLine> amountsLines = new List<AmountLine>();

	public List<AttributeLine> attributesLines = new List<AttributeLine>();

	public List<CheckboxLine> checkboxLines = new List<CheckboxLine>();

	public Transform conditionsContainerNormal;

	public Transform conditionsContainerAdditional;

	public void Init()
	{
		AddAmountLine(Db.Get().Amounts.HitPoints, null);
		AddAttributeLine(Db.Get().CritterAttributes.Happiness, null);
		AddAmountLine(Db.Get().Amounts.Wildness, null);
		AddAmountLine(Db.Get().Amounts.Incubation, null);
		AddAmountLine(Db.Get().Amounts.Viability, null);
		AddAmountLine(Db.Get().Amounts.Fertility, null);
		AddAmountLine(Db.Get().Amounts.Age, null);
		AddAmountLine(Db.Get().Amounts.Stress, null);
		AddAttributeLine(Db.Get().Attributes.QualityOfLife, null);
		AddAmountLine(Db.Get().Amounts.Bladder, null);
		AddAmountLine(Db.Get().Amounts.Breath, null);
		AddAmountLine(Db.Get().Amounts.Stamina, null);
		AddAmountLine(Db.Get().Amounts.Calories, null);
		AddAttributeLine(Db.Get().Attributes.GermSusceptibility, null);
		AddAmountLine(Db.Get().Amounts.ScaleGrowth, null);
		AddAmountLine(Db.Get().Amounts.Temperature, null);
		AddAmountLine(Db.Get().Amounts.Decor, null);
		AddCheckboxLine(Db.Get().Amounts.AirPressure, conditionsContainerNormal, (GameObject go) => GetAirPressureLabel(go), delegate(GameObject go)
		{
			if ((UnityEngine.Object)go.GetComponent<PressureVulnerable>() != (UnityEngine.Object)null && go.GetComponent<PressureVulnerable>().pressure_sensitive)
			{
				return CheckboxLineDisplayType.Normal;
			}
			return CheckboxLineDisplayType.Hidden;
		}, (GameObject go) => check_pressure(go), (GameObject go) => GetAirPressureTooltip(go));
		AddCheckboxLine(null, conditionsContainerNormal, (GameObject go) => GetAtmosphereLabel(go), delegate(GameObject go)
		{
			if ((UnityEngine.Object)go.GetComponent<PressureVulnerable>() != (UnityEngine.Object)null && go.GetComponent<PressureVulnerable>().safe_atmospheres.Count > 0)
			{
				return CheckboxLineDisplayType.Normal;
			}
			return CheckboxLineDisplayType.Hidden;
		}, (GameObject go) => check_atmosphere(go), (GameObject go) => GetAtmosphereTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Temperature, conditionsContainerNormal, (GameObject go) => GetInternalTemperatureLabel(go), delegate(GameObject go)
		{
			if ((UnityEngine.Object)go.GetComponent<TemperatureVulnerable>() != (UnityEngine.Object)null)
			{
				return CheckboxLineDisplayType.Normal;
			}
			return CheckboxLineDisplayType.Hidden;
		}, (GameObject go) => check_temperature(go), (GameObject go) => GetInternalTemperatureTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Fertilization, conditionsContainerAdditional, (GameObject go) => GetFertilizationLabel(go), delegate(GameObject go)
		{
			if ((UnityEngine.Object)go.GetComponent<Growing>() == (UnityEngine.Object)null)
			{
				return CheckboxLineDisplayType.Hidden;
			}
			if (go.GetComponent<Growing>().Replanted)
			{
				return CheckboxLineDisplayType.Normal;
			}
			return CheckboxLineDisplayType.Diminished;
		}, (GameObject go) => check_fertilizer(go), (GameObject go) => GetFertilizationTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Irrigation, conditionsContainerAdditional, (GameObject go) => GetIrrigationLabel(go), delegate(GameObject go)
		{
			Growing component = go.GetComponent<Growing>();
			return (!((UnityEngine.Object)component != (UnityEngine.Object)null) || !component.Replanted) ? CheckboxLineDisplayType.Diminished : CheckboxLineDisplayType.Normal;
		}, (GameObject go) => check_irrigation(go), (GameObject go) => GetIrrigationTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Illumination, conditionsContainerNormal, (GameObject go) => GetIlluminationLabel(go), (GameObject go) => CheckboxLineDisplayType.Normal, (GameObject go) => check_illumination(go), (GameObject go) => GetIlluminationTooltip(go));
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		SimAndRenderScheduler.instance.Add(this, false);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		SimAndRenderScheduler.instance.Remove(this);
	}

	private void AddAmountLine(Amount amount, Func<AmountInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(LineItemPrefab, base.gameObject, false);
		gameObject.GetComponentInChildren<Image>().sprite = Assets.GetSprite(amount.uiSprite);
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		AmountLine item = default(AmountLine);
		item.amount = amount;
		item.go = gameObject;
		item.locText = gameObject.GetComponentInChildren<LocText>();
		item.toolTip = gameObject.GetComponentInChildren<ToolTip>();
		item.imageToggle = gameObject.GetComponentInChildren<ValueTrendImageToggle>();
		item.toolTipFunc = ((tooltip_func == null) ? new Func<AmountInstance, string>(amount.GetTooltip) : tooltip_func);
		amountsLines.Add(item);
	}

	private void AddAttributeLine(Klei.AI.Attribute attribute, Func<AttributeInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(LineItemPrefab, base.gameObject, false);
		gameObject.GetComponentInChildren<Image>().sprite = Assets.GetSprite(attribute.uiSprite);
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		AttributeLine item = default(AttributeLine);
		item.attribute = attribute;
		item.go = gameObject;
		item.locText = gameObject.GetComponentInChildren<LocText>();
		item.toolTip = gameObject.GetComponentInChildren<ToolTip>();
		gameObject.GetComponentInChildren<ValueTrendImageToggle>().gameObject.SetActive(false);
		item.toolTipFunc = ((tooltip_func == null) ? new Func<AttributeInstance, string>(attribute.GetTooltip) : tooltip_func);
		attributesLines.Add(item);
	}

	private void AddCheckboxLine(Amount amount, Transform parentContainer, Func<GameObject, string> label_text_func, Func<GameObject, CheckboxLineDisplayType> display_condition, Func<GameObject, bool> checkbox_value_func, Func<GameObject, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(CheckboxLinePrefab, base.gameObject, false);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		CheckboxLine item = default(CheckboxLine);
		item.go = gameObject;
		item.parentContainer = parentContainer;
		item.amount = amount;
		item.locText = (component.GetReference("Label") as LocText);
		item.get_value = checkbox_value_func;
		item.display_condition = display_condition;
		item.label_text_func = label_text_func;
		item.go.name = "Checkbox_";
		if (amount != null)
		{
			GameObject go = item.go;
			go.name += amount.Name;
		}
		else
		{
			GameObject go2 = item.go;
			go2.name += "Unnamed";
		}
		if (tooltip_func != null)
		{
			item.tooltip = tooltip_func;
			ToolTip tt = item.go.GetComponent<ToolTip>();
			tt.refreshWhileHovering = true;
			tt.OnToolTip = delegate
			{
				tt.ClearMultiStringTooltip();
				tt.AddMultiStringTooltip(tooltip_func(selectedEntity), null);
				return string.Empty;
			};
		}
		checkboxLines.Add(item);
	}

	public void Refresh()
	{
		if (!((UnityEngine.Object)selectedEntity == (UnityEngine.Object)null) && !((UnityEngine.Object)selectedEntity.gameObject == (UnityEngine.Object)null))
		{
			Amounts amounts = selectedEntity.GetAmounts();
			Attributes attributes = selectedEntity.GetAttributes();
			if (amounts != null && attributes != null)
			{
				WiltCondition component = selectedEntity.GetComponent<WiltCondition>();
				if ((UnityEngine.Object)component == (UnityEngine.Object)null)
				{
					conditionsContainerNormal.gameObject.SetActive(false);
					conditionsContainerAdditional.gameObject.SetActive(false);
					foreach (AmountLine amountsLine in amountsLines)
					{
						AmountLine current = amountsLine;
						bool flag = current.TryUpdate(amounts);
						if (current.go.activeSelf != flag)
						{
							current.go.SetActive(flag);
						}
					}
					foreach (AttributeLine attributesLine in attributesLines)
					{
						AttributeLine current2 = attributesLine;
						bool flag2 = current2.TryUpdate(attributes);
						if (current2.go.activeSelf != flag2)
						{
							current2.go.SetActive(flag2);
						}
					}
				}
				bool flag3 = false;
				for (int i = 0; i < checkboxLines.Count; i++)
				{
					CheckboxLine checkboxLine = checkboxLines[i];
					CheckboxLineDisplayType checkboxLineDisplayType = CheckboxLineDisplayType.Hidden;
					CheckboxLine checkboxLine2 = checkboxLines[i];
					if (checkboxLine2.amount != null)
					{
						for (int j = 0; j < amounts.Count; j++)
						{
							AmountInstance amountInstance = amounts[j];
							if (checkboxLine.amount == amountInstance.amount)
							{
								checkboxLineDisplayType = checkboxLine.display_condition(selectedEntity.gameObject);
								break;
							}
						}
					}
					else
					{
						checkboxLineDisplayType = checkboxLine.display_condition(selectedEntity.gameObject);
					}
					if (checkboxLineDisplayType != CheckboxLineDisplayType.Hidden)
					{
						checkboxLine.locText.SetText(checkboxLine.label_text_func(selectedEntity.gameObject));
						if (!checkboxLine.go.activeSelf)
						{
							checkboxLine.go.SetActive(true);
						}
						GameObject gameObject = checkboxLine.go.GetComponent<HierarchyReferences>().GetReference("Check").gameObject;
						gameObject.SetActive(checkboxLine.get_value(selectedEntity.gameObject));
						if ((UnityEngine.Object)checkboxLine.go.transform.parent != (UnityEngine.Object)checkboxLine.parentContainer)
						{
							checkboxLine.go.transform.SetParent(checkboxLine.parentContainer);
							checkboxLine.go.transform.localScale = Vector3.one;
						}
						if ((UnityEngine.Object)checkboxLine.parentContainer == (UnityEngine.Object)conditionsContainerAdditional)
						{
							flag3 = true;
						}
						if (checkboxLineDisplayType == CheckboxLineDisplayType.Normal)
						{
							if (checkboxLine.get_value(selectedEntity.gameObject))
							{
								checkboxLine.locText.color = Color.black;
								gameObject.transform.parent.GetComponent<Image>().color = Color.black;
							}
							else
							{
								Color color = new Color(0.992156863f, 0f, 0.101960786f);
								checkboxLine.locText.color = color;
								gameObject.transform.parent.GetComponent<Image>().color = color;
							}
						}
						else
						{
							checkboxLine.locText.color = Color.grey;
							gameObject.transform.parent.GetComponent<Image>().color = Color.grey;
						}
					}
					else if (checkboxLine.go.activeSelf)
					{
						checkboxLine.go.SetActive(false);
					}
				}
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					Growing component2 = component.GetComponent<Growing>();
					conditionsContainerNormal.gameObject.SetActive(true);
					conditionsContainerAdditional.gameObject.SetActive((UnityEngine.Object)component2 != (UnityEngine.Object)null);
					if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
					{
						LocText reference = conditionsContainerNormal.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
						reference.text = string.Empty;
					}
					else
					{
						LocText reference = conditionsContainerNormal.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
						reference.text = string.Empty;
						reference.text = string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().WildGrowthTime(), "F1"));
						reference.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().WildGrowthTime(), "F1")));
						reference = conditionsContainerAdditional.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
						reference.color = ((!selectedEntity.GetComponent<Growing>().Replanted) ? Color.grey : Color.black);
						reference.text = string.Empty;
						reference.text = ((!flag3) ? string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")) : string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")));
						reference.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")));
					}
					foreach (AmountLine amountsLine2 in amountsLines)
					{
						AmountLine current3 = amountsLine2;
						current3.go.SetActive(false);
					}
					foreach (AttributeLine attributesLine2 in attributesLines)
					{
						AttributeLine current4 = attributesLine2;
						current4.go.SetActive(false);
					}
				}
			}
		}
	}

	private string GetAirPressureTooltip(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_PRESSURE.text.Replace("{pressure}", GameUtil.GetFormattedMass(component.GetExternalPressure, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetInternalTemperatureTooltip(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_TEMPERATURE.text.Replace("{temperature}", GameUtil.GetFormattedTemperature(component.InternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
	}

	private string GetFertilizationTooltip(GameObject go)
	{
		FertilizationMonitor.Instance sMI = go.GetSMI<FertilizationMonitor.Instance>();
		if (sMI == null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_FERTILIZER.text.Replace("{mass}", GameUtil.GetFormattedMass(sMI.total_fertilizer_available, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetIrrigationTooltip(GameObject go)
	{
		IrrigationMonitor.Instance sMI = go.GetSMI<IrrigationMonitor.Instance>();
		if (sMI == null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_IRRIGATION.text.Replace("{mass}", GameUtil.GetFormattedMass(sMI.total_fertilizer_available, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetIlluminationTooltip(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return string.Empty;
		}
		if ((component.prefersDarkness && component.IsComfortable()) || (!component.prefersDarkness && !component.IsComfortable()))
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_DARK;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_LIGHT;
	}

	private string GetReceptacleTooltip(GameObject go)
	{
		ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return string.Empty;
		}
		if (component.HasOperationalReceptacle())
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_RECEPTACLE_OPERATIONAL;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_RECEPTACLE_INOPERATIONAL;
	}

	private string GetAtmosphereTooltip(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ATMOSPHERE.text.Replace("{element}", component.GetExternalElement.name);
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ATMOSPHERE;
	}

	private string GetAirPressureLabel(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		return Db.Get().Amounts.AirPressure.Name + "\n    • " + GameUtil.GetFormattedMass(component.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Gram, false, "{0:0.#}") + " - " + GameUtil.GetFormattedMass(component.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Gram, true, "{0:0.#}");
	}

	private string GetInternalTemperatureLabel(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		return Db.Get().Amounts.Temperature.Name + "\n    • " + GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false, false) + " - " + GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
	}

	private string GetFertilizationLabel(GameObject go)
	{
		FertilizationMonitor.Instance sMI = go.GetSMI<FertilizationMonitor.Instance>();
		string text = Db.Get().Amounts.Fertilization.Name;
		PlantElementAbsorber.ConsumeInfo[] consumedElements = sMI.def.consumedElements;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements[i];
			string text2 = text;
			text = text2 + "\n    • " + ElementLoader.GetElement(consumeInfo.tag).name + " " + GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		}
		return text;
	}

	private string GetIrrigationLabel(GameObject go)
	{
		IrrigationMonitor.Instance sMI = go.GetSMI<IrrigationMonitor.Instance>();
		return Db.Get().Amounts.Irrigation.Name + "\n    • " + ElementLoader.GetElement(sMI.def.consumedElements[0].tag).name + ": " + GameUtil.GetFormattedMass(sMI.def.consumedElements[0].massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
	}

	private string GetIlluminationLabel(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		return Db.Get().Amounts.Illumination.Name + "\n    • " + ((!component.prefersDarkness) ? UI.GAMEOBJECTEFFECTS.LIGHT : UI.GAMEOBJECTEFFECTS.DARKNESS);
	}

	private string GetAtmosphereLabel(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		string text = UI.VITALSSCREEN.ATMOSPHERE_CONDITION;
		foreach (Element safe_atmosphere in component.safe_atmospheres)
		{
			text = text + "\n    • " + safe_atmosphere.name;
		}
		return text;
	}

	private bool check_pressure(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.GetExternalPressureState == PressureVulnerable.PressureState.Normal;
		}
		return true;
	}

	private bool check_temperature(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal;
		}
		return true;
	}

	private bool check_irrigation(GameObject go)
	{
		IrrigationMonitor.Instance sMI = go.GetSMI<IrrigationMonitor.Instance>();
		if (sMI != null)
		{
			return !sMI.IsInsideState(sMI.sm.replanted.starved);
		}
		return true;
	}

	private bool check_illumination(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.IsComfortable();
		}
		return true;
	}

	private bool check_receptacle(GameObject go)
	{
		ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return false;
		}
		return component.HasOperationalReceptacle();
	}

	private bool check_fertilizer(GameObject go)
	{
		FertilizationMonitor.Instance sMI = go.GetSMI<FertilizationMonitor.Instance>();
		if (sMI != null)
		{
			return sMI.sm.hasCorrectFertilizer.Get(sMI);
		}
		return true;
	}

	private bool check_atmosphere(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.IsSafeElement(Grid.Element[Grid.PosToCell(go)]);
		}
		return true;
	}
}
