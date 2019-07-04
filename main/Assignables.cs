using System.Collections.Generic;
using UnityEngine;

public class Assignables : KMonoBehaviour
{
	protected List<AssignableSlotInstance> slots = new List<AssignableSlotInstance>();

	private static readonly EventSystem.IntraObjectHandler<Assignables> OnDeathDelegate = new EventSystem.IntraObjectHandler<Assignables>(delegate(Assignables component, object data)
	{
		component.OnDeath(data);
	});

	public List<AssignableSlotInstance> Slots => slots;

	protected IAssignableIdentity GetAssignableIdentity()
	{
		MinionIdentity component = GetComponent<MinionIdentity>();
		if (!((Object)component != (Object)null))
		{
			return GetComponent<MinionAssignablesProxy>();
		}
		return component.assignableProxy.Get();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1623392196, OnDeathDelegate);
	}

	private void OnDeath(object data)
	{
		foreach (AssignableSlotInstance slot in slots)
		{
			slot.Unassign(true);
		}
	}

	public void Add(AssignableSlotInstance slot_instance)
	{
		slots.Add(slot_instance);
	}

	public Assignable GetAssignable(AssignableSlot slot)
	{
		return GetSlot(slot)?.assignable;
	}

	public AssignableSlotInstance GetSlot(AssignableSlot slot)
	{
		Debug.Assert(slots.Count > 0, "GetSlot called with no slots configured");
		if (slot != null)
		{
			foreach (AssignableSlotInstance slot2 in slots)
			{
				if (slot2.slot == slot)
				{
					return slot2;
				}
			}
			return null;
		}
		return null;
	}

	public Assignable AutoAssignSlot(AssignableSlot slot)
	{
		Assignable assignable = GetAssignable(slot);
		if (!((Object)assignable != (Object)null))
		{
			MinionAssignablesProxy component = GetComponent<MinionAssignablesProxy>();
			GameObject targetGameObject = component.GetTargetGameObject();
			if (!((Object)targetGameObject == (Object)null))
			{
				Navigator component2 = targetGameObject.GetComponent<Navigator>();
				IAssignableIdentity assignableIdentity = GetAssignableIdentity();
				int num = 2147483647;
				foreach (Assignable item in Game.Instance.assignmentManager)
				{
					if (!((Object)item == (Object)null) && !item.IsAssigned() && item.slot == slot && item.CanAutoAssignTo(assignableIdentity))
					{
						int navigationCost = item.GetNavigationCost(component2);
						if (navigationCost != -1 && navigationCost < num)
						{
							num = navigationCost;
							assignable = item;
						}
					}
				}
				if ((Object)assignable != (Object)null)
				{
					assignable.Assign(assignableIdentity);
				}
				return assignable;
			}
			Debug.LogWarning("AutoAssignSlot failed, proxy game object was null.");
			return null;
		}
		return assignable;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (AssignableSlotInstance slot in slots)
		{
			slot.Unassign(true);
		}
	}
}
