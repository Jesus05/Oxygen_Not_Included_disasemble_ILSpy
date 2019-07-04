using TUNING;
using UnityEngine;

public class WireBridgeConfig : IBuildingConfig
{
	public const string ID = "WireBridge";

	protected virtual string GetID()
	{
		return "WireBridge";
	}

	public override BuildingDef CreateBuildingDef()
	{
		string iD = GetID();
		int width = 3;
		int height = 1;
		string anim = "utilityelectricbridge_kanim";
		int hitpoints = 30;
		float construction_time = 3f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.WireBridge;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.ObjectLayer = ObjectLayer.WireConnectors;
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
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
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = AddNetworkLink(go);
		wireUtilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	protected virtual WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = go.AddOrGet<WireUtilityNetworkLink>();
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max1000;
		wireUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		wireUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return wireUtilityNetworkLink;
	}
}
