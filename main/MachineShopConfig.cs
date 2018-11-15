using TUNING;
using UnityEngine;

public class MachineShopConfig : IBuildingConfig
{
	public const string ID = "MachineShop";

	public static readonly Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public const float MASS_PER_TINKER = 5f;

	public static readonly string ROLE_PERK = "IncreaseMachinery";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MachineShop";
		int width = 4;
		int height = 2;
		string anim = "machineshop_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.Deprecated = true;
		buildingDef.ViewMode = SimViewMode.Rooms;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.MachineShop);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
