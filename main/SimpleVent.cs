public class SimpleVent : KMonoBehaviour
{
	[MyCmpGet]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<SimpleVent> OnChangedDelegate = new EventSystem.IntraObjectHandler<SimpleVent>(delegate(SimpleVent component, object data)
	{
		component.OnChanged(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(-592767678, OnChangedDelegate);
		Subscribe(-111137758, OnChangedDelegate);
	}

	protected override void OnSpawn()
	{
		OnChanged(null);
	}

	private void OnChanged(object data)
	{
		if (operational.IsFunctional)
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, this);
		}
		else
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
		}
	}
}
