using STRINGS;
using UnityEngine;

public class DateTime : KScreen
{
	public static DateTime Instance;

	public LocText day;

	private int displayedDayCount = -1;

	[SerializeField]
	private LocText text;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Days;

	[SerializeField]
	private TextStyleSetting tooltipstyle_Playtime;

	[SerializeField]
	public KToggle scheduleToggle;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tooltip.OnToolTip = OnToolTip;
	}

	private void Update()
	{
		if ((Object)GameClock.Instance != (Object)null && displayedDayCount != GameUtil.GetCurrentCycle())
		{
			text.text = Days();
			displayedDayCount = GameUtil.GetCurrentCycle();
		}
	}

	private string Days()
	{
		return GameUtil.GetCurrentCycle().ToString();
	}

	private string OnToolTip()
	{
		if ((Object)GameClock.Instance != (Object)null)
		{
			tooltip.ClearMultiStringTooltip();
			tooltip.AddMultiStringTooltip(string.Format(UI.ASTEROIDCLOCK.CYCLES_OLD, Days()), tooltipstyle_Days);
			tooltip.AddMultiStringTooltip(string.Format(UI.ASTEROIDCLOCK.TIME_PLAYED, (GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), tooltipstyle_Playtime);
		}
		return string.Empty;
	}
}
