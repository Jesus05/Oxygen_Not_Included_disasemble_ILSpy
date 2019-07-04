using System.Collections.Generic;

public class SafetyConditions
{
	public SafetyChecker.Condition IsNotLiquid;

	public SafetyChecker.Condition IsNotLadder;

	public SafetyChecker.Condition IsCorrectTemperature;

	public SafetyChecker.Condition IsWarming;

	public SafetyChecker.Condition IsCooling;

	public SafetyChecker.Condition HasSomeOxygen;

	public SafetyChecker.Condition IsClear;

	public SafetyChecker.Condition IsNotFoundation;

	public SafetyChecker.Condition IsNotDoor;

	public SafetyChecker.Condition IsNotLedge;

	public SafetyChecker.Condition IsNearby;

	public SafetyChecker WarmUpChecker;

	public SafetyChecker CoolDownChecker;

	public SafetyChecker RecoverBreathChecker;

	public SafetyChecker VomitCellChecker;

	public SafetyChecker SafeCellChecker;

	public SafetyChecker IdleCellChecker;

	public SafetyConditions()
	{
		int num = 1;
		IsNearby = new SafetyChecker.Condition("IsNearby", num *= 2, (int cell, int cost, SafetyChecker.Context context) => cost > 5);
		IsNotLedge = new SafetyChecker.Condition("IsNotLedge", num *= 2, delegate(int cell, int cost, SafetyChecker.Context context)
		{
			int cell2 = Grid.CellLeft(cell);
			int i = Grid.CellBelow(cell2);
			if (!Grid.Solid[i])
			{
				int cell3 = Grid.CellRight(cell);
				int i2 = Grid.CellBelow(cell3);
				if (Grid.Solid[i2])
				{
					return true;
				}
				return false;
			}
			return false;
		});
		IsNotLiquid = new SafetyChecker.Condition("IsNotLiquid", num *= 2, (int cell, int cost, SafetyChecker.Context context) => !Grid.Element[cell].IsLiquid);
		IsNotLadder = new SafetyChecker.Condition("IsNotLadder", num *= 2, (int cell, int cost, SafetyChecker.Context context) => !context.navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder) && !context.navigator.NavGrid.NavTable.IsValid(cell, NavType.Pole));
		IsNotDoor = new SafetyChecker.Condition("IsNotDoor", num *= 2, delegate(int cell, int cost, SafetyChecker.Context context)
		{
			int num2 = Grid.CellAbove(cell);
			return !Grid.HasDoor[cell] && Grid.IsValidCell(num2) && !Grid.HasDoor[num2];
		});
		IsCorrectTemperature = new SafetyChecker.Condition("IsCorrectTemperature", num *= 2, (int cell, int cost, SafetyChecker.Context context) => Grid.Temperature[cell] > 285.15f && Grid.Temperature[cell] < 303.15f);
		IsWarming = new SafetyChecker.Condition("IsWarming", num *= 2, (int cell, int cost, SafetyChecker.Context context) => true);
		IsCooling = new SafetyChecker.Condition("IsCooling", num *= 2, (int cell, int cost, SafetyChecker.Context context) => false);
		HasSomeOxygen = new SafetyChecker.Condition("HasSomeOxygen", num *= 2, (int cell, int cost, SafetyChecker.Context context) => context.oxygenBreather.IsBreathableElementAtCell(cell, null));
		IsClear = new SafetyChecker.Condition("IsClear", num *= 2, (int cell, int cost, SafetyChecker.Context context) => context.minionBrain.IsCellClear(cell));
		WarmUpChecker = new SafetyChecker(new List<SafetyChecker.Condition>
		{
			IsWarming
		}.ToArray());
		CoolDownChecker = new SafetyChecker(new List<SafetyChecker.Condition>
		{
			IsCooling
		}.ToArray());
		List<SafetyChecker.Condition> list = new List<SafetyChecker.Condition>
		{
			HasSomeOxygen,
			IsNotDoor
		};
		RecoverBreathChecker = new SafetyChecker(list.ToArray());
		List<SafetyChecker.Condition> list2 = new List<SafetyChecker.Condition>(list)
		{
			IsNotLiquid,
			IsCorrectTemperature
		};
		SafeCellChecker = new SafetyChecker(list2.ToArray());
		IdleCellChecker = new SafetyChecker(new List<SafetyChecker.Condition>(list2)
		{
			IsClear,
			IsNotLadder
		}.ToArray());
		VomitCellChecker = new SafetyChecker(new List<SafetyChecker.Condition>
		{
			IsNotLiquid,
			IsNotLedge,
			IsNearby
		}.ToArray());
	}
}
