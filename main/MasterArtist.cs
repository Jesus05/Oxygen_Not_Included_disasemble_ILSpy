using Klei.AI;
using STRINGS;

public class MasterArtist : RoleConfig
{
	public static string ID = "MasterArtist";

	public MasterArtist()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.MASTER_ARTIST.NAME;
		base.description = DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION;
		base.roleGroup = "Art";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Art
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.CanArt,
			RoleManager.rolePerks.CanArtGreat,
			RoleManager.rolePerks.IncreaseArtLarge
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Art,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_Artist
		};
	}
}
