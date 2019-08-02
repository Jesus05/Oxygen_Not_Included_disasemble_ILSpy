using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Phonobox : StateMachineComponent<Phonobox.StatesInstance>, IEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, Phonobox>
	{
		public class OperationalStates : State
		{
			public State stopped;

			public State pre;

			public State bridge;

			public State playing;

			public State song_end;

			public State post;
		}

		public IntParameter playerCount;

		public State unoperational;

		public OperationalStates operational;

		[CompilerGenerated]
		private static Func<StatesInstance, string> _003C_003Ef__mg_0024cache0;

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
			}).ParamTransition(playerCount, operational.pre, (StatesInstance smi, int p) => p > 0).PlayAnim("on");
			operational.pre.PlayAnim("working_pre").OnAnimQueueComplete(operational.playing);
			operational.playing.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(true);
			}).ScheduleGoTo(25f, operational.song_end).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0)
				.PlayAnim(GetPlayAnim, KAnim.PlayMode.Loop);
			operational.song_end.ParamTransition(playerCount, operational.bridge, (StatesInstance smi, int p) => p > 0).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0);
			operational.bridge.PlayAnim("working_trans").OnAnimQueueComplete(operational.playing);
			operational.post.PlayAnim("working_pst").OnAnimQueueComplete(operational.stopped);
		}

		public static string GetPlayAnim(StatesInstance smi)
		{
			int num = UnityEngine.Random.Range(0, building_anims.Length);
			return building_anims[num];
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Phonobox, object>.GameInstance
	{
		private FetchChore chore;

		private Operational operational;

		public StatesInstance(Phonobox smi)
			: base(smi)
		{
			operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active, false);
		}
	}

	public const string SPECIFIC_EFFECT = "Danced";

	public const string TRACKING_EFFECT = "RecentlyDanced";

	public CellOffset[] choreOffsets = new CellOffset[5]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0),
		new CellOffset(-2, 0),
		new CellOffset(2, 0)
	};

	private PhonoboxWorkable[] workables;

	private Chore[] chores;

	private HashSet<Worker> players = new HashSet<Worker>();

	private static string[] building_anims = new string[3]
	{
		"working_loop",
		"working_loop2",
		"working_loop3"
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule, true);
		}, null, null);
		workables = new PhonoboxWorkable[choreOffsets.Length];
		chores = new Chore[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("PhonoboxWorkable", pos);
			PhonoboxWorkable phonoboxWorkable = go.AddOrGet<PhonoboxWorkable>();
			phonoboxWorkable.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.Dancing);
			phonoboxWorkable.owner = this;
			workables[i] = phonoboxWorkable;
		}
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
		Chore chore = new WorkChore<PhonoboxWorkable>(relax, target, null, true, null, null, OnSocialChoreEnd, false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
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

	public void AddWorker(Worker player)
	{
		players.Add(player);
		base.smi.sm.playerCount.Set(players.Count, base.smi);
	}

	public void RemoveWorker(Worker player)
	{
		players.Remove(player);
		base.smi.sm.playerCount.Set(players.Count, base.smi);
	}

	List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, "Danced", true);
		return list;
	}
}
