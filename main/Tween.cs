using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private static float Scale = 1.025f;

	private static float ScaleSpeed = 0.5f;

	private Selectable Selectable;

	private float Direction = -1f;

	private void Awake()
	{
		Selectable = GetComponent<Selectable>();
	}

	public void OnPointerEnter(PointerEventData data)
	{
		Direction = 1f;
	}

	public void OnPointerExit(PointerEventData data)
	{
		Direction = -1f;
	}

	private void Update()
	{
		if (Selectable.interactable)
		{
			Vector3 localScale = base.transform.localScale;
			float x = localScale.x;
			float a = x + Direction * Time.unscaledDeltaTime * ScaleSpeed;
			a = Mathf.Min(a, Scale);
			a = Mathf.Max(a, 1f);
			if (a != x)
			{
				base.transform.localScale = new Vector3(a, a, 1f);
			}
		}
	}
}
