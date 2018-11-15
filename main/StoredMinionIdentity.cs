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

	private List<Ownables> ownablesList;

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

	protected override void OnPrefabInit()
	{
		Ownables component = GetComponent<Ownables>();
		Equipment component2 = GetComponent<Equipment>();
		foreach (AssignableSlot resource in Db.Get().AssignableSlots.resources)
		{
			if (resource is OwnableSlot)
			{
				OwnableSlotInstance slot_instance = new OwnableSlotInstance(component, (OwnableSlot)resource);
				component.Add(slot_instance);
			}
			else if (resource is EquipmentSlot)
			{
				EquipmentSlotInstance slot_instance2 = new EquipmentSlotInstance(component2, (EquipmentSlot)resource);
				component2.Add(slot_instance2);
			}
		}
		ownablesList = new List<Ownables>
		{
			component
		};
	}

	protected override void OnSpawn()
	{
	}

	public string GetProperName()
	{
		return storedName;
	}

	public List<Ownables> GetOwners()
	{
		return ownablesList;
	}

	public Ownables GetSoleOwner()
	{
		return GetComponent<Ownables>();
	}

	public bool IsNull()
	{
		return (Object)this == (Object)null;
	}
}
