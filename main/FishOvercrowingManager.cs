using System.Collections.Generic;

public class FishOvercrowingManager : KMonoBehaviour, ISim1000ms
{
	private struct Cell
	{
		public int version;

		public int cavityId;
	}

	private struct FishInfo
	{
		public int cell;

		public FishOvercrowdingMonitor.Instance fish;
	}

	private struct CavityInfo
	{
		public int fishCount;

		public int cellCount;
	}

	public static FishOvercrowingManager Instance;

	private List<FishOvercrowdingMonitor.Instance> fishes = new List<FishOvercrowdingMonitor.Instance>();

	private Dictionary<int, CavityInfo> cavityIdToCavityInfo = new Dictionary<int, CavityInfo>();

	private Dictionary<int, int> cellToFishCount = new Dictionary<int, int>();

	private Cell[] cells;

	private int versionCounter = 1;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		cells = new Cell[Grid.CellCount];
	}

	public void Add(FishOvercrowdingMonitor.Instance fish)
	{
		fishes.Add(fish);
	}

	public void Remove(FishOvercrowdingMonitor.Instance fish)
	{
		fishes.Remove(fish);
	}

	public void Sim1000ms(float dt)
	{
		int num = versionCounter++;
		int num2 = 1;
		cavityIdToCavityInfo.Clear();
		cellToFishCount.Clear();
		ListPool<FishInfo, FishOvercrowingManager>.PooledList pooledList = ListPool<FishInfo, FishOvercrowingManager>.Allocate();
		foreach (FishOvercrowdingMonitor.Instance fish in fishes)
		{
			int num3 = Grid.PosToCell(fish);
			if (Grid.IsValidCell(num3))
			{
				FishInfo fishInfo = default(FishInfo);
				fishInfo.cell = num3;
				fishInfo.fish = fish;
				FishInfo item = fishInfo;
				pooledList.Add(item);
				int value = 0;
				cellToFishCount.TryGetValue(num3, out value);
				value++;
				cellToFishCount[num3] = value;
			}
		}
		foreach (FishInfo item2 in pooledList)
		{
			FishInfo current2 = item2;
			ListPool<int, FishOvercrowingManager>.PooledList pooledList2 = ListPool<int, FishOvercrowingManager>.Allocate();
			pooledList2.Add(current2.cell);
			int num4 = 0;
			int num6 = num2++;
			while (num4 < pooledList2.Count)
			{
				int num8 = pooledList2[num4++];
				if (Grid.IsValidCell(num8))
				{
					Cell cell = cells[num8];
					if (cell.version != num && Grid.IsLiquid(num8))
					{
						cell.cavityId = num6;
						cell.version = num;
						int value2 = 0;
						cellToFishCount.TryGetValue(num8, out value2);
						CavityInfo value3 = default(CavityInfo);
						if (!cavityIdToCavityInfo.TryGetValue(num6, out value3))
						{
							value3 = default(CavityInfo);
						}
						value3.fishCount += value2;
						value3.cellCount++;
						cavityIdToCavityInfo[num6] = value3;
						pooledList2.Add(Grid.CellLeft(num8));
						pooledList2.Add(Grid.CellRight(num8));
						pooledList2.Add(Grid.CellAbove(num8));
						pooledList2.Add(Grid.CellBelow(num8));
						cells[num8] = cell;
					}
				}
			}
			pooledList2.Recycle();
		}
		foreach (FishInfo item3 in pooledList)
		{
			FishInfo current3 = item3;
			Cell cell2 = cells[current3.cell];
			CavityInfo value4 = default(CavityInfo);
			cavityIdToCavityInfo.TryGetValue(cell2.cavityId, out value4);
			current3.fish.SetOvercrowdingInfo(value4.cellCount, value4.fishCount);
		}
		pooledList.Recycle();
	}
}
