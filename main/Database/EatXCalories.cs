using STRINGS;
using System.IO;

namespace Database
{
	public class EatXCalories : ColonyAchievementRequirement
	{
		private int numCalories;

		public EatXCalories(int numCalories)
		{
			this.numCalories = numCalories;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_X_CALORIES, numCalories);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_X_CALORIES_DESCRIPTION, numCalories);
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloriesConsumed() / 1000f > (float)numCalories;
		}

		public override void Deserialize(IReader reader)
		{
			numCalories = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numCalories);
		}
	}
}
