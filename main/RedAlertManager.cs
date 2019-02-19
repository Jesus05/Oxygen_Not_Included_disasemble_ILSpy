using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RedAlertManager : GameStateMachine<RedAlertManager, RedAlertManager.Instance>
{
	public new class Instance : GameInstance
	{
		private static Instance instance;

		private bool isToggled;

		private bool hasEmergencyChore;

		public Notification notification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.REDALERT.TOOLTIP, null, false, 0f, null, null, null);

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			instance = this;
		}

		public static void DestroyInstance()
		{
			instance = null;
		}

		public static Instance Get()
		{
			return instance;
		}

		public bool IsOn()
		{
			return base.sm.isOn.Get(base.smi);
		}

		public bool IsToggledOn()
		{
			return isToggled;
		}

		public void Toggle(bool on)
		{
			isToggled = on;
			Refresh();
		}

		public void HasEmergencyChore(bool on)
		{
			hasEmergencyChore = on;
			Refresh();
		}

		private void Refresh()
		{
			base.sm.isOn.Set(isToggled || hasEmergencyChore, base.smi);
		}
	}

	public State off;

	public State on;

	public State on_pst;

	public BoolParameter isOn = new BoolParameter();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		base.serializable = true;
		off.ParamTransition(isOn, on, GameStateMachine<RedAlertManager, Instance, IStateMachineTarget, object>.IsTrue);
		on.Enter("EnterEvent", delegate
		{
			Game.Instance.Trigger(1585324898, null);
		}).Exit("ExitEvent", delegate
		{
			Game.Instance.Trigger(-1393151672, null);
		}).Enter("EnableVignette", delegate
		{
			Vignette.Instance.SetColor(new Color(1f, 0f, 0f, 0.3f));
		})
			.Exit("DisableVignette", delegate
			{
				Vignette.Instance.Reset();
			})
			.Enter("Sounds", delegate
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON", false));
			})
			.ToggleLoopingSound(GlobalAssets.GetSound("RedAlert_LP", false), (Func<Instance, bool>)null)
			.ToggleNotification((Instance smi) => smi.notification)
			.ParamTransition(isOn, off, GameStateMachine<RedAlertManager, Instance, IStateMachineTarget, object>.IsFalse);
		on_pst.Enter("Sounds", delegate
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF", false));
		});
	}
}
