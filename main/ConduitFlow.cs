#define UNITY_ASSERTIONS
using Klei;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{conduitType}")]
public class ConduitFlow : IConduitFlow
{
	[DebuggerDisplay("{NumEntries}")]
	public class SOAInfo
	{
		private abstract class ConduitTask : DivisibleTask<SOAInfo>
		{
			public ConduitFlow manager;

			public ConduitTask(string name)
				: base(name)
			{
			}
		}

		private class ConduitTaskDivision<Task> : TaskDivision<Task, SOAInfo> where Task : ConduitTask, new()
		{
			public void Initialize(int conduitCount, ConduitFlow manager)
			{
				Initialize(conduitCount);
				Task[] tasks = base.tasks;
				foreach (Task val in tasks)
				{
					val.manager = manager;
				}
			}
		}

		private class ConduitJob : WorkItemCollection<ConduitTask, SOAInfo>
		{
			public void Add<Task>(ConduitTaskDivision<Task> taskDivision) where Task : ConduitTask, new()
			{
				Task[] tasks = ((TaskDivision<Task, SOAInfo>)taskDivision).tasks;
				foreach (Task work_item in tasks)
				{
					base.Add((ConduitTask)work_item);
				}
			}
		}

		private class ClearPermanentDiseaseContainer : ConduitTask
		{
			public ClearPermanentDiseaseContainer()
				: base("ClearPermanentDiseaseContainer")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					soaInfo.ForcePermanentDiseaseContainer(i, false);
				}
			}
		}

		private class PublishTemperatureToSim : ConduitTask
		{
			public PublishTemperatureToSim()
				: base("PublishTemperatureToSim")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					HandleVector<int>.Handle handle = soaInfo.temperatureHandles[i];
					if (handle.IsValid())
					{
						float temperature = Game.Instance.conduitTemperatureManager.GetTemperature(handle);
						manager.grid[soaInfo.cells[i]].contents.temperature = temperature;
					}
				}
			}
		}

		private class PublishDiseaseToSim : ConduitTask
		{
			public PublishDiseaseToSim()
				: base("PublishDiseaseToSim")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					HandleVector<int>.Handle handle = soaInfo.diseaseHandles[i];
					if (handle.IsValid())
					{
						ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(handle);
						int num = soaInfo.cells[i];
						manager.grid[num].contents.diseaseIdx = data.diseaseIdx;
						manager.grid[num].contents.diseaseCount = data.diseaseCount;
					}
				}
			}
		}

		private class ResetConduit : ConduitTask
		{
			public ResetConduit()
				: base("ResetConduitTask")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					manager.grid[soaInfo.cells[i]].conduitIdx = -1;
				}
			}
		}

		private class SetInitialContents : ConduitTask
		{
			public SetInitialContents()
				: base("SetInitialContents")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					soaInfo.initialContents[i] = soaInfo.conduits[i].GetContents(manager);
					manager.grid[soaInfo.cells[i]].contents = soaInfo.initialContents[i];
				}
			}
		}

		private class InvalidateLastFlow : ConduitTask
		{
			public InvalidateLastFlow()
				: base("InvalidateLastFlow")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					soaInfo.lastFlowInfo[i] = ConduitFlowInfo.DEFAULT;
				}
			}
		}

		private class PublishTemperatureToGame : ConduitTask
		{
			public PublishTemperatureToGame()
				: base("PublishTemperatureToGame")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					Game.Instance.conduitTemperatureManager.SetData(soaInfo.temperatureHandles[i], ref manager.grid[soaInfo.cells[i]].contents);
				}
			}
		}

		private class PublishDiseaseToGame : ConduitTask
		{
			public PublishDiseaseToGame()
				: base("PublishDiseaseToGame")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					Game.Instance.conduitDiseaseManager.SetData(soaInfo.diseaseHandles[i], ref manager.grid[soaInfo.cells[i]].contents);
				}
			}
		}

		private class FlowThroughVacuum : ConduitTask
		{
			public FlowThroughVacuum()
				: base("FlowThroughVacuum")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					Conduit conduit = soaInfo.conduits[i];
					int cell = conduit.GetCell(manager);
					if (manager.grid[cell].contents.element == SimHashes.Vacuum)
					{
						soaInfo.srcFlowDirections[conduit.idx] = FlowDirections.None;
					}
				}
			}
		}

		private List<Conduit> conduits = new List<Conduit>();

		private List<ConduitConnections> conduitConnections = new List<ConduitConnections>();

		private List<ConduitFlowInfo> lastFlowInfo = new List<ConduitFlowInfo>();

		private List<ConduitContents> initialContents = new List<ConduitContents>();

		private List<GameObject> conduitGOs = new List<GameObject>();

		private List<bool> diseaseContentsVisible = new List<bool>();

		private List<int> cells = new List<int>();

		private List<FlowDirections> permittedFlowDirections = new List<FlowDirections>();

		private List<FlowDirections> srcFlowDirections = new List<FlowDirections>();

		private List<FlowDirections> pullDirections = new List<FlowDirections>();

		private List<FlowDirections> targetFlowDirections = new List<FlowDirections>();

		private List<HandleVector<int>.Handle> structureTemperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> temperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> diseaseHandles = new List<HandleVector<int>.Handle>();

		private ConduitTaskDivision<ClearPermanentDiseaseContainer> clearPermanentDiseaseContainer = new ConduitTaskDivision<ClearPermanentDiseaseContainer>();

		private ConduitTaskDivision<PublishTemperatureToSim> publishTemperatureToSim = new ConduitTaskDivision<PublishTemperatureToSim>();

		private ConduitTaskDivision<PublishDiseaseToSim> publishDiseaseToSim = new ConduitTaskDivision<PublishDiseaseToSim>();

		private ConduitTaskDivision<ResetConduit> resetConduit = new ConduitTaskDivision<ResetConduit>();

		private ConduitJob clearJob = new ConduitJob();

		private ConduitTaskDivision<SetInitialContents> setInitialContents = new ConduitTaskDivision<SetInitialContents>();

		private ConduitTaskDivision<InvalidateLastFlow> invalidateLastFlow = new ConduitTaskDivision<InvalidateLastFlow>();

		private ConduitJob beginFrameJob = new ConduitJob();

		private ConduitTaskDivision<PublishTemperatureToGame> publishTemperatureToGame = new ConduitTaskDivision<PublishTemperatureToGame>();

		private ConduitTaskDivision<PublishDiseaseToGame> publishDiseaseToGame = new ConduitTaskDivision<PublishDiseaseToGame>();

		private ConduitJob endFrameJob = new ConduitJob();

		private ConduitTaskDivision<FlowThroughVacuum> flowThroughVacuum = new ConduitTaskDivision<FlowThroughVacuum>();

		private ConduitJob updateFlowDirectionJob = new ConduitJob();

		public int NumEntries => conduits.Count;

		public int AddConduit(ConduitFlow manager, GameObject conduit_go, int cell)
		{
			int count = conduitConnections.Count;
			Conduit item = new Conduit(count);
			conduits.Add(item);
			conduitConnections.Add(new ConduitConnections
			{
				left = -1,
				right = -1,
				up = -1,
				down = -1
			});
			ConduitContents contents = manager.grid[cell].contents;
			initialContents.Add(contents);
			lastFlowInfo.Add(ConduitFlowInfo.DEFAULT);
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(conduit_go);
			HandleVector<int>.Handle handle2 = Game.Instance.conduitTemperatureManager.Allocate(manager.conduitType, count, handle, ref contents);
			HandleVector<int>.Handle item2 = Game.Instance.conduitDiseaseManager.Allocate(handle2, ref contents);
			cells.Add(cell);
			diseaseContentsVisible.Add(false);
			structureTemperatureHandles.Add(handle);
			temperatureHandles.Add(handle2);
			diseaseHandles.Add(item2);
			conduitGOs.Add(conduit_go);
			permittedFlowDirections.Add(FlowDirections.None);
			srcFlowDirections.Add(FlowDirections.None);
			pullDirections.Add(FlowDirections.None);
			targetFlowDirections.Add(FlowDirections.None);
			return count;
		}

		public void Clear(ConduitFlow manager)
		{
			if (clearJob.Count == 0)
			{
				clearJob.Reset(this);
				clearJob.Add(publishTemperatureToSim);
				clearJob.Add(publishDiseaseToSim);
				clearJob.Add(resetConduit);
			}
			clearPermanentDiseaseContainer.Initialize(conduits.Count, manager);
			publishTemperatureToSim.Initialize(conduits.Count, manager);
			publishDiseaseToSim.Initialize(conduits.Count, manager);
			resetConduit.Initialize(conduits.Count, manager);
			clearPermanentDiseaseContainer.Run(this);
			GlobalJobManager.Run(clearJob);
			for (int i = 0; i != conduits.Count; i++)
			{
				Game.Instance.conduitDiseaseManager.Free(diseaseHandles[i]);
			}
			for (int j = 0; j != conduits.Count; j++)
			{
				Game.Instance.conduitTemperatureManager.Free(temperatureHandles[j]);
			}
			cells.Clear();
			diseaseContentsVisible.Clear();
			permittedFlowDirections.Clear();
			srcFlowDirections.Clear();
			pullDirections.Clear();
			targetFlowDirections.Clear();
			conduitGOs.Clear();
			diseaseHandles.Clear();
			temperatureHandles.Clear();
			structureTemperatureHandles.Clear();
			initialContents.Clear();
			lastFlowInfo.Clear();
			conduitConnections.Clear();
			conduits.Clear();
		}

		public Conduit GetConduit(int idx)
		{
			return conduits[idx];
		}

		public ConduitConnections GetConduitConnections(int idx)
		{
			return conduitConnections[idx];
		}

		public void SetConduitConnections(int idx, ConduitConnections data)
		{
			conduitConnections[idx] = data;
		}

		public float GetConduitTemperature(int idx)
		{
			HandleVector<int>.Handle handle = temperatureHandles[idx];
			float temperature = Game.Instance.conduitTemperatureManager.GetTemperature(handle);
			Debug.Assert(!float.IsNaN(temperature));
			return temperature;
		}

		public void SetConduitTemperatureData(int idx, ref ConduitContents contents)
		{
			HandleVector<int>.Handle handle = temperatureHandles[idx];
			Game.Instance.conduitTemperatureManager.SetData(handle, ref contents);
		}

		public ConduitDiseaseManager.Data GetDiseaseData(int idx)
		{
			HandleVector<int>.Handle handle = diseaseHandles[idx];
			return Game.Instance.conduitDiseaseManager.GetData(handle);
		}

		public void SetDiseaseData(int idx, ref ConduitContents contents)
		{
			HandleVector<int>.Handle handle = diseaseHandles[idx];
			Game.Instance.conduitDiseaseManager.SetData(handle, ref contents);
		}

		public GameObject GetConduitGO(int idx)
		{
			return conduitGOs[idx];
		}

		public void ForcePermanentDiseaseContainer(int idx, bool force_on)
		{
			bool flag = diseaseContentsVisible[idx];
			if (flag != force_on)
			{
				diseaseContentsVisible[idx] = force_on;
				GameObject gameObject = conduitGOs[idx];
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					component.ForcePermanentDiseaseContainer(force_on);
				}
			}
		}

		public Conduit GetConduitFromDirection(int idx, FlowDirections direction)
		{
			Conduit result = Conduit.Invalid();
			ConduitConnections conduitConnections = this.conduitConnections[idx];
			switch (direction)
			{
			case FlowDirections.Left:
				result = ((conduitConnections.left == -1) ? Conduit.Invalid() : conduits[conduitConnections.left]);
				break;
			case FlowDirections.Right:
				result = ((conduitConnections.right == -1) ? Conduit.Invalid() : conduits[conduitConnections.right]);
				break;
			case FlowDirections.Up:
				result = ((conduitConnections.up == -1) ? Conduit.Invalid() : conduits[conduitConnections.up]);
				break;
			case FlowDirections.Down:
				result = ((conduitConnections.down == -1) ? Conduit.Invalid() : conduits[conduitConnections.down]);
				break;
			}
			return result;
		}

		public void BeginFrame(ConduitFlow manager)
		{
			if (beginFrameJob.Count == 0)
			{
				beginFrameJob.Reset(this);
				beginFrameJob.Add(setInitialContents);
				beginFrameJob.Add(invalidateLastFlow);
			}
			setInitialContents.Initialize(conduits.Count, manager);
			invalidateLastFlow.Initialize(conduits.Count, manager);
			GlobalJobManager.Run(beginFrameJob);
		}

		public void EndFrame(ConduitFlow manager)
		{
			if (endFrameJob.Count == 0)
			{
				endFrameJob.Reset(this);
				endFrameJob.Add(publishDiseaseToGame);
			}
			publishTemperatureToGame.Initialize(conduits.Count, manager);
			publishDiseaseToGame.Initialize(conduits.Count, manager);
			publishTemperatureToGame.Run(this);
			GlobalJobManager.Run(endFrameJob);
		}

		public void UpdateFlowDirection(ConduitFlow manager)
		{
			if (updateFlowDirectionJob.Count == 0)
			{
				updateFlowDirectionJob.Reset(this);
				updateFlowDirectionJob.Add(flowThroughVacuum);
			}
			flowThroughVacuum.Initialize(conduits.Count, manager);
			GlobalJobManager.Run(updateFlowDirectionJob);
		}

		public void MarkConduitEmpty(int idx, ConduitFlow manager)
		{
			ConduitFlowInfo conduitFlowInfo = lastFlowInfo[idx];
			if (conduitFlowInfo.direction != 0)
			{
				lastFlowInfo[idx] = ConduitFlowInfo.DEFAULT;
				Conduit conduit = conduits[idx];
				targetFlowDirections[idx] = FlowDirections.None;
				int num = cells[idx];
				manager.grid[num].contents = ConduitContents.Empty;
			}
		}

		public void ResetLastFlowInfo(int idx)
		{
			lastFlowInfo[idx] = ConduitFlowInfo.DEFAULT;
		}

		public void SetLastFlowInfo(int idx, FlowDirections direction, ref ConduitContents contents)
		{
			lastFlowInfo[idx] = new ConduitFlowInfo
			{
				direction = direction,
				contents = contents
			};
		}

		public ConduitContents GetInitialContents(int idx)
		{
			return initialContents[idx];
		}

		public ConduitFlowInfo GetLastFlowInfo(int idx)
		{
			return lastFlowInfo[idx];
		}

		public FlowDirections GetPermittedFlowDirections(int idx)
		{
			return permittedFlowDirections[idx];
		}

		public void SetPermittedFlowDirections(int idx, FlowDirections permitted)
		{
			permittedFlowDirections[idx] = permitted;
		}

		public FlowDirections AddPermittedFlowDirections(int idx, FlowDirections delta)
		{
			List<FlowDirections> list;
			int index;
			return (list = permittedFlowDirections)[index = idx] = (list[index] | delta);
		}

		public FlowDirections RemovePermittedFlowDirections(int idx, FlowDirections delta)
		{
			List<FlowDirections> list;
			int index;
			return (list = permittedFlowDirections)[index = idx] = (FlowDirections)((int)list[index] & (int)(byte)(~(uint)delta));
		}

		public FlowDirections GetTargetFlowDirection(int idx)
		{
			return targetFlowDirections[idx];
		}

		public void SetTargetFlowDirection(int idx, FlowDirections directions)
		{
			targetFlowDirections[idx] = directions;
		}

		public FlowDirections GetSrcFlowDirection(int idx)
		{
			return srcFlowDirections[idx];
		}

		public void SetSrcFlowDirection(int idx, FlowDirections directions)
		{
			srcFlowDirections[idx] = directions;
		}

		public FlowDirections GetPullDirection(int idx)
		{
			return pullDirections[idx];
		}

		public void SetPullDirection(int idx, FlowDirections directions)
		{
			pullDirections[idx] = directions;
		}

		public int GetCell(int idx)
		{
			return cells[idx];
		}

		public void SetCell(int idx, int cell)
		{
			cells[idx] = cell;
		}
	}

	[DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
	public struct ConduitUpdater
	{
		public ConduitFlowPriority priority;

		public Action<float> callback;
	}

	[DebuggerDisplay("conduit {conduitIdx}:{contents.element}")]
	public struct GridNode
	{
		public int conduitIdx;

		public ConduitContents contents;
	}

	public struct SerializedContents
	{
		public SimHashes element;

		public float mass;

		public float temperature;

		public int diseaseHash;

		public int diseaseCount;

		public SerializedContents(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
		{
			this.element = element;
			this.mass = mass;
			this.temperature = temperature;
			diseaseHash = ((disease_idx != 255) ? Db.Get().Diseases[disease_idx].id.GetHashCode() : 0);
			diseaseCount = disease_count;
			if (diseaseCount <= 0)
			{
				diseaseHash = 0;
			}
		}

		public SerializedContents(ConduitContents src)
		{
			this = new SerializedContents(src.element, src.mass, src.temperature, src.diseaseIdx, src.diseaseCount);
		}
	}

	[Flags]
	public enum FlowDirections : byte
	{
		None = 0x0,
		Down = 0x1,
		Left = 0x2,
		Right = 0x4,
		Up = 0x8,
		All = 0xF
	}

	[DebuggerDisplay("conduits l:{left}, r:{right}, u:{up}, d:{down}")]
	public struct ConduitConnections
	{
		public int left;

		public int right;

		public int up;

		public int down;

		public static readonly ConduitConnections DEFAULT = new ConduitConnections
		{
			left = -1,
			right = -1,
			up = -1,
			down = -1
		};
	}

	[DebuggerDisplay("{direction}:{contents.element}")]
	public struct ConduitFlowInfo
	{
		public FlowDirections direction;

		public ConduitContents contents;

		public static readonly ConduitFlowInfo DEFAULT = new ConduitFlowInfo
		{
			direction = FlowDirections.None,
			contents = ConduitContents.Empty
		};
	}

	[Serializable]
	[DebuggerDisplay("conduit {idx}")]
	public struct Conduit : IEquatable<Conduit>
	{
		public int idx;

		public Conduit(int idx)
		{
			this.idx = idx;
		}

		public static Conduit Invalid()
		{
			return new Conduit(-1);
		}

		public FlowDirections GetPermittedFlowDirections(ConduitFlow manager)
		{
			return manager.soaInfo.GetPermittedFlowDirections(idx);
		}

		public void SetPermittedFlowDirections(FlowDirections permitted, ConduitFlow manager)
		{
			manager.soaInfo.SetPermittedFlowDirections(idx, permitted);
		}

		public FlowDirections GetTargetFlowDirection(ConduitFlow manager)
		{
			return manager.soaInfo.GetTargetFlowDirection(idx);
		}

		public void SetTargetFlowDirection(FlowDirections directions, ConduitFlow manager)
		{
			manager.soaInfo.SetTargetFlowDirection(idx, directions);
		}

		public ConduitContents GetContents(ConduitFlow manager)
		{
			int cell = manager.soaInfo.GetCell(idx);
			ConduitContents contents = manager.grid[cell].contents;
			SOAInfo soaInfo = manager.soaInfo;
			contents.temperature = soaInfo.GetConduitTemperature(idx);
			ConduitDiseaseManager.Data diseaseData = soaInfo.GetDiseaseData(idx);
			contents.diseaseIdx = diseaseData.diseaseIdx;
			contents.diseaseCount = diseaseData.diseaseCount;
			return contents;
		}

		public void SetContents(ConduitFlow manager, ConduitContents contents)
		{
			int cell = manager.soaInfo.GetCell(idx);
			manager.grid[cell].contents = contents;
			SOAInfo soaInfo = manager.soaInfo;
			soaInfo.SetConduitTemperatureData(idx, ref contents);
			soaInfo.ForcePermanentDiseaseContainer(idx, contents.diseaseIdx != 255);
			soaInfo.SetDiseaseData(idx, ref contents);
		}

		public ConduitFlowInfo GetLastFlowInfo(ConduitFlow manager)
		{
			return manager.soaInfo.GetLastFlowInfo(idx);
		}

		public ConduitContents GetInitialContents(ConduitFlow manager)
		{
			return manager.soaInfo.GetInitialContents(idx);
		}

		public int GetCell(ConduitFlow manager)
		{
			return manager.soaInfo.GetCell(idx);
		}

		public bool Equals(Conduit other)
		{
			return idx == other.idx;
		}
	}

	[DebuggerDisplay("{element} M:{mass} T:{temperature}")]
	public struct ConduitContents
	{
		public SimHashes element;

		private float initial_mass;

		public float mass_delta;

		public float temperature;

		public byte diseaseIdx;

		public int diseaseCount;

		public static readonly ConduitContents Empty = new ConduitContents
		{
			element = SimHashes.Vacuum,
			initial_mass = 0f,
			mass_delta = 0f,
			temperature = 0f,
			diseaseIdx = 255,
			diseaseCount = 0
		};

		public float mass => initial_mass + mass_delta;

		public float movable_mass => initial_mass + ((!(mass_delta < 0f)) ? (0f - mass_delta) : mass_delta);

		public ConduitContents(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
		{
			Debug.Assert(!float.IsNaN(temperature));
			this.element = element;
			initial_mass = mass;
			mass_delta = 0f;
			this.temperature = temperature;
			diseaseIdx = disease_idx;
			diseaseCount = disease_count;
		}

		public void ConsolidateMass()
		{
			initial_mass += mass_delta;
			mass_delta = 0f;
		}
	}

	[DebuggerDisplay("{network.ConduitType}:{cells.Count}")]
	private struct Network
	{
		public List<int> cells;

		public FlowUtilityNetwork network;
	}

	private struct BuildNetworkTask : IWorkItem<ConduitFlow>
	{
		[DebuggerDisplay("cell {cell}:{distance}")]
		private struct DistanceNode
		{
			public int cell;

			public int distance;
		}

		[DebuggerDisplay("vertices:{vertex_cells.Count}, edges:{edges.Count}")]
		private struct Graph
		{
			[DebuggerDisplay("{cell}:{direction}")]
			public struct Vertex : IEquatable<Vertex>
			{
				public FlowDirections direction;

				public int cell;

				public static Vertex INVALID = new Vertex
				{
					direction = FlowDirections.None,
					cell = -1
				};

				public bool is_valid => cell != -1;

				public bool Equals(Vertex rhs)
				{
					return direction == rhs.direction && cell == rhs.cell;
				}
			}

			[DebuggerDisplay("{vertices[0].cell}:{vertices[0].direction} -> {vertices[1].cell}:{vertices[1].direction}")]
			public struct Edge : IEquatable<Edge>
			{
				[DebuggerDisplay("{cell}:{direction}")]
				public struct VertexIterator
				{
					public int cell;

					public FlowDirections direction;

					private ConduitFlow conduit_flow;

					private Edge edge;

					public VertexIterator(ConduitFlow conduit_flow, Edge edge)
					{
						this.conduit_flow = conduit_flow;
						this.edge = edge;
						cell = edge.vertices[0].cell;
						direction = edge.vertices[0].direction;
					}

					public void Next()
					{
						int conduitIdx = conduit_flow.grid[cell].conduitIdx;
						Conduit conduitFromDirection = conduit_flow.soaInfo.GetConduitFromDirection(conduitIdx, direction);
						Debug.Assert(conduitFromDirection.idx != -1);
						cell = conduitFromDirection.GetCell(conduit_flow);
						if (cell != edge.vertices[1].cell)
						{
							direction = Opposite(direction);
							bool flag = false;
							for (int i = 0; i != 3; i++)
							{
								direction = ComputeNextFlowDirection(direction);
								Conduit conduitFromDirection2 = conduit_flow.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, direction);
								if (conduitFromDirection2.idx != -1)
								{
									flag = true;
									break;
								}
							}
							Debug.Assert(flag);
							if (!flag)
							{
								cell = edge.vertices[1].cell;
							}
						}
					}

					public bool IsValid()
					{
						return cell != edge.vertices[1].cell;
					}
				}

				public Vertex[] vertices;

				public static readonly Edge INVALID = new Edge
				{
					vertices = null
				};

				public bool is_valid => vertices != null;

				public bool Equals(Edge rhs)
				{
					if (vertices != null)
					{
						if (rhs.vertices != null)
						{
							return vertices.Length == rhs.vertices.Length && vertices.Length == 2 && vertices[0].Equals(rhs.vertices[0]) && vertices[1].Equals(rhs.vertices[1]);
						}
						return false;
					}
					return rhs.vertices == null;
				}

				public Edge Invert()
				{
					Edge result = default(Edge);
					result.vertices = new Vertex[2]
					{
						new Vertex
						{
							cell = vertices[1].cell,
							direction = Opposite(vertices[1].direction)
						},
						new Vertex
						{
							cell = vertices[0].cell,
							direction = Opposite(vertices[0].direction)
						}
					};
					return result;
				}

				public VertexIterator Iter(ConduitFlow conduit_flow)
				{
					return new VertexIterator(conduit_flow, this);
				}
			}

			[DebuggerDisplay("cell:{cell}, parent:{parent == null ? -1 : parent.cell}")]
			private class DFSNode
			{
				public int cell;

				public DFSNode parent;
			}

			private ConduitFlow conduit_flow;

			private HashSetPool<int, ConduitFlow>.PooledHashSet vertex_cells;

			private ListPool<Edge, ConduitFlow>.PooledList edges;

			private ListPool<Edge, ConduitFlow>.PooledList cycles;

			private QueuePool<Vertex, ConduitFlow>.PooledQueue bfs_traversal;

			private HashSetPool<int, ConduitFlow>.PooledHashSet visited;

			private ListPool<Vertex, ConduitFlow>.PooledList pseudo_sources;

			public HashSetPool<int, ConduitFlow>.PooledHashSet sources;

			private HashSetPool<int, ConduitFlow>.PooledHashSet sinks;

			private HashSetPool<DFSNode, ConduitFlow>.PooledHashSet dfs_path;

			private ListPool<DFSNode, ConduitFlow>.PooledList dfs_traversal;

			public HashSetPool<int, ConduitFlow>.PooledHashSet dead_ends;

			private ListPool<Vertex, ConduitFlow>.PooledList cycle_vertices;

			public Graph(FlowUtilityNetwork network)
			{
				conduit_flow = null;
				vertex_cells = HashSetPool<int, ConduitFlow>.Allocate();
				edges = ListPool<Edge, ConduitFlow>.Allocate();
				cycles = ListPool<Edge, ConduitFlow>.Allocate();
				bfs_traversal = QueuePool<Vertex, ConduitFlow>.Allocate();
				visited = HashSetPool<int, ConduitFlow>.Allocate();
				pseudo_sources = ListPool<Vertex, ConduitFlow>.Allocate();
				sources = HashSetPool<int, ConduitFlow>.Allocate();
				sinks = HashSetPool<int, ConduitFlow>.Allocate();
				dfs_path = HashSetPool<DFSNode, ConduitFlow>.Allocate();
				dfs_traversal = ListPool<DFSNode, ConduitFlow>.Allocate();
				dead_ends = HashSetPool<int, ConduitFlow>.Allocate();
				cycle_vertices = ListPool<Vertex, ConduitFlow>.Allocate();
			}

			public void Recycle()
			{
				vertex_cells.Recycle();
				edges.Recycle();
				cycles.Recycle();
				bfs_traversal.Recycle();
				visited.Recycle();
				pseudo_sources.Recycle();
				sources.Recycle();
				sinks.Recycle();
				dfs_path.Recycle();
				dfs_traversal.Recycle();
				dead_ends.Recycle();
				cycle_vertices.Recycle();
			}

			public void Build(ConduitFlow conduit_flow, List<FlowUtilityNetwork.IItem> sources, List<FlowUtilityNetwork.IItem> sinks)
			{
				this.conduit_flow = conduit_flow;
				this.sources.Clear();
				for (int i = 0; i < sources.Count; i++)
				{
					int cell = sources[i].Cell;
					if (conduit_flow.grid[cell].conduitIdx != -1)
					{
						this.sources.Add(cell);
					}
				}
				this.sinks.Clear();
				for (int j = 0; j < sinks.Count; j++)
				{
					int cell2 = sinks[j].Cell;
					if (conduit_flow.grid[cell2].conduitIdx != -1)
					{
						this.sinks.Add(cell2);
					}
				}
				Debug.Assert(bfs_traversal.Count == 0);
				visited.Clear();
				foreach (int source in this.sources)
				{
					bfs_traversal.Enqueue(new Vertex
					{
						cell = source,
						direction = FlowDirections.None
					});
					visited.Add(source);
				}
				pseudo_sources.Clear();
				dead_ends.Clear();
				cycles.Clear();
				while (bfs_traversal.Count != 0)
				{
					Vertex node = bfs_traversal.Dequeue();
					vertex_cells.Add(node.cell);
					FlowDirections flowDirections = FlowDirections.None;
					int num = 4;
					if (node.direction != 0)
					{
						flowDirections = Opposite(node.direction);
						num = 3;
					}
					int conduitIdx = conduit_flow.grid[node.cell].conduitIdx;
					for (int k = 0; k != num; k++)
					{
						flowDirections = ComputeNextFlowDirection(flowDirections);
						Conduit conduitFromDirection = conduit_flow.soaInfo.GetConduitFromDirection(conduitIdx, flowDirections);
						Vertex new_node = WalkPath(conduitIdx, conduitFromDirection.idx, flowDirections);
						if (new_node.is_valid)
						{
							Edge edge2 = default(Edge);
							edge2.vertices = new Vertex[2]
							{
								new Vertex
								{
									cell = node.cell,
									direction = flowDirections
								},
								new_node
							};
							Edge item = edge2;
							if (new_node.cell == node.cell)
							{
								cycles.Add(item);
							}
							else if (!edges.Any((Edge edge) => edge.vertices[0].cell == new_node.cell && edge.vertices[1].cell == node.cell) && !edges.Contains(item))
							{
								edges.Add(item);
								if (visited.Add(new_node.cell))
								{
									if (IsSink(new_node.cell))
									{
										pseudo_sources.Add(new_node);
									}
									else
									{
										bfs_traversal.Enqueue(new_node);
									}
								}
							}
						}
					}
					if (bfs_traversal.Count == 0)
					{
						foreach (Vertex pseudo_source in pseudo_sources)
						{
							bfs_traversal.Enqueue(pseudo_source);
						}
						pseudo_sources.Clear();
					}
				}
			}

			private bool IsEndpoint(int cell)
			{
				Debug.Assert(cell != -1);
				GridNode gridNode = conduit_flow.grid[cell];
				return gridNode.conduitIdx == -1 || sources.Contains(cell) || sinks.Contains(cell) || dead_ends.Contains(cell);
			}

			private bool IsSink(int cell)
			{
				return sinks.Contains(cell);
			}

			private bool IsJunction(int cell)
			{
				Debug.Assert(cell != -1);
				GridNode gridNode = conduit_flow.grid[cell];
				Debug.Assert(gridNode.conduitIdx != -1);
				ConduitConnections conduitConnections = conduit_flow.soaInfo.GetConduitConnections(gridNode.conduitIdx);
				return 2 < JunctionValue(conduitConnections.down) + JunctionValue(conduitConnections.left) + JunctionValue(conduitConnections.up) + JunctionValue(conduitConnections.right);
			}

			private int JunctionValue(int conduit)
			{
				return (conduit != -1) ? 1 : 0;
			}

			private Vertex WalkPath(int root_conduit, int conduit, FlowDirections direction)
			{
				if (conduit != -1)
				{
					int cell;
					bool flag;
					do
					{
						cell = conduit_flow.soaInfo.GetCell(conduit);
						if (IsEndpoint(cell) || IsJunction(cell))
						{
							Vertex result = default(Vertex);
							result.cell = cell;
							result.direction = direction;
							return result;
						}
						direction = Opposite(direction);
						flag = true;
						for (int i = 0; i != 3; i++)
						{
							direction = ComputeNextFlowDirection(direction);
							Conduit conduitFromDirection = conduit_flow.soaInfo.GetConduitFromDirection(conduit, direction);
							if (conduitFromDirection.idx != -1)
							{
								conduit = conduitFromDirection.idx;
								flag = false;
								break;
							}
						}
					}
					while (!flag);
					pseudo_sources.Add(new Vertex
					{
						cell = cell,
						direction = ComputeNextFlowDirection(direction)
					});
					dead_ends.Add(cell);
					return Vertex.INVALID;
				}
				return Vertex.INVALID;
			}

			public void Merge(Graph inverted_graph)
			{
				foreach (Edge edge4 in inverted_graph.edges)
				{
					Edge candidate = edge4.Invert();
					if (!edges.Any((Edge edge) => edge.Equals(edge4) || edge.Equals(candidate)))
					{
						edges.Add(candidate);
						vertex_cells.Add(candidate.vertices[0].cell);
						vertex_cells.Add(candidate.vertices[1].cell);
					}
				}
				int num = 1000;
				for (int i = 0; i != num; i++)
				{
					Debug.Assert(i != num - 1);
					bool flag = false;
					foreach (int vertex_cell in vertex_cells)
					{
						if (!IsSink(vertex_cell) && !edges.Any((Edge edge) => edge.vertices[0].cell == vertex_cell))
						{
							int num2 = inverted_graph.edges.FindIndex((Edge inverted_edge) => inverted_edge.vertices[1].cell == vertex_cell);
							if (num2 != -1)
							{
								Edge edge2 = inverted_graph.edges[num2];
								for (int j = 0; j != edges.Count; j++)
								{
									Edge edge3 = edges[j];
									if (edge3.vertices[0].cell == edge2.vertices[0].cell && edge3.vertices[1].cell == edge2.vertices[1].cell)
									{
										edges[j] = edge3.Invert();
									}
								}
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}

			public void BreakCycles()
			{
				visited.Clear();
				foreach (int vertex_cell in vertex_cells)
				{
					if (!visited.Contains(vertex_cell))
					{
						dfs_path.Clear();
						dfs_traversal.Clear();
						dfs_traversal.Add(new DFSNode
						{
							cell = vertex_cell,
							parent = null
						});
						while (dfs_traversal.Count != 0)
						{
							DFSNode dFSNode = dfs_traversal[dfs_traversal.Count - 1];
							dfs_traversal.RemoveAt(dfs_traversal.Count - 1);
							bool flag = false;
							for (DFSNode parent = dFSNode.parent; parent != null; parent = parent.parent)
							{
								if (parent.cell == dFSNode.cell)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								for (int num = edges.Count - 1; num != -1; num--)
								{
									Edge item = edges[num];
									if (item.vertices[0].cell == dFSNode.parent.cell && item.vertices[1].cell == dFSNode.cell)
									{
										cycles.Add(item);
										edges.RemoveAt(num);
									}
								}
							}
							else if (visited.Add(dFSNode.cell))
							{
								foreach (Edge edge in edges)
								{
									Edge current2 = edge;
									if (current2.vertices[0].cell == dFSNode.cell)
									{
										dfs_traversal.Add(new DFSNode
										{
											cell = current2.vertices[1].cell,
											parent = dFSNode
										});
									}
								}
							}
						}
					}
				}
			}

			public void WriteFlow(bool cycles_only = false)
			{
				if (!cycles_only)
				{
					foreach (Edge edge in edges)
					{
						Edge.VertexIterator vertexIterator = edge.Iter(conduit_flow);
						while (vertexIterator.IsValid())
						{
							conduit_flow.soaInfo.AddPermittedFlowDirections(conduit_flow.grid[vertexIterator.cell].conduitIdx, vertexIterator.direction);
							vertexIterator.Next();
						}
					}
				}
				foreach (Edge cycle in cycles)
				{
					Edge current = cycle;
					cycle_vertices.Clear();
					Edge.VertexIterator vertexIterator2 = current.Iter(conduit_flow);
					vertexIterator2.Next();
					while (vertexIterator2.IsValid())
					{
						cycle_vertices.Add(new Vertex
						{
							cell = vertexIterator2.cell,
							direction = vertexIterator2.direction
						});
						vertexIterator2.Next();
					}
					int num = 0;
					int num2 = cycle_vertices.Count - 1;
					FlowDirections direction = current.vertices[0].direction;
					while (num <= num2)
					{
						Vertex vertex = cycle_vertices[num];
						conduit_flow.soaInfo.AddPermittedFlowDirections(conduit_flow.grid[vertex.cell].conduitIdx, Opposite(direction));
						direction = vertex.direction;
						num++;
						Vertex vertex2 = cycle_vertices[num2];
						conduit_flow.soaInfo.AddPermittedFlowDirections(conduit_flow.grid[vertex2.cell].conduitIdx, vertex2.direction);
						num2--;
					}
					HashSetPool<int, ConduitFlow>.PooledHashSet pooledHashSet = dead_ends;
					Vertex vertex3 = cycle_vertices[num];
					pooledHashSet.Add(vertex3.cell);
					HashSetPool<int, ConduitFlow>.PooledHashSet pooledHashSet2 = dead_ends;
					Vertex vertex4 = cycle_vertices[num2];
					pooledHashSet2.Add(vertex4.cell);
				}
			}
		}

		private Network network;

		private QueuePool<DistanceNode, ConduitFlow>.PooledQueue distance_nodes;

		private DictionaryPool<int, int, ConduitFlow>.PooledDictionary distances_via_sources;

		private ListPool<KeyValuePair<int, int>, ConduitFlow>.PooledList from_sources;

		private DictionaryPool<int, int, ConduitFlow>.PooledDictionary distances_via_sinks;

		private ListPool<KeyValuePair<int, int>, ConduitFlow>.PooledList from_sinks;

		private Graph from_sources_graph;

		private Graph from_sinks_graph;

		public BuildNetworkTask(Network network, int conduit_count)
		{
			this.network = network;
			distance_nodes = QueuePool<DistanceNode, ConduitFlow>.Allocate();
			distances_via_sources = DictionaryPool<int, int, ConduitFlow>.Allocate();
			from_sources = ListPool<KeyValuePair<int, int>, ConduitFlow>.Allocate();
			distances_via_sinks = DictionaryPool<int, int, ConduitFlow>.Allocate();
			from_sinks = ListPool<KeyValuePair<int, int>, ConduitFlow>.Allocate();
			from_sources_graph = new Graph(network.network);
			from_sinks_graph = new Graph(network.network);
		}

		public void Finish()
		{
			distances_via_sinks.Recycle();
			distances_via_sources.Recycle();
			distance_nodes.Recycle();
			from_sources.Recycle();
			from_sinks.Recycle();
			from_sources_graph.Recycle();
			from_sinks_graph.Recycle();
		}

		private void ComputeFlow(ConduitFlow outer)
		{
			from_sources_graph.Build(outer, network.network.sources, network.network.sinks);
			from_sinks_graph.Build(outer, network.network.sinks, network.network.sources);
			from_sources_graph.Merge(from_sinks_graph);
			from_sources_graph.BreakCycles();
			from_sources_graph.WriteFlow(false);
			from_sinks_graph.WriteFlow(true);
		}

		private void ComputeOrder(ConduitFlow outer)
		{
			foreach (int source in from_sources_graph.sources)
			{
				distance_nodes.Enqueue(new DistanceNode
				{
					cell = source,
					distance = 0
				});
			}
			foreach (int dead_end in from_sources_graph.dead_ends)
			{
				distance_nodes.Enqueue(new DistanceNode
				{
					cell = dead_end,
					distance = 0
				});
			}
			while (distance_nodes.Count != 0)
			{
				DistanceNode distanceNode = distance_nodes.Dequeue();
				int conduitIdx = outer.grid[distanceNode.cell].conduitIdx;
				if (conduitIdx != -1)
				{
					distances_via_sources[distanceNode.cell] = distanceNode.distance;
					ConduitConnections conduitConnections = outer.soaInfo.GetConduitConnections(conduitIdx);
					FlowDirections permittedFlowDirections = outer.soaInfo.GetPermittedFlowDirections(conduitIdx);
					if ((permittedFlowDirections & FlowDirections.Up) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.up),
							distance = distanceNode.distance + 1
						});
					}
					if ((permittedFlowDirections & FlowDirections.Down) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.down),
							distance = distanceNode.distance + 1
						});
					}
					if ((permittedFlowDirections & FlowDirections.Left) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.left),
							distance = distanceNode.distance + 1
						});
					}
					if ((permittedFlowDirections & FlowDirections.Right) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections.right),
							distance = distanceNode.distance + 1
						});
					}
				}
			}
			from_sources.AddRange(distances_via_sources);
			from_sources.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => b.Value - a.Value);
			distance_nodes.Clear();
			foreach (int source2 in from_sinks_graph.sources)
			{
				distance_nodes.Enqueue(new DistanceNode
				{
					cell = source2,
					distance = 0
				});
			}
			foreach (int dead_end2 in from_sinks_graph.dead_ends)
			{
				distance_nodes.Enqueue(new DistanceNode
				{
					cell = dead_end2,
					distance = 0
				});
			}
			while (distance_nodes.Count != 0)
			{
				DistanceNode distanceNode2 = distance_nodes.Dequeue();
				int conduitIdx2 = outer.grid[distanceNode2.cell].conduitIdx;
				if (conduitIdx2 != -1)
				{
					if (!distances_via_sources.ContainsKey(distanceNode2.cell))
					{
						distances_via_sinks[distanceNode2.cell] = distanceNode2.distance;
					}
					ConduitConnections conduitConnections2 = outer.soaInfo.GetConduitConnections(conduitIdx2);
					if (conduitConnections2.up != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.up) & FlowDirections.Down) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.up),
							distance = distanceNode2.distance + 1
						});
					}
					if (conduitConnections2.down != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.down) & FlowDirections.Up) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.down),
							distance = distanceNode2.distance + 1
						});
					}
					if (conduitConnections2.left != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.left) & FlowDirections.Right) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.left),
							distance = distanceNode2.distance + 1
						});
					}
					if (conduitConnections2.right != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections2.right) & FlowDirections.Left) != 0)
					{
						distance_nodes.Enqueue(new DistanceNode
						{
							cell = outer.soaInfo.GetCell(conduitConnections2.right),
							distance = distanceNode2.distance + 1
						});
					}
				}
			}
			from_sinks.AddRange(distances_via_sinks);
			from_sinks.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => a.Value - b.Value);
			network.cells.Capacity = Mathf.Max(network.cells.Capacity, from_sources.Count + from_sinks.Count);
			foreach (KeyValuePair<int, int> from_source in from_sources)
			{
				network.cells.Add(from_source.Key);
			}
			foreach (KeyValuePair<int, int> from_sink in from_sinks)
			{
				network.cells.Add(from_sink.Key);
			}
		}

		public void Run(ConduitFlow outer)
		{
			ComputeFlow(outer);
			ComputeOrder(outer);
		}
	}

	private struct ConnectContext
	{
		public ListPool<int, ConduitFlow>.PooledList cells;

		public ConduitFlow outer;

		public ConnectContext(ConduitFlow outer)
		{
			this.outer = outer;
			cells = ListPool<int, ConduitFlow>.Allocate();
			cells.Capacity = Mathf.Max(cells.Capacity, outer.soaInfo.NumEntries);
		}

		public void Finish()
		{
			cells.Recycle();
		}
	}

	private struct ConnectTask : IWorkItem<ConnectContext>
	{
		private int start;

		private int end;

		public ConnectTask(int start, int end)
		{
			this.start = start;
			this.end = end;
		}

		public void Run(ConnectContext context)
		{
			for (int i = start; i != end; i++)
			{
				int num = context.cells[i];
				int conduitIdx = context.outer.grid[num].conduitIdx;
				if (conduitIdx != -1)
				{
					UtilityConnections connections = context.outer.networkMgr.GetConnections(num, true);
					if (connections != 0)
					{
						ConduitConnections dEFAULT = ConduitConnections.DEFAULT;
						int num2 = num - 1;
						if (Grid.IsValidCell(num2) && (connections & UtilityConnections.Left) != 0)
						{
							dEFAULT.left = context.outer.grid[num2].conduitIdx;
						}
						num2 = num + 1;
						if (Grid.IsValidCell(num2) && (connections & UtilityConnections.Right) != 0)
						{
							dEFAULT.right = context.outer.grid[num2].conduitIdx;
						}
						num2 = num - Grid.WidthInCells;
						if (Grid.IsValidCell(num2) && (connections & UtilityConnections.Down) != 0)
						{
							dEFAULT.down = context.outer.grid[num2].conduitIdx;
						}
						num2 = num + Grid.WidthInCells;
						if (Grid.IsValidCell(num2) && (connections & UtilityConnections.Up) != 0)
						{
							dEFAULT.up = context.outer.grid[num2].conduitIdx;
						}
						context.outer.soaInfo.SetConduitConnections(conduitIdx, dEFAULT);
					}
				}
			}
		}
	}

	private struct Sink
	{
		public ConduitConsumer consumer;

		public float space_remaining;

		public Sink(FlowUtilityNetwork.IItem sink)
		{
			consumer = ((!((UnityEngine.Object)sink.GameObject != (UnityEngine.Object)null)) ? null : sink.GameObject.GetComponent<ConduitConsumer>());
			space_remaining = ((!((UnityEngine.Object)consumer != (UnityEngine.Object)null) || !consumer.operational.IsOperational) ? 0f : consumer.space_remaining_kg);
		}
	}

	private struct UpdateNetworkTask : IWorkItem<ConduitFlow>
	{
		private Network network;

		private DictionaryPool<int, Sink, ConduitFlow>.PooledDictionary sinks;

		public UpdateNetworkTask(Network network)
		{
			this.network = network;
			sinks = DictionaryPool<int, Sink, ConduitFlow>.Allocate();
			foreach (FlowUtilityNetwork.IItem sink in network.network.sinks)
			{
				sinks.Add(sink.Cell, new Sink(sink));
			}
		}

		public void Run(ConduitFlow conduit_flow)
		{
			foreach (int cell in network.cells)
			{
				conduit_flow.UpdateConduit(conduit_flow.soaInfo.GetConduit(conduit_flow.grid[cell].conduitIdx), sinks);
			}
			foreach (int cell2 in network.cells)
			{
				conduit_flow.grid[cell2].contents.ConsolidateMass();
			}
		}

		public void Finish()
		{
			sinks.Recycle();
		}
	}

	private ConduitType conduitType;

	private float MaxMass = 10f;

	private const float PERCENT_MAX_MASS_FOR_STATE_CHANGE_DAMAGE = 0.1f;

	public const float TickRate = 1f;

	public const float WaitTime = 1f;

	private float elapsedTime = 0f;

	private float lastUpdateTime = float.NegativeInfinity;

	public SOAInfo soaInfo = new SOAInfo();

	private bool dirtyConduitUpdaters = false;

	private List<ConduitUpdater> conduitUpdaters = new List<ConduitUpdater>();

	private GridNode[] grid;

	[Serialize]
	public int[] serializedIdx;

	[Serialize]
	public ConduitContents[] serializedContents;

	[Serialize]
	public SerializedContents[] versionedSerializedContents;

	private IUtilityNetworkMgr networkMgr;

	private HashSet<int> replacements = new HashSet<int>();

	private const int FLOW_DIRECTION_COUNT = 4;

	private List<Network> networks = new List<Network>();

	private WorkItemCollection<BuildNetworkTask, ConduitFlow> build_network_job = new WorkItemCollection<BuildNetworkTask, ConduitFlow>();

	private WorkItemCollection<ConnectTask, ConnectContext> connect_job = new WorkItemCollection<ConnectTask, ConnectContext>();

	private WorkItemCollection<UpdateNetworkTask, ConduitFlow> update_networks_job = new WorkItemCollection<UpdateNetworkTask, ConduitFlow>();

	public float ContinuousLerpPercent => Mathf.Clamp01((Time.time - lastUpdateTime) / 1f);

	public float DiscreteLerpPercent => Mathf.Clamp01(elapsedTime / 1f);

	public event System.Action onConduitsRebuilt;

	public ConduitFlow(ConduitType conduit_type, int num_cells, IUtilityNetworkMgr network_mgr, float max_conduit_mass, float initial_elapsed_time)
	{
		elapsedTime = initial_elapsed_time;
		conduitType = conduit_type;
		networkMgr = network_mgr;
		MaxMass = max_conduit_mass;
		Initialize(num_cells);
		network_mgr.AddNetworksRebuiltListener(OnUtilityNetworksRebuilt);
	}

	public void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default)
	{
		conduitUpdaters.Add(new ConduitUpdater
		{
			priority = priority,
			callback = callback
		});
		dirtyConduitUpdaters = true;
	}

	public void RemoveConduitUpdater(Action<float> callback)
	{
		int num = 0;
		while (true)
		{
			if (num >= conduitUpdaters.Count)
			{
				return;
			}
			ConduitUpdater conduitUpdater = conduitUpdaters[num];
			if (conduitUpdater.callback == callback)
			{
				break;
			}
			num++;
		}
		conduitUpdaters.RemoveAt(num);
		dirtyConduitUpdaters = true;
	}

	private static FlowDirections ComputeFlowDirection(int index)
	{
		Debug.Assert(0 <= index && index < 4);
		return (FlowDirections)(1 << index);
	}

	private static int ComputeIndex(FlowDirections flow)
	{
		switch (flow)
		{
		case FlowDirections.Down:
			return 0;
		case FlowDirections.Left:
			return 1;
		case FlowDirections.Right:
			return 2;
		case FlowDirections.Up:
			return 3;
		default:
			Debug.Assert(false, "multiple bits are set in 'flow'...can't compute refuted index");
			return -1;
		}
	}

	private static FlowDirections ComputeNextFlowDirection(FlowDirections current)
	{
		return (current == FlowDirections.None) ? FlowDirections.Down : ComputeFlowDirection((ComputeIndex(current) + 1) % 4);
	}

	public static FlowDirections Invert(FlowDirections directions)
	{
		return (FlowDirections)(0xF & (byte)(~(uint)directions));
	}

	public static FlowDirections Opposite(FlowDirections directions)
	{
		FlowDirections flowDirections = FlowDirections.None;
		if ((directions & FlowDirections.Left) != 0)
		{
			flowDirections = FlowDirections.Right;
		}
		else if ((directions & FlowDirections.Right) != 0)
		{
			flowDirections = FlowDirections.Left;
		}
		else if ((directions & FlowDirections.Up) != 0)
		{
			flowDirections = FlowDirections.Down;
		}
		else if ((directions & FlowDirections.Down) != 0)
		{
			flowDirections = FlowDirections.Up;
		}
		Debug.Assert((Invert(flowDirections) & directions) == directions, "computing the Opposite of multiple directions is refutable");
		return flowDirections;
	}

	public void Initialize(int num_cells)
	{
		grid = new GridNode[num_cells];
		for (int i = 0; i < num_cells; i++)
		{
			grid[i].conduitIdx = -1;
			grid[i].contents.element = SimHashes.Vacuum;
			grid[i].contents.diseaseIdx = byte.MaxValue;
		}
	}

	private void OnUtilityNetworksRebuilt(IList<UtilityNetwork> networks, ICollection<int> root_nodes)
	{
		RebuildConnections(root_nodes);
		int count = this.networks.Count - networks.Count;
		if (0 < this.networks.Count - networks.Count)
		{
			this.networks.RemoveRange(networks.Count, count);
		}
		Debug.Assert(this.networks.Count <= networks.Count);
		for (int i = 0; i != networks.Count; i++)
		{
			if (i < this.networks.Count)
			{
				List<Network> list = this.networks;
				int index = i;
				Network value = new Network
				{
					network = (FlowUtilityNetwork)networks[i]
				};
				Network network = this.networks[i];
				value.cells = network.cells;
				list[index] = value;
				Network network2 = this.networks[i];
				network2.cells.Clear();
			}
			else
			{
				this.networks.Add(new Network
				{
					network = (FlowUtilityNetwork)networks[i],
					cells = new List<int>()
				});
			}
		}
		build_network_job.Reset(this);
		foreach (Network network3 in this.networks)
		{
			build_network_job.Add(new BuildNetworkTask(network3, soaInfo.NumEntries));
		}
		GlobalJobManager.Run(build_network_job);
		for (int j = 0; j != build_network_job.Count; j++)
		{
			build_network_job.GetWorkItem(j).Finish();
		}
	}

	private void RebuildConnections(IEnumerable<int> root_nodes)
	{
		ConnectContext shared_data = new ConnectContext(this);
		soaInfo.Clear(this);
		replacements.ExceptWith(root_nodes);
		ObjectLayer layer = (conduitType != ConduitType.Gas) ? ObjectLayer.LiquidConduit : ObjectLayer.GasConduit;
		foreach (int root_node in root_nodes)
		{
			GameObject gameObject = Grid.Objects[root_node, (int)layer];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
			{
				global::Conduit component = gameObject.GetComponent<global::Conduit>();
				if (!((UnityEngine.Object)component != (UnityEngine.Object)null) || !component.IsDisconnected())
				{
					int conduitIdx = soaInfo.AddConduit(this, gameObject, root_node);
					grid[root_node].conduitIdx = conduitIdx;
					shared_data.cells.Add(root_node);
				}
			}
		}
		Game.Instance.conduitTemperatureManager.Sim200ms(0f);
		connect_job.Reset(shared_data);
		int num = 256;
		for (int i = 0; i < shared_data.cells.Count; i += num)
		{
			connect_job.Add(new ConnectTask(i, Mathf.Min(i + num, shared_data.cells.Count)));
		}
		GlobalJobManager.Run(connect_job);
		shared_data.Finish();
		if (this.onConduitsRebuilt != null)
		{
			this.onConduitsRebuilt();
		}
	}

	private FlowDirections GetDirection(Conduit conduit, Conduit target_conduit)
	{
		Debug.Assert(conduit.idx != -1);
		Debug.Assert(target_conduit.idx != -1);
		ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduit.idx);
		if (conduitConnections.up != target_conduit.idx)
		{
			if (conduitConnections.down != target_conduit.idx)
			{
				if (conduitConnections.left != target_conduit.idx)
				{
					if (conduitConnections.right != target_conduit.idx)
					{
						return FlowDirections.None;
					}
					return FlowDirections.Right;
				}
				return FlowDirections.Left;
			}
			return FlowDirections.Down;
		}
		return FlowDirections.Up;
	}

	public int ComputeUpdateOrder(int cell)
	{
		foreach (Network network in networks)
		{
			Network current = network;
			int num = current.cells.IndexOf(cell);
			if (num != -1)
			{
				return num;
			}
		}
		return -1;
	}

	public ConduitContents GetContents(int cell)
	{
		ConduitContents contents = grid[cell].contents;
		GridNode gridNode = grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			contents = soaInfo.GetConduit(gridNode.conduitIdx).GetContents(this);
		}
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			Debug.LogError("unexpected temperature");
		}
		return contents;
	}

	public void SetContents(int cell, ConduitContents contents)
	{
		GridNode gridNode = grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			soaInfo.GetConduit(gridNode.conduitIdx).SetContents(this, contents);
		}
		else
		{
			grid[cell].contents = contents;
		}
	}

	public static int GetCellFromDirection(int cell, FlowDirections direction)
	{
		switch (direction)
		{
		case FlowDirections.Left:
			return Grid.CellLeft(cell);
		case FlowDirections.Right:
			return Grid.CellRight(cell);
		case FlowDirections.Up:
			return Grid.CellAbove(cell);
		case FlowDirections.Down:
			return Grid.CellBelow(cell);
		default:
			return -1;
		}
	}

	public void Sim200ms(float dt)
	{
		if (!(dt <= 0f))
		{
			elapsedTime += dt;
			if (!(elapsedTime < 1f))
			{
				elapsedTime -= 1f;
				float obj = 1f;
				lastUpdateTime = Time.time;
				soaInfo.BeginFrame(this);
				update_networks_job.Reset(this);
				foreach (Network network in networks)
				{
					update_networks_job.Add(new UpdateNetworkTask(network));
				}
				GlobalJobManager.Run(update_networks_job);
				for (int i = 0; i != update_networks_job.Count; i++)
				{
					update_networks_job.GetWorkItem(i).Finish();
				}
				if (dirtyConduitUpdaters)
				{
					conduitUpdaters.Sort((ConduitUpdater a, ConduitUpdater b) => a.priority - b.priority);
				}
				soaInfo.EndFrame(this);
				for (int j = 0; j < conduitUpdaters.Count; j++)
				{
					ConduitUpdater conduitUpdater = conduitUpdaters[j];
					conduitUpdater.callback(obj);
				}
			}
		}
	}

	private void UpdateConduit(Conduit conduit, Dictionary<int, Sink> sinks)
	{
		int cell = soaInfo.GetCell(conduit.idx);
		GridNode gridNode = grid[cell];
		bool flag = gridNode.contents.element == SimHashes.Vacuum;
		if (gridNode.contents.mass <= 0f)
		{
			soaInfo.MarkConduitEmpty(conduit.idx, this);
			flag = true;
		}
		FlowDirections permittedFlowDirections = soaInfo.GetPermittedFlowDirections(conduit.idx);
		FlowDirections flowDirections = soaInfo.GetTargetFlowDirection(conduit.idx);
		if (flag)
		{
			for (int i = 0; i != 4; i++)
			{
				flowDirections = ComputeNextFlowDirection(flowDirections);
				if ((permittedFlowDirections & flowDirections) != 0)
				{
					Conduit conduitFromDirection = soaInfo.GetConduitFromDirection(conduit.idx, flowDirections);
					Debug.Assert(conduitFromDirection.idx != -1);
					FlowDirections srcFlowDirection = soaInfo.GetSrcFlowDirection(conduitFromDirection.idx);
					if ((srcFlowDirection & Opposite(flowDirections)) != 0)
					{
						soaInfo.SetPullDirection(conduitFromDirection.idx, flowDirections);
					}
				}
			}
		}
		else
		{
			float num = 0f;
			float num2 = gridNode.contents.movable_mass;
			if (sinks.TryGetValue(cell, out Sink value) && (UnityEngine.Object)value.consumer != (UnityEngine.Object)null)
			{
				num2 = Mathf.Max(0f, gridNode.contents.movable_mass - value.space_remaining);
			}
			for (int j = 0; j != 4; j++)
			{
				flowDirections = ComputeNextFlowDirection(flowDirections);
				if ((permittedFlowDirections & flowDirections) != 0)
				{
					Conduit conduitFromDirection2 = soaInfo.GetConduitFromDirection(conduit.idx, flowDirections);
					Debug.Assert(conduitFromDirection2.idx != -1);
					FlowDirections srcFlowDirection2 = soaInfo.GetSrcFlowDirection(conduitFromDirection2.idx);
					bool flag2 = (srcFlowDirection2 & Opposite(flowDirections)) != FlowDirections.None;
					if (srcFlowDirection2 == FlowDirections.None || flag2)
					{
						int cell2 = soaInfo.GetCell(conduitFromDirection2.idx);
						Debug.Assert(cell2 != -1);
						ConduitContents contents = grid[cell2].contents;
						bool flag3 = contents.element == SimHashes.Vacuum || contents.element == gridNode.contents.element;
						float num3 = Mathf.Max(0f, MaxMass - grid[cell2].contents.mass);
						flag3 = (flag3 && num3 > 0f);
						num = Mathf.Min(num2, num3);
						if (flag2 && flag3)
						{
							soaInfo.SetPullDirection(conduitFromDirection2.idx, flowDirections);
						}
						if (!(num <= 0f) && flag3)
						{
							soaInfo.SetTargetFlowDirection(conduit.idx, flowDirections);
							Debug.Assert(gridNode.contents.temperature > 0f);
							contents.temperature = GameUtil.GetFinalTemperature(gridNode.contents.temperature, num, contents.temperature, contents.mass);
							contents.mass_delta += num;
							contents.element = gridNode.contents.element;
							float num4 = num / gridNode.contents.mass;
							int num5 = (int)(num4 * (float)gridNode.contents.diseaseCount);
							if (num5 != 0)
							{
								SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(gridNode.contents.diseaseIdx, num5, contents.diseaseIdx, contents.diseaseCount);
								contents.diseaseIdx = diseaseInfo.idx;
								contents.diseaseCount = diseaseInfo.count;
							}
							grid[cell2].contents = contents;
							Debug.Assert(num <= gridNode.contents.mass);
							float num6 = gridNode.contents.mass - num;
							num2 -= num;
							if (num6 <= 0f)
							{
								Debug.Assert(num2 <= 0f);
								soaInfo.SetLastFlowInfo(conduit.idx, flowDirections, ref gridNode.contents);
								gridNode.contents = ConduitContents.Empty;
							}
							else
							{
								float num7 = num6 / gridNode.contents.mass;
								int num8 = (int)(num7 * (float)gridNode.contents.diseaseCount);
								Debug.Assert(num8 >= 0);
								ConduitContents contents2 = gridNode.contents;
								contents2.mass_delta -= num6;
								contents2.diseaseCount -= num8;
								gridNode.contents.mass_delta -= num;
								gridNode.contents.diseaseCount = num8;
								if (num8 == 0)
								{
									gridNode.contents.diseaseIdx = byte.MaxValue;
								}
								soaInfo.SetLastFlowInfo(conduit.idx, flowDirections, ref contents2);
							}
							grid[cell].contents = gridNode.contents;
							break;
						}
					}
				}
			}
		}
		FlowDirections flowDirections2 = soaInfo.GetSrcFlowDirection(conduit.idx);
		FlowDirections pullDirection = soaInfo.GetPullDirection(conduit.idx);
		if (flowDirections2 == FlowDirections.None || (Opposite(flowDirections2) & pullDirection) != 0)
		{
			soaInfo.SetPullDirection(conduit.idx, FlowDirections.None);
			soaInfo.SetSrcFlowDirection(conduit.idx, FlowDirections.None);
			ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduit.idx);
			int num9 = 0;
			int num10;
			while (true)
			{
				if (num9 == 4)
				{
					return;
				}
				flowDirections2 = ComputeNextFlowDirection(flowDirections2);
				num10 = -1;
				switch (flowDirections2)
				{
				case FlowDirections.Up:
					num10 = conduitConnections.up;
					break;
				case FlowDirections.Down:
					num10 = conduitConnections.down;
					break;
				case FlowDirections.Left:
					num10 = conduitConnections.left;
					break;
				case FlowDirections.Right:
					num10 = conduitConnections.right;
					break;
				}
				if (num10 != -1)
				{
					FlowDirections permittedFlowDirections2 = soaInfo.GetPermittedFlowDirections(num10);
					if ((permittedFlowDirections2 & Opposite(flowDirections2)) != 0)
					{
						break;
					}
				}
				num9++;
			}
			int cell3 = soaInfo.GetCell(num10);
			ConduitContents contents3 = grid[cell3].contents;
			if ((gridNode.contents.element == SimHashes.Vacuum || contents3.element == gridNode.contents.element) && 0f < grid[cell3].contents.movable_mass)
			{
				soaInfo.SetSrcFlowDirection(conduit.idx, flowDirections2);
			}
		}
	}

	public float AddElement(int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (grid[cell_idx].conduitIdx != -1)
		{
			ConduitContents contents = GetConduit(cell_idx).GetContents(this);
			if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
			{
				return 0f;
			}
			float num = Mathf.Min(mass, MaxMass - contents.mass);
			float num2 = num / mass;
			if (!(num <= 0f))
			{
				contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
				contents.mass_delta += num;
				contents.element = element;
				contents.ConsolidateMass();
				int num3 = (int)(num2 * (float)disease_count);
				if (num3 > 0)
				{
					SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, num3, contents.diseaseIdx, contents.diseaseCount);
					contents.diseaseIdx = diseaseInfo.idx;
					contents.diseaseCount = diseaseInfo.count;
				}
				SetContents(cell_idx, contents);
				return num;
			}
			return 0f;
		}
		return 0f;
	}

	public ConduitContents RemoveElement(int cell, float delta)
	{
		Conduit conduit = GetConduit(cell);
		return (conduit.idx == -1) ? ConduitContents.Empty : RemoveElement(conduit, delta);
	}

	public ConduitContents RemoveElement(Conduit conduit, float delta)
	{
		ConduitContents contents = conduit.GetContents(this);
		float num = Mathf.Min(contents.mass, delta);
		float num2 = contents.mass - num;
		if (!(num2 <= 0f))
		{
			ConduitContents result = contents;
			result.mass_delta -= num2;
			float num3 = num2 / contents.mass;
			int num4 = (int)(num3 * (float)contents.diseaseCount);
			result.diseaseCount = contents.diseaseCount - num4;
			ConduitContents contents2 = contents;
			contents2.mass_delta -= num;
			contents2.diseaseCount = num4;
			if (num4 <= 0)
			{
				contents2.diseaseIdx = byte.MaxValue;
				contents2.diseaseCount = 0;
			}
			conduit.SetContents(this, contents2);
			return result;
		}
		conduit.SetContents(this, ConduitContents.Empty);
		return contents;
	}

	public FlowDirections GetPermittedFlow(int cell)
	{
		Conduit conduit = GetConduit(cell);
		if (conduit.idx != -1)
		{
			return soaInfo.GetPermittedFlowDirections(conduit.idx);
		}
		return FlowDirections.None;
	}

	public bool HasConduit(int cell)
	{
		return grid[cell].conduitIdx != -1;
	}

	public Conduit GetConduit(int cell)
	{
		int conduitIdx = grid[cell].conduitIdx;
		return (conduitIdx == -1) ? Conduit.Invalid() : soaInfo.GetConduit(conduitIdx);
	}

	private void DumpPipeContents(int cell, ConduitContents contents)
	{
		if (contents.element != SimHashes.Vacuum && contents.mass > 0f)
		{
			SimMessages.AddRemoveSubstance(cell, contents.element, CellEventLogger.Instance.ConduitFlowEmptyConduit, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount, true, -1);
			SetContents(cell, ConduitContents.Empty);
		}
	}

	public void EmptyConduit(int cell)
	{
		if (!replacements.Contains(cell))
		{
			DumpPipeContents(cell, grid[cell].contents);
		}
	}

	public void MarkForReplacement(int cell)
	{
		replacements.Add(cell);
	}

	public void DeactivateCell(int cell)
	{
		grid[cell].conduitIdx = -1;
		SetContents(cell, ConduitContents.Empty);
	}

	[Conditional("CHECK_NAN")]
	private void Validate(ConduitContents contents)
	{
		Assert.IsTrue(!float.IsNaN(contents.temperature));
		Assert.IsTrue(!float.IsPositiveInfinity(contents.temperature));
		Assert.IsTrue(!float.IsNegativeInfinity(contents.temperature));
		Assert.IsTrue(contents.mass == 0f || contents.temperature > 0f);
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			Debug.LogError("zero degree pipe contents");
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		int numEntries = soaInfo.NumEntries;
		if (numEntries > 0)
		{
			versionedSerializedContents = new SerializedContents[numEntries];
			serializedIdx = new int[numEntries];
			for (int i = 0; i < numEntries; i++)
			{
				Conduit conduit = soaInfo.GetConduit(i);
				ConduitContents contents = conduit.GetContents(this);
				serializedIdx[i] = soaInfo.GetCell(conduit.idx);
				versionedSerializedContents[i] = new SerializedContents(contents);
			}
		}
		else
		{
			serializedContents = null;
			versionedSerializedContents = null;
			serializedIdx = null;
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		versionedSerializedContents = null;
		serializedContents = null;
		serializedIdx = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.serializedContents != null)
		{
			versionedSerializedContents = new SerializedContents[this.serializedContents.Length];
			for (int i = 0; i < this.serializedContents.Length; i++)
			{
				versionedSerializedContents[i] = new SerializedContents(this.serializedContents[i]);
			}
			this.serializedContents = null;
		}
		if (versionedSerializedContents != null)
		{
			for (int j = 0; j < versionedSerializedContents.Length; j++)
			{
				int num = serializedIdx[j];
				SerializedContents serializedContents = versionedSerializedContents[j];
				ConduitContents contents = (!(serializedContents.mass <= 0f)) ? new ConduitContents(serializedContents.element, Math.Min(MaxMass, serializedContents.mass), serializedContents.temperature, byte.MaxValue, 0) : ConduitContents.Empty;
				if (0 < serializedContents.diseaseCount || serializedContents.diseaseHash != 0)
				{
					contents.diseaseIdx = Db.Get().Diseases.GetIndex(serializedContents.diseaseHash);
					contents.diseaseCount = ((contents.diseaseIdx != 255) ? serializedContents.diseaseCount : 0);
				}
				if (float.IsNaN(contents.temperature) || (contents.temperature <= 0f && contents.element != SimHashes.Vacuum) || 10000f < contents.temperature)
				{
					Vector2I vector2I = Grid.CellToXY(num);
					DeserializeWarnings.Instance.PipeContentsTemperatureIsNan.Warn($"Invalid pipe content temperature of {contents.temperature} detected. Resetting temperature. (x={vector2I.x}, y={vector2I.y}, cell={num})", null);
					contents.temperature = ElementLoader.FindElementByHash(contents.element).defaultValues.temperature;
				}
				SetContents(num, contents);
			}
			versionedSerializedContents = null;
			this.serializedContents = null;
			serializedIdx = null;
		}
	}

	public UtilityNetwork GetNetwork(Conduit conduit)
	{
		int cell = soaInfo.GetCell(conduit.idx);
		return networkMgr.GetNetworkForCell(cell);
	}

	public void ForceRebuildNetworks()
	{
		networkMgr.ForceRebuildNetworks();
	}

	public bool IsConduitFull(int cell_idx)
	{
		ConduitContents contents = grid[cell_idx].contents;
		return MaxMass - contents.mass <= 0f;
	}

	public bool IsConduitEmpty(int cell_idx)
	{
		ConduitContents contents = grid[cell_idx].contents;
		return contents.mass <= 0f;
	}

	public void FreezeConduitContents(int conduit_idx)
	{
		GameObject conduitGO = soaInfo.GetConduitGO(conduit_idx);
		if ((UnityEngine.Object)conduitGO != (UnityEngine.Object)null && soaInfo.GetConduit(conduit_idx).GetContents(this).mass > MaxMass * 0.1f)
		{
			conduitGO.Trigger(-700727624, null);
		}
	}

	public void MeltConduitContents(int conduit_idx)
	{
		GameObject conduitGO = soaInfo.GetConduitGO(conduit_idx);
		if ((UnityEngine.Object)conduitGO != (UnityEngine.Object)null && soaInfo.GetConduit(conduit_idx).GetContents(this).mass > MaxMass * 0.1f)
		{
			conduitGO.Trigger(-1152799878, null);
		}
	}
}
