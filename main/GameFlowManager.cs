using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GameFlowManager : StateMachineComponent<GameFlowManager.StatesInstance>, ISaveLoadable
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, GameFlowManager, object>.GameInstance
	{
		public Notification colonyLostNotification = new Notification(MISC.NOTIFICATIONS.COLONYLOST.NAME, NotificationType.Bad, HashedString.Invalid, null, null, false, 0f, null, null, null);

		public StatesInstance(GameFlowManager smi)
			: base(smi)
		{
		}

		public bool IsIncapacitated(GameObject go)
		{
			return false;
		}

		public void CheckForGameOver()
		{
			if (Game.Instance.GameStarted() && !GenericGameSettings.instance.disableGameOver)
			{
				bool flag = false;
				if (Components.LiveMinionIdentities.Count == 0)
				{
					flag = true;
				}
				else
				{
					flag = true;
					foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
					{
						if (!IsIncapacitated(item.gameObject))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					GoTo(base.sm.gameover.pending);
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, GameFlowManager>
	{
		public class GameOverState : State
		{
			public State pending;

			public State active;
		}

		public State loading;

		public State running;

		public GameOverState gameover;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = loading;
			loading.ScheduleGoTo(4f, running);
			running.Update("CheckForGameOver", delegate(StatesInstance smi, float dt)
			{
				smi.CheckForGameOver();
			}, UpdateRate.SIM_200ms, false);
			gameover.TriggerOnEnter(GameHashes.GameOver, null).ToggleNotification((StatesInstance smi) => smi.colonyLostNotification);
			gameover.pending.Enter("Goto(gameover.active)", delegate(StatesInstance smi)
			{
				States states = this;
				UIScheduler.Instance.Schedule("Goto(gameover.active)", 4f, delegate
				{
					smi.GoTo(states.gameover.active);
				}, null, null);
			});
			gameover.active.Enter(delegate
			{
				if (GenericGameSettings.instance.demoMode)
				{
					DemoTimer.Instance.EndDemo();
				}
				else
				{
					GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.GameOverScreen, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<KScreen>().Show(true);
				}
			});
		}
	}

	[MyCmpAdd]
	private Notifier notifier;

	public static GameFlowManager Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public bool IsGameOver()
	{
		return base.smi.IsInsideState(base.smi.sm.gameover);
	}
}
