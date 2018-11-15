using UnityEngine;

public class CreatureLightToggleController : GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		private const float DIM_TIME = 25f;

		private const float GLOW_TIME = 15f;

		private int originalLux;

		private float originalRange;

		private Light2D light;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			light = master.GetComponent<Light2D>();
			originalLux = light.Lux;
			originalRange = light.Range;
		}

		public void SwitchLight(bool on)
		{
			light.enabled = on;
		}

		public void Dim(float dt)
		{
			float num = (float)originalLux / 25f;
			light.Lux = Mathf.FloorToInt(Mathf.Max(0f, (float)light.Lux - num * dt));
			light.Range = originalRange * (float)light.Lux / (float)originalLux;
			light.Refresh();
		}

		public void Brighten(float dt)
		{
			float num = (float)originalLux / 15f;
			light.Lux = Mathf.CeilToInt(Mathf.Min((float)originalLux, (float)light.Lux + num * dt));
			light.Range = originalRange * (float)light.Lux / (float)originalLux;
			light.Refresh();
		}

		public bool IsOff()
		{
			return light.Lux == 0;
		}

		public bool IsOn()
		{
			return light.Lux >= originalLux;
		}
	}

	private State light_off;

	private State turning_off;

	private State light_on;

	private State turning_on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = light_on;
		base.serializable = true;
		light_off.Enter(delegate(Instance smi)
		{
			smi.SwitchLight(false);
		}).TagTransition(GameTags.Creatures.Overcrowded, turning_on, true);
		turning_off.Update(delegate(Instance smi, float dt)
		{
			smi.Dim(dt);
		}, UpdateRate.SIM_200ms, false).Transition(light_off, (Instance smi) => smi.IsOff(), UpdateRate.SIM_200ms);
		light_on.Enter(delegate(Instance smi)
		{
			smi.SwitchLight(true);
		}).TagTransition(GameTags.Creatures.Overcrowded, turning_off, false);
		turning_on.Enter(delegate(Instance smi)
		{
			smi.SwitchLight(true);
		}).Update(delegate(Instance smi, float dt)
		{
			smi.Brighten(dt);
		}, UpdateRate.SIM_200ms, false).Transition(light_on, (Instance smi) => smi.IsOn(), UpdateRate.SIM_200ms);
	}
}
