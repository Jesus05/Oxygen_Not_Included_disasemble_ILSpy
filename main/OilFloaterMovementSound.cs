internal class OilFloaterMovementSound : KMonoBehaviour
{
	public string sound;

	public bool isPlayingSound;

	public bool isMoving;

	private static readonly EventSystem.IntraObjectHandler<OilFloaterMovementSound> OnObjectMovementStateChangedDelegate = new EventSystem.IntraObjectHandler<OilFloaterMovementSound>(delegate(OilFloaterMovementSound component, object data)
	{
		component.OnObjectMovementStateChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		sound = GlobalAssets.GetSound(sound, false);
		Subscribe(1027377649, OnObjectMovementStateChangedDelegate);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChanged, "OilFloaterMovementSound");
	}

	private void OnObjectMovementStateChanged(object data)
	{
		GameHashes gameHashes = (GameHashes)data;
		isMoving = (gameHashes == GameHashes.ObjectMovementWakeUp);
		UpdateSound();
	}

	private void OnCellChanged()
	{
		UpdateSound();
	}

	private void UpdateSound()
	{
		bool flag = isMoving && GetComponent<Navigator>().CurrentNavType != NavType.Swim;
		if (flag != isPlayingSound)
		{
			LoopingSounds component = GetComponent<LoopingSounds>();
			if (flag)
			{
				component.StartSound(sound);
			}
			else
			{
				component.StopSound(sound);
			}
			isPlayingSound = flag;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChanged);
	}
}
