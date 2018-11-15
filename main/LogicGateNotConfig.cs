public class LogicGateNotConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateNOT";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Not;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateNOT", "logic_not_kanim", 2, 1);
	}
}
