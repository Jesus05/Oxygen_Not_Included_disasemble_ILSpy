using System;
using System.Collections.Generic;
using UnityEngine;

public class SchedulePaintButton : KMonoBehaviour
{
	[SerializeField]
	private LocText label;

	[SerializeField]
	private ImageToggleState toggleState;

	[SerializeField]
	private MultiToggle toggle;

	[SerializeField]
	private ToolTip toolTip;

	public ScheduleGroup group
	{
		get;
		private set;
	}

	public void SetGroup(ScheduleGroup group, Dictionary<string, ColorStyleSetting> styles, Action<SchedulePaintButton> onClick)
	{
		this.group = group;
		if (styles.ContainsKey(group.Id))
		{
			toggleState.SetColorStyle(styles[group.Id]);
		}
		label.text = group.Name;
		MultiToggle multiToggle = toggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			onClick(this);
		});
		toolTip.SetSimpleTooltip(group.GetTooltip());
		base.gameObject.name = "PaintButton_" + group.Id;
	}

	public void SetToggle(bool on)
	{
		toggle.ChangeState(on ? 1 : 0);
	}
}
