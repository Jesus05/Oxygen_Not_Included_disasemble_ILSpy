using System.Collections.Generic;
using TUNING;

public class EdiblesManager : KMonoBehaviour
{
	public class FoodInfo : IConsumableUIItem
	{
		public string Id;

		public string Name;

		public string Description;

		public float CaloriesPerUnit;

		public float PreserveTemperature;

		public float RotTemperature;

		public float StaleTime;

		public float SpoilTime;

		public bool CanRot;

		public int Quality;

		public List<string> Effects;

		public string ConsumableId => Id;

		public string ConsumableName => Name;

		public int MajorOrder => Quality;

		public int MinorOrder => (int)CaloriesPerUnit;

		public bool Display => CaloriesPerUnit != 0f;

		public FoodInfo(string id, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime, bool can_rot)
		{
			Id = id;
			CaloriesPerUnit = caloriesPerUnit;
			Quality = quality;
			PreserveTemperature = preserveTemperatue;
			RotTemperature = rotTemperature;
			StaleTime = spoilTime / 2f;
			SpoilTime = spoilTime;
			CanRot = can_rot;
			Name = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".NAME");
			Description = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".DESC");
			Effects = new List<string>();
			FOOD.FOOD_TYPES_LIST.Add(this);
		}

		public FoodInfo AddEffects(List<string> effects)
		{
			Effects.AddRange(effects);
			return this;
		}
	}

	public FoodInfo GetFoodInfo(string FoodID)
	{
		string b = FoodID.Replace("Compost", "");
		foreach (FoodInfo item in FOOD.FOOD_TYPES_LIST)
		{
			if (item.Id == b)
			{
				return item;
			}
		}
		Debug.LogWarning("No food with ID: " + FoodID);
		return null;
	}
}
