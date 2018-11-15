using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimEventManager
{
	private struct AnimData
	{
		public float frameRate;

		public float totalTime;

		public int numFrames;

		public bool useUnscaledTime;
	}

	[DebuggerDisplay("{controller.name}, Anim={currentAnim}, Frame={currentFrame}, Mode={mode}")]
	public struct EventPlayerData
	{
		public float elapsedTime;

		public KAnim.PlayMode mode;

		public List<AnimEvent> events;

		public List<AnimEvent> updatingEvents;

		public KBatchedAnimController controller;

		public int currentFrame
		{
			get;
			set;
		}

		public int previousFrame
		{
			get;
			set;
		}

		public string name => controller.name;

		public float normalizedTime => elapsedTime / controller.CurrentAnim.totalTime;

		public string currentAnimFile => controller.currentAnimFile;

		public KAnimHashedString currentAnimFileHash => controller.currentAnimFileHash;

		public string currentAnim => controller.currentAnim;

		public Vector3 position => controller.transform.GetPosition();

		public ComponentType GetComponent<ComponentType>()
		{
			return controller.GetComponent<ComponentType>();
		}

		public void AddUpdatingEvent(AnimEvent ev)
		{
			if (updatingEvents == null)
			{
				updatingEvents = new List<AnimEvent>();
			}
			updatingEvents.Add(ev);
		}

		public void SetElapsedTime(float elapsedTime)
		{
			this.elapsedTime = elapsedTime;
		}

		public void FreeResources()
		{
			elapsedTime = 0f;
			mode = KAnim.PlayMode.Once;
			currentFrame = 0;
			previousFrame = 0;
			events = null;
			updatingEvents = null;
			controller = null;
		}
	}

	private struct IndirectionData
	{
		public bool isUIData;

		public HandleVector<int>.Handle animDataHandle;

		public HandleVector<int>.Handle eventDataHandle;

		public IndirectionData(HandleVector<int>.Handle anim_data_handle, HandleVector<int>.Handle event_data_handle, bool is_ui_data)
		{
			isUIData = is_ui_data;
			animDataHandle = anim_data_handle;
			eventDataHandle = event_data_handle;
		}
	}

	private static readonly List<AnimEvent> emptyEventList = new List<AnimEvent>();

	private const int INITIAL_VECTOR_SIZE = 256;

	private KCompactedVector<AnimData> animData = new KCompactedVector<AnimData>(256);

	private KCompactedVector<EventPlayerData> eventData = new KCompactedVector<EventPlayerData>(256);

	private KCompactedVector<AnimData> uiAnimData = new KCompactedVector<AnimData>(256);

	private KCompactedVector<EventPlayerData> uiEventData = new KCompactedVector<EventPlayerData>(256);

	private KCompactedVector<IndirectionData> indirectionData = new KCompactedVector<IndirectionData>(0);

	private List<KBatchedAnimController> finishedCalls = new List<KBatchedAnimController>();

	public void FreeResources()
	{
	}

	public HandleVector<int>.Handle PlayAnim(KAnimControllerBase controller, KAnim.Anim anim, KAnim.PlayMode mode, float time, bool use_unscaled_time)
	{
		AnimData initial_data = default(AnimData);
		initial_data.frameRate = anim.frameRate;
		initial_data.totalTime = anim.totalTime;
		initial_data.numFrames = anim.numFrames;
		initial_data.useUnscaledTime = use_unscaled_time;
		EventPlayerData initial_data2 = default(EventPlayerData);
		initial_data2.elapsedTime = time;
		initial_data2.mode = mode;
		initial_data2.controller = (controller as KBatchedAnimController);
		initial_data2.currentFrame = initial_data2.controller.GetFrameIdx(initial_data2.elapsedTime, false);
		initial_data2.previousFrame = -1;
		initial_data2.events = null;
		initial_data2.updatingEvents = null;
		initial_data2.events = GameAudioSheets.Get().GetEvents(anim.id);
		if (initial_data2.events == null)
		{
			initial_data2.events = emptyEventList;
		}
		if (!initial_data.useUnscaledTime)
		{
			HandleVector<int>.Handle anim_data_handle = animData.Allocate(initial_data);
			HandleVector<int>.Handle event_data_handle = eventData.Allocate(initial_data2);
			return indirectionData.Allocate(new IndirectionData(anim_data_handle, event_data_handle, false));
		}
		HandleVector<int>.Handle anim_data_handle2 = uiAnimData.Allocate(initial_data);
		HandleVector<int>.Handle event_data_handle2 = uiEventData.Allocate(initial_data2);
		return indirectionData.Allocate(new IndirectionData(anim_data_handle2, event_data_handle2, true));
	}

	public void SetMode(HandleVector<int>.Handle handle, KAnim.PlayMode mode)
	{
		if (handle.IsValid())
		{
			IndirectionData data = indirectionData.GetData(handle);
			KCompactedVector<EventPlayerData> kCompactedVector = (!data.isUIData) ? eventData : uiEventData;
			EventPlayerData data2 = kCompactedVector.GetData(data.eventDataHandle);
			data2.mode = mode;
			kCompactedVector.SetData(data.eventDataHandle, data2);
		}
	}

	public void StopAnim(HandleVector<int>.Handle handle)
	{
		if (handle.IsValid())
		{
			IndirectionData data = indirectionData.GetData(handle);
			KCompactedVector<AnimData> kCompactedVector = (!data.isUIData) ? animData : uiAnimData;
			KCompactedVector<EventPlayerData> kCompactedVector2 = (!data.isUIData) ? eventData : uiEventData;
			EventPlayerData data2 = kCompactedVector2.GetData(data.eventDataHandle);
			StopEvents(data2);
			data.animDataHandle = kCompactedVector.Free(data.animDataHandle);
			data.eventDataHandle = kCompactedVector2.Free(data.eventDataHandle);
			indirectionData.SetData(handle, data);
		}
	}

	public float GetElapsedTime(HandleVector<int>.Handle handle)
	{
		IndirectionData data = indirectionData.GetData(handle);
		KCompactedVector<EventPlayerData> kCompactedVector = (!data.isUIData) ? eventData : uiEventData;
		EventPlayerData data2 = kCompactedVector.GetData(data.eventDataHandle);
		return data2.elapsedTime;
	}

	public void SetElapsedTime(HandleVector<int>.Handle handle, float elapsed_time)
	{
		IndirectionData data = indirectionData.GetData(handle);
		KCompactedVector<EventPlayerData> kCompactedVector = (!data.isUIData) ? eventData : uiEventData;
		EventPlayerData data2 = kCompactedVector.GetData(data.eventDataHandle);
		data2.elapsedTime = elapsed_time;
		kCompactedVector.SetData(data.eventDataHandle, data2);
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Update(deltaTime, animData.GetDataList(), eventData.GetDataList());
		Update(unscaledDeltaTime, uiAnimData.GetDataList(), uiEventData.GetDataList());
		for (int i = 0; i < finishedCalls.Count; i++)
		{
			finishedCalls[i].TriggerStop();
		}
		finishedCalls.Clear();
	}

	private void Update(float dt, List<AnimData> anim_data, List<EventPlayerData> event_data)
	{
		if (!(dt <= 0f))
		{
			for (int i = 0; i < event_data.Count; i++)
			{
				EventPlayerData eventPlayerData = event_data[i];
				if (!((Object)eventPlayerData.controller == (Object)null))
				{
					eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
					event_data[i] = eventPlayerData;
					PlayEvents(eventPlayerData);
					eventPlayerData.previousFrame = eventPlayerData.currentFrame;
					eventPlayerData.elapsedTime += dt * eventPlayerData.controller.GetPlaySpeed();
					event_data[i] = eventPlayerData;
					if (eventPlayerData.mode != KAnim.PlayMode.Paused)
					{
						if (eventPlayerData.updatingEvents != null)
						{
							for (int j = 0; j < eventPlayerData.updatingEvents.Count; j++)
							{
								AnimEvent animEvent = eventPlayerData.updatingEvents[j];
								animEvent.OnUpdate(eventPlayerData);
							}
						}
						event_data[i] = eventPlayerData;
						if (eventPlayerData.mode != 0)
						{
							int currentFrame = eventPlayerData.currentFrame;
							AnimData animData = anim_data[i];
							if (currentFrame >= animData.numFrames - 1)
							{
								StopEvents(eventPlayerData);
								finishedCalls.Add(eventPlayerData.controller);
							}
						}
					}
				}
			}
		}
	}

	private void PlayEvents(EventPlayerData data)
	{
		for (int i = 0; i < data.events.Count; i++)
		{
			AnimEvent animEvent = data.events[i];
			animEvent.Play(data);
		}
	}

	private void StopEvents(EventPlayerData data)
	{
		for (int i = 0; i < data.events.Count; i++)
		{
			AnimEvent animEvent = data.events[i];
			animEvent.Stop(data);
		}
		if (data.updatingEvents != null)
		{
			data.updatingEvents.Clear();
		}
	}
}
