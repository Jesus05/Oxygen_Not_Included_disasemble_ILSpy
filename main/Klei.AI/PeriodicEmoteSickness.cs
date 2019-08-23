using UnityEngine;

namespace Klei.AI
{
	public class PeriodicEmoteSickness : Sickness.SicknessComponent
	{
		public class StatesInstance : GameStateMachine<States, StatesInstance, SicknessInstance, object>.GameInstance
		{
			public PeriodicEmoteSickness periodicEmoteSickness;

			public StatesInstance(SicknessInstance master, PeriodicEmoteSickness periodicEmoteSickness)
				: base(master)
			{
				this.periodicEmoteSickness = periodicEmoteSickness;
			}

			public Reactable GetReactable()
			{
				GameObject gameObject = base.master.gameObject;
				HashedString id = "PeriodicEmoteSickness";
				ChoreType emote = Db.Get().ChoreTypes.Emote;
				HashedString animset = "anim_sneeze_kanim";
				float cooldown = periodicEmoteSickness.cooldown;
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(gameObject, id, emote, animset, 0f, cooldown, float.PositiveInfinity);
				HashedString[] anims = periodicEmoteSickness.anims;
				foreach (HashedString anim in anims)
				{
					selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
					{
						anim = anim
					});
				}
				return selfEmoteReactable;
			}
		}

		public class States : GameStateMachine<States, StatesInstance, SicknessInstance>
		{
			public override void InitializeStates(out BaseState default_state)
			{
				default_state = root;
				root.ToggleReactable((StatesInstance smi) => smi.GetReactable());
			}
		}

		private HashedString[] anims;

		private float cooldown;

		public PeriodicEmoteSickness(HashedString kanim, HashedString[] anims, float cooldown)
		{
			this.anims = anims;
			this.cooldown = cooldown;
		}

		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
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
