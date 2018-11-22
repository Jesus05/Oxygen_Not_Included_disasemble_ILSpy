using System.Collections.Generic;
using UnityEngine;

public class ResourceRemainingDisplayScreen : KScreen
{
	public static ResourceRemainingDisplayScreen instance;

	public GameObject dispayPrefab;

	public LocText label;

	private Recipe currentRecipe;

	private List<Element> selected_elements = new List<Element>();

	private int numberOfPendingConstructions = 0;

	private int displayedConstructionCostMultiplier = 0;

	private RectTransform rect;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Activate();
		instance = this;
		dispayPrefab.SetActive(false);
	}

	public void ActivateDisplay(GameObject target)
	{
		numberOfPendingConstructions = 0;
		dispayPrefab.SetActive(true);
	}

	public void DeactivateDisplay()
	{
		dispayPrefab.SetActive(false);
	}

	public void SetResources(IList<Element> _selected_elements, Recipe recipe)
	{
		selected_elements.Clear();
		foreach (Element _selected_element in _selected_elements)
		{
			selected_elements.Add(_selected_element);
		}
		currentRecipe = recipe;
	}

	public void SetNumberOfPendingConstructions(int number)
	{
		numberOfPendingConstructions = number;
	}

	public void Update()
	{
		if (dispayPrefab.activeSelf)
		{
			if ((Object)base.canvas != (Object)null)
			{
				if ((Object)rect == (Object)null)
				{
					rect = GetComponent<RectTransform>();
				}
				rect.anchoredPosition = WorldToScreen(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
			}
			if (displayedConstructionCostMultiplier == numberOfPendingConstructions)
			{
				label.text = "";
			}
			else
			{
				displayedConstructionCostMultiplier = numberOfPendingConstructions;
			}
		}
	}

	public string GetString()
	{
		string text = "";
		if (selected_elements != null && currentRecipe != null)
		{
			for (int i = 0; i < currentRecipe.Ingredients.Count; i++)
			{
				Element element = selected_elements[i];
				Tag tag = GameTagExtensions.Create(element.id);
				float num = currentRecipe.Ingredients[i].amount * (float)numberOfPendingConstructions;
				float num2 = WorldInventory.Instance.GetTotalAmount(tag) - WorldInventory.Instance.GetAmount(tag);
				float num3 = WorldInventory.Instance.GetTotalAmount(tag) - (num2 + num);
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				string text2 = text;
				text = text2 + tag.ProperName() + ": " + GameUtil.GetFormattedMass(num3, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") + " / " + GameUtil.GetFormattedMass(currentRecipe.Ingredients[i].amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				if (i < selected_elements.Count - 1)
				{
					text += "\n";
				}
			}
		}
		return text;
	}
}
