using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusItem : Resource
{
	public enum IconType
	{
		Info,
		Exclamation,
		Custom
	}

	[Flags]
	public enum StatusItemOverlays
	{
		None = 0x2,
		PowerMap = 0x4,
		Temperature = 0x8,
		ThermalComfort = 0x10,
		Light = 0x20,
		LiquidPlumbing = 0x40,
		GasPlumbing = 0x80,
		Decor = 0x100,
		Pathogens = 0x200,
		Farming = 0x400,
		Rooms = 0x1000,
		Suits = 0x2000,
		Logic = 0x4000,
		Conveyor = 0x8000
	}

	public string tooltipText;

	public string notificationText;

	public string notificationTooltipText;

	public float notificationDelay;

	public string soundPath;

	public string iconName;

	public TintedSprite sprite;

	public bool shouldNotify;

	public IconType iconType;

	public NotificationType notificationType;

	public Notification.ClickCallback notificationClickCallback;

	public Func<string, object, string> resolveStringCallback;

	public Func<string, object, string> resolveTooltipCallback;

	public bool allowMultiples;

	public Func<HashedString, object, bool> conditionalOverlayCallback;

	public HashedString render_overlay;

	public int status_overlays;

	private string composedPrefix;

	private bool showShowWorldIcon = true;

	public const int ALL_OVERLAYS = 63486;

	private static Dictionary<HashedString, StatusItemOverlays> overlayBitfieldMap = new Dictionary<HashedString, StatusItemOverlays>
	{
		{
			OverlayModes.None.ID,
			StatusItemOverlays.None
		},
		{
			OverlayModes.Power.ID,
			StatusItemOverlays.PowerMap
		},
		{
			OverlayModes.Temperature.ID,
			StatusItemOverlays.Temperature
		},
		{
			OverlayModes.ThermalConductivity.ID,
			StatusItemOverlays.ThermalComfort
		},
		{
			OverlayModes.Light.ID,
			StatusItemOverlays.Light
		},
		{
			OverlayModes.LiquidConduits.ID,
			StatusItemOverlays.LiquidPlumbing
		},
		{
			OverlayModes.GasConduits.ID,
			StatusItemOverlays.GasPlumbing
		},
		{
			OverlayModes.SolidConveyor.ID,
			StatusItemOverlays.Conveyor
		},
		{
			OverlayModes.Decor.ID,
			StatusItemOverlays.Decor
		},
		{
			OverlayModes.Disease.ID,
			StatusItemOverlays.Pathogens
		},
		{
			OverlayModes.Crop.ID,
			StatusItemOverlays.Farming
		},
		{
			OverlayModes.Rooms.ID,
			StatusItemOverlays.Rooms
		},
		{
			OverlayModes.Suit.ID,
			StatusItemOverlays.Suits
		},
		{
			OverlayModes.Logic.ID,
			StatusItemOverlays.Logic
		},
		{
			OverlayModes.Oxygen.ID,
			StatusItemOverlays.None
		},
		{
			OverlayModes.TileMode.ID,
			StatusItemOverlays.None
		}
	};

	private StatusItem(string id, string composed_prefix)
		: base(id, Strings.Get(composed_prefix + ".NAME"))
	{
		composedPrefix = composed_prefix;
		tooltipText = Strings.Get(composed_prefix + ".TOOLTIP");
	}

	public StatusItem(string id, string prefix, string icon, IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 63486)
		: this(id, "STRINGS." + prefix + ".STATUSITEMS." + id.ToUpper())
	{
		switch (icon_type)
		{
		case IconType.Info:
			icon = "dash";
			break;
		case IconType.Exclamation:
			icon = "status_item_exclamation";
			break;
		}
		iconName = icon;
		notificationType = notification_type;
		sprite = Assets.GetTintedSprite(icon);
		iconType = icon_type;
		allowMultiples = allow_multiples;
		this.render_overlay = render_overlay;
		showShowWorldIcon = showWorldIcon;
		this.status_overlays = status_overlays;
		if (sprite == null)
		{
			Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon);
		}
	}

	public StatusItem(string id, string name, string tooltip, string icon, IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 63486)
		: base(id, name)
	{
		switch (icon_type)
		{
		case IconType.Info:
			icon = "dash";
			break;
		case IconType.Exclamation:
			icon = "status_item_exclamation";
			break;
		}
		iconName = icon;
		notificationType = notification_type;
		sprite = Assets.GetTintedSprite(icon);
		tooltipText = tooltip;
		iconType = icon_type;
		allowMultiples = allow_multiples;
		this.render_overlay = render_overlay;
		this.status_overlays = status_overlays;
		if (sprite == null)
		{
			Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon);
		}
	}

	public void AddNotification(string sound_path = null, string notification_text = null, string notification_tooltip = null, float notification_delay = 0f)
	{
		shouldNotify = true;
		notificationDelay = notification_delay;
		if (sound_path == null)
		{
			NotificationType notificationType = this.notificationType;
			if (notificationType == NotificationType.Bad)
			{
				soundPath = "Warning";
			}
			else
			{
				soundPath = "Notification";
			}
		}
		else
		{
			soundPath = sound_path;
		}
		if (notification_text != null)
		{
			notificationText = notification_text;
		}
		else
		{
			DebugUtil.Assert(composedPrefix != null, "When adding a notification, either set the status prefix or specify strings!");
			notificationText = Strings.Get(composedPrefix + ".NOTIFICATION_NAME");
		}
		if (notification_tooltip != null)
		{
			notificationTooltipText = notification_tooltip;
		}
		else
		{
			DebugUtil.Assert(composedPrefix != null, "When adding a notification, either set the status prefix or specify strings!");
			notificationTooltipText = Strings.Get(composedPrefix + ".NOTIFICATION_TOOLTIP");
		}
	}

	public virtual string GetName(object data)
	{
		return ResolveString(Name, data);
	}

	public virtual string GetTooltip(object data)
	{
		return ResolveTooltip(tooltipText, data);
	}

	private string ResolveString(string str, object data)
	{
		if (resolveStringCallback != null && data != null)
		{
			return resolveStringCallback(str, data);
		}
		return str;
	}

	private string ResolveTooltip(string str, object data)
	{
		if (data != null)
		{
			if (resolveTooltipCallback != null)
			{
				return resolveTooltipCallback(str, data);
			}
			if (resolveStringCallback != null)
			{
				return resolveStringCallback(str, data);
			}
		}
		return str;
	}

	public bool ShouldShowIcon()
	{
		return iconType == IconType.Custom && showShowWorldIcon;
	}

	public virtual void ShowToolTip(ToolTip tooltip_widget, object data, TextStyleSetting property_style)
	{
		tooltip_widget.ClearMultiStringTooltip();
		string tooltip = GetTooltip(data);
		tooltip_widget.AddMultiStringTooltip(tooltip, property_style);
	}

	public void SetIcon(Image image, object data)
	{
		if (sprite != null)
		{
			image.color = sprite.color;
			image.sprite = sprite.sprite;
		}
	}

	public bool UseConditionalCallback(HashedString overlay, Transform transform)
	{
		return overlay != OverlayModes.None.ID && conditionalOverlayCallback != null && conditionalOverlayCallback(overlay, transform);
	}

	public StatusItem SetResolveStringCallback(Func<string, object, string> cb)
	{
		resolveStringCallback = cb;
		return this;
	}

	public static StatusItemOverlays GetStatusItemOverlayBySimViewMode(HashedString mode)
	{
		if (overlayBitfieldMap.TryGetValue(mode, out StatusItemOverlays value))
		{
			return value;
		}
		Debug.LogWarning("ViewMode " + mode + " has no StatusItemOverlay value");
		value = StatusItemOverlays.None;
		return value;
	}
}
