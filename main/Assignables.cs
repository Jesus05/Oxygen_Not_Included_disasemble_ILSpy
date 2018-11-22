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

	protected IAssignableIdentity GetAssignableIdentity()
	{
		MinionIdentity component = GetComponent<MinionIdentity>();
		if (!((Object)component != (Object)null))
		{
			return GetComponent<MinionAssignablesProxy>();
		}
		return component.assignableProxy.Get();
	}

	protected GameObject GetTargetGameObject()
	{
		IAssignableIdentity assignableIdentity = GetAssignableIdentity();
		if (!(assignableIdentity is MinionAssignablesProxy))
		{
			return null;
		}
		return ((assignableIdentity as MinionAssignablesProxy).target as KMonoBehaviour).gameObject;
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
		if (slot != null)
		{
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
		return null;
	}

	public Assignable AutoAssignSlot(AssignableSlot slot)
	{
		Assignable assignable = GetAssignable(slot);
		if (!((Object)assignable != (Object)null))
		{
			MinionAssignablesProxy component = GetComponent<MinionAssignablesProxy>();
			if (component.target == null)
			{
				component.RestoreTargetFromInstanceID();
			}
			Navigator component2 = component.GetTargetGameObject().GetComponent<Navigator>();
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
				assignable.Assign(GetComponent<IAssignableIdentity>());
			}
			return assignable;
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
