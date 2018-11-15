using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EventLogger<EventInstanceType, EventType> : KMonoBehaviour, ISaveLoadable where EventInstanceType : EventInstanceBase where EventType : EventBase
{
	private const int MAX_NUM_EVENTS = 10000;

	private List<EventType> Events = new List<EventType>();

	[Serialize]
	private List<EventInstanceType> EventInstances = new List<EventInstanceType>();

	public IEnumerator<EventInstanceType> GetEnumerator()
	{
		return EventInstances.GetEnumerator();
	}

	public EventType AddEvent(EventType ev)
	{
		for (int i = 0; i < Events.Count; i++)
		{
			if (Events[i].hash == ev.hash)
			{
				Events[i] = ev;
				return Events[i];
			}
		}
		Events.Add(ev);
		return ev;
	}

	public EventInstanceType Add(EventInstanceType ev)
	{
		if (EventInstances.Count > 10000)
		{
			EventInstances.RemoveAt(0);
		}
		EventInstances.Add(ev);
		return ev;
	}

	[OnDeserialized]
	protected internal void OnDeserialized()
	{
		if (EventInstances.Count > 10000)
		{
			EventInstances.RemoveRange(0, EventInstances.Count - 10000);
		}
		for (int i = 0; i < EventInstances.Count; i++)
		{
			for (int j = 0; j < Events.Count; j++)
			{
				if (Events[j].hash == EventInstances[i].eventHash)
				{
					EventInstances[i].ev = Events[j];
					break;
				}
			}
		}
	}

	public void Clear()
	{
		EventInstances.Clear();
	}
}
