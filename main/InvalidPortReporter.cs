using UnityEngine;

public class InvalidPortReporter : KMonoBehaviour
{
	public static readonly Operational.Flag portsNotOverlapping = new Operational.Flag("ports_not_overlapping", Operational.Flag.Type.Functional);

	private static readonly EventSystem.IntraObjectHandler<InvalidPortReporter> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<InvalidPortReporter>(delegate(InvalidPortReporter component, object data)
	{
		component.OnTagsChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnTagsChanged(null);
		Subscribe(-1582839653, OnTagsChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnTagsChanged(object data)
	{
		bool flag = base.gameObject.HasTag(GameTags.HasInvalidPorts);
		Operational component = GetComponent<Operational>();
		if ((Object)component != (Object)null)
		{
			component.SetFlag(portsNotOverlapping, !flag);
		}
		KSelectable component2 = GetComponent<KSelectable>();
		if ((Object)component2 != (Object)null)
		{
			component2.ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidPortOverlap, flag, base.gameObject);
		}
	}
}
