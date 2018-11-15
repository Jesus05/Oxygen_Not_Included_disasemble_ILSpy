using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KTreeItem : MonoBehaviour
{
	public delegate void StateChanged(KTreeItem item, bool value);

	[SerializeField]
	private bool childrenVisible;

	[SerializeField]
	private bool checkVisible;

	[SerializeField]
	private bool isChecked;

	[SerializeField]
	private Sprite spriteOpen;

	[SerializeField]
	private Sprite spriteClosed;

	[SerializeField]
	private Image openedImage;

	[SerializeField]
	private Text label;

	[SerializeField]
	private Toggle checkbox;

	[SerializeField]
	private GameObject childrenRoot;

	[NonSerialized]
	public object userData;

	private List<KTreeItem> childItems = new List<KTreeItem>();

	[NonSerialized]
	public KTreeItem parent;

	public string text
	{
		get
		{
			return label.text;
		}
		set
		{
			base.name = value;
			label.text = value;
		}
	}

	public bool checkboxEnabled
	{
		get
		{
			return checkbox.gameObject.activeSelf;
		}
		set
		{
			checkbox.gameObject.SetActive(value);
		}
	}

	public bool checkboxChecked
	{
		get
		{
			return checkbox.isOn;
		}
		set
		{
			checkbox.isOn = value;
		}
	}

	public bool opened
	{
		get
		{
			return childrenVisible;
		}
		set
		{
			childrenVisible = value;
			UpdateOpened();
		}
	}

	public IList<KTreeItem> children => childItems;

	public event StateChanged onOpenChanged;

	public event StateChanged onCheckChanged;

	private void Awake()
	{
		UpdateOpened();
		SetImageAlpha(0f);
	}

	private void SetImageAlpha(float a)
	{
		Color color = openedImage.color;
		color.a = a;
		openedImage.color = color;
	}

	public void AddChild(KTreeItem child)
	{
		childItems.Add(child);
		child.transform.SetParent(childrenRoot.transform, false);
		child.parent = this;
		SetImageAlpha(1f);
	}

	public void RemoveChild(KTreeItem child)
	{
		childItems.Remove(child);
		if (childItems.Count == 0)
		{
			SetImageAlpha(0f);
		}
	}

	public void ToggleOpened()
	{
		opened = !opened;
		UpdateOpened();
		if (this.onOpenChanged != null)
		{
			this.onOpenChanged(this, opened);
		}
	}

	public void ToggleChecked()
	{
		if (this.onCheckChanged != null)
		{
			this.onCheckChanged(this, checkboxChecked);
		}
	}

	private void UpdateOpened()
	{
		openedImage.sprite = ((!opened) ? spriteClosed : spriteOpen);
		childrenRoot.SetActive(opened);
	}
}
