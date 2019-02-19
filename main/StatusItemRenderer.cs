using System.Collections.Generic;
using UnityEngine;

public class StatusItemRenderer
{
	public struct Entry
	{
		private struct MeshBuilder
		{
			private Vector3[] vertices;

			private Vector2[] uvs;

			private Vector2[] uv2s;

			private int[] triangles;

			private Color32[] colors;

			private int quadIdx;

			private Material material;

			private static int[] textureIds = new int[11]
			{
				Shader.PropertyToID("_Tex0"),
				Shader.PropertyToID("_Tex1"),
				Shader.PropertyToID("_Tex2"),
				Shader.PropertyToID("_Tex3"),
				Shader.PropertyToID("_Tex4"),
				Shader.PropertyToID("_Tex5"),
				Shader.PropertyToID("_Tex6"),
				Shader.PropertyToID("_Tex7"),
				Shader.PropertyToID("_Tex8"),
				Shader.PropertyToID("_Tex9"),
				Shader.PropertyToID("_Tex10")
			};

			public MeshBuilder(int quad_count, Material material)
			{
				vertices = new Vector3[4 * quad_count];
				uvs = new Vector2[4 * quad_count];
				uv2s = new Vector2[4 * quad_count];
				colors = new Color32[4 * quad_count];
				triangles = new int[6 * quad_count];
				this.material = material;
				quadIdx = 0;
			}

			public void AddQuad(Vector2 center, Vector2 half_size, float z, Sprite sprite, Color color)
			{
				if (quadIdx != textureIds.Length)
				{
					Rect rect = sprite.rect;
					Rect textureRect = sprite.textureRect;
					float num = textureRect.width / rect.width;
					float num2 = textureRect.height / rect.height;
					int num3 = 4 * quadIdx;
					vertices[num3] = new Vector3((center.x - half_size.x) * num, (center.y - half_size.y) * num2, z);
					vertices[1 + num3] = new Vector3((center.x - half_size.x) * num, (center.y + half_size.y) * num2, z);
					vertices[2 + num3] = new Vector3((center.x + half_size.x) * num, (center.y - half_size.y) * num2, z);
					vertices[3 + num3] = new Vector3((center.x + half_size.x) * num, (center.y + half_size.y) * num2, z);
					float num4 = textureRect.x / (float)sprite.texture.width;
					float num5 = textureRect.y / (float)sprite.texture.height;
					float num6 = textureRect.width / (float)sprite.texture.width;
					float num7 = textureRect.height / (float)sprite.texture.height;
					uvs[num3] = new Vector2(num4, num5);
					uvs[1 + num3] = new Vector2(num4, num5 + num7);
					uvs[2 + num3] = new Vector2(num4 + num6, num5);
					uvs[3 + num3] = new Vector2(num4 + num6, num5 + num7);
					colors[num3] = color;
					colors[1 + num3] = color;
					colors[2 + num3] = color;
					colors[3 + num3] = color;
					float x = (float)quadIdx + 0.5f;
					uv2s[num3] = new Vector2(x, 0f);
					uv2s[1 + num3] = new Vector2(x, 0f);
					uv2s[2 + num3] = new Vector2(x, 0f);
					uv2s[3 + num3] = new Vector2(x, 0f);
					int num8 = 6 * quadIdx;
					triangles[num8] = num3;
					triangles[1 + num8] = num3 + 1;
					triangles[2 + num8] = num3 + 2;
					triangles[3 + num8] = num3 + 2;
					triangles[4 + num8] = num3 + 1;
					triangles[5 + num8] = num3 + 3;
					material.SetTexture(textureIds[quadIdx], sprite.texture);
					quadIdx++;
				}
			}

			public void End(Mesh mesh)
			{
				mesh.Clear();
				mesh.vertices = vertices;
				mesh.uv = uvs;
				mesh.uv2 = uv2s;
				mesh.colors32 = colors;
				mesh.SetTriangles(triangles, 0);
				mesh.RecalculateBounds();
			}
		}

		public int handle;

		public Transform transform;

		public List<StatusItem> statusItems;

		public Mesh mesh;

		public bool dirty;

		public int layer;

		public Material material;

		public Vector3 offset;

		public bool hasVisibleStatusItems;

		public bool isBuilding;

		public void Init(Shader shader)
		{
			statusItems = new List<StatusItem>();
			mesh = new Mesh();
			mesh.name = "StatusItemRenderer";
			dirty = true;
			material = new Material(shader);
		}

