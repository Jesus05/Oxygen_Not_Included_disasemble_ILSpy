using ProcGen;
using System;
using UnityEngine;

public class DiggerMonitor : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
		}

		private void CheckInSolid(int cell)
		{
			int num = Grid.PosToCell(base.gameObject);
			if (cell == num && Grid.IsSolidCell(num))
			{
				Navigator component = base.gameObject.GetComponent<Navigator>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.CurrentNavType != NavType.Solid)
				{
					component.SetCurrentNavType(NavType.Solid);
				}
			}
		}

		private void OnSolidChanged(int cell)
		{
			CheckInSolid(cell);
		}

		public bool IsOnSurface()
		{
			int cell = Grid.PosToCell(this);
			SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
			if (subWorldZoneType == SubWorld.ZoneType.Space)
			{
				int num = Grid.CellAbove(cell);
				while (Grid.IsValidCell(num) && !Grid.Solid[num])
				{
					num = Grid.CellAbove(num);
				}
				return !Grid.IsValidCell(num);
			}
			return false;
		}
	}

	public State loop;

	public State dig;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		loop.EventTransition(GameHashes.BeginMeteorBombardment, (Instance smi) => Game.Instance, dig, (Instance smi) => smi.IsOnSurface());
		dig.ToggleBehaviour(GameTags.Creatures.Tunnel, (Instance smi) => true, null).GoTo(loop);
	}
}
