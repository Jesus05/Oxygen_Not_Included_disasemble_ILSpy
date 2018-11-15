using UnityEngine;

public class BurrowMonitor : GameStateMachine<BurrowMonitor, BurrowMonitor.Instance, IStateMachineTarget, BurrowMonitor.Def>
{
	public class Def : BaseDef
	{
		public float burrowHardnessLimit = 20f;

		public float minimumAwakeTime = 24f;

		public Vector2 moundColliderSize = new Vector2f(1f, 1.5f);

		public Vector2 moundColliderOffset = new Vector2(0f, 0.75f);
	}

	public new class Instance : GameInstance
	{
		private Vector2 originalColliderSize;

		private Vector2 originalColliderOffset;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			KBoxCollider2D component = master.GetComponent<KBoxCollider2D>();
			originalColliderSize = component.size;
			originalColliderOffset = component.offset;
		}

		public bool EmergeIsClear()
		{
			int cell = Grid.PosToCell(base.gameObject);
			if (!Grid.IsValidCell(cell) || !Grid.IsValidCell(Grid.CellAbove(cell)))
			{
				return false;
			}
			int i = Grid.CellAbove(cell);
			if (Grid.Solid[i])
			{
				return false;
			}
			if (Grid.IsSubstantialLiquid(Grid.CellAbove(cell), 0.9f))
			{
				return false;
			}
			return true;
		}

		public bool ShouldBurrow()
		{
			if (GameClock.Instance.IsNighttime())
			{
				return false;
			}
			if (!CanBurrowInto(Grid.CellBelow(Grid.PosToCell(base.gameObject))))
			{
				return false;
			}
			return true;
		}

		public bool CanBurrowInto(int cell)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (!Grid.Solid[cell])
			{
				return false;
			}
			if (Grid.IsSubstantialLiquid(Grid.CellAbove(cell), 0.35f))
			{
				return false;
			}
			if ((Object)Grid.Objects[cell, 1] != (Object)null)
			{
				return false;
			}
			if ((float)(int)Grid.Element[cell].hardness > base.def.burrowHardnessLimit)
			{
				return false;
			}
			if (Grid.Foundation[cell])
			{
				return false;
			}
			return true;
		}

		public bool IsEntombed()
		{
			int num = Grid.PosToCell(base.smi);
			return Grid.IsValidCell(num) && Grid.Solid[num];
		}

		public void ExitBurrowComplete()
		{
			base.smi.GetComponent<KBatchedAnimController>().Play("idle_loop", KAnim.PlayMode.Once, 1f, 0f);
			GoTo(base.sm.openair);
		}

		public void BurrowComplete()
		{
			base.smi.transform.SetPosition(Grid.CellToPosCBC(Grid.CellBelow(Grid.PosToCell(base.transform.GetPosition())), Grid.SceneLayer.Creatures));
			base.smi.GetComponent<KBatchedAnimController>().Play("idle_mound", KAnim.PlayMode.Once, 1f, 0f);
			GoTo(base.sm.entombed);
		}

		public void SetCollider(bool original_size)
		{
			KBoxCollider2D component = base.master.GetComponent<KBoxCollider2D>();
			if (original_size)
			{
				component.size = originalColliderSize;
				component.offset = originalColliderOffset;
			}
			else
			{
				component.size = base.def.moundColliderSize;
				component.offset = base.def.moundColliderOffset;
			}
		}
	}

	public State openair;

	public State entombed;

	public State burrowcomplete;

	public State exitburrowcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = openair;
		openair.ToggleBehaviour(GameTags.Creatures.WantsToEnterBurrow, (Instance smi) => smi.ShouldBurrow() && smi.timeinstate > smi.def.minimumAwakeTime, delegate(Instance smi)
		{
			smi.BurrowComplete();
		}).Transition(entombed, (Instance smi) => smi.IsEntombed(), UpdateRate.SIM_200ms).Enter("SetFallAnim", delegate(Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().anim = "fall";
		})
			.Enter("SetCollider", delegate(Instance smi)
			{
				smi.SetCollider(true);
			});
		entombed.Enter("SetFallAnim", delegate(Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().anim = "dormant_pre";
		}).Enter("SetCollider", delegate(Instance smi)
		{
			smi.SetCollider(false);
		}).Transition(openair, (Instance smi) => !smi.IsEntombed(), UpdateRate.SIM_200ms)
			.ToggleBehaviour(GameTags.Creatures.Burrowed, (Instance smi) => smi.IsEntombed(), delegate(Instance smi)
			{
				smi.GoTo(openair);
			})
			.ToggleBehaviour(GameTags.Creatures.WantsToExitBurrow, (Instance smi) => smi.EmergeIsClear() && GameClock.Instance.IsNighttime(), delegate(Instance smi)
			{
				smi.ExitBurrowComplete();
			});
	}
}
