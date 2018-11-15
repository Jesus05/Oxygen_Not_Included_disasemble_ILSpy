using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownables : Assignables
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void UnassignAll()
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
