using Klei.AI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewRationsScreen : CrewListScreen<CrewRationsEntry>
{
	[SerializeField]
	private KButton closebutton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		closebutton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RefreshCrewPortraitContent();
		SortByPreviousSelected();
	}

	private void SortByPreviousSelected()
	{
		if (!((Object)sortToggleGroup == (Object)null) && !((Object)lastSortToggle == (Object)null))
		{
			for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
			{
				OverviewColumnIdentity component = ColumnTitlesContainer.GetChild(i).GetComponent<OverviewColumnIdentity>();
				Toggle component2 = ColumnTitlesContainer.GetChild(i).GetComponent<Toggle>();
				if ((Object)component2 == (Object)lastSortToggle)
				{
					if (component.columnID == "name")
					{
						SortByName(lastSortReversed);
					}
					if (component.columnID == "health")
					{
						SortByAmount("HitPoints", lastSortReversed);
					}
					if (component.columnID == "stress")
					{
						SortByAmount("Stress", lastSortReversed);
					}
					if (component.columnID == "calories")
					{
						SortByAmount("Calories", lastSortReversed);
					}
				}
			}
		}
	}

	protected override void PositionColumnTitles()
	{
		base.PositionColumnTitles();
		for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
		{
			OverviewColumnIdentity component = ColumnTitlesContainer.GetChild(i).GetComponent<OverviewColumnIdentity>();
			if (component.Sortable)
			{
				Toggle toggle = ColumnTitlesContainer.GetChild(i).GetComponent<Toggle>();
				toggle.group = sortToggleGroup;
				ImageToggleState toggleImage = toggle.GetComponentInChildren<ImageToggleState>(true);
				if (component.columnID == "name")
				{
					toggle.onValueChanged.AddListener(delegate
					{
						SortByName(!toggle.isOn);
						lastSortToggle = toggle;
						lastSortReversed = !toggle.isOn;
						ResetSortToggles(toggle);
						if (toggle.isOn)
						{
							toggleImage.SetActive();
						}
						else
						{
							toggleImage.SetInactive();
						}
					});
				}
				if (component.columnID == "health")
				{
					toggle.onValueChanged.AddListener(delegate
					{
						SortByAmount("HitPoints", !toggle.isOn);
						lastSortToggle = toggle;
						lastSortReversed = !toggle.isOn;
						ResetSortToggles(toggle);
						if (toggle.isOn)
						{
							toggleImage.SetActive();
						}
						else
						{
							toggleImage.SetInactive();
						}
					});
				}
				if (component.columnID == "stress")
				{
					toggle.onValueChanged.AddListener(delegate
					{
						SortByAmount("Stress", !toggle.isOn);
						lastSortToggle = toggle;
						lastSortReversed = !toggle.isOn;
						ResetSortToggles(toggle);
						if (toggle.isOn)
						{
							toggleImage.SetActive();
						}
						else
						{
							toggleImage.SetInactive();
						}
					});
				}
				if (component.columnID == "calories")
				{
					toggle.onValueChanged.AddListener(delegate
					{
						SortByAmount("Calories", !toggle.isOn);
						lastSortToggle = toggle;
						lastSortReversed = !toggle.isOn;
						ResetSortToggles(toggle);
						if (toggle.isOn)
						{
							toggleImage.SetActive();
						}
						else
						{
							toggleImage.SetInactive();
						}
					});
				}
			}
		}
	}

	protected override void SpawnEntries()
	{
		base.SpawnEntries();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			GameObject gameObject = Util.KInstantiateUI(Prefab_CrewEntry, EntriesPanelTransform.gameObject, false);
			CrewRationsEntry component = gameObject.GetComponent<CrewRationsEntry>();
			component.Populate(item);
			EntryObjects.Add(component);
		}
		SortByPreviousSelected();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		foreach (CrewRationsEntry entryObject in EntryObjects)
		{
			entryObject.Refresh();
		}
	}

	private void SortByAmount(string amount_id, bool reverse)
	{
		List<CrewRationsEntry> list = new List<CrewRationsEntry>(EntryObjects);
		list.Sort(delegate(CrewRationsEntry a, CrewRationsEntry b)
		{
			float value = a.Identity.GetAmounts().GetValue(amount_id);
			float value2 = b.Identity.GetAmounts().GetValue(amount_id);
			return value.CompareTo(value2);
		});
		ReorderEntries(list, reverse);
	}

	private void ResetSortToggles(Toggle exceptToggle)
	{
		for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
		{
			Toggle component = ColumnTitlesContainer.GetChild(i).GetComponent<Toggle>();
			ImageToggleState componentInChildren = component.GetComponentInChildren<ImageToggleState>(true);
			if ((Object)component != (Object)exceptToggle)
			{
				componentInChildren.SetDisabled();
			}
		}
	}
}
