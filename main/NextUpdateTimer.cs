using STRINGS;
using System;
using UnityEngine;

public class NextUpdateTimer : KMonoBehaviour
{
	public LocText TimerText;

	public KBatchedAnimController UpdateAnimController;

	public KBatchedAnimController UpdateAnimMeterController;

	public float initialAnimScale;

	public System.DateTime nextReleaseDate;

	public System.DateTime currentReleaseDate;

	private string m_releaseTextOverride;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
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
		RefreshReleaseTimes();
	}

	public void UpdateReleaseTimes(string lastUpdateTime, string nextUpdateTime, string textOverride)
	{
		if (!System.DateTime.TryParse(lastUpdateTime, out currentReleaseDate))
		{
			Debug.LogWarning("Failed to parse last_update_time: " + lastUpdateTime);
		}
		if (!System.DateTime.TryParse(nextUpdateTime, out nextReleaseDate))
		{
			Debug.LogWarning("Failed to parse next_update_time: " + nextUpdateTime);
		}
		m_releaseTextOverride = textOverride;
		RefreshReleaseTimes();
		RefreshScale();
	}

	private void RefreshReleaseTimes()
	{
		TimeSpan timeSpan = nextReleaseDate - currentReleaseDate;
		TimeSpan timeSpan2 = nextReleaseDate - System.DateTime.UtcNow;
		TimeSpan timeSpan3 = System.DateTime.UtcNow - currentReleaseDate;
		string empty = string.Empty;
		string s = "4";
		if (!string.IsNullOrEmpty(m_releaseTextOverride))
		{
			empty = m_releaseTextOverride;
		}
		else if (timeSpan2.TotalHours < 8.0)
		{
			empty = UI.DEVELOPMENTBUILDS.UPDATES.TWENTY_FOUR_HOURS;
			s = "4";
		}
		else if (timeSpan2.TotalDays < 1.0)
		{
			empty = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, 1);
			s = "3";
		}
		else
		{
			int num = timeSpan2.Days % 7;
			int num2 = (timeSpan2.Days - num) / 7;
			if (num2 <= 0)
			{
				empty = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.FINAL_WEEK, num);
				s = "2";
			}
			else
			{
				empty = string.Format(UI.DEVELOPMENTBUILDS.UPDATES.BIGGER_TIMES, num, num2);
				s = "1";
			}
		}
		TimerText.text = empty;
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
