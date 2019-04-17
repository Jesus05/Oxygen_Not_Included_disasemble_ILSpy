using Database;
using FMODUnity;
using Klei.AI;
using KSerialization;
using System.Collections.Generic;
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

	private HandleVector<int>.Handle pickupablesChangedEntry;

	public static TagBits tagBits = new TagBits(STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat(STORAGEFILTERS.FOOD).ToArray());

	private bool pickupablesDirty;

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

	private ArmAnim arm_anim;

	private List<int> reachableCells = new List<int>(100);

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnEndChoreDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnEndChore(data);
	});

	private int serial_no;

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
		simRenderLoadBalance = true;
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
		pickupableExtents = new Extents(this.NaturalBuildingCell(), pickupRange);
		pickupablesChangedEntry = GameScenePartitioner.Instance.Add("SolidTransferArm.PickupablesChanged", base.gameObject, pickupableExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
		pickupablesDirty = true;
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
		GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
		MinionGroupProber.Get().ReleaseProber(this);
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		if (operational.IsOperational)
		{
			RefreshReachableCells();
			pickupablesDirty = true;
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
	}

	private void UpdateArmAnim()
	{
		FetchAreaChore fetchAreaChore = choreDriver.GetCurrentChore() as FetchAreaChore;
		if ((bool)worker.workable && fetchAreaChore != null && rotation_complete)
		{
			StopRotateSound();
			if (fetchAreaChore.IsDelivering)
			{
				SetArmAnim(ArmAnim.Drop);
			}
			else
			{
				SetArmAnim(ArmAnim.Pickup);
			}
		}
		else
		{
			SetArmAnim(ArmAnim.Idle);
		}
	}

	private void RefreshReachableCells()
	{
		ListPool<int, SolidTransferArm>.PooledList pooledList = ListPool<int, SolidTransferArm>.Allocate(reachableCells);
		reachableCells.Clear();
		Grid.CellToXY(Grid.PosToCell(this), out int x, out int y);
		for (int i = y - pickupRange; i < y + pickupRange + 1; i++)
		{
			for (int j = x - pickupRange; j < x + pickupRange + 1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num) && Grid.IsPhysicallyAccessible(x, y, j, i, true))
				{
					reachableCells.Add(num);
				}
			}
		}
		bool flag = false;
		if (reachableCells.Count == pooledList.Count)
		{
			flag = true;
			for (int k = 0; k != reachableCells.Count; k++)
			{
				if (reachableCells[k] != pooledList[k])
				{
					flag = false;
					break;
				}
			}
		}
		pooledList.Recycle();
		if (!flag)
		{
			serial_no++;
			MinionGroupProber.Get().SetValidSerialNos(this, serial_no, serial_no);
			MinionGroupProber.Get().Occupy(this, serial_no, reachableCells);
		}
	}

	public bool IsCellReachable(int cell)
	{
		return reachableCells.Contains(cell);
	}

	private void RefreshPickupables()
	{
		if (pickupablesDirty)
		{
			pickupables.Clear();
			int cell_a = Grid.PosToCell(this);
			foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> prefabIdToFetchable in Game.Instance.fetchManager.prefabIdToFetchables)
			{
				foreach (FetchManager.Fetchable data in prefabIdToFetchable.Value.fetchables.GetDataList())
				{
					FetchManager.Fetchable current = data;
					Pickupable pickupable = current.pickupable;
					int pickupableCell = GetPickupableCell(pickupable);
					int cellRange = Grid.GetCellRange(cell_a, pickupableCell);
					if (cellRange <= pickupRange && IsPickupableRelevantToMyInterests(pickupable) && pickupable.CouldBePickedUpByTransferArm(base.gameObject))
					{
						pickupables.Add(pickupable);
					}
				}
			}
			pickupablesDirty = false;
		}
	}

	private void OnPickupablesChanged(object data)
	{
		Pickupable pickupable = data as Pickupable;
		if ((bool)pickupable && IsPickupableRelevantToMyInterests(pickupable))
		{
			pickupablesDirty = true;
		}
	}

	private bool IsPickupableRelevantToMyInterests(Pickupable pickupable)
	{
		KPrefabID kPrefabID = pickupable.KPrefabID;
		if (!kPrefabID.HasAnyTags(ref tagBits))
		{
			return false;
		}
		int pickupableCell = GetPickupableCell(pickupable);
		if (!IsCellReachable(pickupableCell))
		{
			return false;
		}
		return true;
	}

	public void FindFetchTarget(Storage destination, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, float required_amount, ref Pickupable target)
	{
		RefreshPickupables();
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

	private int GetPickupableCell(Pickupable pickupable)
	{
		if ((bool)pickupable.storage)
		{
			return Grid.PosToCell(pickupable.storage);
		}
		return pickupable.cachedCell;
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
}
