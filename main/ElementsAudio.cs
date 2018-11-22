public class ElementsAudio
{
	public class ElementAudioConfig : Resource
	{
		public SimHashes elementID;

		public AmbienceType ambienceType = AmbienceType.None;

		public SolidAmbienceType solidAmbienceType = SolidAmbienceType.None;

		public string miningSound = "";

		public string miningBreakSound = "";

		public string oreBumpSound = "";

		public string floorEventAudioCategory = "";

		public string creatureChewSound = "";
	}

	private static ElementsAudio _instance = null;

	private ElementAudioConfig[] elementAudioConfigs = null;

	public static ElementsAudio Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ElementsAudio();
			}
			return _instance;
		}
	}

	public void LoadData(ElementAudioConfig[] elements_audio_configs)
	{
		elementAudioConfigs = elements_audio_configs;
	}

	public ElementAudioConfig GetConfigForElement(SimHashes id)
	{
		if (elementAudioConfigs != null)
		{
			for (int i = 0; i < elementAudioConfigs.Length; i++)
			{
				if (elementAudioConfigs[i].elementID == id)
				{
					return elementAudioConfigs[i];
				}
			}
		}
		return null;
	}
}
