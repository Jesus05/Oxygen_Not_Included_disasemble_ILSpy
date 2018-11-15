using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizableDialogScreen : KModalScreen
{
	private struct Button
	{
		public System.Action action;

		public GameObject gameObject;

		public string label;
	}

	public System.Action onDeactivateCB;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject buttonPanel;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText popupMessage;

	[SerializeField]
	private Image image;

	private List<Button> buttons;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(false);
		buttons = new List<Button>();
	}

	public override bool IsModal()
	{
		return true;
	}

	public void AddOption(string text, System.Action action)
	{
		GameObject gameObject = Util.KInstantiateUI(buttonPrefab, buttonPanel, true);
		buttons.Add(new Button
		{
			label = text,
			action = action,
			gameObject = gameObject
		});
	}

	public void PopupConfirmDialog(string text, string title_text = null, Sprite image_sprite = null)
	{
		foreach (Button button in buttons)
		{
			Button current = button;
			current.gameObject.GetComponentInChildren<LocText>().text = current.label;
			current.gameObject.GetComponent<KButton>().onClick += current.action;
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

	protected override void OnDeactivate()
	{
		if (onDeactivateCB != null)
		{
			onDeactivateCB();
		}
		base.OnDeactivate();
	}
}
