using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ScheduleGroupInstance
{
	[Serialize]
	private string scheduleGroupID;

	[Serialize]
	public int segments;

	public ScheduleGroup scheduleGroup
	{
		get
		{
			return Db.Get().ScheduleGroups.Get(scheduleGroupID);
		}
		set
		{
			scheduleGroupID = value.Id;
		}
	}

	public ScheduleGroupInstance(ScheduleGroup scheduleGroup)
	{
		this.scheduleGroup = scheduleGroup;
		segments = scheduleGroup.defaultSegments;
	}
}
