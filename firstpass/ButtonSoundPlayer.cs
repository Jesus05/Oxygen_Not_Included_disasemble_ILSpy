using System;

[Serializable]
public class ButtonSoundPlayer : WidgetSoundPlayer
{
	public enum SoundEvents
	{
		OnClick,
		OnPointerEnter,
		OnClick_Rejected
	}

	public static string[] default_values = new string[3]
	{
		"HUD_Click_Open",
		"HUD_Mouseover",
		"Negative"
	};

	public Func<bool> AcceptClickCondition;

	public WidgetSoundEvent[] button_widget_sound_events = new WidgetSoundEvent[3]
	{
		new WidgetSoundEvent(0, "On Use", "", true),
		new WidgetSoundEvent(1, "On Pointer Enter", "", true),
		new WidgetSoundEvent(2, "On Use Rejected", "", true)
	};

	public override string GetDefaultPath(int idx)
	{
		return default_values[idx];
	}

	public override WidgetSoundEvent[] widget_sound_events()
	{
		return button_widget_sound_events;
	}
}
