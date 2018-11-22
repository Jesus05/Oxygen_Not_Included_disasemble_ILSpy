using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RanchStationConfig : IBuildingConfig
{
	public const string ID = "RanchStation";

	public const string ROLE_TYPE = "Rancher";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "RanchStation";
		int width = 2;
		int height = 3;
		string anim = "rancherstation_kanim";
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
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStation);
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
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.isCreatureEligibleToBeRanchedCb = ((GameObject creature_go, RanchStation.Instance ranch_station_smi) => !creature_go.GetComponent<Effects>().HasEffect("Ranched"));
		def.onRanchCompleteCb = delegate(GameObject creature_go)
		{
			creature_go.GetComponent<Effects>().Add("Ranched", true);
		};
		def.ranchedPreAnim = "grooming_pre";
		def.ranchedLoopAnim = "grooming_loop";
		def.ranchedPstAnim = "grooming_pst";
		def.getTargetRanchCell = delegate(RanchStation.Instance smi)
		{
			int num = Grid.InvalidCell;
			if (!smi.IsNullOrStopped())
			{
				num = Grid.CellRight(Grid.PosToCell(smi.transform.GetPosition()));
				if (!smi.targetRanchable.IsNullOrStopped() && smi.targetRanchable.HasTag(GameTags.Creatures.Flyer))
				{
					num = Grid.CellAbove(num);
				}
			}
			return num;
		};
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		RolePerkMissingComplainer rolePerkMissingComplainer = go.AddOrGet<RolePerkMissingComplainer>();
		rolePerkMissingComplainer.requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
		Prioritizable.AddRef(go);
	}
}
