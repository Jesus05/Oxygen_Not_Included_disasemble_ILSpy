using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GlassForgeConfig : IBuildingConfig
{
	public const string ID = "GlassForge";

	private const float INPUT_KG = 100f;

	public static readonly CellOffset outPipeOffset = new CellOffset(1, 3);

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "GlassForge";
		int width = 5;
		int height = 4;
		string anim = "glassrefinery_kanim";
		int hitpoints = 30;
		float construction_time = 60f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] aLL_MINERALS = MATERIALS.ALL_MINERALS;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_MINERALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = outPipeOffset;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GlassForge glassForge = go.AddOrGet<GlassForge>();
		glassForge.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		glassForge.duplicantOperated = true;
		BuildingTemplates.CreateComplexFabricatorStorage(go, glassForge);
		glassForge.outStorage.capacityKg = 2000f;
		glassForge.storeProduced = true;
		glassForge.inStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		glassForge.buildStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		glassForge.outStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		glassForge.outputOffset = new Vector3(1f, 0.5f);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_metalrefinery_kanim")
		};
		glassForge.resultState = ComplexFabricator.ResultState.Melted;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.storage = glassForge.outStorage;
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = null;
		conduitDispenser.alwaysDispense = true;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Sand).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.MoltenGlass).tag, 25f)
		};
		string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("GlassForge", array[0].material);
		string text = ComplexRecipeManager.MakeRecipeID("GlassForge", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text, array, array2);
		complexRecipe.time = 40f;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.GLASSFORGE.RECIPE_DESCRIPTION, ElementLoader.GetElement(array2[0].material).name, ElementLoader.GetElement(array[0].material).name);
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("GlassForge")
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}
}
