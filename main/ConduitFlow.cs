using Klei;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitFlow : IConduitFlow
{
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

		private class SetUpdatedFalse : ConduitTask
		{
			public SetUpdatedFalse()
				: base("SetUpdatedFalse")
			{
			}

			protected override void RunDivision(SOAInfo soaInfo)
			{
				for (int i = start; i != end; i++)
				{
					soaInfo.updated[i] = false;
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
					soaInfo.lastFlowInfo[i] = ConduitFlowInfo.Invalid;
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
					if (!soaInfo.updated[i])
					{
						Conduit conduit = soaInfo.conduits[i];
						int cell = conduit.GetCell(manager);
						if (manager.grid[cell].contents.element == SimHashes.Vacuum)
						{
							soaInfo.srcFlowDirections[conduit.idx] = conduit.GetNextFlowSource(manager);
						}
					}
				}
			}
		}

		private List<Conduit> conduits = new List<Conduit>();

		private List<ConduitConnections> conduitConnections = new List<ConduitConnections>();

		private List<ConduitFlowInfo> lastFlowInfo = new List<ConduitFlowInfo>();

		private List<ConduitContents> initialContents = new List<ConduitContents>();

		private List<HandleVector<int>.Handle> structureTemperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> temperatureHandles = new List<HandleVector<int>.Handle>();

		private List<HandleVector<int>.Handle> diseaseHandles = new List<HandleVector<int>.Handle>();

		private List<GameObject> conduitGOs = new List<GameObject>();

		private List<bool> diseaseContentsVisible = new List<bool>();

		private List<bool> updated = new List<bool>();

		private List<int> cells = new List<int>();

		private List<int> permittedFlowDirections = new List<int>();

		private List<int> srcFlowIdx = new List<int>();

		private List<FlowDirection> srcFlowDirections = new List<FlowDirection>();

		private List<FlowDirection> targetFlowDirections = new List<FlowDirection>();

		private ConduitTaskDivision<ClearPermanentDiseaseContainer> clearPermanentDiseaseContainer = new ConduitTaskDivision<ClearPermanentDiseaseContainer>();

		private ConduitTaskDivision<PublishTemperatureToSim> publishTemperatureToSim = new ConduitTaskDivision<PublishTemperatureToSim>();

		private ConduitTaskDivision<PublishDiseaseToSim> publishDiseaseToSim = new ConduitTaskDivision<PublishDiseaseToSim>();

		private ConduitTaskDivision<ResetConduit> resetConduit = new ConduitTaskDivision<ResetConduit>();

		private ConduitJob clearJob = new ConduitJob();

		private ConduitTaskDivision<SetUpdatedFalse> setUpdatedFalse = new ConduitTaskDivision<SetUpdatedFalse>();

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
			lastFlowInfo.Add(ConduitFlowInfo.Invalid);
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(conduit_go);
			HandleVector<int>.Handle handle2 = Game.Instance.conduitTemperatureManager.Allocate(manager.conduitType, count, handle, ref contents);
			HandleVector<int>.Handle item2 = Game.Instance.conduitDiseaseManager.Allocate(handle2, ref contents);
			cells.Add(cell);
			updated.Add(false);
			diseaseContentsVisible.Add(false);
			structureTemperatureHandles.Add(handle);
			temperatureHandles.Add(handle2);
			diseaseHandles.Add(item2);
			conduitGOs.Add(conduit_go);
			srcFlowIdx.Add(-1);
			permittedFlowDirections.Add(0);
			srcFlowDirections.Add(FlowDirection.None);
			targetFlowDirections.Add(FlowDirection.None);
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
			updated.Clear();
			diseaseContentsVisible.Clear();
			srcFlowIdx.Clear();
			permittedFlowDirections.Clear();
			srcFlowDirections.Clear();
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
			return Game.Instance.conduitTemperatureManager.GetTemperature(handle);
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

		public Conduit GetConduitFromDirection(int idx, FlowDirection direction)
		{
			Conduit result = Conduit.Invalid();
			ConduitConnections conduitConnections = this.conduitConnections[idx];
			switch (direction)
			{
			case FlowDirection.Left:
				result = ((conduitConnections.left == -1) ? Conduit.Invalid() : conduits[conduitConnections.left]);
				break;
			case FlowDirection.Right:
				result = ((conduitConnections.right == -1) ? Conduit.Invalid() : conduits[conduitConnections.right]);
				break;
			case FlowDirection.Up:
				result = ((conduitConnections.up == -1) ? Conduit.Invalid() : conduits[conduitConnections.up]);
				break;
			case FlowDirection.Down:
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
				beginFrameJob.Add(setUpdatedFalse);
				beginFrameJob.Add(setInitialContents);
				beginFrameJob.Add(invalidateLastFlow);
			}
			setUpdatedFalse.Initialize(conduits.Count, manager);
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
				lastFlowInfo[idx] = ConduitFlowInfo.Invalid;
				Conduit conduit = conduits[idx];
				targetFlowDirections[idx] = conduit.GetNextFlowTarget(manager);
				int num = cells[idx];
				manager.grid[num].contents = ConduitContents.Empty;
			}
		}

		public void ResetLastFlowInfo(int idx)
		{
			lastFlowInfo[idx] = ConduitFlowInfo.Invalid;
		}

		public void SetLastFlowInfo(int idx, FlowDirection direction, ref ConduitContents contents)
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

		public int GetPermittedFlowDirections(int idx)
		{
			return permittedFlowDirections[idx];
		}

		public void SetPermittedFlowDirections(int idx, int permitted)
		{
			permittedFlowDirections[idx] = permitted;
		}

		public FlowDirection GetTargetFlowDirection(int idx)
		{
			return targetFlowDirections[idx];
		}

		public void SetTargetFlowDirection(int idx, FlowDirection directions)
		{
			targetFlowDirections[idx] = directions;
		}

		public int GetSrcFlowIdx(int idx)
		{
			return srcFlowIdx[idx];
		}

		public void SetSrcFlowIdx(int idx, int new_src_idx)
		{
			srcFlowIdx[idx] = new_src_idx;
		}

		public FlowDirection GetSrcFlowDirection(int idx)
		{
			return srcFlowDirections[idx];
		}

		public void SetSrcFlowDirection(int idx, FlowDirection directions)
		{
			srcFlowDirections[idx] = directions;
		}

		public int GetCell(int idx)
		{
			return cells[idx];
		}

		public void SetCell(int idx, int cell)
		{
			cells[idx] = cell;
		}

		public bool GetUpdated(int idx)
		{
			return updated[idx];
		}

		public void SetUpdated(int idx, bool is_updated)
		{
			updated[idx] = is_updated;
		}
	}

	[DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
	public struct ConduitUpdater
	{
		public ConduitFlowPriority priority;

		public Action<float> callback;
	}

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

	public enum FlowDirection
	{
		Blocked = -1,
		None,
		Left,
		Right,
		Up,
		Down,
		Num
	}

	public struct ConduitConnections
	{
		public int left;

		public int right;

		public int up;

		public int down;
	}

	public struct ConduitFlowInfo
	{
		public FlowDirection direction;

		public ConduitContents contents;

		public static readonly ConduitFlowInfo Invalid = new ConduitFlowInfo
		{
			direction = FlowDirection.None,
			contents = ConduitContents.Empty
		};
	}

	[Serializable]
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

		public int GetPermittedFlowDirections(ConduitFlow manager)
		{
			return manager.soaInfo.GetPermittedFlowDirections(idx);
		}

		public void SetPermittedFlowDirections(int permitted, ConduitFlow manager)
		{
			manager.soaInfo.SetPermittedFlowDirections(idx, permitted);
		}

		public FlowDirection GetTargetFlowDirection(ConduitFlow manager)
		{
			return manager.soaInfo.GetTargetFlowDirection(idx);
		}

		public void SetTargetFlowDirection(FlowDirection directions, ConduitFlow manager)
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

		public FlowDirection GetNextFlowSource(ConduitFlow manager)
		{
			int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(idx);
			if (permittedFlowDirections == -1)
			{
				return FlowDirection.Blocked;
			}
			FlowDirection flowDirection = manager.soaInfo.GetSrcFlowDirection(idx);
			if (flowDirection == FlowDirection.None)
			{
				flowDirection = FlowDirection.Down;
			}
			for (int i = 0; i < 5; i++)
			{
				int num = (int)(flowDirection + i - 1);
				int num2 = (num + 1) % 5;
				FlowDirection flowDirection2 = (FlowDirection)(num2 + 1);
				Conduit conduitFromDirection = manager.soaInfo.GetConduitFromDirection(idx, flowDirection2);
				if (conduitFromDirection.idx != -1)
				{
					ConduitContents contents = manager.grid[conduitFromDirection.GetCell(manager)].contents;
					if (contents.element != SimHashes.Vacuum)
					{
						int permittedFlowDirections2 = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection.idx);
						if (permittedFlowDirections2 != -1)
						{
							FlowDirection direction = InverseFlow(flowDirection2);
							Conduit conduitFromDirection2 = manager.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, direction);
							if (conduitFromDirection2.idx != -1 && (permittedFlowDirections2 & FlowBit(direction)) != 0)
							{
								return flowDirection2;
							}
						}
					}
				}
			}
			for (int j = 0; j < 5; j++)
			{
				FlowDirection targetFlowDirection = manager.soaInfo.GetTargetFlowDirection(idx);
				int num3 = (int)(targetFlowDirection + j - 1);
				int num4 = (num3 + 1) % 5;
				FlowDirection flowDirection3 = (FlowDirection)(num4 + 1);
				FlowDirection direction2 = InverseFlow(flowDirection3);
				Conduit conduitFromDirection3 = manager.soaInfo.GetConduitFromDirection(idx, flowDirection3);
				if (conduitFromDirection3.idx != -1)
				{
					int permittedFlowDirections3 = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection3.idx);
					if (permittedFlowDirections3 != -1 && (permittedFlowDirections3 & FlowBit(direction2)) != 0)
					{
						return flowDirection3;
					}
				}
			}
			return FlowDirection.None;
		}

		public FlowDirection GetNextFlowTarget(ConduitFlow manager)
		{
			int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(idx);
			if (permittedFlowDirections == -1)
			{
				return FlowDirection.Blocked;
			}
			for (int i = 0; i < 5; i++)
			{
				FlowDirection targetFlowDirection = manager.soaInfo.GetTargetFlowDirection(idx);
				int num = (int)(targetFlowDirection + i - 1);
				int num2 = (num + 1) % 5;
				int num3 = num2 + 1;
				Conduit conduitFromDirection = manager.soaInfo.GetConduitFromDirection(idx, (FlowDirection)num3);
				if (conduitFromDirection.idx != -1 && (permittedFlowDirections & FlowBit((FlowDirection)num3)) != 0)
				{
					return (FlowDirection)num3;
				}
			}
			return FlowDirection.Blocked;
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

		public float mass;

		public float temperature;

		public byte diseaseIdx;

		public int diseaseCount;

		public static readonly ConduitContents Empty = new ConduitContents
		{
			element = SimHashes.Vacuum,
			mass = 0f,
			temperature = 0f,
			diseaseIdx = 255,
			diseaseCount = 0
		};

		public ConduitContents(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
		{
			this.element = element;
			this.mass = mass;
			this.temperature = temperature;
			diseaseIdx = disease_idx;
			diseaseCount = disease_count;
		}
	}

	private class UpdateConduits : DivisibleTask<ConduitFlow>
	{
		public UpdateConduits()
			: base("UpdateConduits")
		{
		}

		protected override void RunDivision(ConduitFlow conduitFlow)
		{
			for (int i = start; i != end; i++)
			{
				foreach (Conduit item in conduitFlow.pathList[i])
				{
					conduitFlow.UpdateConduit(item);
				}
			}
		}
	}

	private ConduitType conduitType;

	private float MaxMass = 10f;

	private const float PERCENT_MAX_MASS_FOR_STATE_CHANGE_DAMAGE = 0.1f;

	public const float TickRate = 1f;

	public const float WaitTime = 1f;

	private float elapsedTime;

	private float lastUpdateTime = float.NegativeInfinity;

	public SOAInfo soaInfo = new SOAInfo();

	private bool dirtyConduitUpdaters;

	private List<ConduitUpdater> conduitUpdaters = new List<ConduitUpdater>();

	private GridNode[] grid;

	[Serialize]
	public int[] serializedIdx;

	[Serialize]
	public ConduitContents[] serializedContents;

	[Serialize]
	public SerializedContents[] versionedSerializedContents;

	private IUtilityNetworkMgr networkMgr;

	private HashSet<int> visited = new HashSet<int>();

	private HashSet<int> replacements = new HashSet<int>();

	private List<Conduit> path = new List<Conduit>();

	private List<List<Conduit>> pathList = new List<List<Conduit>>();

	private TaskDivision<UpdateConduits, ConduitFlow> updateConduits = new TaskDivision<UpdateConduits, ConduitFlow>();

	private WorkItemCollection<UpdateConduits, ConduitFlow> updateConduitsJob = new WorkItemCollection<UpdateConduits, ConduitFlow>();

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

	public static int FlowBit(FlowDirection direction)
	{
		return 1 << (int)(direction - 1);
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
		foreach (FlowUtilityNetwork network in networks)
		{
			ScanNetworkSources(network);
		}
		RefreshPaths();
	}

	private void RebuildConnections(IEnumerable<int> root_nodes)
	{
		soaInfo.Clear(this);
		pathList.Clear();
		ObjectLayer layer = (conduitType != ConduitType.Gas) ? ObjectLayer.LiquidConduit : ObjectLayer.GasConduit;
		foreach (int root_node in root_nodes)
		{
			if (replacements.Contains(root_node))
			{
				replacements.Remove(root_node);
			}
			GameObject gameObject = Grid.Objects[root_node, (int)layer];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
			{
				global::Conduit component = gameObject.GetComponent<global::Conduit>();
				if (!((UnityEngine.Object)component != (UnityEngine.Object)null) || !component.IsDisconnected())
				{
					int conduitIdx = soaInfo.AddConduit(this, gameObject, root_node);
					grid[root_node].conduitIdx = conduitIdx;
				}
			}
		}
		Game.Instance.conduitTemperatureManager.Sim200ms(0f);
		foreach (int root_node2 in root_nodes)
		{
			UtilityConnections connections = networkMgr.GetConnections(root_node2, true);
			if (connections != 0 && grid[root_node2].conduitIdx != -1)
			{
				int conduitIdx2 = grid[root_node2].conduitIdx;
				ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduitIdx2);
				int num = root_node2 - 1;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Left) != 0)
				{
					conduitConnections.left = grid[num].conduitIdx;
				}
				num = root_node2 + 1;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Right) != 0)
				{
					conduitConnections.right = grid[num].conduitIdx;
				}
				num = root_node2 - Grid.WidthInCells;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Down) != 0)
				{
					conduitConnections.down = grid[num].conduitIdx;
				}
				num = root_node2 + Grid.WidthInCells;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Up) != 0)
				{
					conduitConnections.up = grid[num].conduitIdx;
				}
				soaInfo.SetConduitConnections(conduitIdx2, conduitConnections);
			}
		}
		if (this.onConduitsRebuilt != null)
		{
			this.onConduitsRebuilt();
		}
	}

	public void ScanNetworkSources(FlowUtilityNetwork network)
	{
		if (network != null)
		{
			for (int i = 0; i < network.sources.Count; i++)
			{
				FlowUtilityNetwork.IItem item = network.sources[i];
				path.Clear();
				visited.Clear();
				FindSinks(i, item.Cell);
			}
		}
	}

	public void RefreshPaths()
	{
		foreach (List<Conduit> path2 in pathList)
		{
			for (int i = 0; i < path2.Count - 1; i++)
			{
				Conduit conduit = path2[i];
				if (conduit.GetTargetFlowDirection(this) == FlowDirection.None)
				{
					FlowDirection direction = GetDirection(conduit, path2[i + 1]);
					conduit.SetTargetFlowDirection(direction, this);
				}
			}
		}
	}

	private void FindSinks(int source_idx, int cell)
	{
		GridNode gridNode = grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			FindSinksInternal(source_idx, gridNode.conduitIdx);
		}
	}

	private void FindSinksInternal(int source_idx, int conduit_idx)
	{
		if (!visited.Contains(conduit_idx))
		{
			visited.Add(conduit_idx);
			Conduit conduit = soaInfo.GetConduit(conduit_idx);
			int permittedFlowDirections = conduit.GetPermittedFlowDirections(this);
			if (permittedFlowDirections != -1)
			{
				path.Add(conduit);
				FlowUtilityNetwork.IItem item = (FlowUtilityNetwork.IItem)networkMgr.GetEndpoint(soaInfo.GetCell(conduit_idx));
				if (item != null && item.EndpointType == Endpoint.Sink)
				{
					FoundSink(source_idx);
				}
				ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduit_idx);
				if (conduitConnections.down != -1)
				{
					FindSinksInternal(source_idx, conduitConnections.down);
				}
				if (conduitConnections.left != -1)
				{
					FindSinksInternal(source_idx, conduitConnections.left);
				}
				if (conduitConnections.right != -1)
				{
					FindSinksInternal(source_idx, conduitConnections.right);
				}
				if (conduitConnections.up != -1)
				{
					FindSinksInternal(source_idx, conduitConnections.up);
				}
				if (path.Count > 0)
				{
					path.RemoveAt(path.Count - 1);
				}
			}
		}
	}

	private FlowDirection GetDirection(Conduit conduit, Conduit target_conduit)
	{
		ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduit.idx);
		if (conduitConnections.up == target_conduit.idx)
		{
			return FlowDirection.Up;
		}
		if (conduitConnections.down == target_conduit.idx)
		{
			return FlowDirection.Down;
		}
		if (conduitConnections.left == target_conduit.idx)
		{
			return FlowDirection.Left;
		}
		if (conduitConnections.right == target_conduit.idx)
		{
			return FlowDirection.Right;
		}
		return FlowDirection.None;
	}

	private void FoundSink(int source_idx)
	{
		for (int i = 0; i < path.Count - 1; i++)
		{
			FlowDirection direction = GetDirection(path[i], path[i + 1]);
			FlowDirection direction2 = InverseFlow(direction);
			SOAInfo sOAInfo = soaInfo;
			Conduit conduit = path[i];
			int cellFromDirection = GetCellFromDirection(sOAInfo.GetCell(conduit.idx), direction2);
			SOAInfo sOAInfo2 = soaInfo;
			Conduit conduit2 = path[i];
			Conduit conduitFromDirection = sOAInfo2.GetConduitFromDirection(conduit2.idx, direction2);
			if (i != 0 && (path[i].GetPermittedFlowDirections(this) & FlowBit(direction2)) != 0)
			{
				int num = cellFromDirection;
				SOAInfo sOAInfo3 = soaInfo;
				Conduit conduit3 = path[i - 1];
				if (num == sOAInfo3.GetCell(conduit3.idx))
				{
					continue;
				}
				SOAInfo sOAInfo4 = soaInfo;
				Conduit conduit4 = path[i];
				if (sOAInfo4.GetSrcFlowIdx(conduit4.idx) != source_idx && (conduitFromDirection.GetPermittedFlowDirections(this) & FlowBit(direction2)) != 0)
				{
					continue;
				}
			}
			int permittedFlowDirections = path[i].GetPermittedFlowDirections(this);
			SOAInfo sOAInfo5 = soaInfo;
			Conduit conduit5 = path[i];
			sOAInfo5.SetSrcFlowIdx(conduit5.idx, source_idx);
			path[i].SetPermittedFlowDirections(permittedFlowDirections | FlowBit(direction), this);
			path[i].SetTargetFlowDirection(direction, this);
		}
		for (int j = 1; j < path.Count; j++)
		{
			FlowDirection direction3 = GetDirection(path[j], path[j - 1]);
			SOAInfo sOAInfo6 = soaInfo;
			Conduit conduit6 = path[j];
			sOAInfo6.SetSrcFlowDirection(conduit6.idx, direction3);
		}
		List<Conduit> list = new List<Conduit>(path);
		list.Reverse();
		TryAdd(list);
	}

	private static int FindIndex(List<Conduit> path, int idx)
	{
		for (int i = 0; i < path.Count; i++)
		{
			Conduit conduit = path[i];
			if (conduit.idx == idx)
			{
				return i;
			}
		}
		return -1;
	}

	private void TryAdd(List<Conduit> new_path)
	{
		foreach (List<Conduit> path2 in pathList)
		{
			if (path2.Count >= new_path.Count)
			{
				bool flag = false;
				List<Conduit> list = path2;
				Conduit conduit = new_path[0];
				int num = FindIndex(list, conduit.idx);
				List<Conduit> list2 = path2;
				Conduit conduit2 = new_path[new_path.Count - 1];
				int num2 = FindIndex(list2, conduit2.idx);
				if (num != -1 && num2 != -1)
				{
					flag = true;
					int num3 = num;
					int num4 = 0;
					while (num3 < num2)
					{
						Conduit conduit3 = path2[num3];
						int idx = conduit3.idx;
						Conduit conduit4 = new_path[num4];
						if (idx != conduit4.idx)
						{
							flag = false;
							break;
						}
						num3++;
						num4++;
					}
				}
				if (flag)
				{
					return;
				}
			}
		}
		for (int num5 = pathList.Count - 1; num5 >= 0; num5--)
		{
			if (pathList[num5].Count <= 0)
			{
				pathList.RemoveAt(num5);
			}
		}
		for (int num6 = pathList.Count - 1; num6 >= 0; num6--)
		{
			List<Conduit> list3 = pathList[num6];
			if (new_path.Count >= list3.Count)
			{
				bool flag2 = false;
				Conduit conduit5 = list3[0];
				int num7 = FindIndex(new_path, conduit5.idx);
				Conduit conduit6 = list3[list3.Count - 1];
				int num8 = FindIndex(new_path, conduit6.idx);
				if (num7 != -1 && num8 != -1)
				{
					flag2 = true;
					int num9 = num7;
					int num10 = 0;
					while (num9 < num8)
					{
						Conduit conduit7 = new_path[num9];
						int idx2 = conduit7.idx;
						Conduit conduit8 = list3[num10];
						if (idx2 != conduit8.idx)
						{
							flag2 = false;
							break;
						}
						num9++;
						num10++;
					}
				}
				if (flag2)
				{
					pathList.RemoveAt(num6);
				}
			}
		}
		foreach (List<Conduit> path3 in pathList)
		{
			for (int num11 = new_path.Count - 1; num11 >= 0; num11--)
			{
				Conduit conduit9 = new_path[num11];
				int num12 = FindIndex(path3, conduit9.idx);
				if (num12 != -1)
				{
					int permittedFlowDirections = soaInfo.GetPermittedFlowDirections(conduit9.idx);
					if (Mathf.IsPowerOfTwo(permittedFlowDirections))
					{
						new_path.RemoveAt(num11);
					}
				}
			}
		}
		pathList.Add(new_path);
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
			Output.LogError("unexpected temperature");
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

	public static int GetCellFromDirection(int cell, FlowDirection direction)
	{
		switch (direction)
		{
		case FlowDirection.Left:
			return Grid.CellLeft(cell);
		case FlowDirection.Right:
			return Grid.CellRight(cell);
		case FlowDirection.Up:
			return Grid.CellAbove(cell);
		case FlowDirection.Down:
			return Grid.CellBelow(cell);
		default:
			return -1;
		}
	}

	public static FlowDirection InverseFlow(FlowDirection direction)
	{
		switch (direction)
		{
		case FlowDirection.Left:
			return FlowDirection.Right;
		case FlowDirection.Right:
			return FlowDirection.Left;
		case FlowDirection.Up:
			return FlowDirection.Down;
		case FlowDirection.Down:
			return FlowDirection.Up;
		default:
			return FlowDirection.None;
		}
	}

	public void Sim200ms(float dt)
	{
		if (!(dt <= 0f))
		{
			elapsedTime += dt;
			if (!(elapsedTime < 1f))
			{
				float obj = 1f;
				elapsedTime -= 1f;
				lastUpdateTime = Time.time;
				soaInfo.BeginFrame(this);
				if (updateConduitsJob.Count == 0)
				{
					updateConduitsJob.Reset(this);
					UpdateConduits[] tasks = updateConduits.tasks;
					foreach (UpdateConduits work_item in tasks)
					{
						updateConduitsJob.Add(work_item);
					}
				}
				updateConduits.Initialize(pathList.Count);
				updateConduits.Run(this);
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

	private void UpdateConduit(Conduit conduit)
	{
		if (!soaInfo.GetUpdated(conduit.idx))
		{
			if (soaInfo.GetSrcFlowDirection(conduit.idx) == FlowDirection.None)
			{
				soaInfo.SetSrcFlowDirection(conduit.idx, conduit.GetNextFlowSource(this));
			}
			int cell = soaInfo.GetCell(conduit.idx);
			ConduitContents contents = grid[cell].contents;
			if (contents.element != SimHashes.Vacuum)
			{
				if (contents.mass <= 0f)
				{
					soaInfo.MarkConduitEmpty(conduit.idx, this);
				}
				else
				{
					FlowDirection targetFlowDirection = soaInfo.GetTargetFlowDirection(conduit.idx);
					Conduit conduitFromDirection = soaInfo.GetConduitFromDirection(conduit.idx, targetFlowDirection);
					if (conduitFromDirection.idx == -1)
					{
						soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
					}
					else
					{
						int cell2 = soaInfo.GetCell(conduitFromDirection.idx);
						ConduitContents contents2 = grid[cell2].contents;
						if (contents2.element != SimHashes.Vacuum && contents2.element != contents.element)
						{
							soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
						}
						else
						{
							int permittedFlowDirections = soaInfo.GetPermittedFlowDirections(conduit.idx);
							if ((permittedFlowDirections & FlowBit(targetFlowDirection)) != 0)
							{
								bool flag = false;
								for (int i = 0; i < 5; i++)
								{
									Conduit conduitFromDirection2 = soaInfo.GetConduitFromDirection(conduitFromDirection.idx, soaInfo.GetSrcFlowDirection(conduitFromDirection.idx));
									if (conduitFromDirection2.idx == conduit.idx)
									{
										flag = true;
										break;
									}
									if (conduitFromDirection2.idx != -1)
									{
										int cell3 = soaInfo.GetCell(conduitFromDirection2.idx);
										ConduitContents contents3 = grid[cell3].contents;
										if (contents3.element != SimHashes.Vacuum)
										{
											break;
										}
									}
									soaInfo.SetSrcFlowDirection(conduitFromDirection.idx, conduitFromDirection.GetNextFlowSource(this));
								}
								if (flag)
								{
									float b = Mathf.Max(0f, MaxMass - contents2.mass);
									float num = Mathf.Min(contents.mass, b);
									if (num > 0f)
									{
										int disease_count = (int)(num / contents.mass * (float)contents.diseaseCount);
										num = AddElementToGrid(cell2, contents.element, num, contents.temperature, contents.diseaseIdx, disease_count);
										ConduitContents contents4 = RemoveElementFromGrid(conduit, num);
										soaInfo.SetLastFlowInfo(conduit.idx, soaInfo.GetTargetFlowDirection(conduit.idx), ref contents4);
										soaInfo.SetUpdated(conduitFromDirection.idx, true);
										soaInfo.SetSrcFlowDirection(conduitFromDirection.idx, conduitFromDirection.GetNextFlowSource(this));
									}
								}
							}
							soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
						}
					}
				}
			}
		}
	}

	public float AddElement(int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (grid[cell_idx].conduitIdx == -1)
		{
			return 0f;
		}
		ConduitContents contents = GetConduit(cell_idx).GetContents(this);
		if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
		{
			return 0f;
		}
		float num = Mathf.Min(mass, MaxMass - contents.mass);
		float num2 = num / mass;
		if (num <= 0f)
		{
			return 0f;
		}
		contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
		contents.mass += num;
		contents.element = element;
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

	private float AddElementToGrid(int cell_idx, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		ConduitContents contents = grid[cell_idx].contents;
		if (contents.element != element && contents.element != SimHashes.Vacuum && mass > 0f)
		{
			return 0f;
		}
		float num = Mathf.Min(mass, MaxMass - contents.mass);
		if (num <= 0f)
		{
			return 0f;
		}
		contents.temperature = GameUtil.GetFinalTemperature(temperature, num, contents.temperature, contents.mass);
		contents.mass += num;
		contents.element = element;
		float num2 = num / mass;
		int num3 = (int)(num2 * (float)disease_count);
		if (num3 > 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, num3, contents.diseaseIdx, contents.diseaseCount);
			contents.diseaseIdx = diseaseInfo.idx;
			contents.diseaseCount = diseaseInfo.count;
		}
		grid[cell_idx].contents = contents;
		return num;
	}

	public ConduitContents RemoveElement(int cell, float delta)
	{
		Conduit conduit = GetConduit(cell);
		if (conduit.idx != -1)
		{
			return RemoveElement(conduit, delta);
		}
		return ConduitContents.Empty;
	}

	public ConduitContents RemoveElement(Conduit conduit, float delta)
	{
		ConduitContents contents = conduit.GetContents(this);
		ConduitContents result = contents;
		ConduitContents contents2 = default(ConduitContents);
		result.mass = Mathf.Min(contents.mass, delta);
		float num = contents.mass - result.mass;
		if (num <= 0f)
		{
			contents2 = ConduitContents.Empty;
		}
		else
		{
			float num2 = num / contents.mass;
			int num3 = (int)(num2 * (float)contents.diseaseCount);
			result.diseaseCount = contents.diseaseCount - num3;
			contents2.mass = num;
			contents2.temperature = contents.temperature;
			contents2.element = contents.element;
			contents2.diseaseIdx = contents.diseaseIdx;
			contents2.diseaseCount = num3;
			if (num3 <= 0)
			{
				contents2.diseaseIdx = byte.MaxValue;
				contents2.diseaseCount = 0;
			}
		}
		conduit.SetContents(this, contents2);
		return result;
	}

	private ConduitContents RemoveElementFromGrid(Conduit conduit, float delta)
	{
		int cell = soaInfo.GetCell(conduit.idx);
		ConduitContents contents = grid[cell].contents;
		ConduitContents result = contents;
		ConduitContents contents2 = default(ConduitContents);
		result.mass = Mathf.Min(contents.mass, delta);
		float num = contents.mass - result.mass;
		if (num <= 0f)
		{
			contents2 = ConduitContents.Empty;
		}
		else
		{
			float num2 = num / contents.mass;
			int num3 = (int)(num2 * (float)contents.diseaseCount);
			result.diseaseCount = contents.diseaseCount - num3;
			contents2.mass = num;
			contents2.temperature = contents.temperature;
			contents2.element = contents.element;
			contents2.diseaseIdx = contents.diseaseIdx;
			contents2.diseaseCount = num3;
			if (num3 <= 0)
			{
				contents2.diseaseIdx = byte.MaxValue;
				contents2.diseaseCount = 0;
			}
		}
		grid[cell].contents = contents2;
		return result;
	}

	public int GetPermittedFlow(int cell)
	{
		Conduit conduit = GetConduit(cell);
		if (conduit.idx == -1)
		{
			return 0;
		}
		return soaInfo.GetPermittedFlowDirections(conduit.idx);
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
		if (contents.mass > 0f && contents.temperature <= 0f)
		{
			Output.LogError("zero degree pipe contents");
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
			ConduitContents contents = default(ConduitContents);
			for (int j = 0; j < versionedSerializedContents.Length; j++)
			{
				int num = serializedIdx[j];
				SerializedContents serializedContents = versionedSerializedContents[j];
				if (serializedContents.mass <= 0f)
				{
					contents.element = SimHashes.Vacuum;
					contents.mass = 0f;
					contents.temperature = 0f;
				}
				else
				{
					contents.element = serializedContents.element;
					contents.mass = serializedContents.mass;
					contents.temperature = serializedContents.temperature;
				}
				if (serializedContents.diseaseCount <= 0 || serializedContents.diseaseHash == 0)
				{
					contents.diseaseCount = 0;
					contents.diseaseIdx = byte.MaxValue;
				}
				else
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
				contents.mass = Math.Min(MaxMass, contents.mass);
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
		if ((UnityEngine.Object)conduitGO != (UnityEngine.Object)null)
		{
			ConduitContents contents = soaInfo.GetConduit(conduit_idx).GetContents(this);
			if (contents.mass > MaxMass * 0.1f)
			{
				conduitGO.Trigger(-700727624, null);
			}
		}
	}

	public void MeltConduitContents(int conduit_idx)
	{
		GameObject conduitGO = soaInfo.GetConduitGO(conduit_idx);
		if ((UnityEngine.Object)conduitGO != (UnityEngine.Object)null)
		{
			ConduitContents contents = soaInfo.GetConduit(conduit_idx).GetContents(this);
			if (contents.mass > MaxMass * 0.1f)
			{
				conduitGO.Trigger(-1152799878, null);
			}
		}
	}
}
