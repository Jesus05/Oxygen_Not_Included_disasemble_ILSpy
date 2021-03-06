using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GourmetCookingStationConfig : IBuildingConfig
{
	public const string ID = "GourmetCookingStation";

	private const float FUEL_STORE_CAPACITY = 10f;

	private const float FUEL_CONSUME_RATE = 0.1f;

	private const float CO2_EMIT_RATE = 0.025f;

	private Tag FUEL_TAG = new Tag("Methane");

	private static readonly List<Storage.StoredItemModifier> GourmetCookingStationStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "GourmetCookingStation";
		int width = 3;
		int height = 3;
		string anim = "cookstation_gourmet_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		buildingDef.SelfHeatKilowattsWhenActive = 8f;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GourmetCookingStation gourmetCookingStation = go.AddOrGet<GourmetCookingStation>();
		gourmetCookingStation.resultState = ComplexFabricator.ResultState.Heated;
		gourmetCookingStation.heatedTemperature = 368.15f;
		gourmetCookingStation.duplicantOperated = true;
		gourmetCookingStation.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, gourmetCookingStation);
		gourmetCookingStation.fuelTag = FUEL_TAG;
		gourmetCookingStation.outStorage.capacityKg = 10f;
		gourmetCookingStation.inStorage.SetDefaultStoredItemModifiers(GourmetCookingStationStoredItemModifiers);
		gourmetCookingStation.buildStorage.SetDefaultStoredItemModifiers(GourmetCookingStationStoredItemModifiers);
		gourmetCookingStation.outStorage.SetDefaultStoredItemModifiers(GourmetCookingStationStoredItemModifiers);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.capacityTag = FUEL_TAG;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.storage = gourmetCookingStation.inStorage;
		conduitConsumer.forceAlwaysSatisfied = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(FUEL_TAG, 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.025f, SimHashes.CarbonDioxide, 348.15f, false, false, 0f, 3f, 1f, byte.MaxValue, 0)
		};
		ConfigureRecipes();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("GrilledPrickleFruit", 2f),
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 2f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Salsa", 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.SALSA.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"GourmetCookingStation"
		};
		complexRecipe.sortOrder = 300;
		SalsaConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("FriedMushroom", 1f),
			new ComplexRecipe.RecipeElement("Lettuce", 4f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("MushroomWrap", 1f)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array3, array4);
		complexRecipe = new ComplexRecipe(id2, array3, array4);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.MUSHROOMWRAP.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"GourmetCookingStation"
		};
		complexRecipe.sortOrder = 400;
		MushroomWrapConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("CookedMeat", 1f),
			new ComplexRecipe.RecipeElement("CookedFish", 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SurfAndTurf", 1f)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array5, array6);
		complexRecipe = new ComplexRecipe(id3, array5, array6);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.SURFANDTURF.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"GourmetCookingStation"
		};
		complexRecipe.sortOrder = 500;
		SurfAndTurfConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 10f),
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SpiceBread", 1f)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array7, array8);
		complexRecipe = new ComplexRecipe(id4, array7, array8);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.SPICEBREAD.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"GourmetCookingStation"
		};
		complexRecipe.sortOrder = 600;
		SpiceBreadConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Tofu", 1f),
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SpicyTofu", 1f)
		};
		string id5 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array9, array10);
		complexRecipe = new ComplexRecipe(id5, array9, array10);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.SPICYTOFU.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"GourmetCookingStation"
		};
		complexRecipe.sortOrder = 800;
		SpicyTofuConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[3]
		{
			new ComplexRecipe.RecipeElement("ColdWheatBread", 1f),
			new ComplexRecipe.RecipeElement("Lettuce", 1f),
			new ComplexRecipe.RecipeElement("CookedMeat", 1f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Burger", 1f)
		};
		string id6 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array11, array12);
		complexRecipe = new ComplexRecipe(id6, array11, array12);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.BURGER.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"GourmetCookingStation"
		};
		complexRecipe.sortOrder = 900;
		BurgerConfig.recipe = complexRecipe;
	}
}
