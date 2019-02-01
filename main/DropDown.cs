using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : KMonoBehaviour
{
	public RectTransform targetDropDownContainer;

	public IListableOption selectedEntry;

	public LocText selectedLabel;

	public KButton openButton;

	public Transform contentContainer;

	public GameObject scrollRect;

	public RectTransform dropdownAlignmentTarget;

	public GameObject rowEntryPrefab;

	public bool addEmptyRow = true;

	public object targetData = null;

	private List<IListableOption> entries = new List<IListableOption>();

	private Action<IListableOption, object> onEntrySelectedAction;

	private Action<DropDownEntry, object> rowRefreshAction;

	private Dictionary<IListableOption, GameObject> rowLookup = new Dictionary<IListableOption, GameObject>();

	private Func<IListableOption, IListableOption, object, int> sortFunction;

	private GameObject emptyRow;

	private bool built = false;

	private bool displaySelectedValueWhenClosed = true;

	private const int ROWS_BEFORE_SCROLL = 8;

	private KCanvasScaler canvasScaler;

	public bool open
	{
		get;
		private set;
	}

	public void Initialize(IEnumerable<IListableOption> contentKeys, Action<IListableOption, object> onEntrySelectedAction, Func<IListableOption, IListableOption, object, int> sortFunction = null, Action<DropDownEntry, object> refreshAction = null, bool displaySelectedValueWhenClosed = true, object targetData = null)
	{
		this.targetData = targetData;
		this.sortFunction = sortFunction;
		this.onEntrySelectedAction = onEntrySelectedAction;
		this.displaySelectedValueWhenClosed = displaySelectedValueWhenClosed;
		rowRefreshAction = refreshAction;
		ChangeContent(contentKeys);
		openButton.ClearOnClick();
		openButton.onClick += delegate
		{
			OnClick();
		};
		canvasScaler = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>();
	}

	public void OnClick()
	{
		if (!open)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void ChangeContent(IEnumerable<IListableOption> contentKeys)
	{
		entries.Clear();
		foreach (IListableOption contentKey in contentKeys)
		{
			entries.Add(contentKey);
		}
		built = false;
	}

	private void Update()
	{
		if (open && (Input.GetMouseButtonDown(0) || Input.GetAxis("Mouse ScrollWheel") != 0f))
		{
			float canvasScale = canvasScaler.GetCanvasScale();
			Vector3 position = scrollRect.rectTransform().GetPosition();
			float x = position.x;
			Vector2 sizeDelta = scrollRect.rectTransform().sizeDelta;
			float num = x + sizeDelta.x * canvasScale;
			Vector3 mousePos = KInputManager.GetMousePos();
			if (!(num < mousePos.x))
			{
				Vector3 position2 = scrollRect.rectTransform().GetPosition();
				float x2 = position2.x;
				Vector3 mousePos2 = KInputManager.GetMousePos();
				if (!(x2 > mousePos2.x))
				{
					Vector3 position3 = scrollRect.rectTransform().GetPosition();
					float y = position3.y;
					Vector2 sizeDelta2 = scrollRect.rectTransform().sizeDelta;
					float num2 = y - sizeDelta2.y * canvasScale;
					Vector3 mousePos3 = KInputManager.GetMousePos();
					if (!(num2 > mousePos3.y))
					{
						Vector3 position4 = scrollRect.rectTransform().GetPosition();
						float y2 = position4.y;
						Vector3 mousePos4 = KInputManager.GetMousePos();
						if (!(y2 < mousePos4.y))
						{
							return;
						}
					}
				}
			}
			Close();
		}
	}

	private void Build(List<IListableOption> contentKeys)
	{
		built = true;
		for (int num = contentContainer.childCount - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(contentContainer.GetChild(num));
		}
		rowLookup.Clear();
		if (addEmptyRow)
		{
			emptyRow = Util.KInstantiateUI(rowEntryPrefab, contentContainer.gameObject, true);
			emptyRow.GetComponent<KButton>().onClick += delegate
			{
				onEntrySelectedAction(null, targetData);
				Close();
			};
			emptyRow.GetComponent<DropDownEntry>().label.text = UI.DROPDOWN.NONE;
		}
		for (int i = 0; i < contentKeys.Count; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(rowEntryPrefab, contentContainer.gameObject, true);
			IListableOption id = contentKeys[i];
			gameObject.GetComponent<DropDownEntry>().entryData = id;
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				onEntrySelectedAction(id, targetData);
				if (displaySelectedValueWhenClosed)
				{
					selectedLabel.text = id.GetProperName();
				}
				Close();
			};
			rowLookup.Add(id, gameObject);
		}
		RefreshEntries();
		Close();
		scrollRect.gameObject.transform.SetParent(targetDropDownContainer.transform);
		scrollRect.gameObject.SetActive(false);
	}

	private void RefreshEntries()
	{
		foreach (KeyValuePair<IListableOption, GameObject> item in rowLookup)
		{
			DropDownEntry component = item.Value.GetComponent<DropDownEntry>();
			component.label.text = item.Key.GetProperName();
			if ((UnityEngine.Object)component.portrait != (UnityEngine.Object)null && item.Key is IAssignableIdentity)
			{
				component.portrait.SetIdentityObject(item.Key as IAssignableIdentity, true);
			}
		}
		if (sortFunction != null)
		{
			entries.Sort((IListableOption a, IListableOption b) => sortFunction(a, b, targetData));
			for (int i = 0; i < entries.Count; i++)
			{
				rowLookup[entries[i]].transform.SetAsFirstSibling();
			}
			if ((UnityEngine.Object)emptyRow != (UnityEngine.Object)null)
			{
				emptyRow.transform.SetAsFirstSibling();
			}
		}
		foreach (KeyValuePair<IListableOption, GameObject> item2 in rowLookup)
		{
			DropDownEntry component2 = item2.Value.GetComponent<DropDownEntry>();
			rowRefreshAction(component2, targetData);
		}
		if ((UnityEngine.Object)emptyRow != (UnityEngine.Object)null)
		{
			rowRefreshAction(emptyRow.GetComponent<DropDownEntry>(), targetData);
		}
	}

	protected override void OnCleanUp()
	{
		Util.KDestroyGameObject(scrollRect);
		base.OnCleanUp();
	}

	public void Open()
	{
		if (!built)
		{
			Build(entries);
		}
		else
		{
			RefreshEntries();
		}
		open = true;
		scrollRect.gameObject.SetActive(true);
		scrollRect.rectTransform().localScale = Vector3.one;
		foreach (KeyValuePair<IListableOption, GameObject> item in rowLookup)
		{
			item.Value.SetActive(true);
		}
		RectTransform rectTransform = scrollRect.rectTransform();
		Vector2 sizeDelta = scrollRect.rectTransform().sizeDelta;
		rectTransform.sizeDelta = new Vector2(sizeDelta.x, 32f * (float)Mathf.Min(contentContainer.childCount, 8));
		Vector3 a = dropdownAlignmentTarget.TransformPoint(dropdownAlignmentTarget.rect.x, dropdownAlignmentTarget.rect.y, 0f);
		float x = Mathf.Min(0f, (float)Screen.width - (a.x + rowEntryPrefab.GetComponent<LayoutElement>().minWidth));
		float y = a.y;
		Vector2 sizeDelta2 = scrollRect.rectTransform().sizeDelta;
		Vector2 v = new Vector2(x, 0f - Mathf.Min(0f, y - sizeDelta2.y));
		a += (Vector3)v;
		scrollRect.rectTransform().SetPosition(a);
	}

	public void Close()
	{
		open = false;
		foreach (KeyValuePair<IListableOption, GameObject> item in rowLookup)
		{
			item.Value.SetActive(false);
		}
		scrollRect.SetActive(false);
	}
}
