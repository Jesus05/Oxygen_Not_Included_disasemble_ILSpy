using UnityEngine;

public class GameScreenManager : KMonoBehaviour
{
	public enum UIRenderTarget
	{
		WorldSpace,
		ScreenSpaceCamera,
		ScreenSpaceOverlay,
		HoverTextScreen
	}

	public GameObject ssHoverTextCanvas;

	public GameObject ssCameraCanvas;

	public GameObject ssOverlayCanvas;

	public GameObject worldSpaceCanvas;

	[SerializeField]
	private Color[] uiColors;

	public static GameScreenManager Instance
	{
		get;
		private set;
	}

	public static Color[] UIColors => Instance.uiColors;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public Camera GetCamera(UIRenderTarget target)
	{
		switch (target)
		{
		case UIRenderTarget.WorldSpace:
			return worldSpaceCanvas.GetComponent<Canvas>().worldCamera;
		case UIRenderTarget.ScreenSpaceOverlay:
			return ssOverlayCanvas.GetComponent<Canvas>().worldCamera;
		case UIRenderTarget.ScreenSpaceCamera:
			return ssCameraCanvas.GetComponent<Canvas>().worldCamera;
		case UIRenderTarget.HoverTextScreen:
			return ssHoverTextCanvas.GetComponent<Canvas>().worldCamera;
		default:
			return base.gameObject.GetComponent<Canvas>().worldCamera;
		}
	}

	public void SetCamera(UIRenderTarget target, Camera camera)
	{
		switch (target)
		{
		case UIRenderTarget.WorldSpace:
			worldSpaceCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		case UIRenderTarget.ScreenSpaceOverlay:
			ssOverlayCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		default:
			ssCameraCanvas.GetComponent<Canvas>().worldCamera = camera;
			break;
		}
	}

	public GameObject GetParent(UIRenderTarget target)
	{
		switch (target)
		{
		case UIRenderTarget.WorldSpace:
			return worldSpaceCanvas;
		case UIRenderTarget.ScreenSpaceOverlay:
			return ssOverlayCanvas;
		case UIRenderTarget.ScreenSpaceCamera:
			return ssCameraCanvas;
		case UIRenderTarget.HoverTextScreen:
			return ssHoverTextCanvas;
		default:
			return base.gameObject;
		}
	}

	public GameObject ActivateScreen(GameObject screen, GameObject parent = null, UIRenderTarget target = UIRenderTarget.ScreenSpaceOverlay)
	{
		if ((Object)parent == (Object)null)
		{
			parent = GetParent(target);
		}
		KScreenManager.AddExistingChild(parent, screen);
		KScreen component = screen.GetComponent<KScreen>();
		component.Activate();
		return screen;
	}

	public KScreen InstantiateScreen(GameObject screenPrefab, GameObject parent = null, UIRenderTarget target = UIRenderTarget.ScreenSpaceOverlay)
	{
		if ((Object)parent == (Object)null)
		{
			parent = GetParent(target);
		}
		GameObject gameObject = KScreenManager.AddChild(parent, screenPrefab);
		return gameObject.GetComponent<KScreen>();
	}

	public KScreen StartScreen(GameObject screenPrefab, GameObject parent = null, UIRenderTarget target = UIRenderTarget.ScreenSpaceOverlay)
	{
		if ((Object)parent == (Object)null)
		{
			parent = GetParent(target);
		}
		GameObject gameObject = KScreenManager.AddChild(parent, screenPrefab);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component;
	}
}
