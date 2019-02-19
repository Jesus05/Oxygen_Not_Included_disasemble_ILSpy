using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equipment : Assignables
{
	private SchedulerHandle refreshHandle;

	private static readonly EventSystem.IntraObjectHandler<Equipment> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equipment>(delegate(Equipment component, object data)
	{
		component.destroyed = true;
	});

	public bool destroyed
	{
		get;
		private set;
	}

	private GameObject GetTargetGameObject()
	{
		IAssignableIdentity assignableIdentity = GetAssignableIdentity();
		MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)assignableIdentity;
		if ((bool)minionAssignablesProxy)
		{
			return minionAssignablesProxy.GetTargetGameObject();
		}
		return null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Equipment.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1502190696, SetDestroyedTrueDelegate);
		Subscribe(1969584890, SetDestroyedTrueDelegate);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		refreshHandle.ClearScheduler();
		Components.Equipment.Remove(this);
	}

	public void Equip(Equippable equippable)
	{
		AssignableSlotInstance slot = GetSlot(equippable.slot);
		slot.Assign(equippable);
		GameObject targetGameObject = GetTargetGameObject();
		targetGameObject.Trigger(-448952673, equippable.GetComponent<KPrefabID>());
		equippable.Trigger(-1617557748, this);
		Attributes attributes = targetGameObject.GetAttributes();
		if (attributes != null)
		{
			foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
			{
				attributes.Add(attributeModifier);
			}
		}
		SnapOn component = targetGameObject.GetComponent<SnapOn>();
		if ((Object)component != (Object)null)
		{
			component.AttachSnapOnByName(equippable.def.SnapOn);
			if (equippable.def.SnapOn1 != null)
			{
				component.AttachSnapOnByName(equippable.def.SnapOn1);
			}
		}
		KBatchedAnimController component2 = targetGameObject.GetComponent<KBatchedAnimController>();
		if ((Object)component2 != (Object)null && (Object)equippable.def.BuildOverride != (Object)null)
		{
			component2.GetComponent<SymbolOverrideController>().AddBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
		}
		if ((bool)equippable.transform.parent)
		{
			Storage component3 = equippable.transform.parent.GetComponent<Storage>();
			if ((bool)component3)
			{
				component3.Drop(equippable.gameObject);
			}
		}
		equippable.transform.parent = slot.gameObject.transform;
		equippable.transform.SetLocalPosition(Vector3.zero);
		equippable.OnEquip(slot);
		if (refreshHandle.TimeRemaining > 0f)
		{
			Debug.LogWarning(targetGameObject.GetProperName() + " is already in the process of changing equipment (equip)", null);
			refreshHandle.ClearScheduler();
		}
		CreatureSimTemperatureTransfer transferer = targetGameObject.GetComponent<CreatureSimTemperatureTransfer>();
		if (!((Object)component2 == (Object)null))
		{
			refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 2f, delegate
			{
				if ((Object)transferer != (Object)null)
				{
					transferer.RefreshRegistration();
				}
			}, null, null);
		}
		Game.Instance.Trigger(-2146166042, null);
	}

	public void Unequip(Equippable equippable)
	{
		AssignableSlotInstance slot = GetSlot(equippable.slot);
		slot.Unassign(true);
		equippable.Trigger(-170173755, this);
		GameObject targetGameObject = GetTargetGameObject();
		if ((bool)targetGameObject)
		{
			targetGameObject.Trigger(-1285462312, equippable.GetComponent<KPrefabID>());
			KBatchedAnimController component = targetGameObject.GetComponent<KBatchedAnimController>();
			if (!destroyed)
			{
				if ((Object)equippable.def.BuildOverride != (Object)null && (Object)component != (Object)null)
				{
					component.GetComponent<SymbolOverrideController>().TryRemoveBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
				}
				Attributes attributes = targetGameObject.GetAttributes();
				if (attributes != null)
				{
					foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
					{
						attributes.Remove(attributeModifier);
					}
				}
				if (!equippable.def.IsBody)
				{
					SnapOn component2 = targetGameObject.GetComponent<SnapOn>();
					component2.DetachSnapOnByName(equippable.def.SnapOn);
					if (equippable.def.SnapOn1 != null)
					{
						component2.DetachSnapOnByName(equippable.def.SnapOn1);
					}
				}
				if ((bool)equippable.transform.parent)
				{
					Storage component3 = equippable.transform.parent.GetComponent<Storage>();
					if ((bool)component3)
					{
						component3.Drop(equippable.gameObject);
					}
				}
				equippable.transform.parent = null;
				equippable.transform.SetPosition(targetGameObject.transform.GetPosition() + Vector3.up / 2f);
				KBatchedAnimController component4 = equippable.GetComponent<KBatchedAnimController>();
				if ((bool)component4)
				{
					component4.SetSceneLayer(Grid.SceneLayer.Ore);
				}
				if (!((Object)component == (Object)null))
				{
					if (refreshHandle.TimeRemaining > 0f)
					{
						refreshHandle.ClearScheduler();
					}
					refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 1f, delegate
					{
						GameObject gameObject = (!((Object)this != (Object)null)) ? null : GetTargetGameObject();
						if ((bool)gameObject)
						{
							CreatureSimTemperatureTransfer component5 = gameObject.GetComponent<CreatureSimTemperatureTransfer>();
							if ((Object)component5 != (Object)null)
							{
								component5.RefreshRegistration();
							}
						}
					}, null, null);
				}
			}
			Game.Instance.Trigger(-2146166042, null);
		}
	}

	public bool IsEquipped(Equippable equippable)
	{
		return equippable.assignee is Equipment && (Object)(Equipment)equippable.assignee == (Object)this && equippable.isEquipped;
	}

	public bool IsSlotOccupied(AssignableSlot slot)
	{
		EquipmentSlotInstance equipmentSlotInstance = GetSlot(slot) as EquipmentSlotInstance;
		return equipmentSlotInstance.IsAssigned() && (equipmentSlotInstance.assignable as Equippable).isEquipped;
	}

	public void UnequipAll()
	{
		foreach (AssignableSlotInstance slot in slots)
		{
			if ((Object)slot.assignable != (Object)null)
			{
				slot.assignable.Unassign();
			}
		}
	}
}
