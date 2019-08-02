using System.IO;

namespace Database
{
	public class DiscoverNum : ColonyAchievementRequirement
	{
		private int numToDiscover;

		public DiscoverNum(int numToDiscover)
		{
			this.numToDiscover = numToDiscover;
		}

		public override bool Success()
		{
			return WorldInventory.Instance.GetDiscovered().Count >= numToDiscover;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numToDiscover);
		}

		public override void Deserialize(IReader reader)
		{
			numToDiscover = reader.ReadInt32();
		}
	}
}
