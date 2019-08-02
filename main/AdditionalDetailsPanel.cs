using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalDetailsPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		detailsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		drawer = new DetailsPanelDrawer(attributesLabelTemplate, detailsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	private void Update()
	{
		Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	private void Refresh()
	{
		drawer.BeginDrawing();
		RefreshDetails();
		drawer.EndDrawing();
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void RefreshDetails()
	{
		detailsPanel.SetActive(true);
		detailsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_DETAILS;
		PrimaryElement component = selectedTarget.GetComponent<PrimaryElement>();
		CellSelectionObject component2 = selectedTarget.GetComponent<CellSelectionObject>();
		float mass;
		float temperature;
		Element element;
		byte diseaseIdx;
		int diseaseCount;
		if ((Object)component != (Object)null)
		{
			mass = component.Mass;
			temperature = component.Temperature;
			element = component.Element;
			diseaseIdx = component.DiseaseIdx;
			diseaseCount = component.DiseaseCount;
		}
		else
		{
			if (!((Object)component2 != (Object)null))
			{
				return;
			}
			mass = component2.Mass;
			temperature = component2.temperature;
			element = component2.element;
			diseaseIdx = component2.diseaseIdx;
			diseaseCount = component2.diseaseCount;
		}
		bool flag = element.id == SimHashes.Vacuum || element.id == SimHashes.Void;
		float specificHeatCapacity = element.specificHeatCapacity;
		float highTemp = element.highTemp;
		float lowTemp = element.lowTemp;
		drawer.NewLabel(drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name)).Tooltip(drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name)).NewLabel(drawer.Format(UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")))
			.Tooltip(drawer.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")));
		if (!flag)
		{
			bool flag2 = false;
			float num = element.thermalConductivity;
			Building component3 = selectedTarget.GetComponent<Building>();
			if ((Object)component3 != (Object)null)
			{
				num *= component3.Def.ThermalConductivity;
				flag2 = (component3.Def.ThermalConductivity < 1f);
			}
			string temperatureUnitSuffix = GameUtil.GetTemperatureUnitSuffix();
			float shc = specificHeatCapacity * 1f;
			string text = string.Format(UI.ELEMENTAL.SHC.NAME, GameUtil.GetDisplaySHC(shc).ToString("0.000"));
			string text2 = UI.ELEMENTAL.SHC.TOOLTIP;
			text2 = text2.Replace("{SPECIFIC_HEAT_CAPACITY}", text + GameUtil.GetSHCSuffix());
			text2 = text2.Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
			string text3 = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, GameUtil.GetDisplayThermalConductivity(num).ToString("0.000"));
			string text4 = UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP;
			text4 = text4.Replace("{THERMAL_CONDUCTIVITY}", text3 + GameUtil.GetThermalConductivitySuffix());
			text4 = text4.Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(drawer.Format(UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).NewLabel(drawer.Format(UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(diseaseIdx, diseaseCount, false)))
				.Tooltip(drawer.Format(UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(diseaseIdx, diseaseCount, true)))
				.NewLabel(text)
				.Tooltip(text2)
				.NewLabel(text3)
				.Tooltip(text4);
			if (flag2)
			{
				drawer.NewLabel(UI.GAMEOBJECTEFFECTS.INSULATED.NAME).Tooltip(UI.GAMEOBJECTEFFECTS.INSULATED.TOOLTIP);
			}
		}
		if (element.IsSolid)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(drawer.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			ElementChunk component4 = selectedTarget.GetComponent<ElementChunk>();
			if ((Object)component4 != (Object)null)
			{
				AttributeModifier attributeModifier = component.Element.attributeModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id);
				if (attributeModifier != null)
				{
					drawer.NewLabel(drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString(selectedTarget.gameObject))).Tooltip(drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString(selectedTarget.gameObject)));
				}
			}
		}
		else if (element.IsLiquid)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(drawer.Format(UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).NewLabel(drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)))
				.Tooltip(drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
		}
		else if (!flag)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(drawer.Format(UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
		}
		Attributes attributes = selectedTarget.GetAttributes();
		if (attributes != null)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				AttributeInstance attributeInstance = attributes.AttributeTable[i];
				if (attributeInstance.Attribute.ShowInUI == Attribute.Display.Details || attributeInstance.Attribute.ShowInUI == Attribute.Display.Expectation)
				{
					drawer.NewLabel(attributeInstance.modifier.Name + ": " + attributeInstance.GetFormattedValue()).Tooltip(attributeInstance.GetAttributeValueTooltip());
				}
			}
		}
		List<Descriptor> detailDescriptors = GameUtil.GetDetailDescriptors(GameUtil.GetAllDescriptors(selectedTarget, false));
		for (int j = 0; j < detailDescriptors.Count; j++)
		{
			Descriptor descriptor = detailDescriptors[j];
			drawer.NewLabel(descriptor.text).Tooltip(descriptor.tooltipText);
		}
	}
}
