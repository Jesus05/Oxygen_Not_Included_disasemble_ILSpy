using System.Collections.Generic;

public class NavigationReservations : KMonoBehaviour
{
	public static NavigationReservations Instance;

	public static int InvalidReservation = -1;

	private Dictionary<int, int> cellOccupancyDensity = new Dictionary<int, int>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public int GetOccupancyCount(int cell)
	{
		if (cellOccupancyDensity.ContainsKey(cell))
		{
			return cellOccupancyDensity[cell];
		}
		return 0;
	}

	public void AddOccupancy(int cell)
	{
		if (!cellOccupancyDensity.ContainsKey(cell))
		{
			cellOccupancyDensity.Add(cell, 1);
		}
		else
		{
			Dictionary<int, int> dictionary;
			int key;
			(dictionary = cellOccupancyDensity)[key = cell] = dictionary[key] + 1;
		}
	}

	public void RemoveOccupancy(int cell)
	{
		int value = 0;
		if (cellOccupancyDensity.TryGetValue(cell, out value))
		{
			if (value == 1)
			{
				cellOccupancyDensity.Remove(cell);
			}
			else
			{
				cellOccupancyDensity[cell] = value - 1;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}
}
