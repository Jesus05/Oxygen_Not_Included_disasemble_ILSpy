using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectedFoodDiagram : MonoBehaviour
{
	private class FoodBit
	{
		public string name;

		public float rations;

		public int disease;

		public float DiseasePerRation => (float)disease / rations;

		public override string ToString()
		{
			return $"{name}: {DiseasePerRation:0.##} (x{rations:0.##} = {disease})";
		}
	}

	public LocText minText;

	public LocText maxText;

	public LocText avgText;

	public LocText medianText;

	private void Update()
	{
		List<FoodBit> list = new List<FoodBit>();
		if ((Object)WorldInventory.Instance != (Object)null)
		{
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(GameTags.Edible);
			if (pickupables == null)
			{
				return;
			}
			foreach (Pickupable item in pickupables)
			{
				Edible component = item.GetComponent<Edible>();
				if ((Object)component != (Object)null)
				{
					PrimaryElement component2 = component.GetComponent<PrimaryElement>();
					list.Add(new FoodBit
					{
						name = component.name,
						rations = component.Calories / 1000f / 1000f,
						disease = component2.DiseaseCount
					});
				}
			}
		}
		if (list.Count != 0)
		{
			list.Sort((FoodBit a, FoodBit b) => a.DiseasePerRation.CompareTo(b.DiseasePerRation));
			FoodBit foodBit = list[0];
			minText.text = "Min: " + foodBit.ToString();
			FoodBit foodBit2 = list[list.Count - 1];
			maxText.text = "Max: " + foodBit2.ToString();
			FoodBit foodBit3 = list[list.Count / 2];
			medianText.text = "Median: " + foodBit3.ToString();
			float num = (from b in list
			select b.rations).Sum();
			int num2 = (from b in list
			select b.disease).Sum();
			avgText.text = "Average: " + ((float)num2 / num).ToString();
		}
	}
}
