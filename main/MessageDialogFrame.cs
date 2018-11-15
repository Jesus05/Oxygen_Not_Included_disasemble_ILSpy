using System;
using UnityEngine;

public class MessageDialogFrame : KScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KToggle nextMessageButton;

	[SerializeField]
	private GameObject dontShowAgainElement;

	[SerializeField]
	private MultiToggle dontShowAgainButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private RectTransform body;

	private System.Action dontShowAgainDelegate;

	public override float GetSortKey()
	{
		return 9999f;
	}

	protected override void OnActivate()
	{
		closeButton.onClick += OnClickClose;
		nextMessageButton.onClick += OnClickNextMessage;
		MultiToggle multiToggle = dontShowAgainButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(OnClickDontShowAgain));
		bool flag = KPlayerPrefs.GetInt("HideTutorial_CheckState", 0) == 1;
		dontShowAgainButton.ChangeState((!flag) ? 1 : 0);
		Subscribe(Messenger.Instance.gameObject, -599791736, OnMessagesChanged);
		OnMessagesChanged(null);
	}

	protected override void OnDeactivate()
	{
		Unsubscribe(Messenger.Instance.gameObject, -599791736, OnMessagesChanged);
	}

	private void OnClickClose()
	{
		TryDontShowAgain();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnClickNextMessage()
	{
		TryDontShowAgain();
		UnityEngine.Object.Destroy(base.gameObject);
		NotificationScreen.Instance.OnClickNextMessage();
	}

	private void OnClickDontShowAgain()
	{
		dontShowAgainButton.NextState();
		KPlayerPrefs.SetInt("HideTutorial_CheckState", (dontShowAgainButton.CurrentState == 0) ? 1 : 0);
	}

	private void OnMessagesChanged(object data)
	{
		nextMessageButton.gameObject.SetActive(Messenger.Instance.Count != 0);
	}

	public void SetMessage(MessageDialog dialog, Message message)
	{
		title.text = message.GetTitle().ToUpper();
		dialog.GetComponent<RectTransform>().SetParent(body.GetComponent<RectTransform>());
		RectTransform component = dialog.GetComponent<RectTransform>();
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		dialog.transform.SetLocalPosition(Vector3.zero);
		dialog.SetMessage(message);
		dialog.OnClickAction();
		if (dialog.CanDontShowAgain)
		{
			dontShowAgainElement.SetActive(true);
			dontShowAgainDelegate = dialog.OnDontShowAgain;
		}
		else
		{
			dontShowAgainElement.SetActive(false);
			dontShowAgainDelegate = null;
		}
	}

	private void TryDontShowAgain()
	{
		if (dontShowAgainDelegate != null && dontShowAgainButton.CurrentState == 0)
		{
			dontShowAgainDelegate();
		}
	}
}
