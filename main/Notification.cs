using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Notification
{
	public delegate void ClickCallback(object data);

	public HashedString Group;

	public object tooltipData;

	public bool expires = true;

	public bool playSound = true;

	public ClickCallback customClickCallback;

	public object customClickData;

	private string notifierName;

	public NotificationType Type
	{
		get;
		set;
	}

	public Notifier Notifier
	{
		get;
		set;
	}

	public Transform clickFocus
	{
		get;
		set;
	}

	public float Time
	{
		get;
		set;
	}

	public float GameTime
	{
		get;
		set;
	}

	public float Delay
	{
		get;
		set;
	}

	public int Idx
	{
		get;
		set;
	}

	public Func<List<Notification>, object, string> ToolTip
	{
		get;
		set;
	}

	public string titleText
	{
		get;
		private set;
	}

	public string NotifierName
	{
		get
		{
			return notifierName;
		}
		set
		{
			notifierName = value;
			titleText = ReplaceTags(titleText);
		}
	}

	public Notification(string title, NotificationType type, HashedString group, Func<List<Notification>, object, string> tooltip = null, object tooltip_data = null, bool expires = true, float delay = 0f, ClickCallback custom_click_callback = null, object custom_click_data = null, Transform click_focus = null)
	{
		titleText = title;
		Group = group;
		Type = type;
		ToolTip = tooltip;
		tooltipData = tooltip_data;
		this.expires = expires;
		Delay = delay;
		customClickCallback = custom_click_callback;
		customClickData = custom_click_data;
		clickFocus = click_focus;
	}

	public bool IsReady()
	{
		return UnityEngine.Time.time >= GameTime + Delay;
	}

	public void Clear()
	{
		if ((UnityEngine.Object)Notifier != (UnityEngine.Object)null)
		{
			Notifier.Remove(this);
		}
	}

	private string ReplaceTags(string text)
	{
		DebugUtil.Assert(text != null);
		int num = text.IndexOf('{');
		int num2 = text.IndexOf('}');
		if (0 <= num && num < num2)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = 0;
			while (0 <= num)
			{
				string value = text.Substring(num3, num - num3);
				stringBuilder.Append(value);
				num2 = text.IndexOf('}', num);
				if (num >= num2)
				{
					break;
				}
				string tag = text.Substring(num + 1, num2 - num - 1);
				string tagDescription = GetTagDescription(tag);
				stringBuilder.Append(tagDescription);
				num3 = num2 + 1;
				num = text.IndexOf('{', num2);
			}
			stringBuilder.Append(text.Substring(num3, text.Length - num3));
			return stringBuilder.ToString();
		}
		return text;
	}

	private string GetTagDescription(string tag)
	{
		string text = null;
		if (!(tag == "NotifierName"))
		{
			return "UNKNOWN TAG: " + tag;
		}
		return notifierName;
	}
}
