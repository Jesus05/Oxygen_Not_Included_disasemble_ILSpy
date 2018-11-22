using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ApothecaryConfig : IBuildingConfig
{
	public const string ID = "Apothecary";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Apothecary";
		int width = 2;
		int height = 3;
		string anim = "apothecary_kanim";
		int hitpoints = 30;
		float construction_time = 120f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_apothecary_kanim")
		};
		go.AddOrGet<ComplexFabricatorWorkable>().AnimOffset = new Vector3(-1f, 0f, 0f);
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Water".ToTag(), 100f),
			new ComplexRecipe.RecipeElement("Sand".ToTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("GenericPill".ToTag(), 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = 40f;
		complexRecipe.description = ITEMS.PILLS.PLACEBO.RECIPEDESC;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag>
		{
			"Apothecary"
		};
		GenericPillConfig.recipe = complexRecipe;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Carbon".ToTag(), 100f),
			new ComplexRecipe.RecipeElement(SwampLilyFlowerConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("VitaminSupplement".ToTag(), 1f)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("Apothecary", array3, array4);
		complexRecipe = new ComplexRecipe(id2, array3, array4);
		complexRecipe.time = 40f;
		complexRecipe.description = ITEMS.PILLS.VITAMINSUPPLEMENT.RECIPEDESC;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag>
		{
			"Apothecary"
		};
		VitaminSupplementConfig.recipe = complexRecipe;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.SetAttributeConverter(Db.Get().AttributeConverters.CompoundingSpeed);
		};
	}
}
