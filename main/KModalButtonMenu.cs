using UnityEngine;

public class KModalButtonMenu : KButtonMenu
{
	private bool shown;

	[SerializeField]
	private GameObject panelRoot;

	private GameObject childDialog;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		activateOnSpawn = true;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((Object)childDialog == (Object)null)
		{
			Trigger(476357528, null);
		}
	}

	public override bool IsModal()
	{
		return true;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if ((Object)SpeedControlScreen.Instance != (Object)null)
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
		if ((Object)CameraController.Instance != (Object)null)
		{
			CameraController.Instance.DisableUserCameraControl = show;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		base.OnKeyUp(e);
		e.Consumed = true;
	}

	public void SetBackgroundActive(bool active)
	{
	}

	protected GameObject ActivateChildScreen(GameObject screenPrefab)
	{
		GameObject gameObject = childDialog = Util.KInstantiateUI(screenPrefab, base.transform.parent.gameObject, false);
		gameObject.Subscribe(476357528, Unhide);
		Hide();
		return gameObject;
	}

	private void Hide()
	{
		panelRoot.rectTransform().localScale = Vector3.zero;
	}

	private void Unhide(object data = null)
	{
		panelRoot.rectTransform().localScale = Vector3.one;
		childDialog.Unsubscribe(476357528, Unhide);
		childDialog = null;
	}
}
