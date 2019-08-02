using System;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class EventHandlerAttribute : Attribute
	{
		public EventType? handledEvent
		{
			get;
			private set;
		}

		public int priority
		{
			get;
			private set;
		}

		public EventHandlerAttribute(EventType eventType, int priorityValue)
		{
			handledEvent = eventType;
			priority = priorityValue;
		}

		public EventHandlerAttribute(int priorityValue)
		{
			handledEvent = null;
			priority = priorityValue;
		}

		public EventHandlerAttribute(EventType eventType)
		{
			handledEvent = eventType;
			priority = 50;
		}

		public EventHandlerAttribute()
		{
			handledEvent = null;
		}

		internal static bool AssureValidity(MethodInfo method, EventHandlerAttribute attr)
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(NodeEditorInputInfo))
				{
					return true;
				}
				Debug.LogWarning("Method " + method.Name + " has incorrect signature for EventHandlerAttribute!");
			}
			return false;
		}
	}
}
