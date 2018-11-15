using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Terrace : ModifierModule, IModule3D, IModule
	{
		protected List<float> _controlPoints = new List<float>(2);

		protected bool _invert;

		public bool Invert
		{
			get
			{
				return _invert;
			}
			set
			{
				_invert = value;
			}
		}

		public Terrace()
		{
		}

		public Terrace(IModule source)
			: base(source)
		{
		}

		public Terrace(IModule source, bool invert)
			: base(source)
		{
			_invert = invert;
		}

		public void AddControlPoint(float input)
		{
			if (_controlPoints.Contains(input))
			{
				throw new ArgumentException($"Cannont insert ControlPoint({input}) : Each control point is required to contain a unique input value");
			}
			_controlPoints.Add(input);
			SortControlPoints();
		}

		public int CountControlPoints()
		{
			return _controlPoints.Count;
		}

		public IList<float> getControlPoints()
		{
			return _controlPoints.AsReadOnly();
		}

		public void ClearControlPoints()
		{
			_controlPoints.Clear();
		}

		public void MakeControlPoints(int controlPointCount)
		{
			if (controlPointCount < 2)
			{
				throw new ArgumentException("Two or more control points must be specified.");
			}
			ClearControlPoints();
			float num = 2f / ((float)controlPointCount - 1f);
			float num2 = -1f;
			for (int i = 0; i < controlPointCount; i++)
			{
				AddControlPoint(num2);
				num2 += num;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)_sourceModule).GetValue(x, y, z);
			int i;
			for (i = 0; i < _controlPoints.Count && !(value < _controlPoints[i]); i++)
			{
			}
			int num = Libnoise.Clamp(i - 1, 0, _controlPoints.Count - 1);
			int num2 = Libnoise.Clamp(i, 0, _controlPoints.Count - 1);
			if (num == num2)
			{
				return _controlPoints[num2];
			}
			float a = _controlPoints[num];
			float b = _controlPoints[num2];
			float num3 = (value - a) / (b - a);
			if (_invert)
			{
				num3 = 1f - num3;
				Libnoise.SwapValues(ref a, ref b);
			}
			num3 *= num3;
			return Libnoise.Lerp(a, b, num3);
		}

		protected void SortControlPoints()
		{
			_controlPoints.Sort(delegate(float p1, float p2)
			{
				if (p1 > p2)
				{
					return 1;
				}
				if (p1 < p2)
				{
					return -1;
				}
				return 0;
			});
		}
	}
}
