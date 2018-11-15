public class SpeedOneShotUpdater : OneShotSoundParameterUpdater
{
	public SpeedOneShotUpdater()
		: base("Speed")
	{
	}

	public override void Play(Sound sound)
	{
		sound.ev.setParameterValueByIndex(sound.description.GetParameterIdx(base.parameter), SpeedLoopingSoundUpdater.GetSpeedParameterValue());
	}
}
