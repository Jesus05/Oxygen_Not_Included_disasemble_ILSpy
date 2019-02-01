using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class StationaryChoreRangeVisualizer : KMonoBehaviour
{
	private struct VisData
	{
		public int cell;

		public KBatchedAnimController controller;
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Rotatable rotatable;

	public int x;

	public int y;

	public int width;

	public int height;

	public bool movable = false;

	public Grid.SceneLayer sceneLayer = Grid.SceneLayer.FXFront;

	public CellOffset vision_offset;

	public Func<int, bool> blocking_cb = Grid.PhysicalBlockingCB;

	public bool blocking_tile_visible = true;

	private static readonly string AnimName = "transferarmgrid_kanim";

	private static readonly HashedString[] PreAnims = new HashedString[2]
	{
		"grid_pre",
		"grid_loop"
	};

	private static readonly HashedString PostAnim = "grid_pst";

	private List<VisData> visualizers = new List<VisData>();

	private List<int> newCells = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer> OnSelectDelegate = new EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer>(delegate(StationaryChoreRangeVisualizer component, object data)
	{
		component.OnSelect(data);
	});

	private static readonly EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer> OnRotatedDelegate = new EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer>(delegate(StationaryChoreRangeVisualizer component, object data)
	{
		component.OnRotated(data);
	});

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1503271301, OnSelectDelegate);
		if (movable)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "StationaryChoreRangeVisualizer.OnSpawn");
			Subscribe(-1643076535, OnRotatedDelegate);
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		Unsubscribe(-1503271301, OnSelectDelegate, false);
		Unsubscribe(-1643076535, OnRotatedDelegate, false);
		ClearVisualizers();
		base.OnCleanUp();
	}

	private void OnSelect(object data)
	{
		if ((bool)data)
		{
			SoundEvent.PlayOneShot(GlobalAssets.GetSound("RadialGrid_form", false), base.transform.position);
			UpdateVisualizers();
		}
		else
		{
			SoundEvent.PlayOneShot(GlobalAssets.GetSound("RadialGrid_disappear", false), base.transform.position);
			ClearVisualizers();
		}
	}

	private void OnRotated(object data)
	{
		UpdateVisualizers();
	}

	private void OnCellChange()
	{
		UpdateVisualizers();
	}

	private void UpdateVisualizers()
	{
		newCells.Clear();
		CellOffset rotatedCellOffset = vision_offset;
		if ((bool)rotatable)
		{
			rotatedCellOffset = rotatable.GetRotatedCellOffset(vision_offset);
		}
		int cell = Grid.PosToCell(base.transform.gameObject);
		int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
		Grid.CellToXY(cell2, out int num, out int num2);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				CellOffset offset = new CellOffset(this.x + j, this.y + i);
				if ((bool)rotatable)
				{
					offset = rotatable.GetRotatedCellOffset(offset);
				}
				int num3 = Grid.OffsetCell(cell, offset);
				if (Grid.IsValidCell(num3))
				{
					Grid.CellToXY(num3, out int x, out int y);
					if (Grid.TestLineOfSight(num, num2, x, y, blocking_cb, blocking_tile_visible))
					{
						newCells.Add(num3);
					}
				}
			}
		}
		for (int num4 = visualizers.Count - 1; num4 >= 0; num4--)
		{
			List<int> list = newCells;
			VisData visData = visualizers[num4];
			if (list.Contains(visData.cell))
			{
				List<int> list2 = newCells;
				VisData visData2 = visualizers[num4];
				list2.Remove(visData2.cell);
			}
			else
			{
				VisData visData3 = visualizers[num4];
				DestroyEffect(visData3.controller);
				visualizers.RemoveAt(num4);
			}
		}
		for (int k = 0; k < newCells.Count; k++)
		{
			KBatchedAnimController controller = CreateEffect(newCells[k]);
			visualizers.Add(new VisData
			{
				cell = newCells[k],
				controller = controller
			});
		}
	}

	private void ClearVisualizers()
	{
		for (int i = 0; i < visualizers.Count; i++)
		{
			VisData visData = visualizers[i];
			DestroyEffect(visData.controller);
		}
		visualizers.Clear();
	}

	private KBatchedAnimController CreateEffect(int cell)
	{
		KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect(AnimName, Grid.CellToPosCCC(cell, sceneLayer), null, false, sceneLayer, true);
		kBatchedAnimController.destroyOnAnimComplete = false;
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kBatchedAnimController.gameObject.SetActive(true);
		kBatchedAnimController.Play(PreAnims, KAnim.PlayMode.Loop);
		return kBatchedAnimController;
	}

	private void DestroyEffect(KBatchedAnimController controller)
	{
		controller.destroyOnAnimComplete = true;
		controller.Play(PostAnim, KAnim.PlayMode.Once, 1f, 0f);
	}
}