		public void Render(StatusItemRenderer renderer, Vector3 camera_bl, Vector3 camera_tr, HashedString overlay)
		{
			if (!DebugHandler.HideUI)
			{
				Vector3 vector = Vector3.zero;
				if ((Object)transform != (Object)null)
				{
					vector = transform.GetPosition();
					if (isBuilding)
					{
						Building component = transform.GetComponent<Building>();
						if ((Object)component != (Object)null)
						{
							vector.x += (float)((component.Def.WidthInCells - 1) % 2) / 2f;
						}
					}
					if (!(vector.x < camera_bl.x) && !(vector.x > camera_tr.x) && !(vector.y < camera_bl.y) && !(vector.y > camera_tr.y))
					{
						int cell = Grid.PosToCell(vector);
						if (!Grid.IsValidCell(cell) || Grid.IsVisible(cell))
						{
							KSelectable component2 = transform.GetComponent<KSelectable>();
							if (component2.IsSelectable)
							{
								renderer.visibleEntries.Add(this);
								if (dirty)
								{
									int num = 0;
									foreach (StatusItem statusItem2 in statusItems)
									{
										if (statusItem2.UseConditionalCallback(overlay, transform) || !(overlay != OverlayModes.None.ID) || !(statusItem2.render_overlay != overlay))
										{
											num++;
										}
									}
									hasVisibleStatusItems = (num != 0);
									MeshBuilder meshBuilder = new MeshBuilder(num + 6, material);
									float num2 = 0.25f;
									float z = -5f;
									Vector2 b = new Vector2(0.05f, -0.05f);
									float num3 = 0.02f;
									Color32 c = new Color32(0, 0, 0, byte.MaxValue);
									Color32 c2 = new Color32(0, 0, 0, 75);
									Color32 c3 = renderer.backgroundColor;
									if (renderer.selectedHandle == handle || renderer.highlightHandle == handle)
									{
										c3 = renderer.selectedColor;
									}
									meshBuilder.AddQuad(new Vector2(0f, 0.29f) + b, new Vector2(0.05f, 0.05f), z, renderer.arrowSprite, c2);
									meshBuilder.AddQuad(new Vector2(0f, 0f) + b, new Vector2(num2 * (float)num, num2), z, renderer.backgroundSprite, c2);
									meshBuilder.AddQuad(new Vector2(0f, 0f), new Vector2(num2 * (float)num + num3, num2 + num3), z, renderer.backgroundSprite, c);
									meshBuilder.AddQuad(new Vector2(0f, 0f), new Vector2(num2 * (float)num, num2), z, renderer.backgroundSprite, c3);
									int num4 = 0;
									for (int i = 0; i < statusItems.Count; i++)
									{
										StatusItem statusItem = statusItems[i];
										if (statusItem.UseConditionalCallback(overlay, transform) || !(overlay != OverlayModes.None.ID) || !(statusItem.render_overlay != overlay))
										{
											float x = (float)num4 * num2 * 2f - num2 * (float)(num - 1);
											Sprite sprite = statusItems[i].sprite.sprite;
											meshBuilder.AddQuad(new Vector2(x, 0f), new Vector2(num2, num2), z, sprite, c);
											num4++;
										}
									}
									meshBuilder.AddQuad(new Vector2(0f, 0.29f + num3), new Vector2(0.05f + num3, 0.05f + num3), z, renderer.arrowSprite, c);
									meshBuilder.AddQuad(new Vector2(0f, 0.29f), new Vector2(0.05f, 0.05f), z, renderer.arrowSprite, c3);
									meshBuilder.End(mesh);
									dirty = false;
								}
								if (hasVisibleStatusItems && (Object)GameScreenManager.Instance != (Object)null)
								{
									Graphics.DrawMesh(mesh, vector + offset, Quaternion.identity, material, renderer.layer, GameScreenManager.Instance.worldSpaceCanvas.GetComponent<Canvas>().worldCamera, 0, null, false, false);
								}
							}
						}
					}
				}
				else
				{
					string text = "Error cleaning up status items:";
					foreach (StatusItem statusItem3 in statusItems)
					{
						text += statusItem3.Id;
					}
					Debug.LogWarning(text, null);
				}
			}
		}

		public void Add(StatusItem status_item)
		{
			statusItems.Add(status_item);
			dirty = true;
		}

		public void Remove(StatusItem status_item)
		{
			statusItems.Remove(status_item);
			dirty = true;
		}

