using Klei.AI;
using STRINGS;

public class SeniorFarmer : RoleConfig
{
	public const string ID = "SeniorFarmer";

	public SeniorFarmer()
	{
		base.id = "SeniorFarmer";
		base.name = DUPLICANTS.ROLES.SENIOR_FARMER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION;
		base.roleGroup = "Farming";
		base.hat = Game.Instance.roleManager.GetHat("SeniorFarmer");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Botanist
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseBotanyLarge,
			RoleManager.rolePerks.CanFarmTinker
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Farming,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Farmer
		};
	}
}
