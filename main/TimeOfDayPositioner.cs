using UnityEngine;

public class TimeOfDayPositioner : KMonoBehaviour
{
	[SerializeField]
	private RectTransform targetRect;

	private void Update()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		float f = currentCycleAsPercentage * targetRect.rect.width;
		RectTransform rectTransform = base.transform as RectTransform;
		rectTransform.anchoredPosition = targetRect.anchoredPosition + new Vector2(Mathf.Round(f), 0f);
	}
}
