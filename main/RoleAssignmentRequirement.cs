using System;

public class RoleAssignmentRequirement
{
	private string description;

	public string id
	{
		get;
		protected set;
	}

	public Func<MinionResume, bool> isSatisfied
	{
		get;
		protected set;
	}

	public RoleAssignmentRequirement(string id, string description, Func<MinionResume, bool> isSatisfied)
	{
		this.id = id;
		this.description = description;
		this.isSatisfied = isSatisfied;
	}

	public virtual string GetDescription()
	{
		return description;
	}
}
