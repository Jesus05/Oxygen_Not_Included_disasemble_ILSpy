#define DEBUG_LOG
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KSerialization
{
	public class Manager
	{
		private static Dictionary<string, SerializationTemplate> serializationTemplatesByTypeName = new Dictionary<string, SerializationTemplate>();

		private static Dictionary<string, DeserializationTemplate> deserializationTemplatesByTypeName = new Dictionary<string, DeserializationTemplate>();

		private static Dictionary<Type, SerializationTemplate> serializationTemplatesByType = new Dictionary<Type, SerializationTemplate>();

		private static Dictionary<Type, DeserializationTemplate> deserializationTemplatesByType = new Dictionary<Type, DeserializationTemplate>();

		private static Dictionary<DeserializationTemplate, KeyValuePair<SerializationTemplate, DeserializationMapping>> deserializationMappings = new Dictionary<DeserializationTemplate, KeyValuePair<SerializationTemplate, DeserializationMapping>>();

		private static Dictionary<Type, TypeInfo> typeInfoMap = new Dictionary<Type, TypeInfo>();

		private static Assembly[] assemblies = null;

		public static void Initialize()
		{
			assemblies = AppDomain.CurrentDomain.GetAssemblies();
		}

		public static Type GetType(string type_name)
		{
			Type type = Type.GetType(type_name);
			if (type == null)
			{
				Assembly[] array = assemblies;
				foreach (Assembly assembly in array)
				{
					type = assembly.GetType(type_name);
					if (type != null)
					{
						break;
					}
				}
			}
			if (type == null)
			{
				DebugLog.Output(DebugLog.Level.Warning, "Failed to find type named: " + type_name);
			}
			return type;
		}

		public static TypeInfo GetTypeInfo(Type type)
		{
			if (typeInfoMap.TryGetValue(type, out TypeInfo value))
			{
				return value;
			}
			value = EncodeTypeInfo(type);
			return value;
		}

		public static SerializationTemplate GetSerializationTemplate(Type type)
		{
			if (type == null)
			{
				throw new InvalidOperationException("Invalid type encountered when serializing");
			}
			SerializationTemplate value = null;
			if (!serializationTemplatesByType.TryGetValue(type, out value))
			{
				value = new SerializationTemplate(type);
				serializationTemplatesByType[type] = value;
				serializationTemplatesByTypeName[type.GetKTypeString()] = value;
			}
			return value;
		}

		public static SerializationTemplate GetSerializationTemplate(string type_name)
		{
			if (type_name == null || type_name == string.Empty)
			{
				throw new InvalidOperationException("Invalid type name encountered when serializing");
			}
			SerializationTemplate value = null;
			if (!serializationTemplatesByTypeName.TryGetValue(type_name, out value))
			{
				Type type = GetType(type_name);
				if (type != null)
				{
					value = new SerializationTemplate(type);
					serializationTemplatesByType[type] = value;
					serializationTemplatesByTypeName[type_name] = value;
				}
			}
			return value;
		}

		public static DeserializationTemplate GetDeserializationTemplate(Type type)
		{
			DeserializationTemplate value = null;
			deserializationTemplatesByType.TryGetValue(type, out value);
			return value;
		}

		public static DeserializationTemplate GetDeserializationTemplate(string type_name)
		{
			DeserializationTemplate value = null;
			deserializationTemplatesByTypeName.TryGetValue(type_name, out value);
			return value;
		}

		public static void SerializeDirectory(BinaryWriter writer)
		{
			writer.Write(serializationTemplatesByTypeName.Count);
			foreach (KeyValuePair<string, SerializationTemplate> item in serializationTemplatesByTypeName)
			{
				string key = item.Key;
				SerializationTemplate value = item.Value;
				try
				{
					writer.WriteKleiString(key);
					value.SerializeTemplate(writer);
				}
				catch (Exception ex)
				{
					DebugUtil.LogErrorArgs("Error serializing template " + key + "\n", ex.Message, ex.StackTrace);
				}
			}
		}

		public static void DeserializeDirectory(IReader reader)
		{
			deserializationTemplatesByTypeName.Clear();
			deserializationTemplatesByType.Clear();
			deserializationMappings.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				try
				{
					DeserializationTemplate value = new DeserializationTemplate(text, reader);
					deserializationTemplatesByTypeName[text] = value;
					Type type = GetType(text);
					if (type != null)
					{
						deserializationTemplatesByType[type] = value;
					}
				}
				catch (Exception ex)
				{
					string text2 = "Error deserializing template " + text + "\n" + ex.Message + "\n" + ex.StackTrace;
					Debug.LogError(text2);
					throw new Exception(text2, ex);
				}
			}
		}

		public static void Clear()
		{
			serializationTemplatesByTypeName.Clear();
			serializationTemplatesByType.Clear();
			deserializationTemplatesByTypeName.Clear();
			deserializationTemplatesByType.Clear();
			deserializationMappings.Clear();
			typeInfoMap.Clear();
			Helper.ClearTypeInfoMask();
		}

		public static bool HasDeserializationMapping(Type type)
		{
			return GetDeserializationTemplate(type) != null && GetSerializationTemplate(type) != null;
		}

		public static DeserializationMapping GetDeserializationMapping(Type type)
		{
			DeserializationTemplate deserializationTemplate = GetDeserializationTemplate(type);
			if (deserializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize a class named: " + type.GetKTypeString() + " but no such class exists");
			}
			SerializationTemplate serializationTemplate = GetSerializationTemplate(type);
			if (serializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize into a class named: " + type.GetKTypeString() + " but no such class exists");
			}
			return GetMapping(deserializationTemplate, serializationTemplate);
		}

		public static DeserializationMapping GetDeserializationMapping(string type_name)
		{
			DeserializationTemplate deserializationTemplate = GetDeserializationTemplate(type_name);
			if (deserializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize a class named: " + type_name + " but no such class exists");
			}
			SerializationTemplate serializationTemplate = GetSerializationTemplate(type_name);
			if (serializationTemplate == null)
			{
				throw new ArgumentException("Tried to deserialize into a class named: " + type_name + " but no such class exists");
			}
			return GetMapping(deserializationTemplate, serializationTemplate);
		}

		private static DeserializationMapping GetMapping(DeserializationTemplate dtemplate, SerializationTemplate stemplate)
		{
			DeserializationMapping deserializationMapping = null;
			if (deserializationMappings.TryGetValue(dtemplate, out KeyValuePair<SerializationTemplate, DeserializationMapping> value))
			{
				deserializationMapping = value.Value;
			}
			else
			{
				deserializationMapping = new DeserializationMapping(dtemplate, stemplate);
				value = new KeyValuePair<SerializationTemplate, DeserializationMapping>(stemplate, deserializationMapping);
				deserializationMappings[dtemplate] = value;
			}
			return deserializationMapping;
		}

		private static TypeInfo EncodeTypeInfo(Type type)
		{
			TypeInfo typeInfo = new TypeInfo();
			typeInfo.type = type;
			typeInfo.info = Helper.EncodeSerializationType(type);
			if (type.IsGenericType)
			{
				typeInfo.genericTypeArgs = type.GetGenericArguments();
				typeInfo.subTypes = new TypeInfo[typeInfo.genericTypeArgs.Length];
				for (int i = 0; i < typeInfo.genericTypeArgs.Length; i++)
				{
					typeInfo.subTypes[i] = GetTypeInfo(typeInfo.genericTypeArgs[i]);
				}
			}
			else if (typeof(Array).IsAssignableFrom(type))
			{
				Type elementType = type.GetElementType();
				typeInfo.subTypes = new TypeInfo[1];
				typeInfo.subTypes[0] = GetTypeInfo(elementType);
			}
			return typeInfo;
		}
	}
}
