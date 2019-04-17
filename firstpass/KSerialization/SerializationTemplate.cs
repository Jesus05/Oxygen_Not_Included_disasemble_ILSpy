using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace KSerialization
{
	public class SerializationTemplate
	{
		public struct SerializationField
		{
			public FieldInfo field;

			public TypeInfo typeInfo;
		}

		public struct SerializationProperty
		{
			public PropertyInfo property;

			public TypeInfo typeInfo;
		}

		public Type serializableType;

		public TypeInfo typeInfo;

		public List<SerializationField> serializableFields = new List<SerializationField>();

		public List<SerializationProperty> serializableProperties = new List<SerializationProperty>();

		public MethodInfo onSerializing;

		public MethodInfo onSerialized;

		public SerializationTemplate(Type type)
		{
			serializableType = type;
			typeInfo = Manager.GetTypeInfo(type);
			type.GetSerializationMethods(typeof(OnSerializingAttribute), typeof(OnSerializedAttribute), out onSerializing, out onSerialized);
			switch (GetSerializationConfig(type))
			{
			default:
				return;
			case MemberSerialization.OptOut:
				while (type != typeof(object))
				{
					AddPublicFields(type);
					AddPublicProperties(type);
					type = type.BaseType;
				}
				return;
			case MemberSerialization.OptIn:
				break;
			}
			while (type != typeof(object))
			{
				AddOptInFields(type);
				AddOptInProperties(type);
				type = type.BaseType;
			}
		}

		private MemberSerialization GetSerializationConfig(Type type)
		{
			MemberSerialization memberSerialization = MemberSerialization.Invalid;
			Type type2 = null;
			while (type != typeof(object))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(SerializationConfig), false);
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					Attribute attribute = (Attribute)array[i];
					if (attribute is SerializationConfig)
					{
						SerializationConfig serializationConfig = attribute as SerializationConfig;
						if (serializationConfig.MemberSerialization == memberSerialization || memberSerialization == MemberSerialization.Invalid)
						{
							memberSerialization = serializationConfig.MemberSerialization;
							type2 = type.BaseType;
							break;
						}
						string text = "Found conflicting serialization configurations on type " + type2.ToString() + " and " + type.ToString();
						Debug.LogError(text);
						throw new ArgumentException(text);
					}
				}
				type = type.BaseType;
			}
			if (memberSerialization == MemberSerialization.Invalid)
			{
				memberSerialization = MemberSerialization.OptOut;
			}
			return memberSerialization;
		}

		public override string ToString()
		{
			string text = "Template: " + serializableType.ToString() + "\n";
			foreach (SerializationField serializableField in serializableFields)
			{
				text = text + "\t" + serializableField.ToString() + "\n";
			}
			return text;
		}

		private void AddPublicFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] array = fields;
			foreach (FieldInfo field in array)
			{
				AddValidField(field);
			}
		}

		private void AddOptInFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(false);
				object[] array2 = customAttributes;
				foreach (object obj in array2)
				{
					if (obj != null && obj is Serialize)
					{
						AddValidField(fieldInfo);
					}
				}
			}
		}

		private void AddValidField(FieldInfo field)
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(NonSerializedAttribute), false);
			if (customAttributes == null || customAttributes.Length <= 0)
			{
				serializableFields.Add(new SerializationField
				{
					field = field,
					typeInfo = Manager.GetTypeInfo(field.FieldType)
				});
			}
		}

		private void AddPublicProperties(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo property in array)
			{
				AddValidProperty(property);
			}
		}

		private void AddOptInProperties(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(false);
				object[] array2 = customAttributes;
				foreach (object obj in array2)
				{
					if (obj != null && obj is Serialize)
					{
						AddValidProperty(propertyInfo);
					}
				}
			}
		}

		private void AddValidProperty(PropertyInfo property)
		{
			object[] customAttributes = property.GetCustomAttributes(typeof(NonSerializedAttribute), false);
			if (customAttributes == null || customAttributes.Length <= 0)
			{
				MethodInfo setMethod = property.GetSetMethod();
				if (setMethod != null)
				{
					serializableProperties.Add(new SerializationProperty
					{
						property = property,
						typeInfo = Manager.GetTypeInfo(property.PropertyType)
					});
				}
			}
		}

		public void SerializeTemplate(BinaryWriter writer)
		{
			writer.Write(serializableFields.Count);
			writer.Write(serializableProperties.Count);
			foreach (SerializationField serializableField in serializableFields)
			{
				SerializationField current = serializableField;
				writer.WriteKleiString(current.field.Name);
				Type fieldType = current.field.FieldType;
				WriteType(writer, fieldType);
			}
			foreach (SerializationProperty serializableProperty in serializableProperties)
			{
				SerializationProperty current2 = serializableProperty;
				writer.WriteKleiString(current2.property.Name);
				Type propertyType = current2.property.PropertyType;
				WriteType(writer, propertyType);
			}
		}

		private void WriteType(BinaryWriter writer, Type type)
		{
			SerializationTypeInfo serializationTypeInfo = Helper.EncodeSerializationType(type);
			writer.Write((byte)serializationTypeInfo);
			if (type.IsGenericType)
			{
				if (Helper.IsUserDefinedType(serializationTypeInfo))
				{
					writer.WriteKleiString(type.GetKTypeString());
				}
				Type[] genericArguments = type.GetGenericArguments();
				writer.Write((byte)genericArguments.Length);
				for (int i = 0; i < genericArguments.Length; i++)
				{
					WriteType(writer, genericArguments[i]);
				}
			}
			else if (Helper.IsArray(serializationTypeInfo))
			{
				Type elementType = type.GetElementType();
				WriteType(writer, elementType);
			}
			else if (type.IsEnum || Helper.IsUserDefinedType(serializationTypeInfo))
			{
				writer.WriteKleiString(type.GetKTypeString());
			}
		}

		public void SerializeData(object obj, BinaryWriter writer)
		{
			if (onSerializing != null)
			{
				onSerializing.Invoke(obj, null);
			}
			foreach (SerializationField serializableField in serializableFields)
			{
				SerializationField current = serializableField;
				try
				{
					object value = current.field.GetValue(obj);
					writer.WriteValue(current.typeInfo, value);
				}
				catch (Exception innerException)
				{
					string text = $"Error occurred while serializing field {current.field.Name} on template {serializableType.Name}";
					Debug.LogError(text);
					throw new ArgumentException(text, innerException);
				}
			}
			foreach (SerializationProperty serializableProperty in serializableProperties)
			{
				SerializationProperty current2 = serializableProperty;
				try
				{
					object value2 = current2.property.GetValue(obj, null);
					writer.WriteValue(current2.typeInfo, value2);
				}
				catch (Exception innerException2)
				{
					string text2 = $"Error occurred while serializing property {current2.property.Name} on template {serializableType.Name}";
					Debug.LogError(text2);
					throw new ArgumentException(text2, innerException2);
				}
			}
			if (onSerialized != null)
			{
				onSerialized.Invoke(obj, null);
			}
		}
	}
}
