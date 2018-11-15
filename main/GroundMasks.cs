using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundMasks : ScriptableObject
{
	public struct UVData
	{
		public Vector2 bl;

		public Vector2 br;

		public Vector2 tl;

		public Vector2 tr;

		public UVData(Vector2 bl, Vector2 br, Vector2 tl, Vector2 tr)
		{
			this.bl = bl;
			this.br = br;
			this.tl = tl;
			this.tr = tr;
		}
	}

	public struct Tile
	{
		public bool isSource;

		public UVData[] variationUVs;
	}

	public class BiomeMaskData
	{
		public string name;

		public Tile[] tiles;

		public BiomeMaskData(string name)
		{
			this.name = name;
			tiles = new Tile[16];
		}

		public void GenerateRotations()
		{
			for (int i = 1; i < 15; i++)
			{
				if (!tiles[i].isSource)
				{
					Tile tile = tiles[i];
					tile.variationUVs = GetNonNullRotationUVs(i);
					tiles[i] = tile;
				}
			}
		}

		public UVData[] GetNonNullRotationUVs(int dest_mask)
		{
			UVData[] array = null;
			int num = dest_mask;
			for (int i = 0; i < 3; i++)
			{
				int num2 = (num & 1) >> 0;
				int num3 = (num & 2) >> 1;
				int num4 = (num & 4) >> 2;
				int num5 = (num & 8) >> 3;
				int num6 = (num5 << 2) | (num4 << 0) | (num3 << 3) | (num2 << 1);
				if (tiles[num6].isSource)
				{
					array = new UVData[tiles[num6].variationUVs.Length];
					for (int j = 0; j < tiles[num6].variationUVs.Length; j++)
					{
						UVData uVData = tiles[num6].variationUVs[j];
						UVData uVData2 = uVData;
						switch (i)
						{
						case 2:
							uVData2 = new UVData(uVData.br, uVData.tr, uVData.bl, uVData.tl);
							break;
						case 1:
							uVData2 = new UVData(uVData.tr, uVData.tl, uVData.br, uVData.bl);
							break;
						case 0:
							uVData2 = new UVData(uVData.tl, uVData.bl, uVData.tr, uVData.br);
							break;
						default:
							Output.LogError("Unhandled rotation case");
							break;
						}
						array[j] = uVData2;
					}
					break;
				}
				num = num6;
			}
			return array;
		}

		public void Validate()
		{
			for (int i = 1; i < tiles.Length; i++)
			{
				Tile tile = tiles[i];
				if (tile.variationUVs == null)
				{
					Output.LogError(name, "has invalid tile at index", i);
				}
			}
		}
	}

	public TextureAtlas maskAtlas;

	[NonSerialized]
	public Dictionary<string, BiomeMaskData> biomeMasks;

	public void Initialize()
	{
		if (!((UnityEngine.Object)maskAtlas == (UnityEngine.Object)null) && maskAtlas.items != null)
		{
			biomeMasks = new Dictionary<string, BiomeMaskData>();
			TextureAtlas.Item[] items = maskAtlas.items;
			for (int i = 0; i < items.Length; i++)
			{
				TextureAtlas.Item item = items[i];
				string name = item.name;
				int num = name.IndexOf('/');
				string text = name.Substring(0, num);
				string value = name.Substring(num + 1, 4);
				text = text.ToLower();
				for (int num2 = text.IndexOf('_'); num2 != -1; num2 = text.IndexOf('_'))
				{
					text = text.Remove(num2, 1);
				}
				BiomeMaskData value2 = null;
				if (!biomeMasks.TryGetValue(text, out value2))
				{
					value2 = new BiomeMaskData(text);
					biomeMasks[text] = value2;
				}
				int num3 = Convert.ToInt32(value, 2);
				Tile tile = value2.tiles[num3];
				if (tile.variationUVs == null)
				{
					tile.isSource = true;
					tile.variationUVs = new UVData[1];
				}
				else
				{
					UVData[] array = new UVData[tile.variationUVs.Length + 1];
					Array.Copy(tile.variationUVs, array, tile.variationUVs.Length);
					tile.variationUVs = array;
				}
				Vector4 vector = new Vector4(item.uvBox.x, item.uvBox.w, item.uvBox.z, item.uvBox.y);
				Vector2 bl = new Vector2(vector.x, vector.y);
				Vector2 br = new Vector2(vector.z, vector.y);
				Vector2 tl = new Vector2(vector.x, vector.w);
				Vector2 tr = new Vector2(vector.z, vector.w);
				UVData uVData = new UVData(bl, br, tl, tr);
				tile.variationUVs[tile.variationUVs.Length - 1] = uVData;
				value2.tiles[num3] = tile;
			}
			foreach (KeyValuePair<string, BiomeMaskData> biomeMask in biomeMasks)
			{
				biomeMask.Value.GenerateRotations();
				biomeMask.Value.Validate();
			}
		}
	}

	[ContextMenu("Regenerate")]
	private void Regenerate()
	{
		Initialize();
		foreach (KeyValuePair<string, BiomeMaskData> biomeMask in biomeMasks)
		{
			BiomeMaskData value = biomeMask.Value;
			Output.Log(value.name);
			for (int i = 1; i < value.tiles.Length; i++)
			{
				Tile tile = value.tiles[i];
				Output.Log("Tile", i, "has", tile.variationUVs.Length, "variations");
			}
		}
	}
}
