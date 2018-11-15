using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class SlimeLung : Disease
	{
		public class SlimeLungComponent : DiseaseComponent
		{
			public class StatesInstance : GameStateMachine<States, StatesInstance, DiseaseInstance, object>.GameInstance
			{
				public float lastCoughtTime;

				public StatesInstance(DiseaseInstance master)
					: base(master)
				{
				}

				public Reactable GetReactable()
				{
					return new SelfEmoteReactable(base.master.gameObject, "SlimeLungCough", Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim", 0f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
					{
						anim = (HashedString)"sneeze",
						finishcb = new Action<GameObject>(ProduceSlime)
					}).AddStep(new EmoteReactable.EmoteStep
					{
						anim = (HashedString)"sneeze_pst"
					}).AddStep(new EmoteReactable.EmoteStep
					{
						startcb = new Action<GameObject>(FinishedCoughing)
					});
				}

				private void ProduceSlime(GameObject cougher)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(cougher);
					int gameCell = Grid.PosToCell(cougher);
					SimMessages.AddRemoveSubstance(gameCell, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.Cough, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex("SlimeLung"), 1000, true, -1);
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, base.master.modifier.Name, 1000), cougher.transform, 1.5f, false);
				}

				private void FinishedCoughing(GameObject cougher)
				{
					base.sm.coughFinished.Trigger(this);
				}
			}

			public class States : GameStateMachine<States, StatesInstance, DiseaseInstance>
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
					breathing.DefaultState(breathing.normal).TagTransition(GameTags.NoOxygen, notbreathing, false).Enter("SetCoughTime", delegate(StatesInstance smi)
					{
						smi.lastCoughtTime = Time.time;
					})
						.Update("Cough", delegate(StatesInstance smi, float dt)
						{
							if (!smi.master.IsDoctored && Time.time - smi.lastCoughtTime > 20f)
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

			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
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
				list.Add(new Descriptor(DUPLICANTS.DISEASES.SLIMELUNG.COUGH_SYMPTOM, DUPLICANTS.DISEASES.SLIMELUNG.COUGH_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false));
				return list;
			}
		}

		private const float COUGH_FREQUENCY = 20f;

		private const float COUGH_MASS = 0.1f;

		private const int DISEASE_AMOUNT = 1000;

		private const float DEATH_TIMER = 4800f;

		public const string ID = "SlimeLung";

		public SlimeLung()
			: base("SlimeLung", DiseaseType.Pathogen, Severity.Critical, 0.00025f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 2400f, 1, new RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
			doctorRequired = true;
			fatalityDuration = 4800f;
			AddDiseaseComponent(new CommonSickEffectDisease());
			AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[2]
			{
				new AttributeModifier("BreathDelta", -1.13636363f, DUPLICANTS.DISEASES.SLIMELUNG.NAME, false, false, true),
				new AttributeModifier("Athletics", -3f, DUPLICANTS.DISEASES.SLIMELUNG.NAME, false, false, true)
			}));
			AddDiseaseComponent(new SlimeLungComponent());
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.66666675f),
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(500f),
				overPopulationHalfLife = new float?(1200f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(3000f),
				overPopulationHalfLife = new float?(1200f),
				diffusionScale = new float?(1E-06f),
				minDiffusionCount = new int?(1000000)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.SlimeMold)
			{
				underPopulationDeathRate = new float?(0f),
				populationHalfLife = new float?(-3000f),
				overPopulationHalfLife = new float?(3000f),
				maxCountPerKG = new float?(4500f),
				diffusionScale = new float?(0.05f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = new float?(250f),
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(1200f),
				maxCountPerKG = new float?(10000f),
				minDiffusionCount = new int?(5100),
				diffusionScale = new float?(0.005f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				underPopulationDeathRate = new float?(0f),
				populationHalfLife = new float?(-300f),
				overPopulationHalfLife = new float?(1200f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(10f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(300f),
				maxCountPerKG = new float?(100f),
				diffusionScale = new float?(0.01f)
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = new float?(float.PositiveInfinity)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.DirtyWater)
			{
				populationHalfLife = new float?(-12000f)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(-12000f)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(3000f)
			});
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}

		public override List<Descriptor> GetDiseaseSourceDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.SLIMELUNG.DISEASE_SOURCE_DESCRIPTOR, GameUtil.GetFormattedTime(20f), GameUtil.GetFormattedDiseaseAmount(1000), base.Name), string.Format(DUPLICANTS.DISEASES.SLIMELUNG.DISEASE_SOURCE_DESCRIPTOR_TOOLTIP, GameUtil.GetFormattedTime(20f), GameUtil.GetFormattedDiseaseAmount(1000), base.Name), Descriptor.DescriptorType.DiseaseSource, false));
			return list;
		}
	}
}
