using UnityEngine;

public class HealthyGameMessageScreen : KMonoBehaviour
{
	public KButton confirmButton;

	public CanvasGroup canvasGroup;

	private float spawnTime;

	private float totalTime = 10f;

	private float fadeTime = 1.5f;

	private bool isFirstUpdate = true;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		confirmButton.onClick += delegate
		{
			Object.Destroy(base.gameObject);
		};
		confirmButton.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (isFirstUpdate)
		{
			isFirstUpdate = false;
			spawnTime = Time.unscaledTime;
		}
		else
		{
			float num = Mathf.Min(Time.unscaledDeltaTime, 0.0333333351f);
			float num2 = Time.unscaledTime - spawnTime;
			if (num2 < totalTime - fadeTime)
			{
				canvasGroup.alpha += num * (1f / fadeTime);
			}
			else if (num2 >= totalTime + 0.75f)
			{
				Object.Destroy(base.gameObject);
			}
			else if (num2 >= totalTime - fadeTime)
			{
				canvasGroup.alpha -= num * (1f / fadeTime);
			}
		}
	}
}
