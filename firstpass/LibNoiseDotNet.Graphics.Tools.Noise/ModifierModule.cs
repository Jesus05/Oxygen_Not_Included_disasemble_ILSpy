namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class ModifierModule : IModule
	{
		protected IModule _sourceModule;

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

		public ModifierModule()
		{
		}

		public ModifierModule(IModule source)
		{
			_sourceModule = source;
		}
	}
}
