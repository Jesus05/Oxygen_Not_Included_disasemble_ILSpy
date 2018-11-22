using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimUpdater : Singleton<KBatchedAnimUpdater>
{
	public enum RegistrationState
	{
		Registered,
		PendingRemoval,
		Unregistered
	}

	private struct RegistrationInfo
	{
		public bool register;

		public int transformId;

		public int controllerInstanceId;

		public KBatchedAnimController controller;
	}

	private struct ControllerChunkInfo
	{
		public KBatchedAnimController controller;

		public Vector2I chunkXY;
	}

	private struct MovingControllerInfo
	{
		public int controllerInstanceId;

		public KBatchedAnimController controller;

		public Vector2I chunkXY;
	}

	private const int VISIBLE_BORDER = 4;

	public static readonly Vector2I INVALID_CHUNK_ID = Vector2I.minusone;

	private List<KBatchedAnimController>[,] controllerGrid = null;

	private List<KBatchedAnimController> updateList = new List<KBatchedAnimController>();

	private List<KBatchedAnimController> alwaysUpdateList = new List<KBatchedAnimController>();

	private bool[,] visibleChunkGrid;

	private bool[,] previouslyVisibleChunkGrid;

	private List<Vector2I> visibleChunks = new List<Vector2I>();

	private List<Vector2I> previouslyVisibleChunks = new List<Vector2I>();

	private Vector2I vis_chunk_min = Vector2I.zero;

	private Vector2I vis_chunk_max = Vector2I.zero;

	private List<RegistrationInfo> queuedRegistrations = new List<RegistrationInfo>();

	private Dictionary<int, ControllerChunkInfo> controllerChunkInfos = new Dictionary<int, ControllerChunkInfo>();

	private List<MovingControllerInfo> movingControllerInfos = new List<MovingControllerInfo>();

	private const int CHUNKS_TO_CLEAN_PER_TICK = 16;

	private int cleanUpChunkIndex = 0;

	private static readonly Vector2 VISIBLE_RANGE_SCALE = new Vector2(1.5f, 1.5f);

	public void InitializeGrid()
	{
		Clear();
		Vector2I visibleSize = GetVisibleSize();
		int num = (visibleSize.x + 32 - 1) / 32;
		int num2 = (visibleSize.y + 32 - 1) / 32;
		controllerGrid = new List<KBatchedAnimController>[num, num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				List<KBatchedAnimController>[,] array = controllerGrid;
				int num3 = j;
				int num4 = i;
				List<KBatchedAnimController> list = new List<KBatchedAnimController>();
				array[num3, num4] = list;
			}
		}
		visibleChunks.Clear();
		previouslyVisibleChunks.Clear();
		previouslyVisibleChunkGrid = new bool[num, num2];
		visibleChunkGrid = new bool[num, num2];
	}

	public Vector2I GetVisibleSize()
	{
		float num = (float)Grid.WidthInCells;
		Vector2 vISIBLE_RANGE_SCALE = VISIBLE_RANGE_SCALE;
		int a = (int)(num * vISIBLE_RANGE_SCALE.x);
		float num2 = (float)Grid.HeightInCells;
		Vector2 vISIBLE_RANGE_SCALE2 = VISIBLE_RANGE_SCALE;
		return new Vector2I(a, (int)(num2 * vISIBLE_RANGE_SCALE2.y));
	}

	public void Clear()
	{
		for (int i = 0; i < updateList.Count; i++)
		{
			if ((UnityEngine.Object)updateList[i] != (UnityEngine.Object)null)
			{
				UnityEngine.Object.DestroyImmediate(updateList[i]);
			}
		}
		updateList.Clear();
		for (int j = 0; j < alwaysUpdateList.Count; j++)
		{
			if ((UnityEngine.Object)alwaysUpdateList[j] != (UnityEngine.Object)null)
			{
				UnityEngine.Object.DestroyImmediate(alwaysUpdateList[j]);
			}
		}
		alwaysUpdateList.Clear();
		queuedRegistrations.Clear();
		visibleChunks.Clear();
		previouslyVisibleChunks.Clear();
		controllerGrid = null;
		previouslyVisibleChunkGrid = null;
		visibleChunkGrid = null;
	}

	public void UpdateRegister(KBatchedAnimController controller)
	{
		switch (controller.updateRegistrationState)
		{
		case RegistrationState.PendingRemoval:
			controller.updateRegistrationState = RegistrationState.Registered;
			break;
		case RegistrationState.Unregistered:
		{
			List<KBatchedAnimController> list = (controller.visibilityType != KAnimControllerBase.VisibilityType.Always) ? updateList : alwaysUpdateList;
			list.Add(controller);
			controller.updateRegistrationState = RegistrationState.Registered;
			break;
		}
		}
	}

	public void UpdateUnregister(KBatchedAnimController controller)
	{
		switch (controller.updateRegistrationState)
		{
		case RegistrationState.Registered:
			controller.updateRegistrationState = RegistrationState.PendingRemoval;
			break;
		}
	}

	public void VisibilityRegister(KBatchedAnimController controller)
	{
		queuedRegistrations.Add(new RegistrationInfo
		{
			transformId = controller.transform.GetInstanceID(),
			controllerInstanceId = controller.GetInstanceID(),
			controller = controller,
			register = true
		});
	}

	public void VisibilityUnregister(KBatchedAnimController controller)
	{
		if (!App.IsExiting)
		{
			queuedRegistrations.Add(new RegistrationInfo
			{
				transformId = controller.transform.GetInstanceID(),
				controllerInstanceId = controller.GetInstanceID(),
				controller = controller,
				register = false
			});
		}
	}

	private List<KBatchedAnimController> GetControllerList(Vector2I chunk_xy)
	{
		List<KBatchedAnimController> result = null;
		if (controllerGrid != null && 0 <= chunk_xy.x && chunk_xy.x < controllerGrid.GetLength(0) && 0 <= chunk_xy.y && chunk_xy.y < controllerGrid.GetLength(1))
		{
			result = controllerGrid[chunk_xy.x, chunk_xy.y];
		}
		return result;
	}

	public void LateUpdate()
	{
		ProcessMovingAnims();
		UpdateVisibility();
		ProcessRegistrations();
		CleanUp();
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		int count = alwaysUpdateList.Count;
		for (int i = 0; i < count; i++)
		{
			if (alwaysUpdateList[i].updateRegistrationState != 0)
			{
				alwaysUpdateList[i].updateRegistrationState = RegistrationState.Unregistered;
				alwaysUpdateList[i] = null;
			}
			else
			{
				alwaysUpdateList[i].UpdateAnim(unscaledDeltaTime);
			}
		}
		if (DoGridProcessing())
		{
			unscaledDeltaTime = Time.deltaTime;
			int count2 = updateList.Count;
			for (int j = 0; j < count2; j++)
			{
				if (updateList[j].updateRegistrationState != 0)
				{
					updateList[j].updateRegistrationState = RegistrationState.Unregistered;
					updateList[j] = null;
				}
				else
				{
					updateList[j].UpdateAnim(unscaledDeltaTime);
				}
			}
		}
	}

	public bool IsChunkVisible(Vector2I chunk_xy)
	{
		return visibleChunkGrid[chunk_xy.x, chunk_xy.y];
	}

	public void GetVisibleArea(out Vector2I vis_chunk_min, out Vector2I vis_chunk_max)
	{
		vis_chunk_min = this.vis_chunk_min;
		vis_chunk_max = this.vis_chunk_max;
	}

	public static Vector2I PosToChunkXY(Vector3 pos)
	{
		Vector2I cell_xy = Grid.PosToXY(pos);
		return KAnimBatchManager.CellXYToChunkXY(cell_xy);
	}

	private void UpdateVisibility()
	{
		if (DoGridProcessing())
		{
			GetVisibleCellRange(out Vector2I min, out Vector2I max);
			vis_chunk_min = new Vector2I(min.x / 32, min.y / 32);
			vis_chunk_max = new Vector2I(max.x / 32, max.y / 32);
			vis_chunk_max.x = Math.Min(vis_chunk_max.x, controllerGrid.GetLength(0) - 1);
			vis_chunk_max.y = Math.Min(vis_chunk_max.y, controllerGrid.GetLength(1) - 1);
			bool[,] array = previouslyVisibleChunkGrid;
			previouslyVisibleChunkGrid = visibleChunkGrid;
			visibleChunkGrid = array;
			Array.Clear(visibleChunkGrid, 0, visibleChunkGrid.Length);
			List<Vector2I> list = previouslyVisibleChunks;
			previouslyVisibleChunks = visibleChunks;
			visibleChunks = list;
			visibleChunks.Clear();
			for (int i = vis_chunk_min.y; i <= vis_chunk_max.y; i++)
			{
				for (int j = vis_chunk_min.x; j <= vis_chunk_max.x; j++)
				{
					visibleChunkGrid[j, i] = true;
					visibleChunks.Add(new Vector2I(j, i));
					if (!previouslyVisibleChunkGrid[j, i])
					{
						List<KBatchedAnimController> list2 = controllerGrid[j, i];
						for (int k = 0; k < list2.Count; k++)
						{
							KBatchedAnimController kBatchedAnimController = list2[k];
							if (!((UnityEngine.Object)kBatchedAnimController == (UnityEngine.Object)null))
							{
								kBatchedAnimController.SetVisiblity(true);
							}
						}
					}
				}
			}
			for (int l = 0; l < previouslyVisibleChunks.Count; l++)
			{
				Vector2I vector2I = previouslyVisibleChunks[l];
				if (!visibleChunkGrid[vector2I.x, vector2I.y])
				{
					List<KBatchedAnimController> list3 = controllerGrid[vector2I.x, vector2I.y];
					for (int m = 0; m < list3.Count; m++)
					{
						KBatchedAnimController kBatchedAnimController2 = list3[m];
						if (!((UnityEngine.Object)kBatchedAnimController2 == (UnityEngine.Object)null))
						{
							kBatchedAnimController2.SetVisiblity(false);
						}
					}
				}
			}
		}
	}

	private void ProcessMovingAnims()
	{
		for (int i = 0; i < movingControllerInfos.Count; i++)
		{
			MovingControllerInfo value = movingControllerInfos[i];
			if (!((UnityEngine.Object)value.controller == (UnityEngine.Object)null))
			{
				Vector2I vector2I = PosToChunkXY(value.controller.PositionIncludingOffset);
				if (value.chunkXY != vector2I)
				{
					ControllerChunkInfo value2 = default(ControllerChunkInfo);
					bool test = controllerChunkInfos.TryGetValue(value.controllerInstanceId, out value2);
					DebugUtil.Assert(test);
					DebugUtil.Assert((UnityEngine.Object)value.controller == (UnityEngine.Object)value2.controller);
					DebugUtil.Assert(value2.chunkXY == value.chunkXY);
					List<KBatchedAnimController> controllerList = GetControllerList(value2.chunkXY);
					if (controllerList != null)
					{
						DebugUtil.Assert(controllerList.Contains(value2.controller));
						controllerList.Remove(value2.controller);
					}
					controllerList = GetControllerList(vector2I);
					if (controllerList != null)
					{
						DebugUtil.Assert(!controllerList.Contains(value2.controller));
						controllerList.Add(value2.controller);
					}
					value.chunkXY = vector2I;
					movingControllerInfos[i] = value;
					value2.chunkXY = vector2I;
					controllerChunkInfos[value.controllerInstanceId] = value2;
					if (controllerList != null)
					{
						value2.controller.SetVisiblity(visibleChunkGrid[vector2I.x, vector2I.y]);
					}
					else
					{
						value2.controller.SetVisiblity(false);
					}
				}
			}
		}
	}

	private void ProcessRegistrations()
	{
		ListPool<KBatchedAnimController, KBatchedAnimUpdater>.PooledList pooledList = ListPool<KBatchedAnimController, KBatchedAnimUpdater>.Allocate();
		for (int i = 0; i < queuedRegistrations.Count; i++)
		{
			RegistrationInfo info = queuedRegistrations[i];
			if (info.register)
			{
				if (!((UnityEngine.Object)info.controller == (UnityEngine.Object)null))
				{
					int instanceID = info.controller.GetInstanceID();
					DebugUtil.Assert(!controllerChunkInfos.ContainsKey(instanceID));
					ControllerChunkInfo controllerChunkInfo = default(ControllerChunkInfo);
					controllerChunkInfo.controller = info.controller;
					controllerChunkInfo.chunkXY = PosToChunkXY(info.controller.PositionIncludingOffset);
					ControllerChunkInfo value = controllerChunkInfo;
					controllerChunkInfos[instanceID] = value;
					Singleton<CellChangeMonitor>.Instance.RegisterMovementStateChanged(info.controller.transform, OnMovementStateChanged);
					List<KBatchedAnimController> controllerList = GetControllerList(value.chunkXY);
					if (controllerList != null)
					{
						DebugUtil.Assert(!controllerList.Contains(info.controller));
						controllerList.Add(info.controller);
					}
					if (Singleton<CellChangeMonitor>.Instance.IsMoving(info.controller.transform))
					{
						movingControllerInfos.Add(new MovingControllerInfo
						{
							controllerInstanceId = instanceID,
							controller = info.controller,
							chunkXY = value.chunkXY
						});
					}
					if (controllerList != null && visibleChunkGrid[value.chunkXY.x, value.chunkXY.y])
					{
						pooledList.Add(info.controller);
					}
				}
			}
			else
			{
				ControllerChunkInfo value2 = default(ControllerChunkInfo);
				if (controllerChunkInfos.TryGetValue(info.controllerInstanceId, out value2))
				{
					if ((UnityEngine.Object)info.controller != (UnityEngine.Object)null)
					{
						List<KBatchedAnimController> controllerList2 = GetControllerList(value2.chunkXY);
						if (controllerList2 != null)
						{
							DebugUtil.Assert(controllerList2.Contains(info.controller));
							controllerList2.Remove(info.controller);
						}
					}
					movingControllerInfos.RemoveAll((MovingControllerInfo x) => x.controllerInstanceId == info.controllerInstanceId);
					Singleton<CellChangeMonitor>.Instance.UnregisterMovementStateChanged(info.transformId, OnMovementStateChanged);
					controllerChunkInfos.Remove(info.controllerInstanceId);
					pooledList.Remove(info.controller);
				}
			}
		}
		queuedRegistrations.Clear();
		foreach (KBatchedAnimController item in pooledList)
		{
			if ((UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				item.SetVisiblity(true);
			}
		}
		pooledList.Recycle();
	}

	public void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		if (!((UnityEngine.Object)transform == (UnityEngine.Object)null))
		{
			KBatchedAnimController component = transform.GetComponent<KBatchedAnimController>();
			int controller_instance_id = component.GetInstanceID();
			ControllerChunkInfo value = default(ControllerChunkInfo);
			bool test = controllerChunkInfos.TryGetValue(controller_instance_id, out value);
			DebugUtil.Assert(test);
			if (is_moving)
			{
				movingControllerInfos.Add(new MovingControllerInfo
				{
					controllerInstanceId = controller_instance_id,
					controller = component,
					chunkXY = value.chunkXY
				});
			}
			else
			{
				movingControllerInfos.RemoveAll((MovingControllerInfo x) => x.controllerInstanceId == controller_instance_id);
			}
		}
	}

	private void CleanUp()
	{
		updateList.RemoveAll((KBatchedAnimController item) => (UnityEngine.Object)item == (UnityEngine.Object)null);
		alwaysUpdateList.RemoveAll((KBatchedAnimController item) => (UnityEngine.Object)item == (UnityEngine.Object)null);
		if (DoGridProcessing())
		{
			int length = controllerGrid.GetLength(0);
			for (int i = 0; i < 16; i++)
			{
				int num = (cleanUpChunkIndex + i) % controllerGrid.Length;
				int num2 = num % length;
				int num3 = num / length;
				List<KBatchedAnimController> list = controllerGrid[num2, num3];
				list.RemoveAll((KBatchedAnimController item) => (UnityEngine.Object)item == (UnityEngine.Object)null);
			}
			cleanUpChunkIndex = (cleanUpChunkIndex + 16) % controllerGrid.Length;
		}
	}

	public static void GetVisibleCellRange(out Vector2I min, out Vector2I max)
	{
		Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
		min.x -= 4;
		min.y -= 4;
		float num = (float)Grid.WidthInCells;
		Vector2 vISIBLE_RANGE_SCALE = VISIBLE_RANGE_SCALE;
		min.x = Math.Min((int)(num * vISIBLE_RANGE_SCALE.x) - 1, Math.Max(0, min.x));
		float num2 = (float)Grid.HeightInCells;
		Vector2 vISIBLE_RANGE_SCALE2 = VISIBLE_RANGE_SCALE;
		min.y = Math.Min((int)(num2 * vISIBLE_RANGE_SCALE2.y) - 1, Math.Max(0, min.y));
		max.x += 4;
		max.y += 4;
		float num3 = (float)Grid.WidthInCells;
		Vector2 vISIBLE_RANGE_SCALE3 = VISIBLE_RANGE_SCALE;
		max.x = Math.Min((int)(num3 * vISIBLE_RANGE_SCALE3.x) - 1, Math.Max(0, max.x));
		float num4 = (float)Grid.HeightInCells;
		Vector2 vISIBLE_RANGE_SCALE4 = VISIBLE_RANGE_SCALE;
		max.y = Math.Min((int)(num4 * vISIBLE_RANGE_SCALE4.y) - 1, Math.Max(0, max.y));
	}

	private bool DoGridProcessing()
	{
		return controllerGrid != null && (UnityEngine.Object)Camera.main != (UnityEngine.Object)null;
	}
}
