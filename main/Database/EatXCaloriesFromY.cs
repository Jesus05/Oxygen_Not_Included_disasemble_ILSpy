using KSerialization;
using STRINGS;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class EatXCaloriesFromY : ColonyAchievementRequirement
	{
		private int numCalories;

		private List<string> fromFoodType = new List<string>();

		public EatXCaloriesFromY(int numCalories, List<string> fromFoodType)
		{
			this.numCalories = numCalories;
			this.fromFoodType = fromFoodType;
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloiresConsumedByFood(fromFoodType) / 1000f > (float)numCalories;
		}

		public override void Deserialize(IReader reader)
		{
			numCalories = reader.ReadInt32();
			int num = reader.ReadInt32();
			fromFoodType = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				string item = reader.ReadKleiString();
				fromFoodType.Add(item);
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numCalories);
			writer.Write(fromFoodType.Count);
			for (int i = 0; i < fromFoodType.Count; i++)
			{
				writer.WriteKleiString(fromFoodType[i]);
			}
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIES_FROM_MEAT, GameUtil.GetFormattedCalories((!complete) ? (RationTracker.Get().GetCaloiresConsumedByFood(fromFoodType) / 1000f) : ((float)numCalories), GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories((float)numCalories, GameUtil.TimeSlice.None, true));
		}
	}
}
