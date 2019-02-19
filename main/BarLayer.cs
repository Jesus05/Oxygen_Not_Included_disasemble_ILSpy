using System.Collections.Generic;
using UnityEngine;

public class BarLayer : GraphLayer
{
	public GameObject bar_container;

	public GameObject prefab_bar;

	public GraphedBarFormatting[] bar_formats;

	private List<GraphedBar> bars = new List<GraphedBar>();

	public int bar_count => bars.Count;

	public void NewBar(int[] values, float x_position, string ID = "")
	{
		GameObject gameObject = Util.KInstantiateUI(prefab_bar, bar_container, true);
		if (ID == string.Empty)
		{
			ID = bars.Count.ToString();
		}
		gameObject.name = $"bar_{ID}";
		GraphedBar component = gameObject.GetComponent<GraphedBar>();
		component.SetFormat(bar_formats[bars.Count % bar_formats.Length]);
		int[] array = new int[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			int[] array2 = array;
			int num = i;
			float height = base.graph.rectTransform().rect.height;
			Vector2 relativeSize = base.graph.GetRelativeSize(new Vector2(0f, (float)values[i]));
			array2[num] = (int)(height * relativeSize.y);
		}
		GraphedBar graphedBar = component;
		int[] values2 = array;
		Vector2 relativePosition = base.graph.GetRelativePosition(new Vector2(x_position, 0f));
		graphedBar.SetValues(values2, relativePosition.x);
		bars.Add(component);
	}

	public void ClearBars()
	{
		foreach (GraphedBar bar in bars)
		{
			if ((Object)bar != (Object)null && (Object)bar.gameObject != (Object)null)
			{
				Object.DestroyImmediate(bar.gameObject);
			}
		}
		bars.Clear();
	}
}
