using TUNING;
using UnityEngine;

public class WireRefinedBridgeConfig : WireBridgeConfig
{
	public new const string ID = "WireRefinedBridge";

	protected override string GetID()
	{
		return "WireRefinedBridge";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef();
		buildingDef.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim("utilityelectricbridgeconductive_kanim")
		};
		buildingDef.Mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireRefinedBridge");
		return buildingDef;
	}

	protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max2000;
		return wireUtilityNetworkLink;
	}
}
