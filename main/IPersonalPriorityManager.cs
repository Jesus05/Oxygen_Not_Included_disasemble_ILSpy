public interface IPersonalPriorityManager
{
	int GetAssociatedSkillLevel(ChoreGroup group);

	int GetPersonalPriority(ChoreGroup group, out bool auto_assigned);

	void SetPersonalPriority(ChoreGroup group, int value, bool auto_assigned);

	bool CanRoleManageChoreGroup(ChoreGroup group);

	bool IsChoreGroupDisabled(ChoreGroup group);

	void ResetPersonalPriorities();
}
