using STRINGS;
using TUNING;
using UnityEngine;

public class BunkerDoorConfig : IBuildingConfig
{
	public const string ID = "BunkerDoor";

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(Door.OPEN_CLOSE_PORT_ID, new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_PORT_DESC, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "BunkerDoor";
		int width = 4;
		int height = 1;
		string anim = "door_bunker_kanim";
		int hitpoints = 1000;
		float construction_time = 120f;
		float[] construction_mass = new float[1]
		{
			500f
		};
		string[] construction_materials = new string[1]
		{
			(-899253461).ToString()
		};
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Tile;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, construction_mass, construction_materials, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, nONE, 1f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.OverheatTemperature = 1273.15f;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
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
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 0.01f;
		door.poweredAnimSpeed = 0.1f;
		door.hasComplexUserControls = true;
		door.allowAutoControl = false;
		door.doorOpeningSoundEventName = "BunkerDoor_opening";
		door.doorClosingSoundEventName = "BunkerDoor_closing";
		door.verticalOrientation = Orientation.R90;
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 3f;
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.initialAnim = "closed";
		component.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		component2.AddTag(GameTags.Bunker);
	}
}
