using TUNING;
using UnityEngine;

public class FlowerVaseHangingFancyConfig : IBuildingConfig
{
	public const string ID = "FlowerVaseHangingFancy";

	public override BuildingDef CreateBuildingDef()
	{
		string text = "FlowerVaseHangingFancy";
		int num = 1;
		int num2 = 2;
		string text2 = "flowervase_hanging_kanim";
		int num3 = 10;
		float num4 = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] tRANSPARENTS = MATERIALS.TRANSPARENTS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnCeiling;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		string id = text;
		int width = num;
		int height = num2;
		string anim = text2;
		int hitpoints = num3;
		float construction_time = num4;
		float[] construction_mass = tIER;
		string[] construction_materials = tRANSPARENTS;
		float melting_point = num5;
		BuildLocationRule build_location_rule = buildLocationRule;
		EffectorValues decor = default(EffectorValues);
		EffectorValues tIER2 = BUILDINGS.DECOR.BONUS.TIER1;
		decor.amount = tIER2.amount;
		EffectorValues tIER3 = BUILDINGS.DECOR.BONUS.TIER3;
		decor.radius = tIER3.radius;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, construction_mass, construction_materials, melting_point, build_location_rule, decor, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		buildingDef.GenerateOffsets(1, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.DecorSeed);
		plantablePlot.plantLayer = Grid.SceneLayer.BuildingFront;
		plantablePlot.occupyingObjectVisualOffset = new Vector3(0f, -0.45f, 0f);
		go.AddOrGet<FlowerVase>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
