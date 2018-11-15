using Klei.AI;
using STRINGS;

public class JuniorMiner : RoleConfig
{
	public static string ID = "JuniorMiner";

	public JuniorMiner()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_MINER.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION;
		base.roleGroup = "Mining";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Digging
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseDigSpeedSmall,
			RoleManager.rolePerks.CanDigVeryFirm
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Dig
		};
	}
}
