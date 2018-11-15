using System;
using System.Reflection;

namespace NodeEditorFramework
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ContextEntryAttribute : Attribute
	{
		public ContextType contextType
		{
			get;
			private set;
		}

		public string contextPath
		{
			get;
			private set;
		}

		public ContextEntryAttribute(ContextType type, string path)
		{
			contextType = type;
			contextPath = path;
		}

		internal static bool AssureValidity(MethodInfo method, ContextEntryAttribute attr)
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(NodeEditorInputInfo))
				{
					return true;
				}
				Debug.LogWarning("Method " + method.Name + " has incorrect signature for ContextAttribute!", null);
			}
			return false;
		}
	}
}
