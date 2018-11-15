using UnityEngine;

public class LogicGateBufferConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateBUFFER";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.CustomSingle;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateBUFFER", "logic_buffer_kanim", 2, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGateBuffer logicGateBuffer = go.AddComponent<LogicGateBuffer>();
		logicGateBuffer.op = GetLogicOp();
	}
}
