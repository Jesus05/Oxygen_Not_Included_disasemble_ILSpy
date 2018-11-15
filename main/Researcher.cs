using Klei.AI;
using STRINGS;

public class Researcher : RoleConfig
{
	public static string ID = "Researcher";

	public Researcher()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.RESEARCHER.NAME;
		base.description = DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION;
		base.roleGroup = "Research";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Learning
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.IncreaseLearningMedium,
			RoleManager.rolePerks.CanStudyWorldObjects,
			RoleManager.rolePerks.AllowAdvancedResearch
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Research,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorResearcher
		};
	}
}
