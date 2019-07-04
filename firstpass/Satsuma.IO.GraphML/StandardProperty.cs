using System;
using System.Globalization;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class StandardProperty<T> : DictionaryProperty<T>
	{
		private static readonly StandardType Type = ParseType(typeof(T));

		private static readonly string TypeString = TypeToGraphML(Type);

		public StandardProperty()
		{
		}

		internal StandardProperty(XElement xKey)
			: this()
		{
			XAttribute xAttribute = xKey.Attribute("attr.type");
			if (xAttribute == null || xAttribute.Value != TypeString)
			{
				throw new ArgumentException("Key not compatible with property.");
			}
			LoadFromKeyElement(xKey);
		}

		private static StandardType ParseType(Type t)
		{
			if (t != typeof(bool))
			{
				if (t != typeof(double))
				{
					if (t != typeof(float))
					{
						if (t != typeof(int))
						{
							if (t != typeof(long))
							{
								if (t != typeof(string))
								{
									throw new ArgumentException("Invalid type for a standard GraphML property.");
								}
								return StandardType.String;
							}
							return StandardType.Long;
						}
						return StandardType.Int;
					}
					return StandardType.Float;
				}
				return StandardType.Double;
			}
			return StandardType.Bool;
		}

		private static string TypeToGraphML(StandardType type)
		{
			switch (type)
			{
			case StandardType.Bool:
				return "boolean";
			case StandardType.Double:
				return "double";
			case StandardType.Float:
				return "float";
			case StandardType.Int:
				return "int";
			case StandardType.Long:
				return "long";
			default:
				return "string";
			}
		}

		private static object ParseValue(string value)
		{
			switch (Type)
			{
			case StandardType.Bool:
				return value == "true";
			case StandardType.Double:
				return double.Parse(value, CultureInfo.InvariantCulture);
			case StandardType.Float:
				return float.Parse(value, CultureInfo.InvariantCulture);
			case StandardType.Int:
				return int.Parse(value, CultureInfo.InvariantCulture);
			case StandardType.Long:
				return long.Parse(value, CultureInfo.InvariantCulture);
			default:
				return value;
			}
		}

		public override XElement GetKeyElement()
		{
			XElement keyElement = base.GetKeyElement();
			keyElement.SetAttributeValue("attr.type", TypeString);
			return keyElement;
		}

		protected override T ReadValue(XElement x)
		{
			return (T)ParseValue(x.Value);
		}

		protected override XElement WriteValue(T value)
		{
			return new XElement("dummy", value.ToString());
		}
	}
}
