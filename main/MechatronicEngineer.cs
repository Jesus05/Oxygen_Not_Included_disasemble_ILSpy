using Klei.AI;
using STRINGS;

public class MechatronicEngineer : RoleConfig
{
	public const string ID = "MechatronicEngineer";

	public MechatronicEngineer()
	{
		base.id = "MechatronicEngineer";
		base.name = DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME;
		base.description = DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat("MechatronicEngineer");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Machinery
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.IncreaseMachineryLarge,
			RoleManager.rolePerks.IncreaseConstructionMechatronics,
			RoleManager.rolePerks.ConveyorBuild
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[3]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MachineTechnician,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MaterialsManager
		};
	}
}
