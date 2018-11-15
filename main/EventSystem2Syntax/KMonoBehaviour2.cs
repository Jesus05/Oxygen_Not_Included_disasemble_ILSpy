using System;

namespace EventSystem2Syntax
{
	internal class KMonoBehaviour2
	{
		protected virtual void OnPrefabInit()
		{
		}

		public void Subscribe(int evt, Action<object> cb)
		{
		}

		public void Trigger(int evt, object data)
		{
		}

		public void Subscribe<ListenerType, EventType>(Action<ListenerType, EventType> cb) where EventType : IEventData
		{
		}

		public void Trigger<EventType>(EventType evt) where EventType : IEventData
		{
		}
	}
}
