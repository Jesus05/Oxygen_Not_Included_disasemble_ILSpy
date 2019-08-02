using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class ZombieSpores : Disease
	{
		public const string ID = "ZombieSpores";

		public ZombieSpores()
			: base("ZombieSpores", 50, new RangeInfo(168.15f, 258.15f, 513.15f, 563.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.66666675f),
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(500f),
				overPopulationHalfLife = new float?(1200f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(3000f),
				overPopulationHalfLife = new float?(1200f),
				diffusionScale = new float?(1E-06f),
				minDiffusionCount = new int?(1000000)
			});
			SimHashes[] array = new SimHashes[2]
			{
				SimHashes.Carbon,
				SimHashes.Diamond
			};
			foreach (SimHashes element in array)
			{
				AddGrowthRule(new ElementGrowthRule(element)
				{
					underPopulationDeathRate = new float?(0f),
					populationHalfLife = new float?(float.PositiveInfinity),
					overPopulationHalfLife = new float?(3000f),
					maxCountPerKG = new float?(1000f),
					diffusionScale = new float?(0.005f)
				});
			}
			AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = new float?(250f),
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(1200f),
				maxCountPerKG = new float?(10000f),
				minDiffusionCount = new int?(5100),
				diffusionScale = new float?(0.005f)
			});
			SimHashes[] array2 = new SimHashes[3]
			{
				SimHashes.CarbonDioxide,
				SimHashes.Methane,
				SimHashes.SourGas
			};
			foreach (SimHashes element2 in array2)
			{
				AddGrowthRule(new ElementGrowthRule(element2)
				{
					underPopulationDeathRate = new float?(0f),
					populationHalfLife = new float?(float.PositiveInfinity),
					overPopulationHalfLife = new float?(6000f)
				});
			}
			AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(300f),
				maxCountPerKG = new float?(100f),
				diffusionScale = new float?(0.01f)
			});
			SimHashes[] array3 = new SimHashes[4]
			{
				SimHashes.CrudeOil,
				SimHashes.Petroleum,
				SimHashes.Naphtha,
				SimHashes.LiquidMethane
			};
			foreach (SimHashes element3 in array3)
			{
				AddGrowthRule(new ElementGrowthRule(element3)
				{
					populationHalfLife = new float?(float.PositiveInfinity),
					overPopulationHalfLife = new float?(6000f),
					maxCountPerKG = new float?(1000f),
					diffusionScale = new float?(0.005f)
				});
			}
			AddGrowthRule(new ElementGrowthRule(SimHashes.Chlorine)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = new float?(float.PositiveInfinity)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.Chlorine)
			{
				populationHalfLife = new float?(10f)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}
	}
}
