using UnityEngine;

public class CameraFollowHelper : KMonoBehaviour
{
	private void LateUpdate()
	{
		if ((Object)CameraController.Instance != (Object)null)
		{
			CameraController.Instance.UpdateFollowTarget();
		}
	}
}
