using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Klei
{
	public class CSVReader
	{
		private struct ParseWorkItem : IWorkItem<object>
		{
			public string line;

			public string[] row;

			public void Run(object shared_data)
			{
				row = SplitCsvLine(line);
			}
		}

		private static Regex regex = new Regex("(((?<x>(?=[,\\r\\n]+))|\"(?<x>([^\"]|\"\")+)\"|(?<x>[^,\\r\\n]+)),?)", RegexOptions.ExplicitCapture);

		public static void DebugOutputGrid(string[,] grid)
		{
			string text = string.Empty;
			for (int i = 0; i < grid.GetUpperBound(1); i++)
			{
				for (int j = 0; j < grid.GetUpperBound(0); j++)
				{
					text += grid[j, i];
					text += "|";
				}
				text += "\n";
			}
			Debug.Log(text, null);
		}

		public static string[,] SplitCsvGrid(string csvText, string csv_name)
		{
			string[] array = csvText.Split('\n', '\r');
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Length != 0 && !text.StartsWith("#"))
				{
					list.Add(text);
				}
			}
			List<string> list2 = new List<string>();
			for (int j = 0; j < list.Count; j++)
			{
				string text2 = list[j];
				int num = 0;
				for (int k = 0; k < text2.Length; k++)
				{
					if (text2[k] == '"')
					{
						num++;
					}
				}
				if (num % 2 == 1)
				{
					text2 = (list[j + 1] = list[j] + '\n' + list[j + 1]);
				}
				else
				{
					list2.Add(text2);
				}
			}
			list2.RemoveAll((string x) => x.StartsWith("#"));
			string[][] array3 = new string[list2.Count][];
			for (int l = 0; l < list2.Count; l++)
			{
				array3[l] = SplitCsvLine(list2[l]);
			}
			int num2 = 0;
			for (int m = 0; m < array3.Length; m++)
			{
				num2 = Mathf.Max(num2, array3[m].Length);
			}
			string[,] array4 = new string[num2 + 1, array3.Length + 1];
			for (int n = 0; n < array3.Length; n++)
			{
				string[] array5 = array3[n];
				for (int num3 = 0; num3 < array5.Length; num3++)
				{
					string[,] array6 = array4;
					int num4 = num3;
					int num5 = n;
					string obj = array5[num3];
					array6[num4, num5] = obj;
					string[,] array7 = array4;
					int num6 = num3;
					int num7 = n;
					string text4 = array4[num3, n].Replace("\"\"", "\"");
					array7[num6, num7] = text4;
				}
			}
			return array4;
		}

		public static string[] SplitCsvLine(string line)
		{
			line = line.Replace("\n\n", "\n");
			return (from Match m in regex.Matches(line)
			select m.Groups[1].Value).ToArray();
		}
	}
}
