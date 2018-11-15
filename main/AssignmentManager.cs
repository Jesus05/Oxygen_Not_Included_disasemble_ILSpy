using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class AssignmentManager : KMonoBehaviour
{
	private List<Assignable> assignables = new List<Assignable>();

	public Dictionary<string, AssignmentGroup> assignment_groups = new Dictionary<string, AssignmentGroup>
	{
		{
			"public",
			new AssignmentGroup("public", new IAssignableIdentity[0], UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.PUBLIC)
		}
	};

	private List<Assignable> PreferredAssignableResults = new List<Assignable>();

	public IEnumerator<Assignable> GetEnumerator()
	{
		return assignables.GetEnumerator();
	}

	public void Add(Assignable assignable)
	{
		assignables.Add(assignable);
	}

	public void Remove(Assignable assignable)
	{
		assignables.Remove(assignable);
	}

	public void AddAssignmentGroup(string id, IAssignableIdentity[] members, string name)
	{
		if (!assignment_groups.ContainsKey(id))
		{
			assignment_groups.Add(id, new AssignmentGroup(id, members, name));
		}
	}

	public void AddToAssignmentGroup(string group_id, IAssignableIdentity member)
	{
		assignment_groups[group_id].AddMember(member);
	}

	public void RemoveFromAssignmentGroup(string group_id, IAssignableIdentity member)
	{
		assignment_groups[group_id].RemoveMember(member);
	}

	public void RemoveFromAllGroups(IAssignableIdentity member)
	{
		foreach (Assignable assignable in assignables)
		{
			if (assignable.assignee == member)
			{
				assignable.Unassign();
			}
		}
		foreach (KeyValuePair<string, AssignmentGroup> assignment_group in assignment_groups)
		{
			if (assignment_group.Value.HasMember(member))
			{
				assignment_group.Value.RemoveMember(member);
			}
		}
	}

	public List<Assignable> GetPreferredAssignables(Assignables owner, AssignableSlot slot)
	{
		PreferredAssignableResults.Clear();
		int num = 2147483647;
		foreach (Assignable assignable in assignables)
		{
			if (assignable.slot == slot && assignable.assignee != null)
			{
				List<Ownables> owners = assignable.assignee.GetOwners();
				if (owners.Count > 0)
				{
					foreach (Ownables item in owners)
					{
						if ((Object)item.gameObject == (Object)owner.gameObject)
						{
							if (assignable.assignee is Room && (assignable.assignee as Room).roomType.priority_building_use)
							{
								PreferredAssignableResults.Clear();
								PreferredAssignableResults.Add(assignable);
								return PreferredAssignableResults;
							}
							if (owners.Count == num)
							{
								PreferredAssignableResults.Add(assignable);
							}
							else if (owners.Count < num)
							{
								num = owners.Count;
								PreferredAssignableResults.Clear();
								PreferredAssignableResults.Add(assignable);
							}
						}
					}
				}
			}
		}
		return PreferredAssignableResults;
	}
}
