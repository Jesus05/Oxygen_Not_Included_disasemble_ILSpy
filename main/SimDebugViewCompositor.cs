using UnityEngine;

public class SimDebugViewCompositor : MonoBehaviour
{
	public Material material;

	public static SimDebugViewCompositor Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Start()
	{
		material = new Material(Shader.Find("Klei/PostFX/SimDebugViewCompositor"));
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, material);
	}

	public void Toggle(bool is_on)
	{
		base.enabled = is_on;
	}
}
