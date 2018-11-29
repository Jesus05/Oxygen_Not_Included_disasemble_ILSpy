using FMOD.Studio;
using FMODUnity;
using STRINGS;
using System;
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

	public DescriptorPanel IngredientsDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

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

	public ScriptableObject styleTooltipHeader;

	public ScriptableObject styleTooltipBody;

	private ComplexFabricator targetFab;

	private ComplexRecipe selectedRecipe;

	private Dictionary<GameObject, ComplexRecipe> recipeMap;

	private List<GameObject> recipeToggles = new List<GameObject>();

	private int targetOrdersUpdatedSubHandle = -1;

	public override string GetTitle()
	{
		if (!((UnityEngine.Object)targetFab == (UnityEngine.Object)null))
		{
			return string.Format(Strings.Get(titleKey), targetFab.GetProperName());
		}
		return Strings.Get(titleKey).ToString().Replace("{0}", "");
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<ComplexFabricator>() != (UnityEngine.Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		ComplexFabricator component = target.GetComponent<ComplexFabricator>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			Debug.LogError("The object selected doesn't have a ComplexFabricator!", null);
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
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				RefreshQueueCountDisplay(gameObject, targetFab);
			}
		}
		if (targetFab.CurrentMachineOrder != null)
		{
			currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, targetFab.CurrentMachineOrder.parentOrder.recipe.GetUIName());
		}
		else
		{
			currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
		}
		if (targetFab.GetMachineOrders().Count > 1)
		{
			nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, targetFab.GetMachineOrders()[1].parentOrder.recipe.GetUIName());
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
		}
		base.OnShow(show);
	}

	public void Initialize(ComplexFabricator target)
	{
		if ((UnityEngine.Object)target == (UnityEngine.Object)null)
		{
			Debug.LogError("ComplexFabricator provided was null.", null);
		}
		else
		{
			targetFab = target;
			base.gameObject.SetActive(true);
			ComplexRecipe[] recipes = targetFab.GetRecipes();
			Array.Sort(recipes, (ComplexRecipe a, ComplexRecipe b) => a.sortOrder - b.sortOrder);
			recipeMap = new Dictionary<GameObject, ComplexRecipe>();
			recipeToggles.ForEach(delegate(GameObject rbi)
			{
				UnityEngine.Object.Destroy(rbi.gameObject);
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
			ComplexRecipe[] array = recipes;
			foreach (ComplexRecipe recipe in array)
			{
				if (recipe.IsRequiredTechUnlocked() || DebugHandler.InstantBuildMode)
				{
					bool flag = true;
					ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
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
							ComplexRecipe.RecipeElement[] ingredients2 = recipe.ingredients;
							foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
							{
								GameObject gameObject = Util.KInstantiateUI(component2.GetReference("FromIconPrefab").gameObject, component2.GetReference("FromIcons").gameObject, true);
								gameObject.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement2.material, "ui", false).first;
								gameObject.GetComponent<Image>().color = Def.GetUISprite(recipeElement2.material, "ui", false).second;
								gameObject.gameObject.name = recipeElement2.material.Name;
							}
							ComplexRecipe.RecipeElement[] results = recipe.results;
							foreach (ComplexRecipe.RecipeElement recipeElement3 in results)
							{
								GameObject gameObject2 = Util.KInstantiateUI(component2.GetReference("ToIconPrefab").gameObject, component2.GetReference("ToIcons").gameObject, true);
								gameObject2.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement3.material, "ui", false).first;
								gameObject2.GetComponent<Image>().color = Def.GetUISprite(recipeElement3.material, "ui", false).second;
								gameObject2.gameObject.name = recipeElement3.material.Name;
							}
							break;
						}
						case StyleSetting.ListQueueHybrid:
						{
							entryGO = Util.KInstantiateUI(recipeButtonQueueHybrid, recipeGrid, false);
							recipeMap.Add(entryGO, recipe);
							Image image2 = entryGO.GetComponentsInChildrenOnly<Image>()[2];
							if (!recipe.useResultAsDescription)
							{
								image2.sprite = uISprite.first;
								image2.color = uISprite.second;
							}
							else
							{
								image2.sprite = uISprite2.first;
								image2.color = uISprite2.second;
							}
							entryGO.GetComponentInChildren<LocText>().text = recipe.GetUIName();
							bool flag2 = CheckRecipeRequirements(recipe);
							image2.material = ((!flag2) ? Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial);
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
							Image image = newToggle.gameObject.GetComponentsInChildrenOnly<Image>()[0];
							if (target.sideScreenStyle == StyleSetting.GridInput || target.sideScreenStyle == StyleSetting.ListInput)
							{
								image.sprite = uISprite.first;
								image.color = uISprite.second;
							}
							else
							{
								image.sprite = uISprite2.first;
								image.color = uISprite2.second;
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
						component3.AddMultiStringTooltip("\n", null);
						component3.AddMultiStringTooltip("Ingredients", styleTooltipHeader);
						foreach (Descriptor ingredientDescription in GetIngredientDescriptions(recipe))
						{
							component3.AddMultiStringTooltip(ingredientDescription.IndentedText(), styleTooltipBody);
						}
						component3.AddMultiStringTooltip("\n", null);
						component3.AddMultiStringTooltip("Results", styleTooltipHeader);
						List<Descriptor> resultDescriptions = GetResultDescriptions(recipe);
						for (int m = 0; m < resultDescriptions.Count; m++)
						{
							string text = (m != 0) ? "" : "    • ";
							component3.AddMultiStringTooltip(resultDescriptions[m].IndentedText(), styleTooltipBody);
						}
						if (targetFab.sideScreenStyle != StyleSetting.ListQueueHybrid)
						{
							newToggle.onClick += delegate
							{
								ToggleClicked(newToggle);
							};
							entryGO.SetActive(true);
						}
						recipeToggles.Add(entryGO);
					}
				}
			}
			RefreshIngredientDescriptors();
			LayoutElement component4 = buttonScrollContainer.GetComponent<LayoutElement>();
			float num2 = (float)num;
			Vector2 sizeDelta = recipeButtonQueueHybrid.rectTransform().sizeDelta;
			component4.minHeight = Mathf.Min(512f, 2f + num2 * sizeDelta.y);
			if (recipeToggles.Count > 0)
			{
				bool flag3 = false;
				if (selectedRecipeFabricatorMap.ContainsKey(targetFab))
				{
					int num3 = selectedRecipeFabricatorMap[targetFab];
					if (num3 < recipeToggles.Count)
					{
						ToggleClicked(recipeToggles[num3].GetComponent<KToggle>());
						flag3 = true;
					}
				}
				if (!flag3)
				{
					recipeToggles.ForEach(delegate(GameObject tg)
					{
						if ((UnityEngine.Object)tg != (UnityEngine.Object)selectedToggle)
						{
							ImageToggleState component5 = tg.GetComponent<ImageToggleState>();
							if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
							{
								tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
							}
						}
					});
				}
			}
		}
	}

	private void RefreshQueueCountDisplay(GameObject entryGO, ComplexFabricator fabricator)
	{
		HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
		bool flag = fabricator.GetRecipeQueueCount(recipeMap[entryGO]) == ComplexFabricator.QUEUE_INFINITE_COUNT;
		component.GetReference<LocText>("CountLabel").text = ((!flag) ? fabricator.GetRecipeQueueCount(recipeMap[entryGO]).ToString() : "");
		component.GetReference<RectTransform>("InfiniteIcon").gameObject.SetActive(flag);
		component.GetReference<RectTransform>("CountLabelX").gameObject.SetActive(!flag);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!recipeMap.ContainsKey(toggle.gameObject))
		{
			Debug.LogError("Recipe not found on recipe list.", null);
		}
		else
		{
			selectedToggle = toggle;
			selectedToggle.isOn = true;
			selectedToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			recipeToggles.ForEach(delegate(GameObject tg)
			{
				if ((UnityEngine.Object)tg != (UnityEngine.Object)selectedToggle)
				{
					tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
				}
			});
			selectedRecipe = recipeMap[toggle.gameObject];
			selectedRecipeFabricatorMap[targetFab] = recipeToggles.IndexOf(toggle.gameObject);
			RefreshIngredientDescriptors();
			RefreshResultDescriptors();
		}
	}

	private void RefreshResultDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(GetResultDescriptions(selectedRecipe));
		list.AddRange(targetFab.AdditionalEffectsForRecipe(selectedRecipe));
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list, 1);
			list.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, Descriptor.DescriptorType.Effect, false));
			EffectsDescriptorPanel.gameObject.SetActive(true);
		}
		EffectsDescriptorPanel.SetDescriptors(list);
	}

	private bool CheckRecipeRequirements(ComplexRecipe recipe)
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

	private void RefreshIngredientDescriptors()
	{
		foreach (KeyValuePair<GameObject, ComplexRecipe> item in recipeMap)
		{
			HierarchyReferences component = item.Key.GetComponent<HierarchyReferences>();
			bool flag = CheckRecipeRequirements(item.Value);
			component.GetReference<Image>("StatusBG").color = ((!flag) ? new Color(0.5f, 0.5f, 0.5f, 0.2f) : Color.clear);
			component.GetReference<LocText>("Label").color = ((!flag) ? new Color(0.22f, 0.22f, 0.22f, 1f) : Color.black);
		}
	}

	private void Update()
	{
		RefreshIngredientDescriptors();
	}

	public List<Descriptor> GetIngredientDescriptions(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			string formattedByTag2 = GameUtil.GetFormattedByTag(recipeElement.material, WorldInventory.Instance.GetAmount(recipeElement.material), GameUtil.TimeSlice.None);
			string text = (!(WorldInventory.Instance.GetAmount(recipeElement.material) >= recipeElement.amount)) ? ("<color=#F44A47>    • " + string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2) + "</color>") : ("    • " + string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2));
			list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Requirement, false));
		}
		return list;
	}

	public List<Descriptor> GetResultDescriptions(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe.RecipeElement[] results = recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement in results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			list.Add(new Descriptor(string.Format("    • " + UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), string.Format("    • " + UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), Descriptor.DescriptorType.Requirement, false));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
				GameUtil.IndentListOfDescriptors(materialDescriptors, 2);
				list.AddRange(materialDescriptors);
			}
		}
		return list;
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
