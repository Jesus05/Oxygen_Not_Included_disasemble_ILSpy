using STRINGS;

public class LogicGateNotConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateNOT";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Not;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		LogicGate.LogicGateDescriptions logicGateDescriptions = new LogicGate.LogicGateDescriptions();
		logicGateDescriptions.output = new LogicGate.LogicGateDescriptions.Description
		{
			name = (string)BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_NAME,
			active = (string)BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_ACTIVE,
			inactive = (string)BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_INACTIVE
		};
		return logicGateDescriptions;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateNOT", "logic_not_kanim", 2, 1);
	}
}
