using Database;
using FMODUnity;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidTransferArm : StateMachineComponent<SolidTransferArm.SMInstance>, ISim1000ms, IRenderEveryTick
{
	private enum ArmAnim
	{
		Idle,
		Pickup,
		Drop
	}

	public class SMInstance : GameStateMachine<States, SMInstance, SolidTransferArm, object>.GameInstance
	{
		public SMInstance(SolidTransferArm master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, SolidTransferArm>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;
		}

		public BoolParameter transferring;

		public State off;

		public ReadyStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (SMInstance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(SMInstance smi)
			{
				smi.master.StopRotateSound();
			});
			on.DefaultState(on.idle).EventTransition(GameHashes.OperationalChanged, off, (SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, on.working, (SMInstance smi) => smi.GetComponent<Operational>().IsActive);
			on.working.PlayAnim("working").EventTransition(GameHashes.ActiveChanged, on.idle, (SMInstance smi) => !smi.GetComponent<Operational>().IsActive);
		}
	}

	private struct BatchUpdateContext
	{
		public ListPool<SolidTransferArm, BatchUpdateContext>.PooledList solid_transfer_arms;

		public ListPool<bool, BatchUpdateContext>.PooledList refreshed_reachable_cells;

		public ListPool<int, BatchUpdateContext>.PooledList cells;

		public ListPool<GameObject, BatchUpdateContext>.PooledList game_objects;

		public BatchUpdateContext(List<UpdateBucketWithUpdater<ISim1000ms>.Entry> solid_transfer_arms)
		{
			this.solid_transfer_arms = ListPool<SolidTransferArm, BatchUpdateContext>.Allocate();
			this.solid_transfer_arms.Capacity = solid_transfer_arms.Count;
			refreshed_reachable_cells = ListPool<bool, BatchUpdateContext>.Allocate();
			refreshed_reachable_cells.Capacity = solid_transfer_arms.Count;
			cells = ListPool<int, BatchUpdateContext>.Allocate();
			cells.Capacity = solid_transfer_arms.Count;
			game_objects = ListPool<GameObject, BatchUpdateContext>.Allocate();
			game_objects.Capacity = solid_transfer_arms.Count;
			for (int i = 0; i != solid_transfer_arms.Count; i++)
			{
				UpdateBucketWithUpdater<ISim1000ms>.Entry value = solid_transfer_arms[i];
				value.lastUpdateTime = 0f;
				solid_transfer_arms[i] = value;
				SolidTransferArm solidTransferArm = (SolidTransferArm)value.data;
				if (solidTransferArm.operational.IsOperational)
				{
					this.solid_transfer_arms.Add(solidTransferArm);
					refreshed_reachable_cells.Add(false);
					cells.Add(Grid.PosToCell(solidTransferArm));
					game_objects.Add(solidTransferArm.gameObject);
				}
			}
		}

		public void Finish()
		{
			for (int i = 0; i != solid_transfer_arms.Count; i++)
			{
				if (refreshed_reachable_cells[i])
				{
					solid_transfer_arms[i].IncrementSerialNo();
				}
				solid_transfer_arms[i].Sim();
			}
			refreshed_reachable_cells.Recycle();
			cells.Recycle();
			game_objects.Recycle();
			solid_transfer_arms.Recycle();
		}
	}

	private struct BatchUpdateTask : IWorkItem<BatchUpdateContext>
	{
		private int start;

		private int end;

		private HashSetPool<int, SolidTransferArm>.PooledHashSet reachable_cells_workspace;

		public BatchUpdateTask(int start, int end)
		{
			this.start = start;
			this.end = end;
			reachable_cells_workspace = HashSetPool<int, SolidTransferArm>.Allocate();
		}

		public void Run(BatchUpdateContext context)
		{
			for (int i = start; i != end; i++)
			{
				context.refreshed_reachable_cells[i] = context.solid_transfer_arms[i].AsyncUpdate(context.cells[i], reachable_cells_workspace, context.game_objects[i]);
			}
		}

		public void Finish()
		{
			reachable_cells_workspace.Recycle();
		}
	}

	public struct CachedPickupable
	{
		public Pickupable pickupable;

		public int storage_cell;
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private Worker worker;

	[MyCmpAdd]
	private ChoreConsumer choreConsumer;

	[MyCmpAdd]
	private ChoreDriver choreDriver;

	public int pickupRange = 4;

	private float max_carry_weight = 1000f;

	private List<Pickupable> pickupables = new List<Pickupable>();

	public static TagBits tagBits = new TagBits(STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat(STORAGEFILTERS.FOOD).ToArray());

	private Extents pickupableExtents;

	private KBatchedAnimController arm_anim_ctrl;

	private GameObject arm_go;

	private LoopingSounds looping_sounds;

	private bool rotateSoundPlaying;

	[EventRef]
	private string rotateSound = "TransferArm_rotate";

	private KAnimLink link;

	private float arm_rot = 45f;

	private float turn_rate = 360f;

	private bool rotation_complete;

	private ArmAnim arm_anim = ArmAnim.Idle;

	private HashSet<int> reachableCells = new HashSet<int>();

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnEndChoreDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnEndChore(data);
	});

	private static List<CachedPickupable> cached_pickupables = new List<CachedPickupable>();

	private static WorkItemCollection<BatchUpdateTask, BatchUpdateContext> batch_update_job = new WorkItemCollection<BatchUpdateTask, BatchUpdateContext>();

	private int serial_no = 0;

	private static HashedString HASH_ROTATION = "rotation";

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreConsumer.AddProvider(GlobalChoreProvider.Instance);
		choreConsumer.SetReach(pickupRange);
		Klei.AI.Attributes attributes = this.GetAttributes();
		if (attributes.Get(Db.Get().Attributes.CarryAmount) == null)
		{
			attributes.Add(Db.Get().Attributes.CarryAmount);
		}
		AttributeModifier modifier = new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, max_carry_weight, base.gameObject.GetProperName(), false, false, true);
		this.GetAttributes().Add(modifier);
		worker.usesMultiTool = false;
		storage.fxPrefix = Storage.FXPrefix.PickedUp;
		simRenderLoadBalance = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		string name = component.name + ".arm";
		arm_go = new GameObject(name);
		arm_go.SetActive(false);
		arm_go.transform.parent = component.transform;
		looping_sounds = arm_go.AddComponent<LoopingSounds>();
		rotateSound = GlobalAssets.GetSound(rotateSound, false);
		KPrefabID kPrefabID = arm_go.AddComponent<KPrefabID>();
		kPrefabID.PrefabTag = new Tag(name);
		arm_anim_ctrl = arm_go.AddComponent<KBatchedAnimController>();
		arm_anim_ctrl.AnimFiles = new KAnimFile[1]
		{
			component.AnimFiles[0]
		};
		arm_anim_ctrl.initialAnim = "arm";
		arm_anim_ctrl.isMovable = true;
		arm_anim_ctrl.sceneLayer = Grid.SceneLayer.TransferArm;
		component.SetSymbolVisiblity("arm_target", false);
		bool symbolVisible;
		Vector4 column = component.GetSymbolTransform(new HashedString("arm_target"), out symbolVisible).GetColumn(3);
		Vector3 position = column;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.TransferArm);
		arm_go.transform.SetPosition(position);
		arm_go.SetActive(true);
		link = new KAnimLink(component, arm_anim_ctrl);
		ChoreGroups choreGroups = Db.Get().ChoreGroups;
		for (int i = 0; i < choreGroups.Count; i++)
		{
			choreConsumer.SetPermittedByUser(choreGroups[i], true);
		}
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(1745615042, OnEndChoreDelegate);
		RotateArm(rotatable.GetRotatedOffset(Vector3.up), true, 0f);
		DropLeftovers();
		component.enabled = false;
		component.enabled = true;
		MinionGroupProber.Get().SetValidSerialNos(this, serial_no, serial_no);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		MinionGroupProber.Get().ReleaseProber(this);
		base.OnCleanUp();
	}

	private static void CachePickupables()
	{
		cached_pickupables.Clear();
		foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> prefabIdToFetchable in Game.Instance.fetchManager.prefabIdToFetchables)
		{
			List<FetchManager.Fetchable> dataList = prefabIdToFetchable.Value.fetchables.GetDataList();
			cached_pickupables.Capacity = Math.Max(cached_pickupables.Capacity, cached_pickupables.Count + dataList.Count);
			foreach (FetchManager.Fetchable item in dataList)
			{
				FetchManager.Fetchable current = item;
				cached_pickupables.Add(new CachedPickupable
				{
					pickupable = current.pickupable,
					storage_cell = current.pickupable.storageCell
				});
			}
		}
	}

	public static void BatchUpdate(List<UpdateBucketWithUpdater<ISim1000ms>.Entry> solid_transfer_arms, float time_delta)
	{
		BatchUpdateContext shared_data = new BatchUpdateContext(solid_transfer_arms);
		if (shared_data.solid_transfer_arms.Count == 0)
		{
			shared_data.Finish();
		}
		else
		{
			CachePickupables();
			batch_update_job.Reset(shared_data);
			int num = Math.Max(1, shared_data.solid_transfer_arms.Count / CPUBudget.coreCount);
			int num2 = Math.Min(shared_data.solid_transfer_arms.Count, CPUBudget.coreCount);
			for (int i = 0; i != num2; i++)
			{
				int num3 = i * num;
				int end = (i != num2 - 1) ? (num3 + num) : shared_data.solid_transfer_arms.Count;
				batch_update_job.Add(new BatchUpdateTask(num3, end));
			}
			GlobalJobManager.Run(batch_update_job);
			for (int j = 0; j != batch_update_job.Count; j++)
			{
				batch_update_job.GetWorkItem(j).Finish();
			}
			shared_data.Finish();
		}
	}

	private void Sim()
	{
		Chore.Precondition.Context out_context = default(Chore.Precondition.Context);
		if (choreConsumer.FindNextChore(ref out_context))
		{
			if (out_context.chore is FetchChore)
			{
				choreDriver.SetChore(out_context);
				arm_anim_ctrl.enabled = false;
				arm_anim_ctrl.enabled = true;
			}
			else
			{
				Debug.Assert(false, "I am but a lowly transfer arm. I should only acquire FetchChores: " + out_context.chore);
			}
		}
		operational.SetActive(choreDriver.HasChore(), false);
	}

	public void Sim1000ms(float dt)
	{
	}

	private void UpdateArmAnim()
	{
		FetchAreaChore fetchAreaChore = choreDriver.GetCurrentChore() as FetchAreaChore;
		if ((bool)worker.workable && fetchAreaChore != null && rotation_complete)
		{
			StopRotateSound();
			SetArmAnim((!fetchAreaChore.IsDelivering) ? ArmAnim.Pickup : ArmAnim.Drop);
		}
		else
		{
			SetArmAnim(ArmAnim.Idle);
		}
	}

	private bool AsyncUpdate(int cell, HashSet<int> workspace, GameObject game_object)
	{
		workspace.Clear();
		Grid.CellToXY(cell, out int x, out int y);
		for (int i = y - pickupRange; i < y + pickupRange + 1; i++)
		{
			for (int j = x - pickupRange; j < x + pickupRange + 1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num) && Grid.IsPhysicallyAccessible(x, y, j, i, true))
				{
					workspace.Add(num);
				}
			}
		}
		bool flag = !reachableCells.SetEquals(workspace);
		if (flag)
		{
			reachableCells.Clear();
			reachableCells.UnionWith(workspace);
		}
		pickupables.Clear();
		foreach (CachedPickupable cached_pickupable in cached_pickupables)
		{
			CachedPickupable current = cached_pickupable;
			int cellRange = Grid.GetCellRange(cell, current.storage_cell);
			if (cellRange <= pickupRange && IsPickupableRelevantToMyInterests(current.pickupable.KPrefabID, current.storage_cell) && current.pickupable.CouldBePickedUpByTransferArm(game_object))
			{
				pickupables.Add(current.pickupable);
			}
		}
		return flag;
	}

	private void IncrementSerialNo()
	{
		serial_no++;
		MinionGroupProber.Get().SetValidSerialNos(this, serial_no, serial_no);
		MinionGroupProber.Get().Occupy(this, serial_no, reachableCells);
	}

	public bool IsCellReachable(int cell)
	{
		return reachableCells.Contains(cell);
	}

	private bool IsPickupableRelevantToMyInterests(KPrefabID prefabID, int storage_cell)
	{
		return prefabID.HasAnyTags(ref tagBits) && IsCellReachable(storage_cell);
	}

	public void FindFetchTarget(Storage destination, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, float required_amount, ref Pickupable target)
	{
		target = FetchManager.FindFetchTarget(pickupables, destination, ref tag_bits, ref required_tags, ref forbid_tags, required_amount);
	}

	public void RenderEveryTick(float dt)
	{
		if ((bool)worker.workable)
		{
			Vector3 targetPoint = worker.workable.GetTargetPoint();
			targetPoint.z = 0f;
			Vector3 position = base.transform.GetPosition();
			position.z = 0f;
			Vector3 target_dir = Vector3.Normalize(targetPoint - position);
			RotateArm(target_dir, false, dt);
		}
		UpdateArmAnim();
	}

	private void SetArmAnim(ArmAnim new_anim)
	{
		if (new_anim != arm_anim)
		{
			arm_anim = new_anim;
			switch (arm_anim)
			{
			case ArmAnim.Idle:
				arm_anim_ctrl.Play("arm", KAnim.PlayMode.Loop, 1f, 0f);
				break;
			case ArmAnim.Pickup:
				arm_anim_ctrl.Play("arm_pickup", KAnim.PlayMode.Loop, 1f, 0f);
				break;
			case ArmAnim.Drop:
				arm_anim_ctrl.Play("arm_drop", KAnim.PlayMode.Loop, 1f, 0f);
				break;
			}
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			if (choreDriver.HasChore())
			{
				choreDriver.StopChore();
			}
			UpdateArmAnim();
		}
	}

	private void OnEndChore(object data)
	{
		DropLeftovers();
	}

	private void DropLeftovers()
	{
		if (!storage.IsEmpty() && !choreDriver.HasChore())
		{
			storage.DropAll(false, false, default(Vector3), true);
		}
	}

	private void SetArmRotation(float rot)
	{
		arm_rot = rot;
		arm_go.transform.rotation = Quaternion.Euler(0f, 0f, arm_rot);
	}

	private void RotateArm(Vector3 target_dir, bool warp, float dt)
	{
		float num = MathUtil.AngleSigned(Vector3.up, target_dir, Vector3.forward);
		float num2 = num - arm_rot;
		if (num2 < -180f)
		{
			num2 += 360f;
		}
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (!warp)
		{
			num2 = Mathf.Clamp(num2, (0f - turn_rate) * dt, turn_rate * dt);
		}
		arm_rot += num2;
		SetArmRotation(arm_rot);
		rotation_complete = Mathf.Approximately(num2, 0f);
		if (!warp && !rotation_complete)
		{
			if (!rotateSoundPlaying)
			{
				StartRotateSound();
			}
			SetRotateSoundParameter(arm_rot);
		}
		else
		{
			StopRotateSound();
		}
	}

	private void StartRotateSound()
	{
		if (!rotateSoundPlaying)
		{
			looping_sounds.StartSound(rotateSound);
			rotateSoundPlaying = true;
		}
	}

	private void SetRotateSoundParameter(float arm_rot)
	{
		if (rotateSoundPlaying)
		{
			looping_sounds.SetParameter(rotateSound, HASH_ROTATION, arm_rot);
		}
	}

	private void StopRotateSound()
	{
		if (rotateSoundPlaying)
		{
			looping_sounds.StopSound(rotateSound);
			rotateSoundPlaying = false;
		}
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name, int count)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void EndDetailedSample()
	{
	}
}
