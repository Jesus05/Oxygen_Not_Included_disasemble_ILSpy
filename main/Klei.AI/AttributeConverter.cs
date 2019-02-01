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
			if (!((Object)component != (Object)null))
			{
				return null;
			}
			return component.Get(this);
		}
	}
}
