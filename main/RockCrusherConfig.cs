using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RockCrusherConfig : IBuildingConfig
{
	public const string ID = "RockCrusher";

	private const float INPUT_KG = 100f;

	private const float METAL_ORE_EFFICIENCY = 0.5f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "RockCrusher";
		int width = 4;
		int height = 4;
		string anim = "rockrefinery_kanim";
		int hitpoints = 30;
		float construction_time = 60f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
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
			Assets.GetAnim("anim_interacts_rockrefinery_kanim")
		};
		complexFabricatorWorkable.workingPstComplete = "working_pst_complete";
		Tag tag = SimHashes.Sand.CreateTag();
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.HasTag(GameTags.Crushable));
		ComplexRecipe complexRecipe;
		foreach (Element item in list)
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement(item.tag, 100f)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement(tag, 100f)
			};
			string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", item.tag);
			string text = ComplexRecipeManager.MakeRecipeID("RockCrusher", array, array2);
			complexRecipe = new ComplexRecipe(text, array, array2);
			complexRecipe.time = 40f;
			complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, item.name, tag.ProperName());
			complexRecipe.useResultAsDescription = true;
			complexRecipe.displayInputAndOutput = true;
			complexRecipe.fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			};
			ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		}
		List<Element> list2 = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal));
		foreach (Element item2 in list2)
		{
			Element highTempTransition = item2.highTempTransition;
			Element lowTempTransition = highTempTransition.lowTempTransition;
			if (lowTempTransition != item2)
			{
				ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
				{
					new ComplexRecipe.RecipeElement(item2.tag, 100f)
				};
				ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[2]
				{
					new ComplexRecipe.RecipeElement(lowTempTransition.tag, 50f),
					new ComplexRecipe.RecipeElement(tag, 50f)
				};
				string obsolete_id2 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", lowTempTransition.tag);
				string text2 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array3, array4);
				complexRecipe = new ComplexRecipe(text2, array3, array4);
				complexRecipe.time = 40f;
				complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, lowTempTransition.name, item2.name);
				complexRecipe.useResultAsDescription = true;
				complexRecipe.displayInputAndOutput = true;
				complexRecipe.fabricators = new List<Tag>
				{
					TagManager.Create("RockCrusher")
				};
				ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id2, text2);
			}
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("EggShell", 5f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f)
		};
		string obsolete_id3 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element.tag);
		string text3 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array5, array6);
		complexRecipe = new ComplexRecipe(text3, array5, array6);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), MISC.TAGS.EGGSHELL);
		complexRecipe.useResultAsDescription = true;
		complexRecipe.displayInputAndOutput = true;
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("RockCrusher")
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id3, text3);
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Fossil).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag, 95f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("RockCrusher", array7, array8);
		complexRecipe = new ComplexRecipe(id, array7, array8);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, SimHashes.Fossil.CreateTag().ProperName(), SimHashes.SedimentaryRock.CreateTag().ProperName(), SimHashes.Lime.CreateTag().ProperName());
		complexRecipe.useResultAsDescription = true;
		complexRecipe.displayInputAndOutput = true;
		complexRecipe.fabricators = new List<Tag>
		{
			TagManager.Create("RockCrusher")
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
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
