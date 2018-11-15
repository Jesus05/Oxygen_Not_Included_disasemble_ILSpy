using Klei.AI;
using STRINGS;

public class SeniorResearcher : RoleConfig
{
	public static string ID = "SeniorResearcher";

	public SeniorResearcher()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION;
		base.roleGroup = "Research";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Learning
		};
		base.perks = new RolePerk[4]
		{
			RoleManager.rolePerks.IncreaseLearningLarge,
			RoleManager.rolePerks.AllowInterstellarResearch,
			RoleManager.rolePerks.CanStudyWorldObjects,
			RoleManager.rolePerks.AllowAdvancedResearch
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Research,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Researcher
		};
	}
}
