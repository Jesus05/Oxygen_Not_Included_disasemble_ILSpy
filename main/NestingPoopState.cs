using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class NestingPoopState : GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>
{
	public class Def : BaseDef
	{
		public Tag nestingPoopElement = Tag.Invalid;

		public Def(Tag tag)
		{
			nestingPoopElement = tag;
		}
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		private int lastPoopCell = -1;

		public int targetPoopCell = -1;

		private Tag currentlyPoopingElement = Tag.Invalid;

		[CompilerGenerated]
		private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Poop);
		}

		private static bool IsValidNestingCell(int cell, object arg)
		{
			return Grid.IsValidCell(cell) && !Grid.Solid[cell] && Grid.Solid[Grid.CellBelow(cell)] && (IsValidPoopFromCell(cell, true) || IsValidPoopFromCell(cell, false));
		}

		private static bool IsValidPoopFromCell(int cell, bool look_left)
		{
			if (look_left)
			{
				int num = Grid.CellDownLeft(cell);
				int num2 = Grid.CellLeft(cell);
				return Grid.IsValidCell(num) && Grid.Solid[num] && Grid.IsValidCell(num2) && !Grid.Solid[num2];
			}
			int num3 = Grid.CellDownRight(cell);
			int num4 = Grid.CellRight(cell);
			return Grid.IsValidCell(num3) && Grid.Solid[num3] && Grid.IsValidCell(num4) && !Grid.Solid[num4];
		}

		public int GetPoopPosition()
		{
			targetPoopCell = GetTargetPoopCell();
			List<Direction> list = new List<Direction>();
			if (IsValidPoopFromCell(targetPoopCell, true))
			{
				list.Add(Direction.Left);
			}
			if (IsValidPoopFromCell(targetPoopCell, false))
			{
				list.Add(Direction.Right);
			}
			if (list.Count > 0)
			{
				Direction d = list[UnityEngine.Random.Range(0, list.Count)];
				int cellInDirection = Grid.GetCellInDirection(targetPoopCell, d);
				if (Grid.IsValidCell(cellInDirection))
				{
					return cellInDirection;
				}
			}
			if (Grid.IsValidCell(targetPoopCell))
			{
				return targetPoopCell;
			}
			if (!Grid.IsValidCell(Grid.PosToCell(this)))
			{
				Debug.LogWarning("This is bad, how is Mole occupying an invalid cell?");
			}
			return Grid.PosToCell(this);
		}

		private int GetTargetPoopCell()
		{
			CreatureCalorieMonitor.Instance sMI = base.smi.GetSMI<CreatureCalorieMonitor.Instance>();
			currentlyPoopingElement = sMI.stomach.GetNextPoopEntry();
			int num = GameUtil.FloodFillFind<object>(start_cell: (!(currentlyPoopingElement == base.smi.def.nestingPoopElement) || !(base.smi.def.nestingPoopElement != Tag.Invalid) || lastPoopCell == -1) ? Grid.PosToCell(this) : lastPoopCell, fn: IsValidNestingCell, arg: null, max_depth: 8, stop_at_solid: false, stop_at_liquid: true);
			if (num == -1)
			{
				CellOffset[] array = new CellOffset[5]
				{
					new CellOffset(0, 0),
					new CellOffset(-1, 0),
					new CellOffset(1, 0),
					new CellOffset(-1, -1),
					new CellOffset(1, -1)
				};
				num = Grid.OffsetCell(lastPoopCell, array[UnityEngine.Random.Range(0, array.Length)]);
				int num2 = Grid.CellAbove(num);
				while (Grid.IsValidCell(num2) && Grid.Solid[num2])
				{
					num = num2;
					num2 = Grid.CellAbove(num);
				}
			}
			return num;
		}

		public void SetLastPoopCell()
		{
			if (currentlyPoopingElement == base.smi.def.nestingPoopElement)
			{
				lastPoopCell = Grid.PosToCell(this);
			}
		}
	}

	public State goingtopoop;

	public State pooping;

	public State behaviourcomplete;

	public State failedtonest;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtopoop;
		goingtopoop.MoveTo((Instance smi) => smi.GetPoopPosition(), pooping, failedtonest, false);
		failedtonest.Enter(delegate(Instance smi)
		{
			smi.SetLastPoopCell();
		}).GoTo(pooping);
		pooping.Enter(delegate(Instance smi)
		{
			Facing component = smi.master.GetComponent<Facing>();
			component.SetFacing(Grid.PosToCell(smi.master.gameObject) > smi.targetPoopCell);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null).PlayAnim("poop")
			.OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.Enter(delegate(Instance smi)
		{
			smi.SetLastPoopCell();
		}).PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.Poop, false);
	}
}
