public class AmbientSoundManager : KMonoBehaviour
{
	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	public static AmbientSoundManager Instance
	{
		get;
		private set;
	}

	public static void Destroy()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Instance = null;
	}
}
