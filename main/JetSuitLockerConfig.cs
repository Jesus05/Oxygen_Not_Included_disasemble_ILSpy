using TUNING;
using UnityEngine;

public class JetSuitLockerConfig : IBuildingConfig
{
	public const string ID = "JetSuitLocker";

	public const float O2_CAPACITY = 200f;

	public const float SUIT_CAPACITY = 200f;

	private ConduitPortInfo secondaryInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));

	public override BuildingDef CreateBuildingDef()
	{
		string id = "JetSuitLocker";
		int width = 2;
		int height = 4;
		string anim = "changingarea_jetsuit_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, new float[1]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		}, rEFINED_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, nONE, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "JetSuitLocker");
		return buildingDef;
	}

	private void AttachPort(GameObject go)
	{
		ConduitSecondaryInput conduitSecondaryInput = go.AddComponent<ConduitSecondaryInput>();
		conduitSecondaryInput.portInfo = secondaryInputPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SuitLocker suitLocker = go.AddOrGet<SuitLocker>();
		suitLocker.OutfitTags = new Tag[1]
		{
			GameTags.JetSuit
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.capacityKG = 200f;
		JetSuitLocker jetSuitLocker = go.AddComponent<JetSuitLocker>();
		jetSuitLocker.portInfo = secondaryInputPort;
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[2]
		{
			new Tag("JetSuitLocker"),
			new Tag("JetSuitMarker")
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 500f;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		AttachPort(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		AttachPort(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
