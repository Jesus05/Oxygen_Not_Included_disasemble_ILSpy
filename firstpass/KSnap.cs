using UnityEngine;

[ExecuteInEditMode]
public class KSnap : MonoBehaviour
{
	public enum LeftRight
	{
		None,
		Left,
		Middle,
		Right
	}

	public enum TopBottom
	{
		None,
		Top,
		Middle,
		Bottom
	}

	public GameObject target;

	public LeftRight horizontal;

	public TopBottom vertical;

	public Vector2 offset;

	[SerializeField]
	private bool keepOnScreen;

	private Vector3[] corners = new Vector3[4];

	public void SetTarget(GameObject newtarget)
	{
		target = newtarget;
		Update();
	}

	private void Update()
	{
		if ((Object)target != (Object)null)
		{
			RectTransform rectTransform = target.rectTransform();
			if ((Object)rectTransform != (Object)null)
			{
				rectTransform.GetWorldCorners(corners);
				Vector3 vector = corners[2];
				Vector3 vector2 = corners[0];
				Vector3 position = base.transform.GetPosition();
				if (horizontal == LeftRight.Left)
				{
					position.x = vector2.x + offset.x;
				}
				else if (horizontal == LeftRight.Right)
				{
					position.x = vector.x + offset.x;
				}
				else if (horizontal == LeftRight.Middle)
				{
					position.x = vector.x + (vector2.x - vector.x) / 2f + offset.x;
				}
				if (vertical == TopBottom.Top)
				{
					position.y = vector.y + offset.y;
				}
				else if (vertical == TopBottom.Bottom)
				{
					position.y = vector2.y + offset.y;
				}
				else if (vertical == TopBottom.Middle)
				{
					position.y = vector2.y + (vector.y - vector2.y) / 2f + offset.y;
				}
				base.transform.SetPosition(position);
				KeepOnScreen();
			}
		}
	}

	private void KeepOnScreen()
	{
		if (keepOnScreen)
		{
			RectTransform rectTransform = base.transform.rectTransform();
			rectTransform.GetWorldCorners(corners);
			Vector3 zero = Vector3.zero;
			Vector3[] array = corners;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = array[i];
				if (vector.x < 0f)
				{
					zero.x = Mathf.Max(zero.x, 0f - vector.x);
				}
				if (vector.x > (float)Screen.width)
				{
					zero.x = Mathf.Min(zero.x, (float)Screen.width - vector.x);
				}
				if (vector.y < 0f)
				{
					zero.y = Mathf.Max(zero.y, 0f - vector.y);
				}
				if (vector.y > (float)Screen.height)
				{
					zero.y = Mathf.Min(zero.y, (float)Screen.height - vector.y);
				}
			}
			base.transform.SetPosition(base.transform.GetPosition() + zero);
		}
	}
}
