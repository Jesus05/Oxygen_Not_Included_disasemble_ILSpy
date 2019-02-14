using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class WireBridgeHighWattageConfig : IBuildingConfig
{
	public const string ID = "WireBridgeHighWattage";

	protected virtual string GetID()
	{
		return "WireBridgeHighWattage";
	}

	public override BuildingDef CreateBuildingDef()
	{
		string iD = GetID();
		int width = 1;
		int height = 1;
		string anim = "heavywatttile_kanim";
		int hitpoints = 100;
		float construction_time = 3f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.HighWattBridgeTile;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER5, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		buildingDef.IsFoundation = true;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridgesFront;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileFront;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireBridgeHighWattage");
		buildingDef.ReplacementTags = new List<Tag>();
		buildingDef.ReplacementTags.Add(GameTags.FloorTiles);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.PENALTY_3;
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		buildingHP.destroyOnDamaged = true;
		go.AddOrGet<TileTemperature>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.choreTags = GameTags.ChoreTypes.WiringChores;
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.WireBridges);
		go.AddOrGet<BuildingCellVisualizer>();
	}

	protected virtual WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = go.AddOrGet<WireUtilityNetworkLink>();
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max20000;
		wireUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		wireUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return wireUtilityNetworkLink;
	}
}
