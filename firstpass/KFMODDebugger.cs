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

	public bool debugEnabled = false;

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
		if (!s.Contains("Buildings"))
		{
			if (!s.Contains("Notifications"))
			{
				if (!s.Contains("UI"))
				{
					if (!s.Contains("Creatures"))
					{
						if (!s.Contains("Duplicant_voices"))
						{
							if (!s.Contains("Ambience"))
							{
								if (!s.Contains("Environment"))
								{
									if (!s.Contains("FX"))
									{
										if (!s.Contains("Duplicant_actions/LowImportance/Movement"))
										{
											if (!s.Contains("Duplicant_actions"))
											{
												if (!s.Contains("Plants"))
												{
													if (!s.Contains("Music"))
													{
														return DebugSoundType.Uncategorized;
													}
													return DebugSoundType.Music;
												}
												return DebugSoundType.Plants;
											}
											return DebugSoundType.DupeActions;
										}
										return DebugSoundType.DupeMovement;
									}
									return DebugSoundType.FX;
								}
								return DebugSoundType.Environment;
							}
							return DebugSoundType.Ambience;
						}
						return DebugSoundType.DupeVoices;
					}
					return DebugSoundType.Creatures;
				}
				return DebugSoundType.UI;
			}
			return DebugSoundType.Notifications;
		}
		return DebugSoundType.Buildings;
	}
}
