using STRINGS;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialogScreen : KModalScreen
{
	private System.Action confirmAction;

	private System.Action cancelAction;

	private System.Action configurableAction;

	public System.Action onDeactivateCB;

	[SerializeField]
	private GameObject confirmButton;

	[SerializeField]
	private GameObject cancelButton;

	[SerializeField]
	private GameObject configurableButton;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText popupMessage;

	[SerializeField]
	private Image image;

	[SerializeField]
	private GameObject fadeBG;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(false);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			OnSelect_CANCEL();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public void PopupConfirmDialog(string text, System.Action on_confirm, System.Action on_cancel, string configurable_text = null, System.Action on_configurable_clicked = null, string title_text = null, string confirm_text = null, string cancel_text = null, Sprite image_sprite = null, bool activateBlackBackground = true)
	{
		while ((UnityEngine.Object)base.transform.parent.GetComponent<Canvas>() == (UnityEngine.Object)null && (UnityEngine.Object)base.transform.parent.parent != (UnityEngine.Object)null)
		{
			base.transform.SetParent(base.transform.parent.parent);
		}
		base.transform.SetAsLastSibling();
		fadeBG.SetActive(activateBlackBackground);
		confirmAction = on_confirm;
		cancelAction = on_cancel;
		configurableAction = on_configurable_clicked;
		int num = 0;
		if (confirmAction != null)
		{
			num++;
		}
		if (cancelAction != null)
		{
			num++;
		}
		if (configurableAction != null)
		{
			num++;
		}
		confirmButton.GetComponentInChildren<LocText>().text = ((confirm_text != null) ? confirm_text : UI.CONFIRMDIALOG.OK.text);
		cancelButton.GetComponentInChildren<LocText>().text = ((cancel_text != null) ? cancel_text : UI.CONFIRMDIALOG.CANCEL.text);
		confirmButton.GetComponent<KButton>().onClick += OnSelect_OK;
		cancelButton.GetComponent<KButton>().onClick += OnSelect_CANCEL;
		configurableButton.GetComponent<KButton>().onClick += OnSelect_third;
		cancelButton.SetActive(on_cancel != null);
		if ((UnityEngine.Object)configurableButton != (UnityEngine.Object)null)
		{
			configurableButton.SetActive(configurableAction != null);
			if (configurable_text != null)
			{
				LocText componentInChildren = configurableButton.GetComponentInChildren<LocText>();
				componentInChildren.text = configurable_text;
			}
		}
		if ((UnityEngine.Object)image_sprite != (UnityEngine.Object)null)
		{
			image.sprite = image_sprite;
			image.gameObject.SetActive(true);
		}
		if (title_text != null)
		{
			titleText.text = title_text;
		}
		popupMessage.text = text;
	}

	public void OnSelect_OK()
	{
		Deactivate();
		if (confirmAction != null)
		{
			confirmAction();
		}
	}

	public void OnSelect_CANCEL()
	{
		Deactivate();
		if (cancelAction != null)
		{
			cancelAction();
		}
	}

	public void OnSelect_third()
	{
		Deactivate();
		if (configurableAction != null)
		{
			configurableAction();
		}
	}

	protected override void OnDeactivate()
	{
		if (onDeactivateCB != null)
		{
			onDeactivateCB();
		}
		base.OnDeactivate();
	}
}
