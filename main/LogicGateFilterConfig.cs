using STRINGS;
using UnityEngine;

public class LogicGateFilterConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateFILTER";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		LogicGate.LogicGateDescriptions logicGateDescriptions = new LogicGate.LogicGateDescriptions();
		logicGateDescriptions.output = new LogicGate.LogicGateDescriptions.Description
		{
			name = (string)BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_NAME,
			active = (string)BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_ACTIVE,
			inactive = (string)BUILDINGS.PREFABS.LOGICGATEFILTER.OUTPUT_INACTIVE
		};
		return logicGateDescriptions;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateFILTER", "logic_filter_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateFilter logicGateFilter = go.AddComponent<LogicGateFilter>();
		logicGateFilter.op = GetLogicOp();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGateFilter component = game_object.GetComponent<LogicGateFilter>();
			component.SetPortDescriptions(GetDescriptions());
		};
	}
}
