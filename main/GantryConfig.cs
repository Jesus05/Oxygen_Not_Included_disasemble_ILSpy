using STRINGS;
using TUNING;
using UnityEngine;

public class GantryConfig : IBuildingConfig
{
	public const string ID = "Gantry";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(Gantry.PORT_ID, new CellOffset(-1, 1), STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT_INACTIVE, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Gantry";
		int width = 6;
		int height = 2;
		string anim = "gantry_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] construction_materials = new string[1]
		{
			(-899253461).ToString()
		};
		float melting_point = 3200f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, construction_materials, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, nONE, 1f);
		buildingDef.ObjectLayer = ObjectLayer.Gantry;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.Entombable = true;
		buildingDef.IsFoundation = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-2, 0);
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
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
		go.AddOrGet<Gantry>();
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		Object.DestroyImmediate(go.GetComponent<LogicOperationalController>());
	}
}
