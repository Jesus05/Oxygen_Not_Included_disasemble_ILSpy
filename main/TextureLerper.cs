using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureLerper
{
	private static int offsetCounter;

	public string name;

	private RenderTexture[] BlendTextures = new RenderTexture[2];

	private float BlendDt;

	private float BlendTime;

	private int BlendIdx;

	private Material Material;

	public float Speed = 1f;

	private Mesh mesh;

	private RenderTexture source;

	private RenderTexture dest;

	private GameObject meshGO;

	private GameObject cameraGO;

	private Camera textureCam;

	private float blend;

	public TextureLerper(Texture target_texture, string name, FilterMode filter_mode = FilterMode.Bilinear, TextureFormat texture_format = TextureFormat.ARGB32)
	{
		this.name = name;
		Init(target_texture.width, target_texture.height, name, filter_mode, texture_format);
		Material.SetTexture("_TargetTex", target_texture);
	}

	private void Init(int width, int height, string name, FilterMode filter_mode, TextureFormat texture_format)
	{
		for (int i = 0; i < 2; i++)
		{
			BlendTextures[i] = new RenderTexture(width, height, 0, TextureUtil.GetRenderTextureFormat(texture_format));
			BlendTextures[i].filterMode = filter_mode;
			BlendTextures[i].name = name;
		}
		Material = new Material(Shader.Find("Klei/LerpEffect"));
		Material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
		mesh = new Mesh();
		mesh.name = "LerpEffect";
		mesh.vertices = new Vector3[4]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(1f, 0f, 0f)
		};
		mesh.triangles = new int[6]
		{
			0,
			1,
			2,
			0,
			3,
			1
		};
		mesh.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),
			new Vector2(1f, 0f)
		};
		int layer = LayerMask.NameToLayer("RTT");
		int mask = LayerMask.GetMask("RTT");
		cameraGO = new GameObject();
		cameraGO.name = "TextureLerper_" + name;
		textureCam = cameraGO.AddComponent<Camera>();
		textureCam.transform.SetPosition(new Vector3((float)offsetCounter + 0.5f, 0.5f, 0f));
		textureCam.clearFlags = CameraClearFlags.Nothing;
		textureCam.depth = -100f;
		textureCam.allowHDR = false;
		textureCam.orthographic = true;
		textureCam.orthographicSize = 0.5f;
		textureCam.cullingMask = mask;
		textureCam.targetTexture = dest;
		textureCam.nearClipPlane = -5f;
		textureCam.farClipPlane = 5f;
		textureCam.useOcclusionCulling = false;
		textureCam.aspect = 1f;
		textureCam.rect = new Rect(0f, 0f, 1f, 1f);
		meshGO = new GameObject();
		meshGO.name = "mesh";
		meshGO.transform.parent = cameraGO.transform;
		meshGO.transform.SetLocalPosition(new Vector3(-0.5f, -0.5f, 0f));
		meshGO.isStatic = true;
		MeshRenderer meshRenderer = meshGO.AddComponent<MeshRenderer>();
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		MeshFilter meshFilter = meshGO.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		meshRenderer.sharedMaterial = Material;
		cameraGO.SetLayerRecursively(layer);
		offsetCounter++;
	}

	public void LongUpdate(float dt)
	{
		BlendDt = dt;
		BlendTime = 0f;
	}

	public Texture Update()
	{
		float num = Time.deltaTime * Speed;
		if (Time.deltaTime == 0f)
		{
			num = Time.unscaledDeltaTime * Speed;
		}
		float value = Mathf.Min(num / Mathf.Max(BlendDt - BlendTime, 0f), 1f);
		BlendTime += num;
		source = BlendTextures[BlendIdx];
		BlendIdx = (BlendIdx + 1) % 2;
		dest = BlendTextures[BlendIdx];
		Vector4 visibleCellRange = GetVisibleCellRange();
		visibleCellRange = new Vector4(0f, 0f, (float)Grid.WidthInCells, (float)Grid.HeightInCells);
		Material.SetFloat("_Lerp", value);
		Material.SetTexture("_SourceTex", source);
		Material.SetVector("_MeshParams", visibleCellRange);
		textureCam.targetTexture = dest;
		return dest;
	}

	private Vector4 GetVisibleCellRange()
	{
		Camera main = Camera.main;
		float cellSizeInMeters = Grid.CellSizeInMeters;
		Ray ray = main.ViewportPointToRay(Vector3.zero);
		Vector3 origin = ray.origin;
		float z = origin.z;
		Vector3 direction = ray.direction;
		float distance = Mathf.Abs(z / direction.z);
		Vector3 point = ray.GetPoint(distance);
		int cell = Grid.PosToCell(point);
		float num = 0f - Grid.HalfCellSizeInMeters;
		point = Grid.CellToPos(cell, num, num, num);
		int num2 = Math.Max(0, (int)(point.x / cellSizeInMeters));
		int num3 = Math.Max(0, (int)(point.y / cellSizeInMeters));
		ray = main.ViewportPointToRay(Vector3.one);
		Vector3 origin2 = ray.origin;
		float z2 = origin2.z;
		Vector3 direction2 = ray.direction;
		distance = Mathf.Abs(z2 / direction2.z);
		point = ray.GetPoint(distance);
		int a = Mathf.CeilToInt(point.x / cellSizeInMeters);
		int a2 = Mathf.CeilToInt(point.y / cellSizeInMeters);
		a = Mathf.Min(a, Grid.WidthInCells - 1);
		a2 = Mathf.Min(a2, Grid.HeightInCells - 1);
		return new Vector4((float)num2, (float)num3, (float)a, (float)a2);
	}
}
