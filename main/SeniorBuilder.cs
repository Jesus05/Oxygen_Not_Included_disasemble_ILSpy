using Klei.AI;
using STRINGS;

public class SeniorBuilder : RoleConfig
{
	public static string ID = "SeniorBuilder";

	public SeniorBuilder()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.SENIOR_BUILDER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION;
		base.roleGroup = "Building";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Construction
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.IncreaseConstructionLarge
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Build,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Builder
		};
	}
}
