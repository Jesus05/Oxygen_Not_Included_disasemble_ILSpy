using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SuitFabricatorConfig : IBuildingConfig
{
	public const string ID = "SuitFabricator";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "SuitFabricator";
		int width = 4;
		int height = 3;
		string anim = "suit_maker_kanim";
		int hitpoints = 100;
		float construction_time = 240f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.resultState = ComplexFabricator.ResultState.Heated;
		complexFabricator.heatedTemperature = 318.15f;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_suit_fabricator_kanim")
		};
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		ConfigureRecipes();
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
		complexRecipe.fabricators = new List<Tag>
		{
			"SuitFabricator"
		};
		complexRecipe.requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id;
		AtmoSuitConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array3, array4);
		complexRecipe = new ComplexRecipe(id2, array3, array4);
		complexRecipe.time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
		complexRecipe.fabricators = new List<Tag>
		{
			"SuitFabricator"
		};
		complexRecipe.requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id;
		AtmoSuitConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array5, array6);
		complexRecipe = new ComplexRecipe(id3, array5, array6);
		complexRecipe.time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
		complexRecipe.fabricators = new List<Tag>
		{
			"SuitFabricator"
		};
		complexRecipe.requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id;
		AtmoSuitConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement((-899253461).ToString(), 200f),
			new ComplexRecipe.RecipeElement((-486269331).ToString(), 25f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Jet_Suit".ToTag(), 1f)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array7, array8);
		complexRecipe = new ComplexRecipe(id4, array7, array8);
		complexRecipe.time = (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
		complexRecipe.description = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
		complexRecipe.fabricators = new List<Tag>
		{
			"SuitFabricator"
		};
		complexRecipe.requiredTech = Db.Get().TechItems.jetSuit.parentTech.Id;
		JetSuitConfig.recipe = complexRecipe;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabInitFn += delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits, true);
		};
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
