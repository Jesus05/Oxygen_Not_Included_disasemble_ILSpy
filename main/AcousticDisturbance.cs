using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AcousticDisturbance
{
	private static readonly HashedString[] PreAnims = new HashedString[2]
	{
		"grid_pre",
		"grid_loop"
	};

	private static readonly HashedString PostAnim = "grid_pst";

	private static float distanceDelay = 0.25f;

	private static float duration = 3f;

	private static HashSet<int> cellsInRange = new HashSet<int>();

	[CompilerGenerated]
	private static Action<object> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<object> _003C_003Ef__mg_0024cache1;

	public static void Emit(object data, int EmissionRadius)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 vector = gameObject.transform.GetPosition();
		int num = Grid.PosToCell(vector);
		int num2 = EmissionRadius * EmissionRadius;
		int max_depth = Mathf.CeilToInt((float)EmissionRadius);
		DetermineCellsInRadius(num, 0, max_depth, cellsInRange);
		DrawVisualEffect(num, cellsInRange);
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if ((UnityEngine.Object)minionIdentity.gameObject != (UnityEngine.Object)gameObject.gameObject)
			{
				Vector2 vector2 = minionIdentity.transform.GetPosition();
				float num3 = Vector2.SqrMagnitude(vector - vector2);
				if (num3 <= (float)num2)
				{
					int item = Grid.PosToCell(vector2);
					if (cellsInRange.Contains(item) && minionIdentity.GetSMI<StaminaMonitor.Instance>().IsSleeping())
					{
						minionIdentity.Trigger(-527751701, data);
						minionIdentity.Trigger(1621815900, data);
					}
				}
			}
		}
		cellsInRange.Clear();
	}

	private static void DrawVisualEffect(int center_cell, HashSet<int> cells)
	{
		SoundEvent.PlayOneShot(GlobalResources.Instance().AcousticDisturbanceSound, Grid.CellToPos(center_cell));
		foreach (int cell in cells)
		{
			int gridDistance = GetGridDistance(cell, center_cell);
			GameScheduler.Instance.Schedule("radialgrid_pre", distanceDelay * (float)gridDistance, SpawnEffect, cell, null);
		}
	}

	private static void SpawnEffect(object data)
	{
		Grid.SceneLayer layer = Grid.SceneLayer.InteriorWall;
		int cell = (int)data;
		KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("radialgrid_kanim", Grid.CellToPosCCC(cell, layer), null, false, layer, false);
		kBatchedAnimController.destroyOnAnimComplete = false;
		kBatchedAnimController.Play(PreAnims, KAnim.PlayMode.Loop);
		GameScheduler.Instance.Schedule("radialgrid_loop", duration, DestroyEffect, kBatchedAnimController, null);
	}

	private static void DestroyEffect(object data)
	{
		KBatchedAnimController kBatchedAnimController = (KBatchedAnimController)data;
		kBatchedAnimController.destroyOnAnimComplete = true;
		kBatchedAnimController.Play(PostAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetGridDistance(int cell, int center_cell)
	{
		Vector2I u = Grid.CellToXY(cell);
		Vector2I v = Grid.CellToXY(center_cell);
		Vector2I vector2I = u - v;
		return Math.Abs(vector2I.x) + Math.Abs(vector2I.y);
	}

	private static void DetermineCellsInRadius(int cell, int depth, int max_depth, HashSet<int> cells_in_range)
	{
		if (Grid.IsValidCell(cell) && !Grid.Solid[cell])
		{
			cells_in_range.Add(cell);
			if (depth < max_depth)
			{
				int depth2 = depth + 1;
				int num = Grid.CellBelow(cell);
				int num2 = Grid.CellAbove(cell);
				int num3 = cell - 1;
				int num4 = cell + 1;
				bool flag = Grid.IsValidCell(num) && !Grid.Solid[num];
				bool flag2 = Grid.IsValidCell(num2) && !Grid.Solid[num2];
				bool flag3 = Grid.IsValidCell(num3) && !Grid.Solid[num3];
				bool flag4 = Grid.IsValidCell(num4) && !Grid.Solid[num4];
				if (flag || flag3)
				{
					DetermineCellsInRadius(num - 1, depth2, max_depth, cells_in_range);
				}
				DetermineCellsInRadius(num, depth2, max_depth, cells_in_range);
				if (flag || flag4)
				{
					DetermineCellsInRadius(num + 1, depth2, max_depth, cells_in_range);
				}
				DetermineCellsInRadius(num3, depth2, max_depth, cells_in_range);
				DetermineCellsInRadius(num4, depth2, max_depth, cells_in_range);
				if (flag2 || flag3)
				{
					DetermineCellsInRadius(num2 - 1, depth2, max_depth, cells_in_range);
				}
				DetermineCellsInRadius(num2, depth2, max_depth, cellsInRange);
				if (flag2 || flag4)
				{
					DetermineCellsInRadius(num2 + 1, depth2, max_depth, cells_in_range);
				}
			}
		}
	}
}
