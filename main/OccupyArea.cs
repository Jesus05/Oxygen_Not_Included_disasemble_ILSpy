using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[SkipSaveFileSerialization]
public class OccupyArea : KMonoBehaviour
{
	public CellOffset[] OccupiedCellsOffsets;

	private CellOffset[] AboveOccupiedCellOffsets;

	private int[] occupiedGridCells;

	public ObjectLayer[] objectLayers = new ObjectLayer[0];

	[SerializeField]
	private bool applyToCells = true;

	public bool ApplyToCells
	{
		get
		{
			return applyToCells;
		}
		set
		{
			if (value != applyToCells)
			{
				if (value)
				{
					UpdateOccupiedArea();
				}
				else
				{
					ClearOccupiedArea();
				}
				applyToCells = value;
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (applyToCells)
		{
			UpdateOccupiedArea();
		}
	}

	private void ValidatePosition()
	{
		int cell = Grid.PosToCell(this);
		if (!Grid.IsValidCell(cell))
		{
			Debug.LogWarning(base.name + " is outside the grid! DELETING!", null);
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		ValidatePosition();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		ValidatePosition();
	}

	public void SetCellOffsets(CellOffset[] cells)
	{
		OccupiedCellsOffsets = cells;
	}

	public bool CheckIsOccupying(int checkCell)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (checkCell == num)
		{
			return true;
		}
		CellOffset[] occupiedCellsOffsets = OccupiedCellsOffsets;
		foreach (CellOffset offset in occupiedCellsOffsets)
		{
			if (Grid.OffsetCell(num, offset) == checkCell)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		ClearOccupiedArea();
	}

	private void ClearOccupiedArea()
	{
		if (occupiedGridCells != null)
		{
			ObjectLayer[] array = objectLayers;
			foreach (ObjectLayer objectLayer in array)
			{
				if (objectLayer != ObjectLayer.NumLayers)
				{
					int[] array2 = occupiedGridCells;
					foreach (int cell in array2)
					{
						if ((UnityEngine.Object)Grid.Objects[cell, (int)objectLayer] == (UnityEngine.Object)base.gameObject)
						{
							Grid.Objects[cell, (int)objectLayer] = null;
						}
					}
				}
			}
		}
	}

	public void UpdateOccupiedArea()
	{
		if (objectLayers.Length != 0)
		{
			if (occupiedGridCells == null)
			{
				occupiedGridCells = new int[OccupiedCellsOffsets.Length];
			}
			ClearOccupiedArea();
			int cell = Grid.PosToCell(base.gameObject);
			ObjectLayer[] array = objectLayers;
			foreach (ObjectLayer objectLayer in array)
			{
				if (objectLayer != ObjectLayer.NumLayers)
				{
					for (int j = 0; j < OccupiedCellsOffsets.Length; j++)
					{
						CellOffset offset = OccupiedCellsOffsets[j];
						int num = Grid.OffsetCell(cell, offset);
						Grid.Objects[num, (int)objectLayer] = base.gameObject;
						occupiedGridCells[j] = num;
					}
				}
			}
		}
	}

	public int GetWidthInCells()
	{
		int num = 2147483647;
		int num2 = -2147483648;
		CellOffset[] occupiedCellsOffsets = OccupiedCellsOffsets;
		for (int i = 0; i < occupiedCellsOffsets.Length; i++)
		{
			CellOffset cellOffset = occupiedCellsOffsets[i];
			num = Math.Min(num, cellOffset.x);
			num2 = Math.Max(num2, cellOffset.x);
		}
		return num2 - num + 1;
	}

	public int GetHeightInCells()
	{
		int num = 2147483647;
		int num2 = -2147483648;
		CellOffset[] occupiedCellsOffsets = OccupiedCellsOffsets;
		for (int i = 0; i < occupiedCellsOffsets.Length; i++)
		{
			CellOffset cellOffset = occupiedCellsOffsets[i];
			num = Math.Min(num, cellOffset.y);
			num2 = Math.Max(num2, cellOffset.y);
		}
		return num2 - num + 1;
	}

	public Extents GetExtents()
	{
		return new Extents(Grid.PosToCell(base.gameObject), OccupiedCellsOffsets);
	}

	public Extents GetExtents(Orientation orientation)
	{
		return new Extents(Grid.PosToCell(base.gameObject), OccupiedCellsOffsets, orientation);
	}

	private void OnDrawGizmosSelected()
	{
		int cell = Grid.PosToCell(base.gameObject);
		if (OccupiedCellsOffsets != null)
		{
			CellOffset[] occupiedCellsOffsets = OccupiedCellsOffsets;
			foreach (CellOffset offset in occupiedCellsOffsets)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(Grid.CellToPos(Grid.OffsetCell(cell, offset)) + Vector3.right / 2f + Vector3.up / 2f, Vector3.one);
			}
		}
		if (AboveOccupiedCellOffsets != null)
		{
			CellOffset[] aboveOccupiedCellOffsets = AboveOccupiedCellOffsets;
			foreach (CellOffset offset2 in aboveOccupiedCellOffsets)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube(Grid.CellToPos(Grid.OffsetCell(cell, offset2)) + Vector3.right / 2f + Vector3.up / 2f, Vector3.one * 0.9f);
			}
		}
	}

	public bool CanOccupyArea(int rootCell, ObjectLayer layer)
	{
		for (int i = 0; i < OccupiedCellsOffsets.Length; i++)
		{
			CellOffset offset = OccupiedCellsOffsets[i];
			int cell = Grid.OffsetCell(rootCell, offset);
			if ((UnityEngine.Object)Grid.Objects[cell, (int)layer] != (UnityEngine.Object)null)
			{
				return false;
			}
		}
		return true;
	}

	public bool TestArea(int rootCell, object data, Func<int, object, bool> testDelegate)
	{
		for (int i = 0; i < OccupiedCellsOffsets.Length; i++)
		{
			CellOffset offset = OccupiedCellsOffsets[i];
			int arg = Grid.OffsetCell(rootCell, offset);
			if (!testDelegate(arg, data))
			{
				return false;
			}
		}
		return true;
	}

	public bool TestAreaAbove(int rootCell, object data, Func<int, object, bool> testDelegate)
	{
		if (AboveOccupiedCellOffsets == null)
		{
			List<CellOffset> list = new List<CellOffset>();
			for (int i = 0; i < OccupiedCellsOffsets.Length; i++)
			{
				CellOffset cellOffset = new CellOffset(OccupiedCellsOffsets[i].x, OccupiedCellsOffsets[i].y + 1);
				if (Array.IndexOf(OccupiedCellsOffsets, cellOffset) == -1)
				{
					list.Add(cellOffset);
				}
			}
			AboveOccupiedCellOffsets = list.ToArray();
		}
		for (int j = 0; j < AboveOccupiedCellOffsets.Length; j++)
		{
			int arg = Grid.OffsetCell(rootCell, AboveOccupiedCellOffsets[j]);
			if (!testDelegate(arg, data))
			{
				return false;
			}
		}
		return true;
	}
}
