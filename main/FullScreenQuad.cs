using UnityEngine;

public class FullScreenQuad
{
	private Mesh Mesh;

	private Camera Camera;

	private Material Material;

	private int Layer;

	public FullScreenQuad(string name, Camera camera, bool invert = false)
	{
		Camera = camera;
		Layer = LayerMask.NameToLayer("ForceDraw");
		Mesh = new Mesh();
		Mesh.name = name;
		Mesh.vertices = new Vector3[4]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, -1f, 0f),
			new Vector3(1f, 1f, 0f)
		};
		float y = 1f;
		float y2 = 0f;
		if (invert)
		{
			y = 0f;
			y2 = 1f;
		}
		Mesh.uv = new Vector2[4]
		{
			new Vector2(0f, y2),
			new Vector2(0f, y),
			new Vector2(1f, y2),
			new Vector2(1f, y)
		};
		Mesh.triangles = new int[6]
		{
			0,
			1,
			2,
			2,
			1,
			3
		};
		Mesh.bounds = new Bounds(Vector3.zero, new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f));
		Material = new Material(Shader.Find("Klei/PostFX/FullScreen"));
		Camera.cullingMask |= LayerMask.GetMask("ForceDraw");
	}

	public void Draw(Texture texture)
	{
		Material.mainTexture = texture;
		Graphics.DrawMesh(Mesh, Vector3.zero, Quaternion.identity, Material, Layer, Camera, 0, null, false, false);
	}
}
