using System.Collections.Generic;

namespace TUNING
{
	public class CROPS
	{
		public const float WILD_GROWTH_RATE_MODIFIER = 0.25f;

		public const float GROWTH_RATE = 0.00166666671f;

		public const float WILD_GROWTH_RATE = 0.000416666677f;

		public const float PLANTERPLOT_GROWTH_PENTALY = -0.5f;

		public const float BASE_BONUS_SEED_PROBABILITY = 0.1f;

		public const float SELF_HARVEST_TIME = 2400f;

		public const float SELF_PLANT_TIME = 2400f;

		public const float TREE_BRANCH_SELF_HARVEST_TIME = 12000f;

		public const float FERTILIZATION_GAIN_RATE = 1.66666663f;

		public const float FERTILIZATION_LOSS_RATE = -0.166666672f;

		public static List<Crop.CropVal> CROP_TYPES = new List<Crop.CropVal>
		{
			new Crop.CropVal("BasicPlantFood", 1800f, 1, true),
			new Crop.CropVal(PrickleFruitConfig.ID, 3600f, 1, true),
			new Crop.CropVal(MushroomConfig.ID, 4500f, 1, true),
			new Crop.CropVal("ColdWheatSeed", 10800f, 18, true),
			new Crop.CropVal(SpiceNutConfig.ID, 4800f, 4, true),
			new Crop.CropVal(BasicFabricConfig.ID, 1200f, 1, true),
			new Crop.CropVal(SwampLilyFlowerConfig.ID, 7200f, 2, true),
			new Crop.CropVal("GasGrassHarvested", 2400f, 1, true),
			new Crop.CropVal("WoodLog", 2700f, 300, true),
			new Crop.CropVal("Lettuce", 7200f, 12, true),
			new Crop.CropVal("BeanPlantSeed", 12600f, 12, true),
			new Crop.CropVal("OxyfernSeed", 7200f, 1, true),
			new Crop.CropVal(381665462.ToString(), 3600f, 65, true)
		};
	}
}
