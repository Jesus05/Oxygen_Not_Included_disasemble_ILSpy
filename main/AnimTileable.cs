using UnityEngine;

[SkipSaveFileSerialization]
public class AnimTileable : KMonoBehaviour
{
	private HandleVector<int>.Handle partitionerEntry;

	public ObjectLayer objectLayer = ObjectLayer.Building;

	public Tag[] tags;

	private Extents extents;

	private static readonly KAnimHashedString[] leftSymbols = new KAnimHashedString[3]
	{
		new KAnimHashedString("cap_left"),
		new KAnimHashedString("cap_left_fg"),
		new KAnimHashedString("cap_left_place")
	};

	private static readonly KAnimHashedString[] rightSymbols = new KAnimHashedString[3]
	{
		new KAnimHashedString("cap_right"),
		new KAnimHashedString("cap_right_fg"),
		new KAnimHashedString("cap_right_place")
	};

	private static readonly KAnimHashedString[] topSymbols = new KAnimHashedString[3]
	{
		new KAnimHashedString("cap_top"),
		new KAnimHashedString("cap_top_fg"),
		new KAnimHashedString("cap_top_place")
	};

	private static readonly KAnimHashedString[] bottomSymbols = new KAnimHashedString[3]
	{
		new KAnimHashedString("cap_bottom"),
		new KAnimHashedString("cap_bottom_fg"),
		new KAnimHashedString("cap_bottom_place")
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (tags == null || tags.Length == 0)
		{
			tags = new Tag[1]
			{
				GetComponent<KPrefabID>().PrefabTag
			};
		}
	}

	protected override void OnSpawn()
	{
		OccupyArea component = GetComponent<OccupyArea>();
		if ((Object)component != (Object)null)
		{
			this.extents = component.GetExtents();
		}
		else
		{
			Building component2 = GetComponent<Building>();
			this.extents = component2.GetExtents();
		}
		Extents extents = new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2);
		partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnNeighbourCellsUpdated);
		UpdateEndCaps();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void UpdateEndCaps()
	{
		int cell = Grid.PosToCell(this);
		bool is_visible = true;
		bool is_visible2 = true;
		bool is_visible3 = true;
		bool is_visible4 = true;
		Grid.CellToXY(cell, out int x, out int y);
		CellOffset offset = new CellOffset(extents.x - x - 1, 0);
		CellOffset offset2 = new CellOffset(extents.x - x + extents.width, 0);
		CellOffset offset3 = new CellOffset(0, extents.y - y + extents.height);
		CellOffset offset4 = new CellOffset(0, extents.y - y - 1);
		Rotatable component = GetComponent<Rotatable>();
		if ((bool)component)
		{
			offset = component.GetRotatedCellOffset(offset);
			offset2 = component.GetRotatedCellOffset(offset2);
			offset3 = component.GetRotatedCellOffset(offset3);
			offset4 = component.GetRotatedCellOffset(offset4);
		}
		int num = Grid.OffsetCell(cell, offset);
		int num2 = Grid.OffsetCell(cell, offset2);
		int num3 = Grid.OffsetCell(cell, offset3);
		int num4 = Grid.OffsetCell(cell, offset4);
		if (Grid.IsValidCell(num))
		{
			is_visible = !HasTileableNeighbour(num);
		}
		if (Grid.IsValidCell(num2))
		{
			is_visible2 = !HasTileableNeighbour(num2);
		}
		if (Grid.IsValidCell(num3))
		{
			is_visible3 = !HasTileableNeighbour(num3);
		}
		if (Grid.IsValidCell(num4))
		{
			is_visible4 = !HasTileableNeighbour(num4);
		}
		KBatchedAnimController[] componentsInChildren = GetComponentsInChildren<KBatchedAnimController>();
		KBatchedAnimController[] array = componentsInChildren;
		foreach (KBatchedAnimController kBatchedAnimController in array)
		{
			KAnimHashedString[] array2 = leftSymbols;
			foreach (KAnimHashedString symbol in array2)
			{
				kBatchedAnimController.SetSymbolVisiblity(symbol, is_visible);
			}
			KAnimHashedString[] array3 = rightSymbols;
			foreach (KAnimHashedString symbol2 in array3)
			{
				kBatchedAnimController.SetSymbolVisiblity(symbol2, is_visible2);
			}
			KAnimHashedString[] array4 = topSymbols;
			foreach (KAnimHashedString symbol3 in array4)
			{
				kBatchedAnimController.SetSymbolVisiblity(symbol3, is_visible3);
			}
			KAnimHashedString[] array5 = bottomSymbols;
			foreach (KAnimHashedString symbol4 in array5)
			{
				kBatchedAnimController.SetSymbolVisiblity(symbol4, is_visible4);
			}
		}
	}

	private bool HasTileableNeighbour(int neighbour_cell)
	{
		bool result = false;
		GameObject gameObject = Grid.Objects[neighbour_cell, (int)objectLayer];
		if ((Object)gameObject != (Object)null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if ((Object)component != (Object)null)
			{
				for (int i = 0; i < tags.Length; i++)
				{
					if (component.PrefabTag == tags[i])
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	private void OnNeighbourCellsUpdated(object data)
	{
		if (!((Object)this == (Object)null) && !((Object)base.gameObject == (Object)null) && partitionerEntry.IsValid())
		{
			UpdateEndCaps();
		}
	}
}
