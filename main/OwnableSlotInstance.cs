using System.Diagnostics;

[DebuggerDisplay("{slot.Id}")]
public class OwnableSlotInstance : AssignableSlotInstance
{
	public OwnableSlotInstance(Assignables assignables, OwnableSlot slot)
		: base(assignables, slot)
	{
	}
}
