using Klei;
using Klei.AI;
using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class SimDebugView : KMonoBehaviour
{
	public enum GameGridMode
	{
		GameSolidMap,
		Lighting,
		RoomMap,
		Style,
		PlantDensity,
		DigAmount,
		ForceField
	}

	[Serializable]
	public struct ColorThreshold
	{
		public Color color;

		public float value;
	}

	private struct UpdateSimViewSharedData
	{
		public SimViewMode simViewMode;

		public GameGridMode gameGridMode;

		public SimDebugView simDebugView;

		public PathProber selectedPathProber;

		public byte[] textureBytes;

		public UpdateSimViewSharedData(byte[] texture_bytes, SimViewMode sim_view_mode, GameGridMode game_grid_mode, PathProber selected_path_prober, SimDebugView sim_debug_view)
		{
			textureBytes = texture_bytes;
			simViewMode = sim_view_mode;
			gameGridMode = game_grid_mode;
			simDebugView = sim_debug_view;
			selectedPathProber = selected_path_prober;
		}
	}

	private struct UpdateSimViewWorkItem : IWorkItem<UpdateSimViewSharedData>
	{
		private int x0;

		private int y0;

		private int x1;

		private int y1;

		public UpdateSimViewWorkItem(int x0, int y0, int x1, int y1)
		{
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;
		}

		public void Run(UpdateSimViewSharedData shared_data)
		{
			for (int i = y0; i <= y1; i++)
			{
				for (int j = x0; j <= x1; j++)
				{
					Color black = Color.black;
					int num = Grid.XYToCell(j, i);
					if (Grid.IsValidCell(num))
					{
						black = shared_data.simDebugView.GetColor(num, shared_data.simViewMode, shared_data.gameGridMode, shared_data.selectedPathProber);
						int num2 = num * 4;
						shared_data.textureBytes[num2] = (byte)(Mathf.Min(black.r, 1f) * 255f);
						shared_data.textureBytes[num2 + 1] = (byte)(Mathf.Min(black.g, 1f) * 255f);
						shared_data.textureBytes[num2 + 2] = (byte)(Mathf.Min(black.b, 1f) * 255f);
						shared_data.textureBytes[num2 + 3] = (byte)(Mathf.Min(black.a, 1f) * 255f);
					}
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

	public bool hideFOW;

	public const int colourSize = 4;

	private byte[] texBytes;

	private int currentFrame;

	[SerializeField]
	private Texture2D tex;

	[SerializeField]
	private GameObject plane;

	private SimViewMode mode = SimViewMode.PowerMap;

	private GameGridMode gameGridMode = GameGridMode.DigAmount;

	public float minTempExpected = 173.15f;

	public float maxTempExpected = 423.15f;

	public float minMassExpected = 1.0001f;

	public float maxMassExpected = 10000f;

	public float minPressureExpected = 1.300003f;

	public float maxPressureExpected = 201.3f;

	public float minThermalConductivity;

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

	private const float lum = 1f;

	private static float minMinionTemperature = 260f;

	private static float maxMinionTemperature = 310f;

	private static float minMinionPressure = 80f;

	public Color[] dbColours = new Color[13]
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
		SetMode(SimViewMode.None);
	}

	public void OnReset()
	{
		plane = CreatePlane("SimDebugView", base.transform);
		tex = CreateTexture(out texBytes, Grid.WidthInCells, Grid.HeightInCells);
		plane.GetComponent<Renderer>().sharedMaterial = material;
		plane.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
		plane.transform.SetLocalPosition(new Vector3(0f, 0f, -6f));
		SetMode(SimViewMode.None);
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
			bool flag = mode != SimViewMode.None;
			plane.SetActive(flag);
			SimDebugViewCompositor.Instance.Toggle(mode != SimViewMode.None);
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds0", new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds1", new Vector4(0.5f, 0.6f, 0.7f, 0.8f));
			float x = 0f;
			if (mode == SimViewMode.ThermalConductivity || mode == SimViewMode.TemperatureMap)
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

	public void UpdateData(Texture2D texture, byte[] textureBytes, SimViewMode viewMode, byte alpha)
	{
		switch (viewMode)
		{
		case SimViewMode.HeatFlow:
		case SimViewMode.TemperatureMap:
		case SimViewMode.Decor:
		case SimViewMode.OxygenMap:
			plane.GetComponent<Renderer>().sharedMaterial = material;
			plane.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
			texture.filterMode = FilterMode.Bilinear;
			break;
		case SimViewMode.Disease:
			plane.GetComponent<Renderer>().sharedMaterial = diseaseMaterial;
			plane.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
			texture.filterMode = FilterMode.Bilinear;
			break;
		default:
			plane.GetComponent<Renderer>().sharedMaterial = material;
			plane.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
			texture.filterMode = FilterMode.Point;
			break;
		}
		Grid.GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y);
		PathProber selected_path_prober = null;
		KSelectable selected = SelectTool.Instance.selected;
		if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
		{
			selected_path_prober = selected.GetComponent<PathProber>();
		}
		updateSimViewWorkItems.Reset(new UpdateSimViewSharedData(texBytes, viewMode, gameGridMode, selected_path_prober, this));
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

	public void SetMode(SimViewMode mode)
	{
		this.mode = mode;
		Game.Instance.gameObject.Trigger(1798162660, mode);
	}

	public SimViewMode GetMode()
	{
		return mode;
	}

	public static Color TemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float value = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num = Mathf.Clamp(value, 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, 1f, 1f);
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

	public Color GetColor(int cell, SimViewMode viewMode, GameGridMode ggMode, PathProber path_prober)
	{
		Color result = Color.black;
		bool flag = (Grid.Element[cell].state & Element.State.TemperatureInsulated) != Element.State.Vacuum;
		switch (viewMode)
		{
		case SimViewMode.ThermalConductivity:
			result = GetThermalConductivityColour(flag, cell);
			break;
		case SimViewMode.TemperatureMap:
			result = NormalizedTemperature(Grid.Temperature[cell]);
			break;
		case SimViewMode.Disease:
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
			break;
		case SimViewMode.HeatFlow:
			result = NormalizedHeatFlow(cell);
			break;
		case SimViewMode.TemperatureMapOld:
			if (!flag)
			{
				result = TemperatureToColor(Grid.Temperature[cell], minTempExpected, maxTempExpected);
			}
			break;
		case SimViewMode.DangerMap:
			result = GetDangerMap(cell);
			break;
		case SimViewMode.PressureMap:
			result = GetPressureMapColour(cell);
			break;
		case SimViewMode.OxygenMap:
			result = GetOxygenMapColour(cell);
			break;
		case SimViewMode.TileType:
			result = GetTileTypeColour(cell);
			break;
		case SimViewMode.StateMap:
			result = GetStateMapColour(cell);
			break;
		case SimViewMode.SolidLiquidMap:
			result = GetSolidLiquidMapColour(cell);
			break;
		case SimViewMode.MassMap:
			if (!flag)
			{
				float num2 = Grid.Mass[cell];
				if (num2 > 0f)
				{
					float num3 = (num2 - Instance.minMassExpected) / (Instance.maxMassExpected - Instance.minMassExpected);
					result = Color.HSVToRGB(1f - num3, 1f, 1f);
				}
			}
			break;
		case SimViewMode.GameGrid:
			result = GetGameGridColour(cell, ggMode);
			break;
		case SimViewMode.StateChange:
			result = GetStateChangeColour(cell);
			break;
		case SimViewMode.SuitRequiredMap:
			result = Color.black;
			break;
		case SimViewMode.MinionOccupied:
			if ((UnityEngine.Object)Grid.Objects[cell, 0] != (UnityEngine.Object)null)
			{
				result = Color.white;
			}
			break;
		case SimViewMode.NoisePollution:
			result = GetNoisePollutionColour(cell);
			break;
		case SimViewMode.Decor:
			result = GetDecorColour(cell);
			break;
		case SimViewMode.Reachability:
			result = Color.black;
			break;
		case SimViewMode.Light:
			result = new Color(0.8f, 0.7f, 0.3f, Mathf.Clamp(Mathf.Sqrt((float)(Grid.LightIntensity[cell] + LightGridManager.previewLux[cell])) / Mathf.Sqrt(80000f), 0f, 1f));
			if (Grid.LightIntensity[cell] > 71999)
			{
				float num4 = ((float)Grid.LightIntensity[cell] + (float)LightGridManager.previewLux[cell] - 71999f) / 8001f;
				num4 /= 10f;
				float r = result.r;
				Vector3 vector = Grid.CellToPos2D(cell);
				float xin = vector.x / 8f;
				Vector3 vector2 = Grid.CellToPos2D(cell);
				result.r = r + Mathf.Min(0.1f, PerlinSimplexNoise.noise(xin, vector2.y / 8f + (float)currentFrame / 32f) * num4);
			}
			break;
		case SimViewMode.SimCheckErrorMap:
			result = GetSimCheckErrorMapColour(cell);
			break;
		case SimViewMode.Forcefield:
			if (Grid.ForceField[cell])
			{
				result = Color.white;
			}
			break;
		case SimViewMode.MinionGroupProber:
			result = ((!MinionGroupProber.Get().IsReachable(cell, currentFrame)) ? Color.black : Color.white);
			break;
		case SimViewMode.PathProber:
			if ((UnityEngine.Object)path_prober != (UnityEngine.Object)null && (UnityEngine.Object)path_prober != (UnityEngine.Object)null)
			{
				int cost = path_prober.GetCost(cell);
				result = ((cost == -1) ? Color.black : Color.white);
			}
			break;
		case SimViewMode.Reserved:
			result = ((!Grid.Reserved[cell]) ? Color.black : Color.white);
			break;
		case SimViewMode.AllowPathfinding:
			result = ((!Grid.AllowPathfinding[cell]) ? Color.black : Color.white);
			break;
		case SimViewMode.Rooms:
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			if (cavityForCell != null && cavityForCell.room != null)
			{
				Room room = cavityForCell.room;
				result = room.roomType.category.color;
				result.a = 0.45f;
				if (Grid.IsValidCell(selectedCell))
				{
					CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(selectedCell);
					if (cavityForCell2 == cavityForCell)
					{
						result.a += 0.3f;
					}
				}
			}
			else
			{
				result = Color.black;
			}
			break;
		}
		case SimViewMode.Priorities:
			result = Color.black;
			break;
		case SimViewMode.Crop:
		case SimViewMode.HarvestWhenReady:
			result = Color.black;
			break;
		case SimViewMode.Joules:
		{
			float num = Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
			float t = 0.5f * num / (ElementLoader.FindElementByHash(SimHashes.SandStone).specificHeatCapacity * 294f * 1000000f);
			result = Color.Lerp(Color.black, Color.red, t);
			break;
		}
		}
		return result;
	}

	private Color GetGameGridColour(int cell, GameGridMode mode)
	{
		Color result = new Color32(0, 0, 0, byte.MaxValue);
		switch (mode)
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
		case GameGridMode.ForceField:
			result = ((!Grid.ForceField[cell]) ? Color.black : Color.white);
			break;
		}
		return result;
	}

	public Color32 GetColourForID(int id)
	{
		return networkColours[id % networkColours.Length];
	}

	private Color GetThermalConductivityColour(bool insulated, int cell)
	{
		Color result = Color.black;
		float num = maxThermalConductivity - minThermalConductivity;
		if (!insulated && num != 0f)
		{
			float a = (Grid.Element[cell].thermalConductivity - minThermalConductivity) / num;
			a = Mathf.Max(a, 0f);
			a = Mathf.Min(a, 1f);
			result = new Color(a, a, a);
		}
		return result;
	}

	private Color GetPressureMapColour(int cell)
	{
		Color result = Color.black;
		if (Grid.Pressure[cell] > 0f)
		{
			float value = (Grid.Pressure[cell] - minPressureExpected) / (maxPressureExpected - minPressureExpected);
			float num = Mathf.Clamp(value, 0f, 1f);
			float num2 = num * 0.9f;
			result = new Color(num2, num2, num2, 1f);
		}
		return result;
	}

	private Color GetOxygenMapColour(int cell)
	{
		Color result = Color.black;
		if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
		{
			if (Grid.Mass[cell] > minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
			{
				float time = Mathf.Clamp((Grid.Mass[cell] - minimumBreathable) / optimallyBreathable, 0f, 1f);
				result = breathableGradient.Evaluate(time);
			}
			else
			{
				result = unbreathableColour;
			}
		}
		return result;
	}

	private Color GetTileTypeColour(int cell)
	{
		Element element = Grid.Element[cell];
		return element.substance.debugColour;
	}

	private Color GetStateMapColour(int cell)
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

	private Color GetSolidLiquidMapColour(int cell)
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

	private Color GetStateChangeColour(int cell)
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

	private Color GetNoisePollutionColour(int cell)
	{
		Color result = Color.black;
		if (NoisePolluter.IsNoiseableCell(cell))
		{
			float num = Grid.Loudness[cell];
			if (num == 0f)
			{
				result = dbColours[0];
			}
			else
			{
				float num2 = AudioEventManager.LoudnessToDB(num);
				result = ((num2 <= 36f) ? Color.Lerp(dbColours[0], dbColours[1], Mathf.Abs(num2 / 36f)) : ((num2 >= 36f && num2 < 45f) ? Color.Lerp(dbColours[2], dbColours[3], Mathf.Abs(num2 / 45f)) : ((num2 >= 45f && num2 < 60f) ? Color.Lerp(dbColours[4], dbColours[5], Mathf.Abs((num2 - 45f) / 15f)) : ((num2 >= 60f && num2 < 80f) ? Color.Lerp(dbColours[6], dbColours[7], Mathf.Abs((num2 - 60f) / 20f)) : ((!(num2 >= 80f) || !(num2 < 106f)) ? Color.Lerp(dbColours[10], dbColours[11], Mathf.Abs((num2 - 106f) / 19f)) : Color.Lerp(dbColours[8], dbColours[9], Mathf.Abs((num2 - 80f) / 26f)))))));
			}
		}
		return result;
	}

	public Color GetNoisePollutionCategoryColourFromDecibels(float db)
	{
		Color result = Color.white;
		result = ((db < 36f) ? Color.Lerp(dbColours[0], dbColours[1], 0.8f) : ((db >= 36f && db < 45f) ? Color.Lerp(dbColours[2], dbColours[3], 0.5f) : ((db >= 45f && db < 60f) ? Color.Lerp(dbColours[4], dbColours[5], 0.5f) : ((db >= 60f && db < 80f) ? Color.Lerp(dbColours[6], dbColours[7], 0.5f) : ((!(db >= 80f) || !(db < 106f)) ? Color.Lerp(dbColours[10], dbColours[11], 0.5f) : Color.Lerp(dbColours[8], dbColours[9], 0.5f))))));
		result.a = 1f;
		return result;
	}

	public Color GetNoisePollutionHoverColourFromDecibels(float db)
	{
		Color result = Color.white;
		result = ((db <= 36f) ? Color.Lerp(dbColours[0], dbColours[1], 0.3f) : ((db >= 36f && db < 45f) ? Color.Lerp(dbColours[2], dbColours[3], 0.5f) : ((db >= 45f && db < 60f) ? Color.Lerp(dbColours[4], dbColours[5], 0.5f) : ((db >= 60f && db < 80f) ? Color.Lerp(dbColours[6], dbColours[7], 0.5f) : ((!(db >= 80f) || !(db < 106f)) ? Color.Lerp(dbColours[10], dbColours[11], 0.5f) : Color.Lerp(dbColours[8], dbColours[9], 0.5f))))));
		result.a = 1f;
		return result;
	}

	private Color GetDecorColour(int cell)
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

	private Color GetDangerMap(int cell)
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

	private Color GetSimCheckErrorMapColour(int cell)
	{
		Color result = Color.black;
		Element element = Grid.Element[cell];
		float num = Grid.Mass[cell];
		float num2 = Grid.Temperature[cell];
		if (float.IsNaN(num) || float.IsNaN(num2) || num > 10000f || num2 > 10000f)
		{
			return Color.red;
		}
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
}
