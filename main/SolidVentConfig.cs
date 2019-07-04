using TUNING;
using UnityEngine;

public class SolidVentConfig : IBuildingConfig
{
	public const string ID = "SolidVent";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "SolidVent";
		int width = 1;
		int height = 1;
		string anim = "conveyer_dropper_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
		buildingDef.InputConduitType = ConduitType.Solid;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidVent");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<SimpleVent>();
		go.AddOrGet<SolidConduitConsumer>();
		go.AddOrGet<SolidConduitDropper>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.capacityKg = 100f;
		storage.showInUI = true;
	}
}
