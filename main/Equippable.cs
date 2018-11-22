using Klei.AI;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equippable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor, IQuality, IHasSortOrder
{
	private QualityLevel quality = QualityLevel.Poor;

	[MyCmpAdd]
	private EquippableWorkable equippableWorkable;

	[MyCmpReq]
	private KSelectable selectable;

	public DefHandle defHandle;

	[Serialize]
	public bool isEquipped;

	private bool destroyed = false;

	private static readonly EventSystem.IntraObjectHandler<Equippable> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equippable>(delegate(Equippable component, object data)
	{
		component.destroyed = true;
	});

	public EquipmentDef def
	{
		get
		{
			return defHandle.Get<EquipmentDef>();
		}
		set
		{
			defHandle.Set(value);
		}
	}

	public int sortOrder
	{
		get;
		set;
	}

	public QualityLevel GetQuality()
	{
		return quality;
	}

	public void SetQuality(QualityLevel level)
	{
		quality = level;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (def.AdditionalTags != null)
		{
			Tag[] additionalTags = def.AdditionalTags;
			foreach (Tag tag in additionalTags)
			{
				GetComponent<KPrefabID>().AddTag(tag);
			}
		}
	}

	protected override void OnSpawn()
	{
		if (isEquipped)
		{
			if (assignee != null && assignee is MinionIdentity)
			{
				assignee = (assignee as MinionIdentity).assignableProxy.Get();
				assignee_identityRef.Set(assignee as KMonoBehaviour);
			}
			if (assignee == null && (Object)assignee_identityRef.Get() != (Object)null)
			{
				assignee = assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
			}
			if (assignee != null)
			{
				assignee.GetSoleOwner().GetComponent<Equipment>().Equip(this);
			}
			else
			{
				Debug.LogWarning("Equippable trying to be equipped to missing prefab", null);
				isEquipped = false;
			}
		}
		Subscribe(1969584890, SetDestroyedTrueDelegate);
	}

	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee != assignee)
		{
			if (base.slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity))
			{
				Equipment component = new_assignee.GetSoleOwner().GetComponent<Equipment>();
				AssignableSlotInstance slot = component.GetSlot(base.slot);
				if ((Object)slot.assignable != (Object)null)
				{
					slot.Unassign(true);
				}
			}
			base.Assign(new_assignee);
		}
	}

	public override void Unassign()
	{
		if (isEquipped)
		{
			Equipment equipment = (!(assignee is MinionIdentity)) ? (assignee as KMonoBehaviour).GetComponent<Equipment>() : (assignee as MinionIdentity).assignableProxy.Get().GetComponent<Equipment>();
			equipment.Unequip(this);
			OnUnequip();
		}
		base.Unassign();
	}

	public void OnEquip(AssignableSlotInstance slot)
	{
		isEquipped = true;
		if ((Object)SelectTool.Instance.selected == (Object)selectable)
		{
			SelectTool.Instance.Select(null, false);
		}
		GetComponent<KBatchedAnimController>().enabled = false;
		GetComponent<KSelectable>().IsSelectable = false;
		base.transform.parent = slot.gameObject.transform;
		base.transform.SetLocalPosition(Vector3.zero);
		Effects component = slot.gameObject.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Effects>();
		if ((Object)component != (Object)null)
		{
			foreach (Effect effectImmunite in def.EffectImmunites)
			{
				component.AddImmunity(effectImmunite);
			}
		}
		if (def.OnEquipCallBack != null)
		{
			def.OnEquipCallBack(this);
		}
		GetComponent<KPrefabID>().AddTag(GameTags.Equipped);
	}

	public void OnUnequip()
	{
		isEquipped = false;
		if (!destroyed)
		{
			GetComponent<KPrefabID>().RemoveTag(GameTags.Equipped);
			GetComponent<KBatchedAnimController>().enabled = true;
			GetComponent<KSelectable>().IsSelectable = true;
			if (assignee != null)
			{
				Effects component = assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject()
					.GetComponent<Effects>();
				if ((Object)component != (Object)null)
				{
					foreach (Effect effectImmunite in def.EffectImmunites)
					{
						component.RemoveImmunity(effectImmunite);
					}
				}
				base.gameObject.transform.SetPosition(assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject()
					.transform.GetPosition() + Vector3.up / 2f);
				}
				base.transform.parent = null;
				if (def.OnUnequipCallBack != null)
				{
					def.OnUnequipCallBack(this);
				}
			}
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			if (!((Object)def != (Object)null))
			{
				return new List<Descriptor>();
			}
			List<Descriptor> equipmentEffects = GameUtil.GetEquipmentEffects(def);
			if (def.additionalDescriptors != null)
			{
				foreach (Descriptor additionalDescriptor in def.additionalDescriptors)
				{
					equipmentEffects.Add(additionalDescriptor);
				}
			}
			return equipmentEffects;
		}
	}
