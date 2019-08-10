using System.IO;

namespace Database
{
	public class BlockedCometWithBunkerDoor : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			return Game.Instance.savedInfo.blockedCometWithBunkerDoor;
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override void Deserialize(IReader reader)
		{
		}
	}
}
