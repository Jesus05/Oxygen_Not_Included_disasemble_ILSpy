using KSerialization;
using System.IO;

namespace Database
{
	public class DiscoverTag : ColonyAchievementRequirement
	{
		private Tag tagToDiscover;

		public DiscoverTag(Tag tagToDiscover)
		{
			this.tagToDiscover = tagToDiscover;
		}

		public override bool Success()
		{
			return WorldInventory.Instance.IsDiscovered(tagToDiscover);
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.WriteKleiString(tagToDiscover.ToString());
		}

		public override void Deserialize(IReader reader)
		{
			tagToDiscover = new Tag(reader.ReadKleiString());
		}
	}
}
