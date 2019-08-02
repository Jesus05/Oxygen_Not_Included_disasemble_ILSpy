using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class PollenGerms : Disease
	{
		public const string ID = "PollenGerms";

		public PollenGerms()
			: base("PollenGerms", 5, new RangeInfo(263.15f, 273.15f, 363.15f, 373.15f), new RangeInfo(10f, 100f, 100f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(0.6666667f),
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(3000f),
				maxCountPerKG = new float?(500f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(3000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				diffusionScale = new float?(1E-06f),
				minDiffusionCount = new int?(1000000)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = new float?(500f),
				underPopulationDeathRate = new float?(2.66666675f),
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				maxCountPerKG = new float?(1000000f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.015f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(200f),
				overPopulationHalfLife = new float?(10f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				maxCountPerKG = new float?(100f),
				diffusionScale = new float?(0.01f)
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = new float?(1200f)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(float.PositiveInfinity)
			});
		}
	}
}
