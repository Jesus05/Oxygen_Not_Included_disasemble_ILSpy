namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public abstract class AbstractRenderer
	{
		protected RendererCallback _callBack;

		protected IMap2D<float> _noiseMap;

		public IMap2D<float> NoiseMap
		{
			get
			{
				return _noiseMap;
			}
			set
			{
				_noiseMap = value;
			}
		}

		public RendererCallback CallBack
		{
			get
			{
				return _callBack;
			}
			set
			{
				_callBack = value;
			}
		}

		public abstract void Render();
	}
}
