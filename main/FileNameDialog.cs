using System;
using TMPro;
using UnityEngine;

public class FileNameDialog : KScreen
{
	public Action<string> onConfirm;

	public System.Action onCancel;

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private KButton confirmButton;

	[SerializeField]
	private KButton cancelButton;

	[SerializeField]
	private KButton closeButton;

	public override float GetSortKey()
	{
		return 1000f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		confirmButton.onClick += OnConfirm;
		cancelButton.onClick += OnCancel;
		closeButton.onClick += OnCancel;
		inputField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(inputField, false);
		});
		inputField.onEndEdit.AddListener(OnEndEdit);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		inputField.Select();
		inputField.ActivateInputField();
		CameraController.Instance.DisableUserCameraControl = true;
	}

	protected override void OnDeactivate()
	{
		CameraController.Instance.DisableUserCameraControl = false;
		base.OnDeactivate();
	}

	public void OnConfirm()
	{
		if (onConfirm != null && !string.IsNullOrEmpty(inputField.text))
		{
			string text = inputField.text;
			if (!text.EndsWith(".sav"))
			{
				text += ".sav";
			}
			onConfirm(text);
			Deactivate();
		}
	}

	private void OnEndEdit(string str)
	{
		if (Localization.HasDirtyWords(str))
		{
			inputField.text = string.Empty;
		}
	}

	public void OnCancel()
	{
		if (onCancel != null)
		{
			onCancel();
		}
		Deactivate();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		else if (e.TryConsume(Action.DialogSubmit))
		{
			OnConfirm();
		}
		e.Consumed = true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		e.Consumed = true;
	}
}
