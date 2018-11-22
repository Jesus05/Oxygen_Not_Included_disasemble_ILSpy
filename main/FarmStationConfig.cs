using STRINGS;
using TUNING;
using UnityEngine;

public class FarmStationConfig : IBuildingConfig
{
	public const string ID = "FarmStation";

	public static Tag MATERIAL_FOR_TINKER = GameTags.Fertilizer;

	public static Tag TINKER_TOOLS = FarmStationToolsConfig.tag;

	public const float MASS_PER_TINKER = 5f;

	public const string ROLE_TYPE = "Farmer";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "FarmStation";
		int width = 2;
		int height = 3;
		string anim = "planttender_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.FarmStation);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = MATERIAL_FOR_TINKER;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.capacity = 50f;
		manualDeliveryKG.choreTags = new Tag[1]
		{
			GameTags.ChoreTypes.Farming
		};
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		tinkerStation.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_planttender_kanim")
		};
		tinkerStation.inputMaterial = MATERIAL_FOR_TINKER;
		tinkerStation.massPerTinker = 5f;
		tinkerStation.outputPrefab = TINKER_TOOLS;
		tinkerStation.requiredRolePerk = RoleManager.rolePerks.CanFarmTinker.id;
		tinkerStation.choreType = Db.Get().ChoreTypes.FarmingFabricate.IdHash;
		tinkerStation.fetchChoreType = Db.Get().ChoreTypes.FarmFetch.IdHash;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
	}
}
