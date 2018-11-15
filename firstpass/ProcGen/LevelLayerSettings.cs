using Klei;

namespace ProcGen
{
	public class LevelLayerSettings : YamlIO<LevelLayerSettings>
	{
		public LevelLayer LevelLayers
		{
			get;
			private set;
		}

		public LevelLayerSettings()
		{
			LevelLayers = new LevelLayer();
		}
	}
}
