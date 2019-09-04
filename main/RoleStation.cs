using System;
using System.Collections;
using System.Collections.Generic;

public class RoleStation : Workable, IEffectDescriptor
{
	public class RoleStationSM : GameStateMachine<RoleStationSM, RoleStationSM.Instance, RoleStation>
	{
		public new class Instance : GameInstance
		{
			public Instance(RoleStation master)
				: base(master)
			{
			}
		}

		public State unoperational;

		public State operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
			operational.ToggleChore((Instance smi) => smi.master.CreateWorkChore(), unoperational);
		}
	}

	private Chore chore;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Operational operational;

	private RoleStationSM.Instance smi;

	private Guid skillPointAvailableStatusItem;

	private List<int> subscriptions = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<RoleStation> OnUpdateDelegate = new EventSystem.IntraObjectHandler<RoleStation>(delegate(RoleStation component, object data)
	{
		component.UpdateSkillPointAvailableStatusItem(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.RoleStations.Add(this);
		smi = new RoleStationSM.Instance(this);
		smi.StartSM();
		SetWorkTime(7.53f);
		resetProgressOnStop = true;
		subscriptions.Add(Subscribe(-1523247426, OnUpdateDelegate));
		subscriptions.Add(Subscribe(1505456302, OnUpdateDelegate));
		UpdateSkillPointAvailableStatusItem(null);
	}

	protected override void OnStopWork(Worker worker)
	{
		Telepad.StatesInstance sMI = this.GetSMI<Telepad.StatesInstance>();
		sMI.sm.idlePortal.Trigger(sMI);
	}

	private void UpdateSkillPointAvailableStatusItem(object data = null)
	{
		IEnumerator enumerator = Components.MinionResumes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MinionResume minionResume = (MinionResume)enumerator.Current;
				if (minionResume.TotalSkillPointsGained - minionResume.SkillsMastered > 0)
				{
					if (skillPointAvailableStatusItem == Guid.Empty)
					{
						skillPointAvailableStatusItem = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable, null);
					}
					return;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable, false);
		skillPointAvailableStatusItem = Guid.Empty;
	}

	private Chore CreateWorkChore()
	{
		ChoreType learnSkill = Db.Get().ChoreTypes.LearnSkill;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		return new WorkChore<RoleStation>(learnSkill, this, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		worker.GetComponent<MinionResume>().SkillLearned();
	}

	private void OnSelectRolesClick()
	{
		DetailsScreen.Instance.Show(false);
		ManagementMenu.Instance.ToggleSkills();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (int subscription in subscriptions)
		{
			Game.Instance.Unsubscribe(subscription);
		}
		Components.RoleStations.Remove(this);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return new List<Descriptor>();
	}
}
