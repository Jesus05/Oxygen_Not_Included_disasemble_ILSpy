using STRINGS;
using System;
using UnityEngine;

public class NextUpdateTimer : KMonoBehaviour
{
	public LocText TimerText;

	public KBatchedAnimController UpdateAnimController;

	public KBatchedAnimController UpdateAnimMeterController;

	public float initialAnimScale;

	private bool useSpecificDate = true;

	public System.DateTime nextReleaseDate;

	public System.DateTime currentReleaseDate;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		currentReleaseDate = new System.DateTime(2019, 4, 16, 17, 0, 0, DateTimeKind.Utc);
		nextReleaseDate = new System.DateTime(2019, 7, 14, 17, 0, 0, DateTimeKind.Utc);
		initialAnimScale = UpdateAnimController.animScale;
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(RefreshScale));
	}

	protected override void OnCleanUp()
	{
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(RefreshScale));
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TimeSpan timeSpan = nextReleaseDate - currentReleaseDate;
		TimeSpan timeSpan2 = nextReleaseDate - System.DateTime.UtcNow;
		TimeSpan timeSpan3 = System.DateTime.UtcNow - currentReleaseDate;
		string text = "";
		string s = "1";
		if (useSpecificDate)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.SPECIFIC_DATE;
		}
		else if (timeSpan2.TotalHours < 8.0)
		{
			text = UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
			s = "4";
		}
		else if (timeSpan2.TotalDays < 1.0)
		{
			text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, 1);
			s = "3";
		}
		else
		{
			int num = timeSpan2.Days % 7;
			int num2 = (timeSpan2.Days - num) / 7;
			if (num2 <= 0)
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, num);
				s = "2";
			}
			else
			{
				text = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, num, num2);
				s = "1";
			}
		}
		TimerText.text = text;
		UpdateAnimController.Play(s, KAnim.PlayMode.Loop, 1f, 0f);
		double num3 = timeSpan3.TotalSeconds / timeSpan.TotalSeconds;
		float positionPercent = Mathf.Clamp01((float)num3);
		UpdateAnimMeterController.SetPositionPercent(positionPercent);
	}

	private void RefreshScale()
	{
		float num = 1f;
		num = GetComponentInParent<KCanvasScaler>().GetCanvasScale();
		if ((UnityEngine.Object)UpdateAnimController != (UnityEngine.Object)null)
		{
			UpdateAnimController.animScale = initialAnimScale * (1f / num);
		}
		if ((UnityEngine.Object)UpdateAnimMeterController != (UnityEngine.Object)null)
		{
			UpdateAnimMeterController.animScale = initialAnimScale * (1f / num);
		}
	}
}
