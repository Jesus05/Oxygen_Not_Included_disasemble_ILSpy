using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class KilnConfig : IBuildingConfig
{
	public const string ID = "Kiln";

	public const float INPUT_CLAY_PER_SECOND = 1f;

	public const float CERAMIC_PER_SECOND = 1f;

	public const float CO2_RATIO = 0.1f;

	public const float OUTPUT_TEMP = 303.15f;

	public const float REFILL_RATE = 2400f;

	public const float CERAMIC_STORAGE_AMOUNT = 2400f;

	public const float COAL_RATE = 0.1f;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Kiln";
		int width = 2;
		int height = 2;
		string anim = "kiln_kanim";
		int hitpoints = 100;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tIER2, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = false;
		buildingDef.ExhaustKilowattsWhenActive = 16f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.AudioCategory = "HollowMetal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.duplicantOperated = false;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		ConfgiureRecipes();
		Prioritizable.AddRef(go);
	}

	private void ConfgiureRecipes()
	{
		Tag tag = SimHashes.Ceramic.CreateTag();
		Tag material = SimHashes.Clay.CreateTag();
		Tag material2 = SimHashes.Carbon.CreateTag();
		float num = 100f;
		float num2 = 25f;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(material, num),
			new ComplexRecipe.RecipeElement(material2, num2)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(tag, num)
		};
		string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("Kiln", tag);
		string text = ComplexRecipeManager.MakeRecipeID("Kiln", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text, array, array2);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Clay).name, ElementLoader.FindElementByHash(SimHashes.Ceramic).name);
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("Kiln")
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		Tag tag2 = SimHashes.RefinedCarbon.CreateTag();
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(material2, num + num2)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(tag2, num)
		};
		string obsolete_id2 = ComplexRecipeManager.MakeObsoleteRecipeID("Kiln", tag2);
		string text2 = ComplexRecipeManager.MakeRecipeID("Kiln", array3, array4);
		complexRecipe = new ComplexRecipe(text2, array3, array4);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Carbon).name, ElementLoader.FindElementByHash(SimHashes.RefinedCarbon).name);
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("Kiln")
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id2, text2);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}
}
