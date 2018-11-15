using System;

public class RolePerk
{
	public HashedString id
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

	public Action<MinionResume> OnMinionsChanged
	{
		get;
		protected set;
	}

	public bool affectAll
	{
		get;
		protected set;
	}

	public RolePerk(string id_str, string description, Action<MinionResume> OnApply, Action<MinionResume> OnRemove, Action<MinionResume> OnMinionsChanged, bool affectAll = false)
	{
		id = new HashedString(id_str);
		this.description = description;
		this.OnApply = OnApply;
		this.OnRemove = OnRemove;
		this.OnMinionsChanged = OnMinionsChanged;
		this.affectAll = affectAll;
	}
}
