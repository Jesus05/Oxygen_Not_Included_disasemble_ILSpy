using Klei.AI;
using STRINGS;

public class MaterialsManager : RoleConfig
{
	public static string ID = "MaterialsManager";

	public MaterialsManager()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME;
		base.description = DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION;
		base.roleGroup = "Hauling";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Athletics
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.IncreaseStrengthCourier,
			RoleManager.rolePerks.IncreaseCarryAmountMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Haul,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Hauler
		};
	}
}
