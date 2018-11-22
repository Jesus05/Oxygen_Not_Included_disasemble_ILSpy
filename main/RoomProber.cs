using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomProber : ISim1000ms
{
	public class Tuning : TuningData<Tuning>
	{
		public int maxRoomSize;
	}

	private class CavityFloodFiller
	{
		private HandleVector<int>.Handle[] grid;

		private HandleVector<int>.Handle cavityID;

		private int numCells;

		private int minX;

		private int minY;

		private int maxX;

		private int maxY;

		public int NumCells => numCells;

		public int MinX => minX;

		public int MinY => minY;

		public int MaxX => maxX;

		public int MaxY => maxY;

		public CavityFloodFiller(HandleVector<int>.Handle[] grid)
		{
			this.grid = grid;
		}

		public void Reset(HandleVector<int>.Handle search_id)
		{
			cavityID = search_id;
			numCells = 0;
			minX = 2147483647;
			minY = 2147483647;
			maxX = 0;
			maxY = 0;
		}

		private static bool IsWall(int cell)
		{
			return Grid.Solid[cell] || Grid.HasDoor[cell] || Grid.Foundation[cell];
		}

		public bool ShouldContinue(int flood_cell)
		{
			bool result = false;
			if (!IsWall(flood_cell))
			{
				result = true;
				grid[flood_cell] = cavityID;
				int x = 0;
				int y = 0;
				Grid.CellToXY(flood_cell, out x, out y);
				minX = Math.Min(x, minX);
				minY = Math.Min(y, minY);
				maxX = Math.Max(x, maxX);
				maxY = Math.Max(y, maxY);
				numCells++;
			}
			else
			{
				grid[flood_cell] = HandleVector<int>.InvalidHandle;
			}
			return result;
		}
	}

	public List<Room> rooms = new List<Room>();

	private KCompactedVector<CavityInfo> cavityInfos = new KCompactedVector<CavityInfo>(1024);

	private HandleVector<int>.Handle[] CellCavityID;

	private bool dirty = true;

	private HashSet<int> solidChanges = new HashSet<int>();

	private HashSet<int> visitedCells = new HashSet<int>();

	private HashSet<int> floodFillSet = new HashSet<int>();

	private HashSet<HandleVector<int>.Handle> releasedIDs = new HashSet<HandleVector<int>.Handle>();

	private CavityFloodFiller floodFiller;

	public RoomProber()
	{
		CellCavityID = new HandleVector<int>.Handle[Grid.CellCount];
		floodFiller = new CavityFloodFiller(CellCavityID);
		for (int i = 0; i < CellCavityID.Length; i++)
		{
			solidChanges.Add(i);
		}
		ProcessSolidChanges();
		RefreshRooms();
		World instance = World.Instance;
		instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(SolidChangedEvent));
		GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], OnBuildingsChanged);
	}

	private void SolidChangedEvent(int cell)
	{
		SolidChangedEvent(cell, true);
	}

	private void OnBuildingsChanged(int cell, object building)
	{
		CavityInfo cavityForCell = GetCavityForCell(cell);
		if (cavityForCell != null)
		{
			solidChanges.Add(cell);
			dirty = true;
		}
	}

	public void SolidChangedEvent(int cell, bool ignoreDoors)
	{
		if (!ignoreDoors || !Grid.HasDoor[cell])
		{
			solidChanges.Add(cell);
			dirty = true;
		}
	}

	private CavityInfo CreateNewCavity()
	{
		CavityInfo cavityInfo = new CavityInfo();
		cavityInfo.handle = cavityInfos.Allocate(cavityInfo);
		return cavityInfo;
	}

	private unsafe void ProcessSolidChanges()
	{
		int* ptr = stackalloc int[5];
		*ptr = 0;
		ptr[1] = -Grid.WidthInCells;
		ptr[2] = -1;
		ptr[3] = 1;
		ptr[4] = Grid.WidthInCells;
		foreach (int solidChange in solidChanges)
		{
			for (int i = 0; i < 5; i++)
			{
				int num = solidChange + ptr[i];
				if (Grid.IsValidCell(num))
				{
					floodFillSet.Add(num);
					HandleVector<int>.Handle item = CellCavityID[num];
					if (item.IsValid())
					{
						CellCavityID[num] = HandleVector<int>.InvalidHandle;
						releasedIDs.Add(item);
					}
				}
			}
		}
		CavityInfo cavityInfo = CreateNewCavity();
		foreach (int item2 in floodFillSet)
		{
			if (!visitedCells.Contains(item2))
			{
				HandleVector<int>.Handle handle = CellCavityID[item2];
				if (!handle.IsValid())
				{
					CavityInfo cavityInfo2 = cavityInfo;
					floodFiller.Reset(cavityInfo2.handle);
					GameUtil.FloodFillConditional(item2, floodFiller.ShouldContinue, visitedCells, null);
					if (floodFiller.NumCells > 0)
					{
						cavityInfo2.numCells = floodFiller.NumCells;
						cavityInfo2.minX = floodFiller.MinX;
						cavityInfo2.minY = floodFiller.MinY;
						cavityInfo2.maxX = floodFiller.MaxX;
						cavityInfo2.maxY = floodFiller.MaxY;
						cavityInfo = CreateNewCavity();
					}
				}
			}
		}
		if (cavityInfo.numCells == 0)
		{
			releasedIDs.Add(cavityInfo.handle);
		}
		foreach (HandleVector<int>.Handle releasedID in releasedIDs)
		{
			CavityInfo data = cavityInfos.GetData(releasedID);
			if (data.room != null)
			{
				ClearRoom(data.room);
			}
			cavityInfos.Free(releasedID);
		}
		RebuildDirtyCavities(visitedCells);
		releasedIDs.Clear();
		visitedCells.Clear();
		solidChanges.Clear();
		floodFillSet.Clear();
	}

	private void RebuildDirtyCavities(ICollection<int> visited_cells)
	{
		int maxRoomSize = TuningData<Tuning>.Get().maxRoomSize;
		foreach (int visited_cell in visited_cells)
		{
			HandleVector<int>.Handle handle = CellCavityID[visited_cell];
			if (handle.IsValid())
			{
				CavityInfo data = cavityInfos.GetData(handle);
				if (0 < data.numCells && data.numCells <= maxRoomSize)
				{
					GameObject gameObject = Grid.Objects[visited_cell, 1];
					if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
					{
						KPrefabID component = gameObject.GetComponent<KPrefabID>();
						bool flag = false;
						foreach (KPrefabID building in data.buildings)
						{
							if (component.InstanceID == building.InstanceID)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							data.AddBuilding(component);
						}
					}
				}
			}
		}
		visited_cells.Clear();
	}

	public void Sim1000ms(float dt)
	{
		if (dirty)
		{
			ProcessSolidChanges();
			RefreshRooms();
		}
	}

	private void CreateRoom(CavityInfo cavity)
	{
		Room room = new Room();
		room.cavity = cavity;
		cavity.room = room;
		rooms.Add(room);
		room.roomType = Db.Get().RoomTypes.GetRoomType(room);
		AssignBuildingsToRoom(room);
	}

	private void ClearRoom(Room room)
	{
		UnassignBuildingsToRoom(room);
		room.CleanUp();
		rooms.Remove(room);
	}

	private void RefreshRooms()
	{
		int maxRoomSize = TuningData<Tuning>.Get().maxRoomSize;
		foreach (CavityInfo data in cavityInfos.GetDataList())
		{
			if (data.dirty)
			{
				if (data.numCells > 0)
				{
					if (data.numCells <= maxRoomSize)
					{
						CreateRoom(data);
					}
					foreach (KPrefabID building in data.buildings)
					{
						building.Trigger(144050788, data.room);
					}
				}
				data.dirty = false;
			}
		}
		dirty = false;
	}

	private void AssignBuildingsToRoom(Room room)
	{
		RoomType roomType = room.roomType;
		if (roomType != Db.Get().RoomTypes.Neutral)
		{
			foreach (KPrefabID building in room.buildings)
			{
				Assignable component = building.GetComponent<Assignable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && (roomType.primary_constraint == null || !roomType.primary_constraint.building_criteria(building.GetComponent<KPrefabID>())))
				{
					component.Assign(room);
				}
			}
		}
	}

	private void UnassignBuildingsToRoom(Room room)
	{
		foreach (KPrefabID building in room.buildings)
		{
			if (!((UnityEngine.Object)building == (UnityEngine.Object)null))
			{
				building.Trigger(144050788, null);
				Assignable component = building.GetComponent<Assignable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.assignee == room)
				{
					component.Unassign();
				}
			}
		}
	}

	public Room GetRoomOfGameObject(GameObject go)
	{
		if (!((UnityEngine.Object)go == (UnityEngine.Object)null))
		{
			int cell = Grid.PosToCell(go);
			if (Grid.IsValidCell(cell))
			{
				return GetCavityForCell(cell)?.room;
			}
			return null;
		}
		return null;
	}

	public bool IsInRoomType(GameObject go, RoomType checkType)
	{
		Room roomOfGameObject = GetRoomOfGameObject(go);
		if (roomOfGameObject == null)
		{
			return false;
		}
		RoomType roomType = roomOfGameObject.roomType;
		return checkType == roomType;
	}

	private CavityInfo GetCavityInfo(HandleVector<int>.Handle id)
	{
		CavityInfo result = null;
		if (id.IsValid())
		{
			result = cavityInfos.GetData(id);
		}
		return result;
	}

	public CavityInfo GetCavityForCell(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			HandleVector<int>.Handle id = CellCavityID[cell];
			return GetCavityInfo(id);
		}
		return null;
	}
}
