using Klei.AI;
using TUNING;
using UnityEngine;

public class RanchStationConfig : IBuildingConfig
{
	public const string ID = "RanchStation";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "RanchStation";
		int width = 2;
		int height = 3;
		string anim = "rancherstation_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStation, false);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		go.AddOrGet<LogicOperationalController>();
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.isCreatureEligibleToBeRanchedCb = ((GameObject creature_go, RanchStation.Instance ranch_station_smi) => !creature_go.GetComponent<Effects>().HasEffect("Ranched"));
		def.onRanchCompleteCb = delegate(GameObject creature_go)
		{
			RanchableMonitor.Instance sMI = creature_go.GetSMI<RanchableMonitor.Instance>();
			RanchStation.Instance targetRanchStation = sMI.targetRanchStation;
			RancherChore.RancherChoreStates.Instance sMI2 = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>();
			GameObject go2 = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>().sm.rancher.Get(sMI2);
			float num2 = 1f + go2.GetAttributes().Get(Db.Get().Attributes.Ranching.Id).GetTotalValue() * 0.1f;
			creature_go.GetComponent<Effects>().Add("Ranched", true).timeRemaining *= num2;
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
		SkillPerkMissingComplainer skillPerkMissingComplainer = go.AddOrGet<SkillPerkMissingComplainer>();
		skillPerkMissingComplainer.requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
		Prioritizable.AddRef(go);
	}
}
