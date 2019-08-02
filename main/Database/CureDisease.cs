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
	}
}
