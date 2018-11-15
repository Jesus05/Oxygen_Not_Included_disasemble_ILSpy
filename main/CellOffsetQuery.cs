public class CellOffsetQuery : CellArrayQuery
{
	public CellArrayQuery Reset(int cell, CellOffset[] offsets)
	{
		int[] array = new int[offsets.Length];
		for (int i = 0; i < offsets.Length; i++)
		{
			array[i] = Grid.OffsetCell(cell, offsets[i]);
		}
		Reset(array);
		return this;
	}
}
