using LibNoiseDotNet.Graphics.Tools.Noise.Utils;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMap : DataMap<float>, IMap2D<float>
	{
		public NoiseMap()
		{
			_hasMaxDimension = false;
			_borderValue = 0f;
			AllocateBuffer();
		}

		public NoiseMap(int width, int height)
		{
			_hasMaxDimension = false;
			_borderValue = 0f;
			AllocateBuffer(width, height);
		}

		public NoiseMap(NoiseMap copy)
		{
			_hasMaxDimension = false;
			_borderValue = 0f;
			CopyFrom(copy);
		}

		public void MinMax(out float min, out float max)
		{
			min = (max = 0f);
			if (_data != null && _data.Length > 0)
			{
				min = (max = _data[0]);
				for (int i = 1; i < _data.Length; i++)
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
			return 32;
		}

		protected override float MaxvalofT()
		{
			return 3.40282347E+38f;
		}

		protected override float MinvalofT()
		{
			return -3.40282347E+38f;
		}

		int get_Width()
		{
			return base.Width;
		}

		int IMap2D<float>.get_Width()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Width
			return this.get_Width();
		}

		int get_Height()
		{
			return base.Height;
		}

		int IMap2D<float>.get_Height()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_Height
			return this.get_Height();
		}

		float get_BorderValue()
		{
			return base.BorderValue;
		}

		float IMap2D<float>.get_BorderValue()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_BorderValue
			return this.get_BorderValue();
		}

		void set_BorderValue(float value)
		{
			base.BorderValue = value;
		}

		void IMap2D<float>.set_BorderValue(float value)
		{
			//ILSpy generated this explicit interface implementation from .override directive in set_BorderValue
			this.set_BorderValue(value);
		}

		float IMap2D<float>.GetValue(int x, int y)
		{
			return GetValue(x, y);
		}

		void IMap2D<float>.SetValue(int x, int y, float value)
		{
			SetValue(x, y, value);
		}

		void IMap2D<float>.SetSize(int width, int height)
		{
			SetSize(width, height);
		}

		void IMap2D<float>.Reset()
		{
			Reset();
		}

		void IMap2D<float>.Clear(float value)
		{
			Clear(value);
		}

		void IMap2D<float>.Clear()
		{
			Clear();
		}
	}
}
