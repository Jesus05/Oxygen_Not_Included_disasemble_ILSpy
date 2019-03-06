using Klei.AI;
using STRINGS;

public class Farmer : RoleConfig
{
	public const string ID = "Farmer";

	public Farmer()
	{
		base.id = "Farmer";
		base.name = DUPLICANTS.ROLES.FARMER.NAME;
		base.description = DUPLICANTS.ROLES.FARMER.DESCRIPTION;
		base.roleGroup = "Farming";
		base.hat = Game.Instance.roleManager.GetHat("Farmer");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Botanist
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseBotanyMedium,
			RoleManager.rolePerks.CanFarmTinker
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Farming,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorFarmer
		};
	}
}
