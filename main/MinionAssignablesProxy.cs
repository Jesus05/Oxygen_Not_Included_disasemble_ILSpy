using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MinionAssignablesProxy : KMonoBehaviour, IAssignableIdentity
{
	public List<Ownables> ownables;

	[Serialize]
	private int target_instance_id = -1;

	private bool slotsConfigured = false;

	private static readonly EventSystem.IntraObjectHandler<MinionAssignablesProxy> OnAssignablesChangedDelegate = new EventSystem.IntraObjectHandler<MinionAssignablesProxy>(delegate(MinionAssignablesProxy component, object data)
	{
		component.OnAssignablesChanged(data);
	});

	public IAssignableIdentity target
	{
		get;
		private set;
	}

	public bool IsConfigured => slotsConfigured;

	public int TargetInstanceID => target_instance_id;

	public GameObject GetTargetGameObject()
	{
		if (target == null && target_instance_id != -1)
		{
			RestoreTargetFromInstanceID();
		}
		KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)target;
		if (!((Object)kMonoBehaviour != (Object)null))
		{
			return null;
		}
		return kMonoBehaviour.gameObject;
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
		ConfigureAssignableSlots();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
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
			if ((bool)instance)
			{
				IAssignableIdentity component = instance.GetComponent<IAssignableIdentity>();
				SetTarget(component, instance.gameObject);
			}
			else
			{
				Debug.LogWarningFormat("RestoreTargetFromInstanceID target ID {0} was not found, destroying proxy object.", target_instance_id);
				Util.KDestroyGameObject(base.gameObject);
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RestoreTargetFromInstanceID();
		if (target != null)
		{
			Subscribe(-1585839766, OnAssignablesChangedDelegate);
			Game.Instance.assignmentManager.AddToAssignmentGroup("public", this);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
		GetComponent<Ownables>().UnassignAll();
		GetComponent<Equipment>().UnequipAll();
		Components.MinionAssignablesProxy.Remove(this);
	}

	private void OnAssignablesChanged(object data)
	{
		if (!target.IsNull())
		{
			KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)target;
			kMonoBehaviour.Trigger(-1585839766, data);
		}
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
					MinionIdentity minionIdentity = target as MinionIdentity;
					if ((bool)minionIdentity)
					{
						minionIdentity.ValidateProxy();
					}
					else
					{
						StoredMinionIdentity storedMinionIdentity = target as StoredMinionIdentity;
						if ((bool)storedMinionIdentity)
						{
							storedMinionIdentity.ValidateProxy();
						}
					}
				}
			}
		}
	}

	public List<Ownables> GetOwners()
	{
		CheckTarget();
		return target.GetOwners();
	}

	public string GetProperName()
	{
		CheckTarget();
		return target.GetProperName();
	}

	public Ownables GetSoleOwner()
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
		bool flag = false;
		if (assignableProxyRef == null)
		{
			assignableProxyRef = new Ref<MinionAssignablesProxy>();
		}
		GameObject gameObject = ((KMonoBehaviour)source).gameObject;
		MinionAssignablesProxy minionAssignablesProxy = assignableProxyRef.Get();
		if ((Object)minionAssignablesProxy == (Object)null)
		{
			flag = true;
			GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab(MinionAssignablesProxyConfig.ID), Grid.SceneLayer.NoLayer, null, 0);
			minionAssignablesProxy = gameObject2.GetComponent<MinionAssignablesProxy>();
			minionAssignablesProxy.SetTarget(source, gameObject);
			gameObject2.SetActive(true);
			assignableProxyRef.Set(minionAssignablesProxy);
		}
		else
		{
			minionAssignablesProxy.SetTarget(source, gameObject);
		}
		return assignableProxyRef;
	}
}
