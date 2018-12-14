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
		equippable.GetComponent<KBatchedAnimController>().enabled = false;
		equippable.OnEquip(slot);
		if (refreshHandle.TimeRemaining > 0f)
		{
			Debug.LogWarning(targetGameObject.GetProperName() + " is already in the process of changing equipment", null);
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
		equippable.GetComponent<KBatchedAnimController>().enabled = true;
		AssignableSlotInstance slot = GetSlot(equippable.slot);
		slot.Unassign(true);
		equippable.Trigger(-170173755, this);
		GameObject targetGameObject = GetTargetGameObject();
		if (!(bool)targetGameObject)
		{
			DebugUtil.DevAssert(false, "GetTargetGameObject returned null in Unequip");
		}
		else
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
				if (!((Object)component == (Object)null))
				{
					refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 1f, delegate
					{
						GameObject gameObject = (!((Object)this != (Object)null)) ? null : GetTargetGameObject();
						if ((bool)gameObject)
						{
							CreatureSimTemperatureTransfer component3 = gameObject.GetComponent<CreatureSimTemperatureTransfer>();
							if ((Object)component3 != (Object)null)
							{
								component3.RefreshRegistration();
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
