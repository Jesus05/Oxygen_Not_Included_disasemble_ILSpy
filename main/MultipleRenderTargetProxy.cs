using System;
using UnityEngine;

public class MultipleRenderTargetProxy : MonoBehaviour
{
	public RenderTexture[] Textures = new RenderTexture[3];

	private bool colouredOverlayBufferEnabled = false;

	private void Start()
	{
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
		CreateRenderTarget();
		ShaderReloader.Register(OnShadersReloaded);
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		colouredOverlayBufferEnabled = enabled;
		CreateRenderTarget();
	}

	private void CreateRenderTarget()
	{
		RenderBuffer[] array = new RenderBuffer[(!colouredOverlayBufferEnabled) ? 2 : 3];
		Textures[0] = RecreateRT(Textures[0], 24, RenderTextureFormat.ARGB32);
		Textures[0].filterMode = FilterMode.Point;
		Textures[0].name = "MRT0";
		Textures[1] = RecreateRT(Textures[1], 0, RenderTextureFormat.ARGB32);
		Textures[1].filterMode = FilterMode.Point;
		Textures[1].name = "MRT1";
		array[0] = Textures[0].colorBuffer;
		array[1] = Textures[1].colorBuffer;
		if (colouredOverlayBufferEnabled)
		{
			Textures[2] = RecreateRT(Textures[2], 0, RenderTextureFormat.ARGB32);
			Textures[2].filterMode = FilterMode.Bilinear;
			Textures[2].name = "MRT2";
			array[2] = Textures[2].colorBuffer;
		}
		Camera component = GetComponent<Camera>();
		component.SetTargetBuffers(array, Textures[0].depthBuffer);
		OnShadersReloaded();
	}

	private RenderTexture RecreateRT(RenderTexture rt, int depth, RenderTextureFormat format)
	{
		RenderTexture result = rt;
		if ((UnityEngine.Object)rt == (UnityEngine.Object)null || rt.width != Screen.width || rt.height != Screen.height || rt.format != format)
		{
			if ((UnityEngine.Object)rt != (UnityEngine.Object)null)
			{
				rt.DestroyRenderTexture();
			}
			result = new RenderTexture(Screen.width, Screen.height, depth, format);
		}
		return result;
	}

	private void OnResize()
	{
		CreateRenderTarget();
	}

	private void Update()
	{
		if (!Textures[0].IsCreated())
		{
			CreateRenderTarget();
		}
	}

	private void OnShadersReloaded()
	{
		Shader.SetGlobalTexture("_MRT0", Textures[0]);
		Shader.SetGlobalTexture("_MRT1", Textures[1]);
		if (colouredOverlayBufferEnabled)
		{
			Shader.SetGlobalTexture("_MRT2", Textures[2]);
		}
	}
}
