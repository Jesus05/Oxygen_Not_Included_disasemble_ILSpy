using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ClothingFabricatorConfig : IBuildingConfig
{
	public const string ID = "ClothingFabricator";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "ClothingFabricator";
		int width = 4;
		int height = 3;
		string anim = "clothingfactory_kanim";
		int hitpoints = 100;
		float construction_time = 240f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(2, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_clothingfactory_kanim")
		};
		go.AddOrGet<ComplexFabricatorWorkable>().AnimOffset = new Vector3(-1f, 0f, 0f);
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ConfigureRecipes();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), (float)TUNING.EQUIPMENT.VESTS.WARM_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Warm_Vest".ToTag(), 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = TUNING.EQUIPMENT.VESTS.WARM_VEST_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.WARM_VEST.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"ClothingFabricator"
		};
		complexRecipe.sortOrder = 1;
		WarmVestConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), (float)TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Cool_Vest".ToTag(), 1f)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array3, array4);
		complexRecipe = new ComplexRecipe(id2, array3, array4);
		complexRecipe.time = TUNING.EQUIPMENT.VESTS.COOL_VEST_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.COOL_VEST.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"ClothingFabricator"
		};
		complexRecipe.sortOrder = 1;
		CoolVestConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), (float)TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Funky_Vest".ToTag(), 1f)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array5, array6);
		complexRecipe = new ComplexRecipe(id3, array5, array6);
		complexRecipe.time = TUNING.EQUIPMENT.VESTS.FUNKY_VEST_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"ClothingFabricator"
		};
		complexRecipe.sortOrder = 1;
		FunkyVestConfig.recipe = complexRecipe;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}
}
