using UnityEngine;

public class HealthBar : ProgressBar
{
	private float showTimer;

	private float maxShowTime = 10f;

	private float alwaysShowThreshold = 0.8f;

	private bool ShouldShow => showTimer > 0f || base.PercentFull < alwaysShowThreshold;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
		base.gameObject.SetActive(ShouldShow);
	}

	public void OnChange()
	{
		base.enabled = true;
		showTimer = maxShowTime;
	}

	public override void Update()
	{
		base.Update();
		if (Time.timeScale > 0f)
		{
			showTimer = Mathf.Max(0f, showTimer - Time.unscaledDeltaTime);
		}
		if (!ShouldShow)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	public override void OnOverlayChanged(object data = null)
	{
		if (autoHide)
		{
			if ((HashedString)data == OverlayModes.None.ID)
			{
				if (!base.gameObject.activeSelf && ShouldShow)
				{
					base.enabled = true;
					base.gameObject.SetActive(true);
				}
			}
			else if (base.gameObject.activeSelf)
			{
				base.enabled = false;
				base.gameObject.SetActive(false);
			}
		}
	}
}
