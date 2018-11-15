using Klei.AI;
using STRINGS;

public class JuniorFarmer : RoleConfig
{
	public const string ID = "JuniorFarmer";

	public JuniorFarmer()
	{
		base.id = "JuniorFarmer";
		base.name = DUPLICANTS.ROLES.JUNIOR_FARMER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION;
		base.roleGroup = "Farming";
		base.hat = Game.Instance.roleManager.GetHat("JuniorFarmer");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Botanist
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.IncreaseBotanySmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Farming
		};
	}
}
