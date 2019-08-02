using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CreatureBait : StateMachineComponent<CreatureBait.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CreatureBait, object>.GameInstance
	{
		public StatesInstance(CreatureBait master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CreatureBait>
	{
		public State idle;

		public State destroy;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Baited).Enter(delegate(StatesInstance smi)
			{
				Element element = ElementLoader.FindElementByName(smi.master.baitElement.ToString());
				KAnim.Build build = element.substance.anim.GetData().build;
				KAnim.Build.Symbol symbol = build.GetSymbol(new KAnimHashedString(build.name));
				HashedString target_symbol = "snapTo_bait";
				SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
				component.AddSymbolOverride(target_symbol, symbol, 0);
			}).TagTransition(GameTags.LureUsed, destroy, false);
			destroy.PlayAnim("use").EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}
	}

	[Serialize]
	public Tag baitElement;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Tag[] constructionElements = GetComponent<Deconstructable>().constructionElements;
		baitElement = constructionElements[1];
		Lure.Instance sMI = base.gameObject.GetSMI<Lure.Instance>();
		sMI.SetActiveLures(new Tag[1]
		{
			baitElement
		});
		base.smi.StartSM();
	}
}
