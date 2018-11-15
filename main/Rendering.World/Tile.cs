namespace Rendering.World
{
	public struct Tile
	{
		public int Idx;

		public TileCells TileCells;

		public int MaskCount;

		public Tile(int idx, int tile_x, int tile_y, int mask_count)
		{
			Idx = idx;
			TileCells = new TileCells(tile_x, tile_y);
			MaskCount = mask_count;
		}
	}
}
