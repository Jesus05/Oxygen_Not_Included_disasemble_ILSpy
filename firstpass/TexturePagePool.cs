using System.Collections.Generic;
using UnityEngine;

public class TexturePagePool
{
	private List<TexturePage>[] activePages = new List<TexturePage>[2];

	private List<TexturePage> freePages = new List<TexturePage>();

	public TexturePagePool()
	{
		activePages[0] = new List<TexturePage>();
		activePages[1] = new List<TexturePage>();
	}

	private int Clamp(int value)
	{
		if (value != 0)
		{
			if (value % 32 != 0)
			{
				return 32 + value / 32 * 32;
			}
			return value;
		}
		return 32;
	}

	public TexturePage Alloc(string name, int width, int height, TextureFormat format)
	{
		int num = Clamp(width);
		int num2 = Clamp(height);
		int num3 = Time.frameCount % 2;
		foreach (TexturePage item in activePages[num3])
		{
			freePages.Add(item);
		}
		activePages[num3].Clear();
		for (int i = 0; i < freePages.Count; i++)
		{
			TexturePage texturePage = freePages[i];
			if (texturePage.width == num && texturePage.height == num2 && texturePage.format == format)
			{
				freePages.RemoveAt(i);
				texturePage.SetName(name);
				return texturePage;
			}
		}
		return new TexturePage(name, num, num2, format);
	}

	public void Release(TexturePage page)
	{
		int num = (Time.frameCount + 1) % 2;
		activePages[num].Add(page);
	}
}
