public class LogicGateXorConfig : LogicGateBaseConfig
{
	public const string ID = "LogicGateXOR";

	protected override LogicGateBase.Op GetLogicOp()
	{
		return LogicGateBase.Op.Xor;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicGateXOR", "logic_xor_kanim", 2, 2);
	}
}
