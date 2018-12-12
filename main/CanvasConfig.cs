using STRINGS;
using TUNING;
using UnityEngine;

public class CanvasConfig : IBuildingConfig
{
	public const string ID = "Canvas";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Canvas";
		int width = 2;
		int height = 2;
		string anim = "painting_kanim";
		int hitpoints = 30;
		float construction_time = 120f;
		float[] construction_mass = new float[2]
		{
			400f,
			1f
		};
		string[] construction_materials = new string[2]
		{
			"Metal",
			"BuildingFiber"
		};
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, construction_mass, construction_materials, melting_point, build_location_rule, new EffectorValues
		{
			amount = 10,
			radius = 6
		}, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.SceneLayer = Grid.SceneLayer.Paintings;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.DefaultAnimState = "off";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Artable artable = go.AddComponent<Painting>();
		artable.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.CANVAS.NAME, "off", 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "art_a", 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "art_b", 10, false, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_c", 15, true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good2", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_d", 15, true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good3", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_e", 15, true, Artable.Status.Great));
	}
}
