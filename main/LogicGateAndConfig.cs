public class LogicGateAndConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateAND";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.And;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateAND", "logic_and_kanim", 2, 2);
	}
}
