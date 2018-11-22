using Klei.AI.DiseaseGrowthRules;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class PutridOdour : Disease
	{
		public class PutridOdourComponent : DiseaseComponent
		{
			private class InstanceData : ISim4000ms
			{
				public KAnimControllerBase controller;

				public void Sim4000ms(float dt)
				{
					if (!((Object)controller == (Object)null))
					{
						GameObject gameObject = controller.gameObject;
						Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
						Vector2 a = gameObject.transform.GetPosition();
						for (int i = 0; i < liveMinionIdentities.Count; i++)
						{
							MinionIdentity minionIdentity = liveMinionIdentities[i];
							if ((Object)minionIdentity.gameObject != (Object)gameObject.gameObject)
							{
								Vector2 b = minionIdentity.transform.GetPosition();
								float num = Vector2.SqrMagnitude(a - b);
								if (num <= 2.25f)
								{
									minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
									minionIdentity.GetComponent<Effects>().Add("SmelledPutridOdour", true);
									minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
								}
							}
						}
					}
				}
			}

			private static readonly HashedString[] WorkLoopAnims = new HashedString[2]
			{
				"working_pre",
				"working_loop"
			};

			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				InstanceData instanceData = new InstanceData();
				KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", go.transform.GetPosition(), go.transform, true, Grid.SceneLayer.Front, false);
				kBatchedAnimController.Play(WorkLoopAnims, KAnim.PlayMode.Loop);
				instanceData.controller = kBatchedAnimController;
				SimAndRenderScheduler.instance.Add(instanceData, false);
				instanceData.Sim4000ms(0f);
				return instanceData;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				InstanceData instanceData = (InstanceData)instance_data;
				KAnimControllerBase controller = instanceData.controller;
				controller.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
				controller.destroyOnAnimComplete = true;
				SimAndRenderScheduler.instance.Remove(instanceData);
			}
		}

		private const float EmitInterval = 5f;

		private const float EmissionRadius = 1.5f;

		private const float MaxDistanceSq = 2.25f;

		public const string ID = "PutridOdour";

		public PutridOdour()
			: base("PutridOdour", DiseaseType.Ailment, Severity.Minor, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Inhalation
			}, 900f, 1, new RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
			AddDiseaseComponent(new CommonSickEffectDisease());
			AddDiseaseComponent(new PutridOdourComponent());
			AddDiseaseComponent(new AnimatedDisease(new HashedString[1]
			{
				"anim_idle_sick_kanim"
			}, Db.Get().Expressions.Sick));
			AddDiseaseComponent(new PeriodicEmoteDisease("anim_idle_sick_kanim", new HashedString[3]
			{
				"idle_pre",
				"idle_default",
				"idle_pst"
			}, 5f));
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.66666675f),
				minCountPerKG = new float?(100f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(1000f),
				overPopulationHalfLife = new float?(3000f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(6000f),
				minDiffusionCount = new int?(1000000)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.SlimeMold)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(10000f),
				diffusionScale = new float?(0.05f)
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(6000f),
				diffusionScale = new float?(0.2f)
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(1E+07f)
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
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}
	}
}
