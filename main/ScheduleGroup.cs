using STRINGS;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{Id}")]
public class ScheduleGroup : Resource
{
	public int defaultSegments
	{
		get;
		private set;
	}

	public string description
	{
		get;
		private set;
	}

	public string notificationTooltip
	{
		get;
		private set;
	}

	public List<ScheduleBlockType> allowedTypes
	{
		get;
		private set;
	}

	public bool alarm
	{
		get;
		private set;
	}

	public ScheduleGroup(string id, ResourceSet parent, int defaultSegments, string name, string description, string notificationTooltip, List<ScheduleBlockType> allowedTypes, bool alarm = false)
		: base(id, parent, name)
	{
		this.defaultSegments = defaultSegments;
		this.description = description;
		this.notificationTooltip = notificationTooltip;
		this.allowedTypes = allowedTypes;
		this.alarm = alarm;
	}

	public bool Allowed(ScheduleBlockType type)
	{
		return allowedTypes.Contains(type);
	}

	public string GetTooltip()
	{
		return string.Format(UI.SCHEDULEGROUPS.TOOLTIP_FORMAT, Name, description);
	}
}
