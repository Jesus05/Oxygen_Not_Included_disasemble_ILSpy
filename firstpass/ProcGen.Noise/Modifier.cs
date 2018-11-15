using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;
using System;
using System.Collections.Generic;

namespace ProcGen.Noise
{
	public class Modifier : NoiseBase
	{
		public enum ModifyType
		{
			_UNSET_,
			Abs,
			Clamp,
			Exponent,
			Invert,
			ScaleBias,
			Scale2d,
			Curve,
			Terrace
		}

		public ModifyType modifyType
		{
			get;
			set;
		}

		public float lower
		{
			get;
			set;
		}

		public float upper
		{
			get;
			set;
		}

		public float exponent
		{
			get;
			set;
		}

		public bool invert
		{
			get;
			set;
		}

		public float scale
		{
			get;
			set;
		}

		public float bias
		{
			get;
			set;
		}

		public Vector2f scale2d
		{
			get;
			set;
		}

		public Modifier()
		{
			modifyType = ModifyType.Abs;
			lower = -1f;
			upper = 1f;
			exponent = 0.02f;
			invert = false;
			scale = 1f;
			bias = 0f;
			scale2d = new Vector2f(1, 1);
		}

		public override Type GetObjectType()
		{
			return typeof(Modifier);
		}

		public IModule3D CreateModule()
		{
			switch (modifyType)
			{
			case ModifyType.Abs:
				return new Abs();
			case ModifyType.Clamp:
			{
				Clamp clamp = new Clamp();
				clamp.LowerBound = lower;
				clamp.UpperBound = upper;
				return clamp;
			}
			case ModifyType.Exponent:
			{
				Exponent exponent = new Exponent();
				exponent.ExponentValue = this.exponent;
				return exponent;
			}
			case ModifyType.Invert:
				return new Invert();
			case ModifyType.Curve:
				return new Curve();
			case ModifyType.Terrace:
				return new Terrace();
			case ModifyType.ScaleBias:
			{
				ScaleBias scaleBias = new ScaleBias();
				scaleBias.Scale = scale;
				scaleBias.Bias = bias;
				return scaleBias;
			}
			case ModifyType.Scale2d:
			{
				Scale2d scale2d = new Scale2d();
				scale2d.Scale = this.scale2d;
				return scale2d;
			}
			default:
				return null;
			}
		}

		public IModule3D CreateModule(IModule3D sourceModule)
		{
			switch (modifyType)
			{
			case ModifyType.Abs:
				return new Abs(sourceModule);
			case ModifyType.Clamp:
				return new Clamp(sourceModule, lower, upper);
			case ModifyType.Exponent:
				return new Exponent(sourceModule, exponent);
			case ModifyType.Invert:
				return new Invert(sourceModule);
			case ModifyType.Curve:
				return new Curve(sourceModule);
			case ModifyType.Terrace:
				return new Terrace(sourceModule);
			case ModifyType.ScaleBias:
				return new ScaleBias(sourceModule, scale, bias);
			case ModifyType.Scale2d:
				return new Scale2d(sourceModule, scale2d);
			default:
				return null;
			}
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, FloatList controlFloats, ControlPointList controlPoints)
		{
			(target as ModifierModule).SourceModule = sourceModule;
			if (modifyType == ModifyType.Curve)
			{
				Curve curve = target as Curve;
				curve.ClearControlPoints();
				List<ControlPoint> controls = controlPoints.GetControls();
				foreach (ControlPoint item in controls)
				{
					curve.AddControlPoint(item);
				}
			}
			else if (modifyType == ModifyType.Terrace)
			{
				Terrace terrace = target as Terrace;
				terrace.ClearControlPoints();
				foreach (float point in controlFloats.points)
				{
					float input = point;
					terrace.AddControlPoint(input);
				}
			}
		}
	}
}
