using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ResourceTreeLoader<T> : ResourceLoader<T> where T : ResourceTreeNode, new()
{
	public ResourceTreeLoader(TextAsset file)
		: base(file)
	{
	}

	public override void Load(TextAsset file)
	{
		Dictionary<string, ResourceTreeNode> dictionary = new Dictionary<string, ResourceTreeNode>();
		using (XmlReader xmlReader = XmlReader.Create(new StringReader(file.text)))
		{
			while (xmlReader.ReadToFollowing("node"))
			{
				xmlReader.MoveToFirstAttribute();
				string value = xmlReader.Value;
				float nodeX = 0f;
				float nodeY = 0f;
				float width = 40f;
				float height = 20f;
				if (xmlReader.ReadToFollowing("Geometry"))
				{
					xmlReader.MoveToAttribute("x");
					nodeX = float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("y");
					nodeY = 0f - float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("width");
					width = float.Parse(xmlReader.Value);
					xmlReader.MoveToAttribute("height");
					height = float.Parse(xmlReader.Value);
				}
				if (xmlReader.ReadToFollowing("NodeLabel"))
				{
					string text = xmlReader.ReadString();
					T val = new T();
					val.Id = text;
					val.Name = text;
					val.nodeX = nodeX;
					val.nodeY = nodeY;
					val.width = width;
					val.height = height;
					dictionary[value] = val;
					resources.Add(val);
				}
			}
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(file.text);
		XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/graphml/graph/edge");
		IEnumerator enumerator = xmlNodeList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				XmlNode xmlNode = (XmlNode)enumerator.Current;
				ResourceTreeNode value2 = null;
				dictionary.TryGetValue(xmlNode.Attributes["source"].Value, out value2);
				ResourceTreeNode value3 = null;
				dictionary.TryGetValue(xmlNode.Attributes["target"].Value, out value3);
				if (value2 != null && value3 != null)
				{
					value2.references.Add(value3);
					ResourceTreeNode.Edge edge = null;
					XmlNode xmlNode2 = null;
					IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							XmlNode xmlNode3 = (XmlNode)enumerator2.Current;
							if (xmlNode3.HasChildNodes)
							{
								xmlNode2 = xmlNode3.FirstChild;
								break;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					string name = xmlNode2.Name;
					ResourceTreeNode.Edge.EdgeType edgeType = (ResourceTreeNode.Edge.EdgeType)Enum.Parse(typeof(ResourceTreeNode.Edge.EdgeType), name);
					edge = new ResourceTreeNode.Edge(value2, value3, edgeType);
					IEnumerator enumerator3 = xmlNode2.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							XmlNode xmlNode4 = (XmlNode)enumerator3.Current;
							if (!(xmlNode4.Name != "Path"))
							{
								edge.sourceOffset = new Vector2f(float.Parse(xmlNode4.Attributes["sx"].Value), 0f - float.Parse(xmlNode4.Attributes["sy"].Value));
								edge.targetOffset = new Vector2f(float.Parse(xmlNode4.Attributes["tx"].Value), 0f - float.Parse(xmlNode4.Attributes["ty"].Value));
								IEnumerator enumerator4 = xmlNode4.ChildNodes.GetEnumerator();
								try
								{
									while (enumerator4.MoveNext())
									{
										XmlNode xmlNode5 = (XmlNode)enumerator4.Current;
										Vector2f point = new Vector2f(float.Parse(xmlNode5.Attributes["x"].Value), 0f - float.Parse(xmlNode5.Attributes["y"].Value));
										edge.AddToPath(point);
									}
								}
								finally
								{
									IDisposable disposable2;
									if ((disposable2 = (enumerator4 as IDisposable)) != null)
									{
										disposable2.Dispose();
									}
								}
								break;
							}
						}
					}
					finally
					{
						IDisposable disposable3;
						if ((disposable3 = (enumerator3 as IDisposable)) != null)
						{
							disposable3.Dispose();
						}
					}
					value2.edges.Add(edge);
				}
			}
		}
		finally
		{
			IDisposable disposable4;
			if ((disposable4 = (enumerator as IDisposable)) != null)
			{
				disposable4.Dispose();
			}
		}
	}
}
