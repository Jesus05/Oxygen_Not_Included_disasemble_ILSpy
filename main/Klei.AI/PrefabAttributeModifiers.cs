using System.Collections.Generic;

namespace Klei.AI
{
	public class PrefabAttributeModifiers : KMonoBehaviour
	{
		public List<AttributeModifier> descriptors = new List<AttributeModifier>();

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public void AddAttributeDescriptor(AttributeModifier modifier)
		{
			descriptors.Add(modifier);
		}

		public void RemovePrefabAttribute(AttributeModifier modifier)
		{
			descriptors.Remove(modifier);
		}
	}
}
