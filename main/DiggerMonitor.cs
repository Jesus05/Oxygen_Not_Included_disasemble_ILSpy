using KSerialization;
using ProcGen;
using System;
using UnityEngine;

public class DiggerMonitor : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>
{
	public class Def : BaseDef
	{
		public int depthToDig
		{
			get;
			set;
		}
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public int lastDigCell = -1;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
			master.Subscribe(387220196, OnDestinationReached);
			master.Subscribe(-766531887, OnDestinationReached);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			World instance = World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(OnSolidChanged));
			base.master.Unsubscribe(387220196, OnDestinationReached);
			base.master.Unsubscribe(-766531887, OnDestinationReached);
		}

		private void OnDestinationReached(object data)
		{
			CheckInSolid();
		}

		private void CheckInSolid()
		{
			Navigator component = base.gameObject.GetComponent<Navigator>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				int cell = Grid.PosToCell(base.gameObject);
				if (component.CurrentNavType != NavType.Solid && Grid.IsSolidCell(cell))
				{
					component.SetCurrentNavType(NavType.Solid);
				}
				else if (component.CurrentNavType == NavType.Solid && !Grid.IsSolidCell(cell))
				{
					component.SetCurrentNavType(NavType.Floor);
					base.gameObject.AddTag(GameTags.Creatures.Falling);
				}
			}
		}

		private void OnSolidChanged(int cell)
		{
			CheckInSolid();
		}

		public bool CanTunnel()
		{
			int num = Grid.PosToCell(this);
			SubWorld.ZoneType subWorldZoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(num);
			if (subWorldZoneType == SubWorld.ZoneType.Space)
			{
				int num2 = num;
				while (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					num2 = Grid.CellAbove(num2);
				}
				if (!Grid.IsValidCell(num2))
				{
					return FoundValidDigCell();
				}
			}
			return false;
		}

		private bool FoundValidDigCell()
		{
			int num = base.smi.def.depthToDig;
			int num2 = lastDigCell = Grid.PosToCell(base.smi.master.gameObject);
			int cell = Grid.CellBelow(num2);
			while (IsValidDigCell(cell, null) && num > 0)
			{
				cell = Grid.CellBelow(cell);
				num--;
			}
			if (num > 0)
			{
				cell = GameUtil.FloodFillFind<object>(IsValidDigCell, null, num2, base.smi.def.depthToDig, false, true);
			}
			lastDigCell = cell;
			return lastDigCell != -1;
		}

		private bool IsValidDigCell(int cell, object arg = null)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell])
			{
				if (!Grid.HasDoor[cell] && !Grid.Foundation[cell])
				{
					byte index = Grid.ElementIdx[cell];
					Element element = ElementLoader.elements[index];
					return Grid.Element[cell].hardness < 150 && !element.HasTag(GameTags.RefinedMetal);
				}
				GameObject gameObject = Grid.Objects[cell, 1];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					return Grid.Element[cell].hardness < 150 && !component.Element.HasTag(GameTags.RefinedMetal);
				}
			}
			return false;
		}
	}

	public State loop;

	public State dig;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		loop.EventTransition(GameHashes.BeginMeteorBombardment, (Instance smi) => Game.Instance, dig, (Instance smi) => smi.CanTunnel());
		dig.ToggleBehaviour(GameTags.Creatures.Tunnel, (Instance smi) => true, null).GoTo(loop);
	}
}
