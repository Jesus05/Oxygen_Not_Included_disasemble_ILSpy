using Klei.AI;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoredMinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption
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
	public Dictionary<string, float> ExperienceByRoleID = new Dictionary<string, float>();

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public string currentRole;

	[Serialize]
	public string targetRole;

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

	public bool HasPerk(RolePerk perk)
	{
		foreach (RoleConfig rolesConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (rolesConfig.HasPerk(perk) && MasteryByRoleID.ContainsKey(rolesConfig.id) && MasteryByRoleID[rolesConfig.id])
			{
				return true;
			}
		}
		return Game.Instance.roleManager.GetRole(currentRole) != null && Game.Instance.roleManager.GetRole(currentRole).HasPerk(perk);
	}

	protected override void OnPrefabInit()
	{
		assignableProxy = new Ref<MinionAssignablesProxy>();
	}

	protected override void OnSpawn()
	{
		if ((Object)assignableProxy.Get() == (Object)null)
		{
			assignableProxy = MinionAssignablesProxy.InitAssignableProxy(assignableProxy, this);
		}
		CleanupLimboMinions();
	}

	private void CleanupLimboMinions()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		bool flag = false;
		if (component.InstanceID == -1)
		{
			Output.LogWarning("Stored minion with an invalid kpid! Attempting to recover...", storedName);
			flag = true;
			if ((Object)KPrefabIDTracker.Get().GetInstance(-1) != (Object)null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
		}
		bool flag2 = false;
		foreach (MinionStorage item in Components.MinionStorages.Items)
		{
			for (int i = 0; i < item.GetStoredMinionInfo().Count; i++)
			{
				MinionStorage.Info info = item.GetStoredMinionInfo()[i];
				if (flag && info.serializedMinion != null && info.serializedMinion.GetId() == -1 && info.name == storedName)
				{
					Output.LogWarning("Found a minion storage with an invalid ref, rebinding.", storedName, item.gameObject.name);
					info = new MinionStorage.Info(storedName, new Ref<KPrefabID>(component));
					item.GetStoredMinionInfo()[i] = info;
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
			Output.LogWarning("Found a stored minion that wasn't in any minion storage. Respawning them at the portal.", storedName);
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

	public bool IsNull()
	{
		return (Object)this == (Object)null;
	}
}
