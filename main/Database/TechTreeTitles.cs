using UnityEngine;

namespace Database
{
	public class TechTreeTitles : ResourceSet<TechTreeTitle>
	{
		public TechTreeTitles(ResourceSet parent)
			: base("TreeTitles", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			foreach (ResourceTreeNode item in resourceTreeLoader)
			{
				string a = item.Id.Substring(0, 1);
				if (string.Equals(a, "_"))
				{
					new TechTreeTitle(item.Id, this, Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + item.Id.ToUpper()), item);
				}
			}
		}
	}
}
