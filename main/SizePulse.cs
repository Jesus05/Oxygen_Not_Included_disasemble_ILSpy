using System;
using UnityEngine;

public class SizePulse : MonoBehaviour
{
	private enum State
	{
		Up,
		Down,
		Finished
	}

	public System.Action onComplete;

	public Vector2 from = Vector2.one;

	public Vector2 to = Vector2.one;

	public float multiplier = 1.25f;

	public float speed = 1f;

	public bool updateWhenPaused;

	private Vector2 cur;

	private State state;

	private void Start()
	{
		if (GetComponents<SizePulse>().Length > 1)
		{
			UnityEngine.Object.Destroy(this);
		}
		RectTransform rectTransform = (RectTransform)base.transform;
		from = rectTransform.localScale;
		cur = from;
		to = from * multiplier;
	}

	private void Update()
	{
		float num = (!updateWhenPaused) ? Time.deltaTime : Time.unscaledDeltaTime;
		num *= speed;
		switch (state)
		{
		case State.Up:
			cur = Vector2.Lerp(cur, to, num);
			if ((to - cur).sqrMagnitude < 0.0001f)
			{
				cur = to;
				state = State.Down;
			}
			break;
		case State.Down:
			cur = Vector2.Lerp(cur, from, num);
			if ((from - cur).sqrMagnitude < 0.0001f)
			{
				cur = from;
				state = State.Finished;
				if (onComplete != null)
				{
					onComplete();
				}
			}
			break;
		}
		RectTransform rectTransform = (RectTransform)base.transform;
		rectTransform.localScale = new Vector3(cur.x, cur.y, 1f);
	}
}
