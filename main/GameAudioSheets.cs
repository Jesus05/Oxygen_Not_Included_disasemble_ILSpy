using System.Collections.Generic;
using UnityEngine;

public class GameAudioSheets : AudioSheets
{
	private class SingleAudioSheetLoader : AsyncLoader
	{
		public AudioSheet sheet;

		public string text;

		public string name;

		public override void Run()
		{
			sheet.soundInfos = new ResourceLoader<AudioSheet.SoundInfo>(text, name).resources.ToArray();
		}
	}

	private class GameAudioSheetLoader : GlobalAsyncLoader<GameAudioSheetLoader>
	{
		public override void CollectLoaders(List<AsyncLoader> loaders)
		{
			foreach (AudioSheet sheet in GameAudioSheets.Get().sheets)
			{
				loaders.Add(new SingleAudioSheetLoader
				{
					sheet = sheet,
					text = sheet.asset.text,
					name = sheet.asset.name
				});
			}
		}

		public override void Run()
		{
		}
	}

	private static GameAudioSheets _Instance;

	private HashSet<HashedString> validFileNames = new HashSet<HashedString>();

	private Dictionary<HashedString, HashSet<HashedString>> animsNotAllowedToPlaySpeech = new Dictionary<HashedString, HashSet<HashedString>>();

	public static GameAudioSheets Get()
	{
		if ((Object)_Instance == (Object)null)
		{
			_Instance = Resources.Load<GameAudioSheets>("GameAudioSheets");
		}
		return _Instance;
	}

	public override void Initialize()
	{
		validFileNames.Add("game_triggered");
		foreach (KAnimFile animAsset in Assets.instance.AnimAssets)
		{
			if (!((Object)animAsset == (Object)null))
			{
				validFileNames.Add(animAsset.name);
			}
		}
		base.Initialize();
		foreach (AudioSheet sheet in sheets)
		{
			AudioSheet.SoundInfo[] soundInfos = sheet.soundInfos;
			foreach (AudioSheet.SoundInfo soundInfo in soundInfos)
			{
				if (soundInfo.Type == "MouthFlapSoundEvent" || soundInfo.Type == "VoiceSoundEvent")
				{
					HashSet<HashedString> value = null;
					if (!animsNotAllowedToPlaySpeech.TryGetValue(soundInfo.File, out value))
					{
						value = new HashSet<HashedString>();
						animsNotAllowedToPlaySpeech[soundInfo.File] = value;
					}
					value.Add(soundInfo.Anim);
				}
			}
		}
	}

	protected override AnimEvent CreateSoundOfType(string type, string file_name, string sound_name, int frame, float min_interval)
	{
		SoundEvent soundEvent = null;
		bool shouldCameraScalePosition = true;
		if (sound_name.Contains(":disable_camera_position_scaling"))
		{
			sound_name = sound_name.Replace(":disable_camera_position_scaling", string.Empty);
			shouldCameraScalePosition = false;
		}
		if (type == "FloorSoundEvent")
		{
			soundEvent = new FloorSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "SoundEvent" || type == "LoopingSoundEvent")
		{
			bool flag = type == "LoopingSoundEvent";
			string[] array = sound_name.Split(':');
			sound_name = array[0];
			string sound_name2 = sound_name;
			bool is_looping = flag;
			soundEvent = new SoundEvent(file_name, sound_name2, frame, true, is_looping, min_interval, false);
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] == "IGNORE_PAUSE")
				{
					soundEvent.ignorePause = true;
				}
				else
				{
					Debug.LogWarning(sound_name + " has unknown parameter " + array[i], null);
				}
			}
		}
		else if (type == "LadderSoundEvent")
		{
			soundEvent = new LadderSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "LaserSoundEvent")
		{
			soundEvent = new LaserSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "HatchDrillSoundEvent")
		{
			soundEvent = new HatchDrillSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "CreatureChewSoundEvent")
		{
			soundEvent = new CreatureChewSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "BuildingDamageSoundEvent")
		{
			soundEvent = new BuildingDamageSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "WallDamageSoundEvent")
		{
			soundEvent = new WallDamageSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "RemoteSoundEvent")
		{
			soundEvent = new RemoteSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "VoiceSoundEvent" || type == "LoopingVoiceSoundEvent")
		{
			soundEvent = new VoiceSoundEvent(file_name, sound_name, frame, type == "LoopingVoiceSoundEvent");
		}
		else if (type == "MouthFlapSoundEvent")
		{
			soundEvent = new MouthFlapSoundEvent(file_name, sound_name, frame, false);
		}
		else if (type == "MainMenuSoundEvent")
		{
			soundEvent = new MainMenuSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "CreatureVariationSoundEvent")
		{
			string sound_name3 = sound_name;
			bool is_looping = type == "LoopingSoundEvent";
			soundEvent = new CreatureVariationSoundEvent(file_name, sound_name3, frame, true, is_looping, min_interval, false);
		}
		else if (type == "CountedSoundEvent")
		{
			soundEvent = new CountedSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
		}
		else if (type == "SculptingSoundEvent")
		{
			soundEvent = new SculptingSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
		}
		else if (type == "PhonoboxSoundEvent")
		{
			soundEvent = new PhonoboxSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (soundEvent != null)
		{
			soundEvent.shouldCameraScalePosition = shouldCameraScalePosition;
		}
		return soundEvent;
	}

	public bool IsAnimAllowedToPlaySpeech(KAnim.Anim anim)
	{
		HashSet<HashedString> value = null;
		if (animsNotAllowedToPlaySpeech.TryGetValue(anim.animFile.name, out value))
		{
			return !value.Contains(anim.hash);
		}
		return true;
	}
}
