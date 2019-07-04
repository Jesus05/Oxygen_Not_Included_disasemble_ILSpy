using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;
using System;

namespace ProcGen.Noise
{
	public class Selector : NoiseBase
	{
		public enum SelectType
		{
			_UNSET_,
			Blend,
			Select
		}

		public SelectType selectType
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

		public float edge
		{
			get;
			set;
		}

		public Selector()
		{
			selectType = SelectType.Blend;
			lower = 0f;
			upper = 1f;
			edge = 0.02f;
		}

		public override Type GetObjectType()
		{
			return typeof(Selector);
		}

		public IModule3D CreateModule()
		{
			if (selectType != SelectType.Blend)
			{
				Select select = new Select();
				select.SetBounds(lower, upper);
				select.EdgeFalloff = edge;
				return select;
			}
			return new Blend();
		}

		public IModule3D CreateModule(IModule3D selectModule, IModule3D leftModule, IModule3D rightModule)
		{
			if (selectType != SelectType.Blend)
			{
				return new Select(selectModule, rightModule, leftModule, lower, upper, edge);
			}
			return new Blend(selectModule, rightModule, leftModule);
		}

		public void SetSouces(IModule3D target, IModule3D controlModule, IModule3D rightModule, IModule3D leftModule)
		{
			if (selectType == SelectType.Blend)
			{
				Blend blend = target as Blend;
				blend.ControlModule = controlModule;
				blend.RightModule = rightModule;
				blend.LeftModule = leftModule;
			}
			Select select = target as Select;
			select.ControlModule = controlModule;
			select.RightModule = rightModule;
			select.LeftModule = leftModule;
		}
	}
}
