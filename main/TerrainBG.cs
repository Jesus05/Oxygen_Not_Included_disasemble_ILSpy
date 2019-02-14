using UnityEngine;

public class TerrainBG : KMonoBehaviour
{
	public Material starsMaterial;

	public Material backgroundMaterial;

	public Material gasMaterial;

	public bool doDraw = true;

	[SerializeField]
	private Texture3D noiseVolume;

	private Mesh starsPlane;

	private Mesh worldPlane;

	private Mesh gasPlane;

	private int layer;

	private MaterialPropertyBlock[] propertyBlocks;

	protected override void OnSpawn()
	{
		layer = LayerMask.NameToLayer("Default");
		noiseVolume = CreateTexture3D(32);
		starsPlane = CreateStarsPlane("StarsPlane");
		worldPlane = CreateWorldPlane("WorldPlane");
		gasPlane = CreateGasPlane("GasPlane");
		propertyBlocks = new MaterialPropertyBlock[Lighting.Instance.Settings.BackgroundLayers];
		for (int i = 0; i < propertyBlocks.Length; i++)
		{
			propertyBlocks[i] = new MaterialPropertyBlock();
		}
	}

	private Texture3D CreateTexture3D(int size)
	{
		Color32[] array = new Color32[size * size * size];
		Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.RGBA32, true);
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				for (int k = 0; k < size; k++)
				{
					Color32 color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255));
					array[i + j * size + k * size * size] = color;
				}
			}
		}
		texture3D.SetPixels32(array);
		texture3D.Apply();
		return texture3D;
	}

	public Mesh CreateGasPlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		array = new Vector3[4]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3((float)Grid.WidthInCells, 0f, 0f),
			new Vector3(0f, Grid.HeightInMeters, 0f),
			new Vector3(Grid.WidthInMeters, Grid.HeightInMeters, 0f)
		};
		array2 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
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
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.bounds = new Bounds(new Vector3((float)Grid.WidthInCells * 0.5f, (float)Grid.HeightInCells * 0.5f, 0f), new Vector3((float)Grid.WidthInCells, (float)Grid.HeightInCells, 0f));
		return mesh;
	}

	public Mesh CreateWorldPlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		array = new Vector3[4]
		{
			new Vector3((float)(-Grid.WidthInCells), (float)(-Grid.HeightInCells), 0f),
			new Vector3((float)Grid.WidthInCells * 2f, (float)(-Grid.HeightInCells), 0f),
			new Vector3((float)(-Grid.WidthInCells), Grid.HeightInMeters * 2f, 0f),
			new Vector3(Grid.WidthInMeters * 2f, Grid.HeightInMeters * 2f, 0f)
		};
		array2 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
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
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.bounds = new Bounds(new Vector3((float)Grid.WidthInCells * 0.5f, (float)Grid.HeightInCells * 0.5f, 0f), new Vector3((float)Grid.WidthInCells, (float)Grid.HeightInCells, 0f));
		return mesh;
	}

	public Mesh CreateStarsPlane(string name)
	{
		Mesh mesh = new Mesh();
		mesh.name = name;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		array = new Vector3[4]
		{
			new Vector3((float)(-Grid.WidthInCells), (float)(-Grid.HeightInCells), 0f),
			new Vector3((float)Grid.WidthInCells * 2f, (float)(-Grid.HeightInCells), 0f),
			new Vector3((float)(-Grid.WidthInCells), Grid.HeightInMeters * 2f, 0f),
			new Vector3(Grid.WidthInMeters * 2f, Grid.HeightInMeters * 2f, 0f)
		};
		array2 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
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
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		Vector2 vector = new Vector2((float)Grid.WidthInCells, 2f * (float)Grid.HeightInCells);
		mesh.bounds = new Bounds(new Vector3(0.5f * vector.x, 0.5f * vector.y, 0f), new Vector3(vector.x, vector.y, 0f));
		return mesh;
	}

	private void LateUpdate()
	{
		if (doDraw)
		{
			starsMaterial.renderQueue = RenderQueues.Stars;
			starsMaterial.SetTexture("_NoiseVolume", noiseVolume);
			Graphics.DrawMesh(position: new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Background) + 1f), mesh: starsPlane, rotation: Quaternion.identity, material: starsMaterial, layer: layer);
			backgroundMaterial.renderQueue = RenderQueues.Backwall;
			for (int i = 0; i < Lighting.Instance.Settings.BackgroundLayers; i++)
			{
				if (i >= Lighting.Instance.Settings.BackgroundLayers - 1)
				{
					float t = (float)i / (float)(Lighting.Instance.Settings.BackgroundLayers - 1);
					float x = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundDarkening, t);
					float z = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundUVScale, t);
					float w = 1f;
					if (i == Lighting.Instance.Settings.BackgroundLayers - 1)
					{
						w = 0f;
					}
					MaterialPropertyBlock materialPropertyBlock = propertyBlocks[i];
					materialPropertyBlock.SetVector("_BackWallParameters", new Vector4(x, Lighting.Instance.Settings.BackgroundClip, z, w));
					Graphics.DrawMesh(position: new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Background)), mesh: worldPlane, rotation: Quaternion.identity, material: backgroundMaterial, layer: layer, camera: null, submeshIndex: 0, properties: materialPropertyBlock);
				}
			}
			gasMaterial.renderQueue = RenderQueues.Gas;
			Graphics.DrawMesh(position: new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Backwall) - 1f), mesh: gasPlane, rotation: Quaternion.identity, material: gasMaterial, layer: layer);
		}
	}
}
