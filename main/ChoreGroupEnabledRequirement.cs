using STRINGS;

public class ChoreGroupEnabledRequirement : RoleAssignmentRequirement
{
	public string choreGroupID;

	public ChoreGroupEnabledRequirement(string choreGroupID)
		: base("ChoreGroupEnabled_" + choreGroupID, string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.CHOREGROUP_ENABLED.DESCRIPTION, Db.Get().ChoreGroups.Get(choreGroupID).Name), (MinionResume resume) => resume.GetComponent<ChoreConsumer>().IsPermittedByTraits(Db.Get().ChoreGroups.Get(choreGroupID)))
	{
		this.choreGroupID = choreGroupID;
	}
}
