using Klei.AI;
using STRINGS;

public class OilTechnician : RoleConfig
{
	public const string ID = "OilTechnician";

	public OilTechnician()
	{
		base.id = "OilTechnician";
		base.name = DUPLICANTS.ROLES.OIL_TECHNICIAN.NAME;
		base.description = DUPLICANTS.ROLES.OIL_TECHNICIAN.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat("OilTechnician");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Machinery
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.ExosuitExpertise
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MachineTechnician
		};
	}
}
