using Klei.AI;
using STRINGS;

public class Cook : RoleConfig
{
	public static string ID = "Cook";

	public Cook()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.COOK.NAME;
		base.description = DUPLICANTS.ROLES.COOK.DESCRIPTION;
		base.roleGroup = "Cooking";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Cooking
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseCookingMedium,
			RoleManager.rolePerks.CanElectricGrill
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Cook,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorCook
		};
	}
}
