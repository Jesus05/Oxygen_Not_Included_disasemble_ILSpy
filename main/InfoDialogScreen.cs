using System;
using System.Collections.Generic;
using UnityEngine;

public class InfoDialogScreen : KModalScreen
{
	[SerializeField]
	private InfoScreenPlainText subHeaderTemplate;

	[SerializeField]
	private InfoScreenPlainText plainTextTemplate;

	[SerializeField]
	private InfoScreenLineItem lineItemTemplate;

	[Space(10f)]
	[SerializeField]
	private LocText header;

	[SerializeField]
	private GameObject contentContainer;

	[SerializeField]
	private GameObject confirmButton;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject buttonPanel;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(false);
		confirmButton.GetComponent<KButton>().onClick += OnSelect_OK;
	}

	public override bool IsModal()
	{
		return true;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			OnSelect_OK();
		}
		else if (PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			OnSelect_OK();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public void AddOption(string text, Action<InfoDialogScreen> action)
	{
		GameObject gameObject = Util.KInstantiateUI(buttonPrefab, buttonPanel, true);
		gameObject.gameObject.GetComponentInChildren<LocText>().text = text;
		gameObject.gameObject.GetComponent<KButton>().onClick += delegate
		{
			action(this);
		};
	}

	public InfoDialogScreen SetHeader(string header)
	{
		this.header.text = header;
		return this;
	}

	public InfoDialogScreen AddPlainText(string text)
	{
		InfoScreenPlainText component = Util.KInstantiateUI(plainTextTemplate.gameObject, contentContainer, false).GetComponent<InfoScreenPlainText>();
		component.SetText(text);
		return this;
	}

	public InfoDialogScreen AddLineItem(string text, string tooltip)
	{
		InfoScreenLineItem component = Util.KInstantiateUI(lineItemTemplate.gameObject, contentContainer, false).GetComponent<InfoScreenLineItem>();
		component.SetText(text);
		component.SetTooltip(tooltip);
		return this;
	}

	public InfoDialogScreen AddSubHeader(string text)
	{
		InfoScreenPlainText component = Util.KInstantiateUI(subHeaderTemplate.gameObject, contentContainer, false).GetComponent<InfoScreenPlainText>();
		component.SetText(text);
		return this;
	}

	public InfoDialogScreen AddDescriptors(List<Descriptor> descriptors)
	{
		for (int i = 0; i < descriptors.Count; i++)
		{
			string text = descriptors[i].IndentedText();
			Descriptor descriptor = descriptors[i];
			AddLineItem(text, descriptor.tooltipText);
		}
		return this;
	}

	public void OnSelect_OK()
	{
		Deactivate();
	}
}
