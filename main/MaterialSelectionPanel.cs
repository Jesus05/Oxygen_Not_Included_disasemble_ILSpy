using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelectionPanel : KScreen
{
	public delegate void SelectElement(Element element, float kgAvailable, float recipe_amount);

	public struct SelectedElemInfo
	{
		public Element element;

		public float kgAvailable;
	}

	public Dictionary<KToggle, Element> ElementToggles = new Dictionary<KToggle, Element>();

	private List<MaterialSelector> MaterialSelectors = new List<MaterialSelector>();

	private List<Element> currentSelectedElements = new List<Element>();

	[SerializeField]
	protected PriorityScreen priorityScreenPrefab;

	[SerializeField]
	protected GameObject priorityScreenParent;

	private PriorityScreen priorityScreen;

	public GameObject MaterialSelectorTemplate;

	public GameObject ResearchRequired;

	private Recipe activeRecipe;

	private static Dictionary<Tag, List<Element>> elementsWithTag = new Dictionary<Tag, List<Element>>();

	public Element CurrentSelectedElement => MaterialSelectors[0].CurrentSelectedElement;

	public IList<Element> GetSelectedElementAsList
	{
		get
		{
			currentSelectedElements.Clear();
			foreach (MaterialSelector materialSelector in MaterialSelectors)
			{
				if (materialSelector.gameObject.activeSelf)
				{
					currentSelectedElements.Add(materialSelector.CurrentSelectedElement);
				}
			}
			return currentSelectedElements;
		}
	}

	public PriorityScreen PriorityScreen => priorityScreen;

	public static void ClearStatics()
	{
		elementsWithTag.Clear();
	}

	protected override void OnPrefabInit()
	{
		elementsWithTag.Clear();
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		for (int i = 0; i < 3; i++)
		{
			MaterialSelector item = Util.KInstantiateUI<MaterialSelector>(MaterialSelectorTemplate, base.gameObject, false);
			MaterialSelectors.Add(item);
		}
		MaterialSelectors[0].gameObject.SetActive(true);
		MaterialSelectorTemplate.SetActive(false);
		ResearchRequired.SetActive(false);
		priorityScreen = Util.KInstantiateUI<PriorityScreen>(priorityScreenPrefab.gameObject, priorityScreenParent, false);
		priorityScreen.InstantiateButtons(OnPriorityClicked, true);
		Game.Instance.Subscribe(-107300940, delegate
		{
			RefreshSelectors();
		});
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		activateOnSpawn = true;
	}

	public void AddSelectAction(MaterialSelector.SelectMaterialActions action)
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.selectMaterialActions = (MaterialSelector.SelectMaterialActions)Delegate.Combine(selector.selectMaterialActions, action);
		});
	}

	public void ClearSelectActions()
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.selectMaterialActions = null;
		});
	}

	public void ClearMaterialToggles()
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.ClearMaterialToggles();
		});
	}

	public void ConfigureScreen(Recipe recipe)
	{
		activeRecipe = recipe;
		RefreshSelectors();
	}

	public bool AllSelectorsSelected()
	{
		foreach (MaterialSelector materialSelector in MaterialSelectors)
		{
			if (materialSelector.gameObject.activeInHierarchy && materialSelector.CurrentSelectedElement == null)
			{
				return false;
			}
		}
		return true;
	}

	public void RefreshSelectors()
	{
		if (activeRecipe != null)
		{
			MaterialSelectors.ForEach(delegate(MaterialSelector selector)
			{
				selector.gameObject.SetActive(false);
			});
			TechItem techItem = Db.Get().TechItems.TryGet(activeRecipe.GetBuildingDef().PrefabID);
			if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && techItem != null && !techItem.IsComplete())
			{
				ResearchRequired.SetActive(true);
				LocText[] componentsInChildren = ResearchRequired.GetComponentsInChildren<LocText>();
				componentsInChildren[0].text = UI.PRODUCTINFO_RESEARCHREQUIRED;
				componentsInChildren[1].text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
				componentsInChildren[1].color = Constants.NEGATIVE_COLOR;
				priorityScreen.gameObject.SetActive(false);
			}
			else
			{
				ResearchRequired.SetActive(false);
				for (int i = 0; i < activeRecipe.Ingredients.Count; i++)
				{
					MaterialSelectors[i].gameObject.SetActive(true);
					MaterialSelectors[i].ConfigureScreen(activeRecipe.Ingredients[i], activeRecipe);
				}
				priorityScreen.gameObject.SetActive(true);
				priorityScreen.gameObject.transform.SetAsLastSibling();
			}
		}
	}

	public void UpdateResourceToggleValues()
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			if (selector.gameObject.activeSelf)
			{
				selector.RefreshToggleContents();
			}
		});
	}

	public bool AutoSelectAvailableMaterial()
	{
		bool result = true;
		for (int i = 0; i < MaterialSelectors.Count; i++)
		{
			MaterialSelector materialSelector = MaterialSelectors[i];
			if (!materialSelector.AutoSelectAvailableMaterial())
			{
				result = false;
			}
		}
		return result;
	}

	public bool CanBuild(Recipe recipe)
	{
		foreach (MaterialSelector materialSelector in MaterialSelectors)
		{
			if (materialSelector.gameObject.activeSelf && materialSelector.CurrentSelectedElement == null)
			{
				return false;
			}
		}
		return true;
	}

	public static SelectedElemInfo Filter(Tag materialCategoryTag)
	{
		SelectedElemInfo result = default(SelectedElemInfo);
		result.element = null;
		result.kgAvailable = 0f;
		if ((UnityEngine.Object)WorldInventory.Instance == (UnityEngine.Object)null || ElementLoader.elements == null || ElementLoader.elements.Count == 0)
		{
			return result;
		}
		List<Element> value = null;
		if (!elementsWithTag.TryGetValue(materialCategoryTag, out value))
		{
			value = new List<Element>();
			foreach (Element element in ElementLoader.elements)
			{
				if (element.tag == materialCategoryTag || element.HasTag(materialCategoryTag))
				{
					value.Add(element);
				}
			}
			elementsWithTag[materialCategoryTag] = value;
		}
		foreach (Element item in value)
		{
			float amount = WorldInventory.Instance.GetAmount(item.tag);
			if (amount > result.kgAvailable)
			{
				result.kgAvailable = amount;
				result.element = item;
			}
		}
		return result;
	}

	public void ToggleShowDescriptorPanels(bool show)
	{
		for (int i = 0; i < MaterialSelectors.Count; i++)
		{
			if ((UnityEngine.Object)MaterialSelectors[i] != (UnityEngine.Object)null)
			{
				MaterialSelectors[i].ToggleShowDescriptorsPanel(show);
			}
		}
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		priorityScreen.SetScreenPriority(priority, false);
	}
}
