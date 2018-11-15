using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Satsuma.IO
{
	public sealed class SimpleGraphFormat
	{
		public IGraph Graph
		{
			get;
			set;
		}

		public IList<Dictionary<Arc, string>> Extensions
		{
			get;
			private set;
		}

		public int StartIndex
		{
			get;
			set;
		}

		public SimpleGraphFormat()
		{
			Extensions = new List<Dictionary<Arc, string>>();
		}

		public Node[] Load(TextReader reader, Directedness directedness)
		{
			if (Graph == null)
			{
				Graph = new CustomGraph();
			}
			IBuildableGraph buildableGraph = (IBuildableGraph)Graph;
			buildableGraph.Clear();
			Regex regex = new Regex("\\s+");
			string[] array = regex.Split(reader.ReadLine());
			int num = int.Parse(array[0], CultureInfo.InvariantCulture);
			int num2 = int.Parse(array[1], CultureInfo.InvariantCulture);
			Node[] array2 = new Node[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = buildableGraph.AddNode();
			}
			Extensions.Clear();
			for (int j = 0; j < num2; j++)
			{
				array = regex.Split(reader.ReadLine());
				int num3 = (int)(long.Parse(array[0], CultureInfo.InvariantCulture) - StartIndex);
				int num4 = (int)(long.Parse(array[1], CultureInfo.InvariantCulture) - StartIndex);
				Arc key = buildableGraph.AddArc(array2[num3], array2[num4], directedness);
				int num5 = array.Length - 2;
				for (int k = 0; k < num5 - Extensions.Count; k++)
				{
					Extensions.Add(new Dictionary<Arc, string>());
				}
				for (int l = 0; l < num5; l++)
				{
					Extensions[l][key] = array[2 + l];
				}
			}
			return array2;
		}

		public Node[] Load(string filename, Directedness directedness)
		{
			using (StreamReader reader = new StreamReader(filename))
			{
				return Load(reader, directedness);
			}
		}

		public void Save(TextWriter writer)
		{
			Regex regex = new Regex("\\s");
			writer.WriteLine(Graph.NodeCount() + " " + Graph.ArcCount(ArcFilter.All));
			Dictionary<Node, long> dictionary = new Dictionary<Node, long>();
			long num = StartIndex;
			foreach (Arc item in Graph.Arcs(ArcFilter.All))
			{
				Node node = Graph.U(item);
				if (!dictionary.TryGetValue(node, out long value))
				{
					Dictionary<Node, long> dictionary2 = dictionary;
					Node key = node;
					long num2 = num;
					num = num2 + 1;
					value = num2;
					dictionary2[key] = num2;
				}
				Node node2 = Graph.V(item);
				if (!dictionary.TryGetValue(node2, out long value2))
				{
					Dictionary<Node, long> dictionary3 = dictionary;
					Node key2 = node2;
					long num3 = num;
					num = num3 + 1;
					value2 = num3;
					dictionary3[key2] = num3;
				}
				writer.Write(value + " " + value2);
				foreach (Dictionary<Arc, string> extension in Extensions)
				{
					extension.TryGetValue(item, out string value3);
					if (string.IsNullOrEmpty(value3) || regex.IsMatch(value3))
					{
						throw new ArgumentException("Extension value is empty or contains whitespaces.");
					}
					writer.Write(' ' + extension[item]);
				}
				writer.WriteLine();
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
