using System;

[Serializable]
public class WidgetSoundPlayer
{
	[Serializable]
	public struct WidgetSoundEvent
	{
		public string Name;

		public string OverrideAssetName;

		public int idx;

		public bool PlaySound;

		public WidgetSoundEvent(int idx, string Name, string OverrideAssetName, bool PlaySound)
		{
			this.idx = idx;
			this.Name = Name;
			this.OverrideAssetName = OverrideAssetName;
			this.PlaySound = PlaySound;
		}
	}

	public bool Enabled = true;

	public static Func<string, string> getSoundPath;

	public virtual string GetDefaultPath(int idx)
	{
		return "";
	}

	public virtual WidgetSoundEvent[] widget_sound_events()
	{
		return null;
	}

	public void Play(int sound_event_idx)
	{
		if (Enabled)
		{
			WidgetSoundEvent widgetSoundEvent = default(WidgetSoundEvent);
			for (int i = 0; i < widget_sound_events().Length; i++)
			{
				if (sound_event_idx == widget_sound_events()[i].idx)
				{
					widgetSoundEvent = widget_sound_events()[sound_event_idx];
					break;
				}
			}
			if (KInputManager.isFocused && widgetSoundEvent.PlaySound && widgetSoundEvent.Name != null && widgetSoundEvent.Name.Length >= 0 && !(widgetSoundEvent.Name == ""))
			{
				KFMOD.PlayOneShot(getSoundPath((!(widgetSoundEvent.OverrideAssetName == "")) ? widgetSoundEvent.OverrideAssetName : GetDefaultPath(widgetSoundEvent.idx)));
			}
		}
	}
}
