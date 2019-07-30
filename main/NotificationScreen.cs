using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreen : KScreen
{
	private class Entry
	{
		public string message;

		public int clickIdx;

		public GameObject label;

		public List<Notification> notifications = new List<Notification>();

		public Notification NextClickedNotification => notifications[clickIdx++ % notifications.Count];

		public Entry(GameObject label)
		{
			this.label = label;
		}

		public void Add(Notification notification)
		{
			notifications.Add(notification);
			UpdateMessage(notification, true);
		}

		public void Remove(Notification notification)
		{
			notifications.Remove(notification);
			UpdateMessage(notification, false);
		}

		public void UpdateMessage(Notification notification, bool playSound = true)
		{
			if (!Game.IsQuitting())
			{
				message = notification.titleText;
				if (notifications.Count > 1)
				{
					if (playSound && (notification.Type == NotificationType.Bad || notification.Type == NotificationType.DuplicantThreatening))
					{
						Instance.PlayDingSound(notification, notifications.Count);
					}
					message = message + " (" + notifications.Count.ToString() + ")";
				}
				if ((UnityEngine.Object)label.gameObject != (UnityEngine.Object)null)
				{
					label.GetComponentInChildren<LocText>().text = message;
				}
			}
		}
	}

	public float lifetime;

	public bool dirty;

	public GameObject LabelPrefab;

	public GameObject LabelsFolder;

	public GameObject MessagesPrefab;

	public GameObject MessagesFolder;

	private MessageDialogFrame messageDialog;

	private float initTime;

	private int notificationIncrement;

	[MyCmpAdd]
	private Notifier notifier;

	[SerializeField]
	private List<MessageDialog> dialogPrefabs = new List<MessageDialog>();

	[SerializeField]
	private Color badColorBG;

	[SerializeField]
	private Color badColor = Color.red;

	[SerializeField]
	private Color normalColorBG;

	[SerializeField]
	private Color normalColor = Color.white;

	[SerializeField]
	private Color warningColorBG;

	[SerializeField]
	private Color warningColor;

	[SerializeField]
	private Color messageColorBG;

	[SerializeField]
	private Color messageColor;

	public Sprite icon_normal;

	public Sprite icon_warning;

	public Sprite icon_bad;

	public Sprite icon_message;

	private List<Notification> pendingNotifications = new List<Notification>();

	private List<Notification> notifications = new List<Notification>();

	public TextStyleSetting TooltipTextStyle;

	private Dictionary<NotificationType, string> notificationSounds = new Dictionary<NotificationType, string>();

	private Dictionary<string, float> timeOfLastNotification = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private List<Entry> entries = new List<Entry>();

	private Dictionary<string, Entry> entriesByMessage = new Dictionary<string, Entry>();

	public static NotificationScreen Instance
	{
		get;
		private set;
	}

	public Color32 BadColorBG => badColorBG;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void OnAddNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Combine(notifier.OnAdd, new Action<Notification>(OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Combine(notifier.OnRemove, new Action<Notification>(OnRemoveNotification));
	}

	private void OnRemoveNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Remove(notifier.OnAdd, new Action<Notification>(OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Remove(notifier.OnRemove, new Action<Notification>(OnRemoveNotification));
	}

	private void OnAddNotification(Notification notification)
	{
		pendingNotifications.Add(notification);
	}

	private void OnRemoveNotification(Notification notification)
	{
		dirty = true;
		pendingNotifications.Remove(notification);
		Entry value = null;
		entriesByMessage.TryGetValue(notification.titleText, out value);
		if (value != null)
		{
			notifications.Remove(notification);
			value.Remove(notification);
			if (value.notifications.Count == 0)
			{
				UnityEngine.Object.Destroy(value.label);
				entriesByMessage[notification.titleText] = null;
				entries.Remove(value);
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		Components.Notifiers.OnAdd += OnAddNotifier;
		Components.Notifiers.OnRemove += OnRemoveNotifier;
		foreach (Notifier item in Components.Notifiers.Items)
		{
			OnAddNotifier(item);
		}
		MessagesPrefab.gameObject.SetActive(false);
		LabelPrefab.gameObject.SetActive(false);
		InitNotificationSounds();
	}

	private void OnNewMessage(object data)
	{
		Message m = (Message)data;
		notifier.Add(new MessageNotification(m), string.Empty);
	}

	private void ShowMessage(MessageNotification mn)
	{
		mn.message.OnClick();
		if (mn.message.ShowDialog())
		{
			for (int i = 0; i < dialogPrefabs.Count; i++)
			{
				if (dialogPrefabs[i].CanDisplay(mn.message))
				{
					if ((UnityEngine.Object)messageDialog != (UnityEngine.Object)null)
					{
						UnityEngine.Object.Destroy(messageDialog.gameObject);
						messageDialog = null;
					}
					messageDialog = Util.KInstantiateUI<MessageDialogFrame>(ScreenPrefabs.Instance.MessageDialogFrame.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
					MessageDialog dialog = Util.KInstantiateUI<MessageDialog>(dialogPrefabs[i].gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
					messageDialog.SetMessage(dialog, mn.message);
					messageDialog.Show(true);
					break;
				}
			}
		}
		Messenger.Instance.RemoveMessage(mn.message);
		mn.Clear();
	}

	public void OnClickNextMessage()
	{
		Notification notification2 = notifications.Find((Notification notification) => notification.Type == NotificationType.Messages);
		ShowMessage((MessageNotification)notification2);
	}

	protected override void OnCleanUp()
	{
		Components.Notifiers.OnAdd -= OnAddNotifier;
		Components.Notifiers.OnRemove -= OnRemoveNotifier;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		initTime = KTime.Instance.UnscaledGameTime;
		LocText[] componentsInChildren = LabelPrefab.GetComponentsInChildren<LocText>();
		LocText[] array = componentsInChildren;
		foreach (LocText locText in array)
		{
			locText.color = normalColor;
		}
		componentsInChildren = MessagesPrefab.GetComponentsInChildren<LocText>();
		LocText[] array2 = componentsInChildren;
		foreach (LocText locText2 in array2)
		{
			locText2.color = normalColor;
		}
		Subscribe(Messenger.Instance.gameObject, 1558809273, OnNewMessage);
		foreach (Message message in Messenger.Instance.Messages)
		{
			Notification notification = new MessageNotification(message);
			notification.playSound = false;
			notifier.Add(notification, string.Empty);
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		dirty = true;
	}

	private void AddNotification(Notification notification)
	{
		notifications.Add(notification);
		notification.Idx = notificationIncrement++;
		Entry entry = null;
		entriesByMessage.TryGetValue(notification.titleText, out entry);
		if (entry == null)
		{
			GameObject label;
			if (notification.Type == NotificationType.Messages)
			{
				label = Util.KInstantiateUI(MessagesPrefab, MessagesFolder, false);
			}
			else
			{
				label = Util.KInstantiateUI(LabelPrefab, LabelsFolder, false);
			}
			label.GetComponentInChildren<NotificationAnimator>().Init();
			label.gameObject.SetActive(true);
			KImage componentInChildren = label.GetComponentInChildren<KImage>(true);
			Button[] componentsInChildren = label.gameObject.GetComponentsInChildren<Button>();
			ColorBlock colors = componentsInChildren[0].colors;
			if (notification.Type == NotificationType.Bad || notification.Type == NotificationType.DuplicantThreatening)
			{
				colors.normalColor = badColorBG;
			}
			else if (notification.Type == NotificationType.Messages)
			{
				colors.normalColor = messageColorBG;
				Debug.Assert(notification.GetType() == typeof(MessageNotification), $"Notification: \"{notification.titleText}\" is not of type MessageNotification");
				componentsInChildren[1].onClick.AddListener(delegate
				{
					List<Notification> list = notifications.FindAll((Notification n) => n.titleText == notification.titleText);
					foreach (Notification item in list)
					{
						MessageNotification messageNotification = (MessageNotification)item;
						Messenger.Instance.RemoveMessage(messageNotification.message);
						messageNotification.Clear();
					}
				});
			}
			else if (notification.Type == NotificationType.Tutorial)
			{
				colors.normalColor = warningColorBG;
			}
			else
			{
				colors.normalColor = normalColorBG;
			}
			componentsInChildren[0].colors = colors;
			componentsInChildren[0].onClick.AddListener(delegate
			{
				OnClick(entry);
			});
			if (notification.ToolTip != null)
			{
				label.GetComponentInChildren<ToolTip>().OnToolTip = delegate
				{
					ToolTip componentInChildren2 = label.GetComponentInChildren<ToolTip>();
					componentInChildren2.ClearMultiStringTooltip();
					componentInChildren2.AddMultiStringTooltip(notification.ToolTip(entry.notifications, notification.tooltipData), TooltipTextStyle);
					return string.Empty;
				};
			}
			entry = new Entry(label);
			entriesByMessage[notification.titleText] = entry;
			entries.Add(entry);
			LocText[] componentsInChildren2 = label.GetComponentsInChildren<LocText>();
			LocText[] array = componentsInChildren2;
			foreach (LocText locText in array)
			{
				switch (notification.Type)
				{
				case NotificationType.Bad:
					locText.color = badColor;
					componentInChildren.sprite = icon_bad;
					break;
				case NotificationType.DuplicantThreatening:
					locText.color = badColor;
					componentInChildren.sprite = icon_bad;
					break;
				case NotificationType.Tutorial:
					locText.color = warningColor;
					componentInChildren.sprite = icon_warning;
					break;
				case NotificationType.Messages:
					locText.color = messageColor;
					componentInChildren.sprite = icon_message;
					break;
				default:
					locText.color = normalColor;
					componentInChildren.sprite = icon_normal;
					break;
				}
				componentInChildren.color = locText.color;
				string str = string.Empty;
				if (KTime.Instance.UnscaledGameTime - initTime > 5f && notification.playSound)
				{
					PlayDingSound(notification, 0);
				}
				else
				{
					str = "too early";
				}
				if (AudioDebug.Get().debugNotificationSounds)
				{
					Debug.Log("Notification(" + notification.titleText + "):" + str);
				}
			}
		}
		entry.Add(notification);
		entry.UpdateMessage(notification, true);
		dirty = true;
		SortNotifications();
	}

	private void SortNotifications()
	{
		notifications.Sort(delegate(Notification n1, Notification n2)
		{
			if (n1.Type == n2.Type)
			{
				return n1.Idx - n2.Idx;
			}
			return n1.Type - n2.Type;
		});
		foreach (Notification notification in notifications)
		{
			Entry value = null;
			entriesByMessage.TryGetValue(notification.titleText, out value);
			value?.label.GetComponent<RectTransform>().SetAsLastSibling();
		}
	}

	private void PlayDingSound(Notification notification, int count)
	{
		if (!notificationSounds.TryGetValue(notification.Type, out string value))
		{
			value = "Notification";
		}
		if (!timeOfLastNotification.TryGetValue(value, out float value2))
		{
			value2 = 0f;
		}
		float value3 = (Time.time - value2) / soundDecayTime;
		timeOfLastNotification[value] = Time.time;
		string sound;
		if (count > 1)
		{
			sound = GlobalAssets.GetSound(value + "_AddCount", true);
			if (sound == null)
			{
				sound = GlobalAssets.GetSound(value, false);
			}
		}
		else
		{
			sound = GlobalAssets.GetSound(value, false);
		}
		if (notification.playSound)
		{
			EventInstance instance = KFMOD.BeginOneShot(sound, Vector3.zero);
			instance.setParameterValue("timeSinceLast", value3);
			KFMOD.EndOneShot(instance);
		}
	}

	private void Update()
	{
		int num = 0;
		while (num < pendingNotifications.Count)
		{
			if (pendingNotifications[num].IsReady())
			{
				AddNotification(pendingNotifications[num]);
				pendingNotifications.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < notifications.Count; i++)
		{
			Notification notification = notifications[i];
			if (notification.Type == NotificationType.Messages)
			{
				num3++;
			}
			else
			{
				num2++;
			}
			if (notification.expires && KTime.Instance.UnscaledGameTime - notification.Time > lifetime)
			{
				dirty = true;
				if ((UnityEngine.Object)notification.Notifier == (UnityEngine.Object)null)
				{
					OnRemoveNotification(notification);
				}
				else
				{
					notification.Clear();
				}
			}
		}
	}

	private void OnClick(Entry entry)
	{
		Notification nextClickedNotification = entry.NextClickedNotification;
		PlaySound3D(GlobalAssets.GetSound("HUD_Click_Open", false));
		if (nextClickedNotification.customClickCallback != null)
		{
			nextClickedNotification.customClickCallback(nextClickedNotification.customClickData);
		}
		else
		{
			if ((UnityEngine.Object)nextClickedNotification.clickFocus != (UnityEngine.Object)null)
			{
				Vector3 position = nextClickedNotification.clickFocus.GetPosition();
				position.z = -40f;
				CameraController.Instance.SetTargetPos(position, 8f, true);
				if ((UnityEngine.Object)nextClickedNotification.clickFocus.GetComponent<KSelectable>() != (UnityEngine.Object)null)
				{
					SelectTool.Instance.Select(nextClickedNotification.clickFocus.GetComponent<KSelectable>(), false);
				}
			}
			else if ((UnityEngine.Object)nextClickedNotification.Notifier != (UnityEngine.Object)null)
			{
				SelectTool.Instance.Select(nextClickedNotification.Notifier.GetComponent<KSelectable>(), false);
			}
			if (nextClickedNotification.Type == NotificationType.Messages)
			{
				ShowMessage((MessageNotification)nextClickedNotification);
			}
		}
	}

	private void PositionLocatorIcon()
	{
	}

	private void InitNotificationSounds()
	{
		notificationSounds[NotificationType.Good] = "Notification";
		notificationSounds[NotificationType.BadMinor] = "Notification";
		notificationSounds[NotificationType.Bad] = "Warning";
		notificationSounds[NotificationType.Neutral] = "Notification";
		notificationSounds[NotificationType.Tutorial] = "Notification";
		notificationSounds[NotificationType.Messages] = "Message";
		notificationSounds[NotificationType.DuplicantThreatening] = "Warning_DupeThreatening";
	}
}
