using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class GraphBase : KMonoBehaviour
{
	[Header("Axis")]
	public GraphAxis axis_x;

	public GraphAxis axis_y;

	[Header("References")]
	public GameObject prefab_guide_x;

	public GameObject prefab_guide_y;

	public GameObject guides_x;

	public GameObject guides_y;

	protected List<GameObject> guides = new List<GameObject>();

	public Vector2 GetRelativePosition(Vector2 absolute_point)
	{
		Vector2 zero = Vector2.zero;
		float num = axis_x.max_value - axis_x.min_value;
		float num2 = absolute_point.x - axis_x.min_value;
		zero.x = num2 / num;
		float num3 = axis_y.max_value - axis_y.min_value;
		float num4 = absolute_point.y - axis_y.min_value;
		zero.y = num4 / num3;
		return zero;
	}

	public Vector2 GetRelativeSize(Vector2 absolute_size)
	{
		return GetRelativePosition(absolute_size);
	}

	public void ClearGuides()
	{
		foreach (GameObject guide in guides)
		{
			if ((Object)guide != (Object)null)
			{
				Object.DestroyImmediate(guide);
			}
		}
		guides.Clear();
	}

	public void RefreshGuides()
	{
		ClearGuides();
		int num = 2;
		GameObject gameObject = Util.KInstantiateUI(prefab_guide_y, guides_y, true);
		gameObject.name = "guides_vertical";
		Vector2[] array = new Vector2[num * (int)(axis_x.range / axis_x.guide_frequency)];
		for (int i = 0; i < array.Length; i += num)
		{
			array[i] = GetRelativePosition(new Vector2((float)i * (axis_x.guide_frequency / (float)num), axis_y.min_value));
			array[i + 1] = GetRelativePosition(new Vector2((float)i * (axis_x.guide_frequency / (float)num), axis_y.max_value));
		}
		gameObject.GetComponent<UILineRenderer>().Points = array;
		guides.Add(gameObject);
		GameObject gameObject2 = Util.KInstantiateUI(prefab_guide_x, guides_x, true);
		gameObject2.name = "guides_horizontal";
		Vector2[] array2 = new Vector2[num * (int)(axis_y.range / axis_y.guide_frequency)];
		for (int j = 0; j < array2.Length; j += num)
		{
			array2[j] = GetRelativePosition(new Vector2(axis_x.min_value, (float)j * (axis_y.guide_frequency / (float)num)));
			array2[j + 1] = GetRelativePosition(new Vector2(axis_x.max_value, (float)j * (axis_y.guide_frequency / (float)num)));
		}
		gameObject2.GetComponent<UILineRenderer>().Points = array2;
		guides.Add(gameObject2);
	}
}
