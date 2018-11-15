using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EditableTitleBar : TitleBar
{
	public KButton editNameButton;

	public KButton randomNameButton;

	public TMP_InputField inputField;

	private Coroutine postEndEdit;

	private Coroutine preToggleNameEditing;

	public event Action<string> OnNameChanged;

	public event System.Action OnStartedEditing;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((UnityEngine.Object)randomNameButton != (UnityEngine.Object)null)
		{
			randomNameButton.onClick += GenerateRandomName;
		}
		if ((UnityEngine.Object)editNameButton != (UnityEngine.Object)null)
		{
			EnableEditButtonClick();
		}
		if ((UnityEngine.Object)inputField != (UnityEngine.Object)null)
		{
			inputField.onEndEdit.AddListener(OnEndEdit);
		}
	}

	private void OnEndEdit(string finalStr)
	{
		finalStr = Localization.FilterDirtyWords(finalStr);
		SetEditingState(false);
		if (!string.IsNullOrEmpty(finalStr))
		{
			if (this.OnNameChanged != null)
			{
				this.OnNameChanged(finalStr);
			}
			titleText.text = finalStr;
			if (postEndEdit != null)
			{
				StopCoroutine(postEndEdit);
			}
			if (base.gameObject.activeInHierarchy && base.enabled)
			{
				postEndEdit = StartCoroutine(PostOnEndEditRoutine());
			}
		}
	}

	private IEnumerator PostOnEndEditRoutine()
	{
		int i = 0;
		if (i < 10)
		{
			yield return (object)new WaitForEndOfFrame();
			/*Error: Unable to find new state assignment for yield return*/;
		}
		EnableEditButtonClick();
		if ((UnityEngine.Object)randomNameButton != (UnityEngine.Object)null)
		{
			randomNameButton.gameObject.SetActive(false);
		}
	}

	private IEnumerator PreToggleNameEditingRoutine()
	{
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void EnableEditButtonClick()
	{
		editNameButton.onClick += delegate
		{
			if (preToggleNameEditing == null)
			{
				preToggleNameEditing = StartCoroutine(PreToggleNameEditingRoutine());
			}
		};
	}

	private void GenerateRandomName()
	{
		if (postEndEdit != null)
		{
			StopCoroutine(postEndEdit);
		}
		string text = GameUtil.GenerateRandomDuplicantName();
		if (this.OnNameChanged != null)
		{
			this.OnNameChanged(text);
		}
		titleText.text = text;
		SetEditingState(true);
	}

	private void ToggleNameEditing()
	{
		editNameButton.ClearOnClick();
		bool flag = !inputField.gameObject.activeInHierarchy;
		if ((UnityEngine.Object)randomNameButton != (UnityEngine.Object)null)
		{
			randomNameButton.gameObject.SetActive(flag);
		}
		SetEditingState(flag);
	}

	private void SetEditingState(bool state)
	{
		titleText.gameObject.SetActive(!state);
		if (setCameraControllerState)
		{
			CameraController.Instance.DisableUserCameraControl = state;
		}
		if (!((UnityEngine.Object)inputField == (UnityEngine.Object)null))
		{
			inputField.gameObject.SetActive(state);
			if (state)
			{
				inputField.text = titleText.text;
				inputField.Select();
				inputField.ActivateInputField();
				if (this.OnStartedEditing != null)
				{
					this.OnStartedEditing();
				}
			}
			else
			{
				inputField.DeactivateInputField();
			}
		}
	}

	public void ForceStopEditing()
	{
		if (postEndEdit != null)
		{
			StopCoroutine(postEndEdit);
		}
		editNameButton.ClearOnClick();
		SetEditingState(false);
		EnableEditButtonClick();
	}

	public void SetUserEditable(bool editable)
	{
		userEditable = editable;
		editNameButton.gameObject.SetActive(editable);
		editNameButton.ClearOnClick();
		EnableEditButtonClick();
	}
}
