using System;
using UnityEngine;
using UnityEngine.UI;

public class MinMaxSlider : KMonoBehaviour
{
	public enum LockingType
	{
		Toggle,
		Drag
	}

	public enum Mode
	{
		Single,
		Double,
		Triple
	}

	public LockingType lockType = LockingType.Drag;

	public bool lockRange = false;

	public bool interactable = true;

	public float minLimit = 0f;

	public float maxLimit = 100f;

	public float range = 50f;

	public float barWidth = 10f;

	public float barHeight = 100f;

	public float currentMinValue = 10f;

	public float currentMaxValue = 90f;

	public float currentExtraValue = 50f;

	public Slider.Direction direction = Slider.Direction.LeftToRight;

	public bool wholeNumbers = true;

	public Action<MinMaxSlider> onMinChange;

	public Action<MinMaxSlider> onMaxChange;

	public Slider minSlider;

	public Slider maxSlider;

	public Slider extraSlider;

	public RectTransform minRect;

	public RectTransform maxRect;

	public RectTransform bgFill;

	public RectTransform mgFill;

	public RectTransform fgFill;

	public Text title;

	[MyCmpGet]
	public ToolTip toolTip;

	public Image icon;

	public Image isOverPowered;

	private Vector3 mousePos;

	public Mode mode
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ToolTip component = base.transform.parent.gameObject.GetComponent<ToolTip>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			UnityEngine.Object.DestroyImmediate(toolTip);
			toolTip = component;
		}
		minSlider.value = currentMinValue;
		maxSlider.value = currentMaxValue;
		minSlider.interactable = interactable;
		maxSlider.interactable = interactable;
		minSlider.maxValue = maxLimit;
		maxSlider.maxValue = maxLimit;
		minSlider.minValue = minLimit;
		maxSlider.minValue = minLimit;
		Slider slider = minSlider;
		Slider.Direction direction = this.direction;
		maxSlider.direction = direction;
		slider.direction = direction;
		if ((UnityEngine.Object)isOverPowered != (UnityEngine.Object)null)
		{
			isOverPowered.enabled = false;
		}
		minSlider.gameObject.SetActive(false);
		if (mode != 0)
		{
			minSlider.gameObject.SetActive(true);
		}
		if ((UnityEngine.Object)extraSlider != (UnityEngine.Object)null)
		{
			extraSlider.value = currentExtraValue;
			Slider slider2 = extraSlider;
			bool flag = wholeNumbers;
			maxSlider.wholeNumbers = flag;
			flag = flag;
			minSlider.wholeNumbers = flag;
			slider2.wholeNumbers = flag;
			extraSlider.direction = this.direction;
			extraSlider.interactable = interactable;
			extraSlider.maxValue = maxLimit;
			extraSlider.minValue = minLimit;
			extraSlider.gameObject.SetActive(false);
			if (mode == Mode.Triple)
			{
				extraSlider.gameObject.SetActive(true);
			}
		}
	}

	public void SetIcon(Image newIcon)
	{
		icon = newIcon;
		icon.gameObject.transform.SetParent(base.transform);
		icon.gameObject.transform.SetAsFirstSibling();
		icon.rectTransform().anchoredPosition = Vector2.zero;
	}

	public void SetMode(Mode mode)
	{
		this.mode = mode;
		if (mode == Mode.Single && (UnityEngine.Object)extraSlider != (UnityEngine.Object)null)
		{
			extraSlider.gameObject.SetActive(false);
			extraSlider.handleRect.gameObject.SetActive(false);
		}
	}

	private void SetAnchor(RectTransform trans, Vector2 min, Vector2 max)
	{
		trans.anchorMin = min;
		trans.anchorMax = max;
	}

	public void SetMinMaxValue(float currentMin, float currentMax, float min, float max)
	{
		minSlider.value = currentMin;
		currentMinValue = currentMin;
		maxSlider.value = currentMax;
		currentMaxValue = currentMax;
		minLimit = min;
		maxLimit = max;
		minSlider.minValue = minLimit;
		maxSlider.minValue = minLimit;
		minSlider.maxValue = maxLimit;
		maxSlider.maxValue = maxLimit;
		if ((UnityEngine.Object)extraSlider != (UnityEngine.Object)null)
		{
			extraSlider.minValue = minLimit;
			extraSlider.maxValue = maxLimit;
		}
	}

	public void SetExtraValue(float current)
	{
		extraSlider.value = current;
		toolTip.toolTip = base.transform.parent.name + ": " + current.ToString("F2");
	}

	public void SetMaxValue(float current, float max)
	{
		float num = current / max * 100f;
		if ((UnityEngine.Object)isOverPowered != (UnityEngine.Object)null)
		{
			isOverPowered.enabled = (num > 100f);
		}
		maxSlider.value = Mathf.Min(100f, num);
		if ((UnityEngine.Object)toolTip != (UnityEngine.Object)null)
		{
			toolTip.toolTip = base.transform.parent.name + ": " + current.ToString("F2") + "/" + max.ToString("F2");
		}
	}

	private void Update()
	{
		if (interactable)
		{
			minSlider.value = Mathf.Clamp(currentMinValue, minLimit, currentMinValue);
			maxSlider.value = Mathf.Max(minSlider.value, Mathf.Clamp(currentMaxValue, Mathf.Max(minSlider.value, minLimit), maxLimit));
			if (direction == Slider.Direction.LeftToRight || direction == Slider.Direction.RightToLeft)
			{
				RectTransform rectTransform = minRect;
				float x = minSlider.value / maxLimit;
				Vector2 anchorMax = minRect.anchorMax;
				rectTransform.anchorMax = new Vector2(x, anchorMax.y);
				RectTransform rectTransform2 = maxRect;
				float x2 = maxSlider.value / maxLimit;
				Vector2 anchorMax2 = maxRect.anchorMax;
				rectTransform2.anchorMax = new Vector2(x2, anchorMax2.y);
				RectTransform rectTransform3 = maxRect;
				float x3 = minSlider.value / maxLimit;
				Vector2 anchorMin = maxRect.anchorMin;
				rectTransform3.anchorMin = new Vector2(x3, anchorMin.y);
			}
			else
			{
				RectTransform rectTransform4 = minRect;
				Vector2 anchorMin2 = minRect.anchorMin;
				rectTransform4.anchorMax = new Vector2(anchorMin2.x, minSlider.value / maxLimit);
				RectTransform rectTransform5 = maxRect;
				Vector2 anchorMin3 = maxRect.anchorMin;
				rectTransform5.anchorMin = new Vector2(anchorMin3.x, minSlider.value / maxLimit);
			}
		}
	}

	public void OnMinValueChanged(float ignoreThis)
	{
		if (interactable)
		{
			if (lockRange)
			{
				currentMaxValue = Mathf.Min(Mathf.Max(minLimit, minSlider.value) + range, maxLimit);
				currentMinValue = Mathf.Max(minLimit, Mathf.Min(maxSlider.value, currentMaxValue - range));
			}
			else
			{
				currentMinValue = Mathf.Clamp(minSlider.value, minLimit, Mathf.Min(maxSlider.value, currentMaxValue));
			}
			if (onMinChange != null)
			{
				onMinChange(this);
			}
		}
	}

	public void OnMaxValueChanged(float ignoreThis)
	{
		if (interactable)
		{
			if (lockRange)
			{
				currentMinValue = Mathf.Max(maxSlider.value - range, minLimit);
				currentMaxValue = Mathf.Max(minSlider.value, Mathf.Clamp(maxSlider.value, Mathf.Max(currentMinValue + range, minLimit), maxLimit));
			}
			else
			{
				currentMaxValue = Mathf.Max(minSlider.value, Mathf.Clamp(maxSlider.value, Mathf.Max(minSlider.value, minLimit), maxLimit));
			}
			if (onMaxChange != null)
			{
				onMaxChange(this);
			}
		}
	}

	public void Lock(bool shouldLock)
	{
		if (interactable && lockType == LockingType.Drag)
		{
			lockRange = shouldLock;
			range = maxSlider.value - minSlider.value;
			mousePos = KInputManager.GetMousePos();
		}
	}

	public void ToggleLock()
	{
		if (interactable && lockType == LockingType.Toggle)
		{
			lockRange = !lockRange;
			if (lockRange)
			{
				range = maxSlider.value - minSlider.value;
			}
		}
	}

	public void OnDrag()
	{
		if (interactable && lockRange && lockType == LockingType.Drag)
		{
			Vector3 vector = KInputManager.GetMousePos();
			float num = vector.x - mousePos.x;
			if (direction == Slider.Direction.TopToBottom || direction == Slider.Direction.BottomToTop)
			{
				Vector3 vector2 = KInputManager.GetMousePos();
				num = vector2.y - mousePos.y;
			}
			currentMinValue = Mathf.Max(currentMinValue + num, minLimit);
			mousePos = KInputManager.GetMousePos();
		}
	}
}
