using System;
using System.Collections.Generic;

public static class OffsetGroups
{
	private class CellOffsetComparer : IComparer<CellOffset>
	{
		public int Compare(CellOffset a, CellOffset b)
		{
			int num = Math.Abs(a.x) + Math.Abs(a.y);
			int value = Math.Abs(b.x) + Math.Abs(b.y);
			return num.CompareTo(value);
		}
	}

	public static CellOffset[] Use = new CellOffset[1]
	{
		default(CellOffset)
	};

	public static CellOffset[] Chat = new CellOffset[6]
	{
		new CellOffset(1, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(-1, -1)
	};

	public static CellOffset[] LeftOnly = new CellOffset[1]
	{
		new CellOffset(-1, 0)
	};

	public static CellOffset[] RightOnly = new CellOffset[1]
	{
		new CellOffset(1, 0)
	};

	public static CellOffset[] Standard = InitGrid(-2, 2, -3, 3);

	public static CellOffset[] LiquidSource = new CellOffset[11]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(-1, 0),
		new CellOffset(0, 1),
		new CellOffset(0, -1),
		new CellOffset(1, 1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(-1, -1),
		new CellOffset(2, 0),
		new CellOffset(-2, 0)
	};

	public static CellOffset[][] InvertedStandardTable = OffsetTable.Mirror(new CellOffset[28][]
	{
		new CellOffset[1]
		{
			new CellOffset(0, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(0, 3),
			new CellOffset(0, 1),
			new CellOffset(0, 2)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(0, -3),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 0)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(1, 0),
			new CellOffset(1, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(0, 1),
			new CellOffset(0, 2)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(0, 1),
			new CellOffset(0, 2),
			new CellOffset(0, 3)
		},
		new CellOffset[1]
		{
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, 0),
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(0, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -3),
			new CellOffset(1, 0),
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -3),
			new CellOffset(0, -1),
			new CellOffset(0, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -3),
			new CellOffset(0, -1),
			new CellOffset(-1, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 3),
			new CellOffset(1, 1),
			new CellOffset(1, 2),
			new CellOffset(1, 3)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -1),
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -2),
			new CellOffset(1, 0),
			new CellOffset(1, -1),
			new CellOffset(2, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -3),
			new CellOffset(1, 0),
			new CellOffset(1, -1),
			new CellOffset(1, -2)
		}
	});

	public static CellOffset[][] InvertedStandardTableWithCorners = OffsetTable.Mirror(new CellOffset[26][]
	{
		new CellOffset[1]
		{
			new CellOffset(0, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(0, 3),
			new CellOffset(0, 1),
			new CellOffset(0, 2)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(0, -3),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(1, 0),
			new CellOffset(1, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(0, 1),
			new CellOffset(0, 2)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(0, 1),
			new CellOffset(0, 2),
			new CellOffset(0, 3)
		},
		new CellOffset[1]
		{
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, 0),
			new CellOffset(1, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, -3),
			new CellOffset(1, 0),
			new CellOffset(1, -1),
			new CellOffset(1, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 3),
			new CellOffset(1, 1),
			new CellOffset(1, 2),
			new CellOffset(1, 3)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -1),
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -2),
			new CellOffset(1, 0),
			new CellOffset(1, -1),
			new CellOffset(2, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -3),
			new CellOffset(1, 0),
			new CellOffset(1, -1),
			new CellOffset(1, -2)
		}
	});

	private static Dictionary<CellOffset[], Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>> reachabilityTableCache = new Dictionary<CellOffset[], Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>>();

	private static readonly CellOffset[] nullFilter = new CellOffset[0];

	public static CellOffset[] InitGrid(int x0, int x1, int y0, int y1)
	{
		List<CellOffset> list = new List<CellOffset>();
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				list.Add(new CellOffset(j, i));
			}
		}
		CellOffset[] array = list.ToArray();
		Array.Sort(array, 0, array.Length, new CellOffsetComparer());
		return array;
	}

	public static CellOffset[][] BuildReachabilityTable(CellOffset[] area_offsets, CellOffset[][] table, CellOffset[] filter)
	{
		Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>> value = null;
		Dictionary<CellOffset[], CellOffset[][]> value2 = null;
		CellOffset[][] value3 = null;
		if (reachabilityTableCache.TryGetValue(area_offsets, out value) && value.TryGetValue(table, out value2) && value2.TryGetValue((filter != null) ? filter : nullFilter, out value3))
		{
			return value3;
		}
		HashSet<CellOffset> hashSet = new HashSet<CellOffset>();
		foreach (CellOffset a in area_offsets)
		{
			foreach (CellOffset[] array in table)
			{
				if (filter == null || Array.IndexOf(filter, array[0]) == -1)
				{
					CellOffset item = a + array[0];
					hashSet.Add(item);
				}
			}
		}
		List<CellOffset[]> list = new List<CellOffset[]>();
		foreach (CellOffset item2 in hashSet)
		{
			CellOffset b = area_offsets[0];
			foreach (CellOffset cellOffset in area_offsets)
			{
				if ((item2 - b).GetOffsetDistance() > (item2 - cellOffset).GetOffsetDistance())
				{
					b = cellOffset;
				}
			}
			foreach (CellOffset[] array2 in table)
			{
				if ((filter == null || Array.IndexOf(filter, array2[0]) == -1) && array2[0] + b == item2)
				{
					CellOffset[] array3 = new CellOffset[array2.Length];
					for (int m = 0; m < array2.Length; m++)
					{
						array3[m] = array2[m] + b;
					}
					list.Add(array3);
				}
			}
		}
		value3 = list.ToArray();
		Array.Sort(value3, (CellOffset[] x, CellOffset[] y) => x[0].GetOffsetDistance().CompareTo(y[0].GetOffsetDistance()));
		if (value == null)
		{
			value = new Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>();
			reachabilityTableCache.Add(area_offsets, value);
		}
		if (value2 == null)
		{
			value2 = new Dictionary<CellOffset[], CellOffset[][]>();
			value.Add(table, value2);
		}
		value2.Add((filter != null) ? filter : nullFilter, value3);
		return value3;
	}
}
