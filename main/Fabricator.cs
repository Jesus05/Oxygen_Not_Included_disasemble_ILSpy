using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Fabricator : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}
}
