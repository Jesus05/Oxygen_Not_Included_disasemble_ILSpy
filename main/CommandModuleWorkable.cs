using UnityEngine;

public class CommandModuleWorkable : Workable
{
	private static CellOffset[] entryOffsets = new CellOffset[5]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(0, 2),
		new CellOffset(0, 3),
		new CellOffset(0, 4)
	};

	private static readonly EventSystem.IntraObjectHandler<CommandModuleWorkable> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CommandModuleWorkable>(delegate(CommandModuleWorkable component, object data)
	{
		component.OnLaunch(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsets(entryOffsets);
		synchronizeAnims = false;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_incubator_kanim")
		};
		SetWorkTime(float.PositiveInfinity);
		showProgressBar = false;
		Subscribe(-1056989049, OnLaunchDelegate);
	}

	private void OnLaunch(object data)
	{
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (!((Object)worker != (Object)null))
		{
			return base.OnWorkTick(worker, dt);
		}
		GameObject gameObject = worker.gameObject;
		CompleteWork(worker);
		GetComponent<MinionStorage>().SerializeMinion(gameObject);
		return true;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
	}
}
