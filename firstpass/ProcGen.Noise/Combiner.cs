using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Combiner;
using System;

namespace ProcGen.Noise
{
	public class Combiner : NoiseBase
	{
		public enum CombinerType
		{
			_UNSET_,
			Add,
			Max,
			Min,
			Multiply,
			Power
		}

		public CombinerType combineType
		{
			get;
			set;
		}

		public Combiner()
		{
			combineType = CombinerType.Add;
		}

		public override Type GetObjectType()
		{
			return typeof(Combiner);
		}

		public IModule3D CreateModule()
		{
			switch (combineType)
			{
			case CombinerType.Add:
				return new Add();
			case CombinerType.Max:
				return new Max();
			case CombinerType.Min:
				return new Min();
			case CombinerType.Multiply:
				return new Multiply();
			case CombinerType.Power:
				return new Power();
			default:
				return null;
			}
		}

		public IModule3D CreateModule(IModule3D leftModule, IModule3D rightModule)
		{
			switch (combineType)
			{
			case CombinerType.Add:
				return new Add(leftModule, rightModule);
			case CombinerType.Max:
				return new Max(leftModule, rightModule);
			case CombinerType.Min:
				return new Min(leftModule, rightModule);
			case CombinerType.Multiply:
				return new Multiply(leftModule, rightModule);
			case CombinerType.Power:
				return new Power(leftModule, rightModule);
			default:
				return null;
			}
		}

		public void SetSouces(IModule3D target, IModule3D leftModule, IModule3D rightModule)
		{
			(target as CombinerModule).LeftModule = leftModule;
			(target as CombinerModule).RightModule = rightModule;
		}
	}
}
