using Klei.AI;
using STRINGS;
using UnityEngine;

public class SuffocationMonitor : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance>
{
	public class NoOxygenState : State
	{
		public State holdingbreath;

		public State suffocating;
	}

	public class SatisfiedState : State
	{
		public State normal;

		public State low;
	}

	public new class Instance : GameInstance
	{
		private AmountInstance breath;

		public AttributeModifier breathing;

		public AttributeModifier holdingbreath;

		private static CellOffset[] pressureTestOffsets = new CellOffset[2]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};

		private const float HIGH_PRESSURE_DELAY = 3f;

		private bool wasInHighPressure;

		private float highPressureTime;

		public OxygenBreather oxygenBreather
		{
			get;
			private set;
		}

		public Instance(OxygenBreather oxygen_breather)
			: base((IStateMachineTarget)oxygen_breather)
		{
			breath = Db.Get().Amounts.Breath.Lookup(base.master.gameObject);
			Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float num = 0.909090936f;
			breathing = new AttributeModifier(deltaAttribute.Id, num, DUPLICANTS.MODIFIERS.BREATHING.NAME, false, false, true);
			holdingbreath = new AttributeModifier(deltaAttribute.Id, 0f - num, DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME, false, false, true);
			oxygenBreather = oxygen_breather;
		}

		public bool IsInBreathableArea()
		{
			return base.master.GetComponent<KPrefabID>().HasTag(GameTags.RecoveringBreath) || base.master.GetComponent<Sensors>().GetSensor<BreathableAreaSensor>().IsBreathable();
		}

		public bool HasSuffocated()
		{
			return breath.value <= 0f;
		}

		public bool IsSuffocating()
		{
			return breath.deltaAttribute.GetTotalValue() <= 0f && breath.value <= 45.4545441f;
		}

		public void Kill()
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
		}

		public void CheckOverPressure()
		{
			if (IsInHighPressure())
			{
				if (!wasInHighPressure)
				{
					wasInHighPressure = true;
					highPressureTime = Time.time;
				}
				else if (Time.time - highPressureTime > 3f)
				{
					base.master.GetComponent<Effects>().Add("PoppedEarDrums", true);
				}
			}
			else
			{
				wasInHighPressure = false;
			}
		}

		private bool IsInHighPressure()
		{
			int cell = Grid.PosToCell(base.gameObject);
			for (int i = 0; i < pressureTestOffsets.Length; i++)
			{
				int num = Grid.OffsetCell(cell, pressureTestOffsets[i]);
				if (Grid.IsValidCell(num) && Grid.Element[num].IsGas && Grid.Mass[num] > 4f)
				{
					return true;
				}
			}
			return false;
		}
	}

	public SatisfiedState satisfied;

	public NoOxygenState nooxygen;

	public State death;

	public State dead;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Update("CheckOverPressure", delegate(Instance smi, float dt)
		{
			smi.CheckOverPressure();
		}, UpdateRate.SIM_200ms, false).TagTransition(GameTags.Dead, dead, false);
		satisfied.DefaultState(satisfied.normal).ToggleAttributeModifier("Breathing", (Instance smi) => smi.breathing, null).EventTransition(GameHashes.ExitedBreathableArea, nooxygen, (Instance smi) => !smi.IsInBreathableArea());
		satisfied.normal.Transition(satisfied.low, (Instance smi) => smi.oxygenBreather.IsLowOxygen(), UpdateRate.SIM_200ms);
		satisfied.low.Transition(satisfied.normal, (Instance smi) => !smi.oxygenBreather.IsLowOxygen(), UpdateRate.SIM_200ms).Transition(nooxygen, (Instance smi) => !smi.IsInBreathableArea(), UpdateRate.SIM_200ms).ToggleEffect("LowOxygen");
		nooxygen.EventTransition(GameHashes.EnteredBreathableArea, satisfied, (Instance smi) => smi.IsInBreathableArea()).TagTransition(GameTags.RecoveringBreath, satisfied, false).ToggleExpression(Db.Get().Expressions.Suffocate, null)
			.ToggleAttributeModifier("Holding Breath", (Instance smi) => smi.holdingbreath, null)
			.ToggleTag(GameTags.NoOxygen)
			.DefaultState(nooxygen.holdingbreath);
		nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath, null).Transition(nooxygen.suffocating, (Instance smi) => smi.IsSuffocating(), UpdateRate.SIM_200ms);
		nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating, null).Transition(death, (Instance smi) => smi.HasSuffocated(), UpdateRate.SIM_200ms);
		death.Enter("SuffocationDeath", delegate(Instance smi)
		{
			smi.Kill();
		});
		dead.DoNothing();
	}
}
