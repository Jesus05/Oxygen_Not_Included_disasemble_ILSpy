using Klei;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TemplateCache
{
	private static string baseTemplatePath = null;

	private static Dictionary<string, TemplateContainer> templates = null;

	private const string defaultAssetFolder = "bases/";

	public static void Init()
	{
		templates = new Dictionary<string, TemplateContainer>();
		baseTemplatePath = Application.streamingAssetsPath + "/templates";
	}

	public static void Clear()
	{
		templates = null;
		baseTemplatePath = null;
	}

	public static string GetTemplatePath()
	{
		return baseTemplatePath;
	}

	public static TemplateContainer GetBaseStartingTemplate()
	{
		if (baseTemplatePath == null)
		{
			Init();
		}
		string filename = Path.Combine(baseTemplatePath, "bases/startingBase.yaml");
		return YamlIO<TemplateContainer>.LoadFile(filename, null);
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!templates.ContainsKey(templatePath))
		{
			templates.Add(templatePath, null);
		}
		if (templates[templatePath] == null)
		{
			string text = Path.Combine(baseTemplatePath, templatePath);
			TemplateContainer templateContainer = YamlIO<TemplateContainer>.LoadFile(text + ".yaml", null);
			if (templateContainer == null)
			{
				Debug.LogWarning("Missing template [" + text + ".yaml]", null);
			}
			templates[templatePath] = templateContainer;
		}
		return templates[templatePath];
	}

	public static List<string> CollectBaseTemplateNames(string folder = "bases/")
	{
		List<string> list = new List<string>();
		string path = Path.Combine(baseTemplatePath, folder);
		string[] files = Directory.GetFiles(path, "*.yaml");
		string[] array = files;
		foreach (string path2 in array)
		{
			string text = folder + Path.GetFileNameWithoutExtension(path2);
			list.Add(text);
			if (!templates.ContainsKey(text))
			{
				templates.Add(text, null);
			}
		}
		list.Sort((string x, string y) => x.CompareTo(y));
		return list;
	}

	public static List<TemplateContainer> CollectBaseTemplateAssets(string folder = "bases/")
	{
		List<TemplateContainer> list = new List<TemplateContainer>();
		string path = Path.Combine(baseTemplatePath, folder);
		string[] files = Directory.GetFiles(path, "*.yaml");
		string[] array = files;
		foreach (string filename in array)
		{
			list.Add(YamlIO<TemplateContainer>.LoadFile(filename, null));
		}
		list.Sort(delegate(TemplateContainer x, TemplateContainer y)
		{
			if (y.priority - x.priority != 0)
			{
				return y.priority - x.priority;
			}
			return x.name.CompareTo(y.name);
		});
		return list;
	}
}
