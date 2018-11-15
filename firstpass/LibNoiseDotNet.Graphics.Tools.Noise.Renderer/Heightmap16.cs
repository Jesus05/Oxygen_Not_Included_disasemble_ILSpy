using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Heightmap16 : DataMap<ushort>, IMap2D<ushort>
	{
		public Heightmap16()
		{
			_borderValue = 0;
			AllocateBuffer();
		}

		public Heightmap16(int width, int height)
		{
			_borderValue = 0;
			AllocateBuffer(width, height);
		}

		public Heightmap16(Heightmap16 copy)
		{
			_borderValue = 0;
			CopyFrom(copy);
		}

		public void MinMax(out ushort min, out ushort max)
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
			return 16;
		}

		protected override ushort MaxvalofT()
		{
			return ushort.MaxValue;
		}

		protected override ushort MinvalofT()
		{
			return 0;
		}

		int get_Width()
		{
			return base.Width;
		}

		int IMap2D<ushort>.get_Width()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Width
			return this.get_Width();
		}

		int get_Height()
		{
			return base.Height;
		}

		int IMap2D<ushort>.get_Height()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Height
			return this.get_Height();
		}

		ushort get_BorderValue()
		{
			return base.BorderValue;
		}

		ushort IMap2D<ushort>.get_BorderValue()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_BorderValue
			return this.get_BorderValue();
		}

		void set_BorderValue(ushort value)
		{
			base.BorderValue = value;
		}

		void IMap2D<ushort>.set_BorderValue(ushort value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in set_BorderValue
			this.set_BorderValue(value);
		}

		ushort IMap2D<ushort>.GetValue(int x, int y)
		{
			return GetValue(x, y);
		}

		void IMap2D<ushort>.SetValue(int x, int y, ushort value)
		{
			SetValue(x, y, value);
		}

		void IMap2D<ushort>.SetSize(int width, int height)
		{
			SetSize(width, height);
		}

		void IMap2D<ushort>.Reset()
		{
			Reset();
		}

		void IMap2D<ushort>.Clear(ushort value)
		{
			Clear(value);
		}

		void IMap2D<ushort>.Clear()
		{
			Clear();
		}
	}
}
