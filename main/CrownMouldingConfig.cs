using TUNING;
using UnityEngine;

public class CrownMouldingConfig : IBuildingConfig
{
	public const string ID = "CrownMoulding";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "CrownMoulding";
		int width = 1;
		int height = 1;
		string anim = "crown_moulding_kanim";
		int hitpoints = 10;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnCeiling;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, new EffectorValues
		{
			amount = 5,
			radius = 3
		}, nONE, 0.2f);
		buildingDef.DefaultAnimState = "S_U";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
