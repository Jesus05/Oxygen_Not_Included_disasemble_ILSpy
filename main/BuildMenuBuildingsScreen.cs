using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuBuildingsScreen : KIconToggleMenu
{
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

	private class UserData
	{
		public BuildingDef def;

		public PlanScreen.RequirementsState requirementsState;

		public UserData(BuildingDef def, PlanScreen.RequirementsState state)
		{
			this.def = def;
			requirementsState = state;
		}
	}

	[SerializeField]
	private Image focusIndicator;

	[SerializeField]
	private Color32 focusedColour;

	[SerializeField]
	private Color32 unfocusedColour;

	public Action<BuildingDef> onBuildingSelected;

	[SerializeField]
	private LocText titleLabel;

	[SerializeField]
	private BuildingToolTipSettings buildingToolTipSettings;

	[SerializeField]
	private LayoutElement contentSizeLayout;

	[SerializeField]
	private GridLayoutGroup gridSizer;

	[SerializeField]
	private Sprite Overlay_NeedTech;

	[SerializeField]
	private Material defaultUIMaterial;

	[SerializeField]
	private Material desaturatedUIMaterial;

	private BuildingDef selectedBuilding;

	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateBuildableStates();
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
		base.onSelect += OnClickBuilding;
		Game.Instance.Subscribe(-1190690038, OnBuildToolDeactivated);
	}

	public void Configure(HashedString category, IList<BuildMenu.BuildingInfo> building_infos)
	{
		ClearButtons();
		SetHasFocus(true);
		List<ToggleInfo> list = new List<ToggleInfo>();
		string text = HashCache.Get().Get(category).ToUpper();
		text = text.Replace(" ", "");
		titleLabel.text = Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".BUILDMENUTITLE");
		foreach (BuildMenu.BuildingInfo building_info in building_infos)
		{
			BuildMenu.BuildingInfo current = building_info;
			BuildingDef def = Assets.GetBuildingDef(current.id);
			if (def.ShowInBuildMenu && !def.Deprecated)
			{
				ToggleInfo item = new ToggleInfo(def.Name, new UserData(def, PlanScreen.RequirementsState.Tech), def.HotKey, () => def.GetUISprite("ui", false));
				list.Add(item);
			}
		}
		Setup(list);
		for (int i = 0; i < toggleInfo.Count; i++)
		{
			RefreshToggle(toggleInfo[i]);
		}
		int num = 0;
		IEnumerator enumerator2 = gridSizer.transform.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				Transform transform = (Transform)enumerator2.Current;
				if (transform.gameObject.activeSelf)
				{
					num++;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator2 as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		gridSizer.constraintCount = Mathf.Min(num, 3);
		int num2 = Mathf.Min(num, gridSizer.constraintCount);
		int num3 = (num + gridSizer.constraintCount - 1) / gridSizer.constraintCount;
		int num4 = num2 - 1;
		int num5 = num3 - 1;
		float num6 = (float)num2;
		Vector2 cellSize = gridSizer.cellSize;
		float num7 = num6 * cellSize.x;
		float num8 = (float)num4;
		Vector2 spacing = gridSizer.spacing;
		float x = num7 + num8 * spacing.x + (float)gridSizer.padding.left + (float)gridSizer.padding.right;
		float num9 = (float)num3;
		Vector2 cellSize2 = gridSizer.cellSize;
		float num10 = num9 * cellSize2.y;
		float num11 = (float)num5;
		Vector2 spacing2 = gridSizer.spacing;
		Vector2 vector = new Vector2(x, num10 + num11 * spacing2.y + (float)gridSizer.padding.top + (float)gridSizer.padding.bottom);
		contentSizeLayout.minWidth = vector.x;
		contentSizeLayout.minHeight = vector.y;
	}

	private void ConfigureToolTip(ToolTip tooltip, BuildingDef def)
	{
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(def.Name, buildingToolTipSettings.BuildButtonName);
		tooltip.AddMultiStringTooltip(def.Effect, buildingToolTipSettings.BuildButtonDescription);
	}

	public void CloseRecipe(bool playSound = false)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		ToolMenu.Instance.ClearSelection();
		DeactivateBuildTools();
		if ((UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)PrebuildTool.Instance)
		{
			SelectTool.Instance.Activate();
		}
		selectedBuilding = null;
		onBuildingSelected(selectedBuilding);
	}

	private void RefreshToggle(ToggleInfo info)
	{
		if (info != null && !((UnityEngine.Object)info.toggle == (UnityEngine.Object)null))
		{
			UserData userData = info.userData as UserData;
			BuildingDef def = userData.def;
			TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
			bool flag = DebugHandler.InstantBuildMode || techItem == null || techItem.IsComplete();
			bool flag2 = flag || techItem == null || techItem.parentTech.ArePrerequisitesComplete();
			KToggle toggle = info.toggle;
			if (toggle.gameObject.activeSelf != flag2)
			{
				toggle.gameObject.SetActive(flag2);
			}
			if (!((UnityEngine.Object)toggle.bgImage == (UnityEngine.Object)null))
			{
				Image image = toggle.bgImage.GetComponentsInChildren<Image>()[1];
				Sprite sprite = image.sprite = def.GetUISprite("ui", false);
				image.SetNativeSize();
				image.rectTransform().sizeDelta /= 4f;
				ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
				component.ClearMultiStringTooltip();
				string text = def.Name;
				string effect = def.Effect;
				if (def.HotKey != Action.NumActions)
				{
					text = GameUtil.AppendHotkeyString(text, def.HotKey);
				}
				component.AddMultiStringTooltip(text, buildingToolTipSettings.BuildButtonName);
				component.AddMultiStringTooltip(effect, buildingToolTipSettings.BuildButtonDescription);
				LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
				if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
				{
					componentInChildren.text = def.Name;
				}
				PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
				ImageToggleState.State state = (requirementsState == PlanScreen.RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled;
				state = ((!((UnityEngine.Object)def == (UnityEngine.Object)selectedBuilding) || (requirementsState != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode)) ? ((requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled) : ImageToggleState.State.Active);
				if ((UnityEngine.Object)def == (UnityEngine.Object)selectedBuilding && state == ImageToggleState.State.Disabled)
				{
					state = ImageToggleState.State.DisabledActive;
				}
				else if (state == ImageToggleState.State.Disabled)
				{
					state = ImageToggleState.State.Disabled;
				}
				toggle.GetComponent<ImageToggleState>().SetState(state);
				Material material;
				Color color;
				if (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode)
				{
					material = defaultUIMaterial;
					color = Color.white;
				}
				else
				{
					material = desaturatedUIMaterial;
					Color color2;
					if (flag)
					{
						color2 = new Color(1f, 1f, 1f, 0.6f);
					}
					else
					{
						Color color4 = image.color = new Color(1f, 1f, 1f, 0.15f);
						color2 = color4;
					}
					color = color2;
				}
				if ((UnityEngine.Object)image.material != (UnityEngine.Object)material)
				{
					image.material = material;
					image.color = color;
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
				else if (requirementsState != PlanScreen.RequirementsState.Complete)
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

	public void ClearUI()
	{
		Show(false);
		ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KToggle toggle in toggles)
		{
			toggle.gameObject.SetActive(false);
			toggle.gameObject.transform.SetParent(null);
			UnityEngine.Object.DestroyImmediate(toggle.gameObject);
		}
		if (toggles != null)
		{
			toggles.Clear();
		}
		if (toggleInfo != null)
		{
			toggleInfo.Clear();
		}
	}

	private void OnClickBuilding(ToggleInfo toggle_info)
	{
		UserData userData = toggle_info.userData as UserData;
		OnSelectBuilding(userData.def);
	}

	private void OnSelectBuilding(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
		if (requirementsState == PlanScreen.RequirementsState.Complete || requirementsState == PlanScreen.RequirementsState.Materials)
		{
			if ((UnityEngine.Object)def != (UnityEngine.Object)selectedBuilding)
			{
				selectedBuilding = def;
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			}
			else
			{
				selectedBuilding = null;
				ClearSelection();
				CloseRecipe(true);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
			}
		}
		else
		{
			selectedBuilding = null;
			ClearSelection();
			CloseRecipe(true);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		onBuildingSelected(selectedBuilding);
	}

	public void UpdateBuildableStates()
	{
		if (toggleInfo != null && toggleInfo.Count > 0)
		{
			BuildingDef buildingDef = null;
			foreach (ToggleInfo item in toggleInfo)
			{
				RefreshToggle(item);
				UserData userData = item.userData as UserData;
				BuildingDef def = userData.def;
				if (!def.Deprecated)
				{
					PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
					if (requirementsState != userData.requirementsState)
					{
						if ((UnityEngine.Object)def == (UnityEngine.Object)BuildMenu.Instance.SelectedBuildingDef)
						{
							buildingDef = def;
						}
						RefreshToggle(item);
						userData.requirementsState = requirementsState;
					}
				}
			}
			if ((UnityEngine.Object)buildingDef != (UnityEngine.Object)null)
			{
				BuildMenu.Instance.RefreshProductInfoScreen(buildingDef);
			}
		}
	}

	private void OnResearchComplete(object data)
	{
		UpdateBuildableStates();
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if ((UnityEngine.Object)activeTool != (UnityEngine.Object)null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type) || typeof(PrebuildTool).IsAssignableFrom(type))
			{
				activeTool.DeactivateTool(null);
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (mouseOver && ConsumeMouseScroll && !e.TryConsume(Action.ZoomIn) && !e.TryConsume(Action.ZoomOut))
		{
			goto IL_0033;
		}
		goto IL_0033;
		IL_0033:
		if (HasFocus)
		{
			if (e.TryConsume(Action.Escape))
			{
				Game.Instance.Trigger(288942073, null);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			}
			else
			{
				base.OnKeyDown(e);
				if (!e.Consumed)
				{
					Action action = e.GetAction();
					if (action >= Action.BUILD_MENU_START_INTERCEPT)
					{
						e.TryConsume(action);
					}
				}
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (HasFocus)
		{
			if ((UnityEngine.Object)selectedBuilding != (UnityEngine.Object)null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
				Game.Instance.Trigger(288942073, null);
			}
			else
			{
				base.OnKeyUp(e);
				if (!e.Consumed)
				{
					Action action = e.GetAction();
					if (action >= Action.BUILD_MENU_START_INTERCEPT)
					{
						e.TryConsume(action);
					}
				}
			}
		}
	}

	public override void Close()
	{
		ToolMenu.Instance.ClearSelection();
		DeactivateBuildTools();
		if ((UnityEngine.Object)PlayerController.Instance.ActiveTool == (UnityEngine.Object)PrebuildTool.Instance)
		{
			SelectTool.Instance.Activate();
		}
		selectedBuilding = null;
		ClearButtons();
		base.gameObject.SetActive(false);
	}

	public override void SetHasFocus(bool has_focus)
	{
		base.SetHasFocus(has_focus);
		if ((UnityEngine.Object)focusIndicator != (UnityEngine.Object)null)
		{
			focusIndicator.color = ((!has_focus) ? unfocusedColour : focusedColour);
		}
	}

	private void OnBuildToolDeactivated(object data)
	{
		CloseRecipe(false);
	}
}
