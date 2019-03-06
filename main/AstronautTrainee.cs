using Klei.AI;
using STRINGS;

public class AstronautTrainee : RoleConfig
{
	public static string ID = "AstronautTrainee";

	public AstronautTrainee()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME;
		base.description = DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Machinery
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.CanTrainToBeAstronaut
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[3]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_SeniorResearcher,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_SuitExpert,
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Operate
		};
	}
}
