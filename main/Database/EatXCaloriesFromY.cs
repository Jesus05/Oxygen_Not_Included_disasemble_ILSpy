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

		public override string Name()
		{
			string text = "";
			foreach (string item in fromFoodType)
			{
				EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(item);
				text = text + foodInfo.Name + " ";
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_X_CALORIES_FROM_FOOD_TYPE, numCalories, text);
		}

		public override string Description()
		{
			string text = "";
			foreach (string item in fromFoodType)
			{
				EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(item);
				text = text + foodInfo.Name + " ";
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_X_CALORIES_FROM_FOOD_TYPE_DESCRIPTION, numCalories, text);
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
	}
}
