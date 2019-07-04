using TUNING;
using UnityEngine;

public class FlyingCreatureBaitConfig : IBuildingConfig
{
	public const string ID = "FlyingCreatureBait";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FlyingCreatureBait", 1, 1, "flowervase_hanging_basic_kanim", 10, 10f, new float[2]
		{
			50f,
			10f
		}, new string[2]
		{
			"Metal",
			"FlyingCritterEdible"
		}, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<CreatureBait>();
		go.AddOrGet<Operational>();
		go.AddTag(GameTags.OneTimeUseLure);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		SymbolOverrideControllerUtil.AddToPrefab(go);
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		go.AddOrGet<LogicOperationalController>();
		Lure.Def def = go.AddOrGetDef<Lure.Def>();
		def.lurePoints = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		def.radius = 32;
		Prioritizable.AddRef(go);
	}
}
