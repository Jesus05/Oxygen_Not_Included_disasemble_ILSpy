using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap8 : DataMap<byte>, IMap2D<byte>
	{
		public Heightmap8()
		{
			_borderValue = 0;
			AllocateBuffer();
		}

		public Heightmap8(int width, int height)
		{
			_borderValue = 0;
			AllocateBuffer(width, height);
		}

		public Heightmap8(Heightmap8 copy)
		{
			_borderValue = 0;
			CopyFrom(copy);
		}

		public void MinMax(out byte min, out byte max)
		{
			min = (max = 0);
			if (_data != null && _data.Length > 0)
			{
				min = (max = _data[0]);
				for (int i = 0; i < _data.Length; i++)
				{
					if (min > _data[i])
					{
						min = _data[i];
					}
					else if (max < _data[i])
					{
						max = _data[i];
					}
				}
			}
		}

		protected override int SizeofT()
		{
			return 8;
		}

		protected override byte MaxvalofT()
		{
			return byte.MaxValue;
		}

		protected override byte MinvalofT()
		{
			return 0;
		}

		int get_Width()
		{
			return base.Width;
		}

		int IMap2D<byte>.get_Width()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Width
			return this.get_Width();
		}

		int get_Height()
		{
			return base.Height;
		}

		int IMap2D<byte>.get_Height()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Height
			return this.get_Height();
		}

		byte get_BorderValue()
		{
			return base.BorderValue;
		}

		byte IMap2D<byte>.get_BorderValue()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_BorderValue
			return this.get_BorderValue();
		}

		void set_BorderValue(byte value)
		{
			base.BorderValue = value;
		}

		void IMap2D<byte>.set_BorderValue(byte value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in set_BorderValue
			this.set_BorderValue(value);
		}

		byte IMap2D<byte>.GetValue(int x, int y)
		{
			return GetValue(x, y);
		}

		void IMap2D<byte>.SetValue(int x, int y, byte value)
		{
			SetValue(x, y, value);
		}

		void IMap2D<byte>.SetSize(int width, int height)
		{
			SetSize(width, height);
		}

		void IMap2D<byte>.Reset()
		{
			Reset();
		}

		void IMap2D<byte>.Clear(byte value)
		{
			Clear(value);
		}

		void IMap2D<byte>.Clear()
		{
			Clear();
		}
	}
}
