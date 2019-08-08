internal abstract class UserVolumeOneShotUpdater : OneShotSoundParameterUpdater
{
	private string playerPref;

	public UserVolumeOneShotUpdater(string parameter, string player_pref)
		: base(parameter)
	{
		playerPref = player_pref;
	}

	public override void Play(Sound sound)
	{
		if (!string.IsNullOrEmpty(playerPref))
		{
			float @float = KPlayerPrefs.GetFloat(playerPref);
			sound.ev.setParameterValueByIndex(sound.description.GetParameterIdx(base.parameter), @float);
		}
	}
}
