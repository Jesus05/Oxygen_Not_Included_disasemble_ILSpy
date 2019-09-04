using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ScheduleBlockButton : KMonoBehaviour
{
	[SerializeField]
	private KImage image;

	[SerializeField]
	private ToolTip toolTip;

	private Dictionary<string, ColorStyleSetting> paintStyles;

	public int idx
	{
		get;
		private set;
	}

	public void Setup(int idx, Dictionary<string, ColorStyleSetting> paintStyles, int totalBlocks)
	{
		this.idx = idx;
		this.paintStyles = paintStyles;
		if (idx < TRAITS.EARLYBIRD_SCHEDULEBLOCK)
		{
			HierarchyReferences component = GetComponent<HierarchyReferences>();
			component.GetReference<RectTransform>("MorningIcon").gameObject.SetActive(true);
		}
		else if (idx >= totalBlocks - 3)
		{
			HierarchyReferences component2 = GetComponent<HierarchyReferences>();
			component2.GetReference<RectTransform>("NightIcon").gameObject.SetActive(true);
		}
		base.gameObject.name = "ScheduleBlock_" + idx.ToString();
	}

	public void SetBlockTypes(List<ScheduleBlockType> blockTypes)
	{
		ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(blockTypes);
		if (scheduleGroup != null && paintStyles.ContainsKey(scheduleGroup.Id))
		{
			image.colorStyleSetting = paintStyles[scheduleGroup.Id];
			image.ApplyColorStyleSetting();
			toolTip.SetSimpleTooltip(scheduleGroup.GetTooltip());
		}
		else
		{
			toolTip.SetSimpleTooltip("UNKNOWN");
		}
	}
}
