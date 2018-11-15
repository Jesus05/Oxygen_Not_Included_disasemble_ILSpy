using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public abstract class AbstractHeightmapRenderer : AbstractRenderer
	{
		protected float _lowerHeightBound;

		protected float _upperHeightBound;

		protected bool _WrapEnabled;

		public float LowerHeightBound => _lowerHeightBound;

		public float UpperHeightBound => _upperHeightBound;

		public bool WrapEnabled
		{
			get
			{
				return _WrapEnabled;
			}
			set
			{
				_WrapEnabled = value;
			}
		}

		public AbstractHeightmapRenderer()
		{
			_WrapEnabled = false;
		}

		public void SetBounds(float lowerBound, float upperBound)
		{
			if (lowerBound == upperBound || lowerBound > upperBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerBound == upperBound or lowerBound > upperBound");
			}
			_lowerHeightBound = lowerBound;
			_upperHeightBound = upperBound;
		}

		public void ExactFit()
		{
			_noiseMap.MinMax(out _lowerHeightBound, out _upperHeightBound);
		}

		public override void Render()
		{
			if (_noiseMap == null)
			{
				throw new ArgumentException("A noise map must be provided");
			}
			if (!CheckHeightmap())
			{
				throw new ArgumentException("An heightmap must be provided");
			}
			if (_noiseMap.Width <= 0 || _noiseMap.Height <= 0)
			{
				throw new ArgumentException("Incoherent noise map size (0,0)");
			}
			if (_lowerHeightBound == _upperHeightBound || _lowerHeightBound > _upperHeightBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerBound == upperBound or lowerBound > upperBound");
			}
			int width = _noiseMap.Width;
			int height = _noiseMap.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = 0;
			int num4 = 0;
			SetHeightmapSize(width, height);
			float boundDiff = _upperHeightBound - _lowerHeightBound;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					float num5 = _noiseMap.GetValue(j, i);
					if (_WrapEnabled)
					{
						int num6 = (j == num) ? num3 : ((j != num3) ? j : num);
						int num7 = (i == num2) ? num4 : ((i != num4) ? i : num2);
						if (num6 != j || num7 != i)
						{
							float value = _noiseMap.GetValue(num6, num7);
							num5 = Libnoise.Lerp(num5, value, 0.5f);
						}
					}
					RenderHeight(j, i, num5, boundDiff);
				}
				if (_callBack != null)
				{
					_callBack(i);
				}
			}
		}

		protected abstract bool CheckHeightmap();

		protected abstract void SetHeightmapSize(int width, int height);

		protected abstract void RenderHeight(int x, int y, float source, float boundDiff);
	}
}
