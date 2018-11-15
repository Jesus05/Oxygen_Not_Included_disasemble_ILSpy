public class KComponentsInitializer : KComponentSpawn
{
	private void Awake()
	{
		KComponentSpawn.instance = this;
		comps = new GameComps();
	}
}