		public void Replace(Entry entry)
		{
			handle = entry.handle;
			transform = entry.transform;
			offset = entry.offset;
			dirty = true;
			statusItems.Clear();
			statusItems.AddRange(entry.statusItems);
		}

		private bool Intersects(Vector2 pos, float scale)
		{
			if ((Object)transform == (Object)null)
			{
				return false;
			}
			Bounds bounds = mesh.bounds;
			Vector3 vector = transform.GetPosition() + offset + bounds.center;
			Vector2 a = new Vector2(vector.x, vector.y);
			Vector3 size = bounds.size;
			Vector2 b = new Vector2(size.x * scale * 0.5f, size.y * scale * 0.5f);
			Vector2 vector2 = a - b;
			Vector2 vector3 = a + b;
			return pos.x >= vector2.x && pos.x <= vector3.x && pos.y >= vector2.y && pos.y <= vector3.y;
		}

		public void GetIntersection(Vector2 pos, List<SelectTool.Intersection> intersections, float scale)
		{
			if (Intersects(pos, scale))
			{
				KSelectable component = transform.GetComponent<KSelectable>();
				if (component.IsSelectable)
				{
					intersections.Add(new SelectTool.Intersection
					{
						component = transform.GetComponent<KSelectable>(),
						distance = -100f
					});
				}
			}
		}

		public void GetIntersection(Vector2 pos, List<KSelectable> selectables, float scale)
		{
			if (Intersects(pos, scale))
			{
				KSelectable component = transform.GetComponent<KSelectable>();
				if (component.IsSelectable && !selectables.Contains(component))
				{
					selectables.Add(component);
				}
			}
		}

		public void Clear()
		{
			statusItems.Clear();
			offset = Vector3.zero;
			dirty = false;
		}

		public void FreeResources()
		{
			if ((Object)mesh != (Object)null)
			{
				Object.DestroyImmediate(mesh);
				mesh = null;
			}
			if ((Object)material != (Object)null)
			{
				Object.DestroyImmediate(material);
			}
		}

		public void MarkDirty()
		{
			dirty = true;
		}
	}

	private Entry[] entries;

	private int entryCount;

	private Dictionary<int, int> handleTable = new Dictionary<int, int>();

	private Shader shader;

	public List<Entry> visibleEntries = new List<Entry>();

	public int layer
	{
		get;
		private set;
	}

	public int selectedHandle
	{
		get;
		private set;
	}

	public int highlightHandle
	{
		get;
		private set;
	}

	public Color32 backgroundColor
	{
		get;
		private set;
	}

	public Color32 selectedColor
	{
		get;
		private set;
	}

	public Sprite arrowSprite
	{
		get;
		private set;
	}

	public Sprite backgroundSprite
	{
		get;
		private set;
	}

	public float scale
	{
		get;
		private set;
	}

	public StatusItemRenderer()
	{
		layer = LayerMask.NameToLayer("UI");
		entries = new Entry[100];
		shader = Shader.Find("Klei/StatusItem");
		for (int i = 0; i < entries.Length; i++)
		{
			Entry entry = default(Entry);
			entry.Init(shader);
			entries[i] = entry;
		}
		backgroundColor = new Color32(244, 74, 71, byte.MaxValue);
		selectedColor = new Color32(225, 181, 180, byte.MaxValue);
		arrowSprite = Assets.GetSprite("StatusBubbleTop");
		backgroundSprite = Assets.GetSprite("StatusBubble");
		scale = 1f;
		Game.Instance.Subscribe(2095258329, OnHighlightObject);
	}

	public int GetIdx(Transform transform)
	{
		int instanceID = transform.GetInstanceID();
		int value = 0;
		if (!handleTable.TryGetValue(instanceID, out value))
		{
			value = entryCount++;
			handleTable[instanceID] = value;
			Entry entry = entries[value];
			entry.handle = instanceID;
			entry.transform = transform;
			entries[value] = entry;
		}
		return value;
	}

	public void Add(Transform transform, StatusItem status_item)
	{
		if (entryCount == entries.Length)
		{
			Entry[] array = new Entry[entries.Length * 2];
			for (int i = 0; i < entries.Length; i++)
			{
				array[i] = entries[i];
			}
			for (int j = entries.Length; j < array.Length; j++)
			{
				array[j].Init(shader);
			}
			entries = array;
		}
		int idx = GetIdx(transform);
		Entry entry = entries[idx];
		entry.isBuilding = ((Object)transform.GetComponent<Building>() != (Object)null);
		entry.Add(status_item);
		entries[idx] = entry;
	}

