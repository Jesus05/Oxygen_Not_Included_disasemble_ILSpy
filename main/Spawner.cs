using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Spawner : KMonoBehaviour, ISaveLoadable
{
	[Serialize]
	public Tag prefabTag;

	[Serialize]
	public int units = 1;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SaveGame.Instance.worldGenSpawner.AddLegacySpawner(prefabTag, Grid.PosToCell(this));
		Util.KDestroyGameObject(base.gameObject);
	}
}
