using Klei.AI;
using STRINGS;
using TUNING;

public class SuitExpert : RoleConfig
{
	public const string ID = "SuitExpert";

	public static readonly AttributeModifier AthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)(-TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS), DUPLICANTS.ROLES.SUIT_EXPERT.NAME, false, false, true);

	public SuitExpert()
	{
		base.id = "SuitExpert";
		base.name = DUPLICANTS.ROLES.SUIT_EXPERT.NAME;
		base.description = DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION;
		base.roleGroup = "Hauling";
		base.hat = Game.Instance.roleManager.GetHat("SuitExpert");
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Athletics
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.ExosuitExpertise,
			RoleManager.rolePerks.IncreaseAthleticsMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Haul,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_MaterialsManager
		};
	}
}
