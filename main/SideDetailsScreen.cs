using System.Collections.Generic;
using UnityEngine;

public class SideDetailsScreen : KScreen
{
	[SerializeField]
	private List<SideTargetScreen> screens;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton backButton;

	[SerializeField]
	private RectTransform body;

	private RectTransform rectTransform;

	private Dictionary<string, SideTargetScreen> screenMap;

	private SideTargetScreen activeScreen;

	public static SideDetailsScreen Instance;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
		Initialize();
		base.gameObject.SetActive(false);
	}

	private void Initialize()
	{
		if (screens != null)
		{
			rectTransform = GetComponent<RectTransform>();
			screenMap = new Dictionary<string, SideTargetScreen>();
			List<SideTargetScreen> list = new List<SideTargetScreen>();
			foreach (SideTargetScreen screen in screens)
			{
				SideTargetScreen sideTargetScreen = Util.KInstantiateUI<SideTargetScreen>(screen.gameObject, body.gameObject, false);
				sideTargetScreen.gameObject.SetActive(false);
				list.Add(sideTargetScreen);
			}
			list.ForEach(delegate(SideTargetScreen s)
			{
				screenMap.Add(s.name, s);
			});
			backButton.onClick += delegate
			{
				Show(false);
			};
		}
	}

	public void SetTitle(string newTitle)
	{
		title.text = newTitle;
	}

	public void SetScreen(string screenName, object content, float x)
	{
		if (!screenMap.ContainsKey(screenName))
		{
			Debug.LogError("Tried to open a screen that does exist on the manager!");
		}
		else if (content == null)
		{
			Debug.LogError("Tried to set " + screenName + " with null content!");
		}
		else
		{
			if (!base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(true);
			}
			Rect rect = rectTransform.rect;
			RectTransform obj = rectTransform;
			Vector2 offsetMin = rectTransform.offsetMin;
			obj.offsetMin = new Vector2(x, offsetMin.y);
			RectTransform obj2 = rectTransform;
			float x2 = x + rect.width;
			Vector2 offsetMax = rectTransform.offsetMax;
			obj2.offsetMax = new Vector2(x2, offsetMax.y);
			if ((Object)activeScreen != (Object)null)
			{
				activeScreen.gameObject.SetActive(false);
			}
			activeScreen = screenMap[screenName];
			activeScreen.gameObject.SetActive(true);
			SetTitle(activeScreen.displayName);
			activeScreen.SetTarget(content);
		}
	}
}
