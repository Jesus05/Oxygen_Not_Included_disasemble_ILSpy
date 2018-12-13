using UnityEngine;
using UnityEngine.UI;

public class KModalScreen : KScreen
{
	private bool shown;

	public bool pause = true;

	public const float SCREEN_SORT_KEY = 100f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		activateOnSpawn = true;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if ((Object)CameraController.Instance != (Object)null)
		{
			CameraController.Instance.DisableUserCameraControl = true;
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((Object)CameraController.Instance != (Object)null)
		{
			CameraController.Instance.DisableUserCameraControl = false;
		}
		Trigger(476357528, null);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnActivate()
	{
		OnShow(true);
	}

	protected override void OnDeactivate()
	{
		OnShow(false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (pause && (Object)SpeedControlScreen.Instance != (Object)null)
		{
			if (show && !shown)
			{
				SpeedControlScreen.Instance.Pause(false);
			}
			else if (!show && shown)
			{
				SpeedControlScreen.Instance.Unpause(false);
			}
			shown = show;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if ((Object)Game.Instance != (Object)null && (e.TryConsume(Action.TogglePause) || e.TryConsume(Action.CycleSpeed)))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		if (!e.Consumed && e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	public void SetBackgroundActive(bool active)
	{
		int num = active ? 70 : 0;
		GetComponent<Image>().color = new Color32(0, 0, 0, (byte)num);
	}
}
