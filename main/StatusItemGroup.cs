using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StatusItemGroup
{
	public struct Entry : IComparable<Entry>, IEquatable<Entry>
	{
		public Guid id;

		public StatusItem item;

		public object data;

		public Notification notification;

		public StatusItemCategory category;

		public Entry(StatusItem item, StatusItemCategory category, object data)
		{
			id = Guid.NewGuid();
			this.item = item;
			this.data = data;
			this.category = category;
			notification = null;
		}

		public string GetName()
		{
			return item.GetName(data);
		}

		public void ShowToolTip(ToolTip tooltip_widget, TextStyleSetting property_style)
		{
			item.ShowToolTip(tooltip_widget, data, property_style);
		}

		public void SetIcon(Image image)
		{
			item.SetIcon(image, data);
		}

		public int CompareTo(Entry other)
		{
			return id.CompareTo(other.id);
		}

		public bool Equals(Entry other)
		{
			return id == other.id;
		}
	}

	private List<Entry> items = new List<Entry>();

	public Action<Entry, StatusItemCategory> OnAddStatusItem;

	public Action<Entry, bool> OnRemoveStatusItem;

	private Vector3 offset = new Vector3(0f, 0f, 0f);

	[CompilerGenerated]
	private static Func<List<Notification>, object, string> _003C_003Ef__mg_0024cache0;

	public GameObject gameObject
	{
		get;
		private set;
	}

	public StatusItemGroup(GameObject go)
	{
		gameObject = go;
	}

	public IEnumerator<Entry> GetEnumerator()
	{
		return items.GetEnumerator();
	}

	public void SetOffset(Vector3 offset)
	{
		this.offset = offset;
		Game.Instance.SetStatusItemOffset(gameObject.transform, offset);
	}

	public Entry GetStatusItem(StatusItemCategory category)
	{
		for (int i = 0; i < items.Count; i++)
		{
			Entry entry = items[i];
			if (entry.category == category)
			{
				return items[i];
			}
		}
		return default(Entry);
	}

	public Guid SetStatusItem(StatusItemCategory category, StatusItem item, object data = null)
	{
		if (item != null && item.allowMultiples)
		{
			throw new ArgumentException(item.Name + " allows multiple instances of itself to be active so you must access it via its handle");
		}
		if (category == null)
		{
			throw new ArgumentException("SetStatusItem requires a category.");
		}
		for (int i = 0; i < items.Count; i++)
		{
			Entry entry = items[i];
			if (entry.category == category)
			{
				Entry entry2 = items[i];
				if (entry2.item == item)
				{
					Entry entry3 = items[i];
					Log("Set (exists in category)", item, entry3.id, category);
					Entry entry4 = items[i];
					return entry4.id;
				}
				Entry entry5 = items[i];
				Log("Set->Remove existing in category", item, entry5.id, category);
				Entry entry6 = items[i];
				RemoveStatusItem(entry6.id, false);
			}
		}
		if (item == null)
		{
			Log("Set (failed)", item, Guid.Empty, category);
			return Guid.Empty;
		}
		Guid guid = AddStatusItem(item, data, category);
		Log("Set (new)", item, guid, category);
		return guid;
	}

	public void SetStatusItem(Guid guid, StatusItemCategory category, StatusItem new_item, object data = null)
	{
		RemoveStatusItem(guid, false);
		if (new_item != null)
		{
			AddStatusItem(new_item, data, category);
		}
	}

	public bool HasStatusItem(StatusItem status_item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			Entry entry = items[i];
			if (entry.item.Id == status_item.Id)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasStatusItemID(StatusItem status_item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			Entry entry = items[i];
			if (entry.item.Id == status_item.Id)
			{
				return true;
			}
		}
		return false;
	}

	public Guid AddStatusItem(StatusItem item, object data = null, StatusItemCategory category = null)
	{
		if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && (item.allowMultiples || !HasStatusItem(item)))
		{
			if (!item.allowMultiples)
			{
				foreach (Entry item2 in items)
				{
					Entry current = item2;
					if (current.item.Id == item.Id)
					{
						throw new ArgumentException("Tried to add " + item.Id + " multiples times which is not permitted.");
					}
				}
			}
			Entry entry = new Entry(item, category, data);
			if (item.shouldNotify)
			{
				entry.notification = new Notification(item.notificationText, item.notificationType, HashedString.Invalid, OnToolTip, expires: false, custom_click_callback: item.notificationClickCallback, tooltip_data: item, delay: item.notificationDelay, custom_click_data: data);
				gameObject.AddOrGet<Notifier>().Add(entry.notification, "");
			}
			if (item.ShouldShowIcon())
			{
				Game.Instance.AddStatusItem(gameObject.transform, item);
				Game.Instance.SetStatusItemOffset(gameObject.transform, offset);
			}
			items.Add(entry);
			if (OnAddStatusItem != null)
			{
				OnAddStatusItem(entry, category);
			}
			return entry.id;
		}
		return Guid.Empty;
	}

	public Guid RemoveStatusItem(StatusItem status_item, bool immediate = false)
	{
		if (status_item.allowMultiples)
		{
			throw new ArgumentException(status_item.Name + " allows multiple instances of itself to be active so it must be released via an instance handle");
		}
		for (int i = 0; i < items.Count; i++)
		{
			Entry entry = items[i];
			if (entry.item.Id == status_item.Id)
			{
				Entry entry2 = items[i];
				return RemoveStatusItem(entry2.id, immediate);
			}
		}
		return Guid.Empty;
	}

	public Guid RemoveStatusItem(Guid guid, bool immediate = false)
	{
		if (!(guid == Guid.Empty))
		{
			for (int i = 0; i < items.Count; i++)
			{
				Entry entry = items[i];
				if (entry.id == guid)
				{
					Entry arg = items[i];
					items.RemoveAt(i);
					if (arg.notification != null)
					{
						gameObject.GetComponent<Notifier>().Remove(arg.notification);
					}
					if (entry.item.ShouldShowIcon() && (UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
					{
						Game.Instance.RemoveStatusItem(gameObject.transform, arg.item);
					}
					if (OnRemoveStatusItem != null)
					{
						OnRemoveStatusItem(arg, immediate);
					}
					return guid;
				}
			}
			return Guid.Empty;
		}
		return guid;
	}

	private static string OnToolTip(List<Notification> notifications, object data)
	{
		StatusItem statusItem = (StatusItem)data;
		string text = statusItem.notificationTooltipText;
		foreach (Notification notification in notifications)
		{
			if (notification != null && (UnityEngine.Object)notification.Notifier != (UnityEngine.Object)null)
			{
				KSelectable component = notification.Notifier.GetComponent<KSelectable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					text = text + "\n• " + component.GetName();
				}
			}
		}
		return text;
	}

	public void Destroy()
	{
		if (!Game.IsQuitting())
		{
			while (items.Count > 0)
			{
				Entry entry = items[0];
				RemoveStatusItem(entry.id, false);
			}
		}
	}

	[Conditional("ENABLE_LOGGER")]
	private void Log(string action, StatusItem item, Guid guid)
	{
	}

	private void Log(string action, StatusItem item, Guid guid, StatusItemCategory category)
	{
	}
}
