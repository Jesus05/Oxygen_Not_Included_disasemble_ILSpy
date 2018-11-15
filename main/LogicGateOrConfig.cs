public class LogicGateOrConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateOR";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Or;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateOR", "logic_or_kanim", 2, 2);
	}
}
