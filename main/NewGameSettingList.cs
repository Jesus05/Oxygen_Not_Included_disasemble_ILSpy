using Klei.CustomSettings;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSettingList : NewGameSettingWidget
{
	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private LocText ValueLabel;

	[SerializeField]
	private ToolTip ValueToolTip;

	[SerializeField]
	private KButton CycleLeft;

	[SerializeField]
	private KButton CycleRight;

	[SerializeField]
	private Image BG;

	private ListSettingConfig config;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		CycleLeft.onClick += DoCycleLeft;
		CycleRight.onClick += DoCycleRight;
	}

	public void Initialize(ListSettingConfig config)
	{
		this.config = config;
		Label.text = config.label;
		ToolTip.toolTip = config.tooltip;
	}

	public override void Refresh()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(config);
		ValueLabel.text = currentQualitySetting.label;
		ValueToolTip.toolTip = currentQualitySetting.tooltip;
		CycleLeft.isInteractable = !config.IsFirstLevel(currentQualitySetting.id);
		CycleRight.isInteractable = !config.IsLastLevel(currentQualitySetting.id);
	}

	private void DoCycleLeft()
	{
		CustomGameSettings.Instance.CycleSettingLevel(config, -1);
		Refresh();
	}

	private void DoCycleRight()
	{
		CustomGameSettings.Instance.CycleSettingLevel(config, 1);
		Refresh();
	}
}
