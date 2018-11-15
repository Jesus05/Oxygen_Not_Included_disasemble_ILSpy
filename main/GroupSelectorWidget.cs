using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectorWidget : MonoBehaviour
{
	[Serializable]
	public struct ItemData
	{
		public Sprite sprite;

		public object userData;

		public ItemData(Sprite sprite, object user_data)
		{
			this.sprite = sprite;
			userData = user_data;
		}
	}

	public struct ItemCallbacks
	{
		public Func<object, IList<int>> getSubPanelDisplayIndices;

		public Action<object, object> onItemAdded;

		public Action<object, object> onItemRemoved;

		public Func<object, object, bool, string> getItemHoverText;
	}

	[SerializeField]
	private GameObject itemTemplate;

	[SerializeField]
	private RectTransform selectedItemsPanel;

	[SerializeField]
	private RectTransform unselectedItemsPanel;

	[SerializeField]
	private KButton addItemButton;

	[SerializeField]
	private int numExpectedPanelColumns = 3;

	private object widgetID;

	private ItemCallbacks itemCallbacks;

	private IList<ItemData> options;

	private List<int> selectedOptionIndices = new List<int>();

	private List<GameObject> selectedVisualizers = new List<GameObject>();

	public void Initialize(object widget_id, IList<ItemData> options, ItemCallbacks item_callbacks)
	{
		widgetID = widget_id;
		this.options = options;
		itemCallbacks = item_callbacks;
		addItemButton.onClick += OnAddItemClicked;
	}

	public void Reconfigure(IList<int> selected_option_indices)
	{
		selectedOptionIndices.Clear();
		selectedOptionIndices.AddRange(selected_option_indices);
		selectedOptionIndices.Sort();
		addItemButton.isInteractable = (selectedOptionIndices.Count < options.Count);
		RebuildSelectedVisualizers();
	}

	private void OnAddItemClicked()
	{
		if (!IsSubPanelOpen())
		{
			int num = RebuildSubPanelOptions();
			if (num > 0)
			{
				GridLayoutGroup component = unselectedItemsPanel.GetComponent<GridLayoutGroup>();
				component.constraintCount = Mathf.Min(numExpectedPanelColumns, unselectedItemsPanel.childCount);
				unselectedItemsPanel.gameObject.SetActive(true);
				unselectedItemsPanel.GetComponent<Selectable>().Select();
			}
		}
		else
		{
			CloseSubPanel();
		}
	}

	private void OnItemAdded(int option_idx)
	{
		if (itemCallbacks.onItemAdded != null)
		{
			Action<object, object> onItemAdded = itemCallbacks.onItemAdded;
			object arg = widgetID;
			ItemData itemData = options[option_idx];
			onItemAdded(arg, itemData.userData);
			RebuildSubPanelOptions();
		}
	}

	private void OnItemRemoved(int option_idx)
	{
		if (itemCallbacks.onItemRemoved != null)
		{
			Action<object, object> onItemRemoved = itemCallbacks.onItemRemoved;
			object arg = widgetID;
			ItemData itemData = options[option_idx];
			onItemRemoved(arg, itemData.userData);
		}
	}

	private void RebuildSelectedVisualizers()
	{
		foreach (GameObject selectedVisualizer in selectedVisualizers)
		{
			Util.KDestroyGameObject(selectedVisualizer);
		}
		selectedVisualizers.Clear();
		foreach (int selectedOptionIndex in selectedOptionIndices)
		{
			GameObject item = CreateItem(selectedOptionIndex, OnItemRemoved, selectedItemsPanel.gameObject, true);
			selectedVisualizers.Add(item);
		}
	}

	private GameObject CreateItem(int idx, Action<int> on_click, GameObject parent, bool is_selected_item)
	{
		GameObject gameObject = Util.KInstantiateUI(itemTemplate, parent, true);
		KButton component = gameObject.GetComponent<KButton>();
		component.onClick += delegate
		{
			on_click(idx);
		};
		Image fgImage = component.fgImage;
		ItemData itemData = options[idx];
		fgImage.sprite = itemData.sprite;
		if ((UnityEngine.Object)parent == (UnityEngine.Object)selectedItemsPanel.gameObject)
		{
			HierarchyReferences component2 = component.GetComponent<HierarchyReferences>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				Component reference = component2.GetReference("CancelImg");
				if ((UnityEngine.Object)reference != (UnityEngine.Object)null)
				{
					reference.gameObject.SetActive(true);
				}
			}
		}
		ToolTip component3 = gameObject.GetComponent<ToolTip>();
		component3.OnToolTip = delegate
		{
			Func<object, object, bool, string> getItemHoverText = itemCallbacks.getItemHoverText;
			object arg = widgetID;
			ItemData itemData2 = options[idx];
			return getItemHoverText(arg, itemData2.userData, is_selected_item);
		};
		return gameObject;
	}

	public bool IsSubPanelOpen()
	{
		return unselectedItemsPanel.gameObject.activeSelf;
	}

	public void CloseSubPanel()
	{
		ClearSubPanelOptions();
		unselectedItemsPanel.gameObject.SetActive(false);
	}

	private void ClearSubPanelOptions()
	{
		IEnumerator enumerator = unselectedItemsPanel.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				Util.KDestroyGameObject(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private int RebuildSubPanelOptions()
	{
		IList<int> list = itemCallbacks.getSubPanelDisplayIndices(widgetID);
		if (list.Count > 0)
		{
			ClearSubPanelOptions();
			foreach (int item in list)
			{
				if (!selectedOptionIndices.Contains(item))
				{
					CreateItem(item, OnItemAdded, unselectedItemsPanel.gameObject, false);
				}
			}
		}
		else
		{
			CloseSubPanel();
		}
		return list.Count;
	}
}
