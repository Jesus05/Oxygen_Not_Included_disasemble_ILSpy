using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SicknessTrigger : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public delegate string SourceCallback(GameObject source, GameObject target);

	[Serializable]
	public struct TriggerInfo
	{
		[HashedEnum]
		public GameHashes srcEvent;

		public string[] sickness_ids;

		public SourceCallback sourceCallback;
	}

	public List<TriggerInfo> triggers = new List<TriggerInfo>();

	public void AddTrigger(GameHashes src_event, string[] sickness_ids, SourceCallback source_callback)
	{
		triggers.Add(new TriggerInfo
		{
			srcEvent = src_event,
			sickness_ids = sickness_ids,
			sourceCallback = source_callback
		});
	}

	protected override void OnSpawn()
	{
		for (int i = 0; i < triggers.Count; i++)
		{
			TriggerInfo trigger = triggers[i];
			Subscribe((int)trigger.srcEvent, delegate(object data)
			{
				OnSicknessTrigger((GameObject)data, trigger);
			});
		}
	}

	private void OnSicknessTrigger(GameObject target, TriggerInfo trigger)
	{
		int num = UnityEngine.Random.Range(0, trigger.sickness_ids.Length);
		string text = trigger.sickness_ids[num];
		Sickness sickness = null;
		Database.Sicknesses sicknesses = Db.Get().Sicknesses;
		for (int i = 0; i < sicknesses.Count; i++)
		{
			if (sicknesses[i].Id == text)
			{
				sickness = sicknesses[i];
				break;
			}
		}
		if (sickness != null)
		{
			string infection_source_info = trigger.sourceCallback(base.gameObject, target);
			SicknessExposureInfo exposure_info = new SicknessExposureInfo(sickness.Id, infection_source_info);
			Klei.AI.Sicknesses sicknesses2 = target.GetComponent<MinionModifiers>().sicknesses;
			sicknesses2.Infect(exposure_info);
		}
		else
		{
			DebugUtil.DevLogErrorFormat(base.gameObject, "Couldn't find sickness with id [{0}]", text);
		}
	}

	public List<Descriptor> EffectDescriptors(GameObject go)
	{
		Dictionary<GameHashes, HashSet<string>> dictionary = new Dictionary<GameHashes, HashSet<string>>();
		foreach (TriggerInfo trigger in triggers)
		{
			TriggerInfo current = trigger;
			HashSet<string> value = null;
			if (!dictionary.TryGetValue(current.srcEvent, out value))
			{
				value = new HashSet<string>();
				dictionary[current.srcEvent] = value;
			}
			string[] sickness_ids = current.sickness_ids;
			foreach (string item in sickness_ids)
			{
				value.Add(item);
			}
		}
		List<Descriptor> list = new List<Descriptor>();
		List<string> list2 = new List<string>();
		string properName = GetComponent<KSelectable>().GetProperName();
		foreach (KeyValuePair<GameHashes, HashSet<string>> item2 in dictionary)
		{
			HashSet<string> value2 = item2.Value;
			list2.Clear();
			foreach (string item3 in value2)
			{
				Sickness sickness = Db.Get().Sicknesses.TryGet(item3);
				list2.Add(sickness.Name);
			}
			string newValue = string.Join(", ", list2.ToArray());
			string @string = Strings.Get("STRINGS.DUPLICANTS.DISEASES.TRIGGERS." + Enum.GetName(typeof(GameHashes), item2.Key).ToUpper()).String;
			string string2 = Strings.Get("STRINGS.DUPLICANTS.DISEASES.TRIGGERS.TOOLTIPS." + Enum.GetName(typeof(GameHashes), item2.Key).ToUpper()).String;
			@string = @string.Replace("{ItemName}", properName).Replace("{Diseases}", newValue);
			string2 = string2.Replace("{ItemName}", properName).Replace("{Diseases}", newValue);
			list.Add(new Descriptor(@string, string2, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return EffectDescriptors(go);
	}
}
