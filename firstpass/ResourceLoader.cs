using Klei;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader<T> where T : Resource, new()
{
	public List<T> resources = new List<T>();

	public ResourceLoader()
	{
	}

	public ResourceLoader(TextAsset file)
	{
		Load(file);
	}

	public ResourceLoader(string text, string name)
	{
		Load(text, name);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return resources.GetEnumerator();
	}

	public void Load(string text, string name)
	{
		string[,] array = CSVReader.SplitCsvGrid(text, name);
		int length = array.GetLength(1);
		for (int i = 1; i < length; i++)
		{
			if (!array[0, i].IsNullOrWhiteSpace())
			{
				T val = new T();
				CSVUtil.ParseData<T>((object)val, array, i);
				if (!val.Disabled)
				{
					resources.Add(val);
				}
			}
		}
	}

	public virtual void Load(TextAsset file)
	{
		if ((Object)file == (Object)null)
		{
			Debug.LogWarning("Missing resource file of type: " + typeof(T).Name);
		}
		else
		{
			Load(file.text, file.name);
		}
	}
}
