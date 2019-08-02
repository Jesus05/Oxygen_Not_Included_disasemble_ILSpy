using NodeEditorFramework.Utilities;
using System;
using System.Reflection;

namespace NodeEditorFramework
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ContextFillerAttribute : Attribute
	{
		public ContextType contextType
		{
			get;
			private set;
		}

		public ContextFillerAttribute(ContextType type)
		{
			contextType = type;
		}

		internal static bool AssureValidity(MethodInfo method, ContextFillerAttribute attr)
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 2 && parameters[0].ParameterType == typeof(NodeEditorInputInfo) && parameters[1].ParameterType == typeof(GenericMenu))
				{
					return true;
				}
				Debug.LogWarning("Method " + method.Name + " has incorrect signature for ContextAttribute!");
			}
			return false;
		}
	}
}
