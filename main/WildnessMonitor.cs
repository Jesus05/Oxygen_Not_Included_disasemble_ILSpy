using Klei.AI;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WildnessMonitor : GameStateMachine<WildnessMonitor, WildnessMonitor.Instance, IStateMachineTarget, WildnessMonitor.Def>
{
	public class Def : BaseDef
	{
		public Effect wildEffect;

		public Effect tameEffect;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
		}
	}

	public new class Instance : GameInstance
	{
		public AmountInstance wildness;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			wildness.value = wildness.GetMax();
		}
	}

	public State wild;

	public State tame;

	private static readonly KAnimHashedString[] DOMESTICATION_SYMBOLS = new KAnimHashedString[2]
	{
		"tag",
		"snapto_tag"
	};

	[CompilerGenerated]
	private static StateMachine<WildnessMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<WildnessMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<WildnessMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static StateMachine<WildnessMonitor, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache4;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = tame;
		base.serializable = true;
		wild.Enter(RefreshAmounts).Enter(HideDomesticationSymbol).Transition(tame, (Instance smi) => !IsWild(smi), UpdateRate.SIM_1000ms)
			.ToggleEffect((Instance smi) => smi.def.wildEffect)
			.ToggleTag(GameTags.Creatures.Wild);
		tame.Enter(RefreshAmounts).Enter(ShowDomesticationSymbol).Transition(wild, IsWild, UpdateRate.SIM_1000ms)
			.ToggleEffect((Instance smi) => smi.def.tameEffect);
	}

	private static void HideDomesticationSymbol(Instance smi)
	{
		KAnimHashedString[] dOMESTICATION_SYMBOLS = DOMESTICATION_SYMBOLS;
		foreach (KAnimHashedString symbol in dOMESTICATION_SYMBOLS)
		{
			smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(symbol, false);
		}
	}

	private static void ShowDomesticationSymbol(Instance smi)
	{
		KAnimHashedString[] dOMESTICATION_SYMBOLS = DOMESTICATION_SYMBOLS;
		foreach (KAnimHashedString symbol in dOMESTICATION_SYMBOLS)
		{
			smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(symbol, true);
		}
	}

	private static bool IsWild(Instance smi)
	{
		return smi.wildness.value > 0f;
	}

	private static void RefreshAmounts(Instance smi)
	{
		bool flag = IsWild(smi);
		smi.wildness.hide = !flag;
		Db.Get().CritterAttributes.Happiness.Lookup(smi.gameObject).hide = flag;
		Db.Get().Amounts.Calories.Lookup(smi.gameObject).hide = flag;
		Db.Get().Amounts.Temperature.Lookup(smi.gameObject).hide = flag;
		AmountInstance amountInstance = Db.Get().Amounts.Fertility.Lookup(smi.gameObject);
		if (amountInstance != null)
		{
			amountInstance.hide = flag;
		}
	}
}
