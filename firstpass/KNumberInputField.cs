using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KNumberInputField : KScreen
{
	public int decimalPlaces = -1;

	public float currentValue;

	public float minValue;

	public float maxValue;

	private bool isEditing;

	[SerializeField]
	private TMP_InputField inputField;

	public TMP_InputField field => inputField;

	public event System.Action onEndEdit;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TMP_InputField tMP_InputField = inputField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, new System.Action(OnEditStart));
		inputField.onEndEdit.AddListener(delegate
		{
			OnEditEnd(inputField.text);
		});
	}

	private void OnEditStart()
	{
		isEditing = true;
		KScreenManager.Instance.RefreshStack();
	}

	private void OnEditEnd(string input)
	{
		if (base.gameObject.activeInHierarchy)
		{
			ProcessInput(input);
			StartCoroutine(DelayedEndEdit());
		}
		else
		{
			StopEditing();
		}
	}

	private IEnumerator DelayedEndEdit()
	{
		if (isEditing)
		{
			yield return (object)new WaitForEndOfFrame();
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}

	private void StopEditing()
	{
		isEditing = false;
		if (this.onEndEdit != null)
		{
			this.onEndEdit();
		}
	}

	private void ProcessInput(string input)
	{
		input = ((!(input == "")) ? input : minValue.ToString());
		float value = minValue;
		try
		{
			value = float.Parse(input);
		}
		catch
		{
		}
		value = Mathf.Clamp(value, minValue, maxValue);
		if (decimalPlaces != -1)
		{
			float num = Mathf.Pow(10f, (float)decimalPlaces);
			value = Mathf.Round(value * num) / num;
		}
		currentValue = value;
	}

	public void SetDisplayValue(string input)
	{
		inputField.text = input;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override float GetSortKey()
	{
		if (!isEditing)
		{
			return base.GetSortKey();
		}
		return 10f;
	}
}
