using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using TemplateClasses;
using UnityEngine;

[Serializable]
public class TemplateContainer : YamlIO<TemplateContainer>
{
	[Serializable]
	public class Info : YamlIO<Info>
	{
		public Vector2f size
		{
			get;
			set;
		}

		public int area
		{
			get;
			set;
		}

		public Tag[] tags
		{
			get;
			set;
		}
	}

	public string name
	{
		get;
		private set;
	}

	public int priority
	{
		get;
		set;
	}

	public Info info
	{
		get;
		set;
	}

	public List<Cell> cells
	{
		get;
		set;
	}

	public List<Prefab> buildings
	{
		get;
		set;
	}

	public List<Prefab> pickupables
	{
		get;
		set;
	}

	public List<Prefab> elementalOres
	{
		get;
		set;
	}

	public List<Prefab> otherEntities
	{
		get;
		set;
	}

	public TemplateContainer()
	{
		Init();
	}

	public void Init(List<Cell> _cells, List<Prefab> _buildings, List<Prefab> _pickupables, List<Prefab> _elementalOres, List<Prefab> _otherEntities)
	{
		cells = _cells;
		buildings = _buildings;
		pickupables = _pickupables;
		elementalOres = _elementalOres;
		otherEntities = _otherEntities;
		info = new Info();
		RefreshInfo();
	}

	public void Init()
	{
		cells = new List<Cell>();
		buildings = new List<Prefab>();
		pickupables = new List<Prefab>();
		elementalOres = new List<Prefab>();
		otherEntities = new List<Prefab>();
		info = new Info();
	}

	public void Init(TemplateContainer template)
	{
		cells = new List<Cell>(template.cells);
		buildings = new List<Prefab>(template.buildings);
		pickupables = new List<Prefab>(template.pickupables);
		elementalOres = new List<Prefab>(template.elementalOres);
		otherEntities = new List<Prefab>(template.otherEntities);
		info = new Info();
		info.size = template.info.size;
		info.area = template.info.area;
		info.tags = (Tag[])template.info.tags.Clone();
	}

	public void RefreshInfo()
	{
		int num = 1;
		int num2 = -1;
		int num3 = 1;
		int num4 = -1;
		foreach (Cell cell in cells)
		{
			if (cell.location_x < num)
			{
				num = cell.location_x;
			}
			if (cell.location_x > num2)
			{
				num2 = cell.location_x;
			}
			if (cell.location_y < num3)
			{
				num3 = cell.location_y;
			}
			if (cell.location_y > num4)
			{
				num4 = cell.location_y;
			}
			info.size = new Vector2((float)(1 + (num2 - num)), (float)(1 + (num4 - num3)));
			info.area = cells.Count;
		}
	}

	public void SaveToYaml(string save_name)
	{
		string text = save_name;
		while (text.Contains("/"))
		{
			int num = text.IndexOf('/') + 1;
			if (text.Length > num)
			{
				text = text.Substring(num);
			}
		}
		name = text;
		string templatePath = TemplateCache.GetTemplatePath();
		if (!File.Exists(templatePath))
		{
			Directory.CreateDirectory(templatePath);
		}
		Save(templatePath + "/" + save_name + ".yaml");
	}
}
