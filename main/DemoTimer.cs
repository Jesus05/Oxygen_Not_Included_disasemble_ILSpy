using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DemoTimer : MonoBehaviour
{
	public static DemoTimer Instance;

	public LocText labelText;

	public Image clockImage;

	public GameObject Prefab_DemoOverScreen;

	public GameObject Prefab_FadeOutScreen;

	private float duration;

	private float elapsed = 0f;

	private bool demoOver = false;

	private float beginTime = -1f;

	public bool CountdownActive = false;

	private GameObject fadeOutScreen;

	private Color fadeOutColor = default(Color);

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void Start()
	{
		Instance = this;
		if (GenericGameSettings.instance != null)
		{
			if (GenericGameSettings.instance.demoMode)
			{
				duration = (float)GenericGameSettings.instance.demoTime;
				labelText.gameObject.SetActive(GenericGameSettings.instance.showDemoTimer);
				clockImage.gameObject.SetActive(GenericGameSettings.instance.showDemoTimer);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		duration = (float)GenericGameSettings.instance.demoTime;
		fadeOutScreen = Util.KInstantiateUI(Prefab_FadeOutScreen, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
		Image component = fadeOutScreen.GetComponent<Image>();
		component.raycastTarget = false;
		fadeOutColor = component.color;
		fadeOutColor.a = 0f;
		fadeOutScreen.GetComponent<Image>().color = fadeOutColor;
	}

	private void Update()
	{
		if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.BackQuote))
		{
			CountdownActive = !CountdownActive;
			UpdateLabel();
		}
		if (!demoOver && CountdownActive)
		{
			if (beginTime == -1f)
			{
				beginTime = Time.unscaledTime;
			}
			elapsed = Mathf.Clamp(0f, Time.unscaledTime - beginTime, duration);
			if (elapsed + 5f >= duration)
			{
				float f = (duration - elapsed) / 5f;
				fadeOutColor.a = Mathf.Min(1f, 1f - Mathf.Sqrt(f));
				fadeOutScreen.GetComponent<Image>().color = fadeOutColor;
			}
			if (elapsed >= duration)
			{
				EndDemo();
			}
			UpdateLabel();
		}
	}

	private void UpdateLabel()
	{
		int num = Mathf.RoundToInt(duration - elapsed);
		int num2 = Mathf.FloorToInt((float)(num / 60));
		int num3 = num % 60;
		labelText.text = UI.DEMOOVERSCREEN.TIMEREMAINING + " " + num2.ToString("00") + ":" + num3.ToString("00");
		if (!CountdownActive)
		{
			labelText.text = UI.DEMOOVERSCREEN.TIMERINACTIVE;
		}
	}

	public void EndDemo()
	{
		if (!demoOver)
		{
			demoOver = true;
			GameObject gameObject = Util.KInstantiateUI(Prefab_DemoOverScreen, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
			gameObject.GetComponent<DemoOverScreen>().Show(true);
		}
	}
}
