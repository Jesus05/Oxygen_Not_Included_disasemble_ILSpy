using Klei;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TemplateCache
{
	private static string baseTemplatePath;

	private static Dictionary<string, TemplateContainer> templates;

	private const string defaultAssetFolder = "bases";

	public static void Init()
	{
		templates = new Dictionary<string, TemplateContainer>();
		baseTemplatePath = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, "templates"));
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

	public static TemplateContainer GetStartingBaseTemplate(string startingTemplateName)
	{
		DebugUtil.Assert(startingTemplateName != null, "Tried loading a starting template named ", startingTemplateName);
		if (baseTemplatePath == null)
		{
			Init();
		}
		return GetTemplate(Path.Combine("bases", startingTemplateName));
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!templates.ContainsKey(templatePath))
		{
			templates.Add(templatePath, null);
		}
		if (templates[templatePath] == null)
		{
			string text = FileSystem.Normalize(Path.Combine(baseTemplatePath, templatePath));
			TemplateContainer templateContainer = YamlIO.LoadFile<TemplateContainer>(text + ".yaml", null, null);
			if (templateContainer == null)
			{
				Debug.LogWarning("Missing template [" + text + ".yaml]");
			}
			templates[templatePath] = templateContainer;
		}
		return templates[templatePath];
	}

	private static void GetAssetPaths(string folder, List<string> paths)
	{
		FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(baseTemplatePath, folder)), "*.yaml", paths);
	}

	public static List<string> CollectBaseTemplateNames(string folder = "bases")
	{
		List<string> list = new List<string>();
		ListPool<string, TemplateContainer>.PooledList pooledList = ListPool<string, TemplateContainer>.Allocate();
		GetAssetPaths(folder, pooledList);
		foreach (string item in pooledList)
		{
			string text = FileSystem.Normalize(Path.Combine(folder, Path.GetFileNameWithoutExtension(item)));
			list.Add(text);
			if (!templates.ContainsKey(text))
			{
				templates.Add(text, null);
			}
		}
		pooledList.Recycle();
		list.Sort((string x, string y) => x.CompareTo(y));
		return list;
	}

	public static List<TemplateContainer> CollectBaseTemplateAssets(string folder = "bases")
	{
		List<TemplateContainer> list = new List<TemplateContainer>();
		ListPool<string, TemplateContainer>.PooledList pooledList = ListPool<string, TemplateContainer>.Allocate();
		GetAssetPaths(folder, pooledList);
		foreach (string item in pooledList)
		{
			list.Add(YamlIO.LoadFile<TemplateContainer>(item, null, null));
		}
		pooledList.Recycle();
		list.Sort(delegate(TemplateContainer x, TemplateContainer y)
		{
			if (y.priority - x.priority == 0)
			{
				return x.name.CompareTo(y.name);
			}
			return y.priority - x.priority;
		});
		return list;
	}
}
