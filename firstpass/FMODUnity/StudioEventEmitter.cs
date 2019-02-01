using FMOD.Studio;
using System;
using System.Threading;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
	public class StudioEventEmitter : MonoBehaviour
	{
		[EventRef]
		public string Event = "";

		public EmitterGameEvent PlayEvent = EmitterGameEvent.None;

		public EmitterGameEvent StopEvent = EmitterGameEvent.None;

		public string CollisionTag = "";

		public bool AllowFadeout = true;

		public bool TriggerOnce = false;

		public bool Preload = false;

		public ParamRef[] Params = new ParamRef[0];

		public bool OverrideAttenuation = false;

		public float OverrideMinDistance = -1f;

		public float OverrideMaxDistance = -1f;

		private EventDescription eventDescription;

		private EventInstance instance;

		private bool hasTriggered = false;

		private bool isQuitting = false;

		public EventDescription EventDescription => eventDescription;

		public EventInstance EventInstance => instance;

		private void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			if (Preload)
			{
				Lookup();
				eventDescription.loadSampleData();
				RuntimeManager.StudioSystem.update();
				eventDescription.getSampleLoadingState(out LOADING_STATE state);
				while (state == LOADING_STATE.LOADING)
				{
					Thread.Sleep(1);
					eventDescription.getSampleLoadingState(out state);
				}
			}
			HandleGameEvent(EmitterGameEvent.ObjectStart);
		}

		private void OnApplicationQuit()
		{
			isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!isQuitting)
			{
				HandleGameEvent(EmitterGameEvent.ObjectDestroy);
				if (instance.isValid())
				{
					RuntimeManager.DetachInstanceFromGameObject(instance);
				}
				if (Preload)
				{
					eventDescription.unloadSampleData();
				}
			}
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
			if (PlayEvent == gameEvent)
			{
				Play();
			}
			if (StopEvent == gameEvent)
			{
				Stop();
			}
		}

		private void Lookup()
		{
			eventDescription = RuntimeManager.GetEventDescription(Event);
		}

		public void Play()
		{
			if ((!TriggerOnce || !hasTriggered) && !string.IsNullOrEmpty(Event))
			{
				if (!eventDescription.isValid())
				{
					Lookup();
				}
				bool oneshot = false;
				if (!Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
				{
					eventDescription.isOneshot(out oneshot);
				}
				eventDescription.is3D(out bool is3D);
				if (!instance.isValid())
				{
					instance.clearHandle();
				}
				if (oneshot && instance.isValid())
				{
					instance.release();
					instance.clearHandle();
				}
				if (!instance.isValid())
				{
					eventDescription.createInstance(out instance);
					if (is3D)
					{
						Rigidbody component = GetComponent<Rigidbody>();
						Rigidbody2D component2 = GetComponent<Rigidbody2D>();
						Transform component3 = GetComponent<Transform>();
						if ((bool)component)
						{
							instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component));
							RuntimeManager.AttachInstanceToGameObject(instance, component3, component);
						}
						else
						{
							instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component2));
							RuntimeManager.AttachInstanceToGameObject(instance, component3, component2);
						}
					}
				}
				ParamRef[] @params = Params;
				foreach (ParamRef paramRef in @params)
				{
					instance.setParameterValue(paramRef.Name, paramRef.Value);
				}
				if (is3D && OverrideAttenuation)
				{
					instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
					instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
				}
				instance.start();
				hasTriggered = true;
			}
		}

		public void Stop()
		{
			if (instance.isValid())
			{
				instance.stop((!AllowFadeout) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
				instance.release();
				instance.clearHandle();
			}
		}

		public void SetParameter(string name, float value)
		{
			if (instance.isValid())
			{
				instance.setParameterValue(name, value);
			}
		}

		public bool IsPlaying()
		{
			if (instance.isValid() && instance.isValid())
			{
				instance.getPlaybackState(out PLAYBACK_STATE state);
				return state != PLAYBACK_STATE.STOPPED;
			}
			return false;
		}
	}
}
