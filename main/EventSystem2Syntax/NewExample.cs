using System;
using System.Runtime.CompilerServices;

namespace EventSystem2Syntax
{
	internal class NewExample : KMonoBehaviour2
	{
		private struct ObjectDestroyedEvent : IEventData
		{
			public bool parameter;
		}

		[CompilerGenerated]
		private static Action<NewExample, ObjectDestroyedEvent> _003C_003Ef__mg_0024cache0;

		protected override void OnPrefabInit()
		{
			Subscribe<NewExample, ObjectDestroyedEvent>(OnObjectDestroyed);
			Trigger(new ObjectDestroyedEvent
			{
				parameter = false
			});
		}

		private static void OnObjectDestroyed(NewExample example, ObjectDestroyedEvent evt)
		{
		}
	}
}
