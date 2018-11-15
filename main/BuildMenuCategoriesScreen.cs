using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuCategoriesScreen : KIconToggleMenu
{
	private class UserData
	{
		public BuildMenu.Category category;

		public int depth;

		public PlanScreen.RequirementsState requirementsState;

		public ImageToggleState.State? currentToggleState;
	}

	public Action<BuildMenu.Category, int> onCategoryClicked;

	[SerializeField]
	public bool modalKeyInputBehaviour;

	[SerializeField]
	private Image focusIndicator;

	[SerializeField]
	private Color32 focusedColour;

	[SerializeField]
	private Color32 unfocusedColour;

	private IList<BuildMenu.Category> subcategories;

	private Dictionary<BuildMenu.Category, List<BuildingDef>> categorizedBuildingMap;

	private Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorizedCategoryMap;

	private BuildMenuBuildingsScreen buildingsScreen;

	private BuildMenu.Category category;

	private IList<BuildMenu.BuildingInfo> buildingInfos;

	private BuildMenu.Category selectedCategory = BuildMenu.Category.INVALID;

	public BuildMenu.Category Category => category;

	public override float GetSortKey()
	{
		return 7f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.onSelect += OnClickCategory;
	}

	public void Configure(BuildMenu.Category category, int depth, object data, Dictionary<BuildMenu.Category, List<BuildingDef>> categorized_building_map, Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorized_category_map, BuildMenuBuildingsScreen buildings_screen)
	{
		this.category = category;
		categorizedBuildingMap = categorized_building_map;
		categorizedCategoryMap = categorized_category_map;
		buildingsScreen = buildings_screen;
		List<ToggleInfo> list = new List<ToggleInfo>();
		if (typeof(IList<BuildMenu.BuildingInfo>).IsAssignableFrom(data.GetType()))
		{
			buildingInfos = (IList<BuildMenu.BuildingInfo>)data;
		}
		else if (typeof(IList<BuildMenu.DisplayInfo>).IsAssignableFrom(data.GetType()))
		{
			subcategories = new List<BuildMenu.Category>();
			IList<BuildMenu.DisplayInfo> list2 = (IList<BuildMenu.DisplayInfo>)data;
			foreach (BuildMenu.DisplayInfo item2 in list2)
			{
				BuildMenu.DisplayInfo current = item2;
				string iconName = current.iconName;
				string str = current.category.ToString().ToUpper();
				ToggleInfo item = new ToggleInfo(Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + str + ".NAME"), iconName, new UserData
				{
					category = current.category,
					depth = depth,
					requirementsState = PlanScreen.RequirementsState.Tech
				}, current.hotkey, Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + str + ".TOOLTIP"), string.Empty);
				list.Add(item);
				subcategories.Add(current.category);
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
		}
		UpdateBuildableStates(true);
	}

	private void OnClickCategory(ToggleInfo toggle_info)
	{
		UserData userData = (UserData)toggle_info.userData;
		PlanScreen.RequirementsState requirementsState = userData.requirementsState;
		if (requirementsState == PlanScreen.RequirementsState.Complete || requirementsState == PlanScreen.RequirementsState.Materials)
		{
			if (selectedCategory != userData.category)
			{
				selectedCategory = userData.category;
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			}
			else
			{
				selectedCategory = BuildMenu.Category.INVALID;
				ClearSelection();
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
			}
		}
		else
		{
			selectedCategory = BuildMenu.Category.INVALID;
			ClearSelection();
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		toggle_info.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
		if (onCategoryClicked != null)
		{
			onCategoryClicked(selectedCategory, userData.depth);
		}
	}

	private void UpdateButtonStates()
	{
		if (toggleInfo != null && toggleInfo.Count > 0)
		{
			foreach (ToggleInfo item in toggleInfo)
			{
				UserData userData = (UserData)item.userData;
				BuildMenu.Category category = userData.category;
				PlanScreen.RequirementsState categoryRequirements = GetCategoryRequirements(category);
				bool flag = categoryRequirements == PlanScreen.RequirementsState.Tech;
				item.toggle.gameObject.SetActive(!flag);
				switch (categoryRequirements)
				{
				case PlanScreen.RequirementsState.Complete:
				{
					ImageToggleState.State state2 = (selectedCategory == BuildMenu.Category.INVALID || category != selectedCategory) ? ImageToggleState.State.Inactive : ImageToggleState.State.Active;
					if (!userData.currentToggleState.HasValue || userData.currentToggleState.GetValueOrDefault() != state2)
					{
						userData.currentToggleState = state2;
						SetImageToggleState(item.toggle.gameObject, state2);
					}
					break;
				}
				case PlanScreen.RequirementsState.Materials:
				{
					item.toggle.fgImage.SetAlpha((!flag) ? 1f : 0.2509804f);
					ImageToggleState.State state = (selectedCategory != BuildMenu.Category.INVALID && category == selectedCategory) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Disabled;
					if (!userData.currentToggleState.HasValue || userData.currentToggleState.GetValueOrDefault() != state)
					{
						userData.currentToggleState = state;
						SetImageToggleState(item.toggle.gameObject, state);
					}
					break;
				}
				}
				GameObject gameObject = item.toggle.fgImage.transform.Find("ResearchIcon").gameObject;
				gameObject.gameObject.SetActive(flag);
			}
		}
	}

	private void SetImageToggleState(GameObject target, ImageToggleState.State state)
	{
		ImageToggleState[] components = target.GetComponents<ImageToggleState>();
		ImageToggleState[] array = components;
		foreach (ImageToggleState imageToggleState in array)
		{
			imageToggleState.SetState(state);
		}
	}

	private PlanScreen.RequirementsState GetCategoryRequirements(BuildMenu.Category category)
	{
		bool flag = true;
		bool flag2 = true;
		List<BuildMenu.Category> value2;
		if (categorizedBuildingMap.TryGetValue(category, out List<BuildingDef> value))
		{
			if (value.Count > 0)
			{
				foreach (BuildingDef item in value)
				{
					if (item.ShowInBuildMenu && !item.Deprecated)
					{
						PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(item);
						flag = (flag && requirementsState == PlanScreen.RequirementsState.Tech);
						flag2 = (flag2 && (requirementsState == PlanScreen.RequirementsState.Materials || requirementsState == PlanScreen.RequirementsState.Tech));
					}
				}
			}
		}
		else if (categorizedCategoryMap.TryGetValue(category, out value2))
		{
			foreach (BuildMenu.Category item2 in value2)
			{
				PlanScreen.RequirementsState categoryRequirements = GetCategoryRequirements(item2);
				flag = (flag && categoryRequirements == PlanScreen.RequirementsState.Tech);
				flag2 = (flag2 && (categoryRequirements == PlanScreen.RequirementsState.Materials || categoryRequirements == PlanScreen.RequirementsState.Tech));
			}
		}
		PlanScreen.RequirementsState result = (!flag) ? (flag2 ? PlanScreen.RequirementsState.Materials : PlanScreen.RequirementsState.Complete) : PlanScreen.RequirementsState.Tech;
		if (DebugHandler.InstantBuildMode)
		{
			result = PlanScreen.RequirementsState.Complete;
		}
		return result;
	}

	public void UpdateNotifications(ICollection<BuildMenu.Category> updated_categories)
	{
		if (toggleInfo != null)
		{
			UpdateBuildableStates(false);
			foreach (ToggleInfo item2 in toggleInfo)
			{
				UserData userData = (UserData)item2.userData;
				BuildMenu.Category item = userData.category;
				if (updated_categories.Contains(item))
				{
					item2.toggle.gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
				}
			}
		}
	}

	public override void Close()
	{
		base.Close();
		selectedCategory = BuildMenu.Category.INVALID;
		SetHasFocus(false);
		if (buildingInfos != null)
		{
			buildingsScreen.Close();
		}
	}

	[ContextMenu("ForceUpdateBuildableStates")]
	private void ForceUpdateBuildableStates()
	{
		UpdateBuildableStates(true);
	}

	public void UpdateBuildableStates(bool skip_flourish)
	{
		if (subcategories != null && subcategories.Count > 0)
		{
			UpdateButtonStates();
			foreach (ToggleInfo item in toggleInfo)
			{
				UserData userData = (UserData)item.userData;
				BuildMenu.Category category = userData.category;
				PlanScreen.RequirementsState categoryRequirements = GetCategoryRequirements(category);
				if (userData.requirementsState != categoryRequirements)
				{
					userData.requirementsState = categoryRequirements;
					item.userData = userData;
					if (!skip_flourish)
					{
						item.toggle.ActivateFlourish(false);
						string text = "NotificationPing";
						Animator component = item.toggle.GetComponent<Animator>();
						if (!component.GetCurrentAnimatorStateInfo(0).IsTag(text))
						{
							item.toggle.gameObject.GetComponent<Animator>().Play(text);
							BuildMenu.Instance.PlayNewBuildingSounds();
						}
					}
				}
			}
		}
		else
		{
			buildingsScreen.UpdateBuildableStates();
		}
	}

	protected override void OnShow(bool show)
	{
		if (buildingInfos != null)
		{
			if (show)
			{
				buildingsScreen.Configure(category, buildingInfos);
				buildingsScreen.Show(true);
			}
			else
			{
				buildingsScreen.Close();
			}
		}
		base.OnShow(show);
	}

	public override void ClearSelection()
	{
		selectedCategory = BuildMenu.Category.INVALID;
		base.ClearSelection();
		foreach (KToggle toggle in toggles)
		{
			toggle.isOn = false;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (modalKeyInputBehaviour)
		{
			if (HasFocus)
			{
				if (e.TryConsume(Action.Escape))
				{
					Game.Instance.Trigger(288942073, null);
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
		else
		{
			base.OnKeyDown(e);
			if (e.Consumed)
			{
				UpdateButtonStates();
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (modalKeyInputBehaviour)
		{
			if (HasFocus)
			{
				if (e.TryConsume(Action.Escape))
				{
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
		else
		{
			base.OnKeyUp(e);
		}
	}

	public override void SetHasFocus(bool has_focus)
	{
		base.SetHasFocus(has_focus);
		if ((UnityEngine.Object)focusIndicator != (UnityEngine.Object)null)
		{
			focusIndicator.color = ((!has_focus) ? unfocusedColour : focusedColour);
		}
	}
}
