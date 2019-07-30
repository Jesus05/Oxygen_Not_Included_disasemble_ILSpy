using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConduitFlowVisualizer
{
	[Serializable]
	public class Tuning
	{
		public bool renderMesh;

		public float size;

		public float spriteCount;

		public float framesPerSecond;

		public Texture2D backgroundTexture;

		public Texture2D foregroundTexture;
	}

	private class ConduitFlowMesh
	{
		private Mesh mesh;

		private Material material;

		private List<Vector3> positions = new List<Vector3>();

		private List<Vector4> uvs = new List<Vector4>();

		private List<int> triangles = new List<int>();

		private List<Color32> colors = new List<Color32>();

		private int quadIndex;

		public ConduitFlowMesh()
		{
			mesh = new Mesh();
			mesh.name = "ConduitMesh";
			material = new Material(Shader.Find("Klei/ConduitBall"));
		}

		public void AddQuad(Vector2 pos, Color32 color, float size, float is_foreground, float highlight, Vector2I uvbl, Vector2I uvtl, Vector2I uvbr, Vector2I uvtr)
		{
			float num = size * 0.5f;
			positions.Add(new Vector3(pos.x - num, pos.y - num, 0f));
			positions.Add(new Vector3(pos.x - num, pos.y + num, 0f));
			positions.Add(new Vector3(pos.x + num, pos.y - num, 0f));
			positions.Add(new Vector3(pos.x + num, pos.y + num, 0f));
			uvs.Add(new Vector4((float)uvbl.x, (float)uvbl.y, is_foreground, highlight));
			uvs.Add(new Vector4((float)uvtl.x, (float)uvtl.y, is_foreground, highlight));
			uvs.Add(new Vector4((float)uvbr.x, (float)uvbr.y, is_foreground, highlight));
			uvs.Add(new Vector4((float)uvtr.x, (float)uvtr.y, is_foreground, highlight));
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			triangles.Add(quadIndex * 4);
			triangles.Add(quadIndex * 4 + 1);
			triangles.Add(quadIndex * 4 + 2);
			triangles.Add(quadIndex * 4 + 2);
			triangles.Add(quadIndex * 4 + 1);
			triangles.Add(quadIndex * 4 + 3);
			quadIndex++;
		}

		public void SetTexture(string id, Texture2D texture)
		{
			material.SetTexture(id, texture);
		}

		public void SetVector(string id, Vector4 data)
		{
			material.SetVector(id, data);
		}

		public void Begin()
		{
			positions.Clear();
			uvs.Clear();
			triangles.Clear();
			colors.Clear();
			quadIndex = 0;
		}

		public void End(float z, int layer)
		{
			mesh.Clear();
			mesh.SetVertices(positions);
			mesh.SetUVs(0, uvs);
			mesh.SetColors(colors);
			mesh.SetTriangles(triangles, 0, false);
			Graphics.DrawMesh(mesh, new Vector3(GRID_OFFSET.x, GRID_OFFSET.y, z - 0.1f), Quaternion.identity, material, layer);
		}

		public void Cleanup()
		{
			UnityEngine.Object.Destroy(mesh);
			mesh = null;
			UnityEngine.Object.Destroy(material);
			material = null;
		}
	}

	private struct AudioInfo
	{
		public int networkID;

		public int blobCount;

		public float distance;

		public Vector3 position;
	}

	private struct RenderMeshContext
	{
		public ListPool<int, ConduitFlowVisualizer>.PooledList visible_conduits;

		public ConduitFlowVisualizer outer;

		public float lerp_percent;

		public RenderMeshContext(ConduitFlowVisualizer outer, float lerp_percent, Vector2I min, Vector2I max)
		{
			this.outer = outer;
			this.lerp_percent = lerp_percent;
			visible_conduits = ListPool<int, ConduitFlowVisualizer>.Allocate();
			visible_conduits.Capacity = outer.flowManager.soaInfo.NumEntries;
			for (int i = 0; i != outer.flowManager.soaInfo.NumEntries; i++)
			{
				Vector2I vector2I = Grid.CellToXY(outer.flowManager.soaInfo.GetCell(i));
				if (min <= vector2I && vector2I <= max)
				{
					visible_conduits.Add(i);
				}
			}
		}

		public void Finish()
		{
			visible_conduits.Recycle();
		}
	}

	private struct RenderMeshTask : IWorkItem<RenderMeshContext>
	{
		public struct Ball
		{
			private class UVPack
			{
				public Vector2I bl;

				public Vector2I tl;

				public Vector2I br;

				public Vector2I tr;
			}

			private Vector2 pos;

			private float size;

			private Color32 color;

			private ConduitFlow.FlowDirections direction;

			private bool foreground;

			private bool highlight;

			private static Dictionary<ConduitFlow.FlowDirections, UVPack> uv_packs = new Dictionary<ConduitFlow.FlowDirections, UVPack>();

			public Ball(ConduitFlow.FlowDirections direction, Vector2 pos, Color32 color, float size, bool foreground, bool highlight)
			{
				this.pos = pos;
				this.size = size;
				this.color = color;
				this.direction = direction;
				this.foreground = foreground;
				this.highlight = highlight;
			}

			public static void InitializeResources()
			{
				uv_packs[ConduitFlow.FlowDirections.None] = new UVPack
				{
					bl = new Vector2I(0, 0),
					tl = new Vector2I(0, 1),
					br = new Vector2I(1, 0),
					tr = new Vector2I(1, 1)
				};
				uv_packs[ConduitFlow.FlowDirections.Left] = new UVPack
				{
					bl = new Vector2I(0, 0),
					tl = new Vector2I(0, 1),
					br = new Vector2I(1, 0),
					tr = new Vector2I(1, 1)
				};
				uv_packs[ConduitFlow.FlowDirections.Right] = uv_packs[ConduitFlow.FlowDirections.Left];
				uv_packs[ConduitFlow.FlowDirections.Up] = new UVPack
				{
					bl = new Vector2I(1, 0),
					tl = new Vector2I(0, 0),
					br = new Vector2I(1, 1),
					tr = new Vector2I(0, 1)
				};
				uv_packs[ConduitFlow.FlowDirections.Down] = uv_packs[ConduitFlow.FlowDirections.Up];
			}

			private static UVPack GetUVPack(ConduitFlow.FlowDirections direction)
			{
				return uv_packs[direction];
			}

			public void Consume(ConduitFlowMesh mesh)
			{
				UVPack uVPack = GetUVPack(direction);
				mesh.AddQuad(pos, color, size, (float)(foreground ? 1 : 0), (float)(highlight ? 1 : 0), uVPack.bl, uVPack.tl, uVPack.br, uVPack.tr);
			}
		}

		private ListPool<Ball, RenderMeshTask>.PooledList moving_balls;

		private ListPool<Ball, RenderMeshTask>.PooledList static_balls;

		private ListPool<ConduitFlow.Conduit, RenderMeshTask>.PooledList moving_conduits;

		private int start;

		private int end;

		public RenderMeshTask(int start, int end)
		{
			this.start = start;
			this.end = end;
			int capacity = end - start;
			moving_balls = ListPool<Ball, RenderMeshTask>.Allocate();
			moving_balls.Capacity = capacity;
			static_balls = ListPool<Ball, RenderMeshTask>.Allocate();
			static_balls.Capacity = capacity;
			moving_conduits = ListPool<ConduitFlow.Conduit, RenderMeshTask>.Allocate();
			moving_conduits.Capacity = capacity;
		}

		public void Run(RenderMeshContext context)
		{
			Element element = null;
			for (int i = start; i != end; i++)
			{
				ConduitFlow.Conduit conduit = context.outer.flowManager.soaInfo.GetConduit(context.visible_conduits[i]);
				ConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(context.outer.flowManager);
				ConduitFlow.ConduitContents initialContents = conduit.GetInitialContents(context.outer.flowManager);
				if (lastFlowInfo.contents.mass > 0f)
				{
					int cell = conduit.GetCell(context.outer.flowManager);
					int cellFromDirection = ConduitFlow.GetCellFromDirection(cell, lastFlowInfo.direction);
					Vector2I v = Grid.CellToXY(cell);
					Vector2I vector2I = Grid.CellToXY(cellFromDirection);
					Vector2 pos = (cell != -1) ? Vector2.Lerp(new Vector2((float)v.x, (float)v.y), new Vector2((float)vector2I.x, (float)vector2I.y), context.lerp_percent) : ((Vector2)v);
					Color32 cellTintColour = context.outer.GetCellTintColour(cell);
					Color32 cellTintColour2 = context.outer.GetCellTintColour(cellFromDirection);
					Color32 color = Color32.Lerp(cellTintColour, cellTintColour2, context.lerp_percent);
					bool highlight = false;
					if (context.outer.showContents)
					{
						if (lastFlowInfo.contents.mass >= initialContents.mass)
						{
							moving_balls.Add(new Ball(lastFlowInfo.direction, pos, color, context.outer.tuning.size, false, false));
						}
						if (element == null || lastFlowInfo.contents.element != element.id)
						{
							element = ElementLoader.FindElementByHash(lastFlowInfo.contents.element);
						}
					}
					else
					{
						element = null;
						int num = Grid.PosToCell(new Vector3(pos.x + GRID_OFFSET.x, pos.y + GRID_OFFSET.y, 0f));
						highlight = (num == context.outer.highlightedCell);
					}
					Color32 contentsColor = context.outer.GetContentsColor(element, color);
					float num2 = 1f;
					if (context.outer.showContents || lastFlowInfo.contents.mass < initialContents.mass)
					{
						num2 = context.outer.CalculateMassScale(lastFlowInfo.contents.mass);
					}
					moving_balls.Add(new Ball(lastFlowInfo.direction, pos, contentsColor, context.outer.tuning.size * num2, true, highlight));
					moving_conduits.Add(conduit);
				}
				if (initialContents.mass > lastFlowInfo.contents.mass && initialContents.mass > 0f)
				{
					int cell2 = conduit.GetCell(context.outer.flowManager);
					Vector2I v2 = Grid.CellToXY(cell2);
					Vector2 pos2 = v2;
					float mass = initialContents.mass - lastFlowInfo.contents.mass;
					bool highlight2 = false;
					Color32 cellTintColour3 = context.outer.GetCellTintColour(cell2);
					float num3 = context.outer.CalculateMassScale(mass);
					if (context.outer.showContents)
					{
						static_balls.Add(new Ball(ConduitFlow.FlowDirections.None, pos2, cellTintColour3, context.outer.tuning.size * num3, false, false));
						if (element == null || initialContents.element != element.id)
						{
							element = ElementLoader.FindElementByHash(initialContents.element);
						}
					}
					else
					{
						element = null;
						highlight2 = (cell2 == context.outer.highlightedCell);
					}
					Color32 contentsColor2 = context.outer.GetContentsColor(element, cellTintColour3);
					static_balls.Add(new Ball(ConduitFlow.FlowDirections.None, pos2, contentsColor2, context.outer.tuning.size * num3, true, highlight2));
				}
			}
		}

		public void Finish(ConduitFlowMesh moving_ball_mesh, ConduitFlowMesh static_ball_mesh, Vector3 camera_pos, ConduitFlowVisualizer visualizer)
		{
			for (int i = 0; i != moving_balls.Count; i++)
			{
				moving_balls[i].Consume(moving_ball_mesh);
			}
			moving_balls.Recycle();
			for (int j = 0; j != static_balls.Count; j++)
			{
				static_balls[j].Consume(static_ball_mesh);
			}
			static_balls.Recycle();
			if (visualizer != null)
			{
				foreach (ConduitFlow.Conduit moving_conduit in moving_conduits)
				{
					visualizer.AddAudioSource(moving_conduit, camera_pos);
				}
			}
			moving_conduits.Recycle();
		}
	}

	private ConduitFlow flowManager;

	private string overlaySound;

	private bool showContents;

	private double animTime;

	private int layer;

	private static Vector2 GRID_OFFSET = new Vector2(0.5f, 0.5f);

	private List<AudioInfo> audioInfo;

	private HashSet<int> insulatedCells = new HashSet<int>();

	private HashSet<int> radiantCells = new HashSet<int>();

	private Game.ConduitVisInfo visInfo;

	private ConduitFlowMesh movingBallMesh;

	private ConduitFlowMesh staticBallMesh;

	private int highlightedCell = -1;

	private Color32 highlightColour = new Color(0.2f, 0.2f, 0.2f, 0.2f);

	private Tuning tuning;

	private static WorkItemCollection<RenderMeshTask, RenderMeshContext> render_mesh_job = new WorkItemCollection<RenderMeshTask, RenderMeshContext>();

	public ConduitFlowVisualizer(ConduitFlow flow_manager, Game.ConduitVisInfo vis_info, string overlay_sound, Tuning tuning)
	{
		flowManager = flow_manager;
		visInfo = vis_info;
		overlaySound = overlay_sound;
		this.tuning = tuning;
		movingBallMesh = new ConduitFlowMesh();
		staticBallMesh = new ConduitFlowMesh();
		RenderMeshTask.Ball.InitializeResources();
	}

	public void FreeResources()
	{
		movingBallMesh.Cleanup();
		staticBallMesh.Cleanup();
	}

	private float CalculateMassScale(float mass)
	{
		float t = (mass - visInfo.overlayMassScaleRange.x) / (visInfo.overlayMassScaleRange.y - visInfo.overlayMassScaleRange.x);
		return Mathf.Lerp(visInfo.overlayMassScaleValues.x, visInfo.overlayMassScaleValues.y, t);
	}

	private Color32 GetContentsColor(Element element, Color32 default_color)
	{
		if (element != null)
		{
			Color c = element.substance.conduitColour;
			c.a = 128f;
			return c;
		}
		return default_color;
	}

	private Color32 GetTintColour()
	{
		return (!showContents) ? visInfo.tint : visInfo.overlayTint;
	}

	private Color32 GetInsulatedTintColour()
	{
		return (!showContents) ? visInfo.insulatedTint : visInfo.overlayInsulatedTint;
	}

	private Color32 GetRadiantTintColour()
	{
		return (!showContents) ? visInfo.radiantTint : visInfo.overlayRadiantTint;
	}

	private Color32 GetCellTintColour(int cell)
	{
		if (!insulatedCells.Contains(cell))
		{
			if (!radiantCells.Contains(cell))
			{
				return GetTintColour();
			}
			return GetRadiantTintColour();
		}
		return GetInsulatedTintColour();
	}

	public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
	{
		animTime += (double)Time.deltaTime;
		if (trigger_audio)
		{
			if (audioInfo == null)
			{
				audioInfo = new List<AudioInfo>();
			}
			for (int i = 0; i < audioInfo.Count; i++)
			{
				AudioInfo value = audioInfo[i];
				value.distance = float.PositiveInfinity;
				value.position = Vector3.zero;
				value.blobCount = (value.blobCount + 1) % 10;
				audioInfo[i] = value;
			}
		}
		if (tuning.renderMesh)
		{
			RenderMesh(z, render_layer, lerp_percent, trigger_audio);
		}
		if (trigger_audio)
		{
			TriggerAudio();
		}
	}

	private void RenderMesh(float z, int render_layer, float lerp_percent, bool trigger_audio)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I min = visibleArea.Min;
		int a = Mathf.Max(0, min.x - 1);
		Vector2I min2 = visibleArea.Min;
		Vector2I min3 = new Vector2I(a, Mathf.Max(0, min2.y - 1));
		int a2 = Grid.WidthInCells - 1;
		Vector2I max = visibleArea.Max;
		int a3 = Mathf.Min(a2, max.x + 1);
		int a4 = Grid.HeightInCells - 1;
		Vector2I max2 = visibleArea.Max;
		Vector2I max3 = new Vector2I(a3, Mathf.Min(a4, max2.y + 1));
		RenderMeshContext shared_data = new RenderMeshContext(this, lerp_percent, min3, max3);
		if (shared_data.visible_conduits.Count == 0)
		{
			shared_data.Finish();
		}
		else
		{
			render_mesh_job.Reset(shared_data);
			int num = Mathf.Max(1, (int)((float)(shared_data.visible_conduits.Count / CPUBudget.coreCount) / 1.5f));
			int num2 = Mathf.Max(1, shared_data.visible_conduits.Count / num);
			for (int i = 0; i != num2; i++)
			{
				int num3 = i * num;
				int end = (i != num2 - 1) ? (num3 + num) : shared_data.visible_conduits.Count;
				render_mesh_job.Add(new RenderMeshTask(num3, end));
			}
			GlobalJobManager.Run(render_mesh_job);
			float z2 = 0f;
			if (showContents)
			{
				z2 = 1f;
			}
			int num4 = (int)(animTime / (1.0 / (double)tuning.framesPerSecond)) % (int)tuning.spriteCount;
			float w = (float)num4 * (1f / tuning.spriteCount);
			movingBallMesh.Begin();
			movingBallMesh.SetTexture("_BackgroundTex", tuning.backgroundTexture);
			movingBallMesh.SetTexture("_ForegroundTex", tuning.foregroundTexture);
			movingBallMesh.SetVector("_SpriteSettings", new Vector4(1f / tuning.spriteCount, 1f, z2, w));
			movingBallMesh.SetVector("_Highlight", new Vector4((float)(int)highlightColour.r / 255f, (float)(int)highlightColour.g / 255f, (float)(int)highlightColour.b / 255f, 0f));
			staticBallMesh.Begin();
			staticBallMesh.SetTexture("_BackgroundTex", tuning.backgroundTexture);
			staticBallMesh.SetTexture("_ForegroundTex", tuning.foregroundTexture);
			staticBallMesh.SetVector("_SpriteSettings", new Vector4(1f / tuning.spriteCount, 1f, z2, 0f));
			staticBallMesh.SetVector("_Highlight", new Vector4((float)(int)highlightColour.r / 255f, (float)(int)highlightColour.g / 255f, (float)(int)highlightColour.b / 255f, 0f));
			Vector3 position = CameraController.Instance.transform.GetPosition();
			ConduitFlowVisualizer visualizer = (!trigger_audio) ? null : this;
			for (int j = 0; j != render_mesh_job.Count; j++)
			{
				render_mesh_job.GetWorkItem(j).Finish(movingBallMesh, staticBallMesh, position, visualizer);
			}
			movingBallMesh.End(z, layer);
			staticBallMesh.End(z, layer);
			shared_data.Finish();
		}
	}

	public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
	{
		showContents = show_contents;
		layer = ((show_contents && move_to_overlay_layer) ? LayerMask.NameToLayer("MaskedOverlay") : 0);
	}

	private void AddAudioSource(ConduitFlow.Conduit conduit, Vector3 camera_pos)
	{
		using (new KProfiler.Region("AddAudioSource", null))
		{
			UtilityNetwork network = flowManager.GetNetwork(conduit);
			if (network != null)
			{
				Vector3 vector = Grid.CellToPosCCC(conduit.GetCell(flowManager), Grid.SceneLayer.Building);
				float num = Vector3.SqrMagnitude(vector - camera_pos);
				bool flag = false;
				for (int i = 0; i < audioInfo.Count; i++)
				{
					AudioInfo value = audioInfo[i];
					if (value.networkID == network.id)
					{
						if (num < value.distance)
						{
							value.distance = num;
							value.position = vector;
							audioInfo[i] = value;
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					AudioInfo item = default(AudioInfo);
					item.networkID = network.id;
					item.position = vector;
					item.distance = num;
					item.blobCount = 0;
					audioInfo.Add(item);
				}
			}
		}
	}

	private void TriggerAudio()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			CameraController instance = CameraController.Instance;
			int num = 0;
			List<AudioInfo> list = new List<AudioInfo>();
			for (int i = 0; i < this.audioInfo.Count; i++)
			{
				CameraController cameraController = instance;
				AudioInfo audioInfo = this.audioInfo[i];
				if (cameraController.IsVisiblePos(audioInfo.position))
				{
					list.Add(this.audioInfo[i]);
					num++;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				AudioInfo audioInfo2 = list[j];
				if (audioInfo2.distance != float.PositiveInfinity)
				{
					EventInstance instance2 = SoundEvent.BeginOneShot(overlaySound, audioInfo2.position);
					instance2.setParameterValue("blobCount", (float)audioInfo2.blobCount);
					instance2.setParameterValue("networkCount", (float)num);
					SoundEvent.EndOneShot(instance2);
				}
			}
		}
	}

	public void AddThermalConductivity(int cell, float conductivity)
	{
		if (conductivity < 1f)
		{
			insulatedCells.Add(cell);
		}
		else if (conductivity > 1f)
		{
			radiantCells.Add(cell);
		}
	}

	public void RemoveThermalConductivity(int cell, float conductivity)
	{
		if (conductivity < 1f)
		{
			insulatedCells.Remove(cell);
		}
		else if (conductivity > 1f)
		{
			radiantCells.Remove(cell);
		}
	}

	public void SetHighlightedCell(int cell)
	{
		highlightedCell = cell;
	}
}
