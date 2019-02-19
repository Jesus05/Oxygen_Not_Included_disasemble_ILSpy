using TUNING;
using UnityEngine;

public class WaterCoolerConfig : IBuildingConfig
{
	public const string ID = "WaterCooler";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "WaterCooler";
		int width = 2;
		int height = 2;
		string anim = "watercooler_kanim";
		int hitpoints = 30;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER1, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 10f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.Water;
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.refillMass = 9f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		go.AddOrGet<WaterCooler>();
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
