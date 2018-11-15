using Klei.AI;
using STRINGS;

public class Plumber : RoleConfig
{
	public static string ID = "Plumber";

	public Plumber()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.PLUMBER.NAME;
		base.description = DUPLICANTS.ROLES.PLUMBER.DESCRIPTION;
		base.roleGroup = "Basekeeping";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Athletics
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseStrengthPlumber,
			RoleManager.rolePerks.CanDoPlumbing
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Basekeep,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Handyman
		};
	}
}
