using TUNING;
using UnityEngine;

public class FacilityBackWallWindowConfig : IBuildingConfig
{
	public const string ID = "FacilityBackWallWindow";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "FacilityBackWallWindow";
		int width = 1;
		int height = 6;
		string anim = "gravitas_window_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] gLASSES = MATERIALS.GLASSES;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.NotInTiles;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, gLASSES, melting_point, build_location_rule, DECOR.BONUS.TIER3, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.DefaultAnimState = "off";
		buildingDef.ObjectLayer = ObjectLayer.Backwall;
		buildingDef.SceneLayer = Grid.SceneLayer.TempShiftPlate;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.objectLayer = ObjectLayer.Backwall;
		ZoneTile zoneTile = go.AddComponent<ZoneTile>();
		zoneTile.width = 1;
		zoneTile.height = 6;
		go.GetComponent<PrimaryElement>().SetElement(SimHashes.Steel);
		go.GetComponent<PrimaryElement>().Temperature = 273f;
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
