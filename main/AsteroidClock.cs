using UnityEngine;
using UnityEngine.UI;

public class AsteroidClock : MonoBehaviour
{
	public Transform rotationTransform;

	public Image NightOverlay;

	private void Awake()
	{
		UpdateOverlay();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if ((Object)GameClock.Instance != (Object)null)
		{
			rotationTransform.rotation = Quaternion.Euler(0f, 0f, 360f * (0f - GameClock.Instance.GetCurrentCycleAsPercentage()));
		}
	}

	private void UpdateOverlay()
	{
		float fillAmount = 0.125f;
		NightOverlay.fillAmount = fillAmount;
	}
}
