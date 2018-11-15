using System;
using System.Collections.Generic;
using UnityEngine;

public class UserMenuScreen : KIconButtonMenu
{
	private GameObject selected;

	public MinMaxSlider sliderPrefab;

	public GameObject sliderParent;

	public PriorityScreen priorityScreenPrefab;

	public GameObject priorityScreenParent;

	private List<MinMaxSlider> sliders = new List<MinMaxSlider>();

	private List<UserMenu.SliderInfo> slidersInfos = new List<UserMenu.SliderInfo>();

	private List<ButtonInfo> buttonInfos = new List<ButtonInfo>();

	private PriorityScreen priorityScreen;

	protected override void OnPrefabInit()
	{
		keepMenuOpen = true;
		base.OnPrefabInit();
		priorityScreen = Util.KInstantiateUI<PriorityScreen>(priorityScreenPrefab.gameObject, priorityScreenParent, false);
		priorityScreen.InstantiateButtons(OnPriorityClicked, true);
		buttonParent.transform.SetAsLastSibling();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(1980521255, OnUIRefresh);
	}

	public void SetSelected(GameObject go)
	{
		ClearPrioritizable();
		selected = go;
		RefreshPrioritizable();
	}

	private void ClearPrioritizable()
	{
		if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
		{
			Prioritizable component = selected.GetComponent<Prioritizable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				Prioritizable prioritizable = component;
				prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Remove(prioritizable.onPriorityChanged, new Action<PrioritySetting>(OnPriorityChanged));
			}
		}
	}

	private void RefreshPrioritizable()
	{
		if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
		{
			Prioritizable component = selected.GetComponent<Prioritizable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.IsPrioritizable())
			{
				Prioritizable prioritizable = component;
				prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(OnPriorityChanged));
				priorityScreen.gameObject.SetActive(true);
				priorityScreen.SetScreenPriority(component.GetMasterPriority(), false);
			}
			else
			{
				priorityScreen.gameObject.SetActive(false);
			}
		}
	}

	public void Refresh(GameObject go)
	{
		if (!((UnityEngine.Object)go != (UnityEngine.Object)selected))
		{
			buttonInfos.Clear();
			slidersInfos.Clear();
			Game.Instance.userMenu.AppendToScreen(go, this);
			SetButtons(buttonInfos);
			base.RefreshButtons();
			RefreshSliders();
			ClearPrioritizable();
			RefreshPrioritizable();
			if ((sliders == null || sliders.Count == 0) && (buttonInfos == null || buttonInfos.Count == 0) && !priorityScreen.gameObject.activeSelf)
			{
				base.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				base.transform.parent.gameObject.SetActive(true);
			}
		}
	}

	public void AddSliders(IList<UserMenu.SliderInfo> sliders)
	{
		slidersInfos.AddRange(sliders);
	}

	public void AddButtons(IList<ButtonInfo> buttons)
	{
		buttonInfos.AddRange(buttons);
	}

	private void OnUIRefresh(object data)
	{
		Refresh(data as GameObject);
	}

	public void RefreshSliders()
	{
		if (sliders != null)
		{
			for (int i = 0; i < sliders.Count; i++)
			{
				UnityEngine.Object.Destroy(sliders[i].gameObject);
			}
			sliders = null;
		}
		if (slidersInfos != null && slidersInfos.Count != 0)
		{
			sliders = new List<MinMaxSlider>();
			for (int j = 0; j < slidersInfos.Count; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(sliderPrefab.gameObject, Vector3.zero, Quaternion.identity);
				slidersInfos[j].sliderGO = gameObject;
				MinMaxSlider component = gameObject.GetComponent<MinMaxSlider>();
				sliders.Add(component);
				Transform parent = (!((UnityEngine.Object)sliderParent != (UnityEngine.Object)null)) ? base.transform : sliderParent.transform;
				gameObject.transform.SetParent(parent, false);
				gameObject.SetActive(true);
				gameObject.name = "Slider";
				if ((bool)component.toolTip)
				{
					component.toolTip.toolTip = slidersInfos[j].toolTip;
				}
				component.lockType = slidersInfos[j].lockType;
				component.interactable = slidersInfos[j].interactable;
				component.minLimit = slidersInfos[j].minLimit;
				component.maxLimit = slidersInfos[j].maxLimit;
				component.currentMinValue = slidersInfos[j].currentMinValue;
				component.currentMaxValue = slidersInfos[j].currentMaxValue;
				component.onMinChange = slidersInfos[j].onMinChange;
				component.onMaxChange = slidersInfos[j].onMaxChange;
				component.direction = slidersInfos[j].direction;
				component.SetMode(slidersInfos[j].mode);
				component.SetMinMaxValue(slidersInfos[j].currentMinValue, slidersInfos[j].currentMaxValue, slidersInfos[j].minLimit, slidersInfos[j].maxLimit);
			}
		}
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
		{
			Prioritizable component = selected.GetComponent<Prioritizable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.SetMasterPriority(priority);
			}
		}
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		priorityScreen.SetScreenPriority(priority, false);
	}
}
