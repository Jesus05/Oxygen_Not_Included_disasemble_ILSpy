using System;
using System.Collections.Generic;

public class RoleSlotUnlock
{
	public string id
	{
		get;
		protected set;
	}

	public string name
	{
		get;
		protected set;
	}

	public string description
	{
		get;
		protected set;
	}

	public List<Tuple<string, int>> slots
	{
		get;
		protected set;
	}

	public Func<bool> isSatisfied
	{
		get;
		protected set;
	}

	public RoleSlotUnlock(string id, string name, string description, List<Tuple<string, int>> slots, Func<bool> isSatisfied)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.slots = slots;
		this.isSatisfied = isSatisfied;
	}
}
