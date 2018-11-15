using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RedAlertManager : GameStateMachine<RedAlertManager, RedAlertManager.Instance>
{
	public new class Instance : GameInstance
	{
		private static Instance instance;

		public Notification notification = new Notification(MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.REDALERT.TOOLTIP, null, false, 0f, null, null);

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

		public void Toggle(bool on)
		{
			base.sm.isOn.Set(on, base.smi);
		}
	}

	public State off;

	public State on;

	public BoolParameter isOn = new BoolParameter();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		base.serializable = true;
		off.ParamTransition(isOn, on, (Instance smi, bool p) => p);
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
			.ToggleNotification((Instance smi) => smi.notification)
			.ParamTransition(isOn, off, (Instance smi, bool p) => !p);
	}
}
