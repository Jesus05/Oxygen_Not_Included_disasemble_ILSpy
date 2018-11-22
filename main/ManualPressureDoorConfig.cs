using TUNING;
using UnityEngine;

public class ManualPressureDoorConfig : IBuildingConfig
{
	public const string ID = "ManualPressureDoor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ManualPressureDoor", 1, 2, "door_manual_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NONE, 1f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_gear_LP", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_open", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_close", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Door door = go.AddOrGet<Door>();
		door.hasComplexUserControls = true;
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.ManualPressure;
		go.AddOrGet<AccessControl>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
		copyBuildingSettings.copyGroupTag = GameTags.Door;
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AccessControl component = go.GetComponent<AccessControl>();
		component.controlEnabled = true;
		KBatchedAnimController component2 = go.GetComponent<KBatchedAnimController>();
		component2.initialAnim = "closed";
	}
}
