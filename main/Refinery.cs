using KSerialization;
using System;

[SerializationConfig(MemberSerialization.OptIn)]
public class Refinery : KMonoBehaviour
{
	[Serializable]
	public struct OrderSaveData
	{
		public string id;

		public bool infinite;

		public OrderSaveData(string id, bool infinite)
		{
			this.id = id;
			this.infinite = infinite;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ComplexFabricator component = GetComponent<ComplexFabricator>();
	}
}
