using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionStorage : KMonoBehaviour
{
	public struct Info
	{
		public Guid id;

		public string name;

		public Ref<KPrefabID> serializedMinion;

		public Info(string name, Ref<KPrefabID> ref_obj)
		{
			id = Guid.NewGuid();
			this.name = name;
			serializedMinion = ref_obj;
		}

		public static Info CreateEmpty()
		{
			Info result = default(Info);
			result.id = Guid.Empty;
			result.name = null;
			result.serializedMinion = null;
			return result;
		}
	}

	[Serialize]
	private List<Info> serializedMinions = new List<Info>();

	private KPrefabID CreateSerializedMinion(GameObject src_minion)
	{
		GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(StoredMinionConfig.ID);
		GameObject gameObject = Util.KInstantiate(prefab, Vector3.zero);
		gameObject.SetActive(true);
		CopyMinion(src_minion.GetComponent<MinionIdentity>(), gameObject.GetComponent<StoredMinionIdentity>());
		RedirectInstanceTracker(src_minion, gameObject);
		Util.KDestroyGameObject(src_minion);
		return gameObject.GetComponent<KPrefabID>();
	}

	private void CopyMinion(MinionIdentity src_id, StoredMinionIdentity dest_id)
	{
		dest_id.storedName = src_id.name;
		dest_id.nameStringKey = src_id.nameStringKey;
		dest_id.gender = src_id.gender;
		dest_id.genderStringKey = src_id.genderStringKey;
		dest_id.arrivalTime = src_id.arrivalTime;
		dest_id.voiceIdx = src_id.voiceIdx;
		dest_id.bodyData = src_id.bodyData;
		Traits component = src_id.GetComponent<Traits>();
		dest_id.traitIDs = new List<string>(component.GetTraitIds());
		Ownables component2 = src_id.GetComponent<Ownables>();
		List<Ref<KPrefabID>> list = new List<Ref<KPrefabID>>();
		foreach (AssignableSlotInstance item in component2)
		{
			if (item != null && !((UnityEngine.Object)item.assignable == (UnityEngine.Object)null))
			{
				Assignable assignable = item.assignable;
				KPrefabID component3 = assignable.GetComponent<KPrefabID>();
				list.Add(new Ref<KPrefabID>(component3));
				assignable.Unassign();
			}
		}
		component2.UnassignAll();
		foreach (Ref<KPrefabID> item2 in list)
		{
			Assignable component4 = item2.Get().GetComponent<Assignable>();
			component4.Assign(dest_id);
		}
		dest_id.assignedItems = list;
		Equipment component5 = src_id.GetComponent<Equipment>();
		List<Ref<KPrefabID>> list2 = new List<Ref<KPrefabID>>();
		foreach (AssignableSlotInstance item3 in component5)
		{
			if (item3 != null)
			{
				Assignable assignable2 = item3.assignable;
				if (!((UnityEngine.Object)assignable2 == (UnityEngine.Object)null))
				{
					KPrefabID component6 = assignable2.GetComponent<KPrefabID>();
					list2.Add(new Ref<KPrefabID>(component6));
					assignable2.Unassign();
				}
			}
		}
		component5.UnequipAll();
		Equipment component7 = dest_id.GetComponent<Equipment>();
		foreach (Ref<KPrefabID> item4 in list2)
		{
			Equippable component8 = item4.Get().GetComponent<Equippable>();
			component8.Assign(dest_id);
			component7.Equip(component8);
		}
		dest_id.equippedItems = list2;
		Accessorizer component9 = src_id.GetComponent<Accessorizer>();
		dest_id.accessories = component9.GetAccessories();
		ConsumableConsumer component10 = src_id.GetComponent<ConsumableConsumer>();
		if (component10.forbiddenTags != null)
		{
			dest_id.forbiddenTags = new List<Tag>(component10.forbiddenTags);
		}
		MinionResume component11 = src_id.GetComponent<MinionResume>();
		dest_id.ExperienceByRoleID = component11.ExperienceByRoleID;
		dest_id.MasteryByRoleID = component11.MasteryByRoleID;
		dest_id.AptitudeByRoleGroup = component11.AptitudeByRoleGroup;
		dest_id.currentRole = component11.CurrentRole;
		dest_id.targetRole = component11.TargetRole;
		ChoreConsumer component12 = src_id.GetComponent<ChoreConsumer>();
		dest_id.choreGroupPriorities = component12.GetChoreGroupPriorities();
		AttributeLevels component13 = src_id.GetComponent<AttributeLevels>();
		component13.OnSerializing();
		dest_id.attributeLevels = new List<AttributeLevels.LevelSaveLoad>(component13.SaveLoadLevels);
		Schedulable component14 = src_id.GetComponent<Schedulable>();
		Schedule schedule = component14.GetSchedule();
		if (schedule != null)
		{
			schedule.Unassign(component14);
			Schedulable component15 = dest_id.GetComponent<Schedulable>();
			schedule.Assign(component15);
		}
	}

	private void CopyMinion(StoredMinionIdentity src_id, MinionIdentity dest_id)
	{
		dest_id.SetName(src_id.storedName);
		dest_id.nameStringKey = src_id.nameStringKey;
		dest_id.gender = src_id.gender;
		dest_id.genderStringKey = src_id.genderStringKey;
		dest_id.arrivalTime = src_id.arrivalTime;
		dest_id.voiceIdx = src_id.voiceIdx;
		dest_id.bodyData = src_id.bodyData;
		if (src_id.traitIDs != null)
		{
			Traits component = dest_id.GetComponent<Traits>();
			component.SetTraitIds(src_id.traitIDs);
		}
		if (src_id.assignedItems != null)
		{
			List<Ref<KPrefabID>> assignedItems = src_id.assignedItems;
			foreach (Ref<KPrefabID> item in assignedItems)
			{
				KPrefabID kPrefabID = item.Get();
				if (!((UnityEngine.Object)kPrefabID == (UnityEngine.Object)null))
				{
					Assignable component2 = kPrefabID.GetComponent<Assignable>();
					component2.Unassign();
				}
			}
			foreach (Ref<KPrefabID> item2 in assignedItems)
			{
				KPrefabID kPrefabID2 = item2.Get();
				if (!((UnityEngine.Object)kPrefabID2 == (UnityEngine.Object)null))
				{
					Assignable component3 = kPrefabID2.GetComponent<Assignable>();
					component3.Assign(dest_id);
				}
			}
			assignedItems.Clear();
		}
		if (src_id.accessories != null)
		{
			Accessorizer component4 = dest_id.GetComponent<Accessorizer>();
			component4.SetAccessories(src_id.accessories);
		}
		ConsumableConsumer component5 = dest_id.GetComponent<ConsumableConsumer>();
		if (src_id.forbiddenTags != null)
		{
			component5.forbiddenTags = src_id.forbiddenTags.ToArray();
		}
		if (src_id.ExperienceByRoleID != null)
		{
			MinionResume component6 = dest_id.GetComponent<MinionResume>();
			component6.ExperienceByRoleID = src_id.ExperienceByRoleID;
			component6.MasteryByRoleID = src_id.MasteryByRoleID;
			component6.AptitudeByRoleGroup = src_id.AptitudeByRoleGroup;
			component6.SetCurrentRole(src_id.currentRole);
			component6.SetTargetRole(src_id.targetRole);
		}
		if (src_id.choreGroupPriorities != null)
		{
			ChoreConsumer component7 = dest_id.GetComponent<ChoreConsumer>();
			component7.SetChoreGroupPriorities(src_id.choreGroupPriorities);
		}
		AttributeLevels component8 = dest_id.GetComponent<AttributeLevels>();
		if (src_id.attributeLevels != null)
		{
			component8.SaveLoadLevels = src_id.attributeLevels.ToArray();
			component8.OnDeserialized();
		}
		dest_id.GetComponent<Accessorizer>().ApplyAccessories();
		List<Ref<KPrefabID>> equippedItems = src_id.equippedItems;
		Equipment component9 = dest_id.GetComponent<Equipment>();
		if (equippedItems != null)
		{
			foreach (Ref<KPrefabID> item3 in equippedItems)
			{
				Equippable component10 = item3.Get().GetComponent<Equippable>();
				component10.Unassign();
			}
			foreach (Ref<KPrefabID> item4 in equippedItems)
			{
				Equippable component11 = item4.Get().GetComponent<Equippable>();
				component11.Assign(dest_id);
				component9.Equip(component11);
			}
			equippedItems.Clear();
		}
		Schedulable component12 = src_id.GetComponent<Schedulable>();
		Schedule schedule = component12.GetSchedule();
		if (schedule != null)
		{
			schedule.Unassign(component12);
			Schedulable component13 = dest_id.GetComponent<Schedulable>();
			schedule.Assign(component13);
		}
	}

	private void RedirectInstanceTracker(GameObject src_minion, GameObject dest_minion)
	{
		KPrefabID component = src_minion.GetComponent<KPrefabID>();
		KPrefabID component2 = dest_minion.GetComponent<KPrefabID>();
		component2.InstanceID = component.InstanceID;
		component.InstanceID = -1;
	}

	public void SerializeMinion(GameObject minion)
	{
		KPrefabID kPrefabID = CreateSerializedMinion(minion);
		Info item = new Info(kPrefabID.GetComponent<StoredMinionIdentity>().storedName, new Ref<KPrefabID>(kPrefabID));
		serializedMinions.Add(item);
	}

	private int GetMinionIndex(Guid id)
	{
		int result = -1;
		for (int i = 0; i < serializedMinions.Count; i++)
		{
			Info info = serializedMinions[i];
			if (info.id == id)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public GameObject DeserializeMinion(Guid id, Vector3 pos)
	{
		int minionIndex = GetMinionIndex(id);
		if (minionIndex < 0 || minionIndex >= serializedMinions.Count)
		{
			return null;
		}
		Info info = serializedMinions[minionIndex];
		KPrefabID kPrefabID = info.serializedMinion.Get();
		if ((UnityEngine.Object)kPrefabID == (UnityEngine.Object)null)
		{
			return null;
		}
		GameObject gameObject = kPrefabID.gameObject;
		GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(MinionConfig.ID);
		GameObject gameObject2 = Util.KInstantiate(prefab, pos);
		StoredMinionIdentity component = gameObject.GetComponent<StoredMinionIdentity>();
		MinionIdentity component2 = gameObject2.GetComponent<MinionIdentity>();
		RedirectInstanceTracker(gameObject, gameObject2);
		gameObject2.SetActive(true);
		CopyMinion(component, component2);
		Util.KDestroyGameObject(gameObject);
		serializedMinions.RemoveAt(minionIndex);
		return gameObject2;
	}

	public void DeleteStoredMinion(Guid id)
	{
		int minionIndex = GetMinionIndex(id);
		if (minionIndex >= 0)
		{
			Info info = serializedMinions[minionIndex];
			if (info.serializedMinion != null)
			{
				Info info2 = serializedMinions[minionIndex];
				Util.KDestroyGameObject(info2.serializedMinion.Get().gameObject);
			}
			serializedMinions.RemoveAt(minionIndex);
		}
	}

	public List<Info> GetStoredMinionInfo()
	{
		return serializedMinions;
	}
}
