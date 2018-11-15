using System;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class NodeGraphicsProperty : DictionaryProperty<NodeGraphics>
	{
		public NodeGraphicsProperty()
		{
			base.Domain = PropertyDomain.Node;
		}

		internal NodeGraphicsProperty(XElement xKey)
			: this()
		{
			XAttribute xAttribute = xKey.Attribute("yfiles.type");
			if (xAttribute == null || xAttribute.Value != "nodegraphics")
			{
				throw new ArgumentException("Key not compatible with property.");
			}
			LoadFromKeyElement(xKey);
		}

		public override XElement GetKeyElement()
		{
			XElement keyElement = base.GetKeyElement();
			keyElement.SetAttributeValue("yfiles.type", "nodegraphics");
			return keyElement;
		}

		protected override NodeGraphics ReadValue(XElement x)
		{
			return new NodeGraphics(x);
		}

		protected override XElement WriteValue(NodeGraphics value)
		{
			return value.ToXml();
		}
	}
}
