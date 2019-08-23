using STRINGS;
using System.IO;

namespace Database
{
	public class ExploreOilFieldSubZone : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			return Game.Instance.savedInfo.discoveredOilField;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ENTER_OIL_BIOME;
		}
	}
}
