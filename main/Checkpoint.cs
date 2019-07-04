using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Checkpoint : StateMachineComponent<Checkpoint.SMInstance>
{
	private class CheckpointReactable : Reactable
	{
		private Checkpoint checkpoint;

		private Navigator reactor_navigator;

		private bool rotated = false;

		public CheckpointReactable(Checkpoint checkpoint)
			: base(checkpoint.gameObject, "CheckpointReactable", Db.Get().ChoreTypes.Checkpoint, 1, 1, false, 0f, 0f, float.PositiveInfinity)
		{
			this.checkpoint = checkpoint;
			rotated = gameObject.GetComponent<Rotatable>().IsRotated;
			preventChoreInterruption = false;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (!((UnityEngine.Object)reactor != (UnityEngine.Object)null))
			{
				if (!((UnityEngine.Object)checkpoint == (UnityEngine.Object)null))
				{
					if (checkpoint.RedLight)
					{
						if (!rotated)
						{
							return transition.x > 0;
						}
						return transition.x < 0;
					}
					return false;
				}
				Cleanup();
				return false;
			}
			return false;
		}

		protected override void InternalBegin()
		{
			reactor_navigator = reactor.GetComponent<Navigator>();
			KBatchedAnimController component = reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"), 1f);
			component.Play("idle_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
			checkpoint.OrphanReactable();
			checkpoint.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			if ((UnityEngine.Object)checkpoint == (UnityEngine.Object)null || !checkpoint.RedLight || (UnityEngine.Object)reactor_navigator == (UnityEngine.Object)null)
			{
				Cleanup();
			}
			else
			{
				reactor_navigator.AdvancePath(false);
				if (!reactor_navigator.path.IsValid())
				{
					Cleanup();
				}
				else
				{
					NavGrid.Transition nextTransition = reactor_navigator.GetNextTransition();
					if (!((!rotated) ? (nextTransition.x > 0) : (nextTransition.x < 0)))
					{
						Cleanup();
					}
				}
			}
		}

		protected override void InternalEnd()
		{
			if ((UnityEngine.Object)reactor != (UnityEngine.Object)null)
			{
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"));
			}
		}

		protected override void InternalCleanup()
		{
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, Checkpoint, object>.GameInstance
	{
		public SMInstance(Checkpoint master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, Checkpoint>
	{
		public BoolParameter redLight;

		public State stop;

		public State go;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = go;
			root.Update("RefreshLight", delegate(SMInstance smi, float dt)
			{
				smi.master.RefreshLight();
			}, UpdateRate.SIM_200ms, false);
			stop.ParamTransition(redLight, go, GameStateMachine<States, SMInstance, Checkpoint, object>.IsFalse).PlayAnim("red_light");
			go.ParamTransition(redLight, stop, GameStateMachine<States, SMInstance, Checkpoint, object>.IsTrue).PlayAnim("green_light");
		}
	}

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private KSelectable selectable;

	private static StatusItem infoStatusItem_Logic;

	private CheckpointReactable reactable;

	public static readonly HashedString PORT_ID = "Checkpoint";

	private bool hasLogicWire;

	private bool hasInputHigh;

	private bool redLight;

	private bool statusDirty = true;

	private static readonly EventSystem.IntraObjectHandler<Checkpoint> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Checkpoint>(delegate(Checkpoint component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Checkpoint> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Checkpoint>(delegate(Checkpoint component, object data)
	{
		component.OnOperationalChanged(data);
	});

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache0;

	private bool RedLightDesiredState => hasLogicWire && !hasInputHigh && operational.IsOperational;

	public bool RedLight => redLight;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		base.smi.StartSM();
		if (infoStatusItem_Logic == null)
		{
			infoStatusItem_Logic = new StatusItem("CheckpointLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			infoStatusItem_Logic.resolveStringCallback = ResolveInfoStatusItem_Logic;
		}
		Refresh(redLight);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		ClearReactable();
	}

	public void RefreshLight()
	{
		if (redLight != RedLightDesiredState)
		{
			Refresh(RedLightDesiredState);
			statusDirty = true;
		}
		if (statusDirty)
		{
			RefreshStatusItem();
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private static string ResolveInfoStatusItem_Logic(string format_str, object data)
	{
		Checkpoint checkpoint = (Checkpoint)data;
		return (!checkpoint.RedLight) ? BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_OPEN : BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_CLOSED;
	}

	private void CreateNewReactable()
	{
		if (reactable == null)
		{
			reactable = new CheckpointReactable(this);
		}
	}

	private void OrphanReactable()
	{
		reactable = null;
	}

	private void ClearReactable()
	{
		if (reactable != null)
		{
			reactable.Cleanup();
			reactable = null;
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			hasInputHigh = (logicValueChanged.newValue > 0);
			hasLogicWire = (GetNetwork() != null);
			statusDirty = true;
		}
	}

	private void OnOperationalChanged(object data)
	{
		statusDirty = true;
	}

	private void RefreshStatusItem()
	{
		bool on = operational.IsOperational && hasLogicWire;
		selectable.ToggleStatusItem(infoStatusItem_Logic, on, this);
		statusDirty = false;
	}

	private void Refresh(bool redLightState)
	{
		redLight = redLightState;
		operational.SetActive(operational.IsOperational && redLight, false);
		base.smi.sm.redLight.Set(redLight, base.smi);
		if (redLight)
		{
			CreateNewReactable();
		}
		else
		{
			ClearReactable();
		}
	}
}
