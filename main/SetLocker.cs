using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class SetLocker : StateMachineComponent<SetLocker.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SetLocker, object>.GameInstance
	{
		public StatesInstance(SetLocker master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SetLocker>
	{
		public State closed;

		public State open;

		public State off;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = closed;
			base.serializable = true;
			closed.PlayAnim("on").Enter(delegate(StatesInstance smi)
			{
				if (smi.master.machineSound != null)
				{
					LoopingSounds component2 = smi.master.GetComponent<LoopingSounds>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
					{
						component2.StartSound(GlobalAssets.GetSound(smi.master.machineSound, false));
					}
				}
			});
			open.PlayAnim("working").OnAnimQueueComplete(off).Exit(delegate(StatesInstance smi)
			{
				smi.master.DropContents();
			});
			off.PlayAnim("off").Enter(delegate(StatesInstance smi)
			{
				if (smi.master.machineSound != null)
				{
					LoopingSounds component = smi.master.GetComponent<LoopingSounds>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.StopSound(GlobalAssets.GetSound(smi.master.machineSound, false));
					}
				}
			});
		}
	}

	public string[] possible_contents_ids;

	public string machineSound;

	public string overrideAnim;

	public Vector2I dropOffset = Vector2I.zero;

	[Serialize]
	private string contents = string.Empty;

	private static readonly EventSystem.IntraObjectHandler<SetLocker> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SetLocker>(delegate(SetLocker component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	[Serialize]
	private bool used;

	private Chore chore;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		contents = possible_contents_ids[UnityEngine.Random.Range(0, possible_contents_ids.Length)];
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	public void DropContents()
	{
		Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), dropOffset.x, dropOffset.y, contents, Grid.SceneLayer.Front).SetActive(true);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(contents.ToTag()).GetProperName(), base.smi.master.transform, 1.5f, false);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.IsInsideState(base.smi.sm.closed) && !used)
		{
			object buttonInfo;
			if (chore != null)
			{
				string iconName = "action_empty_contents";
				string text = UI.USERMENUACTIONS.OPENPOI.NAME_OFF;
				System.Action on_click = OnClickCancel;
				string tooltipText = UI.USERMENUACTIONS.OPENPOI.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_empty_contents";
				string text = UI.USERMENUACTIONS.OPENPOI.NAME;
				System.Action on_click = OnClickOpen;
				string iconName = UI.USERMENUACTIONS.OPENPOI.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	private void OnClickOpen()
	{
		ActivateChore(null);
	}

	private void OnClickCancel()
	{
		CancelChore(null);
	}

	public void ActivateChore(object param = null)
	{
		if (chore == null)
		{
			GetComponent<Workable>().SetWorkTime(1.5f);
			chore = new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, delegate
			{
				CompleteChore();
			}, null, null, true, null, false, true, Assets.GetAnim(overrideAnim), false, true, true, PriorityScreen.PriorityClass.basic, 10, false, true);
			OnRefreshUserMenu(null);
		}
	}

	public void CancelChore(object param = null)
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
		}
	}

	private void CompleteChore()
	{
		used = true;
		base.smi.GoTo(base.smi.sm.open);
		chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}
}
