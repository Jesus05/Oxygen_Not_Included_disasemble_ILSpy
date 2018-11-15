using UnityEngine;

public class GridCompositor : MonoBehaviour
{
	public Material material;

	public static GridCompositor Instance;

	private bool onMajor;

	private bool onMinor;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void Awake()
	{
		Instance = this;
		base.enabled = false;
	}

	private void Start()
	{
		material = new Material(Shader.Find("Klei/PostFX/GridCompositor"));
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, material);
	}

	public void ToggleMajor(bool on)
	{
		onMajor = on;
		Refresh();
	}

	public void ToggleMinor(bool on)
	{
		onMinor = on;
		Refresh();
	}

	private void Refresh()
	{
		base.enabled = (onMinor || onMajor);
	}
}
