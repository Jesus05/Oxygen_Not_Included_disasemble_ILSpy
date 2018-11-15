using UnityEngine;

public class BatchAnimCamera : MonoBehaviour
{
	private static readonly float pan_speed = 5f;

	private static readonly float zoom_speed = 5f;

	public static Bounds bounds = new Bounds(new Vector3(0f, 0f, -50f), new Vector3(0f, 0f, 50f));

	private float zoom_min = 1f;

	private float zoom_max = 100f;

	private Camera cam;

	private bool do_pan;

	private Vector3 last_pan;

	private void Awake()
	{
		cam = GetComponent<Camera>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.RightArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.right * pan_speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.left * pan_speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.up * pan_speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.down * pan_speed * Time.deltaTime);
		}
		ClampToBounds();
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetMouseButtonDown(0))
			{
				do_pan = true;
				last_pan = KInputManager.GetMousePos();
			}
			else if (Input.GetMouseButton(0) && do_pan)
			{
				Vector3 vector = cam.ScreenToViewportPoint(last_pan - KInputManager.GetMousePos());
				Vector3 translation = new Vector3(vector.x * pan_speed, vector.y * pan_speed, 0f);
				base.transform.Translate(translation, Space.World);
				ClampToBounds();
				last_pan = KInputManager.GetMousePos();
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			do_pan = false;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - axis * zoom_speed, zoom_min, zoom_max);
		}
	}

	private void ClampToBounds()
	{
		Vector3 position = base.transform.GetPosition();
		Vector3 position2 = base.transform.GetPosition();
		float x = position2.x;
		Vector3 min = bounds.min;
		float x2 = min.x;
		Vector3 max = bounds.max;
		position.x = Mathf.Clamp(x, x2, max.x);
		Vector3 position3 = base.transform.GetPosition();
		float y = position3.y;
		Vector3 min2 = bounds.min;
		float y2 = min2.y;
		Vector3 max2 = bounds.max;
		position.y = Mathf.Clamp(y, y2, max2.y);
		Vector3 position4 = base.transform.GetPosition();
		float z = position4.z;
		Vector3 min3 = bounds.min;
		float z2 = min3.z;
		Vector3 max3 = bounds.max;
		position.z = Mathf.Clamp(z, z2, max3.z);
		base.transform.SetPosition(position);
	}

	private void OnDrawGizmosSelected()
	{
		DebugExtension.DebugBounds(bounds, Color.red, 0f, true);
	}
}
