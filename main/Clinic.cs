using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Clinic : Workable, IEffectDescriptor
{
	public class ClinicSM : GameStateMachine<ClinicSM, ClinicSM.Instance, Clinic>
	{
		public class OperationalStates : State
		{
			public State idle;

			public HealingStates healing;
		}

		public class HealingStates : State
		{
			public State undoctored;

			public State doctored;

			public State newlyDoctored;
		}

		public new class Instance : GameInstance
		{
			private WorkChore<DoctorChoreWorkable> doctorChore;

			public Instance(Clinic master)
				: base(master)
			{
			}

			public void StartDoctorChore()
			{
				if (base.master.IsValidEffect(base.master.doctoredHealthEffect) || base.master.IsValidEffect(base.master.doctoredDiseaseEffect))
				{
					doctorChore = new WorkChore<DoctorChoreWorkable>(Db.Get().ChoreTypes.Doctor, base.smi.master, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
					WorkChore<DoctorChoreWorkable> workChore = doctorChore;
					workChore.onComplete = (Action<Chore>)Delegate.Combine(workChore.onComplete, (Action<Chore>)delegate
					{
						base.smi.GoTo(base.smi.sm.operational.healing.newlyDoctored);
					});
				}
			}

			public void StopDoctorChore()
			{
				if (doctorChore != null)
				{
					doctorChore.Cancel("StopDoctorChore");
					doctorChore = null;
				}
			}

			public bool HasEffect(string effect)
			{
				bool result = false;
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					Effects component = worker.GetComponent<Effects>();
					result = component.HasEffect(effect);
				}
				return result;
			}

			public EffectInstance StartEffect(string effect, bool should_save)
			{
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					if ((UnityEngine.Object)worker != (UnityEngine.Object)null)
					{
						Effects component = worker.GetComponent<Effects>();
						if (!component.HasEffect(effect))
						{
							return component.Add(effect, should_save);
						}
					}
				}
				return null;
			}

			public void StopEffect(string effect)
			{
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					if ((UnityEngine.Object)worker != (UnityEngine.Object)null)
					{
						Effects component = worker.GetComponent<Effects>();
						if (component.HasEffect(effect))
						{
							component.Remove(effect);
						}
					}
				}
			}
		}

		public State unoperational;

		public OperationalStates operational;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = false;
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(Instance smi)
			{
				Assignable component4 = smi.master.GetComponent<Assignable>();
				component4.Unassign();
			});
			operational.DefaultState(operational.idle).EventTransition(GameHashes.OperationalChanged, unoperational, (Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.AssigneeChanged, unoperational, null)
				.ToggleRecurringChore((Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.Heal, smi.master, null, null, true, null, null, null, false, null, false, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.HealCritical, smi.master, null, null, true, null, null, null, false, null, false, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.RestDueToDisease, smi.master, null, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect))
				.ToggleRecurringChore((Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.SleepDueToDisease, smi.master, null, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect));
			operational.idle.WorkableStartTransition((Instance smi) => smi.master, operational.healing);
			operational.healing.DefaultState(operational.healing.undoctored).WorkableStopTransition((Instance smi) => smi.GetComponent<Clinic>(), operational.idle).Enter(delegate(Instance smi)
			{
				smi.master.GetComponent<Operational>().SetActive(true, false);
			})
				.Exit(delegate(Instance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(false, false);
				});
			operational.healing.undoctored.Enter(delegate(Instance smi)
			{
				smi.StartEffect(smi.master.healthEffect, false);
				smi.StartEffect(smi.master.diseaseEffect, false);
				bool flag = false;
				Worker worker2 = smi.master.worker;
				if ((UnityEngine.Object)worker2 != (UnityEngine.Object)null)
				{
					flag = (smi.HasEffect(smi.master.doctoredHealthEffect) || smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredPlaceholderEffect));
				}
				if (smi.master.AllowDoctoring())
				{
					if (flag)
					{
						smi.GoTo(operational.healing.doctored);
					}
					else
					{
						smi.StartDoctorChore();
					}
				}
			}).Exit(delegate(Instance smi)
			{
				smi.StopEffect(smi.master.healthEffect);
				smi.StopEffect(smi.master.diseaseEffect);
				smi.StopDoctorChore();
			});
			operational.healing.newlyDoctored.Enter(delegate(Instance smi)
			{
				smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
				smi.StartEffect(smi.master.doctoredHealthEffect, true);
				smi.GoTo(operational.healing.doctored);
			});
			operational.healing.doctored.Enter(delegate(Instance smi)
			{
				Effects component3 = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredPlaceholderEffect))
				{
					EffectInstance effectInstance4 = component3.Get(smi.master.doctoredPlaceholderEffect);
					EffectInstance effectInstance5 = smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
					if (effectInstance5 != null)
					{
						float num2 = effectInstance4.effect.duration - effectInstance4.timeRemaining;
						effectInstance5.timeRemaining = effectInstance5.effect.duration - num2;
					}
					EffectInstance effectInstance6 = smi.StartEffect(smi.master.doctoredHealthEffect, true);
					if (effectInstance6 != null)
					{
						float num3 = effectInstance4.effect.duration - effectInstance4.timeRemaining;
						effectInstance6.timeRemaining = effectInstance6.effect.duration - num3;
					}
					component3.Remove(smi.master.doctoredPlaceholderEffect);
				}
			}).ScheduleGoTo(delegate(Instance smi)
			{
				Worker worker = smi.master.worker;
				Effects component2 = worker.GetComponent<Effects>();
				float num = smi.master.doctorVisitInterval;
				if (smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance3 = component2.Get(smi.master.doctoredHealthEffect);
					num = Mathf.Min(num, effectInstance3.GetTimeRemaining());
				}
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect))
				{
					EffectInstance effectInstance3 = component2.Get(smi.master.doctoredDiseaseEffect);
					num = Mathf.Min(num, effectInstance3.GetTimeRemaining());
				}
				return num;
			}, operational.healing.undoctored).Exit(delegate(Instance smi)
			{
				Effects component = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance = component.Get(smi.master.doctoredDiseaseEffect);
					if (effectInstance == null)
					{
						effectInstance = component.Get(smi.master.doctoredHealthEffect);
					}
					EffectInstance effectInstance2 = smi.StartEffect(smi.master.doctoredPlaceholderEffect, true);
					effectInstance2.timeRemaining = effectInstance2.effect.duration - (effectInstance.effect.duration - effectInstance.timeRemaining);
					component.Remove(smi.master.doctoredDiseaseEffect);
					component.Remove(smi.master.doctoredHealthEffect);
				}
			});
		}
	}

	[MyCmpReq]
	private Assignable assignable;

	private static readonly string[] EffectsRemoved = new string[1]
	{
		"SoreBack"
	};

	private const int MAX_RANGE = 10;

	private const float CHECK_RANGE_INTERVAL = 10f;

	public float doctorVisitInterval = 300f;

	public KAnimFile[] workerInjuredAnims;

	public KAnimFile[] workerDiseasedAnims;

	public string diseaseEffect;

	public string healthEffect;

	public string doctoredDiseaseEffect;

	public string doctoredHealthEffect;

	public string doctoredPlaceholderEffect;

	private ClinicSM.Instance clinicSMI;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = false;
		assignable.subSlots = new AssignableSlot[1]
		{
			Db.Get().AssignableSlots.MedicalBed
		};
		assignable.AddAutoassignPrecondition(CanAutoAssignTo);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		Components.Clinics.Add(this);
		SetWorkTime(float.PositiveInfinity);
		clinicSMI = new ClinicSM.Instance(this);
		clinicSMI.StartSM();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		Components.Clinics.Remove(this);
		base.OnCleanUp();
	}

	private KAnimFile[] GetAppropriateOverrideAnims(Worker worker)
	{
		KAnimFile[] result = null;
		if (!worker.GetSMI<WoundMonitor.Instance>().ShouldExitInfirmary())
		{
			result = workerInjuredAnims;
		}
		else if (workerDiseasedAnims != null && IsValidEffect(diseaseEffect) && worker.GetSMI<DiseaseMonitor.Instance>().IsSick())
		{
			result = workerDiseasedAnims;
		}
		return result;
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		overrideAnims = GetAppropriateOverrideAnims(worker);
		return base.GetAnim(worker);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("Sleep", false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		KAnimFile[] appropriateOverrideAnims = GetAppropriateOverrideAnims(worker);
		if (appropriateOverrideAnims == null || appropriateOverrideAnims != overrideAnims)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove("Sleep");
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		assignable.Unassign();
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < EffectsRemoved.Length; i++)
		{
			string effect_id = EffectsRemoved[i];
			component.Remove(effect_id);
		}
	}

	private bool CanAutoAssignTo(MinionIdentity worker)
	{
		bool flag = false;
		if (IsValidEffect(healthEffect))
		{
			Health component = worker.GetComponent<Health>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.hitPoints < component.maxHitPoints)
			{
				flag = true;
			}
		}
		if (!flag && IsValidEffect(diseaseEffect))
		{
			MinionModifiers component2 = worker.GetComponent<MinionModifiers>();
			Diseases diseases = component2.diseases;
			flag = (diseases.Count > 0);
		}
		return flag;
	}

	private bool IsValidEffect(string effect)
	{
		return effect != null && effect != string.Empty;
	}

	private bool AllowDoctoring()
	{
		return IsValidEffect(doctoredDiseaseEffect) || IsValidEffect(doctoredHealthEffect);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (IsValidEffect(healthEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, list, healthEffect, false);
		}
		if (diseaseEffect != healthEffect && IsValidEffect(diseaseEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, list, diseaseEffect, false);
		}
		if (AllowDoctoring())
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.DOCTORING, UI.BUILDINGEFFECTS.TOOLTIPS.DOCTORING, Descriptor.DescriptorType.Effect);
			list.Add(item);
			if (IsValidEffect(doctoredHealthEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, list, doctoredHealthEffect, true);
			}
			if (doctoredDiseaseEffect != doctoredHealthEffect && IsValidEffect(doctoredDiseaseEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, list, doctoredDiseaseEffect, true);
			}
		}
		return list;
	}
}
