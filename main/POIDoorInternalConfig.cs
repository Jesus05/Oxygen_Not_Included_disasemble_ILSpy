using STRINGS;
using TUNING;
using UnityEngine;

public class POIDoorInternalConfig : IBuildingConfig
{
	public static string ID = "POIDoorInternal";

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(Door.OPEN_CLOSE_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_PORT_DESC, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		int width = 1;
		int height = 2;
		string anim = "door_poi_internal_kanim";
		int hitpoints = 30;
		float construction_time = 10f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Tile;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, nONE, 1f);
		buildingDef.ShowInBuildMenu = false;
		buildingDef.Entombable = false;
		buildingDef.Floodable = false;
		buildingDef.Invincible = true;
		buildingDef.IsFoundation = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<AccessControl>();
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 3f;
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AccessControl component = go.GetComponent<AccessControl>();
		Door component2 = go.GetComponent<Door>();
		component2.hasComplexUserControls = false;
		component.controlEnabled = false;
		go.GetComponent<Deconstructable>().allowDeconstruction = true;
		KBatchedAnimController component3 = go.GetComponent<KBatchedAnimController>();
		component3.initialAnim = "closed";
	}
}
