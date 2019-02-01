using System.Collections.Generic;
using UnityEngine;

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

	private RolesScreen rolesScreen;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Operational operational;

	private RoleStationSM.Instance smi;

	private static readonly EventSystem.IntraObjectHandler<RoleStation> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<RoleStation>(delegate(RoleStation component, object data)
	{
		component.OnSelectObject(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1503271301, OnSelectObjectDelegate);
		Components.RoleStations.Add(this);
		smi = new RoleStationSM.Instance(this);
		smi.StartSM();
		SetWorkTime(2f);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	private Chore CreateWorkChore()
	{
		ChoreType switchRole = Db.Get().ChoreTypes.SwitchRole;
		KAnimFile anim = Assets.GetAnim("anim_hat_kanim");
		return new WorkChore<RoleStation>(switchRole, this, null, null, true, null, null, null, false, null, false, true, anim, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		new PutOnHatChore(worker, Db.Get().ChoreTypes.SwitchHat);
	}

	private void ClearRolesScreen()
	{
		if ((Object)rolesScreen != (Object)null)
		{
			rolesScreen.Deactivate();
			rolesScreen = null;
		}
	}

	private void OnSelectRolesClick()
	{
		DetailsScreen.Instance.Show(false);
		if ((Object)rolesScreen == (Object)null)
		{
			ManagementMenu.Instance.ToggleRoles();
		}
		else
		{
			ClearRolesScreen();
		}
	}

	private void OnSelectObject(object data)
	{
		ClearRolesScreen();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.RoleStations.Remove(this);
		ClearRolesScreen();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return new List<Descriptor>();
	}
}
