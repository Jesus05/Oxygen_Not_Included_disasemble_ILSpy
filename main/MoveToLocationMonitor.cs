using STRINGS;
using System;
using UnityEngine;

public class MoveToLocationMonitor : GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public int targetCell;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			master.Subscribe(493375141, OnRefreshUserMenu);
		}

		private void OnRefreshUserMenu(object data)
		{
			UserMenu userMenu = Game.Instance.userMenu;
			GameObject gameObject = base.gameObject;
			string iconName = "action_control";
			string text = UI.USERMENUACTIONS.MOVETOLOCATION.NAME;
			System.Action on_click = OnClickMoveToLocation;
			string tooltipText = UI.USERMENUACTIONS.MOVETOLOCATION.TOOLTIP;
			userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, global::Action.NumActions, null, null, null, tooltipText, true), 0.2f);
		}

		private void OnClickMoveToLocation()
		{
			MoveToLocationTool.Instance.Activate(GetComponent<Navigator>());
		}

		public void MoveToLocation(int cell)
		{
			targetCell = cell;
			base.smi.GoTo(base.smi.sm.satisfied);
			base.smi.GoTo(base.smi.sm.moving);
		}

		public override void StopSM(string reason)
		{
			base.master.Unsubscribe(493375141, OnRefreshUserMenu);
			base.StopSM(reason);
		}
	}

	public State satisfied;

	public State moving;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.DoNothing();
		moving.ToggleChore((Instance smi) => new MoveChore(smi.master, Db.Get().ChoreTypes.MoveTo, (MoveChore.StatesInstance smii) => smi.targetCell, false), satisfied);
	}
}
