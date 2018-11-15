public class WorldGenScreen : KMonoBehaviour
{
	public static WorldGenScreen Instance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}
}
