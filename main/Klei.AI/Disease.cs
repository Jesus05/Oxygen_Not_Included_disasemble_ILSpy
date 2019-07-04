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

		private StringKey name;

		public HashedString id;

		public float strength;

		public RangeInfo temperatureRange;

		public RangeInfo temperatureHalfLives;

		public RangeInfo pressureRange;

		public RangeInfo pressureHalfLives;

		public List<GrowthRule> growthRules;

		public List<ExposureRule> exposureRules;

		public ElemGrowthInfo[] elemGrowthInfo;

		public ElemExposureInfo[] elemExposureInfo;

		public Color32 overlayColour = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		public string overlayLegendHovertext;

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

		public Disease(string id, byte strength, RangeInfo temperature_range, RangeInfo temperature_half_lives, RangeInfo pressure_range, RangeInfo pressure_half_lives)
			: base(id, null, null)
		{
			name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.id = id;
			DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(id);
			overlayColour = info.overlayColour;
			temperatureRange = temperature_range;
			temperatureHalfLives = temperature_half_lives;
			pressureRange = pressure_range;
			pressureHalfLives = pressure_half_lives;
			PopulateElemGrowthInfo();
			ApplyRules();
			string str = Strings.Get("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".LEGEND_HOVERTEXT").ToString();
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
				Debug.Assert(g.GetType() == typeof(GrowthRule), "First rule must be a fully defined base rule.");
				float? underPopulationDeathRate = g.underPopulationDeathRate;
				Debug.Assert(underPopulationDeathRate.HasValue, "First rule must be a fully defined base rule.");
				float? populationHalfLife = g.populationHalfLife;
				Debug.Assert(populationHalfLife.HasValue, "First rule must be a fully defined base rule.");
				float? overPopulationHalfLife = g.overPopulationHalfLife;
				Debug.Assert(overPopulationHalfLife.HasValue, "First rule must be a fully defined base rule.");
				float? diffusionScale = g.diffusionScale;
				Debug.Assert(diffusionScale.HasValue, "First rule must be a fully defined base rule.");
				float? minCountPerKG = g.minCountPerKG;
				Debug.Assert(minCountPerKG.HasValue, "First rule must be a fully defined base rule.");
				float? maxCountPerKG = g.maxCountPerKG;
				Debug.Assert(maxCountPerKG.HasValue, "First rule must be a fully defined base rule.");
				int? minDiffusionCount = g.minDiffusionCount;
				Debug.Assert(minDiffusionCount.HasValue, "First rule must be a fully defined base rule.");
				byte? minDiffusionInfestationTickCount = g.minDiffusionInfestationTickCount;
				Debug.Assert(minDiffusionInfestationTickCount.HasValue, "First rule must be a fully defined base rule.");
			}
			else
			{
				Debug.Assert(g.GetType() != typeof(GrowthRule), "Subsequent rules should not be base rules");
			}
			growthRules.Add(g);
		}

		protected void AddExposureRule(ExposureRule g)
		{
			if (exposureRules == null)
			{
				exposureRules = new List<ExposureRule>();
				Debug.Assert(g.GetType() == typeof(ExposureRule), "First rule must be a fully defined base rule.");
				float? populationHalfLife = g.populationHalfLife;
				Debug.Assert(populationHalfLife.HasValue, "First rule must be a fully defined base rule.");
			}
			else
			{
				Debug.Assert(g.GetType() != typeof(ExposureRule), "Subsequent rules should not be base rules");
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

		public List<Descriptor> GetQuantitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE, GameUtil.GetFormattedTemperature(temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE_TOOLTIP, GameUtil.GetFormattedTemperature(temperatureRange.minViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureRange.maxViable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureRange.minGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureRange.maxGrowth, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Information, false));
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
