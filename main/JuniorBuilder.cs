using Klei.AI;
using STRINGS;

public class JuniorBuilder : RoleConfig
{
	public static string ID = "JuniorBuilder";

	public JuniorBuilder()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION;
		base.roleGroup = "Building";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Construction
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.IncreaseConstructionSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Build
		};
	}
}
