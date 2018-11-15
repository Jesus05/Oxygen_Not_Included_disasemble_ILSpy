using UnityEngine;

[ExecuteInEditMode]
public class KAlign : MonoBehaviour
{
	public enum TargetLeftRight
	{
		None,
		Left,
		Middle,
		Right
	}

	public enum TargetTopBottom
	{
		None,
		Top,
		Middle,
		Bottom
	}

	public enum SourceLeftRight
	{
		Left,
		Middle,
		Right
	}

	public enum SourceTopBottom
	{
		Top,
		Middle,
		Bottom
	}

	public GameObject target;

	public SourceLeftRight sourceHorizontal;

	public SourceTopBottom sourceVertical;

	public TargetLeftRight targetHorizontal;

	public TargetTopBottom targetVertical;

	public Vector2 offset;

	public void SetTarget(GameObject newtarget)
	{
		target = newtarget;
		Update();
	}

	private void OnEnable()
	{
		Update();
	}

	private void Update()
	{
		if ((Object)target != (Object)null)
		{
			RectTransform rectTransform = target.rectTransform();
			if ((Object)rectTransform != (Object)null)
			{
				Vector3[] array = new Vector3[4];
				rectTransform.GetWorldCorners(array);
				Vector3 vector = array[1];
				Vector3 vector2 = array[3];
				Vector3 position = base.transform.GetPosition();
				Vector3[] array2 = new Vector3[4];
				this.rectTransform().GetWorldCorners(array2);
				Vector3 vector3 = array2[1];
				Vector3 vector4 = array2[3];
				float num = position.x;
				float num2 = position.y;
				if (targetHorizontal != 0)
				{
					num = offset.x;
					if (sourceHorizontal == SourceLeftRight.Left)
					{
						num += position.x - vector3.x;
					}
					else if (sourceHorizontal == SourceLeftRight.Right)
					{
						num += position.x - vector4.x;
					}
					else if (sourceHorizontal == SourceLeftRight.Middle)
					{
						num += vector3.x - position.x + (vector4.x - vector3.x) / 2f;
					}
					if (targetHorizontal == TargetLeftRight.Right)
					{
						num += vector2.x;
					}
					else if (targetHorizontal == TargetLeftRight.Left)
					{
						num += vector.x;
					}
					else if (targetHorizontal == TargetLeftRight.Middle)
					{
						num += vector.x + (vector2.x - vector.x) / 2f;
					}
				}
				if (targetVertical != 0)
				{
					num2 = offset.y;
					if (sourceVertical == SourceTopBottom.Top)
					{
						num2 += position.y - vector3.y;
					}
					else if (sourceVertical == SourceTopBottom.Bottom)
					{
						num2 += position.y - vector4.y;
					}
					else if (sourceVertical == SourceTopBottom.Middle)
					{
						num2 += position.y - vector3.y + (vector3.y - vector4.y) / 2f;
					}
					if (targetVertical == TargetTopBottom.Top)
					{
						num2 += vector.y;
					}
					else if (targetVertical == TargetTopBottom.Bottom)
					{
						num2 += vector2.y;
					}
					else if (targetVertical == TargetTopBottom.Middle)
					{
						num2 += vector2.y + (vector.y - vector2.y) / 2f;
					}
				}
				position.x = num;
				position.y = num2;
				base.transform.SetPosition(position);
			}
		}
	}
}
