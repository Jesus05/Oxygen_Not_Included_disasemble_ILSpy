using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CreatureTrapConfig : IBuildingConfig
{
	public const string ID = "CreatureTrap";

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>();

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CreatureTrap", 2, 1, "creaturetrap_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Floodable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = true;
		storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
		storage.sendOnStoreOnSpawn = true;
		Trap trap = go.AddOrGet<Trap>();
		trap.trappableCreatures = new Tag[2]
		{
			GameTags.Creatures.Walker,
			GameTags.Creatures.Hoverer
		};
		trap.trappedOffset = new Vector2(0.5f, 0f);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
