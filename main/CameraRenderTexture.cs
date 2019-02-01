using System;
using UnityEngine;

public class CameraRenderTexture : MonoBehaviour
{
	public string TextureName;

	private RenderTexture resultTexture;

	private Material material;

	private void Awake()
	{
		material = new Material(Shader.Find("Klei/PostFX/CameraRenderTexture"));
	}

	private void Start()
	{
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
		OnResize();
	}

	private void OnResize()
	{
		if ((UnityEngine.Object)resultTexture != (UnityEngine.Object)null)
		{
			resultTexture.DestroyRenderTexture();
		}
		resultTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
		resultTexture.name = base.name;
		resultTexture.filterMode = FilterMode.Point;
		resultTexture.autoGenerateMips = false;
		if (TextureName != "")
		{
			Shader.SetGlobalTexture(TextureName, resultTexture);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, resultTexture, material);
	}

	public RenderTexture GetTexture()
	{
		return resultTexture;
	}

	public bool ShouldFlip()
	{
		return false;
	}
}
