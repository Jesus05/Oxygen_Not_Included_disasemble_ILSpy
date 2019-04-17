using System.Collections.Generic;

public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
	private static MinionGroupProber Instance;

	private Dictionary<object, int>[] cells;

	private Dictionary<object, KeyValuePair<int, int>> valid_serial_nos = new Dictionary<object, KeyValuePair<int, int>>();

	private List<object> pending_removals = new List<object>();

	private readonly object access = new object();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public static MinionGroupProber Get()
	{
		return Instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		cells = new Dictionary<object, int>[Grid.CellCount];
	}

	public bool IsReachable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		Dictionary<object, int> dictionary = cells[cell];
		if (dictionary == null)
		{
			return false;
		}
		bool result = false;
		lock (access)
		{
			pending_removals.Clear();
			foreach (KeyValuePair<object, int> item in dictionary)
			{
				object key = item.Key;
				int value = item.Value;
				if (valid_serial_nos.TryGetValue(key, out KeyValuePair<int, int> value2) && (value == value2.Key || value == value2.Value))
				{
					result = true;
					break;
				}
				pending_removals.Add(key);
			}
			foreach (object pending_removal in pending_removals)
			{
				dictionary.Remove(pending_removal);
				if (dictionary.Count == 0)
				{
					cells[cell] = null;
				}
			}
			return result;
		}
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		foreach (CellOffset offset in offsets)
		{
			if (IsReachable(Grid.OffsetCell(cell, offset)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReachable(Workable workable)
	{
		return IsReachable(Grid.PosToCell(workable), workable.GetOffsets());
	}

	public void Occupy(object prober, int serial_no, List<int> cells)
	{
		lock (access)
		{
			foreach (int cell in cells)
			{
				if (this.cells[cell] == null)
				{
					this.cells[cell] = new Dictionary<object, int>();
				}
				this.cells[cell][prober] = serial_no;
			}
		}
	}

	public void SetValidSerialNos(object prober, int previous_serial_no, int serial_no)
	{
		lock (access)
		{
			valid_serial_nos[prober] = new KeyValuePair<int, int>(previous_serial_no, serial_no);
		}
	}

	public bool ReleaseProber(object prober)
	{
		lock (access)
		{
			return valid_serial_nos.Remove(prober);
		}
	}
}
