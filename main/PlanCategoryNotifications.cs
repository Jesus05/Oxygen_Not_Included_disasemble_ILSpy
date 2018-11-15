using UnityEngine;
using UnityEngine.UI;

public class PlanCategoryNotifications : MonoBehaviour
{
	public Image AttentionImage;

	public void ToggleAttention(bool active)
	{
		if ((bool)AttentionImage)
		{
			AttentionImage.gameObject.SetActive(active);
		}
	}

	public void PingAttention()
	{
	}
}
