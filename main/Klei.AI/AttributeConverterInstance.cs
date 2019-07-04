using UnityEngine;

namespace Klei.AI
{
	public class AttributeConverterInstance : ModifierInstance<AttributeConverter>
	{
		public AttributeConverter converter;

		public AttributeInstance attributeInstance;

		public AttributeConverterInstance(GameObject game_object, AttributeConverter converter, AttributeInstance attribute_instance)
			: base(game_object, converter)
		{
			this.converter = converter;
			attributeInstance = attribute_instance;
		}

		public float Evaluate()
		{
			return converter.multiplier * attributeInstance.GetTotalValue() + converter.baseValue;
		}

		public string DescriptionFromAttribute(float value, GameObject go)
		{
			return converter.DescriptionFromAttribute(Evaluate(), go);
		}
	}
}
