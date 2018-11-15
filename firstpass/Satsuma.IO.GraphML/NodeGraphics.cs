using System;
using System.Globalization;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class NodeGraphics
	{
		private readonly string[] nodeShapeToString = new string[11]
		{
			"rectangle",
			"roundrectangle",
			"ellipse",
			"parallelogram",
			"hexagon",
			"triangle",
			"rectangle3d",
			"octagon",
			"diamond",
			"trapezoid",
			"trapezoid2"
		};

		public double X
		{
			get;
			set;
		}

		public double Y
		{
			get;
			set;
		}

		public double Width
		{
			get;
			set;
		}

		public double Height
		{
			get;
			set;
		}

		public NodeShape Shape
		{
			get;
			set;
		}

		public NodeGraphics()
		{
			double num3 = X = (Y = 0.0);
			num3 = (Width = (Height = 10.0));
			Shape = NodeShape.Rectangle;
		}

		public NodeGraphics(XElement xData)
		{
			XElement xElement = Utils.ElementLocal(xData, "Geometry");
			if (xElement != null)
			{
				X = double.Parse(xElement.Attribute("x").Value, CultureInfo.InvariantCulture);
				Y = double.Parse(xElement.Attribute("y").Value, CultureInfo.InvariantCulture);
				Width = double.Parse(xElement.Attribute("width").Value, CultureInfo.InvariantCulture);
				Height = double.Parse(xElement.Attribute("height").Value, CultureInfo.InvariantCulture);
			}
			XElement xElement2 = Utils.ElementLocal(xData, "Shape");
			if (xElement2 != null)
			{
				Shape = ParseShape(xElement2.Attribute("type").Value);
			}
		}

		private NodeShape ParseShape(string s)
		{
			return (NodeShape)Math.Max(0, Array.IndexOf(nodeShapeToString, s));
		}

		private string ShapeToGraphML(NodeShape shape)
		{
			return nodeShapeToString[(int)shape];
		}

		public XElement ToXml()
		{
			return new XElement("dummy", new XElement(GraphMLFormat.xmlnsY + "ShapeNode", new XElement(GraphMLFormat.xmlnsY + "Geometry", new XAttribute("x", X.ToString(CultureInfo.InvariantCulture)), new XAttribute("y", Y.ToString(CultureInfo.InvariantCulture)), new XAttribute("width", Width.ToString(CultureInfo.InvariantCulture)), new XAttribute("height", Height.ToString(CultureInfo.InvariantCulture))), new XElement(GraphMLFormat.xmlnsY + "Shape", new XAttribute("type", ShapeToGraphML(Shape)))));
		}

		public override string ToString()
		{
			return ToXml().ToString();
		}
	}
}
