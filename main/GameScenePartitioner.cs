using System;
using System.Collections.Generic;

public class GameScenePartitioner : KMonoBehaviour
{
	public interface Iterator
	{
		void Iterate(object obj);

		void Cleanup();
	}

	public ScenePartitionerLayer solidChangedLayer;

	public ScenePartitionerLayer liquidChangedLayer;

	public ScenePartitionerLayer digDestroyedLayer;

	public ScenePartitionerLayer fogOfWarChangedLayer;

	public ScenePartitionerLayer decorProviderLayer;

	public ScenePartitionerLayer attackableEntitiesLayer;

	public ScenePartitionerLayer fetchChoreLayer;

	public ScenePartitionerLayer pickupablesLayer;

	public ScenePartitionerLayer pickupablesChangedLayer;

	public ScenePartitionerLayer gasConduitsLayer;

	public ScenePartitionerLayer liquidConduitsLayer;

	public ScenePartitionerLayer solidConduitsLayer;

	public ScenePartitionerLayer wiresLayer;

	public ScenePartitionerLayer[] objectLayers;

	public ScenePartitionerLayer noisePolluterLayer;

	public ScenePartitionerLayer validNavCellChangedLayer;

	public ScenePartitionerLayer dirtyNavCellUpdateLayer;

	public ScenePartitionerLayer trapsLayer;

	public ScenePartitionerLayer floorSwitchActivatorLayer;

	public ScenePartitionerLayer floorSwitchActivatorChangedLayer;

	public ScenePartitionerLayer collisionLayer;

	public ScenePartitionerLayer lure;

	public ScenePartitionerLayer plants;

	public ScenePartitionerLayer industrialBuildings;

	private ScenePartitioner partitioner;

	private static GameScenePartitioner instance;

	private KCompactedVector<ScenePartitionerEntry> scenePartitionerEntries = new KCompactedVector<ScenePartitionerEntry>(0);

	private List<int> changedCells = new List<int>();

	public static GameScenePartitioner Instance => instance;

	protected override void OnPrefabInit()
	{
		instance = this;
		partitioner = new ScenePartitioner(16, 64, Grid.WidthInCells, Grid.HeightInCells);
		solidChangedLayer = partitioner.CreateMask("SolidChanged");
		liquidChangedLayer = partitioner.CreateMask("LiquidChanged");
		digDestroyedLayer = partitioner.CreateMask("DigDestroyed");
		fogOfWarChangedLayer = partitioner.CreateMask("FogOfWarChanged");
		decorProviderLayer = partitioner.CreateMask("DecorProviders");
		attackableEntitiesLayer = partitioner.CreateMask("FactionedEntities");
		fetchChoreLayer = partitioner.CreateMask("FetchChores");
		pickupablesLayer = partitioner.CreateMask("Pickupables");
		pickupablesChangedLayer = partitioner.CreateMask("PickupablesChanged");
		gasConduitsLayer = partitioner.CreateMask("GasConduit");
		liquidConduitsLayer = partitioner.CreateMask("LiquidConduit");
		solidConduitsLayer = partitioner.CreateMask("SolidConduit");
		noisePolluterLayer = partitioner.CreateMask("NoisePolluters");
		validNavCellChangedLayer = partitioner.CreateMask("validNavCellChangedLayer");
		dirtyNavCellUpdateLayer = partitioner.CreateMask("dirtyNavCellUpdateLayer");
		trapsLayer = partitioner.CreateMask("trapsLayer");
		floorSwitchActivatorLayer = partitioner.CreateMask("FloorSwitchActivatorLayer");
		floorSwitchActivatorChangedLayer = partitioner.CreateMask("FloorSwitchActivatorChangedLayer");
		collisionLayer = partitioner.CreateMask("Collision");
		lure = partitioner.CreateMask("Lure");
		plants = partitioner.CreateMask("Plants");
		industrialBuildings = partitioner.CreateMask("IndustrialBuildings");
		objectLayers = new ScenePartitionerLayer[37];
		for (int i = 0; i < 37; i++)
		{
			ObjectLayer objectLayer = (ObjectLayer)i;
			objectLayers[i] = partitioner.CreateMask(new HashedString(objectLayer.ToString()));
		}
	}

	protected override void OnForcedCleanUp()
	{
		instance = null;
		partitioner.FreeResources();
		partitioner = null;
		solidChangedLayer = null;
		liquidChangedLayer = null;
		digDestroyedLayer = null;
		fogOfWarChangedLayer = null;
		decorProviderLayer = null;
		attackableEntitiesLayer = null;
		fetchChoreLayer = null;
		pickupablesLayer = null;
		pickupablesChangedLayer = null;
		gasConduitsLayer = null;
		liquidConduitsLayer = null;
		solidConduitsLayer = null;
		noisePolluterLayer = null;
		validNavCellChangedLayer = null;
		dirtyNavCellUpdateLayer = null;
		trapsLayer = null;
		floorSwitchActivatorLayer = null;
		floorSwitchActivatorChangedLayer = null;
		objectLayers = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		NavGrid navGrid2 = navGrid;
		navGrid2.OnNavGridUpdateComplete = (Action<HashSet<int>>)Delegate.Combine(navGrid2.OnNavGridUpdateComplete, new Action<HashSet<int>>(OnNavGridUpdateComplete));
		NavTable navTable = navGrid.NavTable;
		navTable.OnValidCellChanged = (Action<int, NavType>)Delegate.Combine(navTable.OnValidCellChanged, new Action<int, NavType>(OnValidNavCellChanged));
	}

