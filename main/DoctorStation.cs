using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DoctorStation : Workable
{
	public class States : GameStateMachine<States, StatesInstance, DoctorStation>
	{
		public class OperationalStates : State
		{
			public State not_ready;

			public ReadyStates ready;
		}

		public class ReadyStates : State
		{
			public State idle;

			public PatientStates has_patient;
		}

		public class PatientStates : State
		{
			public State waiting;

			public State being_treated;
		}

		public State unoperational;

		public OperationalStates operational;

		public BoolParameter hasSupplies;

		public BoolParameter hasPatient;

		public BoolParameter hasDoctor;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = false;
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (StatesInstance smi) => smi.master.operational.IsOperational);
			operational.EventTransition(GameHashes.OperationalChanged, operational, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(operational.not_ready);
			operational.not_ready.ParamTransition(hasSupplies, operational.ready, (StatesInstance smi, bool p) => p);
			operational.ready.DefaultState(operational.ready.idle).ToggleRecurringChore(CreatePatientChore, null).ParamTransition(hasSupplies, operational.not_ready, (StatesInstance smi, bool p) => !p);
			operational.ready.idle.ParamTransition(hasPatient, operational.ready.has_patient, (StatesInstance smi, bool p) => p);
			operational.ready.has_patient.ParamTransition(hasPatient, operational.ready.idle, (StatesInstance smi, bool p) => !p).DefaultState(operational.ready.has_patient.waiting).ToggleRecurringChore(CreateDoctorChore, null);
			operational.ready.has_patient.waiting.ParamTransition(hasDoctor, operational.ready.has_patient.being_treated, (StatesInstance smi, bool p) => p);
			operational.ready.has_patient.being_treated.ParamTransition(hasDoctor, operational.ready.has_patient.waiting, (StatesInstance smi, bool p) => !p);
		}

		private Chore CreatePatientChore(StatesInstance smi)
		{
			WorkChore<DoctorStation> workChore = new WorkChore<DoctorStation>(Db.Get().ChoreTypes.GetDoctored, smi.master, null, true, null, null, null, false, null, false, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, true);
			workChore.AddPrecondition(TreatmentAvailable, smi.master);
			workChore.AddPrecondition(DoctorAvailable, smi.master);
			return workChore;
		}

		private Chore CreateDoctorChore(StatesInstance smi)
		{
			DoctorStationDoctorWorkable component = smi.master.GetComponent<DoctorStationDoctorWorkable>();
			return new WorkChore<DoctorStationDoctorWorkable>(Db.Get().ChoreTypes.Doctor, component, null, true, null, null, null, false, null, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, DoctorStation, object>.GameInstance
	{
		public StatesInstance(DoctorStation master)
			: base(master)
		{
		}
	}

	private static readonly EventSystem.IntraObjectHandler<DoctorStation> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<DoctorStation>(delegate(DoctorStation component, object data)
	{
		component.OnStorageChange(data);
	});

	[MyCmpReq]
	public Storage storage;

	[MyCmpReq]
	public Operational operational;

	private DoctorStationDoctorWorkable doctor_workable;

	[SerializeField]
	public Tag supplyTag;

	private Dictionary<HashedString, Tag> treatments_available = new Dictionary<HashedString, Tag>();

	private StatesInstance smi;

	public static readonly Chore.Precondition TreatmentAvailable = new Chore.Precondition
	{
		id = "TreatmentAvailable",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.TREATMENT_AVAILABLE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			DoctorStation doctorStation2 = (DoctorStation)data;
			return doctorStation2.IsTreatmentAvailable(context.consumerState.gameObject);
		}
	};

	public static readonly Chore.Precondition DoctorAvailable = new Chore.Precondition
	{
		id = "DoctorAvailable",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.DOCTOR_AVAILABLE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			DoctorStation doctorStation = (DoctorStation)data;
			return doctorStation.IsDoctorAvailable(context.consumerState.gameObject);
		}
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		doctor_workable = GetComponent<DoctorStationDoctorWorkable>();
		SetWorkTime(float.PositiveInfinity);
		smi = new StatesInstance(this);
		smi.StartSM();
		OnStorageChange(null);
		Subscribe(-1697596308, OnStorageChangeDelegate);
	}

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		if (smi != null)
		{
			smi.StopSM("OnCleanUp");
			smi = null;
		}
		base.OnCleanUp();
	}

	private void OnStorageChange(object data = null)
	{
		treatments_available.Clear();
		foreach (GameObject item in storage.items)
		{
			if (item.HasTag(GameTags.MedicalSupplies))
			{
				Tag tag = item.PrefabID();
				if (tag == (Tag)"IntermediateCure")
				{
					AddTreatment("SlimeSickness", tag);
				}
				if (tag == (Tag)"AdvancedCure")
				{
					AddTreatment("ZombieSickness", tag);
				}
			}
		}
		bool value = treatments_available.Count > 0;
		smi.sm.hasSupplies.Set(value, smi);
	}

	private void AddTreatment(string id, Tag tag)
	{
		if (!treatments_available.ContainsKey(id))
		{
			treatments_available.Add(id, tag);
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		smi.sm.hasPatient.Set(true, smi);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		smi.sm.hasPatient.Set(false, smi);
	}

	public void SetHasDoctor(bool has)
	{
		smi.sm.hasDoctor.Set(has, smi);
	}

	public void CompleteDoctoring()
	{
		if ((bool)base.worker)
		{
			CompleteDoctoring(base.worker.gameObject);
		}
	}

	private void CompleteDoctoring(GameObject target)
	{
		Sicknesses sicknesses = target.GetSicknesses();
		if (sicknesses != null)
		{
			bool flag = false;
			foreach (SicknessInstance item in sicknesses)
			{
				if (treatments_available.TryGetValue(item.Sickness.id, out Tag value))
				{
					item.Cure();
					storage.ConsumeIgnoringDisease(value, 1f);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarningFormat(base.gameObject, "Failed to treat any disease for {0}", target);
			}
		}
	}

	public bool IsDoctorAvailable(GameObject target)
	{
		if (!string.IsNullOrEmpty(doctor_workable.requiredSkillPerk))
		{
			MinionResume component = target.GetComponent<MinionResume>();
			if (!MinionResume.AnyOtherMinionHasPerk(doctor_workable.requiredSkillPerk, component))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsTreatmentAvailable(GameObject target)
	{
		Sicknesses sicknesses = target.GetSicknesses();
		if (sicknesses != null)
		{
			foreach (SicknessInstance item in sicknesses)
			{
				if (treatments_available.TryGetValue(item.Sickness.id, out Tag _))
				{
					return true;
				}
			}
		}
		return false;
	}
}
