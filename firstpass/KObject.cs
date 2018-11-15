using UnityEngine;

public class KObject
{
	private EventSystem eventSystem;

	public int id
	{
		get;
		private set;
	}

	public bool hasEventSystem => eventSystem != null;

	public KObject(GameObject go)
	{
		id = go.GetInstanceID();
	}

	~KObject()
	{
		OnCleanUp();
	}

	public void OnCleanUp()
	{
		if (eventSystem != null)
		{
			eventSystem.OnCleanUp();
			eventSystem = null;
		}
	}

	public EventSystem GetEventSystem()
	{
		if (eventSystem == null)
		{
			eventSystem = new EventSystem();
		}
		return eventSystem;
	}
}
