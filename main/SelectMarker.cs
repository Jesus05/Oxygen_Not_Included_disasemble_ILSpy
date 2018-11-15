using UnityEngine;

public class SelectMarker : KMonoBehaviour
{
	private Transform targetTransform;

	public void SetTargetTransform(Transform target_transform)
	{
		targetTransform = target_transform;
		LateUpdate();
	}

	private void LateUpdate()
	{
		if ((Object)targetTransform == (Object)null)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			Vector3 position = targetTransform.GetPosition();
			KCollider2D component = targetTransform.GetComponent<KCollider2D>();
			if ((Object)component != (Object)null)
			{
				Vector3 center = component.bounds.center;
				position.x = center.x;
				Vector3 center2 = component.bounds.center;
				float y = center2.y;
				Vector3 size = component.bounds.size;
				position.y = y + size.y / 2f + 0.1f;
			}
			else
			{
				position.y += 2f;
			}
			Vector3 b = new Vector3(0f, (Mathf.Sin(Time.unscaledTime * 4f) + 1f) * 0.1f, 0f);
			base.transform.SetPosition(position + b);
		}
	}
}
