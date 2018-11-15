using Klei.AI;
using STRINGS;

public class MachineTechnician : RoleConfig
{
	public static string ID = "MachineTechnician";

	public MachineTechnician()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME;
		base.description = DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Machinery
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.IncreaseMachinerySmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate
		};
	}
}
