using Klei.AI;
using STRINGS;

public class Hauler : RoleConfig
{
	public const string ID = "Hauler";

	public Hauler()
	{
		base.id = "Hauler";
		base.name = DUPLICANTS.ROLES.HAULER.NAME;
		base.description = DUPLICANTS.ROLES.HAULER.DESCRIPTION;
		base.roleGroup = "Hauling";
		base.hat = Game.Instance.roleManager.GetHat("Hauler");
		experienceRequired = 50f;
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Athletics
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseStrengthGofer,
			RoleManager.rolePerks.IncreaseCarryAmountSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Haul
		};
	}
}
