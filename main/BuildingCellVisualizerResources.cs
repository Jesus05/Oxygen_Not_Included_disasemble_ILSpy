using System;
using UnityEngine;

public class BuildingCellVisualizerResources : ScriptableObject
{
	[Serializable]
	public struct ConnectedDisconnectedColours
	{
		public Color32 connected;

		public Color32 disconnected;
	}

	[Serializable]
	public struct IOColours
	{
		public ConnectedDisconnectedColours input;

		public ConnectedDisconnectedColours output;
	}

	[Header("Electricity")]
	public Color electricityInputColor;

	public Color electricityOutputColor;

	public Sprite electricityInputIcon;

	public Sprite electricityOutputIcon;

	public Sprite electricityConnectedIcon;

	public Sprite electricityBridgeIcon;

	public Sprite electricityBridgeConnectedIcon;

	public Sprite electricityArrowIcon;

	public Sprite switchIcon;

	public Color32 switchColor;

	public Color32 switchOffColor = Color.red;

	[Header("Gas")]
	public Sprite gasInputIcon;

	public Sprite gasOutputIcon;

	public IOColours gasIOColours;

	[Header("Liquid")]
	public Sprite liquidInputIcon;

	public Sprite liquidOutputIcon;

	public IOColours liquidIOColours;

	private static BuildingCellVisualizerResources _Instance;

	public Material backgroundMaterial
	{
		get;
		set;
	}

	public Material iconBackgroundMaterial
	{
		get;
		set;
	}

	public Material powerInputMaterial
	{
		get;
		set;
	}

	public Material powerOutputMaterial
	{
		get;
		set;
	}

	public Material liquidInputMaterial
	{
		get;
		set;
	}

	public Material liquidOutputMaterial
	{
		get;
		set;
	}

	public Material gasInputMaterial
	{
		get;
		set;
	}

	public Material gasOutputMaterial
	{
		get;
		set;
	}

	public Mesh backgroundMesh
	{
		get;
		set;
	}

	public Mesh iconMesh
	{
		get;
		set;
	}

	public int backgroundLayer
	{
		get;
		set;
	}

	public int iconLayer
	{
		get;
		set;
	}

	public static void DestroyInstance()
	{
		_Instance = null;
	}

	public static BuildingCellVisualizerResources Instance()
	{
		if ((UnityEngine.Object)_Instance == (UnityEngine.Object)null)
		{
			_Instance = Resources.Load<BuildingCellVisualizerResources>("BuildingCellVisualizerResources");
			_Instance.Initialize();
		}
		return _Instance;
	}

	private void Initialize()
	{
		Shader shader = Shader.Find("Klei/BuildingCell");
		backgroundMaterial = new Material(shader);
		backgroundMaterial.mainTexture = GlobalResources.Instance().WhiteTexture;
		iconBackgroundMaterial = new Material(shader);
		iconBackgroundMaterial.mainTexture = GlobalResources.Instance().WhiteTexture;
		powerInputMaterial = new Material(shader);
		powerOutputMaterial = new Material(shader);
		liquidInputMaterial = new Material(shader);
		liquidOutputMaterial = new Material(shader);
		gasInputMaterial = new Material(shader);
		gasOutputMaterial = new Material(shader);
		backgroundMesh = CreateMesh("BuildingCellVisualizer", Vector2.zero, 0.5f);
		float num = 0.5f;
		iconMesh = CreateMesh("BuildingCellVisualizerIcon", Vector2.zero, num * 0.5f);
		backgroundLayer = LayerMask.NameToLayer("Default");
		iconLayer = LayerMask.NameToLayer("Place");
	}

	private Mesh CreateMesh(string name, Vector2 base_offset, float half_size)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		mesh.vertices = new Vector3[4]
		{
			new Vector3(0f - half_size + base_offset.x, 0f - half_size + base_offset.y, 0f),
			new Vector3(half_size + base_offset.x, 0f - half_size + base_offset.y, 0f),
			new Vector3(0f - half_size + base_offset.x, half_size + base_offset.y, 0f),
			new Vector3(half_size + base_offset.x, half_size + base_offset.y, 0f)
		};
		mesh.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		mesh.triangles = new int[6]
		{
			0,
			1,
			2,
			2,
			1,
			3
		};
		mesh.RecalculateBounds();
		return mesh;
	}
}
