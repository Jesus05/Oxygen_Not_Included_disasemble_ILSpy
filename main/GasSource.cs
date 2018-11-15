using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class GasSource : SubstanceSource
{
	protected override CellOffset[] GetOffsetGroup()
	{
		return OffsetGroups.LiquidSource;
	}

	protected override IChunkManager GetChunkManager()
	{
		return GasSourceManager.Instance;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}
}
