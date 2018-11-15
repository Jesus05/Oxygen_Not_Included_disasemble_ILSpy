using Klei;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TemplateCache
{
	private struct ParseTemplateWorkItem : IWorkItem<object>
	{
		public string path;

		public TemplateContainer template;

		public void Run(object shared_data)
		{
			template = YamlIO<TemplateContainer>.LoadFile(path);
		}
	}

	private static string baseTemplatePath;

	private static Dictionary<string, TemplateContainer> templates;

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
		return YamlIO<TemplateContainer>.LoadFile(filename);
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
			TemplateContainer templateContainer = YamlIO<TemplateContainer>.LoadFile(text + ".yaml");
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
		WorkItemCollection<ParseTemplateWorkItem, object> workItemCollection = new WorkItemCollection<ParseTemplateWorkItem, object>();
		workItemCollection.Reset(null);
		string[] array = files;
		foreach (string path2 in array)
		{
			workItemCollection.Add(new ParseTemplateWorkItem
			{
				path = path2
			});
		}
		GlobalJobManager.Run(workItemCollection);
		for (int j = 0; j < workItemCollection.Count; j++)
		{
			ParseTemplateWorkItem workItem = workItemCollection.GetWorkItem(j);
			if (workItem.template != null)
			{
				List<TemplateContainer> list2 = list;
				ParseTemplateWorkItem workItem2 = workItemCollection.GetWorkItem(j);
				list2.Add(workItem2.template);
			}
		}
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
