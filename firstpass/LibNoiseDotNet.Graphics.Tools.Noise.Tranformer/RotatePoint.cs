using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class RotatePoint : TransformerModule, IModule3D, IModule
	{
		public const float DEFAULT_ROTATE_X = 0f;

		public const float DEFAULT_ROTATE_Y = 0f;

		public const float DEFAULT_ROTATE_Z = 0f;

		protected IModule _sourceModule;

		protected float _x1Matrix;

		protected float _x2Matrix;

		protected float _x3Matrix;

		protected float _xAngle;

		protected float _y1Matrix;

		protected float _y2Matrix;

		protected float _y3Matrix;

		protected float _yAngle;

		protected float _z1Matrix;

		protected float _z2Matrix;

		protected float _z3Matrix;

		protected float _zAngle;

		public IModule SourceModule
		{
			get
			{
				return _sourceModule;
			}
			set
			{
				_sourceModule = value;
			}
		}

		public float XAngle
		{
			get
			{
				return _xAngle;
			}
			set
			{
				SetAngles(value, _yAngle, _zAngle);
			}
		}

		public float YAngle
		{
			get
			{
				return _yAngle;
			}
			set
			{
				SetAngles(_xAngle, value, _zAngle);
			}
		}

		public float ZAngle
		{
			get
			{
				return _zAngle;
			}
			set
			{
				SetAngles(_xAngle, _yAngle, value);
			}
		}

		public RotatePoint()
		{
		}

		public RotatePoint(IModule source)
		{
			_sourceModule = source;
		}

		public RotatePoint(IModule source, float xAngle, float yAngle, float zAngle)
		{
			_sourceModule = source;
			SetAngles(xAngle, yAngle, zAngle);
		}

		public void SetAngles(float xAngle, float yAngle, float zAngle)
		{
			float num = (float)Math.Cos((double)(xAngle * 0.0174532924f));
			float num2 = (float)Math.Cos((double)(yAngle * 0.0174532924f));
			float num3 = (float)Math.Cos((double)(zAngle * 0.0174532924f));
			float num4 = (float)Math.Sin((double)(xAngle * 0.0174532924f));
			float num5 = (float)Math.Sin((double)(yAngle * 0.0174532924f));
			float num6 = (float)Math.Sin((double)(zAngle * 0.0174532924f));
			_x1Matrix = num5 * num4 * num6 + num2 * num3;
			_y1Matrix = num * num6;
			_z1Matrix = num5 * num3 - num2 * num4 * num6;
			_x2Matrix = num5 * num4 * num3 - num2 * num6;
			_y2Matrix = num * num3;
			_z2Matrix = (0f - num2) * num4 * num3 - num5 * num6;
			_x3Matrix = (0f - num5) * num;
			_y3Matrix = num4;
			_z3Matrix = num2 * num;
			_xAngle = xAngle;
			_yAngle = yAngle;
			_zAngle = zAngle;
		}

		public float GetValue(float x, float y, float z)
		{
			float x2 = _x1Matrix * x + _y1Matrix * y + _z1Matrix * z;
			float y2 = _x2Matrix * x + _y2Matrix * y + _z2Matrix * z;
			float z2 = _x3Matrix * x + _y3Matrix * y + _z3Matrix * z;
			return ((IModule3D)_sourceModule).GetValue(x2, y2, z2);
		}
	}
}
