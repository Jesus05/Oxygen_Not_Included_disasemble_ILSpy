namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Blend : SelectorModule, IModule3D, IModule
	{
		protected IModule _controlModule;

		protected IModule _rightModule;

		protected IModule _leftModule;

		public IModule LeftModule
		{
			get
			{
				return _leftModule;
			}
			set
			{
				_leftModule = value;
			}
		}

		public IModule RightModule
		{
			get
			{
				return _rightModule;
			}
			set
			{
				_rightModule = value;
			}
		}

		public IModule ControlModule
		{
			get
			{
				return _controlModule;
			}
			set
			{
				_controlModule = value;
			}
		}

		public Blend()
		{
		}

		public Blend(IModule controlModule, IModule rightModule, IModule leftModule)
		{
			_controlModule = controlModule;
			_leftModule = leftModule;
			_rightModule = rightModule;
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)_leftModule).GetValue(x, y, z);
			float value2 = ((IModule3D)_rightModule).GetValue(x, y, z);
			float a = (((IModule3D)_controlModule).GetValue(x, y, z) + 1f) / 2f;
			return Libnoise.Lerp(value, value2, a);
		}
	}
}
