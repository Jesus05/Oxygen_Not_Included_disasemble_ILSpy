using STRINGS;
using System.IO;

namespace Database
{
	public class NumberOfDupes : ColonyAchievementRequirement
	{
		private int numDupes = 0;

		public NumberOfDupes(int num)
		{
			numDupes = num;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS, numDupes);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS_DESCRIPTION, numDupes);
		}

		public override bool Success()
		{
			return Components.LiveMinionIdentities.Items.Count >= numDupes;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numDupes);
		}

		public override void Deserialize(IReader reader)
		{
			numDupes = reader.ReadInt32();
		}
	}
}
