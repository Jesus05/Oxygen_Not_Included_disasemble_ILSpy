using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class SlimeSickness : Sickness
	{
		public class SlimeLungComponent : SicknessComponent
		{
			public class StatesInstance : GameStateMachine<States, StatesInstance, SicknessInstance, object>.GameInstance
			{
				public float lastCoughTime;

				public StatesInstance(SicknessInstance master)
					: base(master)
				{
				}

				public Reactable GetReactable()
				{
					return new SelfEmoteReactable(base.master.gameObject, "SlimeLungCough", Db.Get().ChoreTypes.Cough, "anim_slimelungcough_kanim", 0f, 0f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
					{
						anim = (HashedString)"react",
						finishcb = new Action<GameObject>(ProduceSlime)
					}).AddStep(new EmoteReactable.EmoteStep
					{
						startcb = new Action<GameObject>(FinishedCoughing)
					});
				}

				private void ProduceSlime(GameObject cougher)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(cougher);
					int gameCell = Grid.PosToCell(cougher);
					string id = Db.Get().Diseases.SlimeGerms.Id;
					Equippable equippable = base.master.gameObject.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
					if ((UnityEngine.Object)equippable != (UnityEngine.Object)null)
					{
						equippable.GetComponent<Storage>().AddGasChunk(SimHashes.ContaminatedOxygen, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex(id), 1000, false, true);
					}
					else
					{
						SimMessages.AddRemoveSubstance(gameCell, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.Cough, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex(id), 1000, true, -1);
					}
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, base.master.modifier.Name, 1000), cougher.transform, 1.5f, false);
				}

				private void FinishedCoughing(GameObject cougher)
				{
					base.sm.coughFinished.Trigger(this);
				}
			}

			public class States : GameStateMachine<States, StatesInstance, SicknessInstance>
			{
				public class BreathingStates : State
				{
					public State normal;

					public State cough;
				}

				public Signal coughFinished;

				public BreathingStates breathing;

				public State notbreathing;

				public override void InitializeStates(out BaseState default_state)
				{
					default_state = breathing;
					breathing.DefaultState(breathing.normal).TagTransition(GameTags.NoOxygen, notbreathing, false);
					breathing.normal.Enter("SetCoughTime", delegate(StatesInstance smi)
					{
						if (smi.lastCoughTime < Time.time)
						{
							smi.lastCoughTime = Time.time;
						}
					}).Update("Cough", delegate(StatesInstance smi, float dt)
					{
						if (!smi.master.IsDoctored && Time.time - smi.lastCoughTime > 20f)
						{
							smi.GoTo(breathing.cough);
						}
					}, UpdateRate.SIM_4000ms, false);
					breathing.cough.ToggleReactable((StatesInstance smi) => smi.GetReactable()).OnSignal(coughFinished, breathing.normal);
					notbreathing.TagTransition(new Tag[1]
					{
						GameTags.NoOxygen
					}, breathing, true);
				}
			}

			public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
			{
				StatesInstance statesInstance = new StatesInstance(diseaseInstance);
				statesInstance.StartSM();
				return statesInstance;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				StatesInstance statesInstance = (StatesInstance)instance_data;
				statesInstance.StopSM("Cured");
			}

			public override List<Descriptor> GetSymptoms()
			{
				List<Descriptor> list = new List<Descriptor>();
				list.Add(new Descriptor(DUPLICANTS.DISEASES.SLIMESICKNESS.COUGH_SYMPTOM, DUPLICANTS.DISEASES.SLIMESICKNESS.COUGH_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false));
				return list;
			}
		}

		private const float COUGH_FREQUENCY = 20f;

		private const float COUGH_MASS = 0.1f;

		private const int DISEASE_AMOUNT = 1000;

		private const float DEATH_TIMER = 4800f;

		public const string ID = "SlimeSickness";

		public SlimeSickness()
			: base("SlimeSickness", SicknessType.Pathogen, Severity.Minor, 0.00025f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 2220f)
		{
			AddSicknessComponent(new CommonSickEffectSickness());
			AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[2]
			{
				new AttributeModifier("BreathDelta", -1.13636363f, DUPLICANTS.DISEASES.SLIMESICKNESS.NAME, false, false, true),
				new AttributeModifier("Athletics", -3f, DUPLICANTS.DISEASES.SLIMESICKNESS.NAME, false, false, true)
			}));
			AddSicknessComponent(new AnimatedSickness(new HashedString[1]
			{
				"anim_idle_sick_kanim"
			}, Db.Get().Expressions.Sick));
			AddSicknessComponent(new PeriodicEmoteSickness("anim_idle_sick_kanim", new HashedString[3]
			{
				"idle_pre",
				"idle_default",
				"idle_pst"
			}, 50f));
			AddSicknessComponent(new SlimeLungComponent());
		}
	}
}
