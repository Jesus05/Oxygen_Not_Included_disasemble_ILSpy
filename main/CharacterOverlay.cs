[SkipSaveFileSerialization]
public class CharacterOverlay : KMonoBehaviour
{
	private bool registered = false;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Register();
	}

	public void Register()
	{
		if (!registered)
		{
			registered = true;
			NameDisplayScreen.Instance.AddNewEntry(base.gameObject);
		}
	}
}
