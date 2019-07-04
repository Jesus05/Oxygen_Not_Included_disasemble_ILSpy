using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryMessageScreen : KScreen
{
	private const float ALPHA_SPEED = 0.01f;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private GameObject dialog;

	[SerializeField]
	private KButton button;

	[SerializeField]
	[EventRef]
	private string dialogSound;

	[SerializeField]
	private LocText titleLabel;

	[SerializeField]
	private LocText bodyLabel;

	private const float expandedHeight = 300f;

	[SerializeField]
	private GameObject content;

	public bool restoreInterfaceOnClose = true;

	public System.Action OnClose;

	private bool startFade = false;

	public string title
	{
		set
		{
			titleLabel.SetText(value);
		}
	}

	public string body
	{
		set
		{
			bodyLabel.SetText(value);
		}
	}

	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		HideInterface(true);
		CameraController.Instance.FadeOut(0.5f);
	}

	private IEnumerator ExpandPanel()
	{
		content.gameObject.SetActive(true);
		yield return (object)new WaitForSecondsRealtime(0.25f);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private IEnumerator CollapsePanel()
	{
		float height2 = 300f;
		if (height2 > 0f)
		{
			Vector2 sizeDelta = dialog.rectTransform().sizeDelta;
			height2 = Mathf.Lerp(sizeDelta.y, -1f, Time.unscaledDeltaTime * 15f);
			RectTransform rectTransform = dialog.rectTransform();
			Vector2 sizeDelta2 = dialog.rectTransform().sizeDelta;
			rectTransform.sizeDelta = new Vector2(sizeDelta2.x, height2);
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		content.gameObject.SetActive(false);
		if (OnClose != null)
		{
			OnClose();
			OnClose = null;
		}
		Deactivate();
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public static void HideInterface(bool hide)
	{
		NotificationScreen.Instance.Show(!hide);
		OverlayMenu.Instance.Show(!hide);
		if ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null)
		{
			PlanScreen.Instance.Show(!hide);
		}
		if ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null)
		{
			BuildMenu.Instance.Show(!hide);
		}
		ManagementMenu.Instance.Show(!hide);
		ToolMenu.Instance.Show(!hide);
		ToolMenu.Instance.PriorityScreen.Show(!hide);
		ResourceCategoryScreen.Instance.Show(!hide);
		TopLeftControlScreen.Instance.Show(!hide);
		DateTime.Instance.Show(!hide);
		BuildWatermark.Instance.Show(!hide);
		PopFXManager.Instance.Show(!hide);
	}

	public void Update()
	{
		if (startFade)
		{
			Color color = bg.color;
			color.a -= 0.01f;
			if (color.a <= 0f)
			{
				color.a = 0f;
			}
			bg.color = color;
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SelectTool.Instance.Select(null, false);
		button.onClick += delegate
		{
			StartCoroutine(CollapsePanel());
		};
		dialog.GetComponent<KScreen>().Show(false);
		startFade = false;
		CameraController.Instance.DisableUserCameraControl = true;
		KFMOD.PlayOneShot(dialogSound);
		dialog.GetComponent<KScreen>().Activate();
		dialog.GetComponent<KScreen>().SetShouldFadeIn(true);
		dialog.GetComponent<KScreen>().Show(true);
		MusicManager.instance.PlaySong("Music_Victory_01_Message", false);
		StartCoroutine(ExpandPanel());
	}

	protected override void OnDeactivate()
	{
		if (!IsActive())
		{
			goto IL_000e;
		}
		goto IL_000e;
		IL_000e:
		base.OnDeactivate();
		MusicManager.instance.StopSong("Music_Victory_01_Message", true, STOP_MODE.ALLOWFADEOUT);
		if (restoreInterfaceOnClose)
		{
			CameraController.Instance.DisableUserCameraControl = false;
			CameraController.Instance.FadeIn(0f);
			HideInterface(false);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			StartCoroutine(CollapsePanel());
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}
}
