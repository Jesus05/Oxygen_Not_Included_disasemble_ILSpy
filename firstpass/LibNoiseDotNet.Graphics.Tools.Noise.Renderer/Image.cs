using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Image : DataMap<Color>, IMap2D<Color>
	{
		public const int RASTER_MAX_WIDTH = 32767;

		public const int RASTER_MAX_HEIGHT = 32767;

		public Image()
		{
			_hasMaxDimension = true;
			_maxHeight = 32767;
			_maxWidth = 32767;
			_borderValue = Color.TRANSPARENT;
			AllocateBuffer();
		}

		public Image(int width, int height)
		{
			_hasMaxDimension = true;
			_maxHeight = 32767;
			_maxWidth = 32767;
			_borderValue = Color.WHITE;
			AllocateBuffer(width, height);
		}

		public Image(Image copy)
		{
			_hasMaxDimension = true;
			_maxHeight = 32767;
			_maxWidth = 32767;
			_borderValue = Color.WHITE;
			CopyFrom(copy);
		}

		public void MinMax(out Color min, out Color max)
		{
			min = (max = MinvalofT());
			if (_data != null && _data.Length > 0)
			{
				min = (max = _data[0]);
				for (int i = 0; i < _data.Length; i++)
				{
					if (min > (IColor)_data[i])
					{
						min = _data[i];
					}
					else if (max < (IColor)_data[i])
					{
						max = _data[i];
					}
				}
			}
		}

		protected override int SizeofT()
		{
			return 64;
		}

		protected override Color MaxvalofT()
		{
			return Color.WHITE;
		}

		protected override Color MinvalofT()
		{
			return Color.BLACK;
		}

		int get_Width()
		{
			return base.Width;
		}

		int IMap2D<Color>.get_Width()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Width
			return this.get_Width();
		}

		int get_Height()
		{
			return base.Height;
		}

		int IMap2D<Color>.get_Height()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Height
			return this.get_Height();
		}

		Color get_BorderValue()
		{
			return base.BorderValue;
		}

		Color IMap2D<Color>.get_BorderValue()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_BorderValue
			return this.get_BorderValue();
		}

		void set_BorderValue(Color value)
		{
			base.BorderValue = value;
		}

		void IMap2D<Color>.set_BorderValue(Color value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in set_BorderValue
			this.set_BorderValue(value);
		}

		Color IMap2D<Color>.GetValue(int x, int y)
		{
			return GetValue(x, y);
		}

		void IMap2D<Color>.SetValue(int x, int y, Color value)
		{
			SetValue(x, y, value);
		}

		void IMap2D<Color>.SetSize(int width, int height)
		{
			SetSize(width, height);
		}

		void IMap2D<Color>.Reset()
		{
			Reset();
		}

		void IMap2D<Color>.Clear(Color value)
		{
			Clear(value);
		}

		void IMap2D<Color>.Clear()
		{
			Clear();
		}
	}
}
