using STRINGS;
using TUNING;
using UnityEngine;

public class MarbleSculptureConfig : IBuildingConfig
{
	public const string ID = "MarbleSculpture";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MarbleSculpture";
		int width = 2;
		int height = 3;
		string anim = "sculpture_marble_kanim";
		int hitpoints = 10;
		float construction_time = 120f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] pRECIOUS_ROCKS = MATERIALS.PRECIOUS_ROCKS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, pRECIOUS_ROCKS, melting_point, build_location_rule, new EffectorValues
		{
			amount = 20,
			radius = 8
		}, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.DefaultAnimState = "slab";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Artable artable = go.AddComponent<Sculpture>();
		artable.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.NAME, "slab", 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.POORQUALITYNAME, "crap_1", 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.AVERAGEQUALITYNAME, "amazing_1", 15, true, Artable.Status.Great));
	}
}
