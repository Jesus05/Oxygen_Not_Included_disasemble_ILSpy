using System;

public class NavTable
{
	public Action<int, NavType> OnValidCellChanged;

	private short[] NavTypeMasks;

	private short[] ValidCells;

	public NavTable(int cell_count)
	{
		ValidCells = new short[cell_count];
		NavTypeMasks = new short[10];
		for (short num = 0; num < 10; num = (short)(num + 1))
		{
			NavTypeMasks[num] = (short)(1 << (int)num);
		}
	}

	public bool IsValid(int cell, NavType nav_type = NavType.Floor)
	{
		if (Grid.IsValidCell(cell))
		{
			short num = NavTypeMasks[(uint)nav_type];
			return (num & ValidCells[cell]) != 0;
		}
		return false;
	}

	public void SetValid(int cell, NavType nav_type, bool is_valid)
	{
		short num = NavTypeMasks[(uint)nav_type];
		short num2 = ValidCells[cell];
		bool flag = (num2 & num) != 0;
		if (flag != is_valid)
		{
			if (is_valid)
			{
				ValidCells[cell] = (short)(num | num2);
			}
			else
			{
				ValidCells[cell] = (short)(~num & num2);
			}
			if (OnValidCellChanged != null)
			{
				OnValidCellChanged(cell, nav_type);
			}
		}
	}
}
