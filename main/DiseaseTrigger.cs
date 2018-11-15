using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseTrigger : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public delegate string SourceCallback(GameObject source, GameObject target);

	[Serializable]
	public struct TriggerInfo
	{
		[HashedEnum]
		public GameHashes srcEvent;

		public string[] diseaseIDs;

		public SourceCallback sourceCallback;
	}

	public List<TriggerInfo> triggers = new List<TriggerInfo>();

	public void AddTrigger(GameHashes src_event, string[] disease_ids, SourceCallback source_callback)
	{
		triggers.Add(new TriggerInfo
		{
			srcEvent = src_event,
			diseaseIDs = disease_ids,
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
				GameObject gameObject = (GameObject)data;
				Database.Diseases diseases = Db.Get().Diseases;
				int num = UnityEngine.Random.Range(0, trigger.diseaseIDs.Length);
				Disease disease = null;
				for (int j = 0; j < diseases.Count; j++)
				{
					if (diseases[j].Id == trigger.diseaseIDs[num])
					{
						disease = diseases[j];
						break;
					}
				}
				if (disease != null)
				{
					string infection_source_info = trigger.sourceCallback(base.gameObject, gameObject.gameObject);
					DiseaseExposureInfo exposure_info = new DiseaseExposureInfo(disease.Id, infection_source_info);
					bool flag = true;
					Edible component = base.gameObject.GetComponent<Edible>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						Traits component2 = gameObject.GetComponent<Traits>();
						if (component2.HasTrait("IronGut"))
						{
							flag = false;
						}
					}
					if (flag)
					{
						Klei.AI.Diseases diseases2 = gameObject.GetComponent<MinionModifiers>().diseases;
						diseases2.Infect(exposure_info);
					}
				}
				else
				{
					Output.LogErrorWithObj(base.gameObject, "couldn't find disease with id [" + trigger.diseaseIDs[num] + "]");
				}
			});
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
			string[] diseaseIDs = current.diseaseIDs;
			foreach (string item in diseaseIDs)
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
				Disease disease = Db.Get().Diseases.Get(item3);
				list2.Add(disease.Name);
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
