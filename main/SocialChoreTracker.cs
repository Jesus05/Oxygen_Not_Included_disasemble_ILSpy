using System;
using UnityEngine;

public class SocialChoreTracker
{
	public Func<int, Chore> CreateChoreCB;

	public int choreCount;

	private GameObject owner;

	private CellOffset[] choreOffsets;

	private Chore[] chores;

	private HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private bool updating;

	public SocialChoreTracker(GameObject owner, CellOffset[] chore_offsets)
	{
		this.owner = owner;
		choreOffsets = chore_offsets;
		chores = new Chore[choreOffsets.Length];
		Extents extents = new Extents(Grid.PosToCell(owner), choreOffsets);
		validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("PrintingPodSocialize", owner, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, OnCellChanged);
	}

	public void Update(bool update = true)
	{
		if (!updating)
		{
			updating = true;
			int num = 0;
			for (int i = 0; i < choreOffsets.Length; i++)
			{
				CellOffset offset = choreOffsets[i];
				Chore chore = chores[i];
				if (update && num < choreCount && IsOffsetValid(offset))
				{
					num++;
					if (chore == null || chore.isComplete)
					{
						chores[i] = ((CreateChoreCB == null) ? null : CreateChoreCB(i));
					}
				}
				else if (chore != null)
				{
					chore.Cancel("locator invalidated");
					chores[i] = null;
				}
			}
			updating = false;
		}
	}

	private void OnCellChanged(object data)
	{
		if (owner.HasTag(GameTags.Operational))
		{
			Update(true);
		}
	}

	public void Clear()
	{
		GameScenePartitioner.Instance.Free(ref validNavCellChangedPartitionerEntry);
		Update(false);
	}

	private bool IsOffsetValid(CellOffset offset)
	{
		int cell = Grid.PosToCell(owner);
		int cell2 = Grid.OffsetCell(cell, offset);
		int anchor_cell = Grid.CellBelow(cell2);
		return GameNavGrids.FloorValidator.IsWalkableCell(cell2, anchor_cell, Grid.BitFields, false, false);
	}
}
