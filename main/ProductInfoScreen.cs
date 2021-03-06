using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductInfoScreen : KScreen
{
	public TitleBar titleBar;

	public GameObject ProductDescriptionPane;

	public LocText productDescriptionText;

	public DescriptorPanel ProductRequirementsPane;

	public DescriptorPanel ProductEffectsPane;

	public GameObject ProductFlavourPane;

	public LocText productFlavourText;

	public RectTransform BGPanel;

	public MaterialSelectionPanel materialSelectionPanelPrefab;

	private Dictionary<string, GameObject> descLabels = new Dictionary<string, GameObject>();

	public MultiToggle sandboxInstantBuildToggle;

	[NonSerialized]
	public MaterialSelectionPanel materialSelectionPanel;

	[NonSerialized]
	public BuildingDef currentDef;

	public System.Action onElementsFullySelected;

	private bool expandedInfo = true;

	private bool configuring;

	private void RefreshScreen()
	{
		if ((UnityEngine.Object)currentDef != (UnityEngine.Object)null)
		{
			SetTitle(currentDef);
		}
		else
		{
			ClearProduct(true);
		}
	}

	public void ClearProduct(bool deactivateTool = true)
	{
		currentDef = null;
		materialSelectionPanel.ClearMaterialToggles();
		if ((UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)BuildTool.Instance && deactivateTool)
		{
			BuildTool.Instance.Deactivate();
		}
		if ((UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)UtilityBuildTool.Instance || (UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)WireBuildTool.Instance)
		{
			ToolMenu.Instance.ClearSelection();
		}
		ClearLabels();
		Show(false);
	}

	public new void Awake()
	{
		base.Awake();
		materialSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(materialSelectionPanelPrefab.gameObject, base.gameObject, false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((UnityEngine.Object)BuildingGroupScreen.Instance != (UnityEngine.Object)null)
		{
			BuildingGroupScreen instance = BuildingGroupScreen.Instance;
			instance.pointerEnterActions = (PointerEnterActions)Delegate.Combine(instance.pointerEnterActions, new PointerEnterActions(CheckMouseOver));
			BuildingGroupScreen instance2 = BuildingGroupScreen.Instance;
			instance2.pointerExitActions = (PointerExitActions)Delegate.Combine(instance2.pointerExitActions, new PointerExitActions(CheckMouseOver));
		}
		if ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null)
		{
			PlanScreen instance3 = PlanScreen.Instance;
			instance3.pointerEnterActions = (PointerEnterActions)Delegate.Combine(instance3.pointerEnterActions, new PointerEnterActions(CheckMouseOver));
			PlanScreen instance4 = PlanScreen.Instance;
			instance4.pointerExitActions = (PointerExitActions)Delegate.Combine(instance4.pointerExitActions, new PointerExitActions(CheckMouseOver));
		}
		if ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null)
		{
			BuildMenu instance5 = BuildMenu.Instance;
			instance5.pointerEnterActions = (PointerEnterActions)Delegate.Combine(instance5.pointerEnterActions, new PointerEnterActions(CheckMouseOver));
			BuildMenu instance6 = BuildMenu.Instance;
			instance6.pointerExitActions = (PointerExitActions)Delegate.Combine(instance6.pointerExitActions, new PointerExitActions(CheckMouseOver));
		}
		pointerEnterActions = (PointerEnterActions)Delegate.Combine(pointerEnterActions, new PointerEnterActions(CheckMouseOver));
		pointerExitActions = (PointerExitActions)Delegate.Combine(pointerExitActions, new PointerExitActions(CheckMouseOver));
		ConsumeMouseScroll = true;
		sandboxInstantBuildToggle.ChangeState(SandboxToolParameterMenu.instance.settings.InstantBuild ? 1 : 0);
		MultiToggle multiToggle = sandboxInstantBuildToggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SandboxToolParameterMenu.instance.settings.InstantBuild = !SandboxToolParameterMenu.instance.settings.InstantBuild;
			sandboxInstantBuildToggle.ChangeState(SandboxToolParameterMenu.instance.settings.InstantBuild ? 1 : 0);
		});
		sandboxInstantBuildToggle.gameObject.SetActive(Game.Instance.SandboxModeActive);
		Game.Instance.Subscribe(-1948169901, delegate
		{
			sandboxInstantBuildToggle.gameObject.SetActive(Game.Instance.SandboxModeActive);
		});
	}

	public void ConfigureScreen(BuildingDef def)
	{
		configuring = true;
		currentDef = def;
		SetTitle(def);
		SetDescription(def);
		SetEffects(def);
		SetMaterials(def);
		configuring = false;
	}

	private void ExpandInfo(PointerEventData data)
	{
		ToggleExpandedInfo(true);
	}

	private void CollapseInfo(PointerEventData data)
	{
		ToggleExpandedInfo(false);
	}

	public void ToggleExpandedInfo(bool state)
	{
		expandedInfo = state;
		if ((UnityEngine.Object)ProductDescriptionPane != (UnityEngine.Object)null)
		{
			ProductDescriptionPane.SetActive(expandedInfo);
		}
		if ((UnityEngine.Object)ProductRequirementsPane != (UnityEngine.Object)null)
		{
			ProductRequirementsPane.gameObject.SetActive(expandedInfo && ProductRequirementsPane.HasDescriptors());
		}
		if ((UnityEngine.Object)ProductEffectsPane != (UnityEngine.Object)null)
		{
			ProductEffectsPane.gameObject.SetActive(expandedInfo && ProductEffectsPane.HasDescriptors());
		}
		if ((UnityEngine.Object)ProductFlavourPane != (UnityEngine.Object)null)
		{
			ProductFlavourPane.SetActive(expandedInfo);
		}
		if ((UnityEngine.Object)materialSelectionPanel != (UnityEngine.Object)null && materialSelectionPanel.CurrentSelectedElement != (Tag)null)
		{
			materialSelectionPanel.ToggleShowDescriptorPanels(expandedInfo);
		}
	}

	private void CheckMouseOver(PointerEventData data)
	{
		bool state = base.GetMouseOver || ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null && ((PlanScreen.Instance.isActiveAndEnabled && PlanScreen.Instance.GetMouseOver) || BuildingGroupScreen.Instance.GetMouseOver)) || ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null && BuildMenu.Instance.isActiveAndEnabled && BuildMenu.Instance.GetMouseOver);
		ToggleExpandedInfo(state);
	}

	private void Update()
	{
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && (UnityEngine.Object)currentDef != (UnityEngine.Object)null && materialSelectionPanel.CurrentSelectedElement != (Tag)null && !MaterialSelector.AllowInsufficientMaterialBuild() && currentDef.Mass[0] > WorldInventory.Instance.GetAmount(materialSelectionPanel.CurrentSelectedElement))
		{
			materialSelectionPanel.AutoSelectAvailableMaterial();
		}
	}

	private void SetTitle(BuildingDef def)
	{
		titleBar.SetTitle(def.Name);
		bool flag = ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null && PlanScreen.Instance.isActiveAndEnabled && PlanScreen.Instance.BuildableState(def) == PlanScreen.RequirementsState.Complete) || ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null && BuildMenu.Instance.isActiveAndEnabled && BuildMenu.Instance.BuildableState(def) == PlanScreen.RequirementsState.Complete);
		titleBar.GetComponentInChildren<KImage>().ColorState = ((!flag) ? KImage.ColorSelector.Disabled : KImage.ColorSelector.Active);
	}

	private void SetDescription(BuildingDef def)
	{
		if (!((UnityEngine.Object)def == (UnityEngine.Object)null) && !((UnityEngine.Object)productFlavourText == (UnityEngine.Object)null))
		{
			string text = def.Desc;
			Dictionary<Klei.AI.Attribute, float> dictionary = new Dictionary<Klei.AI.Attribute, float>();
			Dictionary<Klei.AI.Attribute, float> dictionary2 = new Dictionary<Klei.AI.Attribute, float>();
			foreach (Klei.AI.Attribute attribute in def.attributes)
			{
				if (!dictionary.ContainsKey(attribute))
				{
					dictionary[attribute] = 0f;
				}
			}
			foreach (AttributeModifier attributeModifier in def.attributeModifiers)
			{
				float value = 0f;
				Klei.AI.Attribute key = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
				dictionary.TryGetValue(key, out value);
				value = (dictionary[key] = value + attributeModifier.Value);
			}
			if (materialSelectionPanel.CurrentSelectedElement != (Tag)null)
			{
				Element element = ElementLoader.GetElement(materialSelectionPanel.CurrentSelectedElement);
				if (element != null)
				{
					foreach (AttributeModifier attributeModifier2 in element.attributeModifiers)
					{
						float value2 = 0f;
						Klei.AI.Attribute key2 = Db.Get().BuildingAttributes.Get(attributeModifier2.AttributeId);
						dictionary2.TryGetValue(key2, out value2);
						value2 = (dictionary2[key2] = value2 + attributeModifier2.Value);
					}
				}
				else
				{
					GameObject gameObject = Assets.TryGetPrefab(materialSelectionPanel.CurrentSelectedElement);
					PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						foreach (AttributeModifier descriptor in component.descriptors)
						{
							float value3 = 0f;
							Klei.AI.Attribute key3 = Db.Get().BuildingAttributes.Get(descriptor.AttributeId);
							dictionary2.TryGetValue(key3, out value3);
							value3 = (dictionary2[key3] = value3 + descriptor.Value);
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				text += "\n\n";
				foreach (KeyValuePair<Klei.AI.Attribute, float> item in dictionary)
				{
					float value4 = 0f;
					dictionary.TryGetValue(item.Key, out value4);
					float value5 = 0f;
					string text2 = string.Empty;
					if (dictionary2.TryGetValue(item.Key, out value5))
					{
						value5 = Mathf.Abs(value4 * value5);
						text2 = "(+" + value5 + ")";
					}
					string text3 = text;
					text = text3 + "\n" + item.Key.Name + ": " + (value4 + value5) + text2;
				}
			}
			productFlavourText.text = text;
		}
	}

	private void SetEffects(BuildingDef def)
	{
		if (productDescriptionText.text != null)
		{
			productDescriptionText.text = $"{def.Effect}";
		}
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def);
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONREQUIREMENTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONREQUIREMENTS, Descriptor.DescriptorType.Effect);
			requirementDescriptors.Insert(0, item);
			ProductRequirementsPane.gameObject.SetActive(true);
		}
		else
		{
			ProductRequirementsPane.gameObject.SetActive(false);
		}
		ProductRequirementsPane.SetDescriptors(requirementDescriptors);
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONEFFECTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONEFFECTS, Descriptor.DescriptorType.Effect);
			effectDescriptors.Insert(0, item2);
			ProductEffectsPane.gameObject.SetActive(true);
		}
		else
		{
			ProductEffectsPane.gameObject.SetActive(false);
		}
		ProductEffectsPane.SetDescriptors(effectDescriptors);
	}

	public void ClearLabels()
	{
		List<string> list = new List<string>(descLabels.Keys);
		if (list.Count > 0)
		{
			foreach (string item in list)
			{
				GameObject gameObject = descLabels[item];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				descLabels.Remove(item);
			}
		}
	}

	public void SetMaterials(BuildingDef def)
	{
		materialSelectionPanel.gameObject.SetActive(true);
		Recipe craftRecipe = def.CraftRecipe;
		materialSelectionPanel.ClearSelectActions();
		materialSelectionPanel.ConfigureScreen(craftRecipe);
		materialSelectionPanel.ToggleShowDescriptorPanels(false);
		materialSelectionPanel.AddSelectAction(RefreshScreen);
		materialSelectionPanel.AddSelectAction(onMenuMaterialChanged);
		materialSelectionPanel.AutoSelectAvailableMaterial();
		ActivateAppropriateTool(def);
	}

	private bool BuildRequirementsMet(BuildingDef def)
	{
		if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
		{
			return true;
		}
		Recipe craftRecipe = def.CraftRecipe;
		if (!materialSelectionPanel.CanBuild(craftRecipe))
		{
			return false;
		}
		if (!Db.Get().TechItems.IsTechItemComplete(def.PrefabID))
		{
			return false;
		}
		return true;
	}

	private void onMenuMaterialChanged()
	{
		if (!((UnityEngine.Object)currentDef == (UnityEngine.Object)null))
		{
			ActivateAppropriateTool(currentDef);
			SetDescription(currentDef);
		}
	}

	private void ActivateAppropriateTool(BuildingDef def)
	{
		Debug.Assert((UnityEngine.Object)def != (UnityEngine.Object)null, "def was null");
		if (materialSelectionPanel.AllSelectorsSelected() && BuildRequirementsMet(def))
		{
			onElementsFullySelected.Signal();
		}
		else if (!MaterialSelector.AllowInsufficientMaterialBuild() && !DebugHandler.InstantBuildMode)
		{
			if ((UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)BuildTool.Instance)
			{
				BuildTool.Instance.Deactivate();
			}
			if ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null)
			{
				PrebuildTool.Instance.Activate(def, PlanScreen.Instance.BuildableState(def));
			}
			if ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null)
			{
				PrebuildTool.Instance.Activate(def, BuildMenu.Instance.BuildableState(def));
			}
		}
	}

	public static bool MaterialsMet(Recipe recipe)
	{
		if (recipe == null)
		{
			Debug.LogError("Trying to verify the materials on a null recipe!");
			return false;
		}
		if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
		{
			Debug.LogError("Trying to verify the materials on a recipe with no MaterialCategoryTags!");
			return false;
		}
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			MaterialSelectionPanel.SelectedElemInfo selectedElemInfo = MaterialSelectionPanel.Filter(recipe.Ingredients[i].tag);
			if (selectedElemInfo.kgAvailable < recipe.Ingredients[i].amount)
			{
				return false;
			}
		}
		return true;
	}

	public void Close()
	{
		if (!configuring)
		{
			ClearProduct(true);
			Show(false);
		}
	}
}
