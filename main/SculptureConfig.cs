using STRINGS;
using TUNING;
using UnityEngine;

public class SculptureConfig : IBuildingConfig
{
	public const string ID = "Sculpture";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Sculpture";
		int width = 1;
		int height = 3;
		string anim = "sculpture_kanim";
		int hitpoints = 30;
		float construction_time = 120f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, new EffectorValues
		{
			amount = 10,
			radius = 8
		}, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.DefaultAnimState = "slab";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Artable artable = go.AddComponent<Sculpture>();
		artable.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.SCULPTURE.NAME, "slab", 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.SCULPTURE.POORQUALITYNAME, "crap_1", 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.SCULPTURE.AVERAGEQUALITYNAME, "good_1", 10, false, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good1", STRINGS.BUILDINGS.PREFABS.SCULPTURE.EXCELLENTQUALITYNAME, "amazing_1", 15, true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good2", STRINGS.BUILDINGS.PREFABS.SCULPTURE.EXCELLENTQUALITYNAME, "amazing_2", 15, true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good3", STRINGS.BUILDINGS.PREFABS.SCULPTURE.EXCELLENTQUALITYNAME, "amazing_3", 15, true, Artable.Status.Great));
	}
}
