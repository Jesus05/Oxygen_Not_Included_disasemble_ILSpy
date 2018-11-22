using UnityEngine;

namespace Klei.AI
{
	public class PeriodicEmoteDisease : Disease.DiseaseComponent
	{
		public class StatesInstance : GameStateMachine<States, StatesInstance, DiseaseInstance, object>.GameInstance
		{
			public PeriodicEmoteDisease periodicEmoteDisease;

			public StatesInstance(DiseaseInstance master, PeriodicEmoteDisease periodicEmoteDisease)
				: base(master)
			{
				this.periodicEmoteDisease = periodicEmoteDisease;
			}
		}

		public class States : GameStateMachine<States, StatesInstance, DiseaseInstance>
		{
			public State emoting;

			public State cooldown;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = emoting;
				emoting.ToggleChore((StatesInstance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.Emote, smi.periodicEmoteDisease.kanim, smi.periodicEmoteDisease.anims, KAnim.PlayMode.Once, false), cooldown);
				cooldown.ScheduleGoTo((StatesInstance smi) => smi.periodicEmoteDisease.cooldown, emoting);
			}
		}

		private HashedString kanim;

		private HashedString[] anims;

		private float cooldown;

		public PeriodicEmoteDisease(HashedString kanim, HashedString[] anims, float cooldown)
		{
			this.kanim = kanim;
			this.anims = anims;
			this.cooldown = cooldown;
		}

		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			StatesInstance statesInstance = new StatesInstance(diseaseInstance, this);
			statesInstance.StartSM();
			return statesInstance;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			StatesInstance statesInstance = (StatesInstance)instance_data;
			statesInstance.StopSM("Cured");
		}
	}
}
