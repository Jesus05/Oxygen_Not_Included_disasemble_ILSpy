using UnityEngine;

public class TexturePage
{
	public int width;

	public int height;

	public TextureFormat format;

	public TexturePagePool pool;

	public Texture2D texture;

	public byte[] bytes;

	public TexturePage(string name, int width, int height, TextureFormat format)
	{
		this.width = width;
		this.height = height;
		this.format = format;
		texture = new Texture2D(width, height, format, false);
		texture.name = name;
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		bytes = new byte[width * height * TextureUtil.GetBytesPerPixel(format)];
		SetName(name);
	}

	public void SetName(string name)
	{
		texture.name = name;
	}
}
