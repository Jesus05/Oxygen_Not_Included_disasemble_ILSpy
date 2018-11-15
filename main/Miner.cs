using Klei.AI;
using STRINGS;

public class Miner : RoleConfig
{
	public static string ID = "Miner";

	public Miner()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.MINER.NAME;
		base.description = DUPLICANTS.ROLES.MINER.DESCRIPTION;
		base.roleGroup = "Mining";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Digging
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.IncreaseDigSpeedMedium,
			RoleManager.rolePerks.CanDigVeryFirm,
			RoleManager.rolePerks.CanDigNearlyImpenetrable
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Dig,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorMiner
		};
	}
}
