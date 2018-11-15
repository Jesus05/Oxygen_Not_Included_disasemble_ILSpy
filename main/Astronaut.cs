using Klei.AI;
using STRINGS;

public class Astronaut : RoleConfig
{
	public static string ID = "Astronaut";

	public Astronaut()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.ASTRONAUT.NAME;
		base.description = DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION;
		base.roleGroup = "Technicals";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Machinery
		};
		base.perks = new RolePerk[1]
		{
			RoleManager.rolePerks.CanUseRockets
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_AstronautTrainee
		};
	}
}
