using KSerialization;
using System.Collections.Generic;

[SerializationConfig(MemberSerialization.OptIn)]
public class RationTracker : KMonoBehaviour, ISaveLoadable
{
	public struct Frame
	{
		public float caloriesProduced;

		public float caloriesConsumed;
	}

	private static RationTracker instance;

	[Serialize]
	public Frame currentFrame = default(Frame);

	[Serialize]
	public Frame previousFrame = default(Frame);

	private static readonly EventSystem.IntraObjectHandler<RationTracker> OnNewDayDelegate = new EventSystem.IntraObjectHandler<RationTracker>(delegate(RationTracker component, object data)
	{
		component.OnNewDay(data);
	});

	public static void DestroyInstance()
	{
		instance = null;
	}

	public static RationTracker Get()
	{
		return instance;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
	}

	protected override void OnSpawn()
	{
		Subscribe(631075836, OnNewDayDelegate);
	}

	private void OnNewDay(object data)
	{
		previousFrame = currentFrame;
		currentFrame = default(Frame);
	}

	public float CountRations(Dictionary<string, float> unitCountByFoodType, bool excludeUnreachable = true)
	{
		float num = 0f;
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(GameTags.Edible);
		if (pickupables != null)
		{
			foreach (Pickupable item in pickupables)
			{
				if (!item.KPrefabID.HasTag(GameTags.StoredPrivate))
				{
					Edible component = item.GetComponent<Edible>();
					num += component.Calories;
					if (unitCountByFoodType != null)
					{
						if (!unitCountByFoodType.ContainsKey(component.FoodID))
						{
							unitCountByFoodType[component.FoodID] = 0f;
						}
						Dictionary<string, float> dictionary;
						string foodID;
						(dictionary = unitCountByFoodType)[foodID = component.FoodID] = dictionary[foodID] + component.Units;
					}
				}
			}
		}
		return num;
	}

	public void RegisterCaloriesProduced(float calories)
	{
		currentFrame.caloriesProduced += calories;
	}

	public void RegisterRationsConsumed(float calories)
	{
		currentFrame.caloriesConsumed += calories;
	}
}
