using Rendering;
using System;
using UnityEngine;

public class BlockTileDecorInfo : ScriptableObject
{
	[Serializable]
	public struct ImageInfo
	{
		public string name;

		public Vector3 offset;

		[NonSerialized]
		public TextureAtlas.Item atlasItem;
	}

	[Serializable]
	public struct Decor
	{
		public string name;

		[EnumFlags]
		public BlockTileRenderer.Bits requiredConnections;

		[EnumFlags]
		public BlockTileRenderer.Bits forbiddenConnections;

		public float probabilityCutoff;

		public ImageInfo[] variants;

		public int sortOrder;
	}

	public TextureAtlas atlas;

	public TextureAtlas atlasSpec;

	public int sortOrder;

	public Decor[] decor;

	public void PostProcess()
	{
		if (decor != null && (UnityEngine.Object)atlas != (UnityEngine.Object)null && atlas.items != null)
		{
			for (int i = 0; i < decor.Length; i++)
			{
				if (decor[i].variants != null && decor[i].variants.Length != 0)
				{
					for (int j = 0; j < decor[i].variants.Length; j++)
					{
						bool flag = false;
						TextureAtlas.Item[] items = atlas.items;
						for (int k = 0; k < items.Length; k++)
						{
							TextureAtlas.Item atlasItem = items[k];
							string text = atlasItem.name;
							int num = text.IndexOf("/");
							if (num != -1)
							{
								text = text.Substring(num + 1);
							}
							if (decor[i].variants[j].name == text)
							{
								decor[i].variants[j].atlasItem = atlasItem;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Output.LogError(base.name, "/", decor[i].name, "could not find ", decor[i].variants[j].name, "in", atlas.name);
						}
					}
				}
			}
		}
	}
}
