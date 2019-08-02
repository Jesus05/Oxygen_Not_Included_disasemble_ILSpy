using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class ConnectionTypes
	{
		private static Dictionary<string, TypeData> types;

		private static Type NullType => typeof(ConnectionTypes);

		public static Type GetType(string typeName)
		{
			return GetTypeData(typeName).Type ?? NullType;
		}

		public static TypeData GetTypeData(string typeName)
		{
			if (types == null || types.Count == 0)
			{
				FetchTypes();
			}
			if (!types.TryGetValue(typeName, out TypeData value))
			{
				Type type = Type.GetType(typeName);
				if (type == null)
				{
					value = types.First().Value;
					Debug.LogError("No TypeData defined for: " + typeName + " and type could not be found either");
				}
				else
				{
					value = ((types.Values.Count > 0) ? types.Values.First((TypeData data) => data.isValid() && data.Type == type) : null);
					if (value == null)
					{
						types.Add(typeName, value = new TypeData(type));
					}
				}
			}
			return value;
		}

		public static TypeData GetTypeData(Type type)
		{
			if (types == null || types.Count == 0)
			{
				FetchTypes();
			}
			TypeData typeData = (types.Values.Count > 0) ? types.Values.First((TypeData data) => data.isValid() && data.Type == type) : null;
			if (typeData == null)
			{
				types.Add(type.Name, typeData = new TypeData(type));
			}
			return typeData;
		}

		internal static void FetchTypes()
		{
			Dictionary<string, TypeData> dictionary = new Dictionary<string, TypeData>();
			dictionary.Add("None", new TypeData(typeof(object)));
			types = dictionary;
			IEnumerable<Assembly> enumerable = from assembly in AppDomain.CurrentDomain.GetAssemblies()
			where assembly.FullName.Contains("Assembly")
			select assembly;
			foreach (Assembly item in enumerable)
			{
				foreach (Type item2 in from T in item.GetTypes()
				where T.IsClass && !T.IsAbstract && Enumerable.Contains(T.GetInterfaces(), typeof(IConnectionTypeDeclaration))
				select T)
				{
					IConnectionTypeDeclaration connectionTypeDeclaration = item.CreateInstance(item2.FullName) as IConnectionTypeDeclaration;
					if (connectionTypeDeclaration == null)
					{
						throw new UnityException("Error with Type Declaration " + item2.FullName);
					}
					types.Add(connectionTypeDeclaration.Identifier, new TypeData(connectionTypeDeclaration));
				}
			}
		}
	}
}
