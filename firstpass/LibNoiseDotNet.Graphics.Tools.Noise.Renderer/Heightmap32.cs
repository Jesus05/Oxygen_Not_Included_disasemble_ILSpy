using LibNoiseDotNet.Graphics.Tools.Noise.Builder;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap32 : NoiseMap
	{
		public Heightmap32()
		{
		}

		public Heightmap32(int width, int height)
			: base(width, height)
		{
		}

		public Heightmap32(Heightmap32 copy)
			: base(copy)
		{
		}
	}
}
