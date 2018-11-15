[SkipSaveFileSerialization]
public class Cancellable : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<Cancellable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Cancellable>(delegate(Cancellable component, object data)
	{
		component.OnCancel(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(2127324410, OnCancelDelegate);
	}

	protected virtual void OnCancel(object data)
	{
		this.DeleteObject();
	}
}
