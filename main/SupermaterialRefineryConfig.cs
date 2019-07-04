using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SupermaterialRefineryConfig : IBuildingConfig
{
	public const string ID = "SupermaterialRefinery";

	private const float INPUT_KG = 100f;

	private const float OUTPUT_KG = 100f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "SupermaterialRefinery";
		int width = 4;
		int height = 5;
		string anim = "supermaterial_refinery_kanim";
		int hitpoints = 30;
		float construction_time = 480f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 1600f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_supermaterial_refinery_kanim")
		};
		Prioritizable.AddRef(go);
		float num = 0.01f;
		float num2 = (1f - num) * 0.5f;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[3]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f * num),
			new ComplexRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 100f * num2),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * num2)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SuperCoolant.CreateTag(), 100f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = 80f;
		complexRecipe.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("SupermaterialRefinery")
		};
		float num3 = 0.15f;
		float num4 = 0.05f;
		float num5 = 1f - num4 - num3;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[3]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num3),
			new ComplexRecipe.RecipeElement(SimHashes.Katairite.CreateTag(), 100f * num5),
			new ComplexRecipe.RecipeElement(BasicFabricConfig.ID.ToTag(), 100f * num4)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SuperInsulator.CreateTag(), 100f)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array3, array4);
		complexRecipe = new ComplexRecipe(id2, array3, array4);
		complexRecipe.time = 80f;
		complexRecipe.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERINSULATOR_RECIPE_DESCRIPTION;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("SupermaterialRefinery")
		};
		float num6 = 0.05f;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Niobium.CreateTag(), 100f * num6),
			new ComplexRecipe.RecipeElement(SimHashes.Tungsten.CreateTag(), 100f * (1f - num6))
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.TempConductorSolid.CreateTag(), 100f)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array5, array6);
		complexRecipe = new ComplexRecipe(id3, array5, array6);
		complexRecipe.time = 80f;
		complexRecipe.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.TEMPCONDUCTORSOLID_RECIPE_DESCRIPTION;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("SupermaterialRefinery")
		};
		float num7 = 0.35f;
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num7),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * (1f - num7))
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.ViscoGel.CreateTag(), 100f)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array7, array8);
		complexRecipe = new ComplexRecipe(id4, array7, array8);
		complexRecipe.time = 80f;
		complexRecipe.description = STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.VISCOGEL_RECIPE_DESCRIPTION;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("SupermaterialRefinery")
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}
}
