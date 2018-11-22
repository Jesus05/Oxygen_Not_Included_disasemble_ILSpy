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
