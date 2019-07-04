using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Satsuma.IO
{
	public sealed class LemonGraphFormat
	{
		public IGraph Graph
		{
			get;
			set;
		}

		public Dictionary<string, Dictionary<Node, string>> NodeMaps
		{
			get;
			private set;
		}

		public Dictionary<string, Dictionary<Arc, string>> ArcMaps
		{
			get;
			private set;
		}

		public Dictionary<string, string> Attributes
		{
			get;
			private set;
		}

		public LemonGraphFormat()
		{
			NodeMaps = new Dictionary<string, Dictionary<Node, string>>();
			ArcMaps = new Dictionary<string, Dictionary<Arc, string>>();
			Attributes = new Dictionary<string, string>();
		}

		private static string Escape(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in s)
			{
				switch (c)
				{
				case '\n':
					stringBuilder.Append("\\n");
					break;
				case '\r':
					stringBuilder.Append("\\r");
					break;
				case '\t':
					stringBuilder.Append("\\t");
					break;
				case '"':
					stringBuilder.Append("\\\"");
					break;
				case '\\':
					stringBuilder.Append("\\\\");
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private static string Unescape(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in s)
			{
				if (flag)
				{
					switch (c)
					{
					case 'n':
						stringBuilder.Append('\n');
						break;
					case 'r':
						stringBuilder.Append('\r');
						break;
					case 't':
						stringBuilder.Append('\t');
						break;
					default:
						stringBuilder.Append(c);
						break;
					}
					flag = false;
				}
				else
				{
					flag = (c == '\\');
					if (!flag)
					{
						stringBuilder.Append(c);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public void Load(TextReader reader, Directedness? directedness)
		{
			if (Graph == null)
			{
				Graph = new CustomGraph();
			}
			IBuildableGraph buildableGraph = (IBuildableGraph)Graph;
			buildableGraph.Clear();
			NodeMaps.Clear();
			Dictionary<string, Node> dictionary = new Dictionary<string, Node>();
			ArcMaps.Clear();
			Attributes.Clear();
			Regex regex = new Regex("\\s*(?:(\"(?:\\\"|.)*\")|(\\S+))\\s*", RegexOptions.None);
			string text = "";
			Directedness directedness2 = Directedness.Directed;
			bool flag = false;
			List<string> list = null;
			int num = -1;
			while (true)
			{
				string text2 = reader.ReadLine();
				if (text2 == null)
				{
					break;
				}
				text2 = text2.Trim();
				if (!(text2 == "") && text2[0] != '#')
				{
					List<string> list2 = regex.Matches(text2).Cast<Match>().Select(delegate(Match m)
					{
						string text5 = m.Value;
						if (!(text5 == ""))
						{
							if (text5[0] == '"' && text5[text5.Length - 1] == '"')
							{
								text5 = Unescape(text5.Substring(1, text5.Length - 2));
							}
							return text5;
						}
						return text5;
					})
						.ToList();
					string text3 = list2.First();
					if (text2[0] == '@')
					{
						text = text3.Substring(1);
						directedness2 = (directedness.HasValue ? directedness.Value : ((!(text == "arcs")) ? Directedness.Undirected : Directedness.Directed));
						flag = true;
					}
					else
					{
						if (text != null)
						{
							if (!(text == "nodes") && !(text == "red_nodes") && !(text == "blue_nodes"))
							{
								if (!(text == "arcs") && !(text == "edges"))
								{
									if (text == "attributes")
									{
										Attributes[list2[0]] = list2[1];
									}
								}
								else if (flag)
								{
									list = list2;
									foreach (string item in list)
									{
										if (!ArcMaps.ContainsKey(item))
										{
											ArcMaps[item] = new Dictionary<Arc, string>();
										}
									}
								}
								else
								{
									Node u = dictionary[list2[0]];
									Node v = dictionary[list2[1]];
									Arc key = buildableGraph.AddArc(u, v, directedness2);
									for (int i = 2; i < list2.Count; i++)
									{
										ArcMaps[list[i - 2]][key] = list2[i];
									}
								}
							}
							else if (flag)
							{
								list = list2;
								for (int j = 0; j < list.Count; j++)
								{
									string text4 = list[j];
									if (text4 == "label")
									{
										num = j;
									}
									if (!NodeMaps.ContainsKey(text4))
									{
										NodeMaps[text4] = new Dictionary<Node, string>();
									}
								}
							}
							else
							{
								Node node = buildableGraph.AddNode();
								for (int k = 0; k < list2.Count; k++)
								{
									NodeMaps[list[k]][node] = list2[k];
									if (k == num)
									{
										dictionary[list2[k]] = node;
									}
								}
							}
						}
						flag = false;
					}
				}
			}
		}

		public void Load(string filename, Directedness? directedness)
		{
			using (StreamReader reader = new StreamReader(filename))
			{
				Load(reader, directedness);
			}
		}

		public void Save(TextWriter writer, IEnumerable<string> comment = null)
		{
			if (comment != null)
			{
				foreach (string item in comment)
				{
					writer.WriteLine("# " + item);
				}
			}
			writer.WriteLine("@nodes");
			writer.Write("label");
			foreach (KeyValuePair<string, Dictionary<Node, string>> nodeMap in NodeMaps)
			{
				if (nodeMap.Key != "label")
				{
					writer.Write(' ' + nodeMap.Key);
				}
			}
			writer.WriteLine();
			foreach (Node item2 in Graph.Nodes())
			{
				writer.Write(item2.Id);
				foreach (KeyValuePair<string, Dictionary<Node, string>> nodeMap2 in NodeMaps)
				{
					if (nodeMap2.Key != "label")
					{
						if (!nodeMap2.Value.TryGetValue(item2, out string value))
						{
							value = "";
						}
						writer.Write(" \"" + Escape(value) + '"');
					}
				}
				writer.WriteLine();
			}
			writer.WriteLine();
			for (int i = 0; i < 2; i++)
			{
				IEnumerable<Arc> enumerable = (i != 0) ? Graph.Arcs(ArcFilter.Edge) : (from arc in Graph.Arcs(ArcFilter.All)
				where !Graph.IsEdge(arc)
				select arc);
				writer.WriteLine((i != 0) ? "@edges" : "@arcs");
				if (ArcMaps.Count == 0)
				{
					writer.WriteLine('-');
				}
				else
				{
					foreach (KeyValuePair<string, Dictionary<Arc, string>> arcMap in ArcMaps)
					{
						writer.Write(arcMap.Key + ' ');
					}
					writer.WriteLine();
				}
				foreach (Arc item3 in enumerable)
				{
					writer.Write(Graph.U(item3).Id + 32 + Graph.V(item3).Id);
					foreach (KeyValuePair<string, Dictionary<Arc, string>> arcMap2 in ArcMaps)
					{
						if (!arcMap2.Value.TryGetValue(item3, out string value2))
						{
							value2 = "";
						}
						writer.Write(" \"" + Escape(value2) + '"');
					}
					writer.WriteLine();
				}
				writer.WriteLine();
			}
			if (Attributes.Count > 0)
			{
				writer.WriteLine("@attributes");
				foreach (KeyValuePair<string, string> attribute in Attributes)
				{
					writer.WriteLine('"' + Escape(attribute.Key) + "\" \"" + Escape(attribute.Value) + '"');
				}
				writer.WriteLine();
			}
		}

		public void Save(string filename, IEnumerable<string> comment = null)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				Save(writer, comment);
			}
		}
	}
}
