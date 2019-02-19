using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class KFMODDebugger : KMonoBehaviour
{
	public struct AudioDebugEntry
	{
		public string log;

		public DebugSoundType soundType;

		public float callTime;
	}

	public enum DebugSoundType
	{
		Uncategorized = -1,
		UI,
		Notifications,
		Buildings,
		DupeVoices,
		DupeMovement,
		DupeActions,
		Creatures,
		Plants,
		Ambience,
		Environment,
		FX,
		Music
	}

	public static KFMODDebugger instance;

	public List<AudioDebugEntry> AudioDebugLog = new List<AudioDebugEntry>();

	public Dictionary<DebugSoundType, bool> allDebugSoundTypes = new Dictionary<DebugSoundType, bool>();

	public bool debugEnabled;

	public static KFMODDebugger Get()
	{
		return instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		IEnumerator enumerator = Enum.GetValues(typeof(DebugSoundType)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DebugSoundType key = (DebugSoundType)enumerator.Current;
				allDebugSoundTypes.Add(key, false);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	protected override void OnCleanUp()
	{
		instance = null;
	}

	[Conditional("ENABLE_KFMOD_LOGGER")]
	public void Log(string s)
	{
	}

	private DebugSoundType GetDebugSoundType(string s)
	{
		if (s.Contains("Buildings"))
		{
			return DebugSoundType.Buildings;
		}
		if (s.Contains("Notifications"))
		{
			return DebugSoundType.Notifications;
		}
		if (s.Contains("UI"))
		{
			return DebugSoundType.UI;
		}
		if (s.Contains("Creatures"))
		{
			return DebugSoundType.Creatures;
		}
		if (s.Contains("Duplicant_voices"))
		{
			return DebugSoundType.DupeVoices;
		}
		if (s.Contains("Ambience"))
		{
			return DebugSoundType.Ambience;
		}
		if (s.Contains("Environment"))
		{
			return DebugSoundType.Environment;
		}
		if (s.Contains("FX"))
		{
			return DebugSoundType.FX;
		}
		if (s.Contains("Duplicant_actions/LowImportance/Movement"))
		{
			return DebugSoundType.DupeMovement;
		}
		if (s.Contains("Duplicant_actions"))
		{
			return DebugSoundType.DupeActions;
		}
		if (s.Contains("Plants"))
		{
			return DebugSoundType.Plants;
		}
		if (s.Contains("Music"))
		{
			return DebugSoundType.Music;
		}
		return DebugSoundType.Uncategorized;
	}
}
