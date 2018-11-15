using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewListScreen<EntryType> : KScreen where EntryType : CrewListEntry
{
	public GameObject Prefab_CrewEntry;

	public List<EntryType> EntryObjects = new List<EntryType>();

	public Transform ScrollRectTransform;

	public Transform EntriesPanelTransform;

	protected Vector2 EntryRectSize = new Vector2(750f, 64f);

	public int maxEntriesBeforeScroll = 5;

	public Scrollbar PanelScrollbar;

	protected ToggleGroup sortToggleGroup;

	protected Toggle lastSortToggle;

	protected bool lastSortReversed;

	public GameObject Prefab_ColumnTitle;

	public Transform ColumnTitlesContainer;

	public bool autoColumn;

	public float columnTitleHorizontalOffset;

	protected override void OnActivate()
	{
		base.OnActivate();
		ClearEntries();
		SpawnEntries();
		PositionColumnTitles();
		if (autoColumn)
		{
			UpdateColumnTitles();
		}
		ConsumeMouseScroll = true;
	}

	protected override void OnCmpEnable()
	{
		if (autoColumn)
		{
			UpdateColumnTitles();
		}
		Reconstruct();
	}

	private void ClearEntries()
	{
		for (int num = EntryObjects.Count - 1; num > -1; num--)
		{
			Util.KDestroyGameObject(EntryObjects[num]);
		}
		EntryObjects.Clear();
	}

	protected void RefreshCrewPortraitContent()
	{
		EntryObjects.ForEach(delegate(EntryType eo)
		{
			eo.RefreshCrewPortraitContent();
		});
	}

	protected virtual void SpawnEntries()
	{
		if (EntryObjects.Count != 0)
		{
			ClearEntries();
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (autoColumn)
		{
			UpdateColumnTitles();
		}
		bool flag = false;
		List<MinionIdentity> liveIdentities = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
		if (EntryObjects.Count != liveIdentities.Count || EntryObjects.FindAll((EntryType o) => liveIdentities.Contains(o.Identity)).Count != EntryObjects.Count)
		{
			flag = true;
		}
		if (flag)
		{
			Reconstruct();
		}
		UpdateScroll();
	}

	public void Reconstruct()
	{
		ClearEntries();
		SpawnEntries();
	}

	private void UpdateScroll()
	{
		if ((bool)PanelScrollbar)
		{
			if (EntryObjects.Count <= maxEntriesBeforeScroll)
			{
				PanelScrollbar.value = Mathf.Lerp(PanelScrollbar.value, 1f, 10f);
				PanelScrollbar.gameObject.SetActive(false);
			}
			else
			{
				PanelScrollbar.gameObject.SetActive(true);
			}
		}
	}

	private void SetHeadersActive(bool state)
	{
		for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
		{
			ColumnTitlesContainer.GetChild(i).gameObject.SetActive(state);
		}
	}

	protected virtual void PositionColumnTitles()
	{
		if (!((Object)ColumnTitlesContainer == (Object)null))
		{
			if (EntryObjects.Count <= 0)
			{
				SetHeadersActive(false);
			}
			else
			{
				SetHeadersActive(true);
				EntryType val = EntryObjects[0];
				int childCount = val.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					EntryType val2 = EntryObjects[0];
					OverviewColumnIdentity component = ((Component)val2.transform.GetChild(i)).GetComponent<OverviewColumnIdentity>();
					if ((Object)component != (Object)null)
					{
						GameObject gameObject = Util.KInstantiate(Prefab_ColumnTitle, null, null);
						gameObject.name = component.Column_DisplayName;
						LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
						gameObject.transform.SetParent(ColumnTitlesContainer);
						componentInChildren.text = ((!component.StringLookup) ? component.Column_DisplayName : ((string)Strings.Get(component.Column_DisplayName)));
						gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.SORTCOLUMN, componentInChildren.text);
						RectTransform rectTransform = gameObject.rectTransform();
						Vector2 anchoredPosition = component.rectTransform().anchoredPosition;
						rectTransform.anchoredPosition = new Vector2(anchoredPosition.x, 0f);
						OverviewColumnIdentity overviewColumnIdentity = gameObject.GetComponent<OverviewColumnIdentity>();
						if ((Object)overviewColumnIdentity == (Object)null)
						{
							overviewColumnIdentity = gameObject.AddComponent<OverviewColumnIdentity>();
						}
						overviewColumnIdentity.Column_DisplayName = component.Column_DisplayName;
						overviewColumnIdentity.columnID = component.columnID;
						overviewColumnIdentity.xPivot = component.xPivot;
						overviewColumnIdentity.Sortable = component.Sortable;
						if (overviewColumnIdentity.Sortable)
						{
							((Component)overviewColumnIdentity).GetComponentInChildren<ImageToggleState>(true).gameObject.SetActive(true);
						}
					}
				}
				UpdateColumnTitles();
				sortToggleGroup = base.gameObject.AddComponent<ToggleGroup>();
				sortToggleGroup.allowSwitchOff = true;
			}
		}
	}

	protected void SortByName(bool reverse)
	{
		List<EntryType> list = new List<EntryType>(EntryObjects);
		list.Sort(delegate(EntryType a, EntryType b)
		{
			string text = a.Identity.GetProperName() + a.gameObject.GetInstanceID();
			string strB = b.Identity.GetProperName() + b.gameObject.GetInstanceID();
			return text.CompareTo(strB);
		});
		ReorderEntries(list, reverse);
	}

	protected void UpdateColumnTitles()
	{
		if (EntryObjects.Count > 0)
		{
			EntryType val = EntryObjects[0];
			if (val.gameObject.activeSelf)
			{
				SetHeadersActive(true);
				for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
				{
					RectTransform rectTransform = ColumnTitlesContainer.GetChild(i).rectTransform();
					int num = 0;
					while (true)
					{
						int num2 = num;
						EntryType val2 = EntryObjects[0];
						if (num2 >= val2.transform.childCount)
						{
							break;
						}
						EntryType val3 = EntryObjects[0];
						OverviewColumnIdentity component = ((Component)val3.transform.GetChild(num)).GetComponent<OverviewColumnIdentity>();
						if ((Object)component != (Object)null && component.Column_DisplayName == rectTransform.name)
						{
							RectTransform rectTransform2 = rectTransform;
							float xPivot = component.xPivot;
							Vector2 pivot = rectTransform.pivot;
							rectTransform2.pivot = new Vector2(xPivot, pivot.y);
							RectTransform rectTransform3 = rectTransform;
							Vector2 anchoredPosition = component.rectTransform().anchoredPosition;
							rectTransform3.anchoredPosition = new Vector2(anchoredPosition.x + columnTitleHorizontalOffset, 0f);
							RectTransform rectTransform4 = rectTransform;
							Vector2 sizeDelta = component.rectTransform().sizeDelta;
							float x = sizeDelta.x;
							Vector2 sizeDelta2 = rectTransform.sizeDelta;
							rectTransform4.sizeDelta = new Vector2(x, sizeDelta2.y);
							Vector2 anchoredPosition2 = rectTransform.anchoredPosition;
							if (anchoredPosition2.x == 0f)
							{
								rectTransform.gameObject.SetActive(false);
							}
							else
							{
								rectTransform.gameObject.SetActive(true);
							}
						}
						num++;
					}
				}
				return;
			}
		}
		SetHeadersActive(false);
	}

	protected void ReorderEntries(List<EntryType> sortedEntries, bool reverse)
	{
		for (int i = 0; i < sortedEntries.Count; i++)
		{
			if (reverse)
			{
				EntryType val = sortedEntries[i];
				val.transform.SetSiblingIndex(sortedEntries.Count - 1 - i);
			}
			else
			{
				EntryType val2 = sortedEntries[i];
				val2.transform.SetSiblingIndex(i);
			}
		}
	}
}
