using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Notifier : KMonoBehaviour
{
	[MyCmpGet]
	private KSelectable Selectable;

	public Action<Notification> OnAdd;

	public Action<Notification> OnRemove;

	public bool DisableNotifications;

	public bool AutoClickFocus = true;

	private Dictionary<HashedString, Notification> NotificationGroups;

	protected override void OnPrefabInit()
	{
		Components.Notifiers.Add(this);
	}

	protected override void OnCleanUp()
	{
		ClearNotifications();
		Components.Notifiers.Remove(this);
	}

	public void Add(Notification notification, string suffix = "")
	{
		if (!((UnityEngine.Object)KScreenManager.Instance == (UnityEngine.Object)null) && !DisableNotifications)
		{
			if ((UnityEngine.Object)notification.Notifier == (UnityEngine.Object)null)
			{
				if ((UnityEngine.Object)Selectable != (UnityEngine.Object)null)
				{
					notification.NotifierName = "• " + Selectable.GetName() + suffix;
				}
				else
				{
					notification.NotifierName = "• " + base.name + suffix;
				}
				notification.Notifier = this;
				if (AutoClickFocus && (UnityEngine.Object)notification.clickFocus == (UnityEngine.Object)null)
				{
					notification.clickFocus = base.transform;
				}
				if (notification.Group.IsValid && notification.Group != (HashedString)string.Empty)
				{
					if (NotificationGroups == null)
					{
						NotificationGroups = new Dictionary<HashedString, Notification>();
					}
					NotificationGroups.TryGetValue(notification.Group, out Notification value);
					if (value != null)
					{
						Remove(value);
					}
					NotificationGroups[notification.Group] = notification;
				}
				if (OnAdd != null)
				{
					OnAdd(notification);
				}
				notification.GameTime = Time.time;
			}
			else
			{
				DebugUtil.Assert((UnityEngine.Object)notification.Notifier == (UnityEngine.Object)this);
			}
			notification.Time = KTime.Instance.UnscaledGameTime;
		}
	}

	public void Remove(Notification notification)
	{
		if ((UnityEngine.Object)notification.Notifier != (UnityEngine.Object)null)
		{
			notification.Notifier = null;
			if (NotificationGroups != null && notification.Group.IsValid && notification.Group != (HashedString)string.Empty)
			{
				NotificationGroups.Remove(notification.Group);
			}
			if (OnRemove != null)
			{
				OnRemove(notification);
			}
		}
	}

	public void ClearNotifications()
	{
		if (NotificationGroups != null)
		{
			List<HashedString> list = new List<HashedString>(NotificationGroups.Keys);
			foreach (HashedString item in list)
			{
				Remove(NotificationGroups[item]);
			}
		}
	}
}
