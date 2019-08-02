using UnityEngine;

namespace Klei.AI
{
	public class AttributeConverter : Resource
	{
		public string description;

		public float multiplier;

		public float baseValue;

		public Attribute attribute;

		public IAttributeFormatter formatter;

		public AttributeConverter(string id, string name, string description, float multiplier, float base_value, Attribute attribute, IAttributeFormatter formatter = null)
			: base(id, name)
		{
			this.description = description;
			this.multiplier = multiplier;
			baseValue = base_value;
			this.attribute = attribute;
			this.formatter = formatter;
		}

		public AttributeConverterInstance Lookup(Component cmp)
		{
			return Lookup(cmp.gameObject);
		}

		public AttributeConverterInstance Lookup(GameObject go)
		{
			AttributeConverters component = go.GetComponent<AttributeConverters>();
			if ((Object)component != (Object)null)
			{
				return component.Get(this);
			}
			return null;
		}

		public string DescriptionFromAttribute(float value, GameObject go)
		{
			string text = (formatter != null) ? formatter.GetFormattedValue(value, formatter.DeltaTimeSlice, go) : ((attribute.formatter == null) ? GameUtil.GetFormattedSimple(value, GameUtil.TimeSlice.None, null) : attribute.formatter.GetFormattedValue(value, attribute.formatter.DeltaTimeSlice, go));
			if (text != null)
			{
				text = GameUtil.AddPositiveSign(text, value > 0f);
				return string.Format(description, text);
			}
			return null;
		}
	}
}
