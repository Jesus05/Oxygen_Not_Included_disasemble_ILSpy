using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public abstract class Disease : Resource
	{
		public struct RangeInfo
		{
			public float minViable;

			public float minGrowth;

			public float maxGrowth;

			public float maxViable;

			public RangeInfo(float min_viable, float min_growth, float max_growth, float max_viable)
			{
				minViable = min_viable;
				minGrowth = min_growth;
				maxGrowth = max_growth;
				maxViable = max_viable;
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(minViable);
				writer.Write(minGrowth);
				writer.Write(maxGrowth);
				writer.Write(maxViable);
			}

			public float GetValue(int idx)
			{
				switch (idx)
				{
				case 0:
					return minViable;
				case 1:
					return minGrowth;
				case 2:
					return maxGrowth;
				case 3:
					return maxViable;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			public static RangeInfo Idempotent()
			{
				return new RangeInfo(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			}
		}

		public abstract class DiseaseComponent
		{
			public abstract object OnInfect(GameObject go, DiseaseInstance diseaseInstance);

			public abstract void OnCure(GameObject go, object instance_data);

			public virtual List<Descriptor> GetSymptoms()
			{
				return null;
			}
		}

		public enum InfectionVector
		{
			Contact,
			Digestion,
			Inhalation,
			Exposure
		}

		public enum DiseaseType
		{
			Pathogen,
			Ailment,
			Injury
		}

		public enum Severity
		{
			Benign,
			Minor,
			Major,
			Critical
		}

		private StringKey name;

		private StringKey descriptiveSymptoms;

		private float sicknessDuration = 600f;

		public bool doctorRequired = false;

		public float fatalityDuration = 0f;

		public HashedString id;

		public DiseaseType diseaseType;

		public Severity severity;

		public float strength;

		public float immuneAttackStrength;

		public RangeInfo temperatureRange;

		public RangeInfo temperatureHalfLives;

		public RangeInfo pressureRange;

		public RangeInfo pressureHalfLives;

		public List<GrowthRule> growthRules;

		public List<ExposureRule> exposureRules;

		public ElemGrowthInfo[] elemGrowthInfo;

		public ElemExposureInfo[] elemExposureInfo;

		public List<InfectionVector> infectionVectors;

		public Color32 overlayColour = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		public string overlayLegendHovertext;

		private List<DiseaseComponent> components = new List<DiseaseComponent>();

		public Amount amount;

		public Attribute amountDeltaAttribute;

		public Attribute cureSpeedBase;

		public static readonly ElemGrowthInfo DEFAULT_GROWTH_INFO = new ElemGrowthInfo
		{
			underPopulationDeathRate = 0f,
			populationHalfLife = float.PositiveInfinity,
			overPopulationHalfLife = float.PositiveInfinity,
			minCountPerKG = 0f,
			maxCountPerKG = float.PositiveInfinity,
			minDiffusionCount = 0,
			diffusionScale = 1f,
			minDiffusionInfestationTickCount = 255
		};

		public static ElemExposureInfo DEFAULT_EXPOSURE_INFO = new ElemExposureInfo
		{
			populationHalfLife = float.PositiveInfinity
		};

		public new string Name => Strings.Get(name);

		public float SicknessDuration => sicknessDuration;

		public Disease(string id, DiseaseType type, Severity severity, float immune_attack_strength, List<InfectionVector> infection_vectors, float sickness_duration, byte strength, RangeInfo temperature_range, RangeInfo temperature_half_lives, RangeInfo pressure_range, RangeInfo pressure_half_lives)
			: base(id, null, null)
		{
			name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.id = id;
			diseaseType = type;
			this.severity = severity;
			immuneAttackStrength = immune_attack_strength;
			infectionVectors = infection_vectors;
			sicknessDuration = sickness_duration;
			DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(id);
			overlayColour = info.overlayColour;
			temperatureRange = temperature_range;
			temperatureHalfLives = temperature_half_lives;
			pressureRange = pressure_range;
			pressureHalfLives = pressure_half_lives;
			descriptiveSymptoms = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".DESCRIPTIVE_SYMPTOMS");
			PopulateElemGrowthInfo();
			ApplyRules();
			string str = Strings.Get("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".LEGEND_HOVERTEXT").ToString();
			foreach (Descriptor qualitativeDescriptor in GetQualitativeDescriptors())
			{
				str = str + "" + qualitativeDescriptor.IndentedText() + "\n";
			}
			overlayLegendHovertext = str + DUPLICANTS.DISEASES.LEGEND_POSTAMBLE;
			Attribute attribute = new Attribute(id + "Min", "Minimum" + id.ToString(), "", "", 0f, Attribute.Display.Normal, false, null, null);
			Attribute attribute2 = new Attribute(id + "Max", "Maximum" + id.ToString(), "", "", 1E+07f, Attribute.Display.Normal, false, null, null);
			amountDeltaAttribute = new Attribute(id + "Delta", id.ToString(), "", "", 0f, Attribute.Display.Normal, false, null, null);
			amount = new Amount(id, id + " " + DUPLICANTS.DISEASES.GERMS, id + " " + DUPLICANTS.DISEASES.GERMS, attribute, attribute2, amountDeltaAttribute, false, Units.Flat, 0.01f, true, null, null);
			Db.Get().Attributes.Add(attribute);
			Db.Get().Attributes.Add(attribute2);
			Db.Get().Attributes.Add(amountDeltaAttribute);
			cureSpeedBase = new Attribute(id + "CureSpeed", false, Attribute.Display.Normal, false, 0f, null, null);
			cureSpeedBase.BaseValue = 1f;
			cureSpeedBase.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			Db.Get().Attributes.Add(cureSpeedBase);
		}

		protected virtual void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(0f),
				minCountPerKG = new float?(100f),
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCountPerKG = new float?(1000f),
				overPopulationHalfLife = new float?(float.PositiveInfinity),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			InitializeElemExposureArray(ref elemExposureInfo, DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = new float?(float.PositiveInfinity)
			});
		}

		protected void AddGrowthRule(GrowthRule g)
		{
			if (growthRules == null)
			{
				growthRules = new List<GrowthRule>();
			}
			growthRules.Add(g);
		}

		protected void AddExposureRule(ExposureRule g)
		{
			if (exposureRules == null)
			{
				exposureRules = new List<ExposureRule>();
			}
			exposureRules.Add(g);
		}

		public CompositeGrowthRule GetGrowthRuleForElement(Element e)
		{
			CompositeGrowthRule compositeGrowthRule = new CompositeGrowthRule();
			if (growthRules != null)
			{
				for (int i = 0; i < growthRules.Count; i++)
				{
					if (growthRules[i].Test(e))
					{
						compositeGrowthRule.Overlay(growthRules[i]);
					}
				}
			}
			return compositeGrowthRule;
		}

		public CompositeExposureRule GetExposureRuleForElement(Element e)
		{
			CompositeExposureRule compositeExposureRule = new CompositeExposureRule();
			if (exposureRules != null)
			{
				for (int i = 0; i < exposureRules.Count; i++)
				{
					if (exposureRules[i].Test(e))
					{
						compositeExposureRule.Overlay(exposureRules[i]);
					}
				}
			}
			return compositeExposureRule;
		}

		public TagGrowthRule GetGrowthRuleForTag(Tag t)
		{
			if (growthRules != null)
			{
				for (int i = 0; i < growthRules.Count; i++)
				{
					TagGrowthRule tagGrowthRule = growthRules[i] as TagGrowthRule;
					if (tagGrowthRule != null && tagGrowthRule.tag == t)
					{
						return tagGrowthRule;
					}
				}
			}
			return null;
		}

		protected void ApplyRules()
		{
			if (growthRules != null)
			{
				for (int i = 0; i < growthRules.Count; i++)
				{
					growthRules[i].Apply(elemGrowthInfo);
				}
			}
			if (exposureRules != null)
			{
				for (int j = 0; j < exposureRules.Count; j++)
				{
					exposureRules[j].Apply(elemExposureInfo);
				}
			}
		}

		protected void InitializeElemGrowthArray(ref ElemGrowthInfo[] infoArray, ElemGrowthInfo default_value)
		{
			List<Element> elements = ElementLoader.elements;
			infoArray = new ElemGrowthInfo[elements.Count];
			for (int i = 0; i < elements.Count; i++)
			{
				infoArray[i] = default_value;
			}
			infoArray[ElementLoader.GetElementIndex(SimHashes.Polypropylene)] = new ElemGrowthInfo
			{
				underPopulationDeathRate = 2.66666675f,
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minCountPerKG = 0f,
				maxCountPerKG = float.PositiveInfinity,
				minDiffusionCount = 2147483647,
				diffusionScale = 1f,
				minDiffusionInfestationTickCount = 255
			};
			infoArray[ElementLoader.GetElementIndex(SimHashes.Vacuum)] = new ElemGrowthInfo
			{
				underPopulationDeathRate = 0f,
				populationHalfLife = 0f,
				overPopulationHalfLife = 0f,
				minCountPerKG = 0f,
				maxCountPerKG = float.PositiveInfinity,
				diffusionScale = 0f,
				minDiffusionInfestationTickCount = 255
			};
		}

		protected void InitializeElemExposureArray(ref ElemExposureInfo[] infoArray, ElemExposureInfo default_value)
		{
			List<Element> elements = ElementLoader.elements;
			infoArray = new ElemExposureInfo[elements.Count];
			for (int i = 0; i < elements.Count; i++)
			{
				infoArray[i] = default_value;
			}
		}

		public float GetGrowthRateForTags(HashSet<Tag> tags, bool overpopulated)
		{
			float num = 1f;
			if (growthRules != null)
			{
				for (int i = 0; i < growthRules.Count; i++)
				{
					TagGrowthRule tagGrowthRule = growthRules[i] as TagGrowthRule;
					if (tagGrowthRule != null && tags.Contains(tagGrowthRule.tag))
					{
						num *= HalfLifeToGrowthRate(((!overpopulated) ? tagGrowthRule.populationHalfLife : tagGrowthRule.overPopulationHalfLife).Value, 1f);
					}
				}
			}
			return num;
		}

		public object[] Infect(GameObject go, DiseaseInstance diseaseInstance, DiseaseExposureInfo exposure_info)
		{
			object[] array = new object[components.Count];
			for (int i = 0; i < components.Count; i++)
			{
				array[i] = components[i].OnInfect(go, diseaseInstance);
			}
			return array;
		}

		public void Cure(GameObject go, object[] componentData)
		{
			for (int i = 0; i < components.Count; i++)
			{
				components[i].OnCure(go, componentData[i]);
			}
		}

		public List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			for (int i = 0; i < components.Count; i++)
			{
				List<Descriptor> symptoms = components[i].GetSymptoms();
				if (symptoms != null)
				{
					list.AddRange(symptoms);
				}
			}
			if (fatalityDuration > 0f)
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM, GameUtil.GetFormattedCycles(fatalityDuration, "F1")), string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM_TOOLTIP, GameUtil.GetFormattedCycles(fatalityDuration, "F1")), Descriptor.DescriptorType.SymptomAidable, false));
			}
			return list;
		}

		public static float HalfLifeToGrowthRate(float half_life_in_seconds, float dt)
		{
			float num = 1f;
			if (half_life_in_seconds != 0f)
			{
				if (half_life_in_seconds != float.PositiveInfinity)
				{
					float num2 = half_life_in_seconds / dt;
					return Mathf.Pow(2f, -1f / num2);
				}
				return 1f;
			}
			return 0f;
		}

		public static float GrowthRateToHalfLife(float growth_rate)
		{
			float num = 1f;
			if (growth_rate != 0f)
			{
				if (growth_rate != 1f)
				{
					return Mathf.Log(2f, growth_rate);
				}
				return float.PositiveInfinity;
			}
			return 0f;
		}

		public float CalculateTemperatureHalfLife(float temperature)
		{
			return CalculateRangeHalfLife(temperature, ref temperatureRange, ref temperatureHalfLives);
		}

		public static float CalculateRangeHalfLife(float range_value, ref RangeInfo range, ref RangeInfo half_lives)
		{
			int num = 3;
			int num2 = 3;
			for (int i = 0; i < 4; i++)
			{
				if (range_value <= range.GetValue(i))
				{
					num = i - 1;
					num2 = i;
					break;
				}
			}
			if (num < 0)
			{
				num = num2;
			}
			float value = half_lives.GetValue(num);
			float value2 = half_lives.GetValue(num2);
			if (num == 1 && num2 == 2)
			{
				return float.PositiveInfinity;
			}
			if (!float.IsInfinity(value) && !float.IsInfinity(value2))
			{
				float value3 = range.GetValue(num);
				float value4 = range.GetValue(num2);
				float t = 0f;
				float num3 = value4 - value3;
				if (num3 > 0f)
				{
					t = (range_value - value3) / num3;
				}
				return Mathf.Lerp(value, value2, t);
			}
			return float.PositiveInfinity;
		}

		protected void AddDiseaseComponent(DiseaseComponent cmp)
		{
			components.Add(cmp);
		}

		public T GetDiseaseComponent<T>() where T : DiseaseComponent
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i] is T)
				{
					return components[i] as T;
				}
			}
			return (T)null;
		}

		public virtual List<Descriptor> GetDiseaseSourceDescriptors()
		{
			return new List<Descriptor>();
		}

		public List<Descriptor> GetQualitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			using (List<InfectionVector>.Enumerator enumerator = infectionVectors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case InfectionVector.Contact:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case InfectionVector.Inhalation:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case InfectionVector.Digestion:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case InfectionVector.Exposure:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					}
				}
			}
			list.Add(new Descriptor(Strings.Get(descriptiveSymptoms), "", Descriptor.DescriptorType.Information, false));
			return list;
		}

		public List<Descriptor> GetQuantitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE, GameUtil.GetFormattedTemperature(temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE_TOOLTIP, GameUtil.GetFormattedTemperature(temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), Descriptor.DescriptorType.Information, false));
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE, GameUtil.GetFormattedMass(pressureRange.minViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(pressureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE_TOOLTIP, GameUtil.GetFormattedMass(pressureRange.minViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(pressureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(pressureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(pressureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Information, false));
			List<GrowthRule> list2 = new List<GrowthRule>();
			List<GrowthRule> list3 = new List<GrowthRule>();
			List<GrowthRule> list4 = new List<GrowthRule>();
			List<GrowthRule> list5 = new List<GrowthRule>();
			List<GrowthRule> list6 = new List<GrowthRule>();
			foreach (GrowthRule growthRule in growthRules)
			{
				float? populationHalfLife = growthRule.populationHalfLife;
				if (populationHalfLife.HasValue && growthRule.Name() != null)
				{
					float? populationHalfLife2 = growthRule.populationHalfLife;
					if (populationHalfLife2.Value < 0f)
					{
						list2.Add(growthRule);
					}
					else
					{
						float? populationHalfLife3 = growthRule.populationHalfLife;
						if (populationHalfLife3.Value == float.PositiveInfinity)
						{
							list3.Add(growthRule);
						}
						else
						{
							float? populationHalfLife4 = growthRule.populationHalfLife;
							if (populationHalfLife4.Value >= 12000f)
							{
								list4.Add(growthRule);
							}
							else
							{
								float? populationHalfLife5 = growthRule.populationHalfLife;
								if (populationHalfLife5.Value >= 1200f)
								{
									list5.Add(growthRule);
								}
								else
								{
									list6.Add(growthRule);
								}
							}
						}
					}
				}
			}
			list.AddRange(BuildGrowthInfoDescriptors(list2, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_TOOLTIP));
			list.AddRange(BuildGrowthInfoDescriptors(list3, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_TOOLTIP));
			list.AddRange(BuildGrowthInfoDescriptors(list4, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_TOOLTIP));
			list.AddRange(BuildGrowthInfoDescriptors(list5, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_TOOLTIP));
			list.AddRange(BuildGrowthInfoDescriptors(list6, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_ON, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_ON_TOOLTIP, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_TOOLTIP));
			return list;
		}

		private List<Descriptor> BuildGrowthInfoDescriptors(List<GrowthRule> rules, string section_text, string section_tooltip, string item_tooltip)
		{
			List<Descriptor> list = new List<Descriptor>();
			if (rules.Count > 0)
			{
				list.Add(new Descriptor(section_text, section_tooltip, Descriptor.DescriptorType.Information, false));
				for (int i = 0; i < rules.Count; i++)
				{
					List<Descriptor> list2 = list;
					string txt = string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWTH_FORMAT, rules[i].Name());
					float? populationHalfLife = rules[i].populationHalfLife;
					list2.Add(new Descriptor(txt, string.Format(item_tooltip, GameUtil.GetFormattedCycles(Mathf.Abs(populationHalfLife.Value), "F1")), Descriptor.DescriptorType.Information, false));
				}
			}
			return list;
		}
	}
}
