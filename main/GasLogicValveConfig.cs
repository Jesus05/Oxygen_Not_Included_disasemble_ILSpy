using STRINGS;
using TUNING;
using UnityEngine;

public class GasLogicValveConfig : IBuildingConfig
{
	public const string ID = "GasLogicValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT_INACTIVE, true, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "GasLogicValve";
		int width = 1;
		int height = 2;
		string anim = "valvegas_logic_kanim";
		int hitpoints = 30;
		float construction_time = 10f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, tIER2, 0.2f);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
		operationalValve.conduitType = ConduitType.Gas;
		operationalValve.maxFlow = 1f;
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
		Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		RequireInputs component = go.GetComponent<RequireInputs>();
		component.SetRequirements(true, false);
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		LogicOperationalController logicOperationalController = go.AddOrGet<LogicOperationalController>();
		logicOperationalController.unNetworkedValue = 0;
	}
}
