using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TimeRangeSideScreen : SideScreenContent, IRender200ms
{
	public Image imageActiveZone;

	private LogicTimeOfDaySensor targetTimedSwitch;

	public KSlider startTime;

	public KSlider duration;

	public RectTransform endIndicator;

	public LocText labelHeaderStart;

	public LocText labelHeaderDuration;

	public LocText labelValueStart;

	public LocText labelValueDuration;

	public RectTransform currentTimeMarker;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		labelHeaderStart.text = UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.ON;
		labelHeaderDuration.text = UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.DURATION;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<LogicTimeOfDaySensor>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetTimedSwitch = target.GetComponent<LogicTimeOfDaySensor>();
		duration.onValueChanged.RemoveAllListeners();
		startTime.onValueChanged.RemoveAllListeners();
		startTime.value = targetTimedSwitch.startTime;
		duration.value = targetTimedSwitch.duration;
		ChangeSetting();
		startTime.onValueChanged.AddListener(delegate
		{
			ChangeSetting();
		});
		duration.onValueChanged.AddListener(delegate
		{
			ChangeSetting();
		});
	}

	private void ChangeSetting()
	{
		targetTimedSwitch.startTime = startTime.value;
		targetTimedSwitch.duration = duration.value;
		imageActiveZone.rectTransform.rotation = Quaternion.identity;
		imageActiveZone.rectTransform.Rotate(0f, 0f, NormalizedValueToDegrees(startTime.value));
		imageActiveZone.fillAmount = duration.value;
		labelValueStart.text = GameUtil.GetFormattedPercent(targetTimedSwitch.startTime * 100f, GameUtil.TimeSlice.None);
		labelValueDuration.text = GameUtil.GetFormattedPercent(targetTimedSwitch.duration * 100f, GameUtil.TimeSlice.None);
		endIndicator.rotation = Quaternion.identity;
		endIndicator.Rotate(0f, 0f, NormalizedValueToDegrees(startTime.value + duration.value));
		startTime.SetTooltipText(string.Format(UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.ON_TOOLTIP, GameUtil.GetFormattedPercent(targetTimedSwitch.startTime * 100f, GameUtil.TimeSlice.None)));
		duration.SetTooltipText(string.Format(UI.UISIDESCREENS.TIME_RANGE_SIDE_SCREEN.DURATION_TOOLTIP, GameUtil.GetFormattedPercent(targetTimedSwitch.duration * 100f, GameUtil.TimeSlice.None)));
	}

	public void Render200ms(float dt)
	{
		currentTimeMarker.rotation = Quaternion.identity;
		currentTimeMarker.Rotate(0f, 0f, NormalizedValueToDegrees(GameClock.Instance.GetCurrentCycleAsPercentage()));
	}

	private float NormalizedValueToDegrees(float value)
	{
		return 360f * value;
	}

	private float SecondsToDegrees(float seconds)
	{
		return 360f * (seconds / 600f);
	}

	private float DegreesToNormalizedValue(float degrees)
	{
		return degrees / 360f;
	}
}
