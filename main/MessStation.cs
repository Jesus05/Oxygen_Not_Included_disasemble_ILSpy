public class MessStation : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_use_machine_kanim")
		};
	}

	protected override void OnCompleteWork(Worker worker)
	{
		worker.workable.GetComponent<Edible>().CompleteWork(worker);
	}
}
