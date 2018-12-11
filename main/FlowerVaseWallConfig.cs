using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FlowerVaseWallConfig : IBuildingConfig
{
	public const string ID = "FlowerVaseWall";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "FlowerVaseWall";
		int width = 1;
		int height = 1;
		string anim = "flowervase_wall_kanim";
		int hitpoints = 10;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnWall;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.DecorSeed);
		plantablePlot.occupyingObjectVisualOffset = new Vector3(0f, -0.25f, 0f);
		SituationalAnim situationalAnim = go.AddOrGet<SituationalAnim>();
		situationalAnim.mustSatisfy = SituationalAnim.MustSatisfy.All;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.instantiateFn += delegate(GameObject inst)
		{
			SituationalAnim component2 = inst.GetComponent<SituationalAnim>();
			component2.anims = new List<Tuple<SituationalAnim.Situation, string>>
			{
				new Tuple<SituationalAnim.Situation, string>(SituationalAnim.Situation.Left, "leftwall"),
				new Tuple<SituationalAnim.Situation, string>(SituationalAnim.Situation.Right, "rightwall")
			};
			component2.test = IsSolidOrTile;
		};
	}

	private bool IsSolidOrTile(int cell)
	{
		return Grid.Solid[cell];
	}
}
