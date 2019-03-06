using Klei.AI;
using STRINGS;

public class Handyman : RoleConfig
{
	public static string ID = "Handyman";

	public Handyman()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.HANDYMAN.NAME;
		base.description = DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION;
		base.roleGroup = "Basekeeping";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Athletics
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.IncreaseStrengthGroundskeeper
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Basekeep
		};
	}
}
