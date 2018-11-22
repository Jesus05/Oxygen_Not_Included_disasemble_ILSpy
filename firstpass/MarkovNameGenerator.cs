using System;
using System.Collections.Generic;

public class MarkovNameGenerator
{
	private Dictionary<string, List<char>> _chains = new Dictionary<string, List<char>>();

	private List<string> _samples = new List<string>();

	private List<string> _used = new List<string>();

	private Random _rnd = new Random();

	private int _order;

	private int _minLength;

	public string NextName
	{
		get
		{
			string text = "";
			do
			{
				int index = _rnd.Next(_samples.Count);
				int length = _samples[index].Length;
				text = _samples[index].Substring(_rnd.Next(0, _samples[index].Length - _order), _order);
				while (text.Length < length)
				{
					string token = text.Substring(text.Length - _order, _order);
					char letter = GetLetter(token);
					if (letter == '?')
					{
						break;
					}
					text += GetLetter(token);
				}
				if (text.Contains(" "))
				{
					string[] array = text.Split(' ');
					text = "";
					for (int i = 0; i < array.Length; i++)
					{
						if (!(array[i] == ""))
						{
							if (array[i].Length == 1)
							{
								array[i] = array[i].ToUpper();
							}
							else
							{
								array[i] = array[i].Substring(0, 1) + array[i].Substring(1).ToLower();
							}
							if (text != "")
							{
								text += " ";
							}
							text += array[i];
						}
					}
				}
				else
				{
					text = text.Substring(0, 1) + text.Substring(1).ToLower();
				}
			}
			while (_used.Contains(text) || text.Length < _minLength);
			_used.Add(text);
			return text;
		}
	}

	public MarkovNameGenerator(IEnumerable<string> sampleNames, int order, int minLength)
	{
		if (order < 1)
		{
			order = 1;
		}
		if (minLength < 1)
		{
			minLength = 1;
		}
		_order = order;
		_minLength = minLength;
		foreach (string sampleName in sampleNames)
		{
			string[] array = sampleName.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim().ToUpper();
				if (text2.Length >= order + 1)
				{
					_samples.Add(text2);
				}
			}
		}
		foreach (string sample in _samples)
		{
			for (int j = 0; j < sample.Length - order; j++)
			{
				string key = sample.Substring(j, order);
				List<char> list = null;
				if (_chains.ContainsKey(key))
				{
					list = _chains[key];
				}
				else
				{
					list = new List<char>();
					_chains[key] = list;
				}
				list.Add(sample[j + order]);
			}
		}
	}

	public void Reset()
	{
		_used.Clear();
	}

	private char GetLetter(string token)
	{
		if (_chains.ContainsKey(token))
		{
			List<char> list = _chains[token];
			int index = _rnd.Next(list.Count);
			return list[index];
		}
		return '?';
	}
}
