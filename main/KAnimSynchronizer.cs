using System.Collections.Generic;
using UnityEngine;

public class KAnimSynchronizer
{
	private KAnimControllerBase masterController;

	private List<KAnimControllerBase> Targets = new List<KAnimControllerBase>();

	public KAnimSynchronizer(KAnimControllerBase master_controller)
	{
		masterController = master_controller;
	}

	private void Clear(KAnimControllerBase controller)
	{
		controller.Play("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void Add(KAnimControllerBase controller)
	{
		Targets.Add(controller);
	}

	public void Remove(KAnimControllerBase controller)
	{
		Clear(controller);
		Targets.Remove(controller);
	}

	public void Clear()
	{
		foreach (KAnimControllerBase target in Targets)
		{
			Clear(target);
		}
		Targets.Clear();
	}

	public void Sync(KAnimControllerBase controller)
	{
		if (!((Object)masterController == (Object)null) && !((Object)controller == (Object)null))
		{
			KAnim.Anim currentAnim = masterController.GetCurrentAnim();
			if (currentAnim != null)
			{
				KAnim.PlayMode mode = masterController.GetMode();
				float playSpeed = masterController.GetPlaySpeed();
				float elapsedTime = masterController.GetElapsedTime();
				controller.Play(currentAnim.name, mode, playSpeed, elapsedTime);
				Facing component = controller.GetComponent<Facing>();
				if ((Object)component != (Object)null)
				{
					Vector3 position = component.transform.GetPosition();
					float x = position.x;
					x += ((!masterController.FlipX) ? 0.5f : (-0.5f));
					component.Face(x);
				}
				else
				{
					controller.FlipX = masterController.FlipX;
					controller.FlipY = masterController.FlipY;
				}
			}
		}
	}

	public void Sync()
	{
		for (int i = 0; i < Targets.Count; i++)
		{
			KAnimControllerBase controller = Targets[i];
			Sync(controller);
		}
	}

	public void SyncTime()
	{
		float elapsedTime = masterController.GetElapsedTime();
		for (int i = 0; i < Targets.Count; i++)
		{
			KAnimControllerBase kAnimControllerBase = Targets[i];
			kAnimControllerBase.SetElapsedTime(elapsedTime);
		}
	}
}
