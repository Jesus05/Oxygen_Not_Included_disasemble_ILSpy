public class SimpleMassStatusItem : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		KSelectable component = GetComponent<KSelectable>();
		component.AddStatusItem(Db.Get().MiscStatusItems.OreMass, base.gameObject);
	}
}
