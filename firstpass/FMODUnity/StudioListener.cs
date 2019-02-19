using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Listener")]
	public class StudioListener : MonoBehaviour
	{
		private Rigidbody rigidBody;

		private Rigidbody2D rigidBody2D;

		public int ListenerNumber;

		private void OnEnable()
		{
			RuntimeUtils.EnforceLibraryOrder();
			if (RuntimeManager.IsInitialized)
			{
				rigidBody = base.gameObject.GetComponent<Rigidbody>();
				rigidBody2D = base.gameObject.GetComponent<Rigidbody2D>();
				RuntimeManager.HasListener[ListenerNumber] = true;
				SetListenerLocation();
			}
			else
			{
				base.enabled = false;
			}
		}

		private void OnDisable()
		{
			RuntimeManager.HasListener[ListenerNumber] = false;
		}

		private void Update()
		{
			SetListenerLocation();
		}

		private void SetListenerLocation()
		{
			if ((bool)rigidBody)
			{
				RuntimeManager.SetListenerLocation(ListenerNumber, base.gameObject, rigidBody);
			}
			else
			{
				RuntimeManager.SetListenerLocation(ListenerNumber, base.gameObject, rigidBody2D);
			}
		}
	}
}
