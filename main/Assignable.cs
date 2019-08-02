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
	private string assignee_groupID = string.Empty;

	public AssignableSlot[] subSlots;

	public bool canBePublic;

	[Serialize]
	private bool canBeAssigned = true;

	private List<Func<MinionAssignablesProxy, bool>> autoassignmentPreconditions = new List<Func<MinionAssignablesProxy, bool>>();

	private List<Func<MinionAssignablesProxy, bool>> assignmentPreconditions = new List<Func<MinionAssignablesProxy, bool>>();

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
		if ((UnityEngine.Object)assignee_identityRef.Get() != (UnityEngine.Object)null)
		{
			return assignee_identityRef.Get().GetComponent<IAssignableIdentity>();
		}
		if (assignee_groupID != string.Empty)
		{
			return Game.Instance.assignmentManager.assignment_groups[assignee_groupID];
		}
		return null;
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
		MinionAssignablesProxy minionAssignablesProxy = identity as MinionAssignablesProxy;
		if ((UnityEngine.Object)minionAssignablesProxy == (UnityEngine.Object)null)
		{
			return true;
		}
		if (!CanAssignTo(minionAssignablesProxy))
		{
			return false;
		}
		foreach (Func<MinionAssignablesProxy, bool> autoassignmentPrecondition in autoassignmentPreconditions)
		{
			if (!autoassignmentPrecondition(minionAssignablesProxy))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanAssignTo(IAssignableIdentity identity)
	{
		MinionAssignablesProxy minionAssignablesProxy = identity as MinionAssignablesProxy;
		if ((UnityEngine.Object)minionAssignablesProxy == (UnityEngine.Object)null)
		{
			return true;
		}
		foreach (Func<MinionAssignablesProxy, bool> assignmentPrecondition in assignmentPreconditions)
		{
			if (!assignmentPrecondition(minionAssignablesProxy))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsAssigned()
	{
		return assignee != null;
	}

	public bool IsAssignedTo(IAssignableIdentity identity)
	{
		Debug.Assert(identity != null, "IsAssignedTo identity is null");
		Ownables soleOwner = identity.GetSoleOwner();
		Debug.Assert((UnityEngine.Object)soleOwner != (UnityEngine.Object)null, "IsAssignedTo identity sole owner is null");
		if (assignee != null)
		{
			foreach (Ownables owner in assignee.GetOwners())
			{
				Debug.Assert(owner, "Assignable owners list contained null");
				if ((UnityEngine.Object)owner.gameObject == (UnityEngine.Object)soleOwner.gameObject)
				{
					return true;
				}
			}
		}
		return false;
	}

	public virtual void Assign(IAssignableIdentity new_assignee)
	{
		if (new_assignee != assignee)
		{
			if (new_assignee is KMonoBehaviour)
			{
				if (!CanAssignTo(new_assignee))
				{
					return;
				}
				assignee_identityRef.Set((KMonoBehaviour)new_assignee);
				assignee_groupID = string.Empty;
			}
			else if (new_assignee is AssignmentGroup)
			{
				assignee_identityRef.Set(null);
				assignee_groupID = ((AssignmentGroup)new_assignee).id;
			}
			GetComponent<KPrefabID>().AddTag(GameTags.Assigned, false);
			assignee = new_assignee;
			if (slot != null && (new_assignee is MinionIdentity || new_assignee is StoredMinionIdentity || new_assignee is MinionAssignablesProxy))
			{
				Ownables soleOwner = new_assignee.GetSoleOwner();
				if ((UnityEngine.Object)soleOwner != (UnityEngine.Object)null)
				{
					soleOwner.GetSlot(slot)?.Assign(this);
				}
				Equipment component = soleOwner.GetComponent<Equipment>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.GetSlot(slot)?.Assign(this);
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
				Ownables soleOwner = assignee.GetSoleOwner();
				if ((bool)soleOwner)
				{
					soleOwner.GetSlot(slot)?.Unassign(true);
					Equipment component = soleOwner.GetComponent<Equipment>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.GetSlot(slot)?.Unassign(true);
					}
				}
			}
			assignee = null;
			if (canBePublic)
			{
				Assign(Game.Instance.assignmentManager.assignment_groups["public"]);
			}
			assignee_identityRef.Set(null);
			assignee_groupID = string.Empty;
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

	public void AddAssignPrecondition(Func<MinionAssignablesProxy, bool> precondition)
	{
		assignmentPreconditions.Add(precondition);
	}

	public void AddAutoassignPrecondition(Func<MinionAssignablesProxy, bool> precondition)
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
