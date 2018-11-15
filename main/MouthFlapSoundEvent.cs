public class MouthFlapSoundEvent : SoundEvent
{
	public MouthFlapSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
		: base(file_name, sound_name, frame, false, is_looping, (float)SoundEvent.IGNORE_INTERVAL, true)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		behaviour.controller.GetSMI<SpeechMonitor.Instance>().PlaySpeech(base.name, null);
	}
}
