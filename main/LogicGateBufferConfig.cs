using STRINGS;
using UnityEngine;

public class LogicGateBufferConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateBUFFER";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		LogicGate.LogicGateDescriptions logicGateDescriptions = new LogicGate.LogicGateDescriptions();
		logicGateDescriptions.output = new LogicGate.LogicGateDescriptions.Description
		{
			name = (string)BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_NAME,
			active = (string)BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_ACTIVE,
			inactive = (string)BUILDINGS.PREFABS.LOGICGATEBUFFER.OUTPUT_INACTIVE
		};
		return logicGateDescriptions;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateBUFFER", "logic_buffer_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateBuffer logicGateBuffer = go.AddComponent<LogicGateBuffer>();
		logicGateBuffer.op = GetLogicOp();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGateBuffer component = game_object.GetComponent<LogicGateBuffer>();
			component.SetPortDescriptions(GetDescriptions());
		};
	}
}
