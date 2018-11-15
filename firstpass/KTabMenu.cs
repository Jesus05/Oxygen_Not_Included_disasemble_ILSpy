using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KTabMenu : KScreen
{
	public delegate void TabActivated(int tabIdx, int previouslyActiveTabIdx);

	[SerializeField]
	protected KTabMenuHeader header;

	[SerializeField]
	protected RectTransform body;

	protected List<KScreen> tabs = new List<KScreen>();

	protected int previouslyActiveTab = -1;

	public int PreviousActiveTab => previouslyActiveTab;

	public event TabActivated onTabActivated;

	public int AddTab(string tabName, KScreen contents)
	{
		int count = tabs.Count;
		header.Add(tabName, ActivateTab, tabs.Count);
		header.SetTabEnabled(count, true);
		tabs.Add(contents);
		return count;
	}

	public int AddTab(Sprite icon, string tabName, KScreen contents, string tooltip = "")
	{
		int count = tabs.Count;
		header.Add(icon, tabName, ActivateTab, tabs.Count, tooltip);
		header.SetTabEnabled(count, true);
		tabs.Add(contents);
		return count;
	}

	public virtual void ActivateTab(int tabIdx)
	{
		header.Activate(tabIdx, previouslyActiveTab);
		for (int i = 0; i < tabs.Count; i++)
		{
			tabs[i].gameObject.SetActive(i == tabIdx);
		}
		ScrollRect component = body.GetComponent<ScrollRect>();
		if ((Object)component != (Object)null && tabIdx < tabs.Count)
		{
			component.content = tabs[tabIdx].GetComponent<RectTransform>();
		}
		if (this.onTabActivated != null)
		{
			this.onTabActivated(tabIdx, previouslyActiveTab);
		}
		previouslyActiveTab = tabIdx;
	}

	protected override void OnDeactivate()
	{
		foreach (KScreen tab in tabs)
		{
			tab.Deactivate();
		}
		base.OnDeactivate();
	}

	public void SetTabEnabled(int tabIdx, bool enabled)
	{
		header.SetTabEnabled(tabIdx, enabled);
	}

	protected int CountTabs()
	{
		int num = 0;
		for (int i = 0; i < header.transform.childCount; i++)
		{
			if (header.transform.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		return num;
	}
}
