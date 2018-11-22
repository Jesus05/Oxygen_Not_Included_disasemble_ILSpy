using FMOD.Studio;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
	public struct PlanInfo
	{
		public HashedString category;

		public bool hideIfNotResearched;

		public object data;

		public PlanInfo(HashedString category, bool hideIfNotResearched, object data)
		{
			this.category = category;
			this.hideIfNotResearched = hideIfNotResearched;
			this.data = data;
		}
	}

	[Serializable]
	public struct BuildingToolTipSettings
	{
		public TextStyleSetting BuildButtonName;

		public TextStyleSetting BuildButtonDescription;

		public TextStyleSetting MaterialRequirement;

		public TextStyleSetting ResearchRequirement;
	}

	[Serializable]
	public struct BuildingNameTextSetting
	{
		public TextStyleSetting ActiveSelected;

		public TextStyleSetting ActiveDeselected;

		public TextStyleSetting InactiveSelected;

		public TextStyleSetting InactiveDeselected;
	}

	private struct ToggleEntry
	{
		public ToggleInfo toggleInfo;

		public HashedString planCategory;

		public List<BuildingDef> buildingDefs;

		public List<Tag> pendingResearchAttentions;

		private List<TechItem> requiredTechItems;

		public ImageToggleState[] toggleImages;

		public bool hideIfNotResearched;

		public ToggleEntry(ToggleInfo toggle_info, HashedString plan_category, List<BuildingDef> building_defs, bool hideIfNotResearched)
		{
			toggleInfo = toggle_info;
			planCategory = plan_category;
			buildingDefs = building_defs;
			this.hideIfNotResearched = hideIfNotResearched;
			pendingResearchAttentions = new List<Tag>();
			requiredTechItems = new List<TechItem>();
			toggleImages = null;
			foreach (BuildingDef building_def in building_defs)
			{
				TechItem techItem = Db.Get().TechItems.TryGet(building_def.PrefabID);
				if (techItem == null)
				{
					requiredTechItems.Clear();
					break;
				}
				if (!requiredTechItems.Contains(techItem))
				{
					requiredTechItems.Add(techItem);
				}
			}
		}

		public bool AreAnyRequiredTechItemsAvailable()
		{
			if (requiredTechItems.Count != 0)
			{
				foreach (TechItem requiredTechItem in requiredTechItems)
				{
					if (TechRequirementsUpcoming(requiredTechItem))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		public void CollectToggleImages()
		{
			toggleImages = toggleInfo.toggle.gameObject.GetComponents<ImageToggleState>();
		}
	}

	public enum RequirementsState
	{
		Tech,
		Materials,
		Complete
	}

	[SerializeField]
	private GameObject planButtonPrefab;

	[SerializeField]
	private GameObject recipeInfoScreenParent;

	[SerializeField]
	private GameObject productInfoScreenPrefab;

	private static Dictionary<HashedString, string> iconNameMap = new Dictionary<HashedString, string>
	{
		{
			CacheHashedString("Base"),
			"icon_category_base"
		},
		{
			CacheHashedString("Oxygen"),
			"icon_category_oxygen"
		},
		{
			CacheHashedString("Power"),
			"icon_category_electrical"
		},
		{
			CacheHashedString("Food"),
			"icon_category_food"
		},
		{
			CacheHashedString("Plumbing"),
			"icon_category_plumbing"
		},
		{
			CacheHashedString("HVAC"),
			"icon_category_ventilation"
		},
		{
			CacheHashedString("Refining"),
			"icon_category_refinery"
		},
		{
			CacheHashedString("Medical"),
			"icon_category_medical"
		},
		{
			CacheHashedString("Furniture"),
			"icon_category_furniture"
		},
		{
			CacheHashedString("Equipment"),
			"icon_category_misc"
		},
		{
			CacheHashedString("Utilities"),
			"icon_category_utilities"
		},
		{
			CacheHashedString("Automation"),
			"icon_category_automation"
		},
		{
			CacheHashedString("Conveyance"),
			"icon_category_shipping"
		},
		{
			CacheHashedString("Rocketry"),
			"icon_category_rocketry"
		}
	};

	private Dictionary<ToggleInfo, bool> CategoryInteractive = new Dictionary<ToggleInfo, bool>();

	private ProductInfoScreen productInfoScreen;

	[SerializeField]
	public BuildingToolTipSettings buildingToolTipSettings;

	public BuildingNameTextSetting buildingNameTextSettings;

	private ToggleInfo activeCategoryInfo;

	private Dictionary<BuildingDef, KToggle> ActiveToggles = new Dictionary<BuildingDef, KToggle>();

	private float timeSinceNotificationPing = 0f;

	private float notificationPingExpire = 0.5f;

	private float specialNotificationEmbellishDelay = 8f;

	private int notificationPingCount = 0;

	private GameObject selectedBuildingGameObject;

	public Transform GroupsTransform;

	public Sprite Overlay_NeedTech;

	public RectTransform buildingGroupsRoot;

	public RectTransform BuildButtonBGPanel;

	public RectTransform BuildingGroupContentsRect;

	public Sprite defaultBuildingIconSprite;

	public Material defaultUIMaterial;

	public Material desaturatedUIMaterial;

	public LocText PlanCategoryLabel;

	private List<ToggleEntry> toggleEntries = new List<ToggleEntry>();

	private int ignoreToolChangeMessages = 0;

	private Dictionary<Def, RequirementsState> buildableDefs = new Dictionary<Def, RequirementsState>();

	[SerializeField]
	private TextStyleSetting[] CategoryLabelTextStyles;

	private float initTime;

	private Dictionary<Tag, HashedString> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private int buildable_state_update_idx = 0;

	private int building_button_refresh_idx = 0;

	private float buildGrid_bg_width = 274f;

	private float buildGrid_bg_borderHeight = 32f;

	private float buildGrid_bg_rowHeight;

	private int buildGrid_maxRowsBeforeScroll = 5;

	public static PlanScreen Instance
	{
		get;
		private set;
	}

	public static Dictionary<HashedString, string> IconNameMap => iconNameMap;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private static HashedString CacheHashedString(string str)
	{
		return HashCache.Get().Add(str);
	}

	public override float GetSortKey()
	{
		return 2f;
	}

	public RequirementsState BuildableState(BuildingDef def)
	{
		RequirementsState value = RequirementsState.Materials;
		if (buildableDefs.TryGetValue(def, out value))
		{
			goto IL_0018;
		}
		goto IL_0018;
		IL_0018:
		return value;
	}

	protected override void OnPrefabInit()
	{
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.OnPrefabInit();
			productInfoScreen = Util.KInstantiateUI<ProductInfoScreen>(productInfoScreenPrefab, recipeInfoScreenParent, true);
			productInfoScreen.rectTransform().pivot = new Vector2(0f, 0f);
			productInfoScreen.rectTransform().SetLocalPosition(new Vector3(280f, 0f, 0f));
			productInfoScreen.onElementsFullySelected = OnRecipeElementsFullySelected;
			Game.Instance.Subscribe(-107300940, OnResearchComplete);
			Game.Instance.Subscribe(1174281782, OnActiveToolChanged);
		}
		buildingGroupsRoot.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		initTime = KTime.Instance.UnscaledGameTime;
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			Instance = this;
			base.onSelect += OnClickCategory;
			Refresh();
			foreach (KToggle toggle in toggles)
			{
				toggle.group = GetComponent<ToggleGroup>();
			}
			GetBuildableStates(true);
			Game.Instance.Subscribe(288942073, OnUIClear);
		}
	}

	public void Refresh()
	{
		List<ToggleInfo> list = new List<ToggleInfo>();
		if (tagCategoryMap == null)
		{
			int building_index = 0;
			tagCategoryMap = new Dictionary<Tag, HashedString>();
			tagOrderMap = new Dictionary<Tag, int>();
			if (TUNING.BUILDINGS.PLANORDER.Count > 12)
			{
				Output.LogWarning("Insufficient keys to cover root plan menu", "Max of 12 keys supported but TUNING.BUILDINGS.PLANORDER has " + TUNING.BUILDINGS.PLANORDER.Count);
			}
			toggleEntries.Clear();
			for (int i = 0; i < TUNING.BUILDINGS.PLANORDER.Count; i++)
			{
				PlanInfo planInfo = TUNING.BUILDINGS.PLANORDER[i];
				Action hotkey = (Action)((i >= 12) ? 232 : (36 + i));
				string icon = iconNameMap[planInfo.category];
				string str = HashCache.Get().Get(planInfo.category).ToUpper();
				ToggleInfo toggleInfo = new ToggleInfo(UI.StripLinkFormatting(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + str + ".NAME")), icon, planInfo.category, hotkey, Strings.Get("STRINGS.UI.BUILDCATEGORIES." + str + ".TOOLTIP"), "");
				list.Add(toggleInfo);
				PopulateOrderInfo(planInfo.category, planInfo.data, tagCategoryMap, tagOrderMap, ref building_index);
				List<BuildingDef> list2 = new List<BuildingDef>();
				foreach (BuildingDef buildingDef in Assets.BuildingDefs)
				{
					HashedString value;
					if (!buildingDef.Deprecated && tagCategoryMap.TryGetValue(buildingDef.Tag, out value) && !(value != planInfo.category))
					{
						list2.Add(buildingDef);
					}
				}
				toggleEntries.Add(new ToggleEntry(toggleInfo, planInfo.category, list2, planInfo.hideIfNotResearched));
			}
			Setup(list);
			toggles.ForEach(delegate(KToggle to)
			{
				ImageToggleState[] components = to.GetComponents<ImageToggleState>();
				ImageToggleState[] array = components;
				foreach (ImageToggleState imageToggleState in array)
				{
					if ((UnityEngine.Object)imageToggleState.TargetImage.sprite != (UnityEngine.Object)null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
					{
						imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
					}
				}
				to.GetComponent<KToggle>().soundPlayer.Enabled = false;
			});
			for (int j = 0; j < toggleEntries.Count; j++)
			{
				ToggleEntry value2 = toggleEntries[j];
				value2.CollectToggleImages();
				toggleEntries[j] = value2;
			}
		}
	}

	private static void PopulateOrderInfo(HashedString category, object data, Dictionary<Tag, HashedString> category_map, Dictionary<Tag, int> order_map, ref int building_index)
	{
		if (data.GetType() == typeof(PlanInfo))
		{
			PlanInfo planInfo = (PlanInfo)data;
			PopulateOrderInfo(planInfo.category, planInfo.data, category_map, order_map, ref building_index);
		}
		else
		{
			IList<string> list = (IList<string>)data;
			foreach (string item in list)
			{
				Tag key = new Tag(item);
				category_map[key] = category;
				order_map[key] = building_index;
				building_index++;
			}
		}
	}

	protected override void OnCmpEnable()
	{
		Refresh();
		productInfoScreen.Show(false);
	}

	protected override void OnCmpDisable()
	{
		ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KeyValuePair<BuildingDef, KToggle> activeToggle in ActiveToggles)
		{
			activeToggle.Value.gameObject.SetActive(false);
			activeToggle.Value.transform.SetParent(null);
			UnityEngine.Object.DestroyImmediate(activeToggle.Value.gameObject);
		}
		ActiveToggles.Clear();
	}

	private void OnSelectBuilding(GameObject button_go, BuildingDef def)
	{
		if ((UnityEngine.Object)button_go == (UnityEngine.Object)null)
		{
			Output.LogWithObj(base.gameObject, "Button gameObject is null");
		}
		else if ((UnityEngine.Object)button_go == (UnityEngine.Object)selectedBuildingGameObject)
		{
			CloseRecipe(true);
		}
		else
		{
			ignoreToolChangeMessages++;
			selectedBuildingGameObject = button_go;
			currentlySelectedToggle = button_go.GetComponent<KToggle>();
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			HashedString category = tagCategoryMap[def.Tag];
			if (GetToggleEntryForCategory(category, out ToggleEntry toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
			{
				toggleEntry.pendingResearchAttentions.Remove(def.Tag);
				button_go.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
				if (toggleEntry.pendingResearchAttentions.Count == 0)
				{
					toggleEntry.toggleInfo.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
				}
			}
			productInfoScreen.ClearProduct(false);
			ToolMenu.Instance.ClearSelection();
			PrebuildTool.Instance.Activate(def, BuildableState(def));
			productInfoScreen.Show(true);
			productInfoScreen.ConfigureScreen(def);
			ignoreToolChangeMessages--;
		}
	}

	private void GetBuildableStates(bool force_update)
	{
		if (Assets.BuildingDefs != null && Assets.BuildingDefs.Count != 0)
		{
			if (timeSinceNotificationPing < specialNotificationEmbellishDelay)
			{
				timeSinceNotificationPing += Time.unscaledDeltaTime;
			}
			if (timeSinceNotificationPing >= notificationPingExpire)
			{
				notificationPingCount = 0;
			}
			int num = 10;
			if (force_update)
			{
				num = Assets.BuildingDefs.Count;
				buildable_state_update_idx = 0;
			}
			ListPool<HashedString, PlanScreen>.PooledList pooledList = ListPool<HashedString, PlanScreen>.Allocate();
			for (int i = 0; i < num; i++)
			{
				buildable_state_update_idx = (buildable_state_update_idx + 1) % Assets.BuildingDefs.Count;
				BuildingDef buildingDef = Assets.BuildingDefs[buildable_state_update_idx];
				if (!buildingDef.Deprecated && tagCategoryMap.TryGetValue(buildingDef.Tag, out HashedString value))
				{
					RequirementsState requirementsState = RequirementsState.Complete;
					if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
					{
						if (!Db.Get().TechItems.IsTechItemComplete(buildingDef.PrefabID))
						{
							requirementsState = RequirementsState.Tech;
						}
						else if (!ProductInfoScreen.MaterialsMet(buildingDef.CraftRecipe))
						{
							requirementsState = RequirementsState.Materials;
						}
					}
					if (!buildableDefs.ContainsKey(buildingDef))
					{
						buildableDefs.Add(buildingDef, requirementsState);
					}
					else if (buildableDefs[buildingDef] != requirementsState)
					{
						buildableDefs[buildingDef] = requirementsState;
						if ((UnityEngine.Object)productInfoScreen.currentDef == (UnityEngine.Object)buildingDef)
						{
							ignoreToolChangeMessages++;
							productInfoScreen.ClearProduct(false);
							productInfoScreen.Show(true);
							productInfoScreen.ConfigureScreen(buildingDef);
							ignoreToolChangeMessages--;
						}
						if (requirementsState == RequirementsState.Complete)
						{
							foreach (ToggleInfo item in toggleInfo)
							{
								HashedString x = (HashedString)item.userData;
								if (x == value)
								{
									string text = "NotificationPing";
									Animator component = item.toggle.GetComponent<Animator>();
									if (!component.GetCurrentAnimatorStateInfo(0).IsTag(text) && !pooledList.Contains(value))
									{
										pooledList.Add(value);
										item.toggle.gameObject.GetComponent<Animator>().Play(text);
										if (KTime.Instance.UnscaledGameTime - initTime > 1.5f)
										{
											if (timeSinceNotificationPing >= specialNotificationEmbellishDelay)
											{
												string sound = GlobalAssets.GetSound("NewBuildable_Embellishment", false);
												if (sound != null)
												{
													EventInstance instance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
													SoundEvent.EndOneShot(instance);
												}
											}
											string sound2 = GlobalAssets.GetSound("NewBuildable", false);
											if (sound2 != null)
											{
												EventInstance instance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition());
												instance2.setParameterValue("playCount", (float)notificationPingCount);
												SoundEvent.EndOneShot(instance2);
											}
										}
										timeSinceNotificationPing = 0f;
										notificationPingCount++;
									}
								}
							}
						}
					}
				}
			}
			pooledList.Recycle();
		}
	}

	private void SetCategoryButtonState()
	{
		foreach (ToggleEntry toggleEntry in toggleEntries)
		{
			ToggleEntry current = toggleEntry;
			ToggleInfo toggleInfo = current.toggleInfo;
			toggleInfo.toggle.ActivateFlourish(activeCategoryInfo != null && toggleInfo.userData == activeCategoryInfo.userData);
			bool flag = false;
			bool flag2 = true;
			if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
			{
				flag = true;
				flag2 = false;
			}
			else
			{
				foreach (BuildingDef buildingDef in current.buildingDefs)
				{
					RequirementsState requirementsState = BuildableState(buildingDef);
					if (requirementsState == RequirementsState.Complete)
					{
						flag = true;
						flag2 = false;
						break;
					}
				}
				if (flag2 && current.AreAnyRequiredTechItemsAvailable())
				{
					flag2 = false;
				}
			}
			CategoryInteractive[toggleInfo] = !flag2;
			GameObject gameObject = toggleInfo.toggle.fgImage.transform.Find("ResearchIcon").gameObject;
			if (!flag)
			{
				if (flag2 && current.hideIfNotResearched)
				{
					toggleInfo.toggle.gameObject.SetActive(false);
				}
				else if (flag2)
				{
					toggleInfo.toggle.gameObject.SetActive(true);
					toggleInfo.toggle.fgImage.SetAlpha(0.2509804f);
					gameObject.gameObject.SetActive(true);
				}
				else
				{
					toggleInfo.toggle.gameObject.SetActive(true);
					toggleInfo.toggle.fgImage.SetAlpha(1f);
					gameObject.gameObject.SetActive(false);
				}
				ImageToggleState.State state = (activeCategoryInfo != null && toggleInfo.userData == activeCategoryInfo.userData) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Disabled;
				ImageToggleState[] toggleImages = current.toggleImages;
				foreach (ImageToggleState imageToggleState in toggleImages)
				{
					imageToggleState.SetState(state);
				}
			}
			else
			{
				toggleInfo.toggle.gameObject.SetActive(true);
				toggleInfo.toggle.fgImage.SetAlpha(1f);
				gameObject.gameObject.SetActive(false);
				ImageToggleState.State state2 = (activeCategoryInfo == null || toggleInfo.userData != activeCategoryInfo.userData) ? ImageToggleState.State.Inactive : ImageToggleState.State.Active;
				ImageToggleState[] toggleImages2 = current.toggleImages;
				foreach (ImageToggleState imageToggleState2 in toggleImages2)
				{
					imageToggleState2.SetState(state2);
				}
			}
		}
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if ((UnityEngine.Object)activeTool != (UnityEngine.Object)null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
			{
				activeTool.DeactivateTool(null);
			}
		}
	}

	public void CloseRecipe(bool playSound = false)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		if (PlayerController.Instance.ActiveTool is PrebuildTool || PlayerController.Instance.ActiveTool is BuildTool)
		{
			ToolMenu.Instance.ClearSelection();
		}
		DeactivateBuildTools();
		if ((UnityEngine.Object)productInfoScreen != (UnityEngine.Object)null)
		{
			productInfoScreen.ClearProduct(true);
		}
		if (activeCategoryInfo != null)
		{
			UpdateBuildingButtonList(activeCategoryInfo);
		}
		selectedBuildingGameObject = null;
	}

	private void CloseCategoryPanel(bool playSound = true)
	{
		activeCategoryInfo = null;
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
		buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Collapse(delegate
		{
			ClearButtons();
			buildingGroupsRoot.gameObject.SetActive(false);
		});
		PlanCategoryLabel.text = "";
	}

	private void OnClickCategory(ToggleInfo toggle_info)
	{
		CloseRecipe(false);
		if (!CategoryInteractive.ContainsKey(toggle_info) || !CategoryInteractive[toggle_info])
		{
			CloseCategoryPanel(false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		else
		{
			if (activeCategoryInfo == toggle_info)
			{
				CloseCategoryPanel(true);
			}
			else
			{
				OpenCategoryPanel(toggle_info, true);
			}
			ConfigurePanelSize();
			SetScrollPoint(0f);
		}
	}

	private void OpenCategoryPanel(ToggleInfo toggle_info, bool play_sound = true)
	{
		HashedString plan_category = (HashedString)toggle_info.userData;
		ClearButtons();
		buildingGroupsRoot.gameObject.SetActive(true);
		activeCategoryInfo = toggle_info;
		if (play_sound)
		{
			UISounds.PlaySound(UISounds.Sound.ClickObject);
		}
		BuildButtonList(plan_category, GroupsTransform.gameObject);
		PlanCategoryLabel.text = activeCategoryInfo.text.ToUpper();
		buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Expand(null);
	}

	public void OpenCategoryByName(string category)
	{
		if (GetToggleEntryForCategory(category, out ToggleEntry toggleEntry))
		{
			OpenCategoryPanel(toggleEntry.toggleInfo, false);
		}
	}

	private void UpdateBuildingButtonList(ToggleInfo toggle_info)
	{
		KToggle toggle = toggle_info.toggle;
		if ((UnityEngine.Object)toggle == (UnityEngine.Object)null)
		{
			foreach (ToggleInfo item in toggleInfo)
			{
				if (item.userData == toggle_info.userData)
				{
					toggle = item.toggle;
				}
			}
		}
		int num = 2;
		if ((UnityEngine.Object)toggle != (UnityEngine.Object)null && ActiveToggles.Count != 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (building_button_refresh_idx >= ActiveToggles.Count)
				{
					building_button_refresh_idx = 0;
				}
				RefreshBuildingButton(ActiveToggles.ElementAt(building_button_refresh_idx).Key, ActiveToggles.ElementAt(building_button_refresh_idx).Value, (HashedString)toggle_info.userData);
				building_button_refresh_idx++;
			}
		}
		if (productInfoScreen.gameObject.activeSelf)
		{
			productInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		GetBuildableStates(false);
		SetCategoryButtonState();
		if (activeCategoryInfo != null)
		{
			UpdateBuildingButtonList(activeCategoryInfo);
		}
	}

	private void BuildButtonList(HashedString plan_category, GameObject parent)
	{
		IOrderedEnumerable<BuildingDef> orderedEnumerable = from def in Assets.BuildingDefs
		where tagCategoryMap.ContainsKey(def.Tag) && tagCategoryMap[def.Tag] == plan_category && !def.Deprecated
		orderby tagOrderMap[def.Tag]
		select def;
		ActiveToggles.Clear();
		int num = 0;
		string plan_category2 = plan_category.ToString();
		foreach (BuildingDef item in orderedEnumerable)
		{
			if (item.ShowInBuildMenu)
			{
				CreateButton(item, parent, plan_category2, num);
				num++;
			}
		}
	}

	private void ConfigurePanelSize()
	{
		GridLayoutGroup component = GroupsTransform.GetComponent<GridLayoutGroup>();
		Vector2 cellSize = component.cellSize;
		float y = cellSize.y;
		Vector2 spacing = component.spacing;
		buildGrid_bg_rowHeight = y + spacing.y;
		int num = GroupsTransform.childCount;
		for (int i = 0; i < GroupsTransform.childCount; i++)
		{
			if (!GroupsTransform.GetChild(i).gameObject.activeSelf)
			{
				num--;
			}
		}
		int num2 = Mathf.CeilToInt((float)num / 3f);
		BuildingGroupContentsRect.GetComponent<ScrollRect>().verticalScrollbar.gameObject.SetActive(num2 >= 4);
		buildingGroupsRoot.sizeDelta = new Vector2(buildGrid_bg_width, buildGrid_bg_borderHeight + (float)Mathf.Clamp(num2, 0, buildGrid_maxRowsBeforeScroll) * buildGrid_bg_rowHeight);
	}

	private void SetScrollPoint(float targetY)
	{
		RectTransform buildingGroupContentsRect = BuildingGroupContentsRect;
		Vector2 anchoredPosition = BuildingGroupContentsRect.anchoredPosition;
		buildingGroupContentsRect.anchoredPosition = new Vector2(anchoredPosition.x, targetY);
	}

	private GameObject CreateButton(BuildingDef def, GameObject parent, string plan_category, int btnIndex)
	{
		GameObject button_go = Util.KInstantiateUI(planButtonPrefab, parent, true);
		button_go.name = UI.StripLinkFormatting(def.name) + " Group:" + plan_category;
		KToggle componentInChildren = button_go.GetComponentInChildren<KToggle>();
		componentInChildren.soundPlayer.Enabled = false;
		ActiveToggles.Add(def, componentInChildren);
		RefreshBuildingButton(def, componentInChildren, plan_category);
		componentInChildren.onClick += delegate
		{
			OnSelectBuilding(button_go, def);
		};
		return button_go;
	}

	private static bool TechRequirementsMet(TechItem techItem)
	{
		return DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem == null || techItem.IsComplete();
	}

	private static bool TechRequirementsUpcoming(TechItem techItem)
	{
		return TechRequirementsMet(techItem);
	}

	private bool GetToggleEntryForCategory(HashedString category, out ToggleEntry toggleEntry)
	{
		foreach (ToggleEntry toggleEntry2 in toggleEntries)
		{
			ToggleEntry current = toggleEntry2;
			if (current.planCategory == category)
			{
				toggleEntry = current;
				return true;
			}
		}
		toggleEntry = default(ToggleEntry);
		return false;
	}

	public void RefreshBuildingButton(BuildingDef def, KToggle toggle, HashedString buildingCategory)
	{
		if (!((UnityEngine.Object)toggle == (UnityEngine.Object)null))
		{
			if (GetToggleEntryForCategory(buildingCategory, out ToggleEntry toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
			{
				toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
			}
			TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
			bool flag = TechRequirementsMet(techItem);
			bool flag2 = TechRequirementsUpcoming(techItem);
			if (toggle.gameObject.activeSelf != flag2)
			{
				toggle.gameObject.SetActive(flag2);
				ConfigurePanelSize();
				SetScrollPoint(0f);
			}
			if (toggle.gameObject.activeInHierarchy && !((UnityEngine.Object)toggle.bgImage == (UnityEngine.Object)null))
			{
				Image image = toggle.bgImage.GetComponentsInChildren<Image>()[1];
				Sprite uISprite = def.GetUISprite("ui", false);
				if ((UnityEngine.Object)uISprite == (UnityEngine.Object)null)
				{
					uISprite = defaultBuildingIconSprite;
				}
				image.sprite = uISprite;
				image.SetNativeSize();
				image.rectTransform().sizeDelta /= 4f;
				ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
				PositionTooltip(toggle, component);
				component.ClearMultiStringTooltip();
				string name = def.Name;
				string effect = def.Effect;
				component.AddMultiStringTooltip(name, buildingToolTipSettings.BuildButtonName);
				component.AddMultiStringTooltip(effect, buildingToolTipSettings.BuildButtonDescription);
				LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
				if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
				{
					componentInChildren.text = def.Name;
				}
				ImageToggleState.State state = (BuildableState(def) == RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled;
				state = ((!((UnityEngine.Object)toggle.gameObject == (UnityEngine.Object)selectedBuildingGameObject) || (BuildableState(def) != RequirementsState.Complete && !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)) ? ((BuildableState(def) == RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled) : ImageToggleState.State.Active);
				if ((UnityEngine.Object)toggle.gameObject == (UnityEngine.Object)selectedBuildingGameObject && state == ImageToggleState.State.Disabled)
				{
					state = ImageToggleState.State.DisabledActive;
				}
				else if (state == ImageToggleState.State.Disabled)
				{
					state = ImageToggleState.State.Disabled;
				}
				toggle.GetComponent<ImageToggleState>().SetState(state);
				Material material = (BuildableState(def) != RequirementsState.Complete && !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive) ? desaturatedUIMaterial : defaultUIMaterial;
				if ((UnityEngine.Object)image.material != (UnityEngine.Object)material)
				{
					image.material = material;
					if ((UnityEngine.Object)material == (UnityEngine.Object)desaturatedUIMaterial)
					{
						if (flag)
						{
							image.color = new Color(1f, 1f, 1f, 0.6f);
						}
						else
						{
							image.color = new Color(1f, 1f, 1f, 0.15f);
						}
					}
					else
					{
						image.color = Color.white;
					}
				}
				Image fgImage = toggle.gameObject.GetComponent<KToggle>().fgImage;
				fgImage.gameObject.SetActive(false);
				if (!flag)
				{
					fgImage.sprite = Overlay_NeedTech;
					fgImage.gameObject.SetActive(true);
					string newString = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
					component.AddMultiStringTooltip("\n", buildingToolTipSettings.ResearchRequirement);
					component.AddMultiStringTooltip(newString, buildingToolTipSettings.ResearchRequirement);
				}
				else if (BuildableState(def) != RequirementsState.Complete)
				{
					fgImage.gameObject.SetActive(false);
					component.AddMultiStringTooltip("\n", buildingToolTipSettings.ResearchRequirement);
					string newString2 = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
					component.AddMultiStringTooltip(newString2, buildingToolTipSettings.ResearchRequirement);
					foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
					{
						string newString3 = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						component.AddMultiStringTooltip(newString3, buildingToolTipSettings.ResearchRequirement);
					}
					component.AddMultiStringTooltip("", buildingToolTipSettings.ResearchRequirement);
				}
			}
		}
	}

	private void PositionTooltip(KToggle toggle, ToolTip tip)
	{
		tip.overrideParentObject = ((!productInfoScreen.gameObject.activeSelf) ? buildingGroupsRoot : productInfoScreen.rectTransform());
	}

	private void SetMaterialTint(KToggle toggle, bool disabled)
	{
		SwapUIAnimationController component = toggle.GetComponent<SwapUIAnimationController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetState(!disabled);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (mouseOver && ConsumeMouseScroll && !e.TryConsume(Action.ZoomIn) && !e.TryConsume(Action.ZoomOut))
		{
			goto IL_0043;
		}
		goto IL_0043;
		IL_0043:
		if (toggles != null)
		{
			if (!e.Consumed && activeCategoryInfo != null && e.TryConsume(Action.Escape))
			{
				OnClickCategory(activeCategoryInfo);
				SelectTool.Instance.Activate();
				ClearSelection();
			}
			else if (!e.Consumed)
			{
				base.OnKeyDown(e);
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if ((UnityEngine.Object)selectedBuildingGameObject != (UnityEngine.Object)null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			CloseRecipe(false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
		else if (activeCategoryInfo != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			OnUIClear(null);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void OnRecipeElementsFullySelected()
	{
		BuildingDef buildingDef = null;
		foreach (KeyValuePair<BuildingDef, KToggle> activeToggle in ActiveToggles)
		{
			if ((UnityEngine.Object)activeToggle.Value == (UnityEngine.Object)currentlySelectedToggle)
			{
				buildingDef = activeToggle.Key;
				break;
			}
		}
		if ((UnityEngine.Object)buildingDef == (UnityEngine.Object)null)
		{
			Debug.Log("No def!", null);
		}
		if (buildingDef.isKAnimTile && buildingDef.isUtility)
		{
			IList<Element> getSelectedElementAsList = productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
			BaseUtilityBuildTool baseUtilityBuildTool = (!((UnityEngine.Object)buildingDef.BuildingComplete.GetComponent<Wire>() != (UnityEngine.Object)null)) ? ((BaseUtilityBuildTool)UtilityBuildTool.Instance) : ((BaseUtilityBuildTool)WireBuildTool.Instance);
			baseUtilityBuildTool.Activate(buildingDef, getSelectedElementAsList);
		}
		else
		{
			BuildTool.Instance.Activate(buildingDef, productInfoScreen.materialSelectionPanel.GetSelectedElementAsList, null);
		}
	}

	public void OnResearchComplete(object tech)
	{
		Tech tech2 = (Tech)tech;
		foreach (TechItem unlockedItem in tech2.unlockedItems)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(unlockedItem.Id);
			if ((UnityEngine.Object)buildingDef != (UnityEngine.Object)null)
			{
				HashedString category = tagCategoryMap[buildingDef.Tag];
				if (GetToggleEntryForCategory(category, out ToggleEntry toggleEntry))
				{
					toggleEntry.pendingResearchAttentions.Add(buildingDef.Tag);
					toggleEntry.toggleInfo.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
				}
			}
		}
	}

	private void OnUIClear(object data)
	{
		if (activeCategoryInfo != null)
		{
			selected = -1;
			OnClickCategory(activeCategoryInfo);
			SelectTool.Instance.Activate();
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
			SelectTool.Instance.Select(null, true);
		}
	}

	private void OnActiveToolChanged(object data)
	{
		if (data != null && ignoreToolChangeMessages <= 0)
		{
			Type type = data.GetType();
			if (!typeof(BuildTool).IsAssignableFrom(type) && !typeof(PrebuildTool).IsAssignableFrom(type) && !typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
			{
				CloseRecipe(false);
				CloseCategoryPanel(false);
			}
		}
	}

	public PrioritySetting GetBuildingPriority()
	{
		return productInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();
	}
}
