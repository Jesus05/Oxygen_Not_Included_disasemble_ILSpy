using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public abstract class Assignable : KMonoBehaviour, ISaveLoadable
{
	public string slotID;

	private AssignableSlot _slot;

	public IAssignableIdentity assignee;

	[Serialize]
	protected Ref<KMonoBehaviour> assignee_identityRef = new Ref<KMonoBehaviour>();

	[Serialize]
	private string assignee_groupID = "";

	public AssignableSlot[] subSlots;

	public bool canBePublic = false;

	[Serialize]
	private bool canBeAssigned = true;

	private List<Func<MinionIdentity, bool>> autoassignmentPreconditions = new List<Func<MinionIdentity, bool>>();

	private List<Func<MinionIdentity, bool>> assignmentPreconditions = new List<Func<MinionIdentity, bool>>();

	public Func<MinionAssignablesProxy, bool> eligibleFilter = null;

	public AssignableSlot slot
	{
		get
		{
			if (_slot == null)
			{
				_slot = Db.Get().AssignableSlots.Get(slotID);
			}
			return _slot;
		}
	}

	public bool CanBeAssigned => canBeAssigned;

	public event Action<IAssignableIdentity> OnAssign;

	[OnDeserialized]
	internal void OnDeserialized()
	{
	}

	private void RestoreAssignee()
	{
		IAssignableIdentity savedAssignee = GetSavedAssignee();
		if (savedAssignee != null)
		{
			Assign(savedAssignee);
		}
	}

	private IAssignableIdentity GetSavedAssignee()
	{
		if (!((UnityEngine.Object)assignee_identityRef.Get() != (UnityEngine.Object)null))
		{
			if (!(assignee_groupID != ""))
			{
				return null;
			}
			return Game.Instance.assignmentManager.assignment_groups[assignee_groupID];
		}
		return assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RestoreAssignee();
		Game.Instance.assignmentManager.Add(this);
		if (assignee == null && canBePublic)
		{
			Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
		}
	}

	protected override void OnCleanUp()
	{
		Unassign();
		Game.Instance.assignmentManager.Remove(this);
		base.OnCleanUp();
	}

	public bool CanAutoAssignTo(IAssignableIdentity identity)
	{
		MinionIdentity minionIdentity = identity as MinionIdentity;
		if (!((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null))
		{
			if (CanAssignTo(minionIdentity))
			{
				foreach (Func<MinionIdentity, bool> autoassignmentPrecondition in autoassignmentPreconditions)
				{
					if (!autoassignmentPrecondition(minionIdentity))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public bool CanAssignTo(IAssignableIdentity identity)
	{
		MinionIdentity minionIdentity = identity as MinionIdentity;
		if (!((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null))
		{
			foreach (Func<MinionIdentity, bool> assignmentPrecondition in assignmentPreconditions)
			{
				if (!assignmentPrecondition(minionIdentity))
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	public bool IsAssigned()
	{
		return assignee != null;
	}

	public virtual void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee != assignee)
		{
			if (new_assignee is KMonoBehaviour)
			{
				KMonoBehaviour obj = new_assignee as KMonoBehaviour;
				if (!CanAssignTo(new_assignee))
				{
					return;
				}
				assignee_identityRef.Set(obj);
				assignee_groupID = "";
			}
			else if (new_assignee is AssignmentGroup)
			{
				assignee_identityRef.Set(null);
				assignee_groupID = (new_assignee as AssignmentGroup).id;
			}
			GetComponent<KPrefabID>().AddTag(GameTags.Assigned);
			assignee = new_assignee;
			if (slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity || new_assignee is MinionAssignablesProxy))
			{
				KMonoBehaviour kMonoBehaviour = new_assignee as KMonoBehaviour;
				Ownables component = kMonoBehaviour.GetComponent<Ownables>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.GetSlot(slot)?.Assign(this);
				}
				Equipment component2 = kMonoBehaviour.GetComponent<Equipment>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					component2.GetSlot(slot)?.Assign(this);
				}
			}
			if (this.OnAssign != null)
			{
				this.OnAssign(new_assignee);
			}
			Trigger(684616645, new_assignee);
		}
	}

	public virtual void Unassign()
	{
		if (assignee != null)
		{
			GetComponent<KPrefabID>().RemoveTag(GameTags.Assigned);
			if (slot != null)
			{
				Assignables assignables = null;
				if (assignee is KMonoBehaviour)
				{
					if (assignee is MinionAssignablesProxy)
					{
						assignables = (assignee as KMonoBehaviour).GetComponent<Ownables>();
					}
					else if (assignee is MinionIdentity || assignee is StoredMinionIdentity)
					{
						assignables = (assignee as KMonoBehaviour).GetComponent<IAssignableIdentity>().GetSoleOwner();
					}
					assignables.GetSlot(slot)?.Unassign(true);
				}
			}
			assignee = null;
			if (canBePublic)
			{
				Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
			}
			assignee_identityRef.Set(null);
			assignee_groupID = "";
			if (this.OnAssign != null)
			{
				this.OnAssign(null);
			}
			Trigger(684616645, null);
		}
	}

	public void SetCanBeAssigned(bool state)
	{
		canBeAssigned = state;
	}

	public void AddAssignPrecondition(Func<MinionIdentity, bool> precondition)
	{
		assignmentPreconditions.Add(precondition);
	}

	public void AddAutoassignPrecondition(Func<MinionIdentity, bool> precondition)
	{
		autoassignmentPreconditions.Add(precondition);
	}

	public int GetNavigationCost(Navigator navigator)
	{
		int num = -1;
		int cell = Grid.PosToCell(this);
		IApproachable component = GetComponent<IApproachable>();
		CellOffset[] array = (component != null) ? component.GetOffsets() : new CellOffset[1]
		{
			default(CellOffset)
		};
		CellOffset[] array2 = array;
		foreach (CellOffset offset in array2)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			int navigationCost = navigator.GetNavigationCost(cell2);
			if (navigationCost != -1 && (num == -1 || navigationCost < num))
			{
				num = navigationCost;
			}
		}
		return num;
	}
}
