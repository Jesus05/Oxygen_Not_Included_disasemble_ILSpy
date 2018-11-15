using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Id}")]
public class ScheduleBlockType : Resource
{
	public Color color
	{
		get;
		private set;
	}

	public string description
	{
		get;
		private set;
	}

	public ScheduleBlockType(string id, ResourceSet parent, string name, string description, Color color)
		: base(id, parent, name)
	{
		this.color = color;
		this.description = description;
	}
}
