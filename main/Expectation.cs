using System;

public class Expectation
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

	public Action<MinionResume> OnApply
	{
		get;
		protected set;
	}

	public Action<MinionResume> OnRemove
	{
		get;
		protected set;
	}

	public Expectation(string id, string name, string description, Action<MinionResume> OnApply, Action<MinionResume> OnRemove)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.OnApply = OnApply;
		this.OnRemove = OnRemove;
	}
}
