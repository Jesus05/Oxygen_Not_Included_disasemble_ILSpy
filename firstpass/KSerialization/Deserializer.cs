#define DEBUG_LOG
using System;

namespace KSerialization
{
	public class Deserializer
	{
		public IReader reader;

		public Deserializer(IReader reader)
		{
			this.reader = reader;
		}

		public bool Deserialize(object obj)
		{
			return Deserialize(obj, reader);
		}

		public static bool Deserialize(object obj, IReader reader)
		{
			string b = reader.ReadKleiString();
			Type type = obj.GetType();
			string kTypeString = type.GetKTypeString();
			if (!(kTypeString == b))
			{
				return false;
			}
			return DeserializeTypeless(type, obj, reader);
		}

		public static bool DeserializeTypeless(Type type, object obj, IReader reader)
		{
			DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type);
			bool flag = false;
			try
			{
				return deserializationMapping.Deserialize(obj, reader);
			}
			catch (Exception ex)
			{
				string text = $"Exception occurred while attempting to deserialize object {obj}({obj.GetType()}).\n{ex.ToString()}";
				DebugLog.Output(DebugLog.Level.Error, text);
				throw new Exception(text, ex);
			}
		}

		public static bool DeserializeTypeless(object obj, IReader reader)
		{
			Type type = obj.GetType();
			DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type);
			try
			{
				return deserializationMapping.Deserialize(obj, reader);
			}
			catch (Exception ex)
			{
				string text = $"Exception occurred while attempting to deserialize object {obj}({obj.GetType()}).\n{ex.ToString()}";
				DebugLog.Output(DebugLog.Level.Error, text);
				throw new Exception(text, ex);
			}
		}

		public static bool Deserialize(Type type, IReader reader, out object result)
		{
			DeserializationMapping deserializationMapping = Manager.GetDeserializationMapping(type);
			bool result2;
			try
			{
				object obj = Activator.CreateInstance(type);
				result2 = deserializationMapping.Deserialize(obj, reader);
				result = obj;
			}
			catch (Exception ex)
			{
				string text = $"Exception occurred while attempting to deserialize into object of type {type.ToString()}.\n{ex.ToString()}";
				DebugLog.Output(DebugLog.Level.Error, text);
				throw new Exception(text, ex);
			}
			return result2;
		}
	}
}
