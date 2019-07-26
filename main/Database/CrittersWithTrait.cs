using KSerialization;
using System.IO;

namespace Database
{
	public class CrittersWithTrait : ColonyAchievementRequirement
	{
		private int numCritters;

		private Tag trait;

		private bool hasTrait;

		public CrittersWithTrait(int numCritters, Tag trait, bool hasTrait = true)
		{
			this.numCritters = numCritters;
			this.trait = trait;
			this.hasTrait = hasTrait;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (Capturable item in Components.Capturables.Items)
			{
				if (item.HasTag(trait) == hasTrait)
				{
					num++;
				}
			}
			return num >= numCritters;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numCritters);
			writer.WriteKleiString(trait.ToString());
			writer.Write((byte)(hasTrait ? 1 : 0));
		}

		public override void Deserialize(IReader reader)
		{
			numCritters = reader.ReadInt32();
			trait = new Tag(reader.ReadKleiString());
			hasTrait = (reader.ReadByte() != 0);
		}
	}
}
