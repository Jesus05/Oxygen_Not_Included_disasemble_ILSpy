using FMODUnity;
using UnityEngine;

public class MiningSounds : KMonoBehaviour
{
	[MyCmpGet]
	private LoopingSounds loopingSounds;

	private FMODAsset miningSound;

	[EventRef]
	private string miningSoundMigrated;

	private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStartMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>(delegate(MiningSounds component, object data)
	{
		component.OnStartMiningSound(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MiningSounds> OnStopMiningSoundDelegate = new EventSystem.IntraObjectHandler<MiningSounds>(delegate(MiningSounds component, object data)
	{
		component.OnStopMiningSound(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(-1762453998, OnStartMiningSoundDelegate);
		Subscribe(939543986, OnStopMiningSoundDelegate);
	}

	private void OnStartMiningSound(object data)
	{
		if ((Object)miningSound == (Object)null)
		{
			Element element = data as Element;
			if (element != null)
			{
				string text = element.substance.GetMiningSound();
				if (text != null && !(text == string.Empty))
				{
					text = "Mine_" + text;
					miningSoundMigrated = GlobalAssets.GetSound(text, false);
					if (miningSoundMigrated != null)
					{
						loopingSounds.StartSound(miningSoundMigrated);
					}
				}
			}
		}
	}

	private void OnStopMiningSound(object data)
	{
		if (miningSoundMigrated != null)
		{
			loopingSounds.StopSound(miningSoundMigrated);
			miningSound = null;
		}
	}
}
