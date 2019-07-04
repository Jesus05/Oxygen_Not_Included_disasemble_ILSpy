using System.Collections.Generic;
using TMPro;
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

	public GameObject prefab_guide_horizontal_label;

	public GameObject prefab_guide_vertical_label;

	public GameObject guides_x;

	public GameObject guides_y;

	public LocText label_title;

	public LocText label_x;

	public LocText label_y;

	public string graphName;

	protected List<GameObject> guides = new List<GameObject>();

	public Vector2 GetRelativePosition(Vector2 absolute_point)
	{
		Vector2 zero = Vector2.zero;
		float num = Mathf.Max(1f, axis_x.max_value - axis_x.min_value);
		float num2 = absolute_point.x - axis_x.min_value;
		zero.x = num2 / num;
		float num3 = Mathf.Max(1f, axis_y.max_value - axis_y.min_value);
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
			Vector2 absolute_point = new Vector2((float)i * (axis_x.guide_frequency / (float)num), axis_y.min_value);
			array[i] = GetRelativePosition(absolute_point);
			Vector2 absolute_point2 = new Vector2((float)i * (axis_x.guide_frequency / (float)num), axis_y.max_value);
			array[i + 1] = GetRelativePosition(absolute_point2);
			GameObject gameObject2 = Util.KInstantiateUI(prefab_guide_vertical_label, gameObject, true);
			gameObject2.GetComponent<LocText>().alignment = TextAlignmentOptions.Bottom;
			gameObject2.GetComponent<LocText>().text = ((int)axis_x.guide_frequency * (i / num)).ToString();
			gameObject2.rectTransform().SetLocalPosition(new Vector2((float)i * (base.gameObject.rectTransform().rect.width / (float)array.Length), 4f) - base.gameObject.rectTransform().rect.size / 2f);
		}
		gameObject.GetComponent<UILineRenderer>().Points = array;
		guides.Add(gameObject);
		GameObject gameObject3 = Util.KInstantiateUI(prefab_guide_x, guides_x, true);
		gameObject3.name = "guides_horizontal";
		Vector2[] array2 = new Vector2[num * (int)(axis_y.range / axis_y.guide_frequency)];
		for (int j = 0; j < array2.Length; j += num)
		{
			Vector2 absolute_point3 = new Vector2(axis_x.min_value, (float)j * (axis_y.guide_frequency / (float)num));
			array2[j] = GetRelativePosition(absolute_point3);
			Vector2 absolute_point4 = new Vector2(axis_x.max_value, (float)j * (axis_y.guide_frequency / (float)num));
			array2[j + 1] = GetRelativePosition(absolute_point4);
			GameObject gameObject4 = Util.KInstantiateUI(prefab_guide_horizontal_label, gameObject3, true);
			gameObject4.GetComponent<LocText>().alignment = TextAlignmentOptions.MidlineLeft;
			gameObject4.GetComponent<LocText>().text = ((int)axis_y.guide_frequency * (j / num)).ToString();
			gameObject4.rectTransform().SetLocalPosition(new Vector2(8f, (float)j * (base.gameObject.rectTransform().rect.height / (float)array2.Length)) - base.gameObject.rectTransform().rect.size / 2f);
		}
		gameObject3.GetComponent<UILineRenderer>().Points = array2;
		guides.Add(gameObject3);
	}
}
