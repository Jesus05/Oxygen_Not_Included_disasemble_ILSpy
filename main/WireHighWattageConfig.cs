using TUNING;
using UnityEngine;

public class WireHighWattageConfig : BaseWireConfig
{
	public const string ID = "HighWattageWire";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = CreateBuildingDef("HighWattageWire", "utilities_electric_insulated_kanim", 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, 0.05f, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER5);
		buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DoPostConfigureComplete(Wire.WattageRating.Max20000, go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
	}
}
