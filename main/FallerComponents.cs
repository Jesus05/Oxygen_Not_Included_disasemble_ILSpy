using UnityEngine;

public class FallerComponents : KGameObjectComponentManager<FallerComponent>
{
	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity)
	{
		return Add(go, new FallerComponent(go.transform, initial_velocity));
	}

	public override void Remove(GameObject go)
	{
		HandleVector<int>.Handle handle = GetHandle(go);
		OnCleanUpImmediate(handle);
		CleanupInfo cleanupInfo = new CleanupInfo(go, handle);
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			cleanupList.Add(cleanupInfo);
		}
		else
		{
			InternalRemoveComponent(cleanupInfo);
		}
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle h)
	{
		FallerComponent data = GetData(h);
		Vector3 position = data.transform.GetPosition();
		int num = Grid.PosToCell(position);
		data.cellChangedCB = delegate
		{
			OnSolidChanged(h);
		};
		float num2 = 0f - GravityComponent.GetRadius(data.transform) - 0.07f;
		int num3 = Grid.PosToCell(new Vector3(position.x, position.y + num2, position.z));
		bool flag = Grid.IsValidCell(num3) && Grid.Solid[num3] && data.initialVelocity.sqrMagnitude == 0f;
		if ((Grid.IsValidCell(num) && Grid.Solid[num]) || flag)
		{
			data.solidChangedCB = delegate
			{
				OnSolidChanged(h);
			};
			int height = 2;
			Vector2I vector2I = Grid.CellToXY(num);
			vector2I.y--;
			if (vector2I.y < 0)
			{
				vector2I.y = 0;
				height = 1;
			}
			else if (vector2I.y == Grid.HeightInCells - 1)
			{
				height = 1;
			}
			data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", data.transform.gameObject, vector2I.x, vector2I.y, 1, height, GameScenePartitioner.Instance.solidChangedLayer, data.solidChangedCB);
			GameComps.Fallers.SetData(h, data);
		}
		else
		{
			GameComps.Fallers.SetData(h, data);
			AddGravity(data.transform, data.initialVelocity);
		}
	}

	protected override void OnSpawn(HandleVector<int>.Handle h)
	{
		base.OnSpawn(h);
		FallerComponent data = GetData(h);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(data.transform, data.cellChangedCB, "FallerComponent.OnSpawn");
	}

	private void OnCleanUpImmediate(HandleVector<int>.Handle h)
	{
		FallerComponent data = GetData(h);
		GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
		if (data.cellChangedCB != null)
		{
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(data.transformInstanceId, data.cellChangedCB);
			data.cellChangedCB = null;
		}
		if (GameComps.Gravities.Has(data.transform.gameObject))
		{
			GameComps.Gravities.Remove(data.transform.gameObject);
		}
		SetData(h, data);
	}

	private static void AddGravity(Transform transform, Vector2 initial_velocity)
	{
		if (!GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Add(transform.gameObject, initial_velocity, delegate
			{
				OnLanded(transform);
			});
			HandleVector<int>.Handle handle = GameComps.Fallers.GetHandle(transform.gameObject);
			FallerComponent data = GameComps.Fallers.GetData(handle);
			if (data.partitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
				GameComps.Fallers.SetData(handle, data);
			}
		}
	}

	private static void RemoveGravity(Transform transform)
	{
		if (GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Remove(transform.gameObject);
			HandleVector<int>.Handle h = GameComps.Fallers.GetHandle(transform.gameObject);
			FallerComponent data = GameComps.Fallers.GetData(h);
			int cell = Grid.PosToCell(transform.GetPosition());
			int cell2 = Grid.CellBelow(cell);
			GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
			if (Grid.IsValidCell(cell2))
			{
				data.solidChangedCB = delegate
				{
					OnSolidChanged(h);
				};
				data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", transform.gameObject, cell2, GameScenePartitioner.Instance.solidChangedLayer, data.solidChangedCB);
			}
			GameComps.Fallers.SetData(h, data);
		}
	}

	private static void OnLanded(Transform transform)
	{
		RemoveGravity(transform);
	}

	private static void OnSolidChanged(HandleVector<int>.Handle handle)
	{
		FallerComponent data = GameComps.Fallers.GetData(handle);
		if (!((Object)data.transform == (Object)null))
		{
			Vector3 position = data.transform.GetPosition();
			position.y = position.y - data.offset - 0.1f;
			int num = Grid.PosToCell(position);
			if (Grid.IsValidCell(num))
			{
				bool flag = !Grid.Solid[num];
				if (flag != data.isFalling)
				{
					data.isFalling = flag;
					if (flag)
					{
						AddGravity(data.transform, Vector2.zero);
					}
					else
					{
						RemoveGravity(data.transform);
					}
				}
			}
		}
	}
}
