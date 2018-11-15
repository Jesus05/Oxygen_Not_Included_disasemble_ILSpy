using Klei.AI;
using STRINGS;

public class SeniorRancher : RoleConfig
{
	public const string ID = "SeniorRancher";

	public SeniorRancher()
	{
		base.id = "SeniorRancher";
		base.name = DUPLICANTS.ROLES.SENIOR_RANCHER.NAME;
		base.description = DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION;
		base.roleGroup = "Ranching";
		base.hat = Game.Instance.roleManager.GetHat("SeniorRancher");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Ranching
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.CanWrangleCreatures,
			RoleManager.rolePerks.CanUseRanchStation,
			RoleManager.rolePerks.IncreaseRanchingMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Ranching,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Rancher
		};
	}
}
