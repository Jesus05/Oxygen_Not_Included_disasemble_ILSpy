using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CrewJobsScreen : CrewListScreen<CrewJobsEntry>
{
	public enum everyoneToggleState
	{
		off,
		mixed,
		on
	}

	public static CrewJobsScreen Instance;

	private Dictionary<Button, everyoneToggleState> EveryoneToggles = new Dictionary<Button, everyoneToggleState>();

	private KeyValuePair<Button, everyoneToggleState> EveryoneAllTaskToggle;

	public TextStyleSetting TextStyle_JobTooltip_Title;

	public TextStyleSetting TextStyle_JobTooltip_Description;

	public TextStyleSetting TextStyle_JobTooltip_RelevantAttributes;

	public Toggle SortEveryoneToggle;

	private List<ChoreGroup> choreGroups = new List<ChoreGroup>();

	private bool dirty;

	private float screenWidth = 0f;

	protected override void OnActivate()
	{
		Instance = this;
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			choreGroups.Add(resource);
		}
		base.OnActivate();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RefreshCrewPortraitContent();
		SortByPreviousSelected();
	}

	protected override void SpawnEntries()
	{
		base.SpawnEntries();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			GameObject gameObject = Util.KInstantiateUI(Prefab_CrewEntry, EntriesPanelTransform.gameObject, false);
			CrewJobsEntry component = gameObject.GetComponent<CrewJobsEntry>();
			component.Populate(item);
			EntryObjects.Add(component);
		}
		SortEveryoneToggle.group = sortToggleGroup;
		ImageToggleState toggleImage = SortEveryoneToggle.GetComponentInChildren<ImageToggleState>(true);
		SortEveryoneToggle.onValueChanged.AddListener(delegate
		{
			SortByName(!SortEveryoneToggle.isOn);
			lastSortToggle = SortEveryoneToggle;
			lastSortReversed = !SortEveryoneToggle.isOn;
			ResetSortToggles(SortEveryoneToggle);
			if (SortEveryoneToggle.isOn)
			{
				toggleImage.SetActive();
			}
			else
			{
				toggleImage.SetInactive();
			}
		});
		SortByPreviousSelected();
		dirty = true;
	}

	private void SortByPreviousSelected()
	{
		if (!((UnityEngine.Object)sortToggleGroup == (UnityEngine.Object)null) && !((UnityEngine.Object)lastSortToggle == (UnityEngine.Object)null))
		{
			int childCount = ColumnTitlesContainer.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (i < choreGroups.Count)
				{
					Toggle componentInChildren = ColumnTitlesContainer.GetChild(i).Find("Title").GetComponentInChildren<Toggle>();
					if ((UnityEngine.Object)componentInChildren == (UnityEngine.Object)lastSortToggle)
					{
						SortByEffectiveness(choreGroups[i], lastSortReversed, false);
						return;
					}
				}
			}
			if ((UnityEngine.Object)SortEveryoneToggle == (UnityEngine.Object)lastSortToggle)
			{
				SortByName(lastSortReversed);
			}
		}
	}

	protected override void PositionColumnTitles()
	{
		base.PositionColumnTitles();
		int childCount = ColumnTitlesContainer.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (i < choreGroups.Count)
			{
				Toggle sortToggle = ColumnTitlesContainer.GetChild(i).Find("Title").GetComponentInChildren<Toggle>();
				ColumnTitlesContainer.GetChild(i).rectTransform().localScale = Vector3.one;
				ChoreGroup chore_group = choreGroups[i];
				ImageToggleState toggleImage = sortToggle.GetComponentInChildren<ImageToggleState>(true);
				sortToggle.group = sortToggleGroup;
				sortToggle.onValueChanged.AddListener(delegate
				{
					bool playSound = false;
					if ((UnityEngine.Object)lastSortToggle == (UnityEngine.Object)sortToggle)
					{
						playSound = true;
					}
					SortByEffectiveness(chore_group, !sortToggle.isOn, playSound);
					lastSortToggle = sortToggle;
					lastSortReversed = !sortToggle.isOn;
					ResetSortToggles(sortToggle);
					if (sortToggle.isOn)
					{
						toggleImage.SetActive();
					}
					else
					{
						toggleImage.SetInactive();
					}
				});
			}
			ToolTip JobTooltip = ColumnTitlesContainer.GetChild(i).GetComponent<ToolTip>();
			ToolTip toolTip = JobTooltip;
			toolTip.OnToolTip = (Func<string>)Delegate.Combine(toolTip.OnToolTip, (Func<string>)(() => GetJobTooltip(JobTooltip.gameObject)));
			Button componentInChildren = ColumnTitlesContainer.GetChild(i).GetComponentInChildren<Button>();
			EveryoneToggles.Add(componentInChildren, everyoneToggleState.on);
		}
		for (int j = 0; j < choreGroups.Count; j++)
		{
			ChoreGroup chore_group2 = choreGroups[j];
			Button b = EveryoneToggles.Keys.ElementAt(j);
			EveryoneToggles.Keys.ElementAt(j).onClick.AddListener(delegate
			{
				ToggleJobEveryone(b, chore_group2);
			});
		}
		Button key = EveryoneToggles.ElementAt(EveryoneToggles.Count - 1).Key;
		key.transform.parent.Find("Title").gameObject.GetComponentInChildren<Toggle>().gameObject.SetActive(false);
		key.onClick.AddListener(delegate
		{
			ToggleAllTasksEveryone();
		});
		EveryoneAllTaskToggle = new KeyValuePair<Button, everyoneToggleState>(key, EveryoneAllTaskToggle.Value);
	}

	private string GetJobTooltip(GameObject go)
	{
		ToolTip component = go.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		OverviewColumnIdentity component2 = go.GetComponent<OverviewColumnIdentity>();
		if (component2.columnID != "AllTasks")
		{
			ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(component2.columnID);
			component.AddMultiStringTooltip(component2.Column_DisplayName, TextStyle_JobTooltip_Title);
			component.AddMultiStringTooltip(choreGroup.description, TextStyle_JobTooltip_Description);
			component.AddMultiStringTooltip("\n", TextStyle_JobTooltip_Description);
			component.AddMultiStringTooltip(UI.TOOLTIPS.JOBSSCREEN_ATTRIBUTES, TextStyle_JobTooltip_Description);
			component.AddMultiStringTooltip("•  " + choreGroup.attribute.Name, TextStyle_JobTooltip_RelevantAttributes);
		}
		return "";
	}

	private void ToggleAllTasksEveryone()
	{
		string name = "HUD_Click_Deselect";
		if (EveryoneAllTaskToggle.Value != everyoneToggleState.on)
		{
			name = "HUD_Click";
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name, false));
		for (int i = 0; i < choreGroups.Count; i++)
		{
			SetJobEveryone(EveryoneAllTaskToggle.Value != everyoneToggleState.on, choreGroups[i]);
		}
	}

	private void SetJobEveryone(Button button, ChoreGroup chore_group)
	{
		SetJobEveryone(EveryoneToggles[button] != everyoneToggleState.on, chore_group);
	}

	private void SetJobEveryone(bool state, ChoreGroup chore_group)
	{
		foreach (CrewJobsEntry entryObject in EntryObjects)
		{
			entryObject.consumer.SetPermittedByUser(chore_group, state);
		}
	}

	private void ToggleJobEveryone(Button button, ChoreGroup chore_group)
	{
		string name = "HUD_Click_Deselect";
		if (EveryoneToggles[button] != everyoneToggleState.on)
		{
			name = "HUD_Click";
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name, false));
		foreach (CrewJobsEntry entryObject in EntryObjects)
		{
			entryObject.consumer.SetPermittedByUser(chore_group, EveryoneToggles[button] != everyoneToggleState.on);
		}
	}

	private void SortByEffectiveness(ChoreGroup chore_group, bool reverse, bool playSound)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		List<CrewJobsEntry> list = new List<CrewJobsEntry>(EntryObjects);
		list.Sort(delegate(CrewJobsEntry a, CrewJobsEntry b)
		{
			float value = a.Identity.GetAttributes().GetValue(chore_group.attribute.Id);
			float value2 = b.Identity.GetAttributes().GetValue(chore_group.attribute.Id);
			return value.CompareTo(value2);
		});
		ReorderEntries(list, reverse);
	}

	private void ResetSortToggles(Toggle exceptToggle)
	{
		for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
		{
			Toggle componentInChildren = ColumnTitlesContainer.GetChild(i).Find("Title").GetComponentInChildren<Toggle>();
			if (!((UnityEngine.Object)componentInChildren == (UnityEngine.Object)null))
			{
				ImageToggleState componentInChildren2 = componentInChildren.GetComponentInChildren<ImageToggleState>(true);
				if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)exceptToggle)
				{
					componentInChildren2.SetDisabled();
				}
			}
		}
		ImageToggleState componentInChildren3 = SortEveryoneToggle.GetComponentInChildren<ImageToggleState>(true);
		if ((UnityEngine.Object)SortEveryoneToggle != (UnityEngine.Object)exceptToggle)
		{
			componentInChildren3.SetDisabled();
		}
	}

	private void Refresh()
	{
		if (dirty)
		{
			int childCount = ColumnTitlesContainer.childCount;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < childCount; i++)
			{
				bool flag3 = false;
				bool flag4 = false;
				if (choreGroups.Count - 1 >= i)
				{
					ChoreGroup chore_group = choreGroups[i];
					for (int j = 0; j < EntryObjects.Count; j++)
					{
						ChoreConsumer consumer = EntryObjects[j].GetComponent<CrewJobsEntry>().consumer;
						if (consumer.IsPermittedByTraits(chore_group))
						{
							if (consumer.IsPermittedByUser(chore_group))
							{
								flag3 = true;
								flag = true;
							}
							else
							{
								flag4 = true;
								flag2 = true;
							}
						}
					}
					if (flag3 && flag4)
					{
						EveryoneToggles[EveryoneToggles.ElementAt(i).Key] = everyoneToggleState.mixed;
					}
					else if (flag3)
					{
						EveryoneToggles[EveryoneToggles.ElementAt(i).Key] = everyoneToggleState.on;
					}
					else
					{
						EveryoneToggles[EveryoneToggles.ElementAt(i).Key] = everyoneToggleState.off;
					}
					Button componentInChildren = ColumnTitlesContainer.GetChild(i).GetComponentInChildren<Button>();
					ImageToggleState component = componentInChildren.GetComponentsInChildren<Image>(true)[1].GetComponent<ImageToggleState>();
					switch (EveryoneToggles[componentInChildren])
					{
					case everyoneToggleState.off:
						component.SetDisabled();
						break;
					case everyoneToggleState.mixed:
						component.SetInactive();
						break;
					case everyoneToggleState.on:
						component.SetActive();
						break;
					}
				}
			}
			if (flag && flag2)
			{
				EveryoneAllTaskToggle = new KeyValuePair<Button, everyoneToggleState>(EveryoneAllTaskToggle.Key, everyoneToggleState.mixed);
			}
			else if (flag)
			{
				EveryoneAllTaskToggle = new KeyValuePair<Button, everyoneToggleState>(EveryoneAllTaskToggle.Key, everyoneToggleState.on);
			}
			else if (flag2)
			{
				EveryoneAllTaskToggle = new KeyValuePair<Button, everyoneToggleState>(EveryoneAllTaskToggle.Key, everyoneToggleState.off);
			}
			ImageToggleState component2 = EveryoneAllTaskToggle.Key.GetComponentsInChildren<Image>(true)[1].GetComponent<ImageToggleState>();
			switch (EveryoneAllTaskToggle.Value)
			{
			case everyoneToggleState.off:
				component2.SetDisabled();
				break;
			case everyoneToggleState.mixed:
				component2.SetInactive();
				break;
			case everyoneToggleState.on:
				component2.SetActive();
				break;
			}
			Vector2 sizeDelta = EntriesPanelTransform.rectTransform().sizeDelta;
			screenWidth = sizeDelta.x;
			ScrollRectTransform.GetComponent<LayoutElement>().minWidth = screenWidth;
			float num = 31f;
			GetComponent<LayoutElement>().minWidth = screenWidth + num;
			dirty = false;
		}
	}

	private void Update()
	{
		Refresh();
	}

	public void Dirty(object data = null)
	{
		dirty = true;
	}
}
