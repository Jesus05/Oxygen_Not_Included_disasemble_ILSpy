using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : KMonoBehaviour
{
	public Image bar;

	private Func<float> updatePercentFull;

	private int overlayUpdateHandle = -1;

	public bool autoHide = true;

	public Color barColor
	{
		get
		{
			return bar.color;
		}
		set
		{
			bar.color = value;
		}
	}

	public float PercentFull
	{
		get
		{
			return bar.fillAmount;
		}
		set
		{
			bar.fillAmount = value;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (autoHide)
		{
			overlayUpdateHandle = Game.Instance.Subscribe(1798162660, OnOverlayChanged);
			if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null && OverlayScreen.Instance.GetMode() != 0)
			{
				base.gameObject.SetActive(false);
			}
		}
		base.enabled = (updatePercentFull != null);
	}

	public void SetUpdateFunc(Func<float> func)
	{
		updatePercentFull = func;
		base.enabled = (updatePercentFull != null);
	}

	public virtual void Update()
	{
		if (updatePercentFull != null)
		{
			PercentFull = updatePercentFull();
		}
	}

	public virtual void OnOverlayChanged(object data = null)
	{
		if (autoHide)
		{
			if ((SimViewMode)data == SimViewMode.None)
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(true);
				}
			}
			else if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (overlayUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(overlayUpdateHandle);
		}
		base.OnCleanUp();
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	public static ProgressBar CreateProgressBar(KMonoBehaviour entity, Func<float> updateFunc)
	{
		ProgressBar progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, null, false);
		progressBar.SetUpdateFunc(updateFunc);
		progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
		progressBar.name = ((!((UnityEngine.Object)entity != (UnityEngine.Object)null)) ? string.Empty : (entity.name + "_")) + " ProgressBar";
		progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
		progressBar.Update();
		Vector3 a = entity.transform.GetPosition() + Vector3.down * 0.5f;
		Building component = entity.GetComponent<Building>();
		TransformExtensions.SetPosition(position: (!((UnityEngine.Object)component != (UnityEngine.Object)null)) ? (a - Vector3.right * 0.5f) : (a - Vector3.right * 0.5f * (float)(component.Def.WidthInCells % 2) + component.Def.placementPivot), transform: progressBar.transform);
		return progressBar;
	}
}
