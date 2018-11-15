using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RocketEngine : StateMachineComponent<RocketEngine.StatesInstance>, IEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RocketEngine, object>.GameInstance
	{
		public StatesInstance(RocketEngine smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RocketEngine>
	{
		public State idle;

		public State burning;

		public State burnComplete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.IgniteEngine, burning, null);
			burning.EventTransition(GameHashes.LandRocket, burnComplete, null).PlayAnim("launch_pre").QueueAnim("launch_loop", true, null)
				.Update(delegate(StatesInstance smi, float dt)
				{
					Vector3 pos = smi.master.gameObject.transform.GetPosition() + smi.master.GetComponent<KBatchedAnimController>().Offset;
					int num = Grid.PosToCell(pos);
					if (Grid.IsValidCell(num))
					{
						SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, 0, 0, -1);
					}
					int num2 = 10;
					for (int i = 1; i < num2; i++)
					{
						int num3 = Grid.OffsetCell(num, -1, -i);
						int num4 = Grid.OffsetCell(num, 0, -i);
						int num5 = Grid.OffsetCell(num, 1, -i);
						if (Grid.IsValidCell(num3))
						{
							SimMessages.ModifyEnergy(num3, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.IsValidCell(num4))
						{
							SimMessages.ModifyEnergy(num4, smi.master.exhaustTemperature / (float)i, 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.IsValidCell(num5))
						{
							SimMessages.ModifyEnergy(num5, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
					}
				}, UpdateRate.SIM_200ms, false);
			burnComplete.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.IgniteEngine, burning, null);
		}
	}

	public float exhaustEmitRate = 50f;

	public float exhaustTemperature = 1500f;

	public SpawnFXHashes explosionEffectHash;

	public SimHashes exhaustElement = SimHashes.CarbonDioxide;

	public Tag fuelTag;

	public float efficiency = 1f;

	public bool requireOxidizer = true;

	public bool mainEngine = true;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (mainEngine)
		{
			RequireAttachedComponent condition = new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(FuelTank), UI.STARMAP.COMPONENT.FUEL_TANK);
			GetComponent<RocketModule>().AddLaunchCondition(condition);
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}
}
