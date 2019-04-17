using UnityEngine;

public class HoverTextScreen : KScreen
{
	[SerializeField]
	private HoverTextSkin skin;

	public Sprite[] HoverIcons;

	public HoverTextDrawer drawer;

	public static HoverTextScreen Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		drawer = new HoverTextDrawer(skin.skin, GetComponent<RectTransform>());
	}

	public HoverTextDrawer BeginDrawing()
	{
		Vector2 localPoint = Vector2.zero;
		Vector2 screenPoint = KInputManager.GetMousePos();
		RectTransform rectTransform = base.transform.parent as RectTransform;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, base.transform.parent.GetComponent<Canvas>().worldCamera, out localPoint);
		float x = localPoint.x;
		Vector2 sizeDelta = rectTransform.sizeDelta;
		localPoint.x = x + sizeDelta.x / 2f;
		float y = localPoint.y;
		Vector2 sizeDelta2 = rectTransform.sizeDelta;
		localPoint.y = y - sizeDelta2.y / 2f;
		drawer.BeginDrawing(localPoint);
		return drawer;
	}

	private void Update()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
		if ((Object)OverlayScreen.Instance == (Object)null || vector.x < 0f || vector.x > Grid.WidthInMeters || vector.y < 0f || vector.y > Grid.HeightInMeters)
		{
			drawer.SetEnabled(false);
		}
		else
		{
			bool enabled = PlayerController.Instance.ActiveTool.ShowHoverUI();
			drawer.SetEnabled(enabled);
		}
	}

	public Sprite GetSprite(string byName)
	{
		Sprite[] hoverIcons = HoverIcons;
		foreach (Sprite sprite in hoverIcons)
		{
			if ((Object)sprite != (Object)null && sprite.name == byName)
			{
				return sprite;
			}
		}
		Debug.LogWarning("No icon named " + byName + " was found on HoverTextScreen.prefab");
		return null;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		drawer.Cleanup();
	}
}
