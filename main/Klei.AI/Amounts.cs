using UnityEngine;

namespace Klei.AI
{
	public class Amounts : Modifications<Amount, AmountInstance>
	{
		public Amounts(GameObject go)
			: base(go, (ResourceSet<Amount>)null)
		{
		}

		public float GetValue(string amount_id)
		{
			AmountInstance amountInstance = Get(amount_id);
			return amountInstance.value;
		}

		public void SetValue(string amount_id, float value)
		{
			AmountInstance amountInstance = Get(amount_id);
			amountInstance.value = value;
		}

		public override AmountInstance Add(AmountInstance instance)
		{
			instance.Activate();
			return base.Add(instance);
		}

		public override void Remove(AmountInstance instance)
		{
			instance.Deactivate();
			base.Remove(instance);
		}

		public void Cleanup()
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Deactivate();
			}
		}
	}
}
