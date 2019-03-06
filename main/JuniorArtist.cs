using Klei.AI;
using STRINGS;

public class JuniorArtist : RoleConfig
{
	public static string ID = "JuniorArtist";

	public JuniorArtist()
	{
		base.id = ID;
		base.name = DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME;
		base.description = DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION;
		base.roleGroup = "Art";
		base.hat = Game.Instance.roleManager.GetHat(ID);
		relevantAttributes = new Attribute[1]
		{
			Db.Get().Attributes.Art
		};
		base.perks = new RolePerk[3]
		{
			RoleManager.rolePerks.CanArt,
			RoleManager.rolePerks.CanArtUgly,
			RoleManager.rolePerks.IncreaseArtSmall
		};
	}

	public override void InitRequirements()
	{
		base.requirements = new RoleAssignmentRequirement[1]
		{
			Game.Instance.roleManager.roleAssignmentRequirements.Can_Art
		};
	}
}
