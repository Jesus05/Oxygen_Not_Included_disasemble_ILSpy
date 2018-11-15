using STRINGS;
using TUNING;
using UnityEngine;

public class ManualGeneratorConfig : IBuildingConfig
{
	public const string ID = "ManualGenerator";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "ManualGenerator";
		int width = 2;
		int height = 2;
		string anim = "generatormanual_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.GeneratorWattageRating = 400f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Breakable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Generator generator = go.AddOrGet<Generator>();
		generator.powerDistributionOrder = 10;
		ManualGenerator manualGenerator = go.AddOrGet<ManualGenerator>();
		manualGenerator.SetSliderValue(50f, 0);
		manualGenerator.workLayer = Grid.SceneLayer.BuildingFront;
		KBatchedAnimController kBatchedAnimController = go.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		kBatchedAnimController.initialAnim = "off";
		Tinkerable.MakePowerTinkerable(go);
	}
}
