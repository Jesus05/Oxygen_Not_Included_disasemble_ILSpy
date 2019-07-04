namespace ProcGen
{
	public class LevelLayerSettings : IMerge<LevelLayerSettings>
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

		public void Merge(LevelLayerSettings other)
		{
			LevelLayers.Merge(other.LevelLayers);
		}
	}
}
