using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Parameter Trigger")]
	public class StudioParameterTrigger : MonoBehaviour
	{
		public EmitterRef[] Emitters;

		public EmitterGameEvent TriggerEvent;

		public string CollisionTag;

		private void Start()
		{
			HandleGameEvent(EmitterGameEvent.ObjectStart);
		}

		private void OnDestroy()
		{
			HandleGameEvent(EmitterGameEvent.ObjectDestroy);
		}

		private void OnEnable()
		{
			HandleGameEvent(EmitterGameEvent.ObjectEnable);
		}

		private void OnDisable()
		{
			HandleGameEvent(EmitterGameEvent.ObjectDisable);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(EmitterGameEvent.TriggerEnter);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(EmitterGameEvent.TriggerExit);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(EmitterGameEvent.TriggerEnter2D);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
			{
				HandleGameEvent(EmitterGameEvent.TriggerExit2D);
			}
		}

		private void OnCollisionEnter()
		{
			HandleGameEvent(EmitterGameEvent.CollisionEnter);
		}

		private void OnCollisionExit()
		{
			HandleGameEvent(EmitterGameEvent.CollisionExit);
		}

		private void OnCollisionEnter2D()
		{
			HandleGameEvent(EmitterGameEvent.CollisionEnter2D);
		}

		private void OnCollisionExit2D()
		{
			HandleGameEvent(EmitterGameEvent.CollisionExit2D);
		}

		private void HandleGameEvent(EmitterGameEvent gameEvent)
		{
			if (TriggerEvent == gameEvent)
			{
				TriggerParameters();
			}
		}

		public void TriggerParameters()
		{
			for (int i = 0; i < Emitters.Length; i++)
			{
				EmitterRef emitterRef = Emitters[i];
				if ((Object)emitterRef.Target != (Object)null)
				{
					for (int j = 0; j < Emitters[i].Params.Length; j++)
					{
						emitterRef.Target.SetParameter(Emitters[i].Params[j].Name, Emitters[i].Params[j].Value);
					}
				}
			}
		}
	}
}
