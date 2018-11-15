namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap32Renderer : AbstractHeightmapRenderer
	{
		protected Heightmap32 _heightmap;

		public Heightmap32 Heightmap
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
			float value = (source <= _lowerHeightBound) ? _lowerHeightBound : ((!(source >= _upperHeightBound)) ? source : _upperHeightBound);
			_heightmap.SetValue(x, y, value);
		}
	}
}
