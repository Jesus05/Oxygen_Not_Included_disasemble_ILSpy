using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ArcadeMachine : StateMachineComponent<ArcadeMachine.StatesInstance>, IEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, ArcadeMachine>
	{
		public class OperationalStates : State
		{
			public State stopped;

			public State pre;

			public State playing;

			public State playing_coop;

			public State post;
		}

		public IntParameter playerCount;

		public State unoperational;

		public OperationalStates operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(false);
			}).TagTransition(GameTags.Operational, operational, false).PlayAnim("off");
			operational.TagTransition(GameTags.Operational, unoperational, true).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.master.UpdateChores(true);
			}).Exit("CancelChore", delegate(StatesInstance smi)
			{
				smi.master.UpdateChores(false);
			})
				.DefaultState(operational.stopped);
			operational.stopped.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(false);
			}).PlayAnim("on").ParamTransition(playerCount, operational.pre, (StatesInstance smi, int p) => p > 0);
			operational.pre.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(true);
			}).PlayAnim("working_pre").OnAnimQueueComplete(operational.playing);
			operational.playing.PlayAnim(GetPlayingAnim, KAnim.PlayMode.Loop).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0).ParamTransition(playerCount, operational.playing_coop, (StatesInstance smi, int p) => p > 1);
			operational.playing_coop.PlayAnim(GetPlayingAnim, KAnim.PlayMode.Loop).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0).ParamTransition(playerCount, operational.playing, (StatesInstance smi, int p) => p == 1);
			operational.post.PlayAnim("working_pst").OnAnimQueueComplete(operational.stopped);
		}

		private string GetPlayingAnim(StatesInstance smi)
		{
			bool flag = smi.master.players.Contains(0);
			bool flag2 = smi.master.players.Contains(1);
			if (flag && !flag2)
			{
				return "working_loop_one_p";
			}
			if (flag2 && !flag)
			{
				return "working_loop_two_p";
			}
			return "working_loop_coop_p";
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, ArcadeMachine, object>.GameInstance
	{
		private Operational operational;

		public StatesInstance(ArcadeMachine smi)
			: base(smi)
		{
			operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active, false);
		}
	}

	public const string SPECIFIC_EFFECT = "PlayedArcade";

	public const string TRACKING_EFFECT = "RecentlyPlayedArcade";

	public CellOffset[] choreOffsets = new CellOffset[2]
	{
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private ArcadeMachineWorkable[] workables;

	private Chore[] chores;

	public HashSet<int> players = new HashSet<int>();

	public KAnimFile[][] overrideAnims = new KAnimFile[2][]
	{
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_arcade_cabinet_playerone_kanim")
		},
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_arcade_cabinet_playertwo_kanim")
		}
	};

	public HashedString[][] workAnims = new HashedString[2][]
	{
		new HashedString[2]
		{
			"working_pre",
			"working_loop_one_p"
		},
		new HashedString[2]
		{
			"working_pre",
			"working_loop_two_p"
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		}, null, null);
		workables = new ArcadeMachineWorkable[choreOffsets.Length];
		chores = new Chore[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("ArcadeMachineWorkable", pos);
			ArcadeMachineWorkable arcadeMachineWorkable = go.AddOrGet<ArcadeMachineWorkable>();
			arcadeMachineWorkable.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.Gaming);
			int player_index = i;
			ArcadeMachineWorkable arcadeMachineWorkable2 = arcadeMachineWorkable;
			arcadeMachineWorkable2.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(arcadeMachineWorkable2.OnWorkableEventCB, (Action<Workable.WorkableEvent>)delegate(Workable.WorkableEvent ev)
			{
				OnWorkableEvent(player_index, ev);
			});
			arcadeMachineWorkable.overrideAnims = overrideAnims[i];
			arcadeMachineWorkable.workAnims = workAnims[i];
			workables[i] = arcadeMachineWorkable;
			workables[i].owner = this;
		}
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		UpdateChores(false);
		for (int i = 0; i < workables.Length; i++)
		{
			if ((bool)workables[i])
			{
				Util.KDestroyGameObject(workables[i]);
				workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = workables[i];
		ChoreType relax = Db.Get().ChoreTypes.Relax;
		Workable target = workable;
		ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
		Chore chore = new WorkChore<ArcadeMachineWorkable>(relax, target, null, true, null, null, OnSocialChoreEnd, false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
		chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return chore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.gameObject.HasTag(GameTags.Operational))
		{
			UpdateChores(true);
		}
	}

	public void UpdateChores(bool update = true)
	{
		for (int i = 0; i < choreOffsets.Length; i++)
		{
			Chore chore = chores[i];
			if (update)
			{
				if (chore == null || chore.isComplete)
				{
					chores[i] = CreateChore(i);
				}
			}
			else if (chore != null)
			{
				chore.Cancel("locator invalidated");
				chores[i] = null;
			}
		}
	}

	public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
	{
		if (ev == Workable.WorkableEvent.WorkStarted)
		{
			players.Add(player);
		}
		else
		{
			players.Remove(player);
		}
		base.smi.sm.playerCount.Set(players.Count, base.smi);
	}

	List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, "PlayedArcade", true);
		return list;
	}
}
