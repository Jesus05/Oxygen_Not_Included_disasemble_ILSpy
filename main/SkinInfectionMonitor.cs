using Klei.AI;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SkinInfectionMonitor : GameStateMachine<SkinInfectionMonitor, SkinInfectionMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private PrimaryElement primaryElement;

		private ImmuneSystemMonitor.Instance immuneSystemMonitor;

		private const float INTERACT_INTERVAL = 1f;

		private float lastInteractTime = float.NegativeInfinity;

		private HandleVector<Game.ComplexCallbackInfo<Sim.DiseaseConsumptionCallback>>.Handle diseaseConsumptionHandle;

		[CompilerGenerated]
		private static Action<Sim.DiseaseConsumptionCallback, object> _003C_003Ef__mg_0024cache0;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			primaryElement = master.GetComponent<PrimaryElement>();
			diseaseConsumptionHandle = Game.Instance.diseaseConsumptionCallbackManager.Add(OnDiseaseConsumedCallback, this, "SkinInfectionMonitor");
		}

		public override void StartSM()
		{
			immuneSystemMonitor = controller.GetSMI<ImmuneSystemMonitor.Instance>();
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			if (diseaseConsumptionHandle.IsValid())
			{
				Game.Instance.diseaseConsumptionCallbackManager.Release(diseaseConsumptionHandle, "Disease consumption cb");
				diseaseConsumptionHandle.Clear();
			}
			base.StopSM(reason);
		}

		public void GetInfectedByContainedDisease(float dt)
		{
			byte diseaseIdx = primaryElement.DiseaseIdx;
			if (diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[diseaseIdx];
				if (disease.infectionVectors.Contains(Disease.InfectionVector.Contact))
				{
					int num = Mathf.CeilToInt(dt);
					primaryElement.ModifyDiseaseCount(-num, "SkinInfectionMonitor.GetInfectedByContainedDisease");
					immuneSystemMonitor.InjectDisease(disease, num, Tag.Invalid, Disease.InfectionVector.Contact);
				}
			}
			else
			{
				base.smi.GoTo(base.sm.clean);
			}
		}

		public void InteractWithWorld()
		{
			float time = Time.time;
			if (time - lastInteractTime > 1f)
			{
				int num = Grid.PosToCell(base.master.transform.GetPosition());
				int num2 = Grid.CellAbove(num);
				int num3 = Grid.CellBelow(num);
				Game.Instance.diseaseConsumptionCallbackManager.GetItem(diseaseConsumptionHandle);
				SimMessages.ConsumeDisease(num, 0.0166666675f, 250000, diseaseConsumptionHandle.index);
				if (Grid.IsValidCell(num2))
				{
					SimMessages.ConsumeDisease(num2, 0.0166666675f, 250000, diseaseConsumptionHandle.index);
				}
				if (Grid.IsValidCell(num3))
				{
					SimMessages.ConsumeDisease(num3, 0.00166666671f, 25000, diseaseConsumptionHandle.index);
				}
				lastInteractTime = time;
			}
		}

		private static void OnDiseaseConsumedCallback(Sim.DiseaseConsumptionCallback cb_info, object data)
		{
			((Instance)data).OnDiseaseConsumed(cb_info);
		}

		private void OnDiseaseConsumed(Sim.DiseaseConsumptionCallback cb_info)
		{
			if (diseaseConsumptionHandle.IsValid() && cb_info.diseaseIdx != 255)
			{
				primaryElement.AddDisease(cb_info.diseaseIdx, cb_info.diseaseCount, "SkinInfectionMonitor.OnDiseaseConsumed");
			}
		}

		public bool IsInfecting()
		{
			byte diseaseIdx = primaryElement.DiseaseIdx;
			return diseaseIdx != 255;
		}
	}

	public State clean;

	public State dirty;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = clean;
		base.serializable = false;
		clean.EventTransition(GameHashes.ExposeToDisease, dirty, (Instance smi) => smi.IsInfecting());
		dirty.Update(delegate(Instance smi, float dt)
		{
			smi.GetInfectedByContainedDisease(dt);
		}, UpdateRate.SIM_200ms, false).EventTransition(GameHashes.ExposeToDisease, clean, (Instance smi) => !smi.IsInfecting());
	}
}
