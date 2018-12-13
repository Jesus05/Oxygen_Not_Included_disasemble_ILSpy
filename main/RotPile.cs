using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RotPile : StateMachineComponent<RotPile.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RotPile, object>.GameInstance
	{
		public AttributeModifier baseDecomposeRate;

		[CompilerGenerated]
		private static Func<List<Notification>, object, string> _003C_003Ef__mg_0024cache0;

		public StatesInstance(RotPile master)
			: base(master)
		{
			if (WorldInventory.Instance.IsReachable(base.smi.master.gameObject.GetComponent<Pickupable>()))
			{
				Notification notification = new Notification(MISC.NOTIFICATIONS.FOODROT.NAME, NotificationType.BadMinor, HashedString.Invalid, OnRottenTooltip, null, true, 0f, null, null)
				{
					tooltipData = master.gameObject.GetProperName()
				};
				base.gameObject.AddOrGet<Notifier>().Add(notification, string.Empty);
			}
		}

		private static string OnRottenTooltip(List<Notification> notifications, object data)
		{
			string text = "\n";
			foreach (Notification notification in notifications)
			{
				if (notification.tooltipData != null)
				{
					text = text + "\n" + (string)notification.tooltipData;
				}
			}
			return string.Format(MISC.NOTIFICATIONS.FOODROT.TOOLTIP, text);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RotPile>
	{
		public State decomposing;

		public State convertDestroy;

		public FloatParameter decompositionAmount;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = decomposing;
			base.serializable = true;
			decomposing.ParamTransition(decompositionAmount, convertDestroy, (StatesInstance smi, float p) => p >= 600f).Update("Decomposing", delegate(StatesInstance smi, float dt)
			{
				decompositionAmount.Delta(dt, smi);
			}, UpdateRate.SIM_200ms, false);
			convertDestroy.Enter(delegate(StatesInstance smi)
			{
				smi.master.ConvertToElement();
			});
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected void ConvertToElement()
	{
		PrimaryElement component = base.smi.master.GetComponent<PrimaryElement>();
		float mass = component.Mass;
		float temperature = component.Temperature;
		if (mass <= 0f)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			SimHashes hash = SimHashes.ToxicSand;
			Substance substance = ElementLoader.FindElementByHash(hash).substance;
			GameObject gameObject = substance.SpawnResource(base.smi.master.transform.GetPosition(), mass, temperature, byte.MaxValue, 0, false, false);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(hash).name, gameObject.transform, 1.5f, false);
			Util.KDestroyGameObject(base.smi.gameObject);
		}
	}
}
