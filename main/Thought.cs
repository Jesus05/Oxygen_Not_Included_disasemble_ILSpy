using UnityEngine;

public class Thought : Resource
{
	public int priority;

	public Sprite sprite;

	public Sprite modeSprite;

	public string sound;

	public Sprite bubbleSprite;

	public string speechPrefix;

	public LocString hoverText;

	public bool showImmediately;

	public float showTime;

	public Thought(string id, ResourceSet parent, Sprite icon, string mode_icon, string sound_name, string bubble, string speech_prefix, LocString hover_text, bool show_immediately = false, float show_time = 4f)
		: base(id, parent, null)
	{
		sprite = icon;
		if (mode_icon != null)
		{
			modeSprite = Assets.GetSprite(mode_icon);
		}
		bubbleSprite = Assets.GetSprite(bubble);
		sound = sound_name;
		speechPrefix = speech_prefix;
		hoverText = hover_text;
		showImmediately = show_immediately;
		showTime = show_time;
	}

	public Thought(string id, ResourceSet parent, string icon, string mode_icon, string sound_name, string bubble, string speech_prefix, LocString hover_text, bool show_immediately = false, float show_time = 4f)
		: base(id, parent, null)
	{
		sprite = Assets.GetSprite(icon);
		if (mode_icon != null)
		{
			modeSprite = Assets.GetSprite(mode_icon);
		}
		bubbleSprite = Assets.GetSprite(bubble);
		sound = sound_name;
		speechPrefix = speech_prefix;
		hoverText = hover_text;
		showImmediately = show_immediately;
		showTime = show_time;
	}
}
