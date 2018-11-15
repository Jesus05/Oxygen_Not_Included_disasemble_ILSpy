#define DEBUG_LOG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace KSerialization
{
	public class DeserializationMapping
	{
		private struct DeserializationInfo
		{
			public bool valid;

			public FieldInfo field;

			public PropertyInfo property;

			public TypeInfo typeInfo;
		}

		private DeserializationTemplate template;

		private List<DeserializationInfo> deserializationInfo = new List<DeserializationInfo>();

		public DeserializationMapping(DeserializationTemplate in_template, SerializationTemplate out_template)
		{
			template = in_template;
			foreach (DeserializationTemplate.SerializedInfo serializedMember in in_template.serializedMembers)
			{
				DeserializationTemplate.SerializedInfo current = serializedMember;
				DeserializationInfo item = new DeserializationInfo
				{
					valid = false
				};
				for (int i = 0; i < out_template.serializableFields.Count; i++)
				{
					SerializationTemplate.SerializationField serializationField = out_template.serializableFields[i];
					if (serializationField.field.Name == current.name)
					{
						SerializationTemplate.SerializationField serializationField2 = out_template.serializableFields[i];
						TypeInfo typeInfo = serializationField2.typeInfo;
						if (current.typeInfo.Equals(typeInfo))
						{
							SerializationTemplate.SerializationField serializationField3 = out_template.serializableFields[i];
							item.field = serializationField3.field;
							item.typeInfo = current.typeInfo;
							item.valid = true;
							break;
						}
					}
				}
				if (!item.valid)
				{
					for (int j = 0; j < out_template.serializableProperties.Count; j++)
					{
						SerializationTemplate.SerializationProperty serializationProperty = out_template.serializableProperties[j];
						if (serializationProperty.property.Name == current.name)
						{
							SerializationTemplate.SerializationProperty serializationProperty2 = out_template.serializableProperties[j];
							TypeInfo typeInfo2 = serializationProperty2.typeInfo;
							if (current.typeInfo.Equals(typeInfo2))
							{
								SerializationTemplate.SerializationProperty serializationProperty3 = out_template.serializableProperties[j];
								PropertyInfo propertyInfo = item.property = serializationProperty3.property;
								item.typeInfo = current.typeInfo;
								item.valid = true;
								break;
							}
						}
					}
				}
				item.valid = (item.valid && item.typeInfo.type != null);
				if (item.valid)
				{
					item.typeInfo.BuildGenericArgs();
				}
				else
				{
					item.typeInfo = current.typeInfo;
				}
				if (item.typeInfo.type == null)
				{
					DebugLog.Output(DebugLog.Level.Warning, $"Tried to deserialize field '{current.name}' on type {in_template.typeName} but it no longer exists");
				}
				deserializationInfo.Add(item);
			}
		}

		public bool Deserialize(object obj, IReader reader)
		{
			if (obj == null)
			{
				throw new ArgumentException("obj cannot be null");
			}
			if (template.onDeserializing != null)
			{
				template.onDeserializing.Invoke(obj, null);
			}
			foreach (DeserializationInfo item in deserializationInfo)
			{
				DeserializationInfo current = item;
				if (current.valid)
				{
					if (current.field != null)
					{
						try
						{
							object value = current.field.GetValue(obj);
							object value2 = ReadValue(current.typeInfo, reader, value);
							current.field.SetValue(obj, value2);
						}
						catch (Exception ex)
						{
							string text = $"Exception occurred while attempting to deserialize field {current.field}({current.field.FieldType}) on object {obj}({obj.GetType()}).\n{ex.ToString()}";
							DebugLog.Output(DebugLog.Level.Error, text);
							throw new Exception(text, ex);
						}
					}
					else
					{
						if (current.property == null)
						{
							throw new Exception("????");
						}
						try
						{
							object value3 = current.property.GetValue(obj, null);
							object value4 = ReadValue(current.typeInfo, reader, value3);
							current.property.SetValue(obj, value4, null);
						}
						catch (Exception ex2)
						{
							string text2 = $"Exception occurred while attempting to deserialize property {current.property}({current.property.PropertyType}) on object {obj}({obj.GetType()}).\n{ex2.ToString()}";
							DebugLog.Output(DebugLog.Level.Error, text2);
							throw new Exception(text2, ex2);
						}
					}
				}
				else
				{
					SerializationTypeInfo serializationTypeInfo = current.typeInfo.info & SerializationTypeInfo.VALUE_MASK;
					switch (serializationTypeInfo)
					{
					case SerializationTypeInfo.Array:
					case SerializationTypeInfo.Dictionary:
					case SerializationTypeInfo.List:
					case SerializationTypeInfo.HashSet:
					case SerializationTypeInfo.Queue:
					{
						int num2 = reader.ReadInt32();
						reader.ReadInt32();
						if (num2 > 0)
						{
							reader.SkipBytes(num2);
						}
						break;
					}
					case SerializationTypeInfo.UserDefined:
					case SerializationTypeInfo.Pair:
					{
						int num = reader.ReadInt32();
						if (num > 0)
						{
							reader.SkipBytes(num);
						}
						break;
					}
					default:
						SkipValue(serializationTypeInfo, reader);
						break;
					}
				}
			}
			if (template.onDeserialized != null)
			{
				template.onDeserialized.Invoke(obj, null);
			}
			return true;
		}

		private object ReadValue(TypeInfo type_info, IReader reader, object base_value)
		{
			object obj = null;
			SerializationTypeInfo serializationTypeInfo = type_info.info & SerializationTypeInfo.VALUE_MASK;
			Type type = type_info.type;
			switch (serializationTypeInfo)
			{
			case SerializationTypeInfo.Array:
			{
				reader.ReadInt32();
				int num4 = reader.ReadInt32();
				if (num4 >= 0)
				{
					obj = Activator.CreateInstance(type, num4);
					Array array3 = obj as Array;
					TypeInfo typeInfo3 = type_info.subTypes[0];
					if (Helper.IsPOD(typeInfo3.info))
					{
						ReadArrayFast(array3, typeInfo3, reader);
					}
					else if (Helper.IsValueType(typeInfo3.info))
					{
						DeserializationMapping deserializationMapping4 = Manager.GetDeserializationMapping(typeInfo3.type);
						object obj4 = Activator.CreateInstance(typeInfo3.type);
						for (int m = 0; m < num4; m++)
						{
							deserializationMapping4.Deserialize(obj4, reader);
							array3.SetValue(obj4, m);
						}
					}
					else
					{
						for (int n = 0; n < num4; n++)
						{
							object value4 = ReadValue(typeInfo3, reader, null);
							array3.SetValue(value4, n);
						}
					}
				}
				break;
			}
			case SerializationTypeInfo.List:
			{
				reader.ReadInt32();
				int num3 = reader.ReadInt32();
				if (num3 >= 0)
				{
					TypeInfo typeInfo2 = type_info.subTypes[0];
					Array array2 = Array.CreateInstance(typeInfo2.type, num3);
					if (Helper.IsPOD(typeInfo2.info))
					{
						ReadArrayFast(array2, typeInfo2, reader);
					}
					else if (Helper.IsValueType(typeInfo2.info))
					{
						DeserializationMapping deserializationMapping3 = Manager.GetDeserializationMapping(typeInfo2.type);
						object obj3 = Activator.CreateInstance(typeInfo2.type);
						for (int k = 0; k < num3; k++)
						{
							deserializationMapping3.Deserialize(obj3, reader);
							array2.SetValue(obj3, k);
						}
					}
					else
					{
						for (int l = 0; l < num3; l++)
						{
							object value3 = ReadValue(typeInfo2, reader, null);
							array2.SetValue(value3, l);
						}
					}
					obj = Activator.CreateInstance(type_info.genericInstantiationType, array2);
				}
				break;
			}
			case SerializationTypeInfo.HashSet:
			{
				reader.ReadInt32();
				int num5 = reader.ReadInt32();
				if (num5 >= 0)
				{
					TypeInfo typeInfo4 = type_info.subTypes[0];
					Type type3 = typeInfo4.type;
					Array array4 = Array.CreateInstance(type3, num5);
					if (Helper.IsValueType(typeInfo4.info))
					{
						DeserializationMapping deserializationMapping5 = Manager.GetDeserializationMapping(typeInfo4.type);
						object obj5 = Activator.CreateInstance(typeInfo4.type);
						for (int num6 = 0; num6 < num5; num6++)
						{
							deserializationMapping5.Deserialize(obj5, reader);
							array4.SetValue(obj5, num6);
						}
					}
					else
					{
						for (int num7 = 0; num7 < num5; num7++)
						{
							object value5 = ReadValue(typeInfo4, reader, null);
							array4.SetValue(value5, num7);
						}
					}
					obj = Activator.CreateInstance(type_info.genericInstantiationType, array4);
				}
				break;
			}
			case SerializationTypeInfo.Dictionary:
			{
				reader.ReadInt32();
				int num9 = reader.ReadInt32();
				if (num9 >= 0)
				{
					obj = Activator.CreateInstance(type_info.genericInstantiationType);
					IDictionary dictionary = obj as IDictionary;
					TypeInfo typeInfo5 = type_info.subTypes[1];
					Array array5 = Array.CreateInstance(typeInfo5.type, num9);
					for (int num10 = 0; num10 < num9; num10++)
					{
						object value6 = ReadValue(typeInfo5, reader, null);
						array5.SetValue(value6, num10);
					}
					TypeInfo typeInfo6 = type_info.subTypes[0];
					Array array6 = Array.CreateInstance(typeInfo6.type, num9);
					for (int num11 = 0; num11 < num9; num11++)
					{
						object value7 = ReadValue(typeInfo6, reader, null);
						array6.SetValue(value7, num11);
					}
					for (int num12 = 0; num12 < num9; num12++)
					{
						dictionary.Add(array6.GetValue(num12), array5.GetValue(num12));
					}
				}
				break;
			}
			case SerializationTypeInfo.Pair:
			{
				int num8 = reader.ReadInt32();
				if (num8 >= 0)
				{
					TypeInfo type_info2 = type_info.subTypes[0];
					TypeInfo type_info3 = type_info.subTypes[1];
					object obj6 = ReadValue(type_info2, reader, null);
					object obj7 = ReadValue(type_info3, reader, null);
					obj = Activator.CreateInstance(type_info.genericInstantiationType, obj6, obj7);
				}
				break;
			}
			case SerializationTypeInfo.Queue:
			{
				reader.ReadInt32();
				int num2 = reader.ReadInt32();
				if (num2 >= 0)
				{
					TypeInfo typeInfo = type_info.subTypes[0];
					Array array = Array.CreateInstance(typeInfo.type, num2);
					if (Helper.IsPOD(typeInfo.info))
					{
						ReadArrayFast(array, typeInfo, reader);
					}
					else if (Helper.IsValueType(typeInfo.info))
					{
						DeserializationMapping deserializationMapping2 = Manager.GetDeserializationMapping(typeInfo.type);
						object obj2 = Activator.CreateInstance(typeInfo.type);
						for (int i = 0; i < num2; i++)
						{
							deserializationMapping2.Deserialize(obj2, reader);
							array.SetValue(obj2, i);
						}
					}
					else
					{
						for (int j = 0; j < num2; j++)
						{
							object value2 = ReadValue(typeInfo, reader, null);
							array.SetValue(value2, j);
						}
					}
					obj = Activator.CreateInstance(type_info.genericInstantiationType, array);
				}
				break;
			}
			case SerializationTypeInfo.UserDefined:
			{
				int num = reader.ReadInt32();
				if (num >= 0)
				{
					Type type2 = type_info.type;
					if (base_value == null)
					{
						ConstructorInfo constructor = type2.GetConstructor(Type.EmptyTypes);
						obj = ((constructor == null) ? FormatterServices.GetUninitializedObject(type2) : Activator.CreateInstance(type2));
					}
					else
					{
						obj = base_value;
					}
					DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type2);
					deserializationMapping.Deserialize(obj, reader);
				}
				break;
			}
			case SerializationTypeInfo.Enumeration:
			{
				int value = reader.ReadInt32();
				obj = Enum.ToObject(type_info.type, value);
				break;
			}
			case SerializationTypeInfo.SByte:
				obj = reader.ReadSByte();
				break;
			case SerializationTypeInfo.Byte:
				obj = reader.ReadByte();
				break;
			case SerializationTypeInfo.Boolean:
				obj = (reader.ReadByte() == 1);
				break;
			case SerializationTypeInfo.Int16:
				obj = reader.ReadInt16();
				break;
			case SerializationTypeInfo.UInt16:
				obj = reader.ReadUInt16();
				break;
			case SerializationTypeInfo.Int32:
				obj = reader.ReadInt32();
				break;
			case SerializationTypeInfo.UInt32:
				obj = reader.ReadUInt32();
				break;
			case SerializationTypeInfo.Int64:
				obj = reader.ReadInt64();
				break;
			case SerializationTypeInfo.UInt64:
				obj = reader.ReadUInt64();
				break;
			case SerializationTypeInfo.Single:
				obj = reader.ReadSingle();
				break;
			case SerializationTypeInfo.Double:
				obj = reader.ReadDouble();
				break;
			case SerializationTypeInfo.String:
				obj = reader.ReadKleiString();
				break;
			case SerializationTypeInfo.Vector2I:
				obj = reader.ReadVector2I();
				break;
			case SerializationTypeInfo.Vector2:
				obj = reader.ReadVector2();
				break;
			case SerializationTypeInfo.Vector3:
				obj = reader.ReadVector3();
				break;
			case SerializationTypeInfo.Colour:
				obj = reader.ReadColour();
				break;
			default:
				throw new ArgumentException("unknown type");
			}
			return obj;
		}

		private void ReadArrayFast(Array dest_array, TypeInfo elem_type_info, IReader reader)
		{
			byte[] src = reader.RawBytes();
			int position = reader.Position;
			int length = dest_array.Length;
			int num;
			switch (elem_type_info.info)
			{
			case SerializationTypeInfo.SByte:
			case SerializationTypeInfo.Byte:
				num = length;
				break;
			case SerializationTypeInfo.Int16:
			case SerializationTypeInfo.UInt16:
				num = length * 2;
				break;
			case SerializationTypeInfo.Int32:
			case SerializationTypeInfo.UInt32:
			case SerializationTypeInfo.Single:
				num = length * 4;
				break;
			case SerializationTypeInfo.Int64:
			case SerializationTypeInfo.UInt64:
			case SerializationTypeInfo.Double:
				num = length * 8;
				break;
			default:
				throw new Exception("unknown pod type");
			}
			Buffer.BlockCopy(src, position, dest_array, 0, num);
			reader.SkipBytes(num);
		}

		private void SkipValue(SerializationTypeInfo type_info, IReader reader)
		{
			switch (type_info)
			{
			case SerializationTypeInfo.SByte:
			case SerializationTypeInfo.Byte:
			case SerializationTypeInfo.Boolean:
				reader.SkipBytes(1);
				break;
			case SerializationTypeInfo.Int16:
			case SerializationTypeInfo.UInt16:
				reader.SkipBytes(2);
				break;
			case SerializationTypeInfo.Int32:
			case SerializationTypeInfo.UInt32:
			case SerializationTypeInfo.Single:
			case SerializationTypeInfo.Enumeration:
				reader.SkipBytes(4);
				break;
			case SerializationTypeInfo.Int64:
			case SerializationTypeInfo.UInt64:
			case SerializationTypeInfo.Double:
			case SerializationTypeInfo.Vector2I:
			case SerializationTypeInfo.Vector2:
				reader.SkipBytes(8);
				break;
			case SerializationTypeInfo.String:
			{
				int num = reader.ReadInt32();
				if (num > 0)
				{
					reader.SkipBytes(num);
				}
				break;
			}
			case SerializationTypeInfo.Vector3:
				reader.SkipBytes(12);
				break;
			case SerializationTypeInfo.Colour:
				reader.SkipBytes(4);
				break;
			default:
				throw new ArgumentException("Unhandled type. Not sure how to skip by");
			}
		}
	}
}
