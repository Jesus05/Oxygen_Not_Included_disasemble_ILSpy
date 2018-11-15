using Klei;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelector : KScreen
{
	public delegate void SelectMaterialActions();

	public Element CurrentSelectedElement;

	public Dictionary<Element, KToggle> ElementToggles = new Dictionary<Element, KToggle>();

	public Dictionary<Recipe, Element> previouslySelectedElements = new Dictionary<Recipe, Element>();

	public SelectMaterialActions selectMaterialActions;

	public SelectMaterialActions deselectMaterialActions;

	private ToggleGroup toggleGroup;

	public GameObject TogglePrefab;

	public GameObject LayoutContainer;

	public GameObject Scrollbar;

	public GameObject Headerbar;

	public GameObject BadBG;

	public LocText NoMaterialDiscovered;

	public GameObject MaterialDescriptionPane;

	public LocText MaterialDescriptionText;

	public DescriptorPanel MaterialEffectsPane;

	public GameObject DescriptorsPanel;

	private KToggle selectedToggle;

	private Recipe.Ingredient activeIngredient;

	private Recipe activeRecipe;

	private float activeMass;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		toggleGroup = GetComponent<ToggleGroup>();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public void ClearMaterialToggles()
	{
		CurrentSelectedElement = null;
		NoMaterialDiscovered.gameObject.SetActive(false);
		foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
		{
			elementToggle.Value.gameObject.SetActive(false);
			Util.KDestroyGameObject(elementToggle.Value.gameObject);
		}
		ElementToggles.Clear();
	}

	public void ConfigureScreen(Recipe.Ingredient ingredient, Recipe recipe)
	{
		ClearMaterialToggles();
		activeIngredient = ingredient;
		activeRecipe = recipe;
		activeMass = ingredient.amount;
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid && (element.tag == ingredient.tag || element.HasTag(ingredient.tag)))
			{
				list.Add(element);
			}
		}
		foreach (Element item in list)
		{
			if (!ElementToggles.ContainsKey(item))
			{
				GameObject gameObject = Util.KInstantiate(TogglePrefab, LayoutContainer, "MaterialSelection_" + item.name);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(true);
				KToggle component = gameObject.GetComponent<KToggle>();
				ElementToggles.Add(item, component);
				component.group = toggleGroup;
				ToolTip component2 = gameObject.gameObject.GetComponent<ToolTip>();
				component2.toolTip = item.name;
			}
		}
		RefreshToggleContents();
	}

	private void SetToggleBGImage(KToggle toggle, Element elem)
	{
		if ((Object)toggle == (Object)selectedToggle)
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimUIMaterial;
			toggle.GetComponent<ImageToggleState>().SetActive();
		}
		else if (WorldInventory.Instance.GetAmount(elem.tag) >= activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimUIMaterial;
			toggle.GetComponentsInChildren<Image>()[1].color = Color.white;
			toggle.GetComponent<ImageToggleState>().SetInactive();
		}
		else
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimMaterialUIDesaturated;
			toggle.GetComponentsInChildren<Image>()[1].color = new Color(1f, 1f, 1f, 0.6f);
			if (!AllowInsufficientMaterialBuild())
			{
				toggle.GetComponent<ImageToggleState>().SetDisabled();
			}
		}
	}

	public void OnSelectMaterial(Element elem, Recipe recipe)
	{
		KToggle x = ElementToggles[elem];
		if ((Object)x != (Object)selectedToggle)
		{
			selectedToggle = x;
			if (recipe != null)
			{
				previouslySelectedElements[recipe] = elem;
			}
			CurrentSelectedElement = elem;
			if (selectMaterialActions != null)
			{
				selectMaterialActions();
			}
			UpdateHeader();
			SetDescription(elem);
			SetEffects(elem);
			if (!MaterialDescriptionPane.gameObject.activeSelf && !MaterialEffectsPane.gameObject.activeSelf)
			{
				DescriptorsPanel.SetActive(false);
			}
			else
			{
				DescriptorsPanel.SetActive(true);
			}
		}
		RefreshToggleContents();
	}

	public void RefreshToggleContents()
	{
		foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
		{
			KToggle value = elementToggle.Value;
			Element elem = elementToggle.Key;
			GameObject gameObject = value.gameObject;
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>();
			LocText locText = componentsInChildren[0];
			LocText locText2 = componentsInChildren[1];
			Image image = gameObject.GetComponentsInChildren<Image>()[1];
			locText2.text = Util.FormatWholeNumber(WorldInventory.Instance.GetAmount(elem.tag));
			locText.text = Util.FormatWholeNumber(activeMass);
			image.sprite = Def.GetUISpriteFromMultiObjectAnim(elementToggle.Key.substance.anim, "ui", false);
			gameObject.SetActive(WorldInventory.Instance.IsDiscovered(elem.tag) || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive);
			SetToggleBGImage(elementToggle.Value, elementToggle.Key);
			value.soundPlayer.AcceptClickCondition = (() => IsEnoughMass(elem.tag));
			value.ClearOnClick();
			if (IsEnoughMass(elem.tag))
			{
				value.onClick += delegate
				{
					OnSelectMaterial(elem, activeRecipe);
				};
			}
		}
		SortElementToggles();
		UpdateMaterialTooltips();
		UpdateHeader();
	}

	private bool IsEnoughMass(Tag t)
	{
		return WorldInventory.Instance.GetAmount(t) >= activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || AllowInsufficientMaterialBuild();
	}

	public bool AutoSelectAvailableMaterial()
	{
		if (activeRecipe == null || ElementToggles.Count == 0)
		{
			return false;
		}
		previouslySelectedElements.TryGetValue(activeRecipe, out Element value);
		if (value != null)
		{
			ElementToggles.TryGetValue(value, out KToggle value2);
			if ((Object)value2 != (Object)null && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || WorldInventory.Instance.GetAmount(value.tag) >= activeMass))
			{
				OnSelectMaterial(value, activeRecipe);
				return true;
			}
		}
		float num = -1f;
		List<Element> list = new List<Element>();
		foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
		{
			list.Add(elementToggle.Key);
		}
		list.Sort(ElementSorter);
		if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
		{
			OnSelectMaterial(list[0], activeRecipe);
			return true;
		}
		Element element = null;
		foreach (Element item in list)
		{
			float amount = WorldInventory.Instance.GetAmount(item.tag);
			if (amount >= activeMass && amount > num)
			{
				num = amount;
				element = item;
			}
		}
		if (element != null)
		{
			OnSelectMaterial(element, activeRecipe);
			return true;
		}
		return false;
	}

	private void SortElementToggles()
	{
		List<Element> list = new List<Element>();
		foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
		{
			list.Add(elementToggle.Key);
		}
		list.Sort(ElementSorter);
		foreach (Element item in list)
		{
			ElementToggles[item].transform.SetAsLastSibling();
		}
		UpdateScrollBar();
	}

	private void UpdateMaterialTooltips()
	{
		foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
		{
			ToolTip component = elementToggle.Value.gameObject.GetComponent<ToolTip>();
			component.toolTip = GameUtil.GetMaterialTooltips(elementToggle.Key);
		}
	}

	private void UpdateScrollBar()
	{
		int num = 0;
		foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
		{
			if (elementToggle.Value.gameObject.activeSelf)
			{
				num++;
			}
		}
		Scrollbar.SetActive(num > 5);
	}

	private void UpdateHeader()
	{
		if (activeIngredient != null)
		{
			int num = 0;
			foreach (KeyValuePair<Element, KToggle> elementToggle in ElementToggles)
			{
				KToggle value = elementToggle.Value;
				if (value.gameObject.activeSelf)
				{
					num++;
				}
			}
			LocText componentInChildren = Headerbar.GetComponentInChildren<LocText>();
			if (num == 0)
			{
				componentInChildren.text = string.Format(UI.PRODUCTINFO_MISSINGRESOURCES_TITLE, activeIngredient.tag.ProperName(), GameUtil.GetFormattedMass(activeIngredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				string text = string.Format(UI.PRODUCTINFO_MISSINGRESOURCES_DESC, activeIngredient.tag.ProperName());
				NoMaterialDiscovered.text = text;
				NoMaterialDiscovered.gameObject.SetActive(true);
				NoMaterialDiscovered.color = Constants.NEGATIVE_COLOR;
				BadBG.SetActive(true);
				Scrollbar.SetActive(false);
				LayoutContainer.SetActive(false);
			}
			else
			{
				componentInChildren.text = string.Format(UI.PRODUCTINFO_SELECTMATERIAL, activeIngredient.tag.ProperName());
				NoMaterialDiscovered.gameObject.SetActive(false);
				BadBG.SetActive(false);
				LayoutContainer.SetActive(true);
				UpdateScrollBar();
			}
		}
	}

	public void ToggleShowDescriptorsPanel(bool show)
	{
		DescriptorsPanel.gameObject.SetActive(show);
	}

	private void SetDescription(Element element)
	{
		StringEntry result = null;
		if (Strings.TryGet(new StringKey("STRINGS.ELEMENTS." + element.tag.ToString().ToUpper() + ".BUILD_DESC"), out result))
		{
			MaterialDescriptionText.text = result.ToString();
			MaterialDescriptionPane.SetActive(true);
		}
		else
		{
			MaterialDescriptionPane.SetActive(false);
		}
	}

	private void SetEffects(Element element)
	{
		List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
		if (materialDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.EFFECTS_HEADER, ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.EFFECTS_HEADER, Descriptor.DescriptorType.Effect);
			materialDescriptors.Insert(0, item);
			MaterialEffectsPane.gameObject.SetActive(true);
			MaterialEffectsPane.SetDescriptors(materialDescriptors);
		}
		else
		{
			MaterialEffectsPane.gameObject.SetActive(false);
		}
	}

	public static bool AllowInsufficientMaterialBuild()
	{
		return GenericGameSettings.instance.allowInsufficientMaterialBuild;
	}

	private int ElementSorter(Element a, Element b)
	{
		if (a.buildMenuSort != b.buildMenuSort)
		{
			return a.buildMenuSort.CompareTo(b.buildMenuSort);
		}
		return a.idx.CompareTo(b.idx);
	}
}
