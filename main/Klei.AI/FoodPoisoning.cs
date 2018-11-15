using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class FoodPoisoning : Disease
	{
		private class FoodPoisoningComponent : DiseaseComponent
		{
			private class InstanceData
			{
				private GameObject go;

				private DiseaseInstance diseaseInstance;

				private Chore chore;

				private SchedulerHandle vomitHandle;

				public Notification vomiting = new Notification(DUPLICANTS.STATUSITEMS.VOMITING.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.VOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null);

				public InstanceData(GameObject go, DiseaseInstance diseaseInstance)
				{
					this.go = go;
					this.diseaseInstance = diseaseInstance;
				}

				public void StartChore()
				{
					ChoreProvider chore_provider = go.GetComponent<ChoreProvider>();
					vomitHandle = GameScheduler.Instance.Schedule("Vomit", 200f, delegate
					{
						if (!((Object)chore_provider == (Object)null))
						{
							if (!diseaseInstance.IsDoctored)
							{
								chore = new VomitChore(Db.Get().ChoreTypes.Vomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, vomiting, delegate
								{
									StartChore();
								});
							}
							else
							{
								StartChore();
							}
						}
					}, null, null);
				}

				public void StopChore()
				{
					if (vomitHandle.IsValid)
					{
						vomitHandle.ClearScheduler();
					}
					if (chore != null)
					{
						chore.Cancel("FoodPoisoning.StopChore");
					}
				}
			}

			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				InstanceData instanceData = new InstanceData(go, diseaseInstance);
				instanceData.StartChore();
				return instanceData;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				InstanceData instanceData = (InstanceData)instance_data;
				instanceData.StopChore();
			}

			public override List<Descriptor> GetSymptoms()
			{
				List<Descriptor> list = new List<Descriptor>();
				list.Add(new Descriptor(DUPLICANTS.DISEASES.FOODPOISONING.VOMIT_SYMPTOM, DUPLICANTS.DISEASES.FOODPOISONING.VOMIT_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false));
				return list;
			}
		}

		public const string ID = "FoodPoisoning";

		private const float VOMIT_FREQUENCY = 200f;

		public FoodPoisoning()
			: base("FoodPoisoning", DiseaseType.Pathogen, Severity.Major, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Digestion
			}, 900f, 1, new RangeInfo(248.15f, 278.15f, 313.15f, 348.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
			AddDiseaseComponent(new CommonSickEffectDisease());
			AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[3]
			{
				new AttributeModifier("BladderDelta", 1.25f, DUPLICANTS.DISEASES.FOODPOISONING.NAME, false, false, true),
				new AttributeModifier("ToiletEfficiency", -0.4f, DUPLICANTS.DISEASES.FOODPOISONING.NAME, false, false, true),
				new AttributeModifier("StaminaDelta", -2.5f, DUPLICANTS.DISEASES.FOODPOISONING.NAME, false, false, true)
			}));
			AddDiseaseComponent(new FoodPoisoningComponent());
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.66666675f),
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(1000f),
				overPopulationHalfLife = new float?(3000f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(300f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(1000000)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ToxicSand)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(12000f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.Creature)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCountPerKG = new float?(4000f),
				overPopulationHalfLife = new float?(3000f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				diffusionScale = new float?(0.001f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = new float?(250f),
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(300f),
				diffusionScale = new float?(0.01f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(10000f),
				overPopulationHalfLife = new float?(3000f),
				diffusionScale = new float?(0.05f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(1000000)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(5000f),
				diffusionScale = new float?(0.2f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.DirtyWater)
			{
				populationHalfLife = new float?(-12000f),
				overPopulationHalfLife = new float?(12000f)
			});
			AddGrowthRule(new TagGrowthRule(GameTags.Edible)
			{
				populationHalfLife = new float?(-12000f),
				overPopulationHalfLife = new float?(float.PositiveInfinity)
			});
			AddGrowthRule(new TagGrowthRule(GameTags.Pickled)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f)
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
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}

		public override List<Descriptor> GetDiseaseSourceDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.FOODPOISONING.DISEASE_SOURCE_DESCRIPTOR, GameUtil.GetFormattedTime(200f), GameUtil.GetFormattedDiseaseAmount(100000), base.Name), string.Format(DUPLICANTS.DISEASES.FOODPOISONING.DISEASE_SOURCE_DESCRIPTOR_TOOLTIP, GameUtil.GetFormattedTime(200f), GameUtil.GetFormattedDiseaseAmount(100000), base.Name), Descriptor.DescriptorType.DiseaseSource, false));
			return list;
		}
	}
}
