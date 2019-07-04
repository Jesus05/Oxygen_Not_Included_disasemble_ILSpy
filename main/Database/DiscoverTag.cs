using KSerialization;
using STRINGS;
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

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.DISCOVER_TAG, tagToDiscover.ProperName());
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.DISCOVER_TAG_DESCRIPTION, tagToDiscover.ProperName());
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
