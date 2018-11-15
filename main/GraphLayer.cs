using UnityEngine;

[RequireComponent(typeof(GraphBase))]
public class GraphLayer : KMonoBehaviour
{
	[MyCmpReq]
	protected GraphBase graph_base;

	public GraphBase graph
	{
		get
		{
			if ((Object)graph_base == (Object)null)
			{
				graph_base = GetComponent<GraphBase>();
			}
			return graph_base;
		}
	}
}