	public void Remove(Transform transform, StatusItem status_item)
	{
		int instanceID = transform.GetInstanceID();
		int value = 0;
		if (handleTable.TryGetValue(instanceID, out value))
		{
			Entry entry = entries[value];
			if (entry.statusItems.Count != 0)
			{
				entry.Remove(status_item);
				entries[value] = entry;
				if (entry.statusItems.Count == 0)
				{
					ClearIdx(value);
				}
			}
		}
	}

	private void ClearIdx(int idx)
	{
		Entry entry = entries[idx];
		handleTable.Remove(entry.handle);
		if (idx != entryCount - 1)
		{
			entry.Replace(entries[entryCount - 1]);
			entries[idx] = entry;
			handleTable[entry.handle] = idx;
		}
		entry = entries[entryCount - 1];
		entry.Clear();
		entries[entryCount - 1] = entry;
		entryCount--;
	}

	private HashedString GetMode()
	{
		if ((Object)OverlayScreen.Instance != (Object)null)
		{
			return OverlayScreen.Instance.mode;
		}
		return OverlayModes.None.ID;
	}

	public void MarkAllDirty()
	{
		for (int i = 0; i < entryCount; i++)
		{
			entries[i].MarkDirty();
		}
	}

	public void RenderEveryTick()
	{
		scale = 1f + Mathf.Sin(Time.unscaledTime * 8f) * 0.1f;
		Shader.SetGlobalVector("_StatusItemParameters", new Vector4(scale, 0f, 0f, 0f));
		Camera main = Camera.main;
		Vector3 position = Camera.main.transform.GetPosition();
		Vector3 camera_tr = main.ViewportToWorldPoint(new Vector3(1f, 1f, position.z));
		Camera main2 = Camera.main;
		Vector3 position2 = Camera.main.transform.GetPosition();
		Vector3 camera_bl = main2.ViewportToWorldPoint(new Vector3(0f, 0f, position2.z));
		visibleEntries.Clear();
		for (int i = 0; i < entryCount; i++)
		{
			entries[i].Render(this, camera_bl, camera_tr, GetMode());
		}
	}

	public void GetIntersections(Vector2 pos, List<SelectTool.Intersection> intersections)
	{
		foreach (Entry visibleEntry in visibleEntries)
		{
			visibleEntry.GetIntersection(pos, intersections, scale);
		}
	}

	public void GetIntersections(Vector2 pos, List<KSelectable> selectables)
	{
		foreach (Entry visibleEntry in visibleEntries)
		{
			visibleEntry.GetIntersection(pos, selectables, scale);
		}
	}

	public void SetOffset(Transform transform, Vector3 offset)
	{
		int value = 0;
		if (handleTable.TryGetValue(transform.GetInstanceID(), out value))
		{
			entries[value].offset = offset;
		}
	}

	private void OnSelectObject(object data)
	{
		int value = 0;
		if (handleTable.TryGetValue(selectedHandle, out value))
		{
			entries[value].MarkDirty();
		}
		GameObject gameObject = (GameObject)data;
		if ((Object)gameObject != (Object)null)
		{
			selectedHandle = gameObject.transform.GetInstanceID();
			if (handleTable.TryGetValue(selectedHandle, out value))
			{
				entries[value].MarkDirty();
			}
		}
		else
		{
			highlightHandle = -1;
		}
	}

	private void OnHighlightObject(object data)
	{
		int value = 0;
		if (handleTable.TryGetValue(highlightHandle, out value))
		{
			Entry entry = entries[value];
			entry.MarkDirty();
			entries[value] = entry;
		}
		GameObject gameObject = (GameObject)data;
		if ((Object)gameObject != (Object)null)
		{
			highlightHandle = gameObject.transform.GetInstanceID();
			if (handleTable.TryGetValue(highlightHandle, out value))
			{
				Entry entry2 = entries[value];
				entry2.MarkDirty();
				entries[value] = entry2;
			}
		}
		else
		{
			highlightHandle = -1;
		}
	}

	public void Destroy()
	{
		Game.Instance.Unsubscribe(-1503271301, OnSelectObject);
		Game.Instance.Unsubscribe(-1201923725, OnHighlightObject);
		Entry[] array = entries;
		for (int i = 0; i < array.Length; i++)
		{
			Entry entry = array[i];
			entry.Clear();
			entry.FreeResources();
		}
	}
}
