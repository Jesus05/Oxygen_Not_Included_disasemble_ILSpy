using UnityEngine;

public abstract class UtilityNetworkLink : KMonoBehaviour
{
	[MyCmpGet]
	private Rotatable rotatable;

	[SerializeField]
	public CellOffset link1;

	[SerializeField]
	public CellOffset link2;

	[SerializeField]
	public bool visualizeOnly;

	private bool connected;

	private static readonly EventSystem.IntraObjectHandler<UtilityNetworkLink> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<UtilityNetworkLink>(delegate(UtilityNetworkLink component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<UtilityNetworkLink> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<UtilityNetworkLink>(delegate(UtilityNetworkLink component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		Connect();
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(774203113, OnBuildingBrokenDelegate, false);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate, false);
		Disconnect();
		base.OnCleanUp();
	}

	private void Connect()
	{
		if (!visualizeOnly && !connected)
		{
			connected = true;
			GetCells(out int linked_cell, out int linked_cell2);
			OnConnect(linked_cell, linked_cell2);
		}
	}

	protected virtual void OnConnect(int cell1, int cell2)
	{
	}

	private void Disconnect()
	{
		if (!visualizeOnly && connected)
		{
			connected = false;
			GetCells(out int linked_cell, out int linked_cell2);
			OnDisconnect(linked_cell, linked_cell2);
		}
	}

	protected virtual void OnDisconnect(int cell1, int cell2)
	{
	}

	public void GetCells(out int linked_cell1, out int linked_cell2)
	{
		Building component = GetComponent<Building>();
		if ((Object)component != (Object)null)
		{
			Orientation orientation = component.Orientation;
			int cell = Grid.PosToCell(base.transform.GetPosition());
			GetCells(cell, orientation, out linked_cell1, out linked_cell2);
		}
		else
		{
			linked_cell1 = -1;
			linked_cell2 = -1;
		}
	}

	public void GetCells(int cell, Orientation orientation, out int linked_cell1, out int linked_cell2)
	{
		CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(link1, orientation);
		CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(link2, orientation);
		linked_cell1 = Grid.OffsetCell(cell, rotatedCellOffset);
		linked_cell2 = Grid.OffsetCell(cell, rotatedCellOffset2);
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		Connect();
	}

	public int GetNetworkCell()
	{
		GetCells(out int linked_cell, out int _);
		return linked_cell;
	}
}
