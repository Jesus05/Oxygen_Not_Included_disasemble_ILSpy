using KSerialization;
using UnityEngine;

public class Unsealable : Workable
{
	[Serialize]
	public bool facingRight;

	[Serialize]
	public bool unsealed;

	private Unsealable()
	{
	}

	public override CellOffset[] GetOffsets(int cell)
	{
		if (facingRight)
		{
			return OffsetGroups.RightOnly;
		}
		return OffsetGroups.LeftOnly;
	}

	protected override void OnPrefabInit()
	{
		faceTargetWhenWorking = true;
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_door_poi_kanim")
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(3f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		unsealed = true;
		base.OnCompleteWork(worker);
		Deconstructable component = GetComponent<Deconstructable>();
		if ((Object)component != (Object)null)
		{
			component.allowDeconstruction = true;
			Game.Instance.Trigger(1980521255, base.gameObject);
		}
	}
}
