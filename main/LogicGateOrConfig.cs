using STRINGS;

public class LogicGateOrConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateOR";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Or;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		LogicGate.LogicGateDescriptions logicGateDescriptions = new LogicGate.LogicGateDescriptions();
		logicGateDescriptions.output = new LogicGate.LogicGateDescriptions.Description
		{
			name = (string)BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_NAME,
			active = (string)BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_ACTIVE,
			inactive = (string)BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_INACTIVE
		};
		return logicGateDescriptions;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateOR", "logic_or_kanim", 2, 2);
	}
}
