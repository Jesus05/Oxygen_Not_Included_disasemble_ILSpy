using System.Collections.Generic;

namespace Klei.AI
{
	public class AttributeConverters : KMonoBehaviour
	{
		public List<AttributeConverterInstance> converters = new List<AttributeConverterInstance>();

		public int Count => converters.Count;

		protected override void OnPrefabInit()
		{
			foreach (AttributeInstance attribute in this.GetAttributes())
			{
				foreach (AttributeConverter converter in attribute.Attribute.converters)
				{
					AttributeConverterInstance item = new AttributeConverterInstance(base.gameObject, converter, attribute);
					converters.Add(item);
				}
			}
		}

		public AttributeConverterInstance Get(AttributeConverter converter)
		{
			foreach (AttributeConverterInstance converter2 in converters)
			{
				if (converter2.converter == converter)
				{
					return converter2;
				}
			}
			return null;
		}

		public AttributeConverterInstance GetConverter(string id)
		{
			foreach (AttributeConverterInstance converter in converters)
			{
				if (converter.converter.Id == id)
				{
					return converter;
				}
			}
			return null;
		}
	}
}
