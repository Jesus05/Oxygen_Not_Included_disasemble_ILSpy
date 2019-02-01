public class SafeCellQuery : PathFinderQuery
{
	public enum SafeFlags
	{
		IsNotLiquid = 1,
		IsNotLadder = 2,
		IsNotLiquidOnMyFace = 4,
		CorrectTemperature = 8,
		HasSomeOxygen = 0x10,
		HasLotsOxygen = 0x20,
		IsClear = 0x40,
		IsNotTube = 0x80
	}

	private MinionBrain brain;

	private int targetCell;

	private int targetCost;

	public SafeFlags targetCellFlags;

	public SafeCellQuery Reset(MinionBrain brain)
	{
		this.brain = brain;
		targetCell = PathFinder.InvalidCell;
		targetCost = 2147483647;
		targetCellFlags = (SafeFlags)0;
		return this;
	}

	public static SafeFlags GetFlags(int cell, MinionBrain brain)
	{
		int num = Grid.CellAbove(cell);
		if (Grid.IsValidCell(num))
		{
			bool flag = brain.IsCellClear(cell);
			bool flag2 = !Grid.Element[cell].IsLiquid;
			bool flag3 = !Grid.Element[num].IsLiquid;
			bool flag4 = Grid.Foundation[cell] || Grid.Foundation[num];
			bool flag5 = Grid.Temperature[cell] < 303f;
			bool flag6 = brain.OxygenBreather.IsBreathableElementAtCell(cell, Grid.DefaultOffset);
			bool flag7 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder) && !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole);
			bool flag8 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Tube);
			bool flag9 = Grid.IsTileUnderConstruction[cell] || Grid.IsTileUnderConstruction[num];
			if (cell == Grid.PosToCell(brain))
			{
				flag6 = !brain.OxygenBreather.IsSuffocating;
			}
			SafeFlags safeFlags = (SafeFlags)0;
			if (flag)
			{
				safeFlags |= SafeFlags.IsClear;
			}
			if (flag5)
			{
				safeFlags |= SafeFlags.CorrectTemperature;
			}
			if (flag6)
			{
				safeFlags |= SafeFlags.HasSomeOxygen;
			}
			if (flag7)
			{
				safeFlags |= SafeFlags.IsNotLadder;
			}
			if (flag8)
			{
				safeFlags |= SafeFlags.IsNotTube;
			}
			if (flag2)
			{
				safeFlags |= SafeFlags.IsNotLiquid;
			}
			if (flag3)
			{
				safeFlags |= SafeFlags.IsNotLiquidOnMyFace;
			}
			if (flag4 || flag9)
			{
				safeFlags = (SafeFlags)0;
			}
			return safeFlags;
		}
		return (SafeFlags)0;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeFlags flags = GetFlags(cell, brain);
		bool flag = flags > targetCellFlags;
		bool flag2 = flags == targetCellFlags && cost < targetCost;
		if (flag || flag2)
		{
			targetCellFlags = flags;
			targetCost = cost;
			targetCell = cell;
		}
		return false;
	}

	public override int GetResultCell()
	{
		return targetCell;
	}
}
