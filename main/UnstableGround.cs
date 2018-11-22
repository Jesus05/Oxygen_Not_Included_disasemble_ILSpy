using KSerialization;

[SerializationConfig(MemberSerialization.OptOut)]
public class UnstableGround : KMonoBehaviour
{
	public SimHashes element;

	public float mass;

	public float temperature;

	public byte diseaseIdx;

	public int diseaseCount;
}
