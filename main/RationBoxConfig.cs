using TUNING;
using UnityEngine;

public class RationBoxConfig : IBuildingConfig
{
	public const string ID = "RationBox";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "RationBox";
		int width = 2;
		int height = 2;
		string anim = "rationbox_kanim";
		int hitpoints = 10;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER0, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_close", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 150f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<RationBox>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
	}
}
