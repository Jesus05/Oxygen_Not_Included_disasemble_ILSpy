namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap8Renderer : AbstractHeightmapRenderer
	{
		protected Heightmap8 _heightmap;

		public Heightmap8 Heightmap
		{
			get
			{
				return _heightmap;
			}
			set
			{
				_heightmap = value;
			}
		}

		protected override void SetHeightmapSize(int width, int height)
		{
			_heightmap.SetSize(width, height);
		}

		protected override bool CheckHeightmap()
		{
			return _heightmap != null;
		}

		protected override void RenderHeight(int x, int y, float source, float boundDiff)
		{
			byte value = (byte)((!(source <= _lowerHeightBound)) ? ((!(source >= _upperHeightBound)) ? ((byte)((source - _lowerHeightBound) / boundDiff * 255f)) : 255) : 0);
			_heightmap.SetValue(x, y, value);
		}
	}
}
