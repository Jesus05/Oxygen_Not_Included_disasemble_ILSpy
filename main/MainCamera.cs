using UnityEngine;

public class MainCamera : MonoBehaviour
{
	private void Awake()
	{
		if ((Object)Camera.main != (Object)null)
		{
			Object.Destroy(Camera.main.gameObject);
		}
		base.gameObject.tag = "MainCamera";
	}
}
