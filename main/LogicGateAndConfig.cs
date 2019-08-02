using STRINGS;

public class LogicGateAndConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateAND";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.And;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		LogicGate.LogicGateDescriptions logicGateDescriptions = new LogicGate.LogicGateDescriptions();
		logicGateDescriptions.output = new LogicGate.LogicGateDescriptions.Description
		{
			name = (string)BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_NAME,
			active = (string)BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_ACTIVE,
			inactive = (string)BUILDINGS.PREFABS.LOGICGATEAND.OUTPUT_INACTIVE
		};
		return logicGateDescriptions;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateAND", "logic_and_kanim", 2, 2);
	}
}
