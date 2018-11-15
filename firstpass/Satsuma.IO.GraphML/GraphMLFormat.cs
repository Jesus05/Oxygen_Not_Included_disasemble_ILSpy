using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class GraphMLFormat
	{
		internal static readonly XNamespace xmlns = "http://graphml.graphdrawing.org/xmlns";

		private static readonly XNamespace xmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance";

		internal static readonly XNamespace xmlnsY = "http://www.yworks.com/xml/graphml";

		private static readonly XNamespace xmlnsYed = "http://www.yworks.com/xml/yed/3";

		private const string xsiSchemaLocation = "http://graphml.graphdrawing.org/xmlns\nhttp://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd";

		private readonly List<Func<XElement, GraphMLProperty>> PropertyLoaders;

		public IGraph Graph
		{
			get;
			set;
		}

		public IList<GraphMLProperty> Properties
		{
			get;
			private set;
		}

		public GraphMLFormat()
		{
			Properties = new List<GraphMLProperty>();
			PropertyLoaders = new List<Func<XElement, GraphMLProperty>>
			{
				(XElement x) => new StandardProperty<bool>(x),
				(XElement x) => new StandardProperty<double>(x),
				(XElement x) => new StandardProperty<float>(x),
				(XElement x) => new StandardProperty<int>(x),
				(XElement x) => new StandardProperty<long>(x),
				(XElement x) => new StandardProperty<string>(x),
				(XElement x) => new NodeGraphicsProperty(x)
			};
		}

		public void RegisterPropertyLoader(Func<XElement, GraphMLProperty> loader)
		{
			PropertyLoaders.Add(loader);
		}

		private static void ReadProperties(Dictionary<string, GraphMLProperty> propertyById, XElement x, object obj)
		{
			foreach (XElement item in Utils.ElementsLocal(x, "data"))
			{
				if (propertyById.TryGetValue(item.Attribute("key").Value, out GraphMLProperty value))
				{
					value.ReadData(x, obj);
				}
			}
		}

		public void Load(XDocument doc)
		{
			if (Graph == null)
			{
				Graph = new CustomGraph();
			}
			IBuildableGraph buildableGraph = (IBuildableGraph)Graph;
			buildableGraph.Clear();
			XElement root = doc.Root;
			Properties.Clear();
			Dictionary<string, GraphMLProperty> dictionary = new Dictionary<string, GraphMLProperty>();
			foreach (XElement item in Utils.ElementsLocal(root, "key"))
			{
				using (List<Func<XElement, GraphMLProperty>>.Enumerator enumerator2 = PropertyLoaders.GetEnumerator())
				{
					while (true)
					{
						if (enumerator2.MoveNext())
						{
							Func<XElement, GraphMLProperty> current2 = enumerator2.Current;
							try
							{
								GraphMLProperty graphMLProperty = current2(item);
								Properties.Add(graphMLProperty);
								dictionary[graphMLProperty.Id] = graphMLProperty;
							}
							catch (ArgumentException)
							{
								continue;
							}
						}
						break;
					}
				}
			}
			XElement xElement = Utils.ElementLocal(root, "graph");
			Directedness directedness = (!(xElement.Attribute("edgedefault").Value == "directed")) ? Directedness.Undirected : Directedness.Directed;
			ReadProperties(dictionary, xElement, Graph);
			Dictionary<string, Node> dictionary2 = new Dictionary<string, Node>();
			foreach (XElement item2 in Utils.ElementsLocal(xElement, "node"))
			{
				Node node = buildableGraph.AddNode();
				dictionary2[item2.Attribute("id").Value] = node;
				ReadProperties(dictionary, item2, node);
			}
			foreach (XElement item3 in Utils.ElementsLocal(xElement, "edge"))
			{
				Node u = dictionary2[item3.Attribute("source").Value];
				Node v = dictionary2[item3.Attribute("target").Value];
				Directedness directedness2 = directedness;
				XAttribute xAttribute = item3.Attribute("directed");
				if (xAttribute != null)
				{
					directedness2 = ((!(xAttribute.Value == "true")) ? Directedness.Undirected : Directedness.Directed);
				}
				Arc arc = buildableGraph.AddArc(u, v, directedness2);
				ReadProperties(dictionary, item3, arc);
			}
		}

		public void Load(XmlReader xml)
		{
			XDocument doc = XDocument.Load(xml);
			Load(doc);
		}

		public void Load(TextReader reader)
		{
			using (XmlReader xml = XmlReader.Create(reader))
			{
				Load(xml);
			}
		}

		public void Load(string filename)
		{
			using (StreamReader reader = new StreamReader(filename))
			{
				Load(reader);
			}
		}

		private void DefinePropertyValues(XmlWriter xml, object obj)
		{
			foreach (GraphMLProperty property in Properties)
			{
				XElement xElement = property.WriteData(obj);
				if (xElement != null)
				{
					xElement.Name = xmlns + "data";
					xElement.SetAttributeValue("key", property.Id);
					xElement.WriteTo(xml);
				}
			}
		}

		private void Save(XmlWriter xml)
		{
			xml.WriteStartDocument();
			xml.WriteStartElement("graphml", xmlns.NamespaceName);
			xml.WriteAttributeString("xmlns", "xsi", null, xmlnsXsi.NamespaceName);
			xml.WriteAttributeString("xmlns", "y", null, xmlnsY.NamespaceName);
			xml.WriteAttributeString("xmlns", "yed", null, xmlnsYed.NamespaceName);
			xml.WriteAttributeString("xsi", "schemaLocation", null, "http://graphml.graphdrawing.org/xmlns\nhttp://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd");
			for (int i = 0; i < Properties.Count; i++)
			{
				GraphMLProperty graphMLProperty = Properties[i];
				graphMLProperty.Id = "d" + i;
				graphMLProperty.GetKeyElement().WriteTo(xml);
			}
			xml.WriteStartElement("graph", xmlns.NamespaceName);
			xml.WriteAttributeString("id", "G");
			xml.WriteAttributeString("edgedefault", "directed");
			xml.WriteAttributeString("parse.nodes", Graph.NodeCount().ToString(CultureInfo.InvariantCulture));
			xml.WriteAttributeString("parse.edges", Graph.ArcCount(ArcFilter.All).ToString(CultureInfo.InvariantCulture));
			xml.WriteAttributeString("parse.order", "nodesfirst");
			DefinePropertyValues(xml, Graph);
			foreach (Node item in Graph.Nodes())
			{
				xml.WriteStartElement("node", xmlns.NamespaceName);
				xml.WriteAttributeString("id", item.Id.ToString(CultureInfo.InvariantCulture));
				DefinePropertyValues(xml, item);
				xml.WriteEndElement();
			}
			foreach (Arc item2 in Graph.Arcs(ArcFilter.All))
			{
				xml.WriteStartElement("edge", xmlns.NamespaceName);
				xml.WriteAttributeString("id", item2.Id.ToString(CultureInfo.InvariantCulture));
				if (Graph.IsEdge(item2))
				{
					xml.WriteAttributeString("directed", "false");
				}
				xml.WriteAttributeString("source", Graph.U(item2).Id.ToString(CultureInfo.InvariantCulture));
				xml.WriteAttributeString("target", Graph.V(item2).Id.ToString(CultureInfo.InvariantCulture));
				DefinePropertyValues(xml, item2);
				xml.WriteEndElement();
			}
			xml.WriteEndElement();
			xml.WriteEndElement();
		}

		public void Save(TextWriter writer)
		{
			using (XmlWriter xml = XmlWriter.Create(writer))
			{
				Save(xml);
			}
		}

		public void Save(string filename)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				Save(writer);
			}
		}
	}
}
