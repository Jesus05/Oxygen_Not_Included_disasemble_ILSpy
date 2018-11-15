public class OutputPoint : KMonoBehaviour
{
	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Operational operational;

	public static readonly Operational.Flag outputClearFlag = new Operational.Flag("output_clear", Operational.Flag.Type.Requirement);

	public int GetOutputCell()
	{
		return Grid.CellBelow(Grid.CellBelow(Grid.PosToCell(this)));
	}
}
