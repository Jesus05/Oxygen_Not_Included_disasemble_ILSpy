using KSerialization;
using System;
using System.Collections.Generic;
using TUNING;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConsumerManager : KMonoBehaviour, ISaveLoadable
{
	public static ConsumerManager instance;

	[Serialize]
	private List<Tag> undiscoveredConsumableTags = new List<Tag>();

	[Serialize]
	private List<Tag> defaultForbiddenTagsList = new List<Tag>();

	public List<Tag> DefaultForbiddenTagsList => defaultForbiddenTagsList;

	public event Action<Tag> OnDiscover;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		instance = this;
		RefreshDiscovered(null);
		WorldInventory.Instance.OnDiscover += OnWorldInventoryDiscover;
		Game.Instance.Subscribe(-107300940, RefreshDiscovered);
	}

	public bool isDiscovered(Tag id)
	{
		return !undiscoveredConsumableTags.Contains(id);
	}

	private void OnWorldInventoryDiscover(Tag category_tag, Tag tag)
	{
		if (undiscoveredConsumableTags.Contains(tag))
		{
			RefreshDiscovered(null);
		}
	}

	public void RefreshDiscovered(object data = null)
	{
		foreach (EdiblesManager.FoodInfo item in FOOD.FOOD_TYPES_LIST)
		{
			if (!ShouldBeDiscovered(item.Id.ToTag()) && !undiscoveredConsumableTags.Contains(item.Id.ToTag()))
			{
				undiscoveredConsumableTags.Add(item.Id.ToTag());
				if (this.OnDiscover != null)
				{
					this.OnDiscover("UndiscoveredSomething".ToTag());
				}
			}
			else if (undiscoveredConsumableTags.Contains(item.Id.ToTag()) && ShouldBeDiscovered(item.Id.ToTag()))
			{
				undiscoveredConsumableTags.Remove(item.Id.ToTag());
				if (this.OnDiscover != null)
				{
					this.OnDiscover(item.Id.ToTag());
				}
				if (!WorldInventory.Instance.IsDiscovered(item.Id.ToTag()))
				{
					if (item.CaloriesPerUnit == 0f)
					{
						WorldInventory.Instance.Discover(item.Id.ToTag(), GameTags.CookingIngredient);
					}
					else
					{
						WorldInventory.Instance.Discover(item.Id.ToTag(), GameTags.Edible);
					}
				}
			}
		}
	}

	private bool ShouldBeDiscovered(Tag food_id)
	{
		if (WorldInventory.Instance.IsDiscovered(food_id))
		{
			return true;
		}
		foreach (Recipe recipe in RecipeManager.Get().recipes)
		{
			if (recipe.Result == food_id)
			{
				string[] fabricators = recipe.fabricators;
				foreach (string id in fabricators)
				{
					if (Db.Get().TechItems.IsTechItemComplete(id))
					{
						return true;
					}
				}
			}
		}
		foreach (Crop item in Components.Crops.Items)
		{
			if (Grid.IsVisible(Grid.PosToCell(item.gameObject)) && item.cropId == food_id.Name)
			{
				return true;
			}
		}
		return false;
	}
}
