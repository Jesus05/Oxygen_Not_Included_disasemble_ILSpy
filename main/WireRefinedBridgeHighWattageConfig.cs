using TUNING;
using UnityEngine;

public class WireRefinedBridgeHighWattageConfig : WireBridgeHighWattageConfig
{
	public new const string ID = "WireRefinedBridgeHighWattage";

	protected override string GetID()
	{
		return "WireRefinedBridgeHighWattage";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef();
		buildingDef.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim("heavywatttile_conductive_kanim")
		};
		buildingDef.Mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireRefinedBridgeHighWattage");
		return buildingDef;
	}

	protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max50000;
		return wireUtilityNetworkLink;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.requiredRolePerk = RoleManager.rolePerks.CanPowerTinker.id;
	}
}
