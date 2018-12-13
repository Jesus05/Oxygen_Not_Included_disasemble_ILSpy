using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class SubmergedStates : GameStateMachine<SubmergedStates, SubmergedStates.Instance, IStateMachineTarget, SubmergedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int targetCell = -1;

		[CompilerGenerated]
		private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Submerged);
		}

		public bool FindTargetCell()
		{
			int num = Grid.PosToCell(base.gameObject);
			targetCell = GameUtil.FloodFillFind<object>(IsAboveWater, null, num, 8, true, false);
			if (targetCell == -1)
			{
				CellOffset[] array = new CellOffset[5]
				{
					new CellOffset(0, 0),
					new CellOffset(-1, 0),
					new CellOffset(1, 0),
					new CellOffset(-1, -1),
					new CellOffset(1, -1)
				};
				targetCell = Grid.OffsetCell(num, array[UnityEngine.Random.Range(0, array.Length)]);
				int cell = Grid.CellAbove(targetCell);
				while (Grid.IsSubstantialLiquid(cell, 0.35f))
				{
					targetCell = cell;
					cell = Grid.CellAbove(targetCell);
				}
			}
			return targetCell != -1;
		}

		private static bool IsAboveWater(int cell, object arg)
		{
			return !Grid.IsSubstantialLiquid(cell, 0.35f);
		}

		public bool IsSubmerged()
		{
			return !IsAboveWater(Grid.PosToCell(base.transform.GetPosition()), null);
		}

		public void PlayIdleAnim(bool loop)
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			Navigator component2 = base.smi.GetComponent<Navigator>();
			KAnim.PlayMode mode = (!loop) ? KAnim.PlayMode.Once : KAnim.PlayMode.Loop;
			NavType nav_type = (!IsSubmerged()) ? NavType.Hover : NavType.Swim;
			HashedString idleAnim = component2.NavGrid.GetIdleAnim(nav_type);
			component.Play(idleAnim, mode, 1f, 0f);
		}
	}

	public State idle;

	public State idle_pre;

	public State movetosurface;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle_pre;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		idle_pre.Enter("PlayIdleAnim", delegate(Instance smi)
		{
			smi.PlayIdleAnim(false);
		}).OnAnimQueueComplete(idle);
		idle.Enter("PlayIdleAnim", delegate(Instance smi)
		{
			smi.PlayIdleAnim(true);
		}).Transition(behaviourcomplete, (Instance smi) => !smi.IsSubmerged(), UpdateRate.SIM_1000ms).Transition(movetosurface, (Instance smi) => smi.FindTargetCell(), UpdateRate.SIM_1000ms);
		movetosurface.MoveTo((Instance smi) => smi.targetCell, idle_pre, idle_pre, false);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Submerged, false);
	}
}
