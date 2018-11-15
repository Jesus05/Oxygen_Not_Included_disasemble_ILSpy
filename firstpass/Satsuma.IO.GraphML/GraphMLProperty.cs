using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public abstract class GraphMLProperty
	{
		public string Name
		{
			get;
			set;
		}

		public PropertyDomain Domain
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		protected GraphMLProperty()
		{
			Domain = PropertyDomain.All;
		}

		protected static string DomainToGraphML(PropertyDomain domain)
		{
			switch (domain)
			{
			case PropertyDomain.Node:
				return "node";
			case PropertyDomain.Arc:
				return "arc";
			case PropertyDomain.Graph:
				return "graph";
			default:
				return "all";
			}
		}

		protected static PropertyDomain ParseDomain(string s)
		{
			if (s != null)
			{
				if (s == "node")
				{
					return PropertyDomain.Node;
				}
				if (s == "edge")
				{
					return PropertyDomain.Arc;
				}
				if (s == "graph")
				{
					return PropertyDomain.Graph;
				}
			}
			return PropertyDomain.All;
		}

		protected virtual void LoadFromKeyElement(XElement xKey)
		{
			Name = xKey.Attribute("attr.name")?.Value;
			Domain = ParseDomain(xKey.Attribute("for").Value);
			Id = xKey.Attribute("id").Value;
			XElement x = Utils.ElementLocal(xKey, "default");
			ReadData(x, null);
		}

		public virtual XElement GetKeyElement()
		{
			XElement xElement = new XElement(GraphMLFormat.xmlns + "key");
			xElement.SetAttributeValue("attr.name", Name);
			xElement.SetAttributeValue("for", DomainToGraphML(Domain));
			xElement.SetAttributeValue("id", Id);
			XElement xElement2 = WriteData(null);
			if (xElement2 != null)
			{
				xElement2.Name = GraphMLFormat.xmlns + "default";
				xElement.Add(xElement2);
			}
			return xElement;
		}

		public abstract void ReadData(XElement x, object key);

		public abstract XElement WriteData(object key);
	}
}
