using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownable : Assignable, ISaveLoadable, IEffectDescriptor
{
	private Color unownedTint = Color.gray;

	private Color ownedTint = Color.white;

	public override void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee != assignee)
		{
			if (base.slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity))
			{
				Ownables soleOwner = new_assignee.GetSoleOwner();
				Ownables component = soleOwner.GetComponent<Ownables>();
				AssignableSlotInstance slot = component.GetSlot(base.slot);
				if (slot != null)
				{
					Assignable assignable = slot.assignable;
					if ((Object)assignable != (Object)null)
					{
						assignable.Unassign();
					}
				}
			}
			base.Assign(new_assignee);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateTint();
		UpdateStatusString();
		base.OnAssign += OnNewAssignment;
	}

	private void OnNewAssignment(IAssignableIdentity assignables)
	{
		UpdateTint();
		UpdateStatusString();
	}

	private void UpdateTint()
	{
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if ((Object)component != (Object)null && component.HasBatchInstanceData)
		{
			component.TintColour = ((assignee != null) ? ownedTint : unownedTint);
		}
		else
		{
			KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
			if ((Object)component2 != (Object)null && component2.HasBatchInstanceData)
			{
				component2.TintColour = ((assignee != null) ? ownedTint : unownedTint);
			}
		}
	}

	private void UpdateStatusString()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (!((Object)component == (Object)null))
		{
			StatusItem statusItem = null;
			component.SetStatusItem(status_item: (assignee == null) ? Db.Get().BuildingStatusItems.Unassigned : ((assignee is MinionIdentity) ? Db.Get().BuildingStatusItems.AssignedTo : ((!(assignee is Room)) ? Db.Get().BuildingStatusItems.AssignedTo : Db.Get().BuildingStatusItems.AssignedTo)), category: Db.Get().StatusItemCategories.Main, data: this);
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.ASSIGNEDDUPLICANT, UI.BUILDINGEFFECTS.TOOLTIPS.ASSIGNEDDUPLICANT, Descriptor.DescriptorType.Requirement);
		list.Add(item);
		return list;
	}
}
