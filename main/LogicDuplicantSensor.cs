using KSerialization;
using System.Collections.Generic;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicDuplicantSensor : Switch, ISim1000ms, ISim200ms
{
	[MyCmpGet]
	private KSelectable selectable;

	[MyCmpGet]
	private Rotatable rotatable;

	public int pickupRange = 4;

	private bool wasOn;

	private List<Pickupable> duplicants = new List<Pickupable>();

	private HandleVector<int>.Handle pickupablesChangedEntry;

	public static TagBits tagBits = new TagBits(new Tag[1]
	{
		GameTags.DupeBrain
	});

	private bool pickupablesDirty;

	private Extents pickupableExtents;

	private List<int> reachableCells = new List<int>(100);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		simRenderLoadBalance = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(true);
		RefreshReachableCells();
		wasOn = switchedOn;
		Vector2I vector2I = Grid.CellToXY(this.NaturalBuildingCell());
		int cell = Grid.XYToCell(vector2I.x, vector2I.y + pickupRange / 2);
		CellOffset offset = new CellOffset(0, pickupRange / 2);
		if ((bool)rotatable)
		{
			offset = rotatable.GetRotatedCellOffset(offset);
			if (Grid.IsCellOffsetValid(this.NaturalBuildingCell(), offset))
			{
				cell = Grid.OffsetCell(this.NaturalBuildingCell(), offset);
			}
		}
		pickupableExtents = new Extents(cell, pickupRange / 2);
		pickupablesChangedEntry = GameScenePartitioner.Instance.Add("DuplicantSensor.PickupablesChanged", base.gameObject, pickupableExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
		pickupablesDirty = true;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
		MinionGroupProber.Get().ReleaseProber(this);
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		RefreshReachableCells();
	}

	public void Sim200ms(float dt)
	{
		RefreshPickupables();
	}

	private void RefreshReachableCells()
	{
		ListPool<int, LogicDuplicantSensor>.PooledList pooledList = ListPool<int, LogicDuplicantSensor>.Allocate(reachableCells);
		reachableCells.Clear();
		Grid.CellToXY(this.NaturalBuildingCell(), out int x, out int y);
		int num = x - pickupRange / 2;
		for (int i = y; i < y + pickupRange + 1; i++)
		{
			for (int j = num; j < num + pickupRange + 1; j++)
			{
				int num2 = Grid.XYToCell(j, i);
				CellOffset offset = new CellOffset(j - x, i - y);
				if ((bool)rotatable)
				{
					offset = rotatable.GetRotatedCellOffset(offset);
					if (Grid.IsCellOffsetValid(this.NaturalBuildingCell(), offset))
					{
						num2 = Grid.OffsetCell(this.NaturalBuildingCell(), offset);
						Vector2I vector2I = Grid.CellToXY(num2);
						if (Grid.IsValidCell(num2) && Grid.IsPhysicallyAccessible(x, y, vector2I.x, vector2I.y, true))
						{
							reachableCells.Add(num2);
						}
					}
				}
				else if (Grid.IsValidCell(num2) && Grid.IsPhysicallyAccessible(x, y, j, i, true))
				{
					reachableCells.Add(num2);
				}
			}
		}
		pooledList.Recycle();
	}

	public bool IsCellReachable(int cell)
	{
		return reachableCells.Contains(cell);
	}

	private void RefreshPickupables()
	{
		if (pickupablesDirty)
		{
			duplicants.Clear();
			ListPool<ScenePartitionerEntry, LogicDuplicantSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicDuplicantSensor>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(pickupableExtents.x, pickupableExtents.y, pickupableExtents.width, pickupableExtents.height, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
			int cell_a = Grid.PosToCell(this);
			for (int i = 0; i < pooledList.Count; i++)
			{
				Pickupable pickupable = pooledList[i].obj as Pickupable;
				int pickupableCell = GetPickupableCell(pickupable);
				int cellRange = Grid.GetCellRange(cell_a, pickupableCell);
				if (IsPickupableRelevantToMyInterestsAndReachable(pickupable) && cellRange <= pickupRange)
				{
					duplicants.Add(pickupable);
				}
			}
			SetState(duplicants.Count > 0);
			pickupablesDirty = false;
		}
	}

	private void OnPickupablesChanged(object data)
	{
		Pickupable pickupable = data as Pickupable;
		if ((bool)pickupable && IsPickupableRelevantToMyInterests(pickupable))
		{
			pickupablesDirty = true;
		}
	}

	private bool IsPickupableRelevantToMyInterests(Pickupable pickupable)
	{
		KPrefabID kPrefabID = pickupable.KPrefabID;
		if (!kPrefabID.HasAnyTags(ref tagBits))
		{
			return false;
		}
		return true;
	}

	private bool IsPickupableRelevantToMyInterestsAndReachable(Pickupable pickupable)
	{
		if (!IsPickupableRelevantToMyInterests(pickupable))
		{
			return false;
		}
		int pickupableCell = GetPickupableCell(pickupable);
		if (!IsCellReachable(pickupableCell))
		{
			return false;
		}
		return true;
	}

	private int GetPickupableCell(Pickupable pickupable)
	{
		return pickupable.cachedCell;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		component.SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play((!switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
		}
	}
}
