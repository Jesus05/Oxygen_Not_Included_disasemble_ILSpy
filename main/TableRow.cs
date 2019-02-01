using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableRow : KMonoBehaviour
{
	public enum RowType
	{
		Header,
		Default,
		Minion
	}

	public RowType rowType;

	private MinionIdentity minion;

	private Dictionary<TableColumn, GameObject> widgets = new Dictionary<TableColumn, GameObject>();

	private Dictionary<string, GameObject> scrollers = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> scrollerBorders = new Dictionary<string, GameObject>();

	public bool isDefault = false;

	public KButton selectMinionButton;

	[SerializeField]
	private ColorStyleSetting style_setting_default;

	[SerializeField]
	private ColorStyleSetting style_setting_minion;

	[SerializeField]
	private GameObject scrollerPrefab;

	[SerializeField]
	private GameObject scrollbarPrefab;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if ((Object)selectMinionButton != (Object)null)
		{
			selectMinionButton.onClick += SelectMinion;
			selectMinionButton.onDoubleClick += SelectAndFocusMinion;
		}
	}

	public GameObject GetScroller(string scrollerID)
	{
		return scrollers[scrollerID];
	}

	public GameObject GetScrollerBorder(string scrolledID)
	{
		return scrollerBorders[scrolledID];
	}

	public void SelectMinion()
	{
		if (!((Object)minion == (Object)null))
		{
			SelectTool.Instance.Select(minion.GetComponent<KSelectable>(), false);
		}
	}

	public void SelectAndFocusMinion()
	{
		if (!((Object)minion == (Object)null))
		{
			SelectTool.Instance.SelectAndFocus(minion.transform.GetPosition(), minion.GetComponent<KSelectable>(), new Vector3(8f, 0f, 0f));
		}
	}

	public void ConfigureContent(MinionIdentity minion, Dictionary<string, TableColumn> columns)
	{
		this.minion = minion;
		KImage componentInChildren = GetComponentInChildren<KImage>(true);
		componentInChildren.colorStyleSetting = ((!((Object)minion == (Object)null)) ? style_setting_minion : style_setting_default);
		componentInChildren.ColorState = KImage.ColorSelector.Inactive;
		foreach (KeyValuePair<string, TableColumn> column in columns)
		{
			GameObject gameObject = null;
			gameObject = ((!((Object)minion == (Object)null)) ? column.Value.GetMinionWidget(base.gameObject) : ((!isDefault) ? column.Value.GetHeaderWidget(base.gameObject) : column.Value.GetDefaultWidget(base.gameObject)));
			widgets.Add(column.Value, gameObject);
			column.Value.widgets_by_row.Add(this, gameObject);
			if (column.Key.Contains("scroller_spacer_") && ((Object)minion != (Object)null || isDefault))
			{
				gameObject.GetComponentInChildren<LayoutElement>().minWidth += 3f;
			}
			if (column.Value.scrollerID != "")
			{
				foreach (string column_scroller in column.Value.screen.column_scrollers)
				{
					if (column_scroller == column.Value.scrollerID)
					{
						if (!scrollers.ContainsKey(column_scroller))
						{
							GameObject gameObject2 = Util.KInstantiateUI(scrollerPrefab, base.gameObject, true);
							KScrollRect scroll_rect = gameObject2.GetComponent<KScrollRect>();
							scroll_rect.onValueChanged.AddListener(delegate
							{
								foreach (TableRow row in column.Value.screen.rows)
								{
									KScrollRect componentInChildren2 = row.GetComponentInChildren<KScrollRect>();
									if ((Object)componentInChildren2 != (Object)null)
									{
										componentInChildren2.horizontalNormalizedPosition = scroll_rect.horizontalNormalizedPosition;
									}
								}
							});
							scrollers.Add(column_scroller, scroll_rect.content.gameObject);
							Transform x = scroll_rect.content.transform.parent.Find("Border");
							if ((Object)x != (Object)null)
							{
								scrollerBorders.Add(column_scroller, scroll_rect.content.transform.parent.Find("Border").gameObject);
							}
						}
						gameObject.transform.SetParent(scrollers[column_scroller].transform);
						scrollers[column_scroller].transform.parent.GetComponent<KScrollRect>().horizontalNormalizedPosition = 0f;
					}
				}
			}
		}
		foreach (KeyValuePair<string, TableColumn> column2 in columns)
		{
			if (column2.Value.on_load_action != null)
			{
				column2.Value.on_load_action(minion, column2.Value.widgets_by_row[this]);
			}
		}
		if ((Object)minion != (Object)null)
		{
			base.gameObject.name = minion.GetProperName();
		}
		else if (isDefault)
		{
			base.gameObject.name = "defaultRow";
		}
		if ((bool)selectMinionButton)
		{
			selectMinionButton.transform.SetAsLastSibling();
		}
		foreach (KeyValuePair<string, GameObject> scrollerBorder in scrollerBorders)
		{
			RectTransform rectTransform = scrollerBorder.Value.rectTransform();
			float width = rectTransform.rect.width;
			scrollerBorder.Value.transform.SetParent(base.gameObject.transform);
			Vector2 vector3 = rectTransform.anchorMin = (rectTransform.anchorMax = new Vector2(0f, 1f));
			RectTransform rectTransform2 = rectTransform;
			float x2 = width;
			vector3 = rectTransform.sizeDelta;
			rectTransform2.sizeDelta = new Vector2(x2, vector3.y);
			RectTransform rectTransform3 = scrollers[scrollerBorder.Key].transform.parent.rectTransform();
			Vector3 localPosition = scrollers[scrollerBorder.Key].transform.parent.rectTransform().GetLocalPosition();
			Vector2 sizeDelta = rectTransform3.sizeDelta;
			float x3 = sizeDelta.x / 2f;
			Vector2 sizeDelta2 = rectTransform3.sizeDelta;
			Vector3 vector4 = localPosition - new Vector3(x3, -1f * (sizeDelta2.y / 2f), 0f);
			vector4.y = 0f;
			RectTransform rectTransform4 = rectTransform;
			Vector2 sizeDelta3 = rectTransform.sizeDelta;
			rectTransform4.sizeDelta = new Vector2(sizeDelta3.x, 374f);
			RectTransform transform = rectTransform;
			Vector3 a = vector4;
			Vector3 up = Vector3.up;
			Vector3 localPosition2 = rectTransform.GetLocalPosition();
			Vector3 a2 = a + up * localPosition2.y;
			Vector3 up2 = Vector3.up;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			transform.SetLocalPosition(a2 + up2 * (0f - anchoredPosition.y));
		}
	}

	public void RefreshScrollers()
	{
		foreach (KeyValuePair<string, GameObject> scroller in scrollers)
		{
			KScrollRect component = scroller.Value.transform.parent.GetComponent<KScrollRect>();
			LayoutElement component2 = component.GetComponent<LayoutElement>();
			Vector2 sizeDelta = component.content.sizeDelta;
			component2.minWidth = Mathf.Min(768f, sizeDelta.x);
		}
		foreach (KeyValuePair<string, GameObject> scrollerBorder in scrollerBorders)
		{
			RectTransform rectTransform = scrollerBorder.Value.rectTransform();
			RectTransform rectTransform2 = rectTransform;
			float minWidth = scrollers[scrollerBorder.Key].transform.parent.GetComponent<LayoutElement>().minWidth;
			Vector2 sizeDelta2 = rectTransform.sizeDelta;
			rectTransform2.sizeDelta = new Vector2(minWidth, sizeDelta2.y);
		}
	}

	public GameObject GetWidget(TableColumn column)
	{
		if (widgets.ContainsKey(column) && (Object)widgets[column] != (Object)null)
		{
			return widgets[column];
		}
		Debug.LogWarning("Widget is null or row does not contain widget for column " + column, null);
		return null;
	}

	public MinionIdentity GetMinionIdentity()
	{
		return minion;
	}

	public bool ContainsWidget(GameObject widget)
	{
		return widgets.ContainsValue(widget);
	}

	public void Clear()
	{
		foreach (KeyValuePair<TableColumn, GameObject> widget in widgets)
		{
			widget.Key.widgets_by_row.Remove(this);
		}
		Object.Destroy(base.gameObject);
	}
}
