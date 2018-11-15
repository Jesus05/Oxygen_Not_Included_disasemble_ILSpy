using System;
using UnityEngine;

public struct FallerComponent
{
	public Transform transform;

	public int transformInstanceId;

	public bool isFalling;

	public float offset;

	public Vector2 initialVelocity;

	public HandleVector<int>.Handle partitionerEntry;

	public Action<object> solidChangedCB;

	public System.Action cellChangedCB;

	public FallerComponent(Transform transform, Vector2 initial_velocity)
	{
		this.transform = transform;
		transformInstanceId = transform.GetInstanceID();
		isFalling = false;
		initialVelocity = initial_velocity;
		partitionerEntry = default(HandleVector<int>.Handle);
		solidChangedCB = null;
		cellChangedCB = null;
		KCircleCollider2D component = transform.GetComponent<KCircleCollider2D>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			offset = component.radius;
		}
		else
		{
			KCollider2D component2 = transform.GetComponent<KCollider2D>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				Vector3 position = transform.GetPosition();
				float y = position.y;
				Vector3 min = component2.bounds.min;
				offset = y - min.y;
			}
			else
			{
				offset = 0f;
			}
		}
	}
}
