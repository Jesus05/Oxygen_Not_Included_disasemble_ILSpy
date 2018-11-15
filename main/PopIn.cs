using UnityEngine;

public class PopIn : MonoBehaviour
{
	private float targetScale;

	public float speed;

	private void OnEnable()
	{
		StartPopIn(true);
	}

	private void Update()
	{
		Vector3 localScale = base.transform.localScale;
		float x = localScale.x;
		float num = Mathf.Lerp(x, targetScale, Time.unscaledDeltaTime * speed);
		base.transform.localScale = new Vector3(num, num, 1f);
	}

	public void StartPopIn(bool force_reset = false)
	{
		if (force_reset)
		{
			base.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
		}
		targetScale = 1f;
	}

	public void StartPopOut()
	{
		targetScale = 0f;
	}
}
