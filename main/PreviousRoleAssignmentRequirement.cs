using STRINGS;

public class PreviousRoleAssignmentRequirement : RoleAssignmentRequirement
{
	public string previousRoleID;

	public PreviousRoleAssignmentRequirement(string previousRoleID)
		: base("HasExperience_" + previousRoleID, UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.HAS_EXPERIENCE.DESCRIPTION, delegate(MinionResume resume)
		{
			if (!(previousRoleID == "NoRole"))
			{
				return resume.HasMasteredRole(previousRoleID);
			}
			return true;
		})
	{
		this.previousRoleID = previousRoleID;
	}

	public override string GetDescription()
	{
		return base.GetDescription().Replace("{0}", Game.Instance.roleManager.GetRole(previousRoleID).name);
	}
}
