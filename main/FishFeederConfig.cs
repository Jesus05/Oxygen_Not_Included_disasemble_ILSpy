using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FishFeederConfig : IBuildingConfig
{
	public const string ID = "FishFeeder";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "FishFeeder";
		int width = 1;
		int height = 3;
		string anim = "fishfeeder_kanim";
		int hitpoints = 100;
		float construction_time = 120f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, nONE, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Entombable = true;
		buildingDef.Floodable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CreatureFeeder);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 200f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 200f;
		storage2.showInUI = true;
		storage2.showDescriptor = true;
		storage2.allowItemRemoval = false;
		go.AddOrGet<StorageLocker>();
		Effect effect = new Effect("AteFromFeeder", STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.TOOLTIP, 600f, true, false, false, null, 0f, null);
		effect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.0333333351f, STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, false, false, true));
		effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 2f, STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, false, false, true));
		Db.Get().effects.Add(effect);
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<CreatureFeeder>().effectId = effect.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
		go.AddOrGetDef<FishFeeder.Def>();
		go.AddOrGetDef<MakeBaseSolid.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, Diet> item in DietManager.CollectDiets(new Tag[1]
		{
			GameTags.Creatures.Species.PacuSpecies
		}))
		{
			list.Add(item.Key);
		}
		def.BuildingComplete.GetComponent<Storage>().storageFilters = list;
	}
}
