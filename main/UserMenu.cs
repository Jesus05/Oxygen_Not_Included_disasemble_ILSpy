using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMenu
{
	public class SliderInfo
	{
		public MinMaxSlider.LockingType lockType = MinMaxSlider.LockingType.Drag;

		public MinMaxSlider.Mode mode;

		public Slider.Direction direction;

		public bool interactable = true;

		public bool lockRange;

		public string toolTip;

		public string toolTipMin;

		public string toolTipMax;

		public float minLimit;

		public float maxLimit = 100f;

		public float currentMinValue = 10f;

		public float currentMaxValue = 90f;

		public GameObject sliderGO;

		public Action<MinMaxSlider> onMinChange;

		public Action<MinMaxSlider> onMaxChange;
	}

	private List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>> buttons = new List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>>();

	private List<SliderInfo> sliders = new List<SliderInfo>();

	private List<KIconButtonMenu.ButtonInfo> sortedButtons = new List<KIconButtonMenu.ButtonInfo>();

	public void Refresh(GameObject go)
	{
		Game.Instance.Trigger(1980521255, go);
	}

	public void AddButton(GameObject go, KIconButtonMenu.ButtonInfo button, float sort_order = 1f)
	{
		if (button.onClick != null)
		{
			System.Action callback = button.onClick;
			button.onClick = delegate
			{
				callback();
				Game.Instance.Trigger(1980521255, go);
			};
		}
		buttons.Add(new KeyValuePair<KIconButtonMenu.ButtonInfo, float>(button, sort_order));
	}

	public void AddSlider(GameObject go, SliderInfo slider)
	{
		sliders.Add(slider);
	}

	public void AppendToScreen(GameObject go, UserMenuScreen screen)
	{
		buttons.Clear();
		sliders.Clear();
		go.Trigger(493375141, null);
		if (buttons.Count > 0)
		{
			buttons.Sort(delegate(KeyValuePair<KIconButtonMenu.ButtonInfo, float> x, KeyValuePair<KIconButtonMenu.ButtonInfo, float> y)
			{
				if (x.Value == y.Value)
				{
					return 0;
				}
				if (x.Value > y.Value)
				{
					return 1;
				}
				return -1;
			});
			for (int i = 0; i < buttons.Count; i++)
			{
				sortedButtons.Add(buttons[i].Key);
			}
			screen.AddButtons(sortedButtons);
			sortedButtons.Clear();
		}
		if (sliders.Count > 0)
		{
			screen.AddSliders(sliders);
		}
	}
}
