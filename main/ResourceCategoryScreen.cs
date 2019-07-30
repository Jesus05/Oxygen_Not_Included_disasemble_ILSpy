using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCategoryScreen : KScreen
{
	public static ResourceCategoryScreen Instance;

	public GameObject Prefab_CategoryBar;

	public Transform CategoryContainer;

	public Dictionary<Tag, ResourceCategoryHeader> DisplayedCategories = new Dictionary<Tag, ResourceCategoryHeader>();

	private Tag[] DisplayedCategoryKeys;

	private int categoryUpdatePacer;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		CreateTagSetHeaders(GameTags.MaterialCategories, GameUtil.MeasureUnit.mass);
		CreateTagSetHeaders(GameTags.CalorieCategories, GameUtil.MeasureUnit.kcal);
		CreateTagSetHeaders(GameTags.UnitCategories, GameUtil.MeasureUnit.quantity);
		if (!DisplayedCategories.ContainsKey(GameTags.Miscellaneous))
		{
			ResourceCategoryHeader value = NewCategoryHeader(GameTags.Miscellaneous, GameUtil.MeasureUnit.mass);
			DisplayedCategories.Add(GameTags.Miscellaneous, value);
		}
		DisplayedCategoryKeys = DisplayedCategories.Keys.ToArray();
	}

	private void CreateTagSetHeaders(IEnumerable<Tag> set, GameUtil.MeasureUnit measure)
	{
		foreach (Tag item in set)
		{
			ResourceCategoryHeader value = NewCategoryHeader(item, measure);
			DisplayedCategories.Add(item, value);
		}
	}

	private void Update()
	{
		if (!((Object)WorldInventory.Instance == (Object)null))
		{
			for (int i = 0; i < 1; i++)
			{
				Tag tag = DisplayedCategoryKeys[categoryUpdatePacer];
				ResourceCategoryHeader resourceCategoryHeader = DisplayedCategories[tag];
				if (WorldInventory.Instance.IsDiscovered(tag) && !resourceCategoryHeader.gameObject.activeInHierarchy)
				{
					resourceCategoryHeader.gameObject.SetActive(true);
				}
				resourceCategoryHeader.UpdateContents();
				categoryUpdatePacer = (categoryUpdatePacer + 1) % DisplayedCategoryKeys.Length;
			}
			if ((Object)MeterScreen.Instance != (Object)null && !MeterScreen.Instance.StartValuesSet)
			{
				MeterScreen.Instance.InitializeValues();
			}
		}
	}

	private ResourceCategoryHeader NewCategoryHeader(Tag categoryTag, GameUtil.MeasureUnit measure)
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_CategoryBar, CategoryContainer.gameObject, false);
		gameObject.name = "CategoryHeader_" + categoryTag.Name;
		ResourceCategoryHeader component = gameObject.GetComponent<ResourceCategoryHeader>();
		component.SetTag(categoryTag, measure);
		return component;
	}

	public static string QuantityTextForMeasure(float quantity, GameUtil.MeasureUnit measure)
	{
		switch (measure)
		{
		case GameUtil.MeasureUnit.mass:
			return GameUtil.GetFormattedMass(quantity, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		case GameUtil.MeasureUnit.kcal:
			return GameUtil.GetFormattedCalories(quantity, GameUtil.TimeSlice.None, true);
		default:
			return quantity.ToString();
		}
	}
}
