using Klei.AI;
using STRINGS;

public class JuniorCook : RoleConfig
{
	public static string ID = "JuniorCook";

	public JuniorCook()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_COOK.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION;
		base.roleGroup = "Cooking";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Cooking
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseCookingSmall,
			RoleManager.rolePerks.CanElectricGrill
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Cook
		};
	}
}
