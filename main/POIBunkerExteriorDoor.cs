using TUNING;
using UnityEngine;

public class POIBunkerExteriorDoor : IBuildingConfig
{
	public const string ID = "POIBunkerExteriorDoor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("POIBunkerExteriorDoor", 1, 2, "door_poi_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Repairable = false;
		buildingDef.Floodable = false;
		buildingDef.Invincible = true;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		buildingDef.ShowInBuildMenu = false;
		SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_gear_LP", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_open", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_close", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Door door = go.AddOrGet<Door>();
		door.hasComplexUserControls = false;
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Sealed;
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<AccessControl>();
		go.AddOrGet<Unsealable>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
		KBatchedAnimController kBatchedAnimController = go.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AccessControl component = go.GetComponent<AccessControl>();
		component.controlEnabled = false;
		go.GetComponent<Deconstructable>().allowDeconstruction = false;
		KBatchedAnimController component2 = go.GetComponent<KBatchedAnimController>();
		component2.initialAnim = "closed";
	}
}
