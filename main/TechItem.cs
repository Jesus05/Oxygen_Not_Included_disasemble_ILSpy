using System;
using UnityEngine;

public class TechItem : Resource
{
	public string description;

	public Func<string, bool, Sprite> getUISprite;

	public Tech parentTech;

	public TechItem(string id, ResourceSet parent, string name, string description, Func<string, bool, Sprite> getUISprite, Tech parentTech)
		: base(id, parent, name)
	{
		this.description = description;
		this.getUISprite = getUISprite;
		this.parentTech = parentTech;
	}

	public Sprite UISprite()
	{
		return getUISprite("ui", false);
	}

	public bool IsComplete()
	{
		return parentTech.IsComplete();
	}
}
