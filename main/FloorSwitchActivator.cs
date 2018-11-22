public class FloorSwitchActivator : KMonoBehaviour
{
	[MyCmpReq]
	private PrimaryElement primaryElement;

	private bool registered = false;

	private HandleVector<int>.Handle partitionerEntry;

	private int last_cell_occupied = -1;

	public PrimaryElement PrimaryElement => primaryElement;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Register();
		OnCellChange();
	}

	protected override void OnCleanUp()
	{
		Unregister();
		base.OnCleanUp();
	}

	private void OnCellChange()
	{
		int num = Grid.PosToCell(this);
		GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, num);
		if (Grid.IsValidCell(last_cell_occupied) && num != last_cell_occupied)
		{
			NotifyChanged(last_cell_occupied);
		}
		NotifyChanged(num);
		last_cell_occupied = num;
	}

	private void NotifyChanged(int cell)
	{
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, this);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Register();
	}

	protected override void OnCmpDisable()
	{
		Unregister();
		base.OnCmpDisable();
	}

	private void Register()
	{
		if (!registered)
		{
			int cell = Grid.PosToCell(this);
			partitionerEntry = GameScenePartitioner.Instance.Add("FloorSwitchActivator.Register", this, cell, GameScenePartitioner.Instance.floorSwitchActivatorLayer, null);
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "FloorSwitchActivator.Register");
			registered = true;
		}
	}

	private void Unregister()
	{
		if (registered)
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
			if (last_cell_occupied > -1)
			{
				NotifyChanged(last_cell_occupied);
			}
			registered = false;
		}
	}
}
