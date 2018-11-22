using System;
using UnityEngine;

public class BloomEffect : MonoBehaviour
{
	private Material BloomMaskMaterial;

	private Material BloomCompositeMaterial;

	public int iterations = 3;

	public float blurSpread = 0.6f;

	public Shader blurShader = null;

	private Material m_Material = null;

	protected Material material
	{
		get
		{
			if ((UnityEngine.Object)m_Material == (UnityEngine.Object)null)
			{
				m_Material = new Material(blurShader);
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		}
	}

	protected void OnDisable()
	{
		if ((bool)m_Material)
		{
			UnityEngine.Object.DestroyImmediate(m_Material);
		}
	}

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		else if (!(bool)blurShader || !material.shader.isSupported)
		{
			base.enabled = false;
		}
		else
		{
			BloomMaskMaterial = new Material(Shader.Find("Klei/PostFX/BloomMask"));
			BloomCompositeMaterial = new Material(Shader.Find("Klei/PostFX/BloomComposite"));
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * blurSpread;
		Graphics.BlitMultiTap(source, dest, material, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, material, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		temporary.name = "bloom_source";
		Graphics.Blit(source, temporary, BloomMaskMaterial);
		int width = Math.Max(source.width / 4, 4);
		int height = Math.Max(source.height / 4, 4);
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
		renderTexture.name = "bloom_downsampled";
		DownSample4x(temporary, renderTexture);
		RenderTexture.ReleaseTemporary(temporary);
		for (int i = 0; i < iterations; i++)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0);
			temporary2.name = "bloom_blurred";
			FourTapCone(renderTexture, temporary2, i);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary2;
		}
		BloomCompositeMaterial.SetTexture("_BloomTex", renderTexture);
		Graphics.Blit(source, destination, BloomCompositeMaterial);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
