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

		public string DescriptionFromAttribute()
		{
			float num = Evaluate();
			string text = (converter.formatter != null) ? converter.formatter.GetFormattedValue(num, converter.formatter.DeltaTimeSlice, base.gameObject) : ((attributeInstance.Attribute.formatter == null) ? GameUtil.GetFormattedSimple(num, GameUtil.TimeSlice.None, null) : attributeInstance.Attribute.formatter.GetFormattedValue(num, attributeInstance.Attribute.formatter.DeltaTimeSlice, base.gameObject));
			if (text != null)
			{
				text = GameUtil.AddPositiveSign(text, num > 0f);
				return string.Format(converter.description, text);
			}
			return null;
		}
	}
}
