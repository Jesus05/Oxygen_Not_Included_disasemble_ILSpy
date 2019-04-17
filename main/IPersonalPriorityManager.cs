public interface IPersonalPriorityManager
{
	int GetAssociatedSkillLevel(ChoreGroup group);

	int GetPersonalPriority(ChoreGroup group);

	void SetPersonalPriority(ChoreGroup group, int value);

	bool IsChoreGroupDisabled(ChoreGroup group);

	void ResetPersonalPriorities();
}
