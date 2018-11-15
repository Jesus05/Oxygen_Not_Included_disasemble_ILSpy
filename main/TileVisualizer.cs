using UnityEngine;

public class TileVisualizer
{
	private static void RefreshCellInternal(int cell, ObjectLayer tile_layer)
	{
		if (!Game.IsQuitting() && Grid.IsValidCell(cell))
		{
			GameObject gameObject = Grid.Objects[cell, (int)tile_layer];
			if ((Object)gameObject != (Object)null)
			{
				World.Instance.blockTileRenderer.Rebuild(tile_layer, cell);
				KAnimGraphTileVisualizer componentInChildren = gameObject.GetComponentInChildren<KAnimGraphTileVisualizer>();
				if ((Object)componentInChildren != (Object)null)
				{
					componentInChildren.Refresh();
				}
			}
		}
	}

	private static void RefreshCell(int cell, ObjectLayer tile_layer)
	{
		if (tile_layer != ObjectLayer.NumLayers)
		{
			RefreshCellInternal(cell, tile_layer);
			RefreshCellInternal(Grid.CellAbove(cell), tile_layer);
			RefreshCellInternal(Grid.CellBelow(cell), tile_layer);
			RefreshCellInternal(Grid.CellLeft(cell), tile_layer);
			RefreshCellInternal(Grid.CellRight(cell), tile_layer);
		}
	}

	public static void RefreshCell(int cell, ObjectLayer tile_layer, ObjectLayer replacement_layer)
	{
		RefreshCell(cell, tile_layer);
		RefreshCell(cell, replacement_layer);
	}
}
