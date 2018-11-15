using System;
using UnityEngine;

public struct GravityComponent
{
	public Transform transform;

	public Vector2 velocity;

	public float radius;

	public float elapsedTime;

	public System.Action onLanded;

	public GravityComponent(Transform transform, System.Action on_landed)
	{
		this = new GravityComponent(transform, on_landed, Vector2.zero);
	}

	public GravityComponent(Transform transform, System.Action on_landed, Vector2 initial_velocity)
	{
		this.transform = transform;
		elapsedTime = 0f;
		velocity = initial_velocity;
		onLanded = on_landed;
		radius = GetRadius(transform);
	}

	public static float GetRadius(Transform transform)
	{
		KCircleCollider2D component = transform.GetComponent<KCircleCollider2D>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.radius;
		}
		KCollider2D component2 = transform.GetComponent<KCollider2D>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			Vector3 position = transform.GetPosition();
			float y = position.y;
			Vector3 min = component2.bounds.min;
			return y - min.y;
		}
		return 0f;
	}
}
