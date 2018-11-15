using KSerialization;
using System;
using System.Collections.Generic;

[Serializable]
public class ScheduleBlock
{
	[Serialize]
	public string name;

	[Serialize]
	public List<ScheduleBlockType> allowed_types;

	[Serialize]
	private string _groupId;

	public string GroupId
	{
		get
		{
			if (_groupId == null)
			{
				_groupId = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(allowed_types).Id;
			}
			return _groupId;
		}
		set
		{
			_groupId = value;
		}
	}

	public ScheduleBlock(string name, List<ScheduleBlockType> allowed_types, string groupId)
	{
		this.name = name;
		this.allowed_types = allowed_types;
		_groupId = groupId;
	}

	public bool IsAllowed(ScheduleBlockType type)
	{
		if (allowed_types != null)
		{
			foreach (ScheduleBlockType allowed_type in allowed_types)
			{
				if (type.IdHash == allowed_type.IdHash)
				{
					return true;
				}
			}
		}
		return false;
	}
}
