using System.Collections.Generic;
using UnityEngine;

public class Assignables : KMonoBehaviour
{
	protected List<AssignableSlotInstance> slots = new List<AssignableSlotInstance>();

	private static readonly EventSystem.IntraObjectHandler<Assignables> OnDeathDelegate = new EventSystem.IntraObjectHandler<Assignables>(delegate(Assignables component, object data)
	{
		component.OnDeath(data);
	});

	public AssignableSlotInstance this[int idx]
	{
		get
		{
			return slots[idx];
		}
	}

	public int Count => slots.Count;

	public IEnumerator<AssignableSlotInstance> GetEnumerator()
	{
		return slots.GetEnumerator();
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
		if (slot == null)
		{
			return null;
		}
		using (IEnumerator<AssignableSlotInstance> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AssignableSlotInstance current = enumerator.Current;
				if (current.slot == slot)
				{
					return current;
				}
			}
		}
		return null;
	}

	public Assignable AutoAssignSlot(AssignableSlot slot)
	{
		Assignable assignable = GetAssignable(slot);
		if ((Object)assignable != (Object)null)
		{
			return assignable;
		}
		Navigator component = GetComponent<Navigator>();
		MinionIdentity component2 = GetComponent<MinionIdentity>();
		int num = 2147483647;
		foreach (Assignable item in Game.Instance.assignmentManager)
		{
			if (!((Object)item == (Object)null) && !item.IsAssigned() && item.slot == slot && item.CanAutoAssignTo(component2))
			{
				int navigationCost = item.GetNavigationCost(component);
				if (navigationCost != -1 && navigationCost < num)
				{
					num = navigationCost;
					assignable = item;
				}
			}
		}
		if ((Object)assignable != (Object)null)
		{
			assignable.Assign(GetComponent<IAssignableIdentity>());
		}
		return assignable;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		using (IEnumerator<AssignableSlotInstance> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AssignableSlotInstance current = enumerator.Current;
				current.Unassign(true);
			}
		}
	}
}
