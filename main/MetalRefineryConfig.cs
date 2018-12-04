using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MetalRefineryConfig : IBuildingConfig
{
	public const string ID = "MetalRefinery";

	private const float INPUT_KG = 100f;

	private const float LIQUID_COOLED_HEAT_PORTION = 0.8f;

	private static readonly Tag COOLANT_TAG = GameTags.Liquid;

	private const float COOLANT_MASS = 400f;

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MetalRefinery";
		int width = 3;
		int height = 4;
		string anim = "metalrefinery_kanim";
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
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		LiquidCooledRefinery liquidCooledRefinery = go.AddOrGet<LiquidCooledRefinery>();
		liquidCooledRefinery.duplicantOperated = true;
		liquidCooledRefinery.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, liquidCooledRefinery);
		liquidCooledRefinery.coolantTag = COOLANT_TAG;
		liquidCooledRefinery.minCoolantMass = 400f;
		liquidCooledRefinery.outStorage.capacityKg = 2000f;
		liquidCooledRefinery.thermalFudge = 0.8f;
		liquidCooledRefinery.inStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		liquidCooledRefinery.buildStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		liquidCooledRefinery.outStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		liquidCooledRefinery.outputOffset = new Vector3(1f, 0.5f);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_metalrefinery_kanim")
		};
		RequireOutputs requireOutputs = go.AddOrGet<RequireOutputs>();
		requireOutputs.ignoreFullPipe = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.capacityKG = 800f;
		conduitConsumer.storage = liquidCooledRefinery.inStorage;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.forceAlwaysSatisfied = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.storage = liquidCooledRefinery.outStorage;
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = null;
		conduitDispenser.alwaysDispense = true;
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal));
		ComplexRecipe complexRecipe;
		foreach (Element item in list)
		{
			Element highTempTransition = item.highTempTransition;
			Element lowTempTransition = highTempTransition.lowTempTransition;
			if (lowTempTransition != item)
			{
				ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
				{
					new ComplexRecipe.RecipeElement(item.tag, 100f)
				};
				ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
				{
					new ComplexRecipe.RecipeElement(lowTempTransition.tag, 100f)
				};
				string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("MetalRefinery", item.tag);
				string text = ComplexRecipeManager.MakeRecipeID("MetalRefinery", array, array2);
				complexRecipe = new ComplexRecipe(text, array, array2);
				complexRecipe.time = 40f;
				complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, lowTempTransition.name, item.name);
				complexRecipe.useResultAsDescription = true;
				complexRecipe.fabricators = new List<Tag>
				{
					TagManager.Create("MetalRefinery")
				};
				ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
			}
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Steel);
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[3]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Iron).tag, 70f),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.RefinedCarbon).tag, 20f),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 10f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Steel).tag, 100f)
		};
		string obsolete_id2 = ComplexRecipeManager.MakeObsoleteRecipeID("MetalRefinery", element.tag);
		string text2 = ComplexRecipeManager.MakeRecipeID("MetalRefinery", array3, array4);
		complexRecipe = new ComplexRecipe(text2, array3, array4);
		complexRecipe.time = 40f;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Steel).name, ElementLoader.FindElementByHash(SimHashes.Iron).name);
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("MetalRefinery")
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id2, text2);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
	}
}
