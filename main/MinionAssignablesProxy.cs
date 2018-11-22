using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class MinionAssignablesProxy : KMonoBehaviour, IAssignableIdentity
{
	public List<Ownables> ownables;

	[Serialize]
	private int target_instance_id = -1;

	private bool slotsConfigured = false;

	public IAssignableIdentity target
	{
		get;
		private set;
	}

	public GameObject GetTargetGameObject()
	{
		if (target == null)
		{
			if (target_instance_id == -1)
			{
				return null;
			}
			RestoreTargetFromInstanceID();
		}
		return (target as KMonoBehaviour).gameObject;
	}

	public void SetTarget(IAssignableIdentity target, GameObject targetGO)
	{
		if ((Object)targetGO == (Object)null)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		this.target = target;
		target_instance_id = targetGO.GetComponent<KPrefabID>().InstanceID;
		base.gameObject.name = "Minion Assignables Proxy : " + targetGO.name;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ownables = new List<Ownables>
		{
			base.gameObject.AddOrGet<Ownables>()
		};
		Components.MinionAssignablesProxy.Add(this);
	}

	public void ConfigureAssignableSlots()
	{
		if (!slotsConfigured)
		{
			Ownables component = GetComponent<Ownables>();
			Equipment component2 = GetComponent<Equipment>();
			if ((Object)component2 != (Object)null)
			{
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
			}
			slotsConfigured = true;
		}
	}

	public void RestoreTargetFromInstanceID()
	{
		if (target_instance_id != -1 && target == null)
		{
			KPrefabID instance = KPrefabIDTracker.Get().GetInstance(target_instance_id);
			IAssignableIdentity component = instance.GetComponent<IAssignableIdentity>();
			SetTarget(component, instance.gameObject);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConfigureAssignableSlots();
		RestoreTargetFromInstanceID();
		Subscribe(-1585839766, delegate(object data)
		{
			if (!target.IsNull())
			{
				(target as KMonoBehaviour).Trigger(-1585839766, data);
			}
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
		GetComponent<Ownables>().UnassignAll();
		GetComponent<Equipment>().UnequipAll();
		Components.MinionAssignablesProxy.Remove(this);
	}

	private void CheckTarget()
	{
		if (target == null)
		{
			KPrefabID instance = KPrefabIDTracker.Get().GetInstance(target_instance_id);
			if ((Object)instance != (Object)null)
			{
				target = instance.GetComponent<IAssignableIdentity>();
				if (target != null)
				{
					if (target is MinionIdentity)
					{
						(target as MinionIdentity).assignableProxy = InitAssignableProxy((target as MinionIdentity).assignableProxy, target as MinionIdentity);
					}
					else if (target is StoredMinionIdentity)
					{
						(target as StoredMinionIdentity).assignableProxy = InitAssignableProxy((target as StoredMinionIdentity).assignableProxy, target as StoredMinionIdentity);
					}
				}
			}
		}
	}

	List<Ownables> IAssignableIdentity.GetOwners()
	{
		CheckTarget();
		return target.GetOwners();
	}

	string IAssignableIdentity.GetProperName()
	{
		CheckTarget();
		return target.GetProperName();
	}

	Ownables IAssignableIdentity.GetSoleOwner()
	{
		CheckTarget();
		return target.GetSoleOwner();
	}

	public bool IsNull()
	{
		CheckTarget();
		return target.IsNull();
	}

	public static Ref<MinionAssignablesProxy> InitAssignableProxy(Ref<MinionAssignablesProxy> assignableProxyRef, IAssignableIdentity source)
	{
		if (assignableProxyRef == null)
		{
			assignableProxyRef = new Ref<MinionAssignablesProxy>();
		}
		if ((Object)assignableProxyRef.Get() == (Object)null)
		{
			Tag tag = MinionAssignablesProxyConfig.ID;
			MinionAssignablesProxy component = GameUtil.KInstantiate(Assets.GetPrefab(tag), Grid.SceneLayer.NoLayer, null, 0).GetComponent<MinionAssignablesProxy>();
			component.SetTarget(source, (source as KMonoBehaviour).gameObject);
			component.gameObject.SetActive(true);
			assignableProxyRef.Set(component);
		}
		assignableProxyRef.Get().SetTarget(source, (source as KMonoBehaviour).gameObject);
		return assignableProxyRef;
	}
}
