using TUNING;
using UnityEngine;

public class CreatureDeliveryPointConfig : IBuildingConfig
{
	public const string ID = "CreatureDeliveryPoint";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CreatureDeliveryPoint", 1, 3, "relocator_dropoff_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CreatureRelocator);
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.BAGABLE_CREATURES;
		storage.workAnims = new HashedString[2]
		{
			"place",
			"release"
		};
		storage.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_restrain_creature_kanim")
		};
		storage.workAnimPlayMode = KAnim.PlayMode.Once;
		storage.synchronizeAnims = false;
		storage.useGunForDelivery = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		go.AddOrGet<CreatureDeliveryPoint>();
		go.AddOrGet<TreeFilterable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<FixedCapturePoint.Def>();
	}
}
