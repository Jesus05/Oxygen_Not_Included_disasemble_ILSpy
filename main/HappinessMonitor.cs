using Klei.AI;
using STRINGS;
using System.Runtime.CompilerServices;

public class HappinessMonitor : GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
	public class Def : BaseDef
	{
		public float threshold = 0f;
	}

	public class UnhappyState : State
	{
		public State wild;

		public State tame;
	}

	public class HappyState : State
	{
		public State wild;

		public State tame;
	}

	public new class Instance : GameInstance
	{
		public AttributeInstance happiness;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			happiness = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Happiness);
		}
	}

	private State satisfied;

	private HappyState happy;

	private UnhappyState unhappy;

	private Effect happyWildEffect;

	private Effect happyTameEffect;

	private Effect unhappyWildEffect;

	private Effect unhappyTameEffect;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache3;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(happy, IsHappy, UpdateRate.SIM_1000ms).Transition(unhappy, GameStateMachine<HappinessMonitor, Instance, IStateMachineTarget, Def>.Not(IsHappy), UpdateRate.SIM_1000ms);
		happy.DefaultState(happy.wild).Transition(satisfied, GameStateMachine<HappinessMonitor, Instance, IStateMachineTarget, Def>.Not(IsHappy), UpdateRate.SIM_1000ms);
		happy.wild.ToggleEffect((Instance smi) => happyWildEffect).TagTransition(GameTags.Creatures.Wild, happy.tame, true);
		happy.tame.ToggleEffect((Instance smi) => happyTameEffect).TagTransition(GameTags.Creatures.Wild, happy.wild, false);
		unhappy.DefaultState(unhappy.wild).Transition(satisfied, IsHappy, UpdateRate.SIM_1000ms);
		unhappy.wild.ToggleEffect((Instance smi) => unhappyWildEffect).TagTransition(GameTags.Creatures.Wild, unhappy.tame, true);
		unhappy.tame.ToggleEffect((Instance smi) => unhappyTameEffect).TagTransition(GameTags.Creatures.Wild, unhappy.wild, false);
		happyWildEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, true, false, false, null, 0f, null);
		happyTameEffect = new Effect("Happy", CREATURES.MODIFIERS.HAPPY.NAME, CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0f, true, false, false, null, 0f, null);
		happyTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 9f, CREATURES.MODIFIERS.HAPPY.NAME, true, false, true));
		unhappyWildEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, true, false, true, null, 0f, null);
		unhappyWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, CREATURES.MODIFIERS.UNHAPPY.NAME, false, false, true));
		unhappyTameEffect = new Effect("Unhappy", CREATURES.MODIFIERS.UNHAPPY.NAME, CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0f, true, false, true, null, 0f, null);
		unhappyTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, CREATURES.MODIFIERS.UNHAPPY.NAME, false, false, true));
	}

	private static bool IsHappy(Instance smi)
	{
		return smi.happiness.GetTotalValue() >= smi.def.threshold;
	}
}
