using System;

[Serializable]
public class ToggleSoundPlayer : WidgetSoundPlayer
{
	public enum SoundEvents
	{
		OnClick_On,
		OnClick_Off,
		OnPointerEnter,
		OnClick_Rejected
	}

	public static readonly string[] default_values = new string[4]
	{
		"HUD_Click",
		"HUD_Click_Deselect",
		"HUD_Mouseover",
		"Negative"
	};

	public Func<bool> AcceptClickCondition;

	public WidgetSoundEvent[] toggle_widget_sound_events = new WidgetSoundEvent[4]
	{
		new WidgetSoundEvent(0, "On Use On", string.Empty, true),
		new WidgetSoundEvent(1, "On Use Off", string.Empty, true),
		new WidgetSoundEvent(2, "On Pointer Enter", string.Empty, true),
		new WidgetSoundEvent(3, "On Use Rejected", string.Empty, true)
	};

	public override string GetDefaultPath(int idx)
	{
		return default_values[idx];
	}

	public override WidgetSoundEvent[] widget_sound_events()
	{
		return toggle_widget_sound_events;
	}
}
