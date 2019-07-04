using System.Collections.Generic;
using UnityEngine;

public class CreatureLightToggleController : GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		private struct ModifyBrightnessTask : IWorkItem<object>
		{
			private LightGridManager.LightGridEmitter emitter;

			public ModifyBrightnessTask(LightGridManager.LightGridEmitter emitter)
			{
				this.emitter = emitter;
				emitter.RemoveFromGrid();
			}

			public void Run(object context)
			{
				emitter.UpdateLitCells();
			}

			public void Finish()
			{
				emitter.AddToGrid(false);
			}
		}

		public delegate void ModifyLuxDelegate(Instance instance, float time_delta);

		private const float DIM_TIME = 25f;

		private const float GLOW_TIME = 15f;

		private int originalLux;

		private float originalRange;

		private Light2D light;

		private static WorkItemCollection<ModifyBrightnessTask, object> modify_brightness_job = new WorkItemCollection<ModifyBrightnessTask, object>();

		public static ModifyLuxDelegate dim = delegate(Instance instance, float time_delta)
		{
			float num2 = (float)instance.originalLux / 25f;
			instance.light.Lux = Mathf.FloorToInt(Mathf.Max(0f, (float)instance.light.Lux - num2 * time_delta));
		};

		public static ModifyLuxDelegate brighten = delegate(Instance instance, float time_delta)
		{
			float num = (float)instance.originalLux / 15f;
			instance.light.Lux = Mathf.CeilToInt(Mathf.Min((float)instance.originalLux, (float)instance.light.Lux + num * time_delta));
		};

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

		public static void ModifyBrightness(List<UpdateBucketWithUpdater<Instance>.Entry> instances, ModifyLuxDelegate modify_lux, float time_delta)
		{
			modify_brightness_job.Reset(null);
			for (int i = 0; i != instances.Count; i++)
			{
				UpdateBucketWithUpdater<Instance>.Entry value = instances[i];
				value.lastUpdateTime = 0f;
				instances[i] = value;
				Instance data = value.data;
				modify_lux(data, time_delta);
				data.light.Range = data.originalRange * (float)data.light.Lux / (float)data.originalLux;
				data.light.RefreshShapeAndPosition();
				if (data.light.RefreshShapeAndPosition() != 0)
				{
					modify_brightness_job.Add(new ModifyBrightnessTask(data.light.emitter));
				}
			}
			GlobalJobManager.Run(modify_brightness_job);
			for (int j = 0; j != modify_brightness_job.Count; j++)
			{
				modify_brightness_job.GetWorkItem(j).Finish();
			}
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
		turning_off.BatchUpdate(delegate(List<UpdateBucketWithUpdater<Instance>.Entry> instances, float time_delta)
		{
			Instance.ModifyBrightness(instances, Instance.dim, time_delta);
		}, UpdateRate.SIM_200ms).Transition(light_off, (Instance smi) => smi.IsOff(), UpdateRate.SIM_200ms);
		light_on.Enter(delegate(Instance smi)
		{
			smi.SwitchLight(true);
		}).TagTransition(GameTags.Creatures.Overcrowded, turning_off, false);
		turning_on.Enter(delegate(Instance smi)
		{
			smi.SwitchLight(true);
		}).BatchUpdate(delegate(List<UpdateBucketWithUpdater<Instance>.Entry> instances, float time_delta)
		{
			Instance.ModifyBrightness(instances, Instance.brighten, time_delta);
		}, UpdateRate.SIM_200ms).Transition(light_on, (Instance smi) => smi.IsOn(), UpdateRate.SIM_200ms);
	}
}
