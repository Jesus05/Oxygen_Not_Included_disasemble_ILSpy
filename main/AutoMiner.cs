using FMODUnity;
using KSerialization;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AutoMiner : StateMachineComponent<AutoMiner.Instance>, ISim1000ms
{
	public class Instance : GameStateMachine<States, Instance, AutoMiner, object>.GameInstance
	{
		public Instance(AutoMiner master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, Instance, AutoMiner>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State moving;

			public State digging;
		}

		public BoolParameter transferring;

		public State off;

		public ReadyStates on;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (AutoMiner.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			on.DefaultState(on.idle).EventTransition(GameHashes.OperationalChanged, off, (AutoMiner.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
			on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, on.moving, (AutoMiner.Instance smi) => smi.GetComponent<Operational>().IsActive);
			on.moving.Exit(delegate(AutoMiner.Instance smi)
			{
				smi.master.StopRotateSound();
			}).PlayAnim("working").EventTransition(GameHashes.ActiveChanged, on.idle, (AutoMiner.Instance smi) => !smi.GetComponent<Operational>().IsActive)
				.Update(delegate(AutoMiner.Instance smi, float dt)
				{
					smi.master.UpdateRotation(dt);
				}, UpdateRate.SIM_33ms, false)
				.Transition(on.digging, RotationComplete, UpdateRate.SIM_200ms);
			on.digging.Enter(delegate(AutoMiner.Instance smi)
			{
				smi.master.StartDig();
			}).Exit(delegate(AutoMiner.Instance smi)
			{
				smi.master.StopDig();
			}).PlayAnim("working")
				.EventTransition(GameHashes.ActiveChanged, on.idle, (AutoMiner.Instance smi) => !smi.GetComponent<Operational>().IsActive)
				.Update(delegate(AutoMiner.Instance smi, float dt)
				{
					smi.master.UpdateDig(dt);
				}, UpdateRate.SIM_200ms, false)
				.Transition(on.moving, GameStateMachine<States, AutoMiner.Instance, AutoMiner, object>.Not(RotationComplete), UpdateRate.SIM_200ms);
		}

		public static bool RotationComplete(AutoMiner.Instance smi)
		{
			return smi.master.RotationComplete;
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

	public int x;

	public int y;

	public int width;

	public int height;

	public CellOffset vision_offset;

	private KBatchedAnimController arm_anim_ctrl;

	private GameObject arm_go;

	private LoopingSounds looping_sounds;

	[EventRef]
	private string rotateSound = "AutoMiner_rotate";

	private KAnimLink link;

	private float arm_rot = 45f;

	private float turn_rate = 180f;

	private bool rotation_complete;

	private bool rotate_sound_playing;

	private GameObject hitEffectPrefab;

	private GameObject hitEffect;

	private int dig_cell = Grid.InvalidCell;

	private static readonly EventSystem.IntraObjectHandler<AutoMiner> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<AutoMiner>(delegate(AutoMiner component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static HashedString HASH_ROTATION = "rotation";

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache0;

	private bool HasDigCell => dig_cell != Grid.InvalidCell;

	private bool RotationComplete => HasDigCell && rotation_complete;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		simRenderLoadBalance = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		hitEffectPrefab = Assets.GetPrefab("fx_dig_splash");
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		string name = component.name + ".gun";
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
		arm_anim_ctrl.initialAnim = "gun";
		arm_anim_ctrl.isMovable = true;
		arm_anim_ctrl.sceneLayer = Grid.SceneLayer.TransferArm;
		component.SetSymbolVisiblity("gun_target", false);
		bool symbolVisible;
		Vector4 column = component.GetSymbolTransform(new HashedString("gun_target"), out symbolVisible).GetColumn(3);
		Vector3 position = column;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.TransferArm);
		arm_go.transform.SetPosition(position);
		arm_go.SetActive(true);
		link = new KAnimLink(component, arm_anim_ctrl);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		RotateArm(rotatable.GetRotatedOffset(Quaternion.Euler(0f, 0f, -45f) * Vector3.up), true, 0f);
		StopDig();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		if (operational.IsOperational)
		{
			RefreshDiggableCell();
			operational.SetActive(HasDigCell, false);
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			dig_cell = Grid.InvalidCell;
			rotation_complete = false;
		}
	}

	public void UpdateRotation(float dt)
	{
		if (HasDigCell)
		{
			Vector3 a = Grid.CellToPosCCC(dig_cell, Grid.SceneLayer.TileMain);
			a.z = 0f;
			Vector3 position = arm_go.transform.GetPosition();
			position.z = 0f;
			Vector3 target_dir = Vector3.Normalize(a - position);
			RotateArm(target_dir, false, dt);
		}
	}

	private Element GetTargetElement()
	{
		if (HasDigCell)
		{
			return Grid.Element[dig_cell];
		}
		return null;
	}

	public void StartDig()
	{
		Element targetElement = GetTargetElement();
		Trigger(-1762453998, targetElement);
		CreateHitEffect();
		arm_anim_ctrl.Play("gun_digging", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void StopDig()
	{
		Trigger(939543986, null);
		DestroyHitEffect();
		arm_anim_ctrl.Play("gun", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void UpdateDig(float dt)
	{
		if (HasDigCell && rotation_complete)
		{
			Diggable.DoDigTick(dig_cell, dt);
			Vector3 a = Grid.CellToPosCCC(dig_cell, Grid.SceneLayer.FXFront2);
			a.z = 0f;
			Vector3 position = arm_go.transform.GetPosition();
			position.z = 0f;
			float sqrMagnitude = (a - position).sqrMagnitude;
			arm_anim_ctrl.GetBatchInstanceData().SetClipRadius(position.x, position.y, sqrMagnitude, true);
			if (!ValidDigCell(dig_cell))
			{
				dig_cell = Grid.InvalidCell;
				rotation_complete = false;
			}
		}
	}

	private void CreateHitEffect()
	{
		if (!((UnityEngine.Object)hitEffectPrefab == (UnityEngine.Object)null))
		{
			if ((UnityEngine.Object)hitEffect != (UnityEngine.Object)null)
			{
				DestroyHitEffect();
			}
			Vector3 position = Grid.CellToPosCCC(dig_cell, Grid.SceneLayer.FXFront2);
			hitEffect = GameUtil.KInstantiate(hitEffectPrefab, position, Grid.SceneLayer.FXFront2, null, 0);
			hitEffect.SetActive(true);
			KBatchedAnimController component = hitEffect.GetComponent<KBatchedAnimController>();
			component.sceneLayer = Grid.SceneLayer.FXFront2;
			component.initialMode = KAnim.PlayMode.Loop;
			component.enabled = false;
			component.enabled = true;
		}
	}

	private void DestroyHitEffect()
	{
		if (!((UnityEngine.Object)hitEffectPrefab == (UnityEngine.Object)null) && (UnityEngine.Object)hitEffect != (UnityEngine.Object)null)
		{
			hitEffect.DeleteObject();
			hitEffect = null;
		}
	}

	private void RefreshDiggableCell()
	{
		CellOffset rotatedCellOffset = vision_offset;
		if ((bool)rotatable)
		{
			rotatedCellOffset = rotatable.GetRotatedCellOffset(vision_offset);
		}
		int cell = Grid.PosToCell(base.transform.gameObject);
		int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
		Grid.CellToXY(cell2, out int num, out int num2);
		float num3 = 3.40282347E+38f;
		int num4 = Grid.InvalidCell;
		Vector3 a = Grid.CellToPos(cell2);
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				CellOffset offset = new CellOffset(this.x + j, this.y + i);
				if ((bool)rotatable)
				{
					offset = rotatable.GetRotatedCellOffset(offset);
				}
				int num5 = Grid.OffsetCell(cell, offset);
				if (Grid.IsValidCell(num5))
				{
					Grid.CellToXY(num5, out int x, out int y);
					if (Grid.IsValidCell(num5) && ValidDigCell(num5) && Grid.TestLineOfSight(num, num2, x, y, DigBlockingCB, false))
					{
						if (num5 == dig_cell)
						{
							flag = true;
						}
						Vector3 b = Grid.CellToPos(num5);
						float num6 = Vector3.Distance(a, b);
						if (num6 < num3)
						{
							num3 = num6;
							num4 = num5;
						}
					}
				}
			}
		}
		if (!flag && dig_cell != num4)
		{
			dig_cell = num4;
			rotation_complete = false;
		}
	}

	private static bool ValidDigCell(int cell)
	{
		if (!Grid.Solid[cell])
		{
			return false;
		}
		if (Grid.Foundation[cell])
		{
			return false;
		}
		if (Grid.Element[cell].hardness >= 150)
		{
			return false;
		}
		return true;
	}

	public static bool DigBlockingCB(int cell)
	{
		if (Grid.Foundation[cell])
		{
			return true;
		}
		if (Grid.Element[cell].hardness >= 150)
		{
			return true;
		}
		return false;
	}

	private void RotateArm(Vector3 target_dir, bool warp, float dt)
	{
		if (!rotation_complete)
		{
			float num = MathUtil.AngleSigned(Vector3.up, target_dir, Vector3.forward);
			float val = num - arm_rot;
			val = MathUtil.Wrap(-180f, 180f, val);
			rotation_complete = Mathf.Approximately(val, 0f);
			float num2 = val;
			if (warp)
			{
				rotation_complete = true;
			}
			else
			{
				num2 = Mathf.Clamp(num2, (0f - turn_rate) * dt, turn_rate * dt);
			}
			arm_rot += num2;
			arm_rot = MathUtil.Wrap(-180f, 180f, arm_rot);
			arm_go.transform.rotation = Quaternion.Euler(0f, 0f, arm_rot);
			if (!rotation_complete)
			{
				StartRotateSound();
				looping_sounds.SetParameter(rotateSound, HASH_ROTATION, arm_rot);
			}
			else
			{
				StopRotateSound();
			}
		}
	}

	private void StartRotateSound()
	{
		if (!rotate_sound_playing)
		{
			looping_sounds.StartSound(rotateSound);
			rotate_sound_playing = true;
		}
	}

	private void StopRotateSound()
	{
		if (rotate_sound_playing)
		{
			looping_sounds.StopSound(rotateSound);
			rotate_sound_playing = false;
		}
	}
}
