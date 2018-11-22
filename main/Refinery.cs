using KSerialization;
using System;
using System.Collections.Generic;

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

	[Serialize]
	private List<OrderSaveData> savedOrders;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ComplexFabricator component = GetComponent<ComplexFabricator>();
		foreach (OrderSaveData savedOrder in savedOrders)
		{
			OrderSaveData order = savedOrder;
			component.CreateUserOrder(ComplexRecipeManager.Get().recipes.Find((ComplexRecipe match) => match.id == order.id), true, null);
		}
	}
}
