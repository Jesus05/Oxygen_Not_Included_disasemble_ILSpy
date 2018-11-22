using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Spores : Disease
	{
		private class SporesComponent : DiseaseComponent
		{
			private class InstanceData : ISim4000ms
			{
				public GameObject go;

				public void Sim4000ms(float dt)
				{
					if (!((Object)go == (Object)null))
					{
						int gameCell = Grid.PosToCell(go.transform.GetPosition());
						float value = Db.Get().Amounts.Temperature.Lookup(go).value;
						SimMessages.AddRemoveSubstance(gameCell, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.05f, value, byte.MaxValue, 0, true, -1);
						KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("spore_fx_kanim", go.transform.GetPosition(), go.transform, true, Grid.SceneLayer.Front, false);
						kBatchedAnimController.Play(WorkLoopAnims, KAnim.PlayMode.Once);
						kBatchedAnimController.destroyOnAnimComplete = true;
					}
				}
			}

			private static readonly HashedString[] WorkLoopAnims = new HashedString[3]
			{
				"working_pre",
				"working_loop",
				"working_pst"
			};

			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				InstanceData instanceData = new InstanceData();
				instanceData.go = go;
				SimAndRenderScheduler.instance.Add(instanceData, false);
				instanceData.Sim4000ms(0f);
				return instanceData;
			}

			public override void OnCure(GameObject go, object instace_data)
			{
				InstanceData obj = (InstanceData)instace_data;
				SimAndRenderScheduler.instance.Remove(obj);
			}

			private void Emit(GameObject go)
			{
			}
		}

		private const float EmitMass = 0.05f;

		private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

		public const string ID = "Spores";

		public Spores()
			: base("Spores", DiseaseType.Ailment, Severity.Major, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Contact
			}, 900f, 0, new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent(), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
			AddDiseaseComponent(new AnimatedDisease(new HashedString[1]
			{
				"anim_idle_spores_kanim"
			}, Db.Get().Expressions.SickSpores));
			AddDiseaseComponent(new SporesComponent());
		}
	}
}
