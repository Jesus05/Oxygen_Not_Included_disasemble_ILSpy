using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class NormalMapRenderer : AbstractImageRenderer
	{
		protected bool _WrapEnabled;

		protected float _bumpHeight;

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

		public float BumpHeight
		{
			get
			{
				return _bumpHeight;
			}
			set
			{
				_bumpHeight = value;
			}
		}

		public NormalMapRenderer()
		{
			_WrapEnabled = false;
			_bumpHeight = 1f;
		}

		public override void Render()
		{
			if (_noiseMap == null)
			{
				throw new ArgumentException("A noise map must be provided");
			}
			if (_image == null)
			{
				throw new ArgumentException("An image map must be provided");
			}
			if (_noiseMap.Width <= 0 || _noiseMap.Height <= 0)
			{
				throw new ArgumentException("Incoherent noise map size (0,0)");
			}
			int width = _noiseMap.Width;
			int height = _noiseMap.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = -num;
			int num4 = -num2;
			_image.SetSize(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int num5;
					int num6;
					if (_WrapEnabled)
					{
						num5 = ((j != num) ? 1 : num3);
						num6 = ((i != num2) ? 1 : num4);
					}
					else
					{
						num5 = ((j != num) ? 1 : 0);
						num6 = ((i != num2) ? 1 : 0);
					}
					float value = _noiseMap.GetValue(j, i);
					float value2 = _noiseMap.GetValue(j + num5, i);
					float value3 = _noiseMap.GetValue(j, i + num6);
					_image.SetValue(j, i, CalcNormalColor(value, value2, value3, _bumpHeight));
				}
				if (_callBack != null)
				{
					_callBack(i);
				}
			}
		}

		private IColor CalcNormalColor(float nc, float nr, float nu, float bumpHeight)
		{
			nc *= bumpHeight;
			nr *= bumpHeight;
			nu *= bumpHeight;
			float num = nc - nr;
			float num2 = nc - nu;
			float num3 = (float)Math.Sqrt((double)(num2 * num2 + num * num + 1f));
			float num4 = (nc - nr) / num3;
			float num5 = (nc - nu) / num3;
			float num6 = 1f / num3;
			byte r = (byte)(Libnoise.FastFloor((num4 + 1f) * 127.5f) & 0xFF);
			byte g = (byte)(Libnoise.FastFloor((num5 + 1f) * 127.5f) & 0xFF);
			byte b = (byte)(Libnoise.FastFloor((num6 + 1f) * 127.5f) & 0xFF);
			return new Color(r, g, b, byte.MaxValue);
		}
	}
}
