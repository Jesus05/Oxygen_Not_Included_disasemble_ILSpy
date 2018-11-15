using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GraveConfig : IBuildingConfig
{
	public const string ID = "Grave";

	private static KAnimFile[] STORAGE_OVERRIDE_ANIM_FILES;

	private static readonly HashedString[] STORAGE_WORK_ANIMS = new HashedString[1]
	{
		"working_pre"
	};

	private static readonly HashedString STORAGE_PST_ANIM = HashedString.Invalid;

	private static readonly List<Storage.StoredItemModifier> StorageModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Grave";
		int width = 1;
		int height = 2;
		string anim = "gravestone_kanim";
		int hitpoints = 30;
		float construction_time = 120f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		STORAGE_OVERRIDE_ANIM_FILES = new KAnimFile[1]
		{
			Assets.GetAnim("anim_bury_dupe_kanim")
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(StorageModifiers);
		storage.overrideAnims = STORAGE_OVERRIDE_ANIM_FILES;
		storage.workAnims = STORAGE_WORK_ANIMS;
		storage.workingPstComplete = STORAGE_PST_ANIM;
		storage.synchronizeAnims = false;
		storage.useGunForDelivery = false;
		storage.workAnimPlayMode = KAnim.PlayMode.Once;
		go.AddOrGet<Grave>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
