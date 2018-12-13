using Klei;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PropertyTextures : KMonoBehaviour, ISim200ms
{
	public enum Property
	{
		StateChange,
		GasPressure,
		GasColour,
		GasDanger,
		FogOfWar,
		Flow,
		SolidDigAmount,
		SolidLiquidGasMass,
		WorldLight,
		Liquid,
		Temperature,
		ExposedToSunlight,
		Num
	}

	private struct TextureProperties
	{
		public string name;

		public Property simProperty;

		public TextureFormat textureFormat;

		public FilterMode filterMode;

		public bool updateEveryFrame;

		public bool updatedExternally;

		public bool blend;

		public float blendSpeed;

		public string texturePropertyName;
	}

	private struct WorkItem : IWorkItem<object>
	{
		public delegate void Callback(TextureRegion texture_region, int x0, int y0, int x1, int y1);

		private int x0;

		private int y0;

		private int x1;

		private int y1;

		private TextureRegion textureRegion;

		private Callback updateTextureCb;

		public WorkItem(TextureRegion texture_region, int x0, int y0, int x1, int y1, Callback update_texture_cb)
		{
			textureRegion = texture_region;
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;
			updateTextureCb = update_texture_cb;
		}

		public void Run(object shared_data)
		{
			updateTextureCb(textureRegion, x0, y0, x1, y1);
		}
	}

	[NonSerialized]
	public bool ForceLightEverywhere;

	[SerializeField]
	private Vector2 PressureRange = new Vector2(15f, 200f);

	[SerializeField]
	private float MinPressureVisibility = 0.1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float TemperatureStateChangeRange = 0.05f;

	public static PropertyTextures instance;

	public static IntPtr externalFlowTex;

	public static IntPtr externalLiquidTex;

	public static IntPtr externalExposedToSunlight;

	public static IntPtr externalSolidDigAmountTex;

	[SerializeField]
	private Vector2 coldRange;

	[SerializeField]
	private Vector2 hotRange;

	public static float FogOfWarScale;

	private int WorldSizeID;

	private int FogOfWarScaleID;

	private int PropTexWsToCsID;

	private int PropTexCsToWsID;

	private int TopBorderHeightID;

	private int NextPropertyIdx;

	public TextureBuffer[] textureBuffers;

	public TextureLerper[] lerpers;

	private TexturePagePool texturePagePool;

	[SerializeField]
	private Texture2D[] externallyUpdatedTextures;

	private TextureProperties[] textureProperties = new TextureProperties[12]
	{
		new TextureProperties
		{
			simProperty = Property.Flow,
			textureFormat = TextureFormat.RGFloat,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 0.25f
		},
		new TextureProperties
		{
			simProperty = Property.Liquid,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Point,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 1f
		},
		new TextureProperties
		{
			simProperty = Property.ExposedToSunlight,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = false,
			blendSpeed = 0f
		},
		new TextureProperties
		{
			simProperty = Property.SolidDigAmount,
			textureFormat = TextureFormat.RGB24,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new TextureProperties
		{
			simProperty = Property.GasColour,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		},
		new TextureProperties
		{
			simProperty = Property.GasDanger,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		},
		new TextureProperties
		{
			simProperty = Property.GasPressure,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		},
		new TextureProperties
		{
			simProperty = Property.FogOfWar,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new TextureProperties
		{
			simProperty = Property.WorldLight,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new TextureProperties
		{
			simProperty = Property.StateChange,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new TextureProperties
		{
			simProperty = Property.SolidLiquidGasMass,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Point,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		},
		new TextureProperties
		{
			simProperty = Property.Temperature,
			textureFormat = TextureFormat.RGB24,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		}
	};

	private List<TextureProperties> allTextureProperties = new List<TextureProperties>();

	private WorkItemCollection<WorkItem, object> workItems = new WorkItemCollection<WorkItem, object>();

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static WorkItem.Callback _003C_003Ef__mg_0024cache8;

	public static bool IsFogOfWarEnabled => FogOfWarScale < 1f;

	public static void DestroyInstance()
	{
		ShaderReloader.Unregister(instance.OnShadersReloaded);
		externalFlowTex = IntPtr.Zero;
		externalLiquidTex = IntPtr.Zero;
		externalExposedToSunlight = IntPtr.Zero;
		externalSolidDigAmountTex = IntPtr.Zero;
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		base.OnPrefabInit();
		ShaderReloader.Register(OnShadersReloaded);
	}

	public void SetFilterMode(Property property, FilterMode mode)
	{
		textureProperties[(int)property].filterMode = mode;
	}

	public Texture GetTexture(Property property)
	{
		return textureBuffers[(int)property].texture;
	}

	private string GetShaderPropertyName(Property property)
	{
		return "_" + property.ToString() + "Tex";
	}

	protected override void OnSpawn()
	{
		if (GenericGameSettings.instance.disableFogOfWar)
		{
			FogOfWarScale = 1f;
		}
		WorldSizeID = Shader.PropertyToID("_WorldSizeInfo");
		FogOfWarScaleID = Shader.PropertyToID("_FogOfWarScale");
		PropTexWsToCsID = Shader.PropertyToID("_PropTexWsToCs");
		PropTexCsToWsID = Shader.PropertyToID("_PropTexCsToWs");
		TopBorderHeightID = Shader.PropertyToID("_TopBorderHeight");
	}

	public void OnReset(object data = null)
	{
		lerpers = new TextureLerper[12];
		texturePagePool = new TexturePagePool();
		textureBuffers = new TextureBuffer[12];
		externallyUpdatedTextures = new Texture2D[12];
		for (int i = 0; i < 12; i++)
		{
			TextureProperties textureProperties = default(TextureProperties);
			textureProperties.textureFormat = TextureFormat.Alpha8;
			textureProperties.filterMode = FilterMode.Bilinear;
			textureProperties.blend = false;
			textureProperties.blendSpeed = 1f;
			TextureProperties item = textureProperties;
			for (int j = 0; j < this.textureProperties.Length; j++)
			{
				if (i == (int)this.textureProperties[j].simProperty)
				{
					item = this.textureProperties[j];
				}
			}
			Property property = (Property)i;
			item.name = property.ToString();
			if ((UnityEngine.Object)externallyUpdatedTextures[i] != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Destroy(externallyUpdatedTextures[i]);
				externallyUpdatedTextures[i] = null;
			}
			Texture texture;
			if (item.updatedExternally)
			{
				externallyUpdatedTextures[i] = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureUtil.TextureFormatToGraphicsFormat(item.textureFormat), TextureCreationFlags.None);
				texture = externallyUpdatedTextures[i];
			}
			else
			{
				TextureBuffer[] array = textureBuffers;
				int num = i;
				Property property2 = (Property)i;
				array[num] = new TextureBuffer(property2.ToString(), Grid.WidthInCells, Grid.HeightInCells, item.textureFormat, item.filterMode, texturePagePool);
				texture = textureBuffers[i].texture;
			}
			if (item.blend)
			{
				TextureLerper[] array2 = lerpers;
				int num2 = i;
				Texture target_texture = texture;
				Property property3 = (Property)i;
				array2[num2] = new TextureLerper(target_texture, property3.ToString(), texture.filterMode, item.textureFormat);
				lerpers[i].Speed = item.blendSpeed;
			}
			Shader.SetGlobalTexture(item.texturePropertyName = (texture.name = GetShaderPropertyName((Property)i)), texture);
			allTextureProperties.Add(item);
		}
	}

	private void OnShadersReloaded()
	{
		for (int i = 0; i < 12; i++)
		{
			TextureLerper textureLerper = lerpers[i];
			if (textureLerper != null)
			{
				TextureProperties textureProperties = allTextureProperties[i];
				Shader.SetGlobalTexture(textureProperties.texturePropertyName, textureLerper.Update());
			}
		}
	}

	public void Sim200ms(float dt)
	{
		if (lerpers != null && lerpers.Length != 0)
		{
			for (int i = 0; i < lerpers.Length; i++)
			{
				lerpers[i]?.LongUpdate(dt);
			}
		}
	}

	private void UpdateTextureThreaded(TextureRegion texture_region, int x0, int y0, int x1, int y1, WorkItem.Callback update_texture_cb)
	{
		workItems.Reset(null);
		int num = 16;
		for (int i = y0; i <= y1; i += num)
		{
			int y2 = Math.Min(i + num - 1, y1);
			workItems.Add(new WorkItem(texture_region, x0, i, x1, y2, update_texture_cb));
		}
		GlobalJobManager.Run(workItems);
	}

	private void UpdateProperty(ref TextureProperties p, int x0, int y0, int x1, int y1)
	{
		if (!Game.Instance.IsLoading())
		{
			int simProperty = (int)p.simProperty;
			if (!p.updatedExternally)
			{
				TextureRegion texture_region = textureBuffers[simProperty].Lock(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
				switch (p.simProperty)
				{
				case Property.StateChange:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateStateChange);
					break;
				case Property.GasPressure:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdatePressure);
					break;
				case Property.GasColour:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateGasColour);
					break;
				case Property.GasDanger:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateDanger);
					break;
				case Property.FogOfWar:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateFogOfWar);
					break;
				case Property.SolidDigAmount:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateSolidDigAmount);
					break;
				case Property.SolidLiquidGasMass:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateSolidLiquidGasMass);
					break;
				case Property.WorldLight:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateWorldLight);
					break;
				case Property.Temperature:
					UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateTemperature);
					break;
				}
				texture_region.Unlock();
			}
			else
			{
				switch (p.simProperty)
				{
				case Property.Flow:
					externallyUpdatedTextures[simProperty].LoadRawTextureData(externalFlowTex, 8 * Grid.WidthInCells * Grid.HeightInCells);
					break;
				case Property.Liquid:
					externallyUpdatedTextures[simProperty].LoadRawTextureData(externalLiquidTex, 4 * Grid.WidthInCells * Grid.HeightInCells);
					break;
				case Property.ExposedToSunlight:
					externallyUpdatedTextures[simProperty].LoadRawTextureData(externalExposedToSunlight, Grid.WidthInCells * Grid.HeightInCells);
					break;
				}
				externallyUpdatedTextures[simProperty].Apply();
			}
		}
	}

	private void LateUpdate()
	{
		if (Grid.IsInitialized())
		{
			Shader.SetGlobalVector(WorldSizeID, new Vector4((float)Grid.WidthInCells, (float)Grid.HeightInCells, 1f / (float)Grid.WidthInCells, 1f / (float)Grid.HeightInCells));
			Shader.SetGlobalVector(PropTexWsToCsID, new Vector4(0f, 0f, 1f, 1f));
			Shader.SetGlobalVector(PropTexCsToWsID, new Vector4(0f, 0f, 1f, 1f));
			Shader.SetGlobalFloat(TopBorderHeightID, (float)Grid.TopBorderHeight);
			GetVisibleCellRange(out int x, out int y, out int x2, out int y2);
			Shader.SetGlobalFloat(FogOfWarScaleID, FogOfWarScale);
			int num = NextPropertyIdx++ % allTextureProperties.Count;
			TextureProperties textureProperties = allTextureProperties[num];
			while (textureProperties.updateEveryFrame)
			{
				num = NextPropertyIdx++ % allTextureProperties.Count;
				textureProperties = allTextureProperties[num];
			}
			for (int i = 0; i < allTextureProperties.Count; i++)
			{
				TextureProperties p = allTextureProperties[i];
				if (num == i || p.updateEveryFrame)
				{
					UpdateProperty(ref p, x, y, x2, y2);
				}
			}
			for (int j = 0; j < 12; j++)
			{
				TextureLerper textureLerper = lerpers[j];
				if (textureLerper != null)
				{
					if (Time.timeScale == 0f)
					{
						textureLerper.LongUpdate(Time.unscaledDeltaTime);
					}
					TextureProperties textureProperties2 = allTextureProperties[j];
					Shader.SetGlobalTexture(textureProperties2.texturePropertyName, textureLerper.Update());
				}
			}
		}
	}

	private void GetVisibleCellRange(out int x0, out int y0, out int x1, out int y1)
	{
		int num = 16;
		Grid.GetVisibleExtents(out x0, out y0, out x1, out y1);
		x0 = Math.Max(0, x0 - num);
		y0 = Math.Max(0, y0 - num);
		x0 = Mathf.Min(x0, Grid.WidthInCells - 1);
		y0 = Mathf.Min(y0, Grid.HeightInCells - 1);
		x1 = Mathf.CeilToInt((float)(x1 + num));
		y1 = Mathf.CeilToInt((float)(y1 + num));
		x1 = Mathf.Max(x1, 0);
		y1 = Mathf.Max(y1, 0);
		x1 = Mathf.Min(x1, Grid.WidthInCells - 1);
		y1 = Mathf.Min(y1, Grid.HeightInCells - 1);
	}

	private static void UpdateFogOfWar(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		byte[] visible = Grid.Visible;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				region.SetBytes(j, i, visible[num]);
			}
		}
	}

	private static void UpdatePressure(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		Vector2 pressureRange = instance.PressureRange;
		float minPressureVisibility = instance.MinPressureVisibility;
		float num = pressureRange.y - pressureRange.x;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num2 = Grid.XYToCell(j, i);
				float num3 = 0f;
				Element element = Grid.Element[num2];
				if (element.IsGas)
				{
					float num4 = Grid.Pressure[num2];
					float b = (!(num4 > 0f)) ? 0f : minPressureVisibility;
					num3 = Mathf.Max(Mathf.Clamp01((num4 - pressureRange.x) / num), b);
				}
				else if (element.IsLiquid)
				{
					int num5 = Grid.CellAbove(num2);
					if (Grid.IsValidCell(num5) && Grid.Element[num5].IsGas)
					{
						float num6 = Grid.Pressure[num5];
						float b2 = (!(num6 > 0f)) ? 0f : minPressureVisibility;
						num3 = Mathf.Max(Mathf.Clamp01((num6 - pressureRange.x) / num), b2);
					}
				}
				region.SetBytes(j, i, (byte)(num3 * 255f));
			}
		}
	}

	private static void UpdateDanger(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				byte b = (byte)((element.id != SimHashes.Oxygen) ? 255 : 0);
				region.SetBytes(j, i, b);
			}
		}
	}

	private static void UpdateStateChange(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		float temperatureStateChangeRange = instance.TemperatureStateChangeRange;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				float num2 = 0f;
				Element element = Grid.Element[num];
				if (!element.IsVacuum)
				{
					float num3 = Grid.Temperature[num];
					float num4 = element.lowTemp * temperatureStateChangeRange;
					float num5 = Mathf.Abs(num3 - element.lowTemp);
					float a = num5 / num4;
					float num6 = element.highTemp * temperatureStateChangeRange;
					float num7 = Mathf.Abs(num3 - element.highTemp);
					float b = num7 / num6;
					num2 = Mathf.Max(num2, 1f - Mathf.Min(a, b));
				}
				region.SetBytes(j, i, (byte)(num2 * 255f));
			}
		}
	}

	private static void UpdateGasColour(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				if (element.IsGas)
				{
					region.SetBytes(j, i, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b, byte.MaxValue);
				}
				else if (element.IsLiquid)
				{
					int cell = Grid.CellAbove(num);
					if (Grid.IsValidCell(cell))
					{
						region.SetBytes(j, i, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b, byte.MaxValue);
					}
					else
					{
						region.SetBytes(j, i, 0, 0, 0, 0);
					}
				}
				else
				{
					region.SetBytes(j, i, 0, 0, 0, 0);
				}
			}
		}
	}

	private static void UpdateLiquid(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = x0; i <= x1; i++)
		{
			int num = Grid.XYToCell(i, y1);
			Element element = Grid.Element[num];
			for (int num2 = y1; num2 >= y0; num2--)
			{
				int num3 = Grid.XYToCell(i, num2);
				Element element2 = Grid.Element[num3];
				if (element2.IsLiquid)
				{
					Color32 colour = element2.substance.colour;
					float liquidMaxMass = Lighting.Instance.Settings.LiquidMaxMass;
					float liquidAmountOffset = Lighting.Instance.Settings.LiquidAmountOffset;
					float num4;
					if (element.IsLiquid || element.IsSolid)
					{
						num4 = 1f;
					}
					else
					{
						num4 = liquidAmountOffset + (1f - liquidAmountOffset) * Mathf.Min(Grid.Mass[num3] / liquidMaxMass, 1f);
						num4 = Mathf.Pow(Mathf.Min(Grid.Mass[num3] / liquidMaxMass, 1f), 0.45f);
					}
					region.SetBytes(i, num2, (byte)(num4 * 255f), colour.r, colour.g, colour.b);
				}
				else
				{
					region.SetBytes(i, num2, 0, 0, 0, 0);
				}
				element = element2;
			}
		}
	}

	private static void UpdateSolidDigAmount(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		int elementIndex = ElementLoader.GetElementIndex(SimHashes.Void);
		for (int i = y0; i <= y1; i++)
		{
			int num = Grid.XYToCell(x0, i);
			int num2 = Grid.XYToCell(x1, i);
			int num3 = num;
			int num4 = x0;
			while (num3 <= num2)
			{
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				if (Grid.ElementIdx[num3] != elementIndex)
				{
					b3 = byte.MaxValue;
				}
				if (Grid.Solid[num3])
				{
					b = byte.MaxValue;
					b2 = (byte)(255f * Grid.Damage[num3]);
				}
				region.SetBytes(num4, i, b, b2, b3);
				num3++;
				num4++;
			}
		}
	}

	private static void UpdateSolidLiquidGasMass(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				Element element = Grid.Element[num];
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				if (element.IsSolid)
				{
					b = byte.MaxValue;
				}
				else if (element.IsLiquid)
				{
					b2 = byte.MaxValue;
				}
				else if (element.IsGas || element.IsVacuum)
				{
					b3 = byte.MaxValue;
				}
				float num2 = Grid.Mass[num];
				float num3 = Mathf.Min(1f, num2 / 2000f);
				if (num2 > 0f)
				{
					num3 = Mathf.Max(0.003921569f, num3);
				}
				region.SetBytes(j, i, b, b2, b3, (byte)(num3 * 255f));
			}
		}
	}

	private static void GetTemperatureAlpha(float t, Vector2 cold_range, Vector2 hot_range, out byte cold_alpha, out byte hot_alpha)
	{
		cold_alpha = 0;
		hot_alpha = 0;
		if (t <= cold_range.y)
		{
			float num = Mathf.Clamp01((cold_range.y - t) / (cold_range.y - cold_range.x));
			cold_alpha = (byte)(num * 255f);
		}
		else if (t >= hot_range.x)
		{
			float num2 = Mathf.Clamp01((t - hot_range.x) / (hot_range.y - hot_range.x));
			hot_alpha = (byte)(num2 * 255f);
		}
	}

	private static void UpdateTemperature(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		Vector2 cold_range = instance.coldRange;
		Vector2 hot_range = instance.hotRange;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int i2 = Grid.XYToCell(j, i);
				float num = Grid.Temperature[i2];
				GetTemperatureAlpha(num, cold_range, hot_range, out byte cold_alpha, out byte hot_alpha);
				byte b = (byte)(255f * Mathf.Pow(Mathf.Clamp(num / 1000f, 0f, 1f), 0.45f));
				region.SetBytes(j, i, cold_alpha, hot_alpha, b);
			}
		}
	}

	private static void UpdateWorldLight(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		if (!instance.ForceLightEverywhere)
		{
			for (int i = y0; i <= y1; i++)
			{
				int num = Grid.XYToCell(x0, i);
				int num2 = Grid.XYToCell(x1, i);
				int num3 = num;
				int num4 = x0;
				while (num3 <= num2)
				{
					Color32 color = (Grid.LightCount[num3] <= 0) ? new Color32(0, 0, 0, byte.MaxValue) : Lighting.Instance.Settings.LightColour;
					region.SetBytes(num4, i, color.r, color.g, color.b, (byte)((color.r + color.g + color.b > 0) ? 255 : 0));
					num3++;
					num4++;
				}
			}
		}
		else
		{
			for (int j = y0; j <= y1; j++)
			{
				for (int k = x0; k <= x1; k++)
				{
					region.SetBytes(k, j, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				}
			}
		}
	}
}
