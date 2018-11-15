namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class AbstractModel
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

		public AbstractModel()
		{
		}

		public AbstractModel(IModule module)
		{
			_sourceModule = module;
		}
	}
}
