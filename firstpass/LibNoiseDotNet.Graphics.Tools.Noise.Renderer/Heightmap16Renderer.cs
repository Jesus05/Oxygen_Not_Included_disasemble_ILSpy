namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap16Renderer : AbstractHeightmapRenderer
	{
		protected Heightmap16 _heightmap;

		public Heightmap16 Heightmap
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
			ushort value = (ushort)((!(source <= _lowerHeightBound)) ? ((!(source >= _upperHeightBound)) ? ((ushort)((source - _lowerHeightBound) / boundDiff * 65535f)) : 65535) : 0);
			_heightmap.SetValue(x, y, value);
		}
	}
}
