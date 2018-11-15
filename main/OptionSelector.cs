using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelector : MonoBehaviour
{
	public class DisplayOptionInfo
	{
		public IList<Sprite> bgOptions;

		public IList<Sprite> fgOptions;

		public int bgIndex;

		public int fgIndex;

		public Color32 fillColour;
	}

	private object id;

	public Action<object, int> OnChangePriority;

	[SerializeField]
	private KImage selectedItem;

	[SerializeField]
	private KImage itemTemplate;

	private void Start()
	{
		KButton component = selectedItem.GetComponent<KButton>();
		component.onBtnClick += OnClick;
	}

	public void Initialize(object id)
	{
		this.id = id;
	}

	private void OnClick(KKeyCode button)
	{
		switch (button)
		{
		case KKeyCode.Mouse0:
			OnChangePriority(id, 1);
			break;
		case KKeyCode.Mouse1:
			OnChangePriority(id, -1);
			break;
		}
	}

	public void ConfigureItem(bool disabled, DisplayOptionInfo display_info)
	{
		KImage kImage = selectedItem;
		HierarchyReferences component = kImage.GetComponent<HierarchyReferences>();
		KImage kImage2 = component.GetReference("BG") as KImage;
		if (display_info.bgOptions == null)
		{
			kImage2.gameObject.SetActive(false);
		}
		else
		{
			kImage2.sprite = display_info.bgOptions[display_info.bgIndex];
		}
		KImage kImage3 = component.GetReference("FG") as KImage;
		if (display_info.fgOptions == null)
		{
			kImage3.gameObject.SetActive(false);
		}
		else
		{
			kImage3.sprite = display_info.fgOptions[display_info.fgIndex];
		}
		KImage kImage4 = component.GetReference("Fill") as KImage;
		if ((UnityEngine.Object)kImage4 != (UnityEngine.Object)null)
		{
			kImage4.enabled = !disabled;
			kImage4.color = display_info.fillColour;
		}
		KImage kImage5 = component.GetReference("Outline") as KImage;
		if ((UnityEngine.Object)kImage5 != (UnityEngine.Object)null)
		{
			kImage5.enabled = !disabled;
		}
	}
}
