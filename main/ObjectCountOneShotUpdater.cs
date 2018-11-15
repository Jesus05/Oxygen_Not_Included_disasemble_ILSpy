using System.Collections.Generic;

internal class ObjectCountOneShotUpdater : OneShotSoundParameterUpdater
{
	private Dictionary<HashedString, int> soundCounts = new Dictionary<HashedString, int>();

	public ObjectCountOneShotUpdater()
		: base("objectCount")
	{
	}

	public override void Update(float dt)
	{
		soundCounts.Clear();
	}

	public override void Play(Sound sound)
	{
		UpdateObjectCountParameter.Settings settings = UpdateObjectCountParameter.GetSettings(sound.path, sound.description);
		int value = 0;
		soundCounts.TryGetValue(sound.path, out value);
		value = (soundCounts[sound.path] = value + 1);
		UpdateObjectCountParameter.ApplySettings(sound.ev, value, settings);
	}
}
