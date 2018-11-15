using Klei.AI;
using STRINGS;

public class Rancher : RoleConfig
{
	public const string ID = "Rancher";

	public Rancher()
	{
		base.id = "Rancher";
		base.name = DUPLICANTS.ROLES.RANCHER.NAME;
		base.description = DUPLICANTS.ROLES.RANCHER.DESCRIPTION;
		base.roleGroup = "Ranching";
		base.hat = Game.Instance.roleManager.GetHat("Rancher");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Ranching
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.CanWrangleCreatures,
			RoleManager.rolePerks.CanUseRanchStation,
			RoleManager.rolePerks.IncreaseRanchingSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Ranching,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorFarmer
		};
	}
}
