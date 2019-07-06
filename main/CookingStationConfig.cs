using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CookingStationConfig : IBuildingConfig
{
	public const string ID = "CookingStation";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "CookingStation";
		int width = 3;
		int height = 2;
		string anim = "cookstation_kanim";
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
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		CookingStation cookingStation = go.AddOrGet<CookingStation>();
		cookingStation.resultState = ComplexFabricator.ResultState.Heated;
		cookingStation.heatedTemperature = 368.15f;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_cookstation_kanim")
		};
		cookingStation.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		ConfigureRecipes();
		go.AddOrGetDef<PoweredController.Def>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, cookingStation);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicPlantFood", 3f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("PickledMeal", 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("CookingStation", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = FOOD.RECIPES.SMALL_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.PICKLEDMEAL.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"CookingStation"
		};
		complexRecipe.sortOrder = 21;
		PickledMealConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("MushBar", 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("FriedMushBar".ToTag(), 1f)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("CookingStation", array3, array4);
		complexRecipe = new ComplexRecipe(id2, array3, array4);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.FRIEDMUSHBAR.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"CookingStation"
		};
		complexRecipe.sortOrder = 1;
		FriedMushBarConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(MushroomConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("FriedMushroom", 1f)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("CookingStation", array5, array6);
		complexRecipe = new ComplexRecipe(id3, array5, array6);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.FRIEDMUSHROOM.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"CookingStation"
		};
		complexRecipe.sortOrder = 20;
		FriedMushroomConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(PrickleFruitConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("GrilledPrickleFruit", 1f)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("CookingStation", array7, array8);
		complexRecipe = new ComplexRecipe(id4, array7, array8);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.GRILLEDPRICKLEFRUIT.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"CookingStation"
		};
		complexRecipe.sortOrder = 20;
		GrilledPrickleFruitConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 3f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("ColdWheatBread", 1f)
		};
		string id5 = ComplexRecipeManager.MakeRecipeID("CookingStation", array9, array10);
		complexRecipe = new ComplexRecipe(id5, array9, array10);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.COLDWHEATBREAD.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"CookingStation"
		};
		complexRecipe.sortOrder = 50;
		ColdWheatBreadConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("RawEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("CookedEgg", 1f)
		};
		string id6 = ComplexRecipeManager.MakeRecipeID("CookingStation", array11, array12);
		complexRecipe = new ComplexRecipe(id6, array11, array12);
		complexRecipe.time = FOOD.RECIPES.STANDARD_COOK_TIME;
		complexRecipe.description = ITEMS.FOOD.COOKEDEGG.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"CookingStation"
		};
		complexRecipe.sortOrder = 1;
		CookedEggConfig.recipe = complexRecipe;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
