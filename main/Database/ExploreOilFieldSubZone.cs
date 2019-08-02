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
	}
}
