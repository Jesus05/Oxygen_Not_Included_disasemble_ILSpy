using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class EspressoMachine : StateMachineComponent<EspressoMachine.StatesInstance>, IEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, EspressoMachine>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;

			public State post;
		}

		private State unoperational;

		private State operational;

		private ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational, false);
			operational.PlayAnim("off").TagTransition(GameTags.Operational, unoperational, true).Transition(ready, IsReady, UpdateRate.SIM_200ms)
				.EventTransition(GameHashes.OnStorageChange, ready, IsReady);
			ready.TagTransition(GameTags.Operational, unoperational, true).DefaultState(ready.idle).ToggleChore(CreateChore, operational);
			ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<EspressoMachineWorkable>(), ready.working).Transition(operational, GameStateMachine<States, StatesInstance, EspressoMachine, object>.Not(IsReady), UpdateRate.SIM_200ms)
				.EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<States, StatesInstance, EspressoMachine, object>.Not(IsReady));
			ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<EspressoMachineWorkable>(), ready.post);
			ready.post.PlayAnim("working_pst").OnAnimQueueComplete(ready);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<EspressoMachineWorkable>();
			ChoreType relax = Db.Get().ChoreTypes.Relax;
			Workable target = component;
			ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
			Chore chore = new WorkChore<EspressoMachineWorkable>(relax, target, null, null, true, null, null, null, false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false);
			chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return chore;
		}

		private bool IsReady(StatesInstance smi)
		{
			PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
			if (!((Object)primaryElement == (Object)null))
			{
				if (!(primaryElement.Mass < WATER_MASS_PER_USE))
				{
					float amountAvailable = smi.GetComponent<Storage>().GetAmountAvailable(INGREDIENT_TAG);
					if (!(amountAvailable < INGREDIENT_MASS_PER_USE))
					{
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, EspressoMachine, object>.GameInstance
	{
		public StatesInstance(EspressoMachine smi)
			: base(smi)
		{
		}
	}

	public const string SPECIFIC_EFFECT = "Espresso";

	public const string TRACKING_EFFECT = "RecentlyEspresso";

	public static Tag INGREDIENT_TAG = new Tag("SpiceNut");

	public static float INGREDIENT_MASS_PER_USE = 1f;

	public static float WATER_MASS_PER_USE = 1f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		}, null, null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
	{
		string arg = tag.ProperName();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		descs.Add(item);
	}

	List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, "Espresso", true);
		AddRequirementDesc(list, INGREDIENT_TAG, INGREDIENT_MASS_PER_USE);
		AddRequirementDesc(list, GameTags.Water, WATER_MASS_PER_USE);
		return list;
	}
}
