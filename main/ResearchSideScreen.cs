using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchSideScreen : SideScreenContent
{
	public KButton selectResearchButton;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	private Action<object> refreshDisplayStateDelegate;

	public LocText DescriptionText;

	public ResearchSideScreen()
	{
		refreshDisplayStateDelegate = RefreshDisplayState;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		selectResearchButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleResearch();
		};
		Research.Instance.Subscribe(-1914338957, refreshDisplayStateDelegate);
		Research.Instance.Subscribe(-125623018, refreshDisplayStateDelegate);
		RefreshDisplayState(null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RefreshDisplayState(null);
		target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
		target.gameObject.Subscribe(-1852328367, refreshDisplayStateDelegate);
		target.gameObject.Subscribe(-592767678, refreshDisplayStateDelegate);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((bool)target)
		{
			target.gameObject.Unsubscribe(-1852328367, refreshDisplayStateDelegate);
			target.gameObject.Unsubscribe(187661686, refreshDisplayStateDelegate);
			target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, refreshDisplayStateDelegate);
		Research.Instance.Unsubscribe(-125623018, refreshDisplayStateDelegate);
		if ((bool)target)
		{
			target.gameObject.Unsubscribe(-1852328367, refreshDisplayStateDelegate);
			target.gameObject.Unsubscribe(187661686, refreshDisplayStateDelegate);
			target = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<ResearchCenter>() != (UnityEngine.Object)null;
	}

	private void RefreshDisplayState(object data = null)
	{
		if (!((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)null))
		{
			ResearchCenter component = SelectTool.Instance.selected.GetComponent<ResearchCenter>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				researchButtonIcon.sprite = Research.Instance.researchTypes.GetResearchType(component.research_point_type_id).sprite;
				TechInstance activeResearch = Research.Instance.GetActiveResearch();
				if (activeResearch == null)
				{
					DescriptionText.text = "<b>" + UI.UISIDESCREENS.RESEARCHSIDESCREEN.NOSELECTEDRESEARCH + "</b>";
				}
				else
				{
					string str = string.Empty;
					if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(component.research_point_type_id) || activeResearch.tech.costsByResearchTypeID[component.research_point_type_id] <= 0f)
					{
						str += "<color=#7f7f7f>";
					}
					str = str + "<b>" + activeResearch.tech.Name + "</b>";
					if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(component.research_point_type_id) || activeResearch.tech.costsByResearchTypeID[component.research_point_type_id] <= 0f)
					{
						str += "</color>";
					}
					foreach (KeyValuePair<string, float> item in activeResearch.tech.costsByResearchTypeID)
					{
						if (item.Value != 0f)
						{
							bool flag = item.Key == component.research_point_type_id;
							str += "\n   ";
							str += "<b>";
							if (!flag)
							{
								str += "<color=#7f7f7f>";
							}
							string text = str;
							str = text + "- " + Research.Instance.researchTypes.GetResearchType(item.Key).name + ": " + activeResearch.progressInventory.PointsByTypeID[item.Key] + "/" + activeResearch.tech.costsByResearchTypeID[item.Key];
							if (!flag)
							{
								str += "</color>";
							}
							str += "</b>";
						}
					}
					DescriptionText.text = str;
				}
			}
		}
	}
}
