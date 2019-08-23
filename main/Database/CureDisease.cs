using STRINGS;
using System.IO;

namespace Database
{
	public class CureDisease : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			return Game.Instance.savedInfo.curedDisease;
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CURED_DISEASE;
		}
	}
}
