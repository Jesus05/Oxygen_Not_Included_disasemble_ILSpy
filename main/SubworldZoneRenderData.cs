using Delaunay.Geo;
using Klei;
using ProcGen;
using UnityEngine;

public class SubworldZoneRenderData : KMonoBehaviour
{
	[SerializeField]
	private Texture2D colourTex = null;

	[SerializeField]
	private Texture2D indexTex = null;

	[HideInInspector]
	public SubWorld.ZoneType[] worldZoneTypes;

	[SerializeField]
	[HideInInspector]
	public Color32[] zoneColours = new Color32[11]
	{
		new Color32(145, 198, 213, 0),
		new Color32(135, 82, 160, 1),
		new Color32(123, 151, 75, 2),
		new Color32(236, 189, 89, 3),
		new Color32(201, 152, 181, 4),
		new Color32(222, 90, 59, 5),
		new Color32(201, 152, 181, 6),
		new Color32(byte.MaxValue, 0, 0, 7),
		new Color32(201, 201, 151, 8),
		new Color32(236, 90, 110, 9),
		new Color32(110, 236, 110, 10)
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GenerateTexture();
	}

	public void GenerateTexture()
	{
		colourTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureFormat.RGB24, false);
		colourTex.name = "SubworldRegionColourData";
		colourTex.filterMode = FilterMode.Bilinear;
		colourTex.wrapMode = TextureWrapMode.Clamp;
		colourTex.anisoLevel = 0;
		indexTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureFormat.Alpha8, false);
		indexTex.name = "SubworldRegionIndexData";
		indexTex.filterMode = FilterMode.Point;
		indexTex.wrapMode = TextureWrapMode.Clamp;
		indexTex.anisoLevel = 0;
		byte[] array = new byte[Grid.WidthInCells * Grid.HeightInCells * 3];
		byte[] array2 = new byte[Grid.WidthInCells * Grid.HeightInCells];
		worldZoneTypes = new SubWorld.ZoneType[Grid.CellCount];
		WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
		{
			WorldDetailSave.OverworldCell overworldCell = worldDetailSave.overworldCells[i];
			Polygon poly = overworldCell.poly;
			zero.y = (float)(int)Mathf.Floor(poly.bounds.yMin);
			while (zero.y < Mathf.Ceil(poly.bounds.yMax))
			{
				zero.x = (float)(int)Mathf.Floor(poly.bounds.xMin);
				while (zero.x < Mathf.Ceil(poly.bounds.xMax))
				{
					if (poly.Contains(zero))
					{
						int num = (int)(zero.x + zero.y * (float)Grid.WidthInCells);
						array2[num] = (byte)((overworldCell.zoneType != SubWorld.ZoneType.Space) ? ((byte)overworldCell.zoneType) : 255);
						Color32 color = zoneColours[(int)overworldCell.zoneType];
						array[num * 3] = color.r;
						array[num * 3 + 1] = color.g;
						array[num * 3 + 2] = color.b;
						int num2 = Grid.XYToCell((int)zero.x, (int)zero.y);
						if (Grid.IsValidCell(num2))
						{
							worldZoneTypes[num2] = overworldCell.zoneType;
						}
					}
					zero.x += 1f;
				}
				zero.y += 1f;
			}
		}
		colourTex.LoadRawTextureData(array);
		indexTex.LoadRawTextureData(array2);
		colourTex.Apply();
		indexTex.Apply();
		OnShadersReloaded();
		ShaderReloader.Register(OnShadersReloaded);
		InitSimZones(array2);
	}

	private void OnShadersReloaded()
	{
		Shader.SetGlobalTexture("_WorldZoneTex", colourTex);
		Shader.SetGlobalTexture("_WorldZoneIndexTex", indexTex);
	}

	public SubWorld.ZoneType GetSubWorldZoneType(int cell)
	{
		if (cell >= 0 && cell < worldZoneTypes.Length)
		{
			return worldZoneTypes[cell];
		}
		return SubWorld.ZoneType.Sandstone;
	}

	private SubWorld.ZoneType GetSubWorldZoneType(Vector2I pos)
	{
		WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
		if (worldDetailSave != null)
		{
			for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
			{
				if (worldDetailSave.overworldCells[i].poly.Contains(pos))
				{
					return worldDetailSave.overworldCells[i].zoneType;
				}
			}
		}
		return SubWorld.ZoneType.Sandstone;
	}

	private Color32 GetZoneColor(SubWorld.ZoneType zone_type)
	{
		Color32 result = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 3);
		Debug.Assert((int)zone_type < zoneColours.Length, "Need to add more colours to handle this zone" + (int)zone_type + "<" + zoneColours.Length);
		return result;
	}

	private unsafe void InitSimZones(byte[] bytes)
	{
		//IL_0018: Incompatible stack types: I vs Ref
		fixed (byte* msg = &((bytes != null && bytes.Length != 0) ? ref bytes[0] : ref *(byte*)null))
		{
			Sim.SIM_HandleMessage(-457308393, bytes.Length, msg);
		}
	}
}
