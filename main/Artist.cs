using Klei.AI;
using STRINGS;

public class Artist : RoleConfig
{
	public static string ID = "Artist";

	public Artist()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.ARTIST.NAME;
		base.description = DUPLICANTS.ROLES.ARTIST.DESCRIPTION;
		base.roleGroup = "Art";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Art
		};
		base.perks = new RolePerk[2]
		{
			RoleManager.rolePerks.CanArt,
			RoleManager.rolePerks.IncreaseArtMedium
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[2]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Art,
			Game.Instance.roleManager.roleAssignmentRequirements.HasExperience_JuniorArtist
		};
	}
}
