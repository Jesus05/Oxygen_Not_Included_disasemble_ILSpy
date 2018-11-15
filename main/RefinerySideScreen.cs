using FMOD.Studio;
using FMODUnity;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefinerySideScreen : SideScreenContent
{
	public enum StyleSetting
	{
		GridResult,
		ListResult,
		GridInput,
		ListInput,
		ListInputOutput,
		GridInputOutput
	}

	public DescriptorPanel IngredientsDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	[Header("Recipe List")]
	[SerializeField]
	private GameObject recipeGrid;

	[SerializeField]
	private GameObject recipeButton;

	[SerializeField]
	private GameObject recipeButtonMultiple;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	private KToggle selectedToggle;

	public LayoutElement buttonScrollContainer;

	public RectTransform buttonContentContainer;

	[SerializeField]
	private GameObject elementContainer;

	[SerializeField]
	private KButton buildBtn;

	[SerializeField]
	private KButton infiniteBuildBtn;

	[SerializeField]
	private BuildQueue queue;

	[SerializeField]
	private LocText descriptionLabel;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private LocText noRecipeSelectedLabel;

	private Dictionary<Refinery, int> selectedRecipeFabricatorMap = new Dictionary<Refinery, int>();

	[EventRef]
	public string createOrderSound;

	[SerializeField]
	private RectTransform content;

	private Refinery targetFab;

	private ComplexRecipe selectedRecipe;

	private Dictionary<KToggle, ComplexRecipe> recipeMap;

	private List<KToggle> recipeToggles = new List<KToggle>();

	public override string GetTitle()
	{
		if ((UnityEngine.Object)targetFab == (UnityEngine.Object)null)
		{
			return Strings.Get(titleKey).ToString().Replace("{0}", string.Empty);
		}
		return string.Format(Strings.Get(titleKey), targetFab.GetProperName());
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<Refinery>() != (UnityEngine.Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		Refinery component = target.GetComponent<Refinery>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogError("The object selected doesn't have a Refinery!", null);
		}
		else
		{
			queue.SetFabricator(component);
			queue.AddAvailableMaterialStorage(component.inStorage);
			queue.AddAvailableMaterialStorage(component.buildStorage);
			Initialize(component);
		}
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot, STOP_MODE.ALLOWFADEOUT);
		}
		base.OnShow(show);
	}

	public void Initialize(Refinery target)
	{
		if ((UnityEngine.Object)target == (UnityEngine.Object)null)
		{
			Debug.LogError("Refinery provided was null.", null);
		}
		else
		{
			targetFab = target;
			base.gameObject.SetActive(true);
			ComplexRecipe[] recipes = targetFab.GetRecipes();
			Array.Sort(recipes, (ComplexRecipe a, ComplexRecipe b) => a.sortOrder - b.sortOrder);
			recipeMap = new Dictionary<KToggle, ComplexRecipe>();
			recipeToggles.ForEach(delegate(KToggle rbi)
			{
				UnityEngine.Object.Destroy(rbi.gameObject);
			});
			recipeToggles.Clear();
			GridLayoutGroup component = recipeGrid.GetComponent<GridLayoutGroup>();
			component.constraintCount = ((targetFab.sideScreenStyle == StyleSetting.ListResult || targetFab.sideScreenStyle == StyleSetting.ListInput || targetFab.sideScreenStyle == StyleSetting.ListInputOutput) ? 1 : 3);
			GridLayoutGroup gridLayoutGroup = component;
			float x = (float)((targetFab.sideScreenStyle != StyleSetting.ListResult && targetFab.sideScreenStyle != StyleSetting.ListInput && targetFab.sideScreenStyle != StyleSetting.ListInputOutput) ? 116 : 272);
			Vector2 cellSize = component.cellSize;
			gridLayoutGroup.cellSize = new Vector2(x, cellSize.y);
			int num = 0;
			ComplexRecipe[] array = recipes;
			foreach (ComplexRecipe complexRecipe in array)
			{
				bool flag = true;
				ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
				foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
				{
					if (!WorldInventory.Instance.IsDiscovered(recipeElement.material) && !DebugHandler.InstantBuildMode)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num++;
					Tuple<Sprite, Color> uISprite = Def.GetUISprite(complexRecipe.ingredients[0].material, "ui", false);
					Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(complexRecipe.results[0].material, "ui", false);
					KToggle newToggle;
					if (target.sideScreenStyle == StyleSetting.GridInputOutput || target.sideScreenStyle == StyleSetting.ListInputOutput || target.sideScreenStyle == StyleSetting.ListInputOutput)
					{
						newToggle = Util.KInstantiateUI<KToggle>(recipeButtonMultiple, recipeGrid, false);
						HierarchyReferences component2 = newToggle.GetComponent<HierarchyReferences>();
						ComplexRecipe.RecipeElement[] ingredients2 = complexRecipe.ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
						{
							GameObject gameObject = Util.KInstantiateUI(component2.GetReference("FromIconPrefab").gameObject, component2.GetReference("FromIcons").gameObject, true);
							gameObject.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement2.material, "ui", false).first;
							gameObject.GetComponent<Image>().color = Def.GetUISprite(recipeElement2.material, "ui", false).second;
							gameObject.gameObject.name = recipeElement2.material.Name;
						}
						ComplexRecipe.RecipeElement[] results = complexRecipe.results;
						foreach (ComplexRecipe.RecipeElement recipeElement3 in results)
						{
							GameObject gameObject2 = Util.KInstantiateUI(component2.GetReference("ToIconPrefab").gameObject, component2.GetReference("ToIcons").gameObject, true);
							gameObject2.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement3.material, "ui", false).first;
							gameObject2.GetComponent<Image>().color = Def.GetUISprite(recipeElement3.material, "ui", false).second;
							gameObject2.gameObject.name = recipeElement3.material.Name;
						}
					}
					else
					{
						newToggle = Util.KInstantiateUI<KToggle>(recipeButton, recipeGrid, false);
						if (target.sideScreenStyle == StyleSetting.GridInput || target.sideScreenStyle == StyleSetting.ListInput)
						{
							Image image = newToggle.gameObject.GetComponentsInChildrenOnly<Image>()[0];
							image.sprite = uISprite.first;
							image.color = uISprite.second;
						}
						else
						{
							Image image2 = newToggle.gameObject.GetComponentsInChildrenOnly<Image>()[0];
							image2.sprite = uISprite2.first;
							image2.color = uISprite2.second;
						}
					}
					newToggle.GetComponentInChildren<LocText>().text = string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, complexRecipe.ingredients[0].material.ProperName(), complexRecipe.results[0].material.ProperName());
					newToggle.GetComponent<ToolTip>().SetSimpleTooltip(complexRecipe.description);
					newToggle.onClick += delegate
					{
						ToggleClicked(newToggle);
					};
					newToggle.gameObject.SetActive(true);
					recipeMap.Add(newToggle, complexRecipe);
					recipeToggles.Add(newToggle);
				}
			}
			buttonScrollContainer.GetComponent<LayoutElement>().minHeight = Mathf.Min(232f, 4f + (float)num * recipeButtonMultiple.GetComponent<LayoutElement>().minHeight);
			if (recipeToggles.Count > 0)
			{
				bool flag2 = false;
				if (selectedRecipeFabricatorMap.ContainsKey(targetFab))
				{
					int num2 = selectedRecipeFabricatorMap[targetFab];
					if (num2 < recipeToggles.Count)
					{
						ToggleClicked(recipeToggles[num2]);
						flag2 = true;
					}
				}
				if (!flag2)
				{
					recipeToggles.ForEach(delegate(KToggle tg)
					{
						if ((UnityEngine.Object)tg != (UnityEngine.Object)selectedToggle)
						{
							tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
						}
					});
					subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPESELECTED);
					descriptionLabel.gameObject.SetActive(false);
					if ((UnityEngine.Object)noRecipeSelectedLabel != (UnityEngine.Object)null)
					{
						noRecipeSelectedLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.SELECTRECIPE);
						noRecipeSelectedLabel.gameObject.SetActive(true);
						IngredientsDescriptorPanel.gameObject.SetActive(false);
						EffectsDescriptorPanel.gameObject.SetActive(false);
					}
					buildBtn.isInteractable = false;
					infiniteBuildBtn.isInteractable = false;
				}
			}
			else
			{
				subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED);
				descriptionLabel.gameObject.SetActive(false);
				if ((UnityEngine.Object)noRecipeSelectedLabel != (UnityEngine.Object)null)
				{
					noRecipeSelectedLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED_BODY);
					noRecipeSelectedLabel.gameObject.SetActive(true);
					IngredientsDescriptorPanel.gameObject.SetActive(false);
					EffectsDescriptorPanel.gameObject.SetActive(false);
				}
				buildBtn.isInteractable = false;
				infiniteBuildBtn.isInteractable = false;
			}
		}
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!recipeMap.ContainsKey(toggle))
		{
			Debug.LogError("Recipe not found on recipe list.", null);
		}
		else
		{
			selectedToggle = toggle;
			selectedToggle.isOn = true;
			selectedToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			buildBtn.isInteractable = true;
			infiniteBuildBtn.isInteractable = true;
			recipeToggles.ForEach(delegate(KToggle tg)
			{
				if ((UnityEngine.Object)tg != (UnityEngine.Object)selectedToggle)
				{
					tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
				}
			});
			selectedRecipe = recipeMap[toggle];
			selectedRecipeFabricatorMap[targetFab] = recipeToggles.IndexOf(toggle);
			buildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE, selectedRecipe.results[0].material.ProperName());
			infiniteBuildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE_INFINITE, selectedRecipe.results[0].material.ProperName());
			buildBtn.ClearOnClick();
			infiniteBuildBtn.ClearOnClick();
			buildBtn.onClick += delegate
			{
				CreateOrder(false);
			};
			infiniteBuildBtn.onClick += delegate
			{
				CreateOrder(true);
			};
			subtitleLabel.SetText(selectedRecipe.results[0].material.ProperName());
			if ((UnityEngine.Object)noRecipeSelectedLabel != (UnityEngine.Object)null)
			{
				noRecipeSelectedLabel.gameObject.SetActive(false);
			}
			descriptionLabel.gameObject.SetActive(true);
			descriptionLabel.SetText(selectedRecipe.description);
			RefreshIngredientDescriptors();
			RefreshResultDescriptors();
		}
	}

	private void RefreshResultDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(GetResultDescription(selectedRecipe));
		list.AddRange(targetFab.AdditionalEffectsForRecipe(selectedRecipe));
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list);
			list.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, Descriptor.DescriptorType.Effect, false));
			EffectsDescriptorPanel.gameObject.SetActive(true);
		}
		EffectsDescriptorPanel.SetDescriptors(list);
	}

	private void RefreshIngredientDescriptors()
	{
		if (selectedRecipe != null)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, Descriptor.DescriptorType.Requirement, false));
			Descriptor ingredientDescription = GetIngredientDescription(selectedRecipe);
			ingredientDescription.IncreaseIndent();
			list.Add(ingredientDescription);
			IngredientsDescriptorPanel.gameObject.SetActive(true);
			IngredientsDescriptorPanel.SetDescriptors(list);
		}
	}

	private void Update()
	{
		RefreshIngredientDescriptors();
	}

	public Descriptor GetIngredientDescription(ComplexRecipe recipe)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < recipe.ingredients.Length; i++)
		{
			ComplexRecipe.RecipeElement recipeElement = recipe.ingredients[i];
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			LocString rECIPERQUIREMENT = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT;
			LocString loc_string = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_INSUFFICIENT;
			if (WorldInventory.Instance.GetAmount(recipeElement.material) >= recipeElement.amount)
			{
				loc_string = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_SUFFICIENT;
			}
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			string formattedByTag2 = GameUtil.GetFormattedByTag(recipeElement.material, WorldInventory.Instance.GetAmount(recipeElement.material), GameUtil.TimeSlice.None);
			text += string.Format(rECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2);
			if (i != recipe.ingredients.Length - 1)
			{
				text += "\n    ";
			}
			text2 += string.Format(loc_string, prefab.GetProperName(), formattedByTag, formattedByTag2);
		}
		return new Descriptor(text, text2, Descriptor.DescriptorType.Requirement, false);
	}

	public List<Descriptor> GetResultDescription(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe.RecipeElement[] results = recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement in results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			LocString rECIPEPRODUCT = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT;
			LocString rECIPEPRODUCT2 = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT;
			list.Add(new Descriptor(string.Format(rECIPEPRODUCT, prefab.GetProperName(), formattedByTag), string.Format(rECIPEPRODUCT2, prefab.GetProperName(), formattedByTag), Descriptor.DescriptorType.Requirement, false));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
				GameUtil.IndentListOfDescriptors(materialDescriptors);
				list.AddRange(materialDescriptors);
			}
		}
		return list;
	}

	private void CreateOrder(bool isInfinite)
	{
		if (selectedRecipe == null)
		{
			Debug.LogError("Cannot create an order for a null recipe", null);
		}
		else
		{
			targetFab.CreateOrder(selectedRecipe, isInfinite, createOrderSound);
		}
	}

	private Element[] GetRecipeElements(Recipe recipe)
	{
		Element[] array = new Element[recipe.Ingredients.Count];
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			Tag tag = recipe.Ingredients[i].tag;
			foreach (Element element in ElementLoader.elements)
			{
				Tag a = GameTagExtensions.Create(element.id);
				if (a == tag)
				{
					array[i] = element;
					break;
				}
			}
		}
		return array;
	}
}
