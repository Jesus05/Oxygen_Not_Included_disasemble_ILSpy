using System;
using UnityEngine;
using UnityEngine.UI;

public class KCanvasScaler : KMonoBehaviour
{
	[Serializable]
	public struct ScaleStep
	{
		public float scale;

		public float maxRes_y;

		public ScaleStep(float maxRes_y, float scale)
		{
			this.maxRes_y = maxRes_y;
			this.scale = scale;
		}
	}

	[MyCmpReq]
	private CanvasScaler canvasScaler;

	public static string UIScalePrefKey = "UIScalePref";

	private float userScale = 1f;

	[Range(0.75f, 2f)]
	private ScaleStep[] scaleSteps = new ScaleStep[3]
	{
		new ScaleStep(720f, 0.86f),
		new ScaleStep(1080f, 1f),
		new ScaleStep(2160f, 1.33f)
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (KPlayerPrefs.HasKey(UIScalePrefKey))
		{
			SetUserScale(KPlayerPrefs.GetFloat(UIScalePrefKey) / 100f);
		}
		else
		{
			SetUserScale(1f);
		}
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
	}

	private void OnResize()
	{
		SetUserScale(userScale);
	}

	public void SetUserScale(float scale)
	{
		userScale = scale;
		canvasScaler.scaleFactor = GetCanvasScale();
	}

	public float GetUserScale()
	{
		return userScale;
	}

	public float GetCanvasScale()
	{
		return userScale * ScreenRelativeScale();
	}

	private float ScreenRelativeScale()
	{
		float dpi = Screen.dpi;
		DebugUtil.LogArgs("SCREEN DPI:", dpi);
		if (!((float)Screen.height <= scaleSteps[0].maxRes_y) && !((float)Screen.width / (float)Screen.height < 1.67777777f))
		{
			if (!((float)Screen.height > scaleSteps[scaleSteps.Length - 1].maxRes_y))
			{
				for (int i = 0; i < scaleSteps.Length; i++)
				{
					if ((float)Screen.height > scaleSteps[i].maxRes_y && (float)Screen.height <= scaleSteps[i + 1].maxRes_y)
					{
						float t = ((float)Screen.height - scaleSteps[i].maxRes_y) / (scaleSteps[i + 1].maxRes_y - scaleSteps[i].maxRes_y);
						return Mathf.Lerp(scaleSteps[i].scale, scaleSteps[i + 1].scale, t);
					}
				}
				return 1f;
			}
			return scaleSteps[scaleSteps.Length - 1].scale;
		}
		return scaleSteps[0].scale;
	}
}
