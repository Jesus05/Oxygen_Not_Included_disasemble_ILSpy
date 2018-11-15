using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

internal class UpdateObjectCountParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public EventInstance ev;

		public Settings settings;
	}

	public struct Settings
	{
		public HashedString path;

		public int parameterIdx;

		public float minObjects;

		public float maxObjects;

		public bool useExponentialCurve;
	}

	private List<Entry> entries = new List<Entry>();

	private static Dictionary<HashedString, Settings> settings = new Dictionary<HashedString, Settings>();

	private static readonly HashedString parameterHash = "objectCount";

	public UpdateObjectCountParameter()
		: base("objectCount")
	{
	}

	public static Settings GetSettings(HashedString path_hash, SoundDescription description)
	{
		Settings value = default(Settings);
		if (!settings.TryGetValue(path_hash, out value))
		{
			value = default(Settings);
			EventDescription eventDescription = RuntimeManager.GetEventDescription(description.path);
			if (eventDescription.getUserProperty("minObj", out USER_PROPERTY property) == RESULT.OK)
			{
				value.minObjects = (float)(short)property.floatValue();
			}
			else
			{
				value.minObjects = 1f;
			}
			if (eventDescription.getUserProperty("maxObj", out USER_PROPERTY property2) == RESULT.OK)
			{
				value.maxObjects = property2.floatValue();
			}
			else
			{
				value.maxObjects = 0f;
			}
			if (eventDescription.getUserProperty("curveType", out USER_PROPERTY property3) == RESULT.OK && property3.stringValue() == "exp")
			{
				value.useExponentialCurve = true;
			}
			value.parameterIdx = description.GetParameterIdx(parameterHash);
			value.path = path_hash;
			settings[path_hash] = value;
		}
		return value;
	}

	public static void ApplySettings(EventInstance ev, int count, Settings settings)
	{
		float num = 0f;
		if (settings.maxObjects != settings.minObjects)
		{
			num = ((float)count - settings.minObjects) / (settings.maxObjects - settings.minObjects);
			num = Mathf.Clamp01(num);
		}
		if (settings.useExponentialCurve)
		{
			num *= num;
		}
		ev.setParameterValueByIndex(settings.parameterIdx, num);
	}

	public override void Add(Sound sound)
	{
		Settings settings = GetSettings(sound.path, sound.description);
		Entry entry = default(Entry);
		entry.ev = sound.ev;
		entry.settings = settings;
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		DictionaryPool<HashedString, int, LoopingSoundManager>.PooledDictionary pooledDictionary = DictionaryPool<HashedString, int, LoopingSoundManager>.Allocate();
		foreach (Entry entry in entries)
		{
			Entry current = entry;
			int value = 0;
			pooledDictionary.TryGetValue(current.settings.path, out value);
			value = (pooledDictionary[current.settings.path] = value + 1);
		}
		foreach (Entry entry2 in entries)
		{
			Entry current2 = entry2;
			int count = pooledDictionary[current2.settings.path];
			ApplySettings(current2.ev, count, current2.settings);
		}
		pooledDictionary.Recycle();
	}

	public override void Remove(Sound sound)
	{
		int num = 0;
		while (true)
		{
			if (num >= entries.Count)
			{
				return;
			}
			Entry entry = entries[num];
			if (entry.ev.handle == sound.ev.handle)
			{
				break;
			}
			num++;
		}
		entries.RemoveAt(num);
	}

	public static void Clear()
	{
		settings.Clear();
	}
}
