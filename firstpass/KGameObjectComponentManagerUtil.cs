using KSerialization;
using System.IO;
using UnityEngine;

public static class KGameObjectComponentManagerUtil
{
	public static void Serialize<MgrType, DataType>(MgrType mgr, GameObject go, BinaryWriter writer) where MgrType : KGameObjectComponentManager<DataType> where DataType : new()
	{
		long position = writer.BaseStream.Position;
		writer.Write(0);
		long position2 = writer.BaseStream.Position;
		HandleVector<int>.Handle handle = ((KGameObjectComponentManager<DataType>)mgr).GetHandle(go);
		DataType data = ((KCompactedVector<DataType>)mgr).GetData(handle);
		Serializer.SerializeTypeless(data, writer);
		long position3 = writer.BaseStream.Position;
		long num = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num);
		writer.BaseStream.Position = position3;
	}

	public static void Deserialize<MgrType, DataType>(MgrType mgr, GameObject go, IReader reader) where MgrType : KGameObjectComponentManager<DataType> where DataType : new()
	{
		HandleVector<int>.Handle handle = ((KGameObjectComponentManager<DataType>)mgr).GetHandle(go);
		Deserializer.Deserialize(typeof(DataType), reader, out object result);
		((KCompactedVector<DataType>)mgr).SetData(handle, (DataType)result);
	}

	public static void Serialize<MgrType, Header, Payload>(MgrType mgr, GameObject go, BinaryWriter writer) where MgrType : KGameObjectSplitComponentManager<Header, Payload> where Header : new()where Payload : new()
	{
		long position = writer.BaseStream.Position;
		writer.Write(0);
		long position2 = writer.BaseStream.Position;
		HandleVector<int>.Handle handle = ((KGameObjectSplitComponentManager<Header, Payload>)mgr).GetHandle(go);
		((KSplitCompactedVector<Header, Payload>)mgr).GetData(handle, out Header header, out Payload payload);
		Serializer.SerializeTypeless(header, writer);
		Serializer.SerializeTypeless(payload, writer);
		long position3 = writer.BaseStream.Position;
		long num = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num);
		writer.BaseStream.Position = position3;
	}

	public static void Deserialize<MgrType, Header, Payload>(MgrType mgr, GameObject go, IReader reader) where MgrType : KGameObjectSplitComponentManager<Header, Payload> where Header : new()where Payload : new()
	{
		HandleVector<int>.Handle handle = ((KGameObjectSplitComponentManager<Header, Payload>)mgr).GetHandle(go);
		Deserializer.Deserialize(typeof(Header), reader, out object result);
		Deserializer.Deserialize(typeof(Payload), reader, out object result2);
		Payload new_data = (Payload)result2;
		((KSplitCompactedVector<Header, Payload>)mgr).SetData(handle, (Header)result, ref new_data);
	}
}
