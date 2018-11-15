using Klei.AI;
using STRINGS;

public class JuniorResearcher : RoleConfig
{
	public static string ID = "JuniorResearcher";

	public JuniorResearcher()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION;
		base.roleGroup = "Research";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Learning
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseLearningSmall,
			RoleManager.rolePerks.AllowAdvancedResearch
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Research
		};
	}
}
