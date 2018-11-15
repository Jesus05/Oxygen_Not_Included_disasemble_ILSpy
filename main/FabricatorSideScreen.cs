using FMOD.Studio;
using FMODUnity;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FabricatorSideScreen : SideScreenContent
{
	public DescriptorPanel IngredientsDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	[Header("Recipe List")]
	[SerializeField]
	private GameObject recipeGrid;

	[SerializeField]
	private GameObject recipeButton;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	private KToggle selectedToggle;

	[SerializeField]
	private GameObject elementContainer;

	[SerializeField]
	private KButton buildBtn;

	[SerializeField]
	private KButton infiniteBuildBtn;

	[SerializeField]
	private BuildQueue queue;

	[SerializeField]
	private GameObject scrollBarContainer;

	[SerializeField]
	private LocText descriptionLabel;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private LocText noRecipeSelectedLabel;

	private Dictionary<Fabricator, int> selectedRecipeFabricatorMap = new Dictionary<Fabricator, int>();

	[EventRef]
	public string createOrderSound;

	private Fabricator targetFab;

	private Recipe selectedRecipe;

	private Dictionary<KToggle, Recipe> recipeMap;

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
		return (UnityEngine.Object)target.GetComponent<Fabricator>() != (UnityEngine.Object)null && (UnityEngine.Object)target.GetComponent<ResearchCenter>() == (UnityEngine.Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		Fabricator component = target.GetComponent<Fabricator>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogError("The object selected doesn't have a fabricator!", null);
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

	public void Initialize(Fabricator target)
	{
		if ((UnityEngine.Object)target == (UnityEngine.Object)null)
		{
			Debug.LogError("Fabricator provided was null.", null);
		}
		else
		{
			targetFab = target;
			base.gameObject.SetActive(true);
			Recipe[] recipes = targetFab.GetRecipes();
			Array.Sort(recipes, (Recipe a, Recipe b) => a.sortOrder - b.sortOrder);
			recipeMap = new Dictionary<KToggle, Recipe>();
			recipeToggles.ForEach(delegate(KToggle rbi)
			{
				UnityEngine.Object.Destroy(rbi.gameObject);
			});
			recipeToggles.Clear();
			Recipe[] array = recipes;
			foreach (Recipe recipe in array)
			{
				if (!recipe.Result.IsValid)
				{
					Debug.LogErrorFormat("Cant proceed without a recipe end product! [{0}]", recipe.Name);
				}
				if (!string.IsNullOrEmpty(recipe.TechUnlock))
				{
					TechItem techItem = Db.Get().TechItems.TryGet(recipe.TechUnlock);
					if (!DebugHandler.InstantBuildMode && techItem != null && !techItem.IsComplete())
					{
						continue;
					}
				}
				if (target.hideRecipesUndiscoveredIngredients)
				{
					bool flag = false;
					foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
					{
						if (!WorldInventory.Instance.IsDiscovered(ingredient.tag) && !DebugHandler.InstantBuildMode)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				GameObject prefab = Assets.GetPrefab(recipe.Result);
				KToggle newToggle = Util.KInstantiateUI<KToggle>(recipeButton, recipeGrid, false);
				newToggle.GetComponentInChildren<LocText>().text = recipe.Name;
				KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
				if (component.AnimFiles == null || component.AnimFiles.Length == 0)
				{
					Debug.LogErrorFormat("Missing UI sprite anim files for {0}", recipe.Name);
				}
				Sprite sprite = (!((UnityEngine.Object)recipe.Icon == (UnityEngine.Object)null)) ? recipe.Icon : Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false);
				if ((UnityEngine.Object)sprite == (UnityEngine.Object)null)
				{
					sprite = elementPlaceholderSpr;
				}
				Image componentInChildrenOnly = newToggle.gameObject.GetComponentInChildrenOnly<Image>();
				componentInChildrenOnly.sprite = sprite;
				newToggle.onClick += delegate
				{
					ToggleClicked(newToggle);
				};
				newToggle.gameObject.SetActive(true);
				if ((UnityEngine.Object)recipe.Icon != (UnityEngine.Object)null)
				{
					componentInChildrenOnly.color = recipe.IconColor;
					componentInChildrenOnly.rectTransform().sizeDelta = new Vector2(-30f, -40f);
					componentInChildrenOnly.rectTransform().anchoredPosition = new Vector2(0f, 14f);
				}
				recipeMap.Add(newToggle, recipe);
				recipeToggles.Add(newToggle);
			}
			if (recipeToggles.Count > 0)
			{
				if (selectedRecipeFabricatorMap.ContainsKey(targetFab))
				{
					int index = selectedRecipeFabricatorMap[targetFab];
					ToggleClicked(recipeToggles[index]);
				}
				else
				{
					selectedRecipe = null;
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
						noRecipeSelectedLabel.gameObject.SetActive(true);
						IngredientsDescriptorPanel.gameObject.SetActive(false);
						EffectsDescriptorPanel.gameObject.SetActive(false);
					}
					buildBtn.isInteractable = false;
					infiniteBuildBtn.isInteractable = false;
				}
			}
			scrollBarContainer.SetActive(recipeToggles.Count > 4);
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
			buildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE, selectedRecipe.Name);
			infiniteBuildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE_INFINITE, selectedRecipe.Name);
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
			string recipeDescription = GameUtil.GetRecipeDescription(selectedRecipe);
			subtitleLabel.SetText(selectedRecipe.Name);
			if ((UnityEngine.Object)noRecipeSelectedLabel != (UnityEngine.Object)null)
			{
				noRecipeSelectedLabel.gameObject.SetActive(false);
			}
			descriptionLabel.gameObject.SetActive(true);
			descriptionLabel.SetText(recipeDescription);
			RefreshIngredientDescriptors();
			GameObject prefab = Assets.GetPrefab(selectedRecipe.Result);
			List<Descriptor> list = new List<Descriptor>();
			List<Descriptor> list2 = new List<Descriptor>();
			list2.AddRange(GameUtil.GetGameObjectRequirements(prefab));
			if (list2.Count > 0)
			{
				GameUtil.IndentListOfDescriptors(list2);
				list2.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTREQUIREMENTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTREQUIREMENTS, Descriptor.DescriptorType.Effect, false));
				EffectsDescriptorPanel.gameObject.SetActive(true);
			}
			List<Descriptor> list3 = new List<Descriptor>();
			list3.AddRange(GameUtil.GetGameObjectEffects(prefab, false));
			if (list3.Count > 0)
			{
				GameUtil.IndentListOfDescriptors(list3);
				list3.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, Descriptor.DescriptorType.Effect, false));
				EffectsDescriptorPanel.gameObject.SetActive(true);
			}
			list.AddRange(list2);
			list.AddRange(list3);
			EffectsDescriptorPanel.SetDescriptors(list);
		}
	}

	private void RefreshIngredientDescriptors()
	{
		if (selectedRecipe != null)
		{
			List<Descriptor> ingredientDescriptions = GetIngredientDescriptions(selectedRecipe.Ingredients);
			if (ingredientDescriptions.Count > 0)
			{
				GameUtil.IndentListOfDescriptors(ingredientDescriptions);
				ingredientDescriptions.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, Descriptor.DescriptorType.Requirement, false));
				IngredientsDescriptorPanel.gameObject.SetActive(true);
			}
			IngredientsDescriptorPanel.SetDescriptors(ingredientDescriptions);
		}
	}

	private void Update()
	{
		RefreshIngredientDescriptors();
	}

	public List<Descriptor> GetIngredientDescriptions(List<Recipe.Ingredient> ingredients)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			Tag tag = ingredient.tag;
			LocString rECIPERQUIREMENT = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT;
			LocString loc_string = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_INSUFFICIENT;
			if (WorldInventory.Instance.GetAmount(tag) >= ingredient.amount)
			{
				loc_string = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_SUFFICIENT;
			}
			string formattedByTag = GameUtil.GetFormattedByTag(tag, ingredient.amount, GameUtil.TimeSlice.None);
			string formattedByTag2 = GameUtil.GetFormattedByTag(tag, WorldInventory.Instance.GetAmount(tag), GameUtil.TimeSlice.None);
			Descriptor item = new Descriptor(string.Format(rECIPERQUIREMENT, tag.ProperName(), formattedByTag, formattedByTag2), string.Format(loc_string, tag.ProperName(), formattedByTag, formattedByTag2), Descriptor.DescriptorType.Requirement, false);
			list.Add(item);
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
			List<Tag> list = new List<Tag>();
			foreach (Recipe.Ingredient ingredient in selectedRecipe.Ingredients)
			{
				list.Add(ingredient.tag);
			}
			targetFab.CreateOrder(selectedRecipe, list, isInfinite, createOrderSound);
		}
	}
}