	public HandleVector<int>.Handle Add(string name, object obj, int x, int y, int width, int height, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		ScenePartitionerEntry scenePartitionerEntry = new ScenePartitionerEntry(name, obj, x, y, width, height, layer, partitioner, event_callback);
		partitioner.Add(scenePartitionerEntry);
		return scenePartitionerEntries.Allocate(scenePartitionerEntry);
	}

	public HandleVector<int>.Handle Add(string name, object obj, Extents extents, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		return Add(name, obj, extents.x, extents.y, extents.width, extents.height, layer, event_callback);
	}

	public HandleVector<int>.Handle Add(string name, object obj, int cell, ScenePartitionerLayer layer, Action<object> event_callback)
	{
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		return Add(name, obj, x, y, 1, 1, layer, event_callback);
	}

	public void AddGlobalLayerListener(ScenePartitionerLayer layer, Action<int, object> action)
	{
		layer.OnEvent = (Action<int, object>)Delegate.Combine(layer.OnEvent, action);
	}

	public void RemoveGlobalLayerListener(ScenePartitionerLayer layer, Action<int, object> action)
	{
		layer.OnEvent = (Action<int, object>)Delegate.Remove(layer.OnEvent, action);
	}

	public void TriggerEvent(List<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		partitioner.TriggerEvent(cells, layer, event_data);
	}

	public void TriggerEvent(HashSet<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		partitioner.TriggerEvent(cells, layer, event_data);
	}

	public void TriggerEvent(Extents extents, ScenePartitionerLayer layer, object event_data)
	{
		partitioner.TriggerEvent(extents.x, extents.y, extents.width, extents.height, layer, event_data);
	}

	public void TriggerEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		partitioner.TriggerEvent(x, y, width, height, layer, event_data);
	}

	public void TriggerEvent(int cell, ScenePartitionerLayer layer, object event_data)
	{
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		TriggerEvent(x, y, 1, 1, layer, event_data);
	}

	public void GatherEntries(Extents extents, ScenePartitionerLayer layer, List<ScenePartitionerEntry> gathered_entries)
	{
		GatherEntries(extents.x, extents.y, extents.width, extents.height, layer, gathered_entries);
	}

	public void GatherEntries(int x_bottomLeft, int y_bottomLeft, int width, int height, ScenePartitionerLayer layer, List<ScenePartitionerEntry> gathered_entries)
	{
		partitioner.GatherEntries(x_bottomLeft, y_bottomLeft, width, height, layer, null, gathered_entries);
	}

	public void Iterate<IteratorType>(int x, int y, int width, int height, ScenePartitionerLayer layer, ref IteratorType iterator) where IteratorType : Iterator
	{
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		Instance.GatherEntries(x, y, width, height, layer, pooledList);
		for (int i = 0; i < ((List<ScenePartitionerEntry>)pooledList).Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = ((List<ScenePartitionerEntry>)pooledList)[i];
			iterator.Iterate(scenePartitionerEntry.obj);
		}
		pooledList.Recycle();
	}

	public void Iterate<IteratorType>(int cell, int radius, ScenePartitionerLayer layer, ref IteratorType iterator) where IteratorType : Iterator
	{
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		Iterate(x - radius, y - radius, radius * 2, radius * 2, layer, ref iterator);
	}

	private void OnValidNavCellChanged(int cell, NavType nav_type)
	{
		changedCells.Add(cell);
	}

	private void OnNavGridUpdateComplete(HashSet<int> dirty_nav_cells)
	{
		if (dirty_nav_cells.Count > 0)
		{
			Instance.TriggerEvent(dirty_nav_cells, Instance.dirtyNavCellUpdateLayer, null);
		}
		if (changedCells.Count > 0)
		{
			Instance.TriggerEvent(changedCells, Instance.validNavCellChangedLayer, null);
			changedCells.Clear();
		}
	}

	public void UpdatePosition(HandleVector<int>.Handle handle, int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		UpdatePosition(handle, vector2I.x, vector2I.y);
	}

	public void UpdatePosition(HandleVector<int>.Handle handle, int x, int y)
	{
		if (handle.IsValid())
		{
			ScenePartitionerEntry data = scenePartitionerEntries.GetData(handle);
			data.UpdatePosition(x, y);
		}
	}

	public void Free(ref HandleVector<int>.Handle handle)
	{
		if (handle.IsValid())
		{
			ScenePartitionerEntry data = scenePartitionerEntries.GetData(handle);
			data.Release();
			scenePartitionerEntries.Free(handle);
			handle.Clear();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		partitioner.Cleanup();
	}
}
