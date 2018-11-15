using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class ImageRenderer : AbstractImageRenderer
	{
		protected Image _backgroundImage;

		protected GradientColor _gradient;

		private bool _lightEnabled;

		private bool _WrapEnabled;

		private float _lightAzimuth;

		private float _lightBrightness;

		private float _lightContrast;

		private float _lightElevation;

		private float _lightIntensity;

		private Color _lightColor;

		private bool _recalcLightValues;

		private float _cosAzimuth;

		private float _cosElevation;

		private float _sinAzimuth;

		private float _sinElevation;

		public bool LightEnabled
		{
			get
			{
				return _lightEnabled;
			}
			set
			{
				_lightEnabled = value;
			}
		}

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

		public Image BackgroundImage
		{
			get
			{
				return _backgroundImage;
			}
			set
			{
				_backgroundImage = value;
			}
		}

		public GradientColor Gradient
		{
			get
			{
				return _gradient;
			}
			set
			{
				_gradient = value;
			}
		}

		public float LightAzimuth
		{
			get
			{
				return _lightAzimuth;
			}
			set
			{
				_lightAzimuth = value;
				_recalcLightValues = true;
			}
		}

		public float LightBrightness
		{
			get
			{
				return _lightBrightness;
			}
			set
			{
				_lightBrightness = value;
				_recalcLightValues = true;
			}
		}

		public float LightContrast
		{
			get
			{
				return _lightContrast;
			}
			set
			{
				if (value <= 0f)
				{
					throw new ArgumentException("Contrast must be greater than 0");
				}
				_lightContrast = value;
				_recalcLightValues = true;
			}
		}

		public float LightElevation
		{
			get
			{
				return _lightElevation;
			}
			set
			{
				_lightElevation = value;
				_recalcLightValues = true;
			}
		}

		public float LightIntensity
		{
			get
			{
				return _lightIntensity;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentException("Intensity must be greater or equals to 0");
				}
				_lightIntensity = value;
				_recalcLightValues = true;
			}
		}

		public Color LightColor
		{
			get
			{
				return _lightColor;
			}
			set
			{
				_lightColor = value;
			}
		}

		public ImageRenderer()
		{
			_lightEnabled = false;
			_WrapEnabled = false;
			_lightAzimuth = 45f;
			_lightBrightness = 1f;
			_lightContrast = 1f;
			_lightElevation = 45f;
			_lightIntensity = 1f;
			_lightColor = Color.WHITE;
			_recalcLightValues = true;
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
			if (_gradient.CountGradientPoints() < 2)
			{
				throw new ArgumentException("Not enought points in the gradient");
			}
			int width = _noiseMap.Width;
			int height = _noiseMap.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = -num;
			int num4 = -num2;
			if (_backgroundImage != null && (_backgroundImage.Width != width || _backgroundImage.Height != height))
			{
				throw new ArgumentException("Incoherent background image size");
			}
			if (!_image.Equals(_backgroundImage))
			{
				_image.SetSize(width, height);
			}
			IColor backgroundColor = Color.WHITE;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					float value = _noiseMap.GetValue(j, i);
					IColor color = _gradient.GetColor(value);
					float num9;
					if (_lightEnabled)
					{
						int num5;
						int num6;
						int num7;
						int num8;
						if (_WrapEnabled)
						{
							if (j == 0)
							{
								num5 = num;
								num6 = 1;
							}
							else if (j == num)
							{
								num5 = -1;
								num6 = num3;
							}
							else
							{
								num5 = -1;
								num6 = 1;
							}
							if (i == 0)
							{
								num7 = num2;
								num8 = 1;
							}
							else if (i == num2)
							{
								num7 = -1;
								num8 = num4;
							}
							else
							{
								num7 = -1;
								num8 = 1;
							}
						}
						else
						{
							if (j == 0)
							{
								num5 = 0;
								num6 = 1;
							}
							else if (j == num)
							{
								num5 = -1;
								num6 = 0;
							}
							else
							{
								num5 = -1;
								num6 = 1;
							}
							if (i == 0)
							{
								num7 = 0;
								num8 = 1;
							}
							else if (i == num2)
							{
								num7 = -1;
								num8 = 0;
							}
							else
							{
								num7 = -1;
								num8 = 1;
							}
						}
						float value2 = _noiseMap.GetValue(j, i);
						float value3 = _noiseMap.GetValue(j + num5, i);
						float value4 = _noiseMap.GetValue(j + num6, i);
						float value5 = _noiseMap.GetValue(j, i + num7);
						float value6 = _noiseMap.GetValue(j, i + num8);
						num9 = CalcLightIntensity(value2, value3, value4, value5, value6);
						num9 *= _lightBrightness;
					}
					else
					{
						num9 = 1f;
					}
					if (_backgroundImage != null)
					{
						backgroundColor = _backgroundImage.GetValue(j, i);
					}
					_image.SetValue(j, i, CalcDestColor(color, backgroundColor, num9));
				}
				if (_callBack != null)
				{
					_callBack(i);
				}
			}
		}

		private IColor CalcDestColor(IColor sourceColor, IColor backgroundColor, float lightValue)
		{
			float n = (float)(int)sourceColor.Red / 255f;
			float n2 = (float)(int)sourceColor.Green / 255f;
			float n3 = (float)(int)sourceColor.Blue / 255f;
			float a = (float)(int)sourceColor.Alpha / 255f;
			float n4 = (float)(int)backgroundColor.Red / 255f;
			float n5 = (float)(int)backgroundColor.Green / 255f;
			float n6 = (float)(int)backgroundColor.Blue / 255f;
			float num = Libnoise.Lerp(n4, n, a);
			float num2 = Libnoise.Lerp(n5, n2, a);
			float num3 = Libnoise.Lerp(n6, n3, a);
			if (_lightEnabled)
			{
				float num4 = lightValue * (float)(int)_lightColor.Red / 255f;
				float num5 = lightValue * (float)(int)_lightColor.Green / 255f;
				float num6 = lightValue * (float)(int)_lightColor.Blue / 255f;
				num *= num4;
				num2 *= num5;
				num3 *= num6;
			}
			num = Libnoise.Clamp01(num);
			num2 = Libnoise.Clamp01(num2);
			num3 = Libnoise.Clamp01(num3);
			return new Color((byte)((uint)(num * 255f) & 0xFF), (byte)((uint)(num2 * 255f) & 0xFF), (byte)((uint)(num3 * 255f) & 0xFF), Math.Max(sourceColor.Alpha, backgroundColor.Alpha));
		}

		private float CalcLightIntensity(float center, float left, float right, float down, float up)
		{
			if (_recalcLightValues)
			{
				_cosAzimuth = (float)Math.Cos((double)(_lightAzimuth * 0.0174532924f));
				_sinAzimuth = (float)Math.Sin((double)(_lightAzimuth * 0.0174532924f));
				_cosElevation = (float)Math.Cos((double)(_lightElevation * 0.0174532924f));
				_sinElevation = (float)Math.Sin((double)(_lightElevation * 0.0174532924f));
				_recalcLightValues = false;
			}
			float num = 1.41421354f * _sinElevation / 2f;
			float num2 = (1f - num) * _lightContrast * 1.41421354f * _cosElevation * _cosAzimuth;
			float num3 = (1f - num) * _lightContrast * 1.41421354f * _cosElevation * _sinAzimuth;
			float num4 = num2 * (left - right) + num3 * (down - up) + num;
			if ((double)num4 < 0.0)
			{
				num4 = 0f;
			}
			return num4;
		}
	}
}
