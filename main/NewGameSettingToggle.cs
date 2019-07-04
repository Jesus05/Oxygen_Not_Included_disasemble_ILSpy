using Klei.CustomSettings;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSettingToggle : NewGameSettingWidget
{
	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private MultiToggle Toggle;

	[SerializeField]
	private ToolTip ToggleToolTip;

	[SerializeField]
	private Image BG;

	private ToggleSettingConfig config;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle toggle = Toggle;
		toggle.onClick = (System.Action)Delegate.Combine(toggle.onClick, new System.Action(ToggleSetting));
	}

	public void Initialize(ToggleSettingConfig config)
	{
		this.config = config;
		Label.text = config.label;
		ToolTip.toolTip = config.tooltip;
	}

	public override void Refresh()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(config);
		Toggle.ChangeState(config.IsOnLevel(currentQualitySetting.id) ? 1 : 0);
		ToggleToolTip.toolTip = currentQualitySetting.tooltip;
	}

	public void ToggleSetting()
	{
		CustomGameSettings.Instance.ToggleSettingLevel(config);
		Refresh();
	}
}
