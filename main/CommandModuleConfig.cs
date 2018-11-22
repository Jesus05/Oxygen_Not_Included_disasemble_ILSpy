using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CommandModuleConfig : IBuildingConfig
{
	public const string ID = "CommandModule";

	private const string TRIGGER_LAUNCH_PORT_ID = "TriggerLaunch";

	private const string LAUNCH_READY_PORT_ID = "LaunchReady";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort("TriggerLaunch", new CellOffset(0, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.OutputPort("LaunchReady", new CellOffset(0, 2), "Ready For Launch", false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "CommandModule";
		int width = 5;
		int height = 5;
		string anim = "rocket_command_module_kanim";
		int hitpoints = 1000;
		float construction_time = 60f;
		float[] cOMMAND_MODULE_MASS = TUNING.BUILDINGS.ROCKETRY_MASS_KG.COMMAND_MODULE_MASS;
		string[] construction_materials = new string[1]
		{
			(-899253461).ToString()
		};
		float melting_point = 9999f;
		BuildLocationRule build_location_rule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tIER = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, cOMMAND_MODULE_MASS, construction_materials, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tIER, 0.2f);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<RocketModule>();
		LaunchConditionManager launchConditionManager = go.AddOrGet<LaunchConditionManager>();
		launchConditionManager.triggerPort = "TriggerLaunch";
		launchConditionManager.statusPort = "LaunchReady";
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		go.AddOrGet<CommandModule>();
		go.AddOrGet<CommandModuleWorkable>();
		go.AddOrGet<MinionStorage>();
		go.AddOrGet<LaunchableRocket>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.RocketCommandModule.Id;
		ownable.canBePublic = false;
		EntityTemplates.ExtendBuildingToRocketModule(go);
	}
}
