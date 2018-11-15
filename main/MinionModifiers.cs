using Klei.AI;
using KSerialization;
using System;
using System.IO;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionModifiers : Modifiers, ISaveLoadable
{
	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnDeathDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnAttachFollowCamDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnAttachFollowCam(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnDetachFollowCamDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnDetachFollowCam(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MinionModifiers> OnBeginChoreDelegate = new EventSystem.IntraObjectHandler<MinionModifiers>(delegate(MinionModifiers component, object data)
	{
		component.OnBeginChore(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		foreach (Klei.AI.Attribute resource in Db.Get().Attributes.resources)
		{
			if (attributes.Get(resource) == null)
			{
				attributes.Add(resource);
			}
		}
		Traits component = GetComponent<Traits>();
		Trait trait = Db.Get().traits.Get(MinionConfig.MINION_BASE_TRAIT_ID);
		component.Add(trait);
		foreach (Disease resource2 in Db.Get().Diseases.resources)
		{
			AmountInstance amountInstance = AddAmount(resource2.amount);
			attributes.Add(resource2.cureSpeedBase);
			amountInstance.SetValue(0f);
		}
		Equipment component2 = GetComponent<Equipment>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			Ownables component3 = GetComponent<Ownables>();
			foreach (AssignableSlot resource3 in Db.Get().AssignableSlots.resources)
			{
				if (resource3 is OwnableSlot)
				{
					OwnableSlotInstance slot_instance = new OwnableSlotInstance(component3, (OwnableSlot)resource3);
					component3.Add(slot_instance);
				}
				else if (resource3 is EquipmentSlot)
				{
					EquipmentSlotInstance slot_instance2 = new EquipmentSlotInstance(component2, (EquipmentSlot)resource3);
					component2.Add(slot_instance2);
				}
			}
		}
		ChoreConsumer component4 = GetComponent<ChoreConsumer>();
		if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
		{
			component4.AddProvider(GlobalChoreProvider.Instance);
			base.gameObject.AddComponent<QualityOfLifeNeed>();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ChoreConsumer component = GetComponent<ChoreConsumer>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			Subscribe(1623392196, OnDeathDelegate);
			Subscribe(-1506069671, OnAttachFollowCamDelegate);
			Subscribe(-485480405, OnDetachFollowCamDelegate);
			Subscribe(-1988963660, OnBeginChoreDelegate);
			AmountInstance amountInstance = this.GetAmounts().Get("Calories");
			amountInstance.OnMaxValueReached = (System.Action)Delegate.Combine(amountInstance.OnMaxValueReached, new System.Action(OnMaxCaloriesReached));
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
			base.transform.SetPosition(position);
			base.gameObject.layer = LayerMask.NameToLayer("Default");
			SetupDependentAttribute(Db.Get().Attributes.CarryAmount, Db.Get().AttributeConverters.CarryAmountFromStrength);
		}
	}

	private AmountInstance AddAmount(Amount amount)
	{
		AmountInstance instance = new AmountInstance(amount, base.gameObject);
		return amounts.Add(instance);
	}

	private void SetupDependentAttribute(Klei.AI.Attribute targetAttribute, AttributeConverter attributeConverter)
	{
		Klei.AI.Attribute attribute = attributeConverter.attribute;
		AttributeInstance attributeInstance = attribute.Lookup(this);
		AttributeModifier target_modifier = new AttributeModifier(targetAttribute.Id, attributeConverter.Lookup(this).Evaluate(), attribute.Name, false, false, false);
		this.GetAttributes().Add(target_modifier);
		AttributeInstance attributeInstance2 = attributeInstance;
		attributeInstance2.OnDirty = (System.Action)Delegate.Combine(attributeInstance2.OnDirty, (System.Action)delegate
		{
			target_modifier.SetValue(attributeConverter.Lookup(this).Evaluate());
		});
	}

	private void OnDeath(object data)
	{
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			item.GetComponent<Effects>().Add("Mourning", true);
		}
	}

	private void OnMaxCaloriesReached()
	{
		GetComponent<Effects>().Add("WellFed", true);
	}

	private void OnBeginChore(object data)
	{
		Storage component = GetComponent<Storage>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.DropAll(false);
		}
	}

	public override void OnSerialize(BinaryWriter writer)
	{
		base.OnSerialize(writer);
	}

	public override void OnDeserialize(IReader reader)
	{
		base.OnDeserialize(reader);
	}

	private void OnAttachFollowCam(object data)
	{
		GetComponent<Effects>().Add("CenterOfAttention", false);
	}

	private void OnDetachFollowCam(object data)
	{
		GetComponent<Effects>().Remove("CenterOfAttention");
	}
}
