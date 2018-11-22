using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseInfoScreen : TargetScreen
{
	private CollapsibleDetailContentPanel infectionPanel;

	private CollapsibleDetailContentPanel immuneSystemPanel;

	private CollapsibleDetailContentPanel diseaseSourcePanel;

	private CollapsibleDetailContentPanel currentGermsPanel;

	private CollapsibleDetailContentPanel infoPanel;

	private static readonly EventSystem.IntraObjectHandler<DiseaseInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DiseaseInfoScreen>(delegate(DiseaseInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		diseaseSourcePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		diseaseSourcePanel.SetTitle(UI.DETAILTABS.DISEASE.DISEASE_SOURCE);
		immuneSystemPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		immuneSystemPanel.SetTitle(UI.DETAILTABS.DISEASE.IMMUNE_SYSTEM);
		currentGermsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		currentGermsPanel.SetTitle(UI.DETAILTABS.DISEASE.CURRENT_GERMS);
		infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		infoPanel.SetTitle(UI.DETAILTABS.DISEASE.GERMS_INFO);
		infectionPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		infectionPanel.SetTitle(UI.DETAILTABS.DISEASE.INFECTION_INFO);
		Subscribe(-1514841199, OnRefreshDataDelegate);
	}

	private void LateUpdate()
	{
		Refresh();
	}

	private void OnRefreshData(object obj)
	{
		Refresh();
	}

	private void Refresh()
	{
		if ((Object)selectedTarget == (Object)null)
		{
			return;
		}
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(selectedTarget, true);
		Diseases diseases = selectedTarget.GetDiseases();
		if (diseases != null)
		{
			for (int i = 0; i < diseases.Count; i++)
			{
				allDescriptors.AddRange(diseases[i].GetDescriptors());
			}
		}
		allDescriptors = allDescriptors.FindAll((Descriptor e) => e.type == Descriptor.DescriptorType.DiseaseSource);
		if (allDescriptors.Count > 0)
		{
			for (int j = 0; j < allDescriptors.Count; j++)
			{
				CollapsibleDetailContentPanel collapsibleDetailContentPanel = diseaseSourcePanel;
				string id = "source_" + j.ToString();
				Descriptor descriptor = allDescriptors[j];
				string text = descriptor.text;
				Descriptor descriptor2 = allDescriptors[j];
				collapsibleDetailContentPanel.SetLabel(id, text, descriptor2.tooltipText);
			}
		}
		if (!CreateImmuneInfo())
		{
			goto IL_00fd;
		}
		goto IL_00fd;
		IL_00fd:
		if (!CreateDiseaseInfo())
		{
			currentGermsPanel.SetTitle(UI.DETAILTABS.DISEASE.NO_CURRENT_GERMS);
			currentGermsPanel.SetLabel("nodisease", UI.DETAILTABS.DISEASE.DETAILS.NODISEASE, UI.DETAILTABS.DISEASE.DETAILS.NODISEASE_TOOLTIP);
		}
		diseaseSourcePanel.Commit();
		immuneSystemPanel.Commit();
		currentGermsPanel.Commit();
		infoPanel.Commit();
		infectionPanel.Commit();
	}

	private bool CreateImmuneInfo()
	{
		ImmuneSystemMonitor.Instance sMI = selectedTarget.GetSMI<ImmuneSystemMonitor.Instance>();
		if (sMI == null)
		{
			return false;
		}
		for (int i = 0; i < Db.Get().Diseases.Count; i++)
		{
			Disease disease = Db.Get().Diseases[i];
			AmountInstance amountInstance = disease.amount.Lookup(selectedTarget);
			if (amountInstance.value > 0f)
			{
				immuneSystemPanel.SetLabel("disease_" + disease.Id, string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.INTERNAL_GERMS, disease.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))), string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.INTERNAL_GERMS_TOOLTIP, disease.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))));
				AttributeModifier currentImmuneModifier = sMI.GetCurrentImmuneModifier(disease);
				if (currentImmuneModifier != null)
				{
					immuneSystemPanel.SetLabel("disease_rate2_" + disease.Id, string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.IMMUNE_ATTACK_RATE2, currentImmuneModifier.GetFormattedString(selectedTarget, false), GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))), string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.IMMUNE_ATTACK_RATE2_TOOLTIP, currentImmuneModifier.GetFormattedString(selectedTarget, false), GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))));
				}
			}
		}
		return true;
	}

	private bool CreateDiseaseInfo()
	{
		PrimaryElement component = selectedTarget.GetComponent<PrimaryElement>();
		if (!((Object)component != (Object)null))
		{
			CellSelectionObject component2 = selectedTarget.GetComponent<CellSelectionObject>();
			if (!((Object)component2 != (Object)null))
			{
				return false;
			}
			return CreateDiseaseInfo_CellSelectionObject(component2);
		}
		return CreateDiseaseInfo_PrimaryElement();
	}

	private string GetFormattedHalfLife(float hl)
	{
		return GetFormattedGrowthRate(Disease.HalfLifeToGrowthRate(hl, 600f));
	}

	private string GetFormattedGrowthRate(float rate)
	{
		if (!(rate < 1f))
		{
			if (!(rate > 1f))
			{
				return string.Format(UI.DETAILTABS.DISEASE.DETAILS.NEUTRAL_FORMAT, UI.DETAILTABS.DISEASE.DETAILS.NEUTRAL_FORMAT_TOOLTIP);
			}
			return string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FORMAT, GameUtil.GetFormattedPercent(100f * (rate - 1f), GameUtil.TimeSlice.None), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FORMAT_TOOLTIP);
		}
		return string.Format(UI.DETAILTABS.DISEASE.DETAILS.DEATH_FORMAT, GameUtil.GetFormattedPercent(100f * (1f - rate), GameUtil.TimeSlice.None), UI.DETAILTABS.DISEASE.DETAILS.DEATH_FORMAT_TOOLTIP);
	}

	private string GetFormattedGrowthEntry(string name, float halfLife, string dyingFormat, string growingFormat, string neutralFormat)
	{
		string format = (halfLife == float.PositiveInfinity) ? neutralFormat : ((!(halfLife > 0f)) ? growingFormat : dyingFormat);
		return string.Format(format, name, GetFormattedHalfLife(halfLife));
	}

	private void BuildFactorsStrings(int diseaseCount, int elementIdx, int environmentCell, float environmentMass, float temperature, HashSet<Tag> tags, Disease disease)
	{
		currentGermsPanel.SetTitle(string.Format(UI.DETAILTABS.DISEASE.CURRENT_GERMS, disease.Name.ToUpper()));
		currentGermsPanel.SetLabel("currentgerms", string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT, disease.Name, GameUtil.GetFormattedDiseaseAmount(diseaseCount)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT_TOOLTIP, GameUtil.GetFormattedDiseaseAmount(diseaseCount)));
		Element e = ElementLoader.elements[elementIdx];
		CompositeGrowthRule growthRuleForElement = disease.GetGrowthRuleForElement(e);
		float tags_multiplier_base = 1f;
		if (tags != null && tags.Count > 0)
		{
			tags_multiplier_base = disease.GetGrowthRateForTags(tags, (float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass);
		}
		float num = DiseaseContainers.CalculateDelta(diseaseCount, elementIdx, environmentMass, environmentCell, temperature, tags_multiplier_base, disease, 1f);
		currentGermsPanel.SetLabel("finaldelta", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE, GameUtil.GetFormattedSimple(num, GameUtil.TimeSlice.PerSecond, "F0")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE_TOOLTIP, GameUtil.GetFormattedSimple(num, GameUtil.TimeSlice.PerSecond, "F0")));
		float num2 = Disease.GrowthRateToHalfLife(1f - num / (float)diseaseCount);
		if (num2 > 0f)
		{
			currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG, GameUtil.GetFormattedCycles(num2, "F1")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG_TOOLTIP, GameUtil.GetFormattedCycles(num2, "F1")));
		}
		else if (num2 < 0f)
		{
			currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS, GameUtil.GetFormattedCycles(0f - num2, "F1")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS_TOOLTIP, GameUtil.GetFormattedCycles(num2, "F1")));
		}
		else
		{
			currentGermsPanel.SetLabel("finalhalflife", UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL_TOOLTIP);
		}
		currentGermsPanel.SetLabel("factors", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TITLE), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TOOLTIP);
		bool flag = false;
		if ((float)diseaseCount < growthRuleForElement.minCountPerKG * environmentMass)
		{
			currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TITLE, GetFormattedGrowthRate(growthRuleForElement.underPopulationDeathRate)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.minCountPerKG * environmentMass)), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.minCountPerKG));
			flag = true;
		}
		else if ((float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass)
		{
			currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TITLE, GetFormattedHalfLife(growthRuleForElement.overPopulationHalfLife)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.maxCountPerKG * environmentMass)), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.maxCountPerKG));
			flag = true;
		}
		if (!flag)
		{
			currentGermsPanel.SetLabel("substrate", GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
		}
		int num3 = 0;
		if (tags != null)
		{
			foreach (Tag tag in tags)
			{
				TagGrowthRule growthRuleForTag = disease.GetGrowthRuleForTag(tag);
				if (growthRuleForTag != null)
				{
					CollapsibleDetailContentPanel collapsibleDetailContentPanel = currentGermsPanel;
					string id = "tag_" + num3;
					string name = growthRuleForTag.Name();
					float? populationHalfLife = growthRuleForTag.populationHalfLife;
					string formattedGrowthEntry = GetFormattedGrowthEntry(name, populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL);
					string name2 = growthRuleForTag.Name();
					float? populationHalfLife2 = growthRuleForTag.populationHalfLife;
					collapsibleDetailContentPanel.SetLabel(id, formattedGrowthEntry, GetFormattedGrowthEntry(name2, populationHalfLife2.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
				}
				num3++;
			}
		}
		if (Grid.IsValidCell(environmentCell))
		{
			CompositeExposureRule exposureRuleForElement = disease.GetExposureRuleForElement(Grid.Element[environmentCell]);
			if (exposureRuleForElement != null && exposureRuleForElement.populationHalfLife != float.PositiveInfinity)
			{
				if (exposureRuleForElement.GetHalfLifeForCount(diseaseCount) > 0f)
				{
					currentGermsPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.DIE_TOOLTIP);
				}
				else
				{
					currentGermsPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.GROW_TOOLTIP);
				}
			}
		}
		float num4 = disease.CalculateTemperatureHalfLife(temperature);
		if (num4 != float.PositiveInfinity)
		{
			if (num4 > 0f)
			{
				currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GetFormattedHalfLife(num4)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.DIE_TOOLTIP);
			}
			else
			{
				currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GetFormattedHalfLife(num4)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.GROW_TOOLTIP);
			}
		}
	}

	private bool CreateDiseaseInfo_PrimaryElement()
	{
		if (!((Object)selectedTarget == (Object)null))
		{
			PrimaryElement component = selectedTarget.GetComponent<PrimaryElement>();
			if (!((Object)component == (Object)null))
			{
				if (component.DiseaseIdx != 255 && component.DiseaseCount > 0)
				{
					Disease disease = Db.Get().Diseases[component.DiseaseIdx];
					int environmentCell = Grid.PosToCell(component.transform.GetPosition());
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					BuildFactorsStrings(component.DiseaseCount, component.Element.idx, environmentCell, component.Mass, component.Temperature, component2.Tags, disease);
					return true;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	private bool CreateDiseaseInfo_CellSelectionObject(CellSelectionObject cso)
	{
		if (cso.diseaseIdx != 255 && cso.diseaseCount > 0)
		{
			Disease disease = Db.Get().Diseases[cso.diseaseIdx];
			int idx = cso.element.idx;
			BuildFactorsStrings(cso.diseaseCount, idx, -1, cso.Mass, cso.temperature, null, disease);
			return true;
		}
		return false;
	}
}
