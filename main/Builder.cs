using Klei.AI;
using STRINGS;

public class Builder : RoleConfig
{
	public static string ID = "Builder";

	public Builder()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.BUILDER.NAME;
		base.description = DUPLICANTS.ROLES.BUILDER.DESCRIPTION;
		base.roleGroup = "Building";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Construction
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.IncreaseConstructionMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Build,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorBuilder
		};
	}
}
