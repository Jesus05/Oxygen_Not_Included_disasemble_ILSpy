internal abstract class UserVolumeOneShotUpdater : OneShotSoundParameterUpdater
{
	public UserVolumeOneShotUpdater(string parameter, string player_pref)
		: base(parameter)
	{
	}

	public override void Play(Sound sound)
	{
		sound.ev.setParameterValueByIndex(sound.description.GetParameterIdx(base.parameter), SpeedLoopingSoundUpdater.GetSpeedParameterValue());
	}
}
