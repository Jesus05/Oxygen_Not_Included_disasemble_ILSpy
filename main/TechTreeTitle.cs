using UnityEngine;

public class TechTreeTitle : Resource
{
	public string desc;

	private ResourceTreeNode node;

	public Vector2 center => node.center;

	public float width => node.width;

	public float height => node.height;

	public TechTreeTitle(string id, ResourceSet parent, string name, ResourceTreeNode node)
		: base(id, parent, name)
	{
		this.node = node;
	}
}
