using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Curve : ModifierModule, IModule3D, IModule
	{
		protected List<ControlPoint> _controlPoints = new List<ControlPoint>(4);

		public Curve()
		{
		}

		public Curve(IModule source)
			: base(source)
		{
		}

		public void AddControlPoint(float input, float output)
		{
			AddControlPoint(new ControlPoint(input, output));
		}

		public void AddControlPoint(ControlPoint point)
		{
			if (_controlPoints.Contains(point))
			{
				throw new ArgumentException($"Cannont insert ControlPoint({point.Input}, {point.Output}) : Each control point is required to contain a unique input value");
			}
			_controlPoints.Add(point);
			SortControlPoints();
		}

		public int CountControlPoints()
		{
			return _controlPoints.Count;
		}

		public IList<ControlPoint> getControlPoints()
		{
			return _controlPoints.AsReadOnly();
		}

		public void ClearControlPoints()
		{
			_controlPoints.Clear();
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)_sourceModule).GetValue(x, y, z);
			int i;
			for (i = 0; i < _controlPoints.Count; i++)
			{
				float num = value;
				ControlPoint controlPoint = _controlPoints[i];
				if (num < controlPoint.Input)
				{
					break;
				}
			}
			int index = Libnoise.Clamp(i - 2, 0, _controlPoints.Count - 1);
			int num2 = Libnoise.Clamp(i - 1, 0, _controlPoints.Count - 1);
			int num3 = Libnoise.Clamp(i, 0, _controlPoints.Count - 1);
			int index2 = Libnoise.Clamp(i + 1, 0, _controlPoints.Count - 1);
			if (num2 == num3)
			{
				ControlPoint controlPoint2 = _controlPoints[num2];
				return controlPoint2.Output;
			}
			ControlPoint controlPoint3 = _controlPoints[num2];
			float input = controlPoint3.Input;
			ControlPoint controlPoint4 = _controlPoints[num3];
			float input2 = controlPoint4.Input;
			float a = (value - input) / (input2 - input);
			ControlPoint controlPoint5 = _controlPoints[index];
			float output = controlPoint5.Output;
			ControlPoint controlPoint6 = _controlPoints[num2];
			float output2 = controlPoint6.Output;
			ControlPoint controlPoint7 = _controlPoints[num3];
			float output3 = controlPoint7.Output;
			ControlPoint controlPoint8 = _controlPoints[index2];
			return Libnoise.Cerp(output, output2, output3, controlPoint8.Output, a);
		}

		protected void SortControlPoints()
		{
			_controlPoints.Sort(delegate(ControlPoint p1, ControlPoint p2)
			{
				if (p1.Input > p2.Input)
				{
					return 1;
				}
				if (p1.Input < p2.Input)
				{
					return -1;
				}
				return 0;
			});
		}
	}
}
