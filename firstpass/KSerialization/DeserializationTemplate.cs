#define DEBUG_LOG
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace KSerialization
{
	public class DeserializationTemplate
	{
		public struct SerializedInfo
		{
			public string name;

			public TypeInfo typeInfo;
		}

		public string typeName;

		public MethodInfo onDeserializing;

		public MethodInfo onDeserialized;

		public List<SerializedInfo> serializedMembers = new List<SerializedInfo>();

		public DeserializationTemplate(string template_type_name, IReader reader)
		{
			typeName = template_type_name;
			DebugLog.Output(DebugLog.Level.Info, "Loading Deserialization Template: " + template_type_name);
			Manager.GetType(template_type_name)?.GetSerializationMethods(typeof(OnDeserializingAttribute), typeof(OnDeserializedAttribute), out onDeserializing, out onDeserialized);
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				DebugLog.Output(DebugLog.Level.Info, "Field " + i.ToString());
				string text = reader.ReadKleiString();
				DebugLog.Output(DebugLog.Level.Info, "Field " + i.ToString() + " == " + text);
				TypeInfo typeInfo = ReadType(reader);
				if (typeInfo.type == null)
				{
					string msg = $"Unknown type encountered while dserializing template {template_type_name} field {i} ({text}) at offset {reader.Position}";
					DebugLog.Output(DebugLog.Level.Warning, msg);
				}
				serializedMembers.Add(new SerializedInfo
				{
					name = text,
					typeInfo = typeInfo
				});
			}
			for (int j = 0; j < num2; j++)
			{
				DebugLog.Output(DebugLog.Level.Info, "Property " + j.ToString());
				string text2 = reader.ReadKleiString();
				DebugLog.Output(DebugLog.Level.Info, "Property " + j.ToString() + " == " + text2);
				TypeInfo typeInfo2 = ReadType(reader);
				if (typeInfo2.type == null)
				{
					string msg2 = $"Unknown type encountered while dserializing template {template_type_name} property {j} ({text2}) at offset {reader.Position}";
					DebugLog.Output(DebugLog.Level.Info, msg2);
				}
				serializedMembers.Add(new SerializedInfo
				{
					name = text2,
					typeInfo = typeInfo2
				});
			}
			DebugLog.Output(DebugLog.Level.Info, "Finished loading template " + template_type_name);
		}

		private TypeInfo ReadType(IReader reader)
		{
			TypeInfo typeInfo = new TypeInfo();
			byte b = (byte)(typeInfo.info = (SerializationTypeInfo)reader.ReadByte());
			SerializationTypeInfo serializationTypeInfo = typeInfo.info & SerializationTypeInfo.VALUE_MASK;
			if (!Helper.IsGenericType(typeInfo.info))
			{
				switch (serializationTypeInfo)
				{
				case SerializationTypeInfo.UserDefined:
				case SerializationTypeInfo.Enumeration:
				{
					string type_name = reader.ReadKleiString();
					typeInfo.type = Manager.GetType(type_name);
					break;
				}
				case SerializationTypeInfo.Array:
					typeInfo.subTypes = new TypeInfo[1];
					typeInfo.subTypes[0] = ReadType(reader);
					if (typeInfo.subTypes[0].type != null)
					{
						typeInfo.type = typeInfo.subTypes[0].type.MakeArrayType();
					}
					else
					{
						typeInfo.type = null;
					}
					break;
				case SerializationTypeInfo.SByte:
					typeInfo.type = typeof(sbyte);
					break;
				case SerializationTypeInfo.Byte:
					typeInfo.type = typeof(byte);
					break;
				case SerializationTypeInfo.Boolean:
					typeInfo.type = typeof(bool);
					break;
				case SerializationTypeInfo.Int16:
					typeInfo.type = typeof(short);
					break;
				case SerializationTypeInfo.UInt16:
					typeInfo.type = typeof(ushort);
					break;
				case SerializationTypeInfo.Int32:
					typeInfo.type = typeof(int);
					break;
				case SerializationTypeInfo.UInt32:
					typeInfo.type = typeof(uint);
					break;
				case SerializationTypeInfo.Int64:
					typeInfo.type = typeof(long);
					break;
				case SerializationTypeInfo.UInt64:
					typeInfo.type = typeof(ulong);
					break;
				case SerializationTypeInfo.Single:
					typeInfo.type = typeof(float);
					break;
				case SerializationTypeInfo.Double:
					typeInfo.type = typeof(double);
					break;
				case SerializationTypeInfo.String:
					typeInfo.type = typeof(string);
					break;
				case SerializationTypeInfo.Vector2I:
					typeInfo.type = typeof(Vector2I);
					break;
				case SerializationTypeInfo.Vector2:
					typeInfo.type = typeof(Vector2);
					break;
				case SerializationTypeInfo.Vector3:
					typeInfo.type = typeof(Vector3);
					break;
				case SerializationTypeInfo.Colour:
					typeInfo.type = typeof(Color);
					break;
				default:
					throw new ArgumentException("unknown type");
				}
			}
			else
			{
				Type type = null;
				switch (serializationTypeInfo)
				{
				case SerializationTypeInfo.Dictionary:
					type = typeof(Dictionary<, >);
					break;
				case SerializationTypeInfo.HashSet:
					type = typeof(HashSet<>);
					break;
				case SerializationTypeInfo.List:
					type = typeof(List<>);
					break;
				case SerializationTypeInfo.Pair:
					type = typeof(KeyValuePair<, >);
					break;
				case SerializationTypeInfo.Queue:
					type = typeof(Queue<>);
					break;
				case SerializationTypeInfo.UserDefined:
				{
					string type_name2 = reader.ReadKleiString();
					typeInfo.type = Manager.GetType(type_name2);
					break;
				}
				default:
					throw new ArgumentException("unknown type");
				}
				byte b2 = reader.ReadByte();
				Type[] array = new Type[b2];
				typeInfo.subTypes = new TypeInfo[b2];
				for (int i = 0; i < b2; i++)
				{
					typeInfo.subTypes[i] = ReadType(reader);
					array[i] = typeInfo.subTypes[i].type;
				}
				if (type != null)
				{
					if (array == null || Array.IndexOf(array, null) != -1)
					{
						typeInfo.type = null;
						return typeInfo;
					}
					typeInfo.type = type.MakeGenericType(array);
				}
				else if (typeInfo.type != null)
				{
					Type[] genericArguments = typeInfo.type.GetGenericArguments();
					if (genericArguments.Length != b2)
					{
						throw new InvalidOperationException("User defined generic type mismatch");
					}
					for (int j = 0; j < b2; j++)
					{
						if (array[j] != genericArguments[j])
						{
							throw new InvalidOperationException("User defined generic type mismatch");
						}
					}
				}
			}
			return typeInfo;
		}
	}
}
