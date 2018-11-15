namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class CombinerModule : IModule
	{
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

		public CombinerModule()
		{
		}

		public CombinerModule(IModule left, IModule right)
		{
			_leftModule = left;
			_rightModule = right;
		}
	}
}
