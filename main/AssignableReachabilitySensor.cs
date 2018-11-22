using UnityEngine;

public class AssignableReachabilitySensor : Sensor
{
	private struct SlotEntry
	{
		public AssignableSlotInstance slot;

		public bool isReachable;
	}

	private SlotEntry[] slots;

	private Navigator navigator;

	public AssignableReachabilitySensor(Sensors sensors)
		: base(sensors)
	{
		MinionAssignablesProxy minionAssignablesProxy = base.gameObject.GetComponent<MinionIdentity>().assignableProxy.Get();
		minionAssignablesProxy.ConfigureAssignableSlots();
		Assignables[] components = minionAssignablesProxy.GetComponents<Assignables>();
		if (components.Length == 0)
		{
			Debug.LogError(base.gameObject.GetProperName() + ": No 'Assignables' components found for AssignableReachabilitySensor", null);
		}
		int num = 0;
		for (int i = 0; i < components.Length; i++)
		{
			num += components[i].Count;
		}
		slots = new SlotEntry[num];
		int num2 = 0;
		foreach (Assignables assignables in components)
		{
			for (int k = 0; k < assignables.Count; k++)
			{
				slots[num2++].slot = assignables[k];
			}
		}
		navigator = GetComponent<Navigator>();
	}

	public bool IsReachable(AssignableSlot slot)
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].slot.slot == slot)
			{
				return slots[i].isReachable;
			}
		}
		Debug.LogError("Could not find slot: " + slot, null);
		return false;
	}

	public override void Update()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			SlotEntry slotEntry = slots[i];
			AssignableSlotInstance slot = slotEntry.slot;
			if (slot.IsAssigned())
			{
				bool flag = slot.assignable.GetNavigationCost(navigator) != -1;
				Operational component = slot.assignable.GetComponent<Operational>();
				if ((Object)component != (Object)null)
				{
					flag = (flag && component.IsOperational);
				}
				if (flag != slotEntry.isReachable)
				{
					slotEntry.isReachable = flag;
					slots[i] = slotEntry;
					Trigger(334784980, slotEntry);
				}
			}
			else if (slotEntry.isReachable)
			{
				slotEntry.isReachable = false;
				slots[i] = slotEntry;
				Trigger(334784980, slotEntry);
			}
		}
	}
}
