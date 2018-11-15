using System.Collections.Generic;

namespace Rendering.World
{
	public abstract class TileRenderer : KMonoBehaviour
	{
		private Tile[] TileGrid;

		private int[] BrushGrid;

		protected int TileGridWidth;

		protected int TileGridHeight;

		private int[] CellTiles = new int[4];

		protected Brush[] Brushes;

		protected Mask[] Masks;

		protected List<Brush> DirtyBrushes = new List<Brush>();

		protected List<Brush> ActiveBrushes = new List<Brush>();

		private VisibleAreaUpdater VisibleAreaUpdater;

		private HashSet<int> ClearTiles = new HashSet<int>();

		private HashSet<int> DirtyTiles = new HashSet<int>();

		public TextureAtlas Atlas;

		protected override void OnSpawn()
		{
			Masks = GetMasks();
			TileGridWidth = Grid.WidthInCells + 1;
			TileGridHeight = Grid.HeightInCells + 1;
			BrushGrid = new int[TileGridWidth * TileGridHeight * 4];
			for (int i = 0; i < BrushGrid.Length; i++)
			{
				BrushGrid[i] = -1;
			}
			TileGrid = new Tile[TileGridWidth * TileGridHeight];
			for (int j = 0; j < TileGrid.Length; j++)
			{
				int tile_x = j % TileGridWidth;
				int tile_y = j / TileGridWidth;
				TileGrid[j] = new Tile(j, tile_x, tile_y, Masks.Length);
			}
			LoadBrushes();
			VisibleAreaUpdater = new VisibleAreaUpdater(UpdateOutsideView, UpdateInsideView, null, null, "TileRenderer");
		}

		protected virtual Mask[] GetMasks()
		{
			return new Mask[16]
			{
				new Mask(Atlas, 0, false, false, false, false),
				new Mask(Atlas, 2, false, false, true, false),
				new Mask(Atlas, 2, false, true, true, false),
				new Mask(Atlas, 1, false, false, true, false),
				new Mask(Atlas, 2, false, false, false, false),
				new Mask(Atlas, 1, true, false, false, false),
				new Mask(Atlas, 3, false, false, false, false),
				new Mask(Atlas, 4, false, false, true, false),
				new Mask(Atlas, 2, false, true, false, false),
				new Mask(Atlas, 3, true, false, false, false),
				new Mask(Atlas, 1, true, false, true, false),
				new Mask(Atlas, 4, false, true, true, false),
				new Mask(Atlas, 1, false, false, false, false),
				new Mask(Atlas, 4, false, false, false, false),
				new Mask(Atlas, 4, false, true, false, false),
				new Mask(Atlas, 0, false, false, false, true)
			};
		}

		private void UpdateInsideView(int cell)
		{
			int[] cellTiles = GetCellTiles(cell);
			foreach (int item in cellTiles)
			{
				ClearTiles.Add(item);
				DirtyTiles.Add(item);
			}
		}

		private void UpdateOutsideView(int cell)
		{
			int[] cellTiles = GetCellTiles(cell);
			foreach (int item in cellTiles)
			{
				ClearTiles.Add(item);
			}
		}

		private int[] GetCellTiles(int cell)
		{
			int x = 0;
			int y = 0;
			Grid.CellToXY(cell, out x, out y);
			CellTiles[0] = y * TileGridWidth + x;
			CellTiles[1] = y * TileGridWidth + (x + 1);
			CellTiles[2] = (y + 1) * TileGridWidth + x;
			CellTiles[3] = (y + 1) * TileGridWidth + (x + 1);
			return CellTiles;
		}

		public abstract void LoadBrushes();

		public void MarkDirty(int cell)
		{
			VisibleAreaUpdater.UpdateCell(cell);
		}

		private void LateUpdate()
		{
			foreach (int clearTile in ClearTiles)
			{
				Clear(ref TileGrid[clearTile], Brushes, BrushGrid);
			}
			ClearTiles.Clear();
			foreach (int dirtyTile in DirtyTiles)
			{
				MarkDirty(ref TileGrid[dirtyTile], Brushes, BrushGrid);
			}
			DirtyTiles.Clear();
			VisibleAreaUpdater.Update();
			foreach (Brush dirtyBrush in DirtyBrushes)
			{
				dirtyBrush.Refresh();
			}
			DirtyBrushes.Clear();
			foreach (Brush activeBrush in ActiveBrushes)
			{
				activeBrush.Render();
			}
		}

		public abstract void MarkDirty(ref Tile tile, Brush[] brush_array, int[] brush_grid);

		public void Clear(ref Tile tile, Brush[] brush_array, int[] brush_grid)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = tile.Idx * 4 + i;
				if (brush_grid[num] != -1)
				{
					brush_array[brush_grid[num]].Remove(tile.Idx);
				}
			}
		}
	}
}
