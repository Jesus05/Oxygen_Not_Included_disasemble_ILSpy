using System.Collections.Generic;
using UnityEngine;

public class PrioritizableRenderer
{
	private Mesh mesh;

	private int layer;

	private Material material;

	private int prioritizableCount;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private int[] triangles;

	private List<Prioritizable> prioritizables;

	public PrioritizableRenderer()
	{
		layer = LayerMask.NameToLayer("UI");
		Shader shader = Shader.Find("Klei/Prioritizable");
		Texture2D texture = Assets.GetTexture("priority_overlay_atlas");
		material = new Material(shader);
		material.SetTexture(Shader.PropertyToID("_MainTex"), texture);
		prioritizables = new List<Prioritizable>();
		mesh = new Mesh();
		mesh.name = "Prioritizables";
		mesh.MarkDynamic();
	}

	public void Cleanup()
	{
		material = null;
		vertices = null;
		uvs = null;
		prioritizables = null;
		triangles = null;
		Object.DestroyImmediate(mesh);
		mesh = null;
	}

	public void RenderEveryTick()
	{
		using (new KProfiler.Region("PrioritizableRenderer", null))
		{
			if (!((Object)GameScreenManager.Instance == (Object)null) && !((Object)SimDebugView.Instance == (Object)null) && !(SimDebugView.Instance.GetMode() != OverlayModes.Priorities.ID))
			{
				prioritizables.Clear();
				for (int i = 0; i < Components.Prioritizables.Count; i++)
				{
					Prioritizable prioritizable = Components.Prioritizables[i];
					if ((Object)prioritizable != (Object)null && prioritizable.showIcon && prioritizable.IsPrioritizable())
					{
						int cell = Grid.PosToCell(prioritizable);
						if (Grid.IsVisible(cell))
						{
							prioritizables.Add(prioritizable);
						}
					}
				}
				if (prioritizableCount != prioritizables.Count)
				{
					prioritizableCount = prioritizables.Count;
					vertices = new Vector3[4 * prioritizableCount];
					uvs = new Vector2[4 * prioritizableCount];
					triangles = new int[6 * prioritizableCount];
				}
				if (prioritizableCount != 0)
				{
					for (int j = 0; j < prioritizables.Count; j++)
					{
						Prioritizable prioritizable2 = prioritizables[j];
						Vector3 vector = Vector3.zero;
						KAnimControllerBase component = prioritizable2.GetComponent<KAnimControllerBase>();
						vector = ((!((Object)component != (Object)null)) ? prioritizable2.transform.GetPosition() : component.GetWorldPivot());
						vector.x += prioritizable2.iconOffset.x;
						vector.y += prioritizable2.iconOffset.y;
						Vector2 vector2 = new Vector2(0.2f, 0.3f) * prioritizable2.iconScale;
						float z = -5f;
						int num = 4 * j;
						vertices[num] = new Vector3(vector.x - vector2.x, vector.y - vector2.y, z);
						vertices[1 + num] = new Vector3(vector.x - vector2.x, vector.y + vector2.y, z);
						vertices[2 + num] = new Vector3(vector.x + vector2.x, vector.y - vector2.y, z);
						vertices[3 + num] = new Vector3(vector.x + vector2.x, vector.y + vector2.y, z);
						float num2 = 0.111111112f;
						PrioritySetting masterPriority = prioritizable2.GetMasterPriority();
						float num3 = -1f;
						if (masterPriority.priority_class >= PriorityScreen.PriorityClass.high)
						{
							num3 += 9f;
						}
						if (masterPriority.priority_class >= PriorityScreen.PriorityClass.emergency)
						{
							num3 += 9f;
						}
						num3 += (float)masterPriority.priority_value;
						float num4 = num2 * num3;
						float num5 = 0f;
						float num6 = num2;
						float num7 = 1f;
						uvs[num] = new Vector2(num4, num5);
						uvs[1 + num] = new Vector2(num4, num5 + num7);
						uvs[2 + num] = new Vector2(num4 + num6, num5);
						uvs[3 + num] = new Vector2(num4 + num6, num5 + num7);
						int num8 = 6 * j;
						triangles[num8] = num;
						triangles[1 + num8] = num + 1;
						triangles[2 + num8] = num + 2;
						triangles[3 + num8] = num + 2;
						triangles[4 + num8] = num + 1;
						triangles[5 + num8] = num + 3;
					}
					mesh.Clear();
					mesh.vertices = vertices;
					mesh.uv = uvs;
					mesh.SetTriangles(triangles, 0);
					mesh.RecalculateBounds();
					Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, layer, GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 0, null, false, false);
				}
			}
		}
	}
}
