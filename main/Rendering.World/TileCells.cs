using System;

namespace Rendering.World
{
	public struct TileCells
	{
		public int Cell0;

		public int Cell1;

		public int Cell2;

		public int Cell3;

		public TileCells(int tile_x, int tile_y)
		{
			int val = Grid.WidthInCells - 1;
			int val2 = Grid.HeightInCells - 1;
			Cell0 = Grid.XYToCell(Math.Min(Math.Max(tile_x - 1, 0), val), Math.Min(Math.Max(tile_y - 1, 0), val2));
			Cell1 = Grid.XYToCell(Math.Min(tile_x, val), Math.Min(Math.Max(tile_y - 1, 0), val2));
			Cell2 = Grid.XYToCell(Math.Min(Math.Max(tile_x - 1, 0), val), Math.Min(tile_y, val2));
			Cell3 = Grid.XYToCell(Math.Min(tile_x, val), Math.Min(tile_y, val2));
		}
	}
}
