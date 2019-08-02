using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem
{
	private struct Entry
	{
		public Action<object> handler;

		public int hash;

		public int id;

		public Entry(int hash, Action<object> handler, int id)
		{
			this.handler = handler;
			this.hash = hash;
			this.id = id;
		}
	}

	private struct SubscribedEntry
	{
		public Action<object> handler;

		public int hash;

		public GameObject go;

		public SubscribedEntry(GameObject go, int hash, Action<object> handler)
		{
			this.go = go;
			this.hash = hash;
			this.handler = handler;
		}
	}

	private struct IntraObjectRoute
	{
		public int eventHash;

		public int handlerIndex;

		public IntraObjectRoute(int eventHash, int handlerIndex)
		{
			this.eventHash = eventHash;
			this.handlerIndex = handlerIndex;
		}

		public bool IsValid()
		{
			return eventHash != 0;
		}
	}

	public abstract class IntraObjectHandlerBase
	{
		public abstract void Trigger(GameObject gameObject, object eventData);
	}

	public class IntraObjectHandler<ComponentType> : IntraObjectHandlerBase
	{
		private Action<ComponentType, object> handler;

		public IntraObjectHandler(Action<ComponentType, object> handler)
		{
			Debug.Assert(handler.Method.IsStatic);
			this.handler = handler;
		}

		public static implicit operator IntraObjectHandler<ComponentType>(Action<ComponentType, object> handler)
		{
			return new IntraObjectHandler<ComponentType>(handler);
		}

		public override void Trigger(GameObject gameObject, object eventData)
		{
			ListPool<ComponentType, IntraObjectHandler<ComponentType>>.PooledList pooledList = ListPool<ComponentType, IntraObjectHandler<ComponentType>>.Allocate();
			gameObject.GetComponents<ComponentType>((List<ComponentType>)pooledList);
			foreach (ComponentType item in pooledList)
			{
				handler(item, eventData);
			}
			pooledList.Recycle();
		}

		public override string ToString()
		{
			return ((handler.Target == null) ? "STATIC" : handler.Target.GetType().ToString()) + "." + handler.Method.ToString();
		}
	}

	private static bool ENABLE_DETAILED_EVENT_PROFILE_INFO = false;

	private int nextId;

	private int currentlyTriggering;

	private bool dirty;

	private ArrayRef<SubscribedEntry> subscribedEvents;

	private ArrayRef<Entry> entries;

	private ArrayRef<IntraObjectRoute> intraObjectRoutes;

	private static Dictionary<int, List<IntraObjectHandlerBase>> intraObjectDispatcher = new Dictionary<int, List<IntraObjectHandlerBase>>();

	private LoggerFIO log;

	public EventSystem()
	{
		log = new LoggerFIO("Events", 35);
	}

	public void Trigger(GameObject go, int hash, object data = null)
	{
		if (!App.IsExiting)
		{
			currentlyTriggering++;
			for (int i = 0; i != intraObjectRoutes.size; i++)
			{
				IntraObjectRoute intraObjectRoute = intraObjectRoutes[i];
				if (intraObjectRoute.eventHash == hash)
				{
					List<IntraObjectHandlerBase> list = intraObjectDispatcher[hash];
					IntraObjectRoute intraObjectRoute2 = intraObjectRoutes[i];
					list[intraObjectRoute2.handlerIndex].Trigger(go, data);
				}
			}
			int size = entries.size;
			if (ENABLE_DETAILED_EVENT_PROFILE_INFO)
			{
				for (int j = 0; j < size; j++)
				{
					Entry entry = entries[j];
					if (entry.hash == hash)
					{
						Entry entry2 = entries[j];
						if (entry2.handler != null)
						{
							Entry entry3 = entries[j];
							entry3.handler(data);
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < size; k++)
				{
					Entry entry4 = entries[k];
					if (entry4.hash == hash)
					{
						Entry entry5 = entries[k];
						if (entry5.handler != null)
						{
							Entry entry6 = entries[k];
							entry6.handler(data);
						}
					}
				}
			}
			currentlyTriggering--;
			if (dirty && currentlyTriggering == 0)
			{
				dirty = false;
				entries.RemoveAllSwap((Entry x) => x.handler == null);
				intraObjectRoutes.RemoveAllSwap((IntraObjectRoute route) => !route.IsValid());
			}
		}
	}

	public void OnCleanUp()
	{
		for (int num = subscribedEvents.size - 1; num >= 0; num--)
		{
			SubscribedEntry subscribedEntry = subscribedEvents[num];
			if ((UnityEngine.Object)subscribedEntry.go != (UnityEngine.Object)null)
			{
				Unsubscribe(subscribedEntry.go, subscribedEntry.hash, subscribedEntry.handler);
			}
		}
		for (int i = 0; i < entries.size; i++)
		{
			Entry value = entries[i];
			value.handler = null;
			entries[i] = value;
		}
		entries.Clear();
		subscribedEvents.Clear();
		intraObjectRoutes.Clear();
	}

	public void UnregisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		int num = 0;
		while (true)
		{
			if (num >= subscribedEvents.size)
			{
				return;
			}
			SubscribedEntry subscribedEntry = subscribedEvents[num];
			if (subscribedEntry.hash == eventName)
			{
				SubscribedEntry subscribedEntry2 = subscribedEvents[num];
				if (subscribedEntry2.handler == handler)
				{
					SubscribedEntry subscribedEntry3 = subscribedEvents[num];
					if ((UnityEngine.Object)subscribedEntry3.go == (UnityEngine.Object)target)
					{
						break;
					}
				}
			}
			num++;
		}
		subscribedEvents.RemoveAt(num);
	}

	public void RegisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		subscribedEvents.Add(new SubscribedEntry(target, eventName, handler));
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		entries.Add(new Entry(hash, handler, ++nextId));
		return nextId;
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		int num = 0;
		while (true)
		{
			if (num >= entries.size)
			{
				return;
			}
			Entry entry = entries[num];
			if (entry.hash == hash)
			{
				Entry entry2 = entries[num];
				if (entry2.handler == handler)
				{
					break;
				}
			}
			num++;
		}
		if (currentlyTriggering == 0)
		{
			entries.RemoveAt(num);
		}
		else
		{
			dirty = true;
			Entry value = entries[num];
			value.handler = null;
			entries[num] = value;
		}
	}

	public void Unsubscribe(int id)
	{
		int num = 0;
		while (true)
		{
			if (num >= entries.size)
			{
				return;
			}
			Entry entry = entries[num];
			if (entry.id == id)
			{
				break;
			}
			num++;
		}
		if (currentlyTriggering == 0)
		{
			entries.RemoveAt(num);
		}
		else
		{
			dirty = true;
			Entry value = entries[num];
			value.handler = null;
			entries[num] = value;
		}
	}

	public int Subscribe(GameObject target, int eventName, Action<object> handler)
	{
		RegisterEvent(target, eventName, handler);
		KObject orCreateObject = KObjectManager.Instance.GetOrCreateObject(target);
		return orCreateObject.GetEventSystem().Subscribe(eventName, handler);
	}

	public int Subscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler)
	{
		if (!intraObjectDispatcher.TryGetValue(eventName, out List<IntraObjectHandlerBase> value))
		{
			value = new List<IntraObjectHandlerBase>();
			intraObjectDispatcher.Add(eventName, value);
		}
		int num = value.IndexOf((IntraObjectHandlerBase)handler);
		if (num == -1)
		{
			value.Add((IntraObjectHandlerBase)handler);
			num = value.Count - 1;
		}
		intraObjectRoutes.Add(new IntraObjectRoute(eventName, num));
		return num;
	}

	public void Unsubscribe(GameObject target, int eventName, Action<object> handler)
	{
		UnregisterEvent(target, eventName, handler);
		if (!((UnityEngine.Object)target == (UnityEngine.Object)null))
		{
			KObject orCreateObject = KObjectManager.Instance.GetOrCreateObject(target);
			orCreateObject.GetEventSystem().Unsubscribe(eventName, handler);
		}
	}

	public void Unsubscribe(int eventName, int subscribeHandle, bool suppressWarnings = false)
	{
		int num = intraObjectRoutes.FindIndex((IntraObjectRoute route) => route.eventHash == eventName && route.handlerIndex == subscribeHandle);
		if (num == -1)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarning("Failed to Unsubscribe event handler: " + intraObjectDispatcher[eventName][subscribeHandle].ToString() + "\nNot subscribed to event");
			}
		}
		else if (currentlyTriggering == 0)
		{
			intraObjectRoutes.RemoveAtSwap(num);
		}
		else
		{
			dirty = true;
			intraObjectRoutes[num] = default(IntraObjectRoute);
		}
	}

	public void Unsubscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler, bool suppressWarnings)
	{
		if (!intraObjectDispatcher.TryGetValue(eventName, out List<IntraObjectHandlerBase> value))
		{
			if (!suppressWarnings)
			{
				Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNo subscriptions have been made to event");
			}
		}
		else
		{
			int num = value.IndexOf((IntraObjectHandlerBase)handler);
			if (num == -1)
			{
				if (!suppressWarnings)
				{
					Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNot subscribed to event");
				}
			}
			else
			{
				Unsubscribe(eventName, num, suppressWarnings);
			}
		}
	}

	public void Unsubscribe(string[] eventNames, Action<object> handler)
	{
		foreach (string s in eventNames)
		{
			int hash = Hash.SDBMLower(s);
			Unsubscribe(hash, handler);
		}
	}
}
