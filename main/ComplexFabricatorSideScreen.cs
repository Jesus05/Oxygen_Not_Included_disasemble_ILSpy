using FMOD.Studio;
using FMODUnity;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComplexFabricatorSideScreen : SideScreenContent
{
	public enum StyleSetting
	{
		GridResult,
		ListResult,
		GridInput,
		ListInput,
		ListInputOutput,
		GridInputOutput,
		ClassicFabricator,
		ListQueueHybrid
	}

	[Header("Recipe List")]
	[SerializeField]
	private GameObject recipeGrid;

	[Header("Recipe button variants")]
	[SerializeField]
	private GameObject recipeButton;

	[SerializeField]
	private GameObject recipeButtonMultiple;

	[SerializeField]
	private GameObject recipeButtonQueueHybrid;

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
	private LocText currentOrderLabel;

	[SerializeField]
	private LocText nextOrderLabel;

	private Dictionary<ComplexFabricator, int> selectedRecipeFabricatorMap = new Dictionary<ComplexFabricator, int>();

	[EventRef]
	public string createOrderSound;

	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private LocText noRecipesDiscoveredLabel;

	public ScriptableObject styleTooltipHeader;

	public ScriptableObject styleTooltipBody;

	private ComplexFabricator targetFab;

	private ComplexRecipe selectedRecipe;

	private Dictionary<GameObject, ComplexRecipe> recipeMap;

	private List<GameObject> recipeToggles = new List<GameObject>();

	public SelectedRecipeQueueScreen recipeScreenPrefab;

	private SelectedRecipeQueueScreen recipeScreen;

	private int targetOrdersUpdatedSubHandle = -1;

	public override string GetTitle()
	{
		if ((Object)targetFab == (Object)null)
		{
			return Strings.Get(titleKey).ToString().Replace("{0}", string.Empty);
		}
		return string.Format(Strings.Get(titleKey), targetFab.GetProperName());
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<ComplexFabricator>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		ComplexFabricator component = target.GetComponent<ComplexFabricator>();
		if ((Object)component == (Object)null)
		{
			Debug.LogError("The object selected doesn't have a ComplexFabricator!");
		}
		else
		{
			if (targetOrdersUpdatedSubHandle != -1)
			{
				Unsubscribe(targetOrdersUpdatedSubHandle);
			}
			Initialize(component);
			targetOrdersUpdatedSubHandle = targetFab.Subscribe(1721324763, UpdateQueueCountLabels);
			UpdateQueueCountLabels(null);
		}
	}

	private void UpdateQueueCountLabels(object data = null)
	{
		ComplexRecipe[] recipes = targetFab.GetRecipes();
		foreach (ComplexRecipe r in recipes)
		{
			GameObject gameObject = recipeToggles.Find((GameObject match) => recipeMap[match] == r);
			if ((Object)gameObject != (Object)null)
			{
				RefreshQueueCountDisplay(gameObject, targetFab);
			}
		}
		if (targetFab.CurrentWorkingOrder != null)
		{
			currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, targetFab.CurrentWorkingOrder.GetUIName());
		}
		else
		{
			currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
		}
		if (targetFab.NextOrder != null)
		{
			nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, targetFab.NextOrder.GetUIName());
		}
		else
		{
			nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
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
			DetailsScreen.Instance.ClearSecondarySideScreen();
			selectedRecipe = null;
			selectedToggle = null;
		}
		base.OnShow(show);
	}

	public void Initialize(ComplexFabricator target)
	{
		if ((Object)target == (Object)null)
		{
			Debug.LogError("ComplexFabricator provided was null.");
		}
		else
		{
			targetFab = target;
			base.gameObject.SetActive(true);
			recipeMap = new Dictionary<GameObject, ComplexRecipe>();
			recipeToggles.ForEach(delegate(GameObject rbi)
			{
				Object.Destroy(rbi.gameObject);
			});
			recipeToggles.Clear();
			GridLayoutGroup component = recipeGrid.GetComponent<GridLayoutGroup>();
			switch (targetFab.sideScreenStyle)
			{
			case StyleSetting.ListResult:
			case StyleSetting.ListInput:
			case StyleSetting.ListInputOutput:
			{
				component.constraintCount = 1;
				GridLayoutGroup gridLayoutGroup2 = component;
				Vector2 cellSize2 = component.cellSize;
				gridLayoutGroup2.cellSize = new Vector2(262f, cellSize2.y);
				break;
			}
			case StyleSetting.ClassicFabricator:
				component.constraintCount = 128;
				component.cellSize = new Vector2(78f, 96f);
				buttonScrollContainer.minHeight = 100f;
				break;
			case StyleSetting.ListQueueHybrid:
				component.constraintCount = 1;
				component.cellSize = new Vector2(264f, 64f);
				buttonScrollContainer.minHeight = 66f;
				break;
			default:
			{
				component.constraintCount = 3;
				GridLayoutGroup gridLayoutGroup = component;
				Vector2 cellSize = component.cellSize;
				gridLayoutGroup.cellSize = new Vector2(116f, cellSize.y);
				break;
			}
			}
			int num = 0;
			ComplexRecipe[] recipes = targetFab.GetRecipes();
			ComplexRecipe[] array = recipes;
			foreach (ComplexRecipe recipe in array)
			{
				bool flag = false;
				if (DebugHandler.InstantBuildMode)
				{
					flag = true;
				}
				else if (recipe.RequiresTechUnlock() && recipe.IsRequiredTechUnlocked())
				{
					flag = true;
				}
				else if (target.GetRecipeQueueCount(recipe) != 0)
				{
					flag = true;
				}
				else if (AnyRecipeRequirementsDiscovered(recipe))
				{
					flag = true;
				}
				else if (HasAnyRecipeRequirements(recipe))
				{
					flag = true;
				}
				if (flag)
				{
					num++;
					Tuple<Sprite, Color> uISprite = Def.GetUISprite(recipe.ingredients[0].material, "ui", false);
					Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(recipe.results[0].material, "ui", false);
					KToggle newToggle = null;
					GameObject entryGO;
					switch (target.sideScreenStyle)
					{
					case StyleSetting.ListInputOutput:
					case StyleSetting.GridInputOutput:
					{
						newToggle = Util.KInstantiateUI<KToggle>(recipeButtonMultiple, recipeGrid, false);
						entryGO = newToggle.gameObject;
						HierarchyReferences component2 = newToggle.GetComponent<HierarchyReferences>();
						ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
						foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
						{
							GameObject gameObject = Util.KInstantiateUI(component2.GetReference("FromIconPrefab").gameObject, component2.GetReference("FromIcons").gameObject, true);
							gameObject.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement.material, "ui", false).first;
							gameObject.GetComponent<Image>().color = Def.GetUISprite(recipeElement.material, "ui", false).second;
							gameObject.gameObject.name = recipeElement.material.Name;
						}
						ComplexRecipe.RecipeElement[] results = recipe.results;
						foreach (ComplexRecipe.RecipeElement recipeElement2 in results)
						{
							GameObject gameObject2 = Util.KInstantiateUI(component2.GetReference("ToIconPrefab").gameObject, component2.GetReference("ToIcons").gameObject, true);
							gameObject2.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement2.material, "ui", false).first;
							gameObject2.GetComponent<Image>().color = Def.GetUISprite(recipeElement2.material, "ui", false).second;
							gameObject2.gameObject.name = recipeElement2.material.Name;
						}
						break;
					}
					case StyleSetting.ListQueueHybrid:
					{
						newToggle = Util.KInstantiateUI<KToggle>(recipeButtonQueueHybrid, recipeGrid, false);
						entryGO = newToggle.gameObject;
						recipeMap.Add(entryGO, recipe);
						Image image = entryGO.GetComponentsInChildrenOnly<Image>()[2];
						if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient)
						{
							image.sprite = uISprite.first;
							image.color = uISprite.second;
						}
						else
						{
							image.sprite = uISprite2.first;
							image.color = uISprite2.second;
						}
						entryGO.GetComponentInChildren<LocText>().text = recipe.GetUIName();
						bool flag2 = HasAllRecipeRequirements(recipe);
						image.material = ((!flag2) ? Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial);
						RefreshQueueCountDisplay(entryGO, targetFab);
						entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("DecrementButton").onClick = delegate
						{
							target.DecrementRecipeQueueCount(recipe, false);
							RefreshQueueCountDisplay(entryGO, target);
						};
						entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("IncrementButton").onClick = delegate
						{
							target.IncrementRecipeQueueCount(recipe);
							RefreshQueueCountDisplay(entryGO, target);
						};
						entryGO.gameObject.SetActive(true);
						break;
					}
					default:
					{
						newToggle = Util.KInstantiateUI<KToggle>(recipeButton, recipeGrid, false);
						entryGO = newToggle.gameObject;
						Image componentInChildrenOnly = newToggle.gameObject.GetComponentInChildrenOnly<Image>();
						if (target.sideScreenStyle == StyleSetting.GridInput || target.sideScreenStyle == StyleSetting.ListInput)
						{
							componentInChildrenOnly.sprite = uISprite.first;
							componentInChildrenOnly.color = uISprite.second;
						}
						else
						{
							componentInChildrenOnly.sprite = uISprite2.first;
							componentInChildrenOnly.color = uISprite2.second;
						}
						break;
					}
					}
					if (targetFab.sideScreenStyle == StyleSetting.ClassicFabricator)
					{
						newToggle.GetComponentInChildren<LocText>().text = recipe.results[0].material.ProperName();
					}
					else if (targetFab.sideScreenStyle != StyleSetting.ListQueueHybrid)
					{
						newToggle.GetComponentInChildren<LocText>().text = string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_WITH_NEWLINES, recipe.ingredients[0].material.ProperName(), recipe.results[0].material.ProperName());
					}
					ToolTip component3 = entryGO.GetComponent<ToolTip>();
					component3.toolTipPosition = ToolTip.TooltipPosition.Custom;
					component3.parentPositionAnchor = new Vector2(0f, 0.5f);
					component3.tooltipPivot = new Vector2(1f, 1f);
					component3.tooltipPositionOffset = new Vector2(-24f, 20f);
					component3.ClearMultiStringTooltip();
					component3.AddMultiStringTooltip(recipe.GetUIName(), styleTooltipHeader);
					component3.AddMultiStringTooltip(recipe.description, styleTooltipBody);
					newToggle.onClick += delegate
					{
						ToggleClicked(newToggle);
					};
					entryGO.SetActive(true);
					recipeToggles.Add(entryGO);
				}
			}
			if (recipeToggles.Count > 0)
			{
				LayoutElement component4 = buttonScrollContainer.GetComponent<LayoutElement>();
				float num2 = (float)num;
				Vector2 sizeDelta = recipeButtonQueueHybrid.rectTransform().sizeDelta;
				component4.minHeight = Mathf.Min(451f, 2f + num2 * sizeDelta.y);
				subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.SUBTITLE);
				noRecipesDiscoveredLabel.gameObject.SetActive(false);
			}
			else
			{
				subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED);
				noRecipesDiscoveredLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED_BODY);
				noRecipesDiscoveredLabel.gameObject.SetActive(true);
				LayoutElement component5 = buttonScrollContainer.GetComponent<LayoutElement>();
				Vector2 sizeDelta2 = noRecipesDiscoveredLabel.rectTransform.sizeDelta;
				component5.minHeight = sizeDelta2.y + 10f;
			}
			RefreshIngredientAvailabilityVis();
		}
	}

	public void RefreshQueueCountDisplayForRecipe(ComplexRecipe recipe, ComplexFabricator fabricator)
	{
		GameObject gameObject = recipeToggles.Find((GameObject match) => recipeMap[match] == recipe);
		if ((Object)gameObject != (Object)null)
		{
			RefreshQueueCountDisplay(gameObject, fabricator);
		}
	}

	private void RefreshQueueCountDisplay(GameObject entryGO, ComplexFabricator fabricator)
	{
		HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
		bool flag = fabricator.GetRecipeQueueCount(recipeMap[entryGO]) == ComplexFabricator.QUEUE_INFINITE;
		component.GetReference<LocText>("CountLabel").text = ((!flag) ? fabricator.GetRecipeQueueCount(recipeMap[entryGO]).ToString() : string.Empty);
		component.GetReference<RectTransform>("InfiniteIcon").gameObject.SetActive(flag);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!recipeMap.ContainsKey(toggle.gameObject))
		{
			Debug.LogError("Recipe not found on recipe list.");
		}
		else
		{
			if ((Object)selectedToggle == (Object)toggle)
			{
				selectedToggle.isOn = false;
				selectedToggle = null;
				selectedRecipe = null;
			}
			else
			{
				selectedToggle = toggle;
				selectedToggle.isOn = true;
				selectedRecipe = recipeMap[toggle.gameObject];
				selectedRecipeFabricatorMap[targetFab] = recipeToggles.IndexOf(toggle.gameObject);
			}
			RefreshIngredientAvailabilityVis();
			if (toggle.isOn)
			{
				recipeScreen = (SelectedRecipeQueueScreen)DetailsScreen.Instance.SetSecondarySideScreen(recipeScreenPrefab, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_DETAILS);
				recipeScreen.SetRecipe(this, targetFab, selectedRecipe);
			}
			else
			{
				DetailsScreen.Instance.ClearSecondarySideScreen();
			}
		}
	}

	private bool HasAnyRecipeRequirements(ComplexRecipe recipe)
	{
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			if (WorldInventory.Instance.GetAmount(recipeElement.material) + targetFab.inStorage.GetAmountAvailable(recipeElement.material) + targetFab.buildStorage.GetAmountAvailable(recipeElement.material) >= recipeElement.amount)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasAllRecipeRequirements(ComplexRecipe recipe)
	{
		bool result = true;
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			if (WorldInventory.Instance.GetAmount(recipeElement.material) + targetFab.inStorage.GetAmountAvailable(recipeElement.material) + targetFab.buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				result = false;
			}
		}
		return result;
	}

	private bool AnyRecipeRequirementsDiscovered(ComplexRecipe recipe)
	{
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			if (WorldInventory.Instance.IsDiscovered(recipeElement.material))
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		RefreshIngredientAvailabilityVis();
	}

	private void RefreshIngredientAvailabilityVis()
	{
		foreach (KeyValuePair<GameObject, ComplexRecipe> item in recipeMap)
		{
			HierarchyReferences component = item.Key.GetComponent<HierarchyReferences>();
			bool flag = HasAllRecipeRequirements(item.Value);
			KToggle component2 = item.Key.GetComponent<KToggle>();
			if (flag)
			{
				if (selectedRecipe == item.Value)
				{
					component2.ActivateFlourish(true, ImageToggleState.State.Active);
				}
				else
				{
					component2.ActivateFlourish(false, ImageToggleState.State.Inactive);
				}
			}
			else if (selectedRecipe == item.Value)
			{
				component2.ActivateFlourish(true, ImageToggleState.State.DisabledActive);
			}
			else
			{
				component2.ActivateFlourish(false, ImageToggleState.State.Disabled);
			}
			component.GetReference<LocText>("Label").color = ((!flag) ? new Color(0.22f, 0.22f, 0.22f, 1f) : Color.black);
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
