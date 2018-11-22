using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class GradientColor
	{
		protected List<GradientPoint> _gradientPoints = new List<GradientPoint>(10);

		public static GradientColor GRAYSCALE
		{
			get
			{
				GradientColor gradientColor = new GradientColor();
				gradientColor.AddGradientPoint(-1f, Color.BLACK);
				gradientColor.AddGradientPoint(1f, Color.WHITE);
				return gradientColor;
			}
		}

		public static GradientColor EMPTY
		{
			get
			{
				GradientColor gradientColor = new GradientColor();
				gradientColor.AddGradientPoint(-1f, Color.TRANSPARENT);
				gradientColor.AddGradientPoint(1f, Color.TRANSPARENT);
				return gradientColor;
			}
		}

		public static GradientColor TERRAIN
		{
			get
			{
				GradientColor gradientColor = new GradientColor();
				gradientColor.AddGradientPoint(-1f, new Color(0, 0, 128, byte.MaxValue));
				gradientColor.AddGradientPoint(-0.25f, new Color(0, 0, byte.MaxValue, byte.MaxValue));
				gradientColor.AddGradientPoint(0f, new Color(0, 128, byte.MaxValue, byte.MaxValue));
				gradientColor.AddGradientPoint(0.0625f, new Color(240, 240, 64, byte.MaxValue));
				gradientColor.AddGradientPoint(0.125f, new Color(32, 160, 0, byte.MaxValue));
				gradientColor.AddGradientPoint(0.375f, new Color(224, 224, 0, byte.MaxValue));
				gradientColor.AddGradientPoint(0.75f, new Color(128, 128, 128, byte.MaxValue));
				gradientColor.AddGradientPoint(1f, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				return gradientColor;
			}
		}

		public GradientColor()
		{
		}

		public GradientColor(IColor color)
		{
			AddGradientPoint(-1f, color);
			AddGradientPoint(1f, color);
		}

		public GradientColor(IColor start, IColor end)
		{
			AddGradientPoint(-1f, start);
			AddGradientPoint(1f, end);
		}

		public void AddGradientPoint(float position, IColor color)
		{
			AddGradientPoint(new GradientPoint(position, color));
		}

		public void AddGradientPoint(GradientPoint point)
		{
			if (_gradientPoints.Contains(point))
			{
				throw new ArgumentException($"Cannont insert GradientPoint({point.Position}, {point.Color}) : Each GradientPoint is required to contain a unique position");
			}
			_gradientPoints.Add(point);
			_gradientPoints.Sort(delegate(GradientPoint p1, GradientPoint p2)
			{
				if (!(p1.Position > p2.Position))
				{
					if (!(p1.Position < p2.Position))
					{
						return 0;
					}
					return -1;
				}
				return 1;
			});
		}

		public void Clear()
		{
			_gradientPoints.Clear();
		}

		public IColor GetColor(float position)
		{
			int i;
			for (i = 0; i < _gradientPoints.Count; i++)
			{
				GradientPoint gradientPoint = _gradientPoints[i];
				if (position < gradientPoint.Position)
				{
					break;
				}
			}
			int num = Libnoise.Clamp(i - 1, 0, _gradientPoints.Count - 1);
			int num2 = Libnoise.Clamp(i, 0, _gradientPoints.Count - 1);
			if (num != num2)
			{
				GradientPoint gradientPoint2 = _gradientPoints[num];
				float position2 = gradientPoint2.Position;
				GradientPoint gradientPoint3 = _gradientPoints[num2];
				float position3 = gradientPoint3.Position;
				float num3 = (position - position2) / (position3 - position2);
				GradientPoint gradientPoint4 = _gradientPoints[num];
				IColor color = gradientPoint4.Color;
				GradientPoint gradientPoint5 = _gradientPoints[num2];
				return Color.Lerp(color, gradientPoint5.Color, num3);
			}
			GradientPoint gradientPoint6 = _gradientPoints[num2];
			return gradientPoint6.Color;
		}

		public int CountGradientPoints()
		{
			return _gradientPoints.Count;
		}

		public IList<GradientPoint> getGradientPoints()
		{
			return _gradientPoints.AsReadOnly();
		}
	}
}
