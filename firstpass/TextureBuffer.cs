using System;
using UnityEngine;

[Serializable]
public class TextureBuffer
{
	public string name;

	public int bytesPerPixel;

	public TexturePagePool pool;

	public TextureFormat format;

	public RenderTexture texture;

	public Material material;

	public TextureBuffer(string name, int width, int height, TextureFormat format, FilterMode filter_mode, TexturePagePool pool)
	{
		this.name = name;
		this.format = format;
		this.pool = pool;
		texture = new RenderTexture(width, height, 0, TextureUtil.GetRenderTextureFormat(format));
		texture.name = name;
		texture.filterMode = filter_mode;
		texture.wrapMode = TextureWrapMode.Clamp;
		material = new Material(Shader.Find("Klei/TexturePage"));
	}

	public TextureRegion Lock(int x, int y, int width, int height)
	{
		TexturePage page = pool.Alloc(name, width, height, format);
		return new TextureRegion(x, y, page, this);
	}

	public void Unlock(TextureRegion region)
	{
		region.page.texture.LoadRawTextureData(region.page.bytes);
		region.page.texture.Apply();
		material.SetVector("_Region", new Vector4((float)region.x / (float)texture.width, (float)region.y / (float)texture.height, (float)(region.x + region.page.width) / (float)texture.width, (float)(region.y + region.page.height) / (float)texture.height));
		material.SetTexture("_MainTex", region.page.texture);
		Graphics.Blit(region.page.texture, texture, material);
		pool.Release(region.page);
	}
}
