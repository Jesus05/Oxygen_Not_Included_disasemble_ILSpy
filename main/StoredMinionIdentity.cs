using Database;
using Klei.AI;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoredMinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption, IPersonalPriorityManager
{
	[Serialize]
	public string storedName;

	[Serialize]
	public string gender;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public KCompBuilder.BodyData bodyData;

	[Serialize]
	public List<Ref<KPrefabID>> assignedItems;

	[Serialize]
	public List<Ref<KPrefabID>> equippedItems;

	[Serialize]
	public List<string> traitIDs;

	[Serialize]
	public List<ResourceRef<Accessory>> accessories;

	[Serialize]
	public List<Tag> forbiddenTags;

	[Serialize]
	public Ref<MinionAssignablesProxy> assignableProxy;

	[Serialize]
	public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public float TotalExperienceGained;

	[Serialize]
	public string currentHat;

	[Serialize]
	public string targetHat;

	[Serialize]
	public Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();

	[Serialize]
	public List<AttributeLevels.LevelSaveLoad> attributeLevels;

	[Serialize]
	public string genderStringKey
	{
		get;
		set;
	}

	[Serialize]
	public string nameStringKey
	{
		get;
		set;
	}

	public bool HasPerk(SkillPerk perk)
	{
		foreach (KeyValuePair<string, bool> item in MasteryBySkillID)
		{
			if (item.Value && Db.Get().Skills.Get(item.Key).perks.Contains(perk))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasMasteredSkill(string skillId)
	{
		return MasteryBySkillID.ContainsKey(skillId) && MasteryBySkillID[skillId];
	}

	protected override void OnPrefabInit()
	{
		assignableProxy = new Ref<MinionAssignablesProxy>();
	}

	protected override void OnSpawn()
	{
		ValidateProxy();
		CleanupLimboMinions();
	}

	public void ValidateProxy()
	{
		assignableProxy = MinionAssignablesProxy.InitAssignableProxy(assignableProxy, this);
	}

	private void CleanupLimboMinions()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		bool flag = false;
		if (component.InstanceID == -1)
		{
			DebugUtil.LogWarningArgs("Stored minion with an invalid kpid! Attempting to recover...", storedName);
			flag = true;
			if ((Object)KPrefabIDTracker.Get().GetInstance(component.InstanceID) != (Object)null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
		}
		if (component.conflicted)
		{
			DebugUtil.LogWarningArgs("Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, storedName);
			if ((Object)KPrefabIDTracker.Get().GetInstance(component.InstanceID) != (Object)null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs("Restored as:", component.InstanceID);
		}
		assignableProxy.Get().SetTarget(this, base.gameObject);
		bool flag2 = false;
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			List<MinionStorage.Info> storedMinionInfo = item.GetStoredMinionInfo();
			for (int i = 0; i < storedMinionInfo.Count; i++)
			{
				MinionStorage.Info info = storedMinionInfo[i];
				if (flag && info.serializedMinion != null && info.serializedMinion.GetId() == -1 && info.name == storedName)
				{
					DebugUtil.LogWarningArgs("Found a minion storage with an invalid ref, rebinding.", component.InstanceID, storedName, item.gameObject.name);
					info = (storedMinionInfo[i] = new MinionStorage.Info(storedName, new Ref<KPrefabID>(component)));
					Assignable component2 = item.GetComponent<Assignable>();
					component2.Assign(this);
					flag2 = true;
					break;
				}
				if (info.serializedMinion != null && (Object)info.serializedMinion.Get() == (Object)component)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				break;
			}
		}
		if (!flag2)
		{
			DebugUtil.LogWarningArgs("Found a stored minion that wasn't in any minion storage. Respawning them at the portal.", component.InstanceID, storedName);
			GameObject telepad = GameUtil.GetTelepad();
			if ((Object)telepad != (Object)null)
			{
				MinionStorage.DeserializeMinion(component.gameObject, telepad.transform.GetPosition());
			}
		}
	}

	public string GetProperName()
	{
		return storedName;
	}

	public List<Ownables> GetOwners()
	{
		return assignableProxy.Get().ownables;
	}

	public Ownables GetSoleOwner()
	{
		return assignableProxy.Get().GetComponent<Ownables>();
	}

	public Accessory GetAccessory(AccessorySlot slot)
	{
		for (int i = 0; i < accessories.Count; i++)
		{
			if (accessories[i].Get() != null && accessories[i].Get().slot == slot)
			{
				return accessories[i].Get();
			}
		}
		return null;
	}

	public bool IsNull()
	{
		return (Object)this == (Object)null;
	}

	public string GetStorageReason()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info item2 in item.GetStoredMinionInfo())
			{
				MinionStorage.Info current2 = item2;
				if ((Object)current2.serializedMinion.Get() == (Object)component)
				{
					return item.GetProperName();
				}
			}
		}
		return string.Empty;
	}

	public bool IsPermittedToConsume(string consumable)
	{
		foreach (Tag forbiddenTag in forbiddenTags)
		{
			if (forbiddenTag == (Tag)consumable)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsChoreGroupDisabled(ChoreGroup chore_group)
	{
		foreach (string traitID in traitIDs)
		{
			if (Db.Get().traits.Exists(traitID))
			{
				Trait trait = Db.Get().traits.Get(traitID);
				if (trait.disabledChoreGroups != null)
				{
					ChoreGroup[] disabledChoreGroups = trait.disabledChoreGroups;
					foreach (ChoreGroup choreGroup in disabledChoreGroups)
					{
						if (choreGroup.IdHash == chore_group.IdHash)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public int GetPersonalPriority(ChoreGroup chore_group)
	{
		if (choreGroupPriorities.TryGetValue(chore_group.IdHash, out ChoreConsumer.PriorityInfo value))
		{
			return value.priority;
		}
		return 0;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
	{
	}

	public void ResetPersonalPriorities()
	{
	}
}
