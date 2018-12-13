using UnityEngine;

public class NavTactic
{
	private int _overlapPenalty = 3;

	private int _preferredRange;

	private int _rangePenalty = 2;

	private int _pathCostPenalty = 1;

	public NavTactic(int preferredRange, int rangePenalty = 1, int overlapPenalty = 1, int pathCostPenalty = 1)
	{
		_overlapPenalty = overlapPenalty;
		_preferredRange = preferredRange;
		_rangePenalty = rangePenalty;
		_pathCostPenalty = pathCostPenalty;
	}

	public int GetCellPreferences(int root, CellOffset[] offsets, Navigator navigator)
	{
		int result = NavigationReservations.InvalidReservation;
		int num = 2147483647;
		for (int i = 0; i < offsets.Length; i++)
		{
			int num2 = Grid.OffsetCell(root, offsets[i]);
			int num3 = 0;
			num3 += _overlapPenalty * NavigationReservations.Instance.GetOccupancyCount(num2);
			num3 += _rangePenalty * Mathf.Abs(_preferredRange - Grid.GetCellDistance(root, num2));
			num3 += _pathCostPenalty * Mathf.Max(navigator.GetNavigationCost(num2), 0);
			if (num3 < num && navigator.CanReach(num2))
			{
				num = num3;
				result = num2;
			}
		}
		return result;
	}
}
