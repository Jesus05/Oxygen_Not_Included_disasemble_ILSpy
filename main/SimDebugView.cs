using Klei;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class SimDebugView : KMonoBehaviour
{
	public static class OverlayModes
	{
		public static readonly HashedString Mass = "Mass";

		public static readonly HashedString Pressure = "Pressure";

		public static readonly HashedString GameGrid = "GameGrid";

		public static readonly HashedString ScenePartitioner = "ScenePartitioner";

		public static readonly HashedString ConduitUpdates = "ConduitUpdates";

		public static readonly HashedString Flow = "Flow";

		public static readonly HashedString StateChange = "StateChange";

		public static readonly HashedString SimCheckErrorMap = "SimCheckErrorMap";

		public static readonly HashedString DupePassable = "DupePassable";

		public static readonly HashedString Foundation = "Foundation";

		public static readonly HashedString FakeFloor = "FakeFloor";

		public static readonly HashedString CritterImpassable = "CritterImpassable";

		public static readonly HashedString DupeImpassable = "DupeImpassable";

		public static readonly HashedString MinionGroupProber = "MinionGroupProber";

		public static readonly HashedString PathProber = "PathProber";

		public static readonly HashedString Reserved = "Reserved";

		public static readonly HashedString AllowPathFinding = "AllowPathFinding";

		public static readonly HashedString Danger = "Danger";

		public static readonly HashedString MinionOccupied = "MinionOccupied";

		public static readonly HashedString TileType = "TileType";

		public static readonly HashedString State = "State";

		public static readonly HashedString SolidLiquid = "SolidLiquid";

		public static readonly HashedString Joules = "Joules";
	}

	public enum GameGridMode
	{
		GameSolidMap,
		Lighting,
		RoomMap,
		Style,
		PlantDensity,
		DigAmount,
		DupePassable
	}

	[Serializable]
	public struct ColorThreshold
	{
		public Color color;

		public float value;
	}

	private struct UpdateSimViewSharedData
	{
		public SimDebugView instance;

		public HashedString simViewMode;

		public SimDebugView simDebugView;

		public byte[] textureBytes;

		public UpdateSimViewSharedData(SimDebugView instance, byte[] texture_bytes, HashedString sim_view_mode, SimDebugView sim_debug_view)
		{
			this.instance = instance;
			textureBytes = texture_bytes;
			simViewMode = sim_view_mode;
			simDebugView = sim_debug_view;
		}
	}

	private struct UpdateSimViewWorkItem : IWorkItem<UpdateSimViewSharedData>
	{
		private int x0;

		private int y0;

		private int x1;

		private int y1;

		[CompilerGenerated]
		private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache0;

		public UpdateSimViewWorkItem(int x0, int y0, int x1, int y1)
		{
			this.x0 = Mathf.Clamp(x0, 0, Grid.WidthInCells - 1);
			this.x1 = Mathf.Clamp(x1, 0, Grid.WidthInCells - 1);
			this.y0 = Mathf.Clamp(y0, 0, Grid.HeightInCells - 1);
			this.y1 = Mathf.Clamp(y1, 0, Grid.HeightInCells - 1);
		}

		public void Run(UpdateSimViewSharedData shared_data)
		{
			if (!shared_data.instance.getColourFuncs.TryGetValue(shared_data.simViewMode, out Func<SimDebugView, int, Color> value))
			{
				value = GetBlack;
			}
			for (int i = y0; i <= y1; i++)
			{
				int num = Grid.XYToCell(x0, i);
				int num2 = Grid.XYToCell(x1, i);
				for (int j = num; j <= num2; j++)
				{
					Color color = value(shared_data.instance, j);
					int num3 = j * 4;
					shared_data.textureBytes[num3] = (byte)(Mathf.Min(color.r, 1f) * 255f);
					shared_data.textureBytes[num3 + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
					shared_data.textureBytes[num3 + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
					shared_data.textureBytes[num3 + 3] = (byte)(Mathf.Min(color.a, 1f) * 255f);
				}
			}
		}
	}

	public enum DangerAmount
	{
		None = 0,
		VeryLow = 1,
		Low = 2,
		Moderate = 3,
		High = 4,
		VeryHigh = 5,
		Extreme = 6,
		MAX_DANGERAMOUNT = 6
	}

	[SerializeField]
	public Material material;

	public Material diseaseMaterial;

	public bool hideFOW = false;

	public const int colourSize = 4;

	private byte[] texBytes;

	private int currentFrame;

	[SerializeField]
	private Texture2D tex;

	[SerializeField]
	private GameObject plane;

	private HashedString mode = global::OverlayModes.Power.ID;

	private GameGridMode gameGridMode = GameGridMode.DigAmount;

	private PathProber selectedPathProber;

	public float minTempExpected = 173.15f;

	public float maxTempExpected = 423.15f;

	public float minMassExpected = 1.0001f;

	public float maxMassExpected = 10000f;

	public float minPressureExpected = 1.300003f;

	public float maxPressureExpected = 201.3f;

	public float minThermalConductivity = 0f;

	public float maxThermalConductivity = 30f;

	public float thresholdRange = 0.001f;

	public float thresholdOpacity = 0.8f;

	public static float minimumBreathable = 0.05f;

	public static float optimallyBreathable = 1f;

	public ColorThreshold[] temperatureThresholds;

	public ColorThreshold[] heatFlowThresholds;

	public Color32[] networkColours;

	public Gradient breathableGradient = new Gradient();

	public Color32 unbreathableColour = new Color(0.5f, 0f, 0f);

	public Color32[] toxicColour = new Color32[2]
	{
		new Color(0.5f, 0f, 0.5f),
		new Color(1f, 0f, 1f)
	};

	public static SimDebugView Instance;

	private WorkItemCollection<UpdateSimViewWorkItem, UpdateSimViewSharedData> updateSimViewWorkItems = new WorkItemCollection<UpdateSimViewWorkItem, UpdateSimViewSharedData>();

	private int selectedCell;

	private Dictionary<HashedString, Action<SimDebugView, Texture>> dataUpdateFuncs = new Dictionary<HashedString, Action<SimDebugView, Texture>>
	{
		{
			global::OverlayModes.Temperature.ID,
			SetDefaultBilinear
		},
		{
			global::OverlayModes.Oxygen.ID,
			SetDefaultBilinear
		},
		{
			global::OverlayModes.Decor.ID,
			SetDefaultBilinear
		},
		{
			global::OverlayModes.TileMode.ID,
			SetDefaultPoint
		},
		{
			global::OverlayModes.Disease.ID,
			SetDisease
		}
	};

	private Dictionary<HashedString, Func<SimDebugView, int, Color>> getColourFuncs = new Dictionary<HashedString, Func<SimDebugView, int, Color>>
	{
		{
			global::OverlayModes.ThermalConductivity.ID,
			GetThermalConductivityColour
		},
		{
			global::OverlayModes.Temperature.ID,
			GetNormalizedTemperatureColourMode
		},
		{
			global::OverlayModes.Disease.ID,
			GetDiseaseColour
		},
		{
			global::OverlayModes.Decor.ID,
			GetDecorColour
		},
		{
			global::OverlayModes.Oxygen.ID,
			GetOxygenMapColour
		},
		{
			global::OverlayModes.Light.ID,
			GetLightColour
		},
		{
			global::OverlayModes.Radiation.ID,
			GetRadiationColour
		},
		{
			global::OverlayModes.Rooms.ID,
			GetRoomsColour
		},
		{
			global::OverlayModes.TileMode.ID,
			GetTileColour
		},
		{
			global::OverlayModes.Suit.ID,
			GetBlack
		},
		{
			global::OverlayModes.Priorities.ID,
			GetBlack
		},
		{
			global::OverlayModes.Crop.ID,
			GetBlack
		},
		{
			global::OverlayModes.Harvest.ID,
			GetBlack
		},
		{
			OverlayModes.GameGrid,
			GetGameGridColour
		},
		{
			OverlayModes.StateChange,
			GetStateChangeColour
		},
		{
			OverlayModes.SimCheckErrorMap,
			GetSimCheckErrorMapColour
		},
		{
			OverlayModes.Foundation,
			GetFoundationColour
		},
		{
			OverlayModes.FakeFloor,
			GetFakeFloorColour
		},
		{
			OverlayModes.DupePassable,
			GetDupePassableColour
		},
		{
			OverlayModes.DupeImpassable,
			GetDupeImpassableColour
		},
		{
			OverlayModes.CritterImpassable,
			GetCritterImpassableColour
		},
		{
			OverlayModes.MinionGroupProber,
			GetMinionGroupProberColour
		},
		{
			OverlayModes.PathProber,
			GetPathProberColour
		},
		{
			OverlayModes.Reserved,
			GetReservedColour
		},
		{
			OverlayModes.AllowPathFinding,
			GetAllowPathFindingColour
		},
		{
			OverlayModes.Danger,
			GetDangerColour
		},
		{
			OverlayModes.MinionOccupied,
			GetMinionOccupiedColour
		},
		{
			OverlayModes.Pressure,
			GetPressureMapColour
		},
		{
			OverlayModes.TileType,
			GetTileTypeColour
		},
		{
			OverlayModes.State,
			GetStateMapColour
		},
		{
			OverlayModes.SolidLiquid,
			GetSolidLiquidMapColour
		},
		{
			OverlayModes.Mass,
			GetMassColour
		},
		{
			OverlayModes.Joules,
			GetJoulesColour
		}
	};

	public static readonly Color[] dbColours = new Color[13]
	{
		new Color(0f, 0f, 0f, 0f),
		new Color(1f, 1f, 1f, 0.3f),
		new Color(0.7058824f, 0.8235294f, 1f, 0.2f),
		new Color(0f, 0.3137255f, 1f, 0.3f),
		new Color(0.7058824f, 1f, 0.7058824f, 0.5f),
		new Color(0.0784313753f, 1f, 0f, 0.7f),
		new Color(1f, 0.9019608f, 0.7058824f, 0.9f),
		new Color(1f, 0.8235294f, 0f, 0.9f),
		new Color(1f, 0.7176471f, 0.3019608f, 0.9f),
		new Color(1f, 0.41568628f, 0f, 0.9f),
		new Color(1f, 0.7058824f, 0.7058824f, 1f),
		new Color(1f, 0f, 0f, 1f),
		new Color(1f, 0f, 0f, 1f)
	};

	private static float minMinionTemperature = 260f;

	private static float maxMinionTemperature = 310f;

	private static float minMinionPressure = 80f;

	[CompilerGenerated]
	private static Action<SimDebugView, Texture> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<SimDebugView, Texture> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Action<SimDebugView, Texture> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<SimDebugView, Texture> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Action<SimDebugView, Texture> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Action<SimDebugView, Texture> _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache8;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache9;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cacheA;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cacheB;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cacheC;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cacheD;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cacheE;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cacheF;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache10;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache11;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache12;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache13;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache14;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache15;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache16;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache17;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache18;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache19;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache1A;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache1B;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache1C;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache1D;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache1E;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache1F;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache20;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache21;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache22;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache23;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache24;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache25;

	[CompilerGenerated]
	private static Func<SimDebugView, int, Color> _003C_003Ef__mg_0024cache26;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		material = UnityEngine.Object.Instantiate(material);
		diseaseMaterial = UnityEngine.Object.Instantiate(diseaseMaterial);
	}

	protected override void OnSpawn()
	{
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", temperatureThresholds[0].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", temperatureThresholds[1].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", temperatureThresholds[2].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color3", temperatureThresholds[3].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color4", temperatureThresholds[4].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color5", temperatureThresholds[5].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color6", temperatureThresholds[6].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color7", temperatureThresholds[7].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", heatFlowThresholds[0].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", heatFlowThresholds[1].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", heatFlowThresholds[2].color);
		SetMode(global::OverlayModes.None.ID);
	}

	public void OnReset()
	{
		plane = CreatePlane("SimDebugView", base.transform);
		tex = CreateTexture(out texBytes, Grid.WidthInCells, Grid.HeightInCells);
		plane.GetComponent<Renderer>().sharedMaterial = material;
		plane.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
		plane.transform.SetLocalPosition(new Vector3(0f, 0f, -6f));
		SetMode(global::OverlayModes.None.ID);
	}

	public static Texture2D CreateTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		Texture2D texture2D = new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat(TextureFormat.RGBA32), TextureCreationFlags.None);
		texture2D.name = "SimDebugView";
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.filterMode = FilterMode.Point;
		return texture2D;
	}

	public static GameObject CreatePlane(string layer, Transform parent)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "overlayViewDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		gameObject.transform.SetParent(parent);
		gameObject.transform.SetPosition(Vector3.zero);
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		Mesh mesh2 = meshFilter.mesh = new Mesh();
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		float y = 2f * (float)Grid.HeightInCells;
		array = new Vector3[4]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3((float)Grid.WidthInCells, 0f, 0f),
			new Vector3(0f, y, 0f),
			new Vector3(Grid.WidthInMeters, y, 0f)
		};
		array2 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 2f),
			new Vector2(1f, 2f)
		};
		array3 = new int[6]
		{
			0,
			2,
			1,
			1,
			2,
			3
		};
		mesh2.vertices = array;
		mesh2.uv = array2;
		mesh2.triangles = array3;
		Vector2 vector = new Vector2((float)Grid.WidthInCells, y);
		mesh2.bounds = new Bounds(new Vector3(0.5f * vector.x, 0.5f * vector.y, 0f), new Vector3(vector.x, vector.y, 0f));
		return gameObject;
	}

	private void Update()
	{
		if (!((UnityEngine.Object)plane == (UnityEngine.Object)null))
		{
			bool flag = mode != global::OverlayModes.None.ID;
			plane.SetActive(flag);
			SimDebugViewCompositor.Instance.Toggle(mode != global::OverlayModes.None.ID && !GameUtil.IsCapturingTimeLapse());
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds0", new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds1", new Vector4(0.5f, 0.6f, 0.7f, 0.8f));
			float x = 0f;
			if (mode == global::OverlayModes.ThermalConductivity.ID || mode == global::OverlayModes.Temperature.ID)
			{
				x = 1f;
			}
			SimDebugViewCompositor.Instance.material.SetVector("_ThresholdParameters", new Vector4(x, thresholdRange, thresholdOpacity, 0f));
			if (flag)
			{
				UpdateData(tex, texBytes, mode, 192);
			}
		}
	}

	private static void SetDefaultBilinear(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.material;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Bilinear;
	}

	private static void SetDefaultPoint(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.material;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Point;
	}

	private static void SetDisease(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.diseaseMaterial;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Bilinear;
	}

	public void UpdateData(Texture2D texture, byte[] textureBytes, HashedString viewMode, byte alpha)
	{
		if (!dataUpdateFuncs.TryGetValue(viewMode, out Action<SimDebugView, Texture> value))
		{
			value = SetDefaultPoint;
		}
		value(this, texture);
		Grid.GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y);
		selectedPathProber = null;
		KSelectable selected = SelectTool.Instance.selected;
		if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
		{
			selectedPathProber = selected.GetComponent<PathProber>();
		}
		updateSimViewWorkItems.Reset(new UpdateSimViewSharedData(this, texBytes, viewMode, this));
		int num = 16;
		for (int i = min_y; i <= max_y; i += num)
		{
			int y = Math.Min(i + num - 1, max_y);
			updateSimViewWorkItems.Add(new UpdateSimViewWorkItem(min_x, i, max_x, y));
		}
		currentFrame = Time.frameCount;
		selectedCell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		GlobalJobManager.Run(updateSimViewWorkItems);
		texture.LoadRawTextureData(textureBytes);
		texture.Apply();
	}

	public void SetGameGridMode(GameGridMode mode)
	{
		gameGridMode = mode;
	}

	public GameGridMode GetGameGridMode()
	{
		return gameGridMode;
	}

	public void SetMode(HashedString mode)
	{
		this.mode = mode;
		Game.Instance.gameObject.Trigger(1798162660, mode);
	}

	public HashedString GetMode()
	{
		return mode;
	}

	public static Color TemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float value = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num = Mathf.Clamp(value, 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, 1f, 1f);
	}

	public static Color LiquidTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float value = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num = Mathf.Clamp(value, 0.5f, 1f);
		float s = Mathf.Clamp(value, 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, s, 1f);
	}

	public static Color SolidTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float value = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num = Mathf.Clamp(value, 0.5f, 1f);
		float s = 1f;
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, s, 1f);
	}

	public static Color GasTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float value = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num = Mathf.Clamp(value, 0f, 0.5f);
		float s = 1f;
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, s, 1f);
	}

	public Color NormalizedTemperature(float temperature)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < temperatureThresholds.Length; i++)
		{
			if (temperature <= temperatureThresholds[i].value)
			{
				num2 = i;
				break;
			}
			num = i;
			num2 = i;
		}
		float a = 0f;
		if (num != num2)
		{
			a = (temperature - temperatureThresholds[num].value) / (temperatureThresholds[num2].value - temperatureThresholds[num].value);
		}
		a = Mathf.Max(a, 0f);
		a = Mathf.Min(a, 1f);
		return Color.Lerp(temperatureThresholds[num].color, temperatureThresholds[num2].color, a);
	}

	public Color NormalizedHeatFlow(int cell)
	{
		int num = 0;
		int num2 = 0;
		float thermalComfort = GameUtil.GetThermalComfort(cell, -0.0836800039f);
		for (int i = 0; i < heatFlowThresholds.Length; i++)
		{
			if (thermalComfort <= heatFlowThresholds[i].value)
			{
				num2 = i;
				break;
			}
			num = i;
			num2 = i;
		}
		float a = 0f;
		if (num != num2)
		{
			a = (thermalComfort - heatFlowThresholds[num].value) / (heatFlowThresholds[num2].value - heatFlowThresholds[num].value);
		}
		a = Mathf.Max(a, 0f);
		a = Mathf.Min(a, 1f);
		Color result = Color.Lerp(heatFlowThresholds[num].color, heatFlowThresholds[num2].color, a);
		if (Grid.Solid[cell])
		{
			result = Color.black;
		}
		return result;
	}

	private static bool IsInsulated(int cell)
	{
		return (Grid.Element[cell].state & Element.State.TemperatureInsulated) != Element.State.Vacuum;
	}

	private static Color GetDiseaseColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (Grid.DiseaseIdx[cell] != 255)
		{
			Disease disease = Db.Get().Diseases[Grid.DiseaseIdx[cell]];
			result = disease.overlayColour;
			result.a = SimUtil.DiseaseCountToAlpha(Grid.DiseaseCount[cell]);
		}
		else
		{
			result.a = 0f;
		}
		return result;
	}

	private static Color GetHeatFlowColour(SimDebugView instance, int cell)
	{
		return instance.NormalizedHeatFlow(cell);
	}

	private static Color GetBlack(SimDebugView instance, int cell)
	{
		return Color.black;
	}

	public static Color GetLightColour(SimDebugView instance, int cell)
	{
		Color result = new Color(0.8f, 0.7f, 0.3f, Mathf.Clamp(Mathf.Sqrt((float)(Grid.LightIntensity[cell] + LightGridManager.previewLux[cell])) / Mathf.Sqrt(80000f), 0f, 1f));
		if (Grid.LightIntensity[cell] > 71999)
		{
			float num = ((float)Grid.LightIntensity[cell] + (float)LightGridManager.previewLux[cell] - 71999f) / 8001f;
			num /= 10f;
			float r = result.r;
			Vector3 vector = Grid.CellToPos2D(cell);
			float xin = vector.x / 8f;
			Vector3 vector2 = Grid.CellToPos2D(cell);
			result.r = r + Mathf.Min(0.1f, PerlinSimplexNoise.noise(xin, vector2.y / 8f + (float)instance.currentFrame / 32f) * num);
		}
		return result;
	}

	public static Color GetRadiationColour(SimDebugView instance, int cell)
	{
		Color result = new Color(0.2f, 0.9f, 0.3f, Mathf.Clamp(Mathf.Sqrt((float)(Grid.RadiationCount[cell] + RadiationGridManager.previewLux[cell])) / Mathf.Sqrt(80000f), 0f, 1f));
		if (Grid.RadiationCount[cell] > 71999)
		{
			float num = ((float)Grid.RadiationCount[cell] + (float)LightGridManager.previewLux[cell] - 71999f) / 8001f;
			num /= 10f;
			float r = result.r;
			Vector3 vector = Grid.CellToPos2D(cell);
			float xin = vector.x / 8f;
			Vector3 vector2 = Grid.CellToPos2D(cell);
			result.r = r + Mathf.Min(0.1f, PerlinSimplexNoise.noise(xin, vector2.y / 8f + (float)instance.currentFrame / 32f) * num);
		}
		return result;
	}

	public static Color GetRoomsColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (Grid.IsValidCell(instance.selectedCell))
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			if (cavityForCell != null && cavityForCell.room != null)
			{
				Room room = cavityForCell.room;
				result = room.roomType.category.color;
				result.a = 0.45f;
				CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(instance.selectedCell);
				if (cavityForCell2 == cavityForCell)
				{
					result.a += 0.3f;
				}
			}
		}
		return result;
	}

	public static Color GetJoulesColour(SimDebugView instance, int cell)
	{
		float num = Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
		float t = 0.5f * num / (ElementLoader.FindElementByHash(SimHashes.SandStone).specificHeatCapacity * 294f * 1000000f);
		return Color.Lerp(Color.black, Color.red, t);
	}

	public static Color GetNormalizedTemperatureColourMode(SimDebugView instance, int cell)
	{
		switch (Game.Instance.temperatureOverlayMode)
		{
		case Game.TemperatureOverlayModes.AbsoluteTemperature:
			return GetNormalizedTemperatureColour(instance, cell);
		case Game.TemperatureOverlayModes.HeatFlow:
			return GetHeatFlowColour(instance, cell);
		case Game.TemperatureOverlayModes.AdaptiveTemperature:
			return GetNormalizedTemperatureColour(instance, cell);
		case Game.TemperatureOverlayModes.StateChange:
			return GetStateChangeProximityColour(instance, cell);
		default:
			return GetNormalizedTemperatureColour(instance, cell);
		}
	}

	public static Color GetStateChangeProximityColour(SimDebugView instance, int cell)
	{
		float temperature = Grid.Temperature[cell];
		Element element = Grid.Element[cell];
		float lowTemp = element.lowTemp;
		float highTemp = element.highTemp;
		if (!element.IsGas)
		{
			if (!element.IsSolid)
			{
				return TemperatureToColor(temperature, lowTemp, highTemp);
			}
			lowTemp = Mathf.Max(highTemp - 150f, lowTemp);
			return SolidTemperatureToColor(temperature, lowTemp, highTemp);
		}
		highTemp = Mathf.Min(lowTemp + 150f, highTemp);
		return GasTemperatureToColor(temperature, lowTemp, highTemp);
	}

	public static Color GetNormalizedTemperatureColour(SimDebugView instance, int cell)
	{
		float temperature = Grid.Temperature[cell];
		return instance.NormalizedTemperature(temperature);
	}

	private static Color GetGameGridColour(SimDebugView instance, int cell)
	{
		Color result = new Color32(0, 0, 0, byte.MaxValue);
		switch (instance.gameGridMode)
		{
		case GameGridMode.DigAmount:
			if (Grid.Element[cell].IsSolid)
			{
				float num = Grid.Damage[cell] / 255f;
				result = Color.HSVToRGB(1f - num, 1f, 1f);
			}
			break;
		case GameGridMode.GameSolidMap:
			result = ((!Grid.Solid[cell]) ? Color.black : Color.white);
			break;
		case GameGridMode.Lighting:
			result = ((Grid.LightCount[cell] <= 0 && LightGridManager.previewLux[cell] <= 0) ? Color.black : Color.white);
			break;
		case GameGridMode.DupePassable:
			result = ((!Grid.DupePassable[cell]) ? Color.black : Color.white);
			break;
		}
		return result;
	}

	public Color32 GetColourForID(int id)
	{
		return networkColours[id % networkColours.Length];
	}

	private static Color GetThermalConductivityColour(SimDebugView instance, int cell)
	{
		bool flag = IsInsulated(cell);
		Color result = Color.black;
		float num = instance.maxThermalConductivity - instance.minThermalConductivity;
		if (!flag && num != 0f)
		{
			float a = (Grid.Element[cell].thermalConductivity - instance.minThermalConductivity) / num;
			a = Mathf.Max(a, 0f);
			a = Mathf.Min(a, 1f);
			result = new Color(a, a, a);
		}
		return result;
	}

	private static Color GetPressureMapColour(SimDebugView instance, int cell)
	{
		Color32 c = Color.black;
		if (Grid.Pressure[cell] > 0f)
		{
			float value = (Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - instance.minPressureExpected);
			float num = Mathf.Clamp(value, 0f, 1f);
			float num2 = num * 0.9f;
			c = new Color(num2, num2, num2, 1f);
		}
		return c;
	}

	private static Color GetOxygenMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
		{
			if (Grid.Mass[cell] > minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
			{
				float time = Mathf.Clamp((Grid.Mass[cell] - minimumBreathable) / optimallyBreathable, 0f, 1f);
				result = instance.breathableGradient.Evaluate(time);
			}
			else
			{
				result = instance.unbreathableColour;
			}
		}
		return result;
	}

	private static Color GetTileColour(SimDebugView instance, int cell)
	{
		float num = 0.33f;
		Color result = new Color(num, num, num);
		Element element = Grid.Element[cell];
		bool flag = false;
		foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
		{
			if (element.HasTag(tileOverlayFilter))
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return result;
		}
		return element.substance.uiColour;
	}

	private static Color GetTileTypeColour(SimDebugView instance, int cell)
	{
		Element element = Grid.Element[cell];
		return element.substance.uiColour;
	}

	private static Color GetStateMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		switch (Grid.Element[cell].state & Element.State.Solid)
		{
		case Element.State.Solid:
			result = Color.blue;
			break;
		case Element.State.Liquid:
			result = Color.green;
			break;
		case Element.State.Gas:
			result = Color.yellow;
			break;
		}
		return result;
	}

	private static Color GetSolidLiquidMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		switch (Grid.Element[cell].state & Element.State.Solid)
		{
		case Element.State.Solid:
			result = Color.blue;
			break;
		case Element.State.Liquid:
			result = Color.green;
			break;
		}
		return result;
	}

	private static Color GetStateChangeColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		Element element = Grid.Element[cell];
		if (!element.IsVacuum)
		{
			float num = Grid.Temperature[cell];
			float num2 = element.lowTemp * 0.05f;
			float num3 = Mathf.Abs(num - element.lowTemp);
			float a = num3 / num2;
			float num4 = element.highTemp * 0.05f;
			float num5 = Mathf.Abs(num - element.highTemp);
			float b = num5 / num4;
			float t = Mathf.Max(0f, 1f - Mathf.Min(a, b));
			result = Color.Lerp(Color.black, Color.red, t);
		}
		return result;
	}

	private static Color GetDecorColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (!Grid.Solid[cell])
		{
			float decorAtCell = GameUtil.GetDecorAtCell(cell);
			float num = decorAtCell / 100f;
			result = ((!(num > 0f)) ? Color.Lerp(new Color(0.15f, 0f, 0f), new Color(1f, 0f, 0f), Mathf.Abs(num)) : Color.Lerp(new Color(0.15f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Abs(num)));
		}
		return result;
	}

	private static Color GetDangerColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		DangerAmount dangerAmount = DangerAmount.None;
		if (!Grid.Element[cell].IsSolid)
		{
			float num = 0f;
			if (Grid.Temperature[cell] < minMinionTemperature)
			{
				num = Mathf.Abs(Grid.Temperature[cell] - minMinionTemperature);
			}
			if (Grid.Temperature[cell] > maxMinionTemperature)
			{
				num = Mathf.Abs(Grid.Temperature[cell] - maxMinionTemperature);
			}
			if (num > 0f)
			{
				if (num < 10f)
				{
					dangerAmount = DangerAmount.VeryLow;
				}
				else if (num < 30f)
				{
					dangerAmount = DangerAmount.Low;
				}
				else if (num < 100f)
				{
					dangerAmount = DangerAmount.Moderate;
				}
				else if (num < 200f)
				{
					dangerAmount = DangerAmount.High;
				}
				else if (num < 400f)
				{
					dangerAmount = DangerAmount.VeryHigh;
				}
				else if (num > 800f)
				{
					dangerAmount = DangerAmount.Extreme;
				}
			}
		}
		if (dangerAmount < DangerAmount.VeryHigh && (Grid.Element[cell].IsVacuum || (Grid.Element[cell].IsGas && (Grid.Element[cell].id != SimHashes.Oxygen || Grid.Pressure[cell] < minMinionPressure))))
		{
			dangerAmount++;
		}
		if (dangerAmount != 0)
		{
			float num2 = (float)dangerAmount / 6f;
			result = Color.HSVToRGB((80f - num2 * 80f) / 360f, 1f, 1f);
		}
		return result;
	}

	private static Color GetSimCheckErrorMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		Element element = Grid.Element[cell];
		float num = Grid.Mass[cell];
		float num2 = Grid.Temperature[cell];
		if (!float.IsNaN(num) && !float.IsNaN(num2) && !(num > 10000f) && !(num2 > 10000f))
		{
			if (element.IsVacuum)
			{
				result = ((num2 != 0f) ? Color.yellow : ((num == 0f) ? Color.gray : Color.blue));
			}
			else if (num2 < 10f)
			{
				result = Color.red;
			}
			else if (Grid.Mass[cell] < 1f && Grid.Pressure[cell] < 1f)
			{
				result = Color.green;
			}
			else if (num2 > element.highTemp + 3f && element.highTempTransition != null)
			{
				result = Color.magenta;
			}
			else if (num2 < element.lowTemp + 3f && element.lowTempTransition != null)
			{
				result = Color.cyan;
			}
			return result;
		}
		return Color.red;
	}

	private static Color GetFakeFloorColour(SimDebugView instance, int cell)
	{
		return (!Grid.FakeFloor[cell]) ? Color.black : Color.cyan;
	}

	private static Color GetFoundationColour(SimDebugView instance, int cell)
	{
		return (!Grid.Foundation[cell]) ? Color.black : Color.white;
	}

	private static Color GetDupePassableColour(SimDebugView instance, int cell)
	{
		return (!Grid.DupePassable[cell]) ? Color.black : Color.green;
	}

	private static Color GetCritterImpassableColour(SimDebugView instance, int cell)
	{
		return (!Grid.CritterImpassable[cell]) ? Color.black : Color.yellow;
	}

	private static Color GetDupeImpassableColour(SimDebugView instance, int cell)
	{
		return (!Grid.DupeImpassable[cell]) ? Color.black : Color.red;
	}

	private static Color GetMinionOccupiedColour(SimDebugView instance, int cell)
	{
		return (!((UnityEngine.Object)Grid.Objects[cell, 0] != (UnityEngine.Object)null)) ? Color.black : Color.white;
	}

	private static Color GetMinionGroupProberColour(SimDebugView instance, int cell)
	{
		return (!MinionGroupProber.Get().IsReachable(cell)) ? Color.black : Color.white;
	}

	private static Color GetPathProberColour(SimDebugView instance, int cell)
	{
		return (!((UnityEngine.Object)instance.selectedPathProber != (UnityEngine.Object)null) || instance.selectedPathProber.GetCost(cell) == -1) ? Color.black : Color.white;
	}

	private static Color GetReservedColour(SimDebugView instance, int cell)
	{
		return (!Grid.Reserved[cell]) ? Color.black : Color.white;
	}

	private static Color GetAllowPathFindingColour(SimDebugView instance, int cell)
	{
		return (!Grid.AllowPathfinding[cell]) ? Color.black : Color.white;
	}

	private static Color GetMassColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (!IsInsulated(cell))
		{
			float num = Grid.Mass[cell];
			if (num > 0f)
			{
				float num2 = (num - Instance.minMassExpected) / (Instance.maxMassExpected - Instance.minMassExpected);
				result = Color.HSVToRGB(1f - num2, 1f, 1f);
			}
		}
		return result;
	}
}
