using Klei;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelector : KScreen
{
	public delegate void SelectMaterialActions();

	public Tag CurrentSelectedElement;

	public Dictionary<Tag, KToggle> ElementToggles = new Dictionary<Tag, KToggle>();

	public int selectorIndex = 0;

	public SelectMaterialActions selectMaterialActions;

	public SelectMaterialActions deselectMaterialActions;

	private ToggleGroup toggleGroup;

	public GameObject TogglePrefab;

	public GameObject LayoutContainer;

	public KScrollRect ScrollRect;

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
		foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
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
		List<Tag> list = new List<Tag>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid && (element.tag == ingredient.tag || element.HasTag(ingredient.tag)))
			{
				list.Add(element.tag);
			}
		}
		foreach (Tag materialBuildingElement in GameTags.MaterialBuildingElements)
		{
			if (materialBuildingElement == ingredient.tag)
			{
				foreach (GameObject item in Assets.GetPrefabsWithTag(materialBuildingElement))
				{
					KPrefabID component = item.GetComponent<KPrefabID>();
					if ((Object)component != (Object)null && !list.Contains(component.PrefabTag))
					{
						list.Add(component.PrefabTag);
					}
				}
			}
		}
		foreach (Tag item2 in list)
		{
			if (!ElementToggles.ContainsKey(item2))
			{
				GameObject gameObject = Util.KInstantiate(TogglePrefab, LayoutContainer, "MaterialSelection_" + item2.ProperName());
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(true);
				KToggle component2 = gameObject.GetComponent<KToggle>();
				ElementToggles.Add(item2, component2);
				component2.group = toggleGroup;
				ToolTip component3 = gameObject.gameObject.GetComponent<ToolTip>();
				component3.toolTip = item2.ProperName();
			}
		}
		RefreshToggleContents();
	}

	private void SetToggleBGImage(KToggle toggle, Tag elem)
	{
		if ((Object)toggle == (Object)selectedToggle)
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimUIMaterial;
			toggle.GetComponent<ImageToggleState>().SetActive();
		}
		else if (WorldInventory.Instance.GetAmount(elem) >= activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
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

	public void OnSelectMaterial(Tag elem, Recipe recipe, bool focusScrollRect = false)
	{
		KToggle x = ElementToggles[elem];
		if ((Object)x != (Object)selectedToggle)
		{
			selectedToggle = x;
			if (recipe != null)
			{
				SaveGame.Instance.materialSelectorSerializer.SetSelectedElement(selectorIndex, recipe.Result, elem);
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
		if (focusScrollRect && ElementToggles.Count > 1)
		{
			List<Tag> list = new List<Tag>();
			foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
			{
				list.Add(elementToggle.Key);
			}
			list.Sort(ElementSorter);
			int num = list.IndexOf(elem);
			float x2 = (float)num / (float)(list.Count - 1);
			ScrollRect.normalizedPosition = new Vector2(x2, 0f);
		}
		RefreshToggleContents();
	}

	public void RefreshToggleContents()
	{
		foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
		{
			KToggle value = elementToggle.Value;
			Tag elem = elementToggle.Key;
			GameObject gameObject = value.gameObject;
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>();
			LocText locText = componentsInChildren[0];
			LocText locText2 = componentsInChildren[1];
			Image image = gameObject.GetComponentsInChildren<Image>()[1];
			locText2.text = Util.FormatWholeNumber(WorldInventory.Instance.GetAmount(elem));
			locText.text = Util.FormatWholeNumber(activeMass);
			GameObject gameObject2 = Assets.TryGetPrefab(elementToggle.Key);
			if ((Object)gameObject2 != (Object)null)
			{
				KBatchedAnimController component = gameObject2.GetComponent<KBatchedAnimController>();
				image.sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false);
			}
			gameObject.SetActive(WorldInventory.Instance.IsDiscovered(elem) || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive);
			SetToggleBGImage(elementToggle.Value, elementToggle.Key);
			value.soundPlayer.AcceptClickCondition = (() => IsEnoughMass(elem));
			value.ClearOnClick();
			if (IsEnoughMass(elem))
			{
				value.onClick += delegate
				{
					OnSelectMaterial(elem, activeRecipe, false);
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
		if (activeRecipe != null && ElementToggles.Count != 0)
		{
			Tag previousElement = SaveGame.Instance.materialSelectorSerializer.GetPreviousElement(selectorIndex, activeRecipe.Result);
			if (previousElement != (Tag)null)
			{
				ElementToggles.TryGetValue(previousElement, out KToggle value);
				if ((Object)value != (Object)null && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || WorldInventory.Instance.GetAmount(previousElement) >= activeMass))
				{
					OnSelectMaterial(previousElement, activeRecipe, true);
					return true;
				}
			}
			float num = -1f;
			List<Tag> list = new List<Tag>();
			foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
			{
				list.Add(elementToggle.Key);
			}
			list.Sort(ElementSorter);
			if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
			{
				Tag tag = null;
				foreach (Tag item in list)
				{
					float amount = WorldInventory.Instance.GetAmount(item);
					if (amount >= activeMass && amount > num)
					{
						num = amount;
						tag = item;
					}
				}
				if (!(tag != (Tag)null))
				{
					return false;
				}
				OnSelectMaterial(tag, activeRecipe, true);
				return true;
			}
			OnSelectMaterial(list[0], activeRecipe, true);
			return true;
		}
		return false;
	}

	private void SortElementToggles()
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
		{
			list.Add(elementToggle.Key);
		}
		list.Sort(ElementSorter);
		foreach (Tag item in list)
		{
			ElementToggles[item].transform.SetAsLastSibling();
		}
		UpdateScrollBar();
	}

	private void UpdateMaterialTooltips()
	{
		foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
		{
			ToolTip component = elementToggle.Value.gameObject.GetComponent<ToolTip>();
			if ((Object)component != (Object)null)
			{
				component.toolTip = GameUtil.GetMaterialTooltips(elementToggle.Key);
			}
		}
	}

	private void UpdateScrollBar()
	{
		int num = 0;
		foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
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
			foreach (KeyValuePair<Tag, KToggle> elementToggle in ElementToggles)
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

	private void SetDescription(Tag element)
	{
		StringEntry result = null;
		if (Strings.TryGet(new StringKey("STRINGS.ELEMENTS." + element.ToString().ToUpper() + ".BUILD_DESC"), out result))
		{
			MaterialDescriptionText.text = result.ToString();
			MaterialDescriptionPane.SetActive(true);
		}
		else
		{
			MaterialDescriptionPane.SetActive(false);
		}
	}

	private void SetEffects(Tag element)
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

	private int ElementSorter(Tag at, Tag bt)
	{
		GameObject gameObject = Assets.TryGetPrefab(at);
		IHasSortOrder hasSortOrder = (!((Object)gameObject != (Object)null)) ? null : gameObject.GetComponent<IHasSortOrder>();
		GameObject gameObject2 = Assets.TryGetPrefab(bt);
		IHasSortOrder hasSortOrder2 = (!((Object)gameObject2 != (Object)null)) ? null : gameObject2.GetComponent<IHasSortOrder>();
		if (hasSortOrder != null && hasSortOrder2 != null)
		{
			return hasSortOrder.sortOrder.CompareTo(hasSortOrder2.sortOrder);
		}
		return 0;
	}
}
