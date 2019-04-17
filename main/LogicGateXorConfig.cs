using STRINGS;

public class LogicGateXorConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateXOR";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Xor;
	}

	protected override LogicGate.LogicGateDescriptions GetDescriptions()
	{
		LogicGate.LogicGateDescriptions logicGateDescriptions = new LogicGate.LogicGateDescriptions();
		logicGateDescriptions.output = new LogicGate.LogicGateDescriptions.Description
		{
			name = (string)BUILDINGS.PREFABS.LOGICGATEXOR.OUTPUT_NAME,
			active = (string)BUILDINGS.PREFABS.LOGICGATEXOR.OUTPUT_ACTIVE,
			inactive = (string)BUILDINGS.PREFABS.LOGICGATEXOR.OUTPUT_INACTIVE
		};
		return logicGateDescriptions;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateXOR", "logic_xor_kanim", 2, 2);
	}
}
