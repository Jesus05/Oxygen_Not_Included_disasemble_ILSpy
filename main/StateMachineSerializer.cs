using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;

public class StateMachineSerializer
{
	private class Entry
	{
		public int version;

		public int dataPos;

		public Type type;

		public string currentState;

		public Entry(int version, int data_pos, Type type, string current_state)
		{
			this.version = version;
			dataPos = data_pos;
			this.type = type;
			currentState = current_state;
		}

		public Entry(StateMachine.Instance smi, BinaryWriter entry_writer)
		{
			version = smi.GetStateMachine().version;
			dataPos = (int)entry_writer.BaseStream.Position;
			type = smi.GetType();
			currentState = smi.GetCurrentState().name;
			Serializer.SerializeTypeless(smi, entry_writer);
			StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
			entry_writer.Write(parameterContexts.Length);
			StateMachine.Parameter.Context[] array = parameterContexts;
			foreach (StateMachine.Parameter.Context context in array)
			{
				long position = (int)entry_writer.BaseStream.Position;
				entry_writer.Write(0);
				long num = (int)entry_writer.BaseStream.Position;
				entry_writer.WriteKleiString(context.GetType().FullName);
				entry_writer.WriteKleiString(context.parameter.name);
				context.Serialize(entry_writer);
				long num2 = (int)entry_writer.BaseStream.Position;
				entry_writer.BaseStream.Position = position;
				long num3 = num2 - num;
				entry_writer.Write((int)num3);
				entry_writer.BaseStream.Position = num2;
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(version);
			writer.Write(dataPos);
			writer.WriteKleiString(type.FullName);
			writer.WriteKleiString(currentState);
		}

		public static Entry Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			int data_pos = reader.ReadInt32();
			string typeName = reader.ReadKleiString();
			string current_state = reader.ReadKleiString();
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				return null;
			}
			return new Entry(num, data_pos, type, current_state);
		}
	}

	private static int serializerVersion = 10;

	private List<Entry> entries = new List<Entry>();

	private FastReader entryData;

	public void Serialize(List<StateMachine.Instance> state_machines, BinaryWriter writer)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter entry_writer = new BinaryWriter(memoryStream);
		List<Entry> serialized_entries = CreateEntries(state_machines, entry_writer);
		long data_size_pos = WriteHeader(writer);
		long position = writer.BaseStream.Position;
		WriteEntries(serialized_entries, writer);
		try
		{
			WriteEntryData(memoryStream, writer);
		}
		catch (Exception obj)
		{
			Debug.Log("Stream size: " + memoryStream.Length, null);
			Debug.Log("StateMachines: ", null);
			foreach (StateMachine.Instance state_machine in state_machines)
			{
				Debug.Log(state_machine.ToString(), null);
			}
			Debug.LogError(obj, null);
		}
		WriteDataSize(position, data_size_pos, writer);
	}

	public void Deserialize(IReader reader)
	{
		if (ReadHeader(reader))
		{
			entries = ReadEntries(reader);
			entryData = ReadEntryData(reader);
		}
	}

	private List<Entry> CreateEntries(List<StateMachine.Instance> state_machines, BinaryWriter entry_writer)
	{
		List<Entry> list = new List<Entry>();
		foreach (StateMachine.Instance state_machine in state_machines)
		{
			if (state_machine.IsRunning())
			{
				Entry item = new Entry(state_machine, entry_writer);
				list.Add(item);
			}
		}
		return list;
	}

	private void WriteEntryData(MemoryStream stream, BinaryWriter writer)
	{
		writer.Write((int)stream.Length);
		writer.Write(stream.ToArray());
	}

	private FastReader ReadEntryData(IReader reader)
	{
		int length = reader.ReadInt32();
		byte[] bytes = reader.ReadBytes(length);
		return new FastReader(bytes);
	}

	private void WriteDataSize(long data_start_pos, long data_size_pos, BinaryWriter writer)
	{
		long position = writer.BaseStream.Position;
		long num = position - data_start_pos;
		writer.BaseStream.Position = data_size_pos;
		writer.Write((int)num);
		writer.BaseStream.Position = position;
	}

	private long WriteHeader(BinaryWriter writer)
	{
		int value = 0;
		writer.Write(serializerVersion);
		long position = writer.BaseStream.Position;
		writer.Write(value);
		return position;
	}

	private bool ReadHeader(IReader reader)
	{
		int num = reader.ReadInt32();
		int length = reader.ReadInt32();
		if (num != serializerVersion)
		{
			Debug.LogWarning("State machine serializer version mismatch: " + num + "!=" + serializerVersion + "\nDiscarding data.", null);
			reader.SkipBytes(length);
			return false;
		}
		return true;
	}

	private void WriteEntries(List<Entry> serialized_entries, BinaryWriter writer)
	{
		writer.Write(serialized_entries.Count);
		for (int i = 0; i < serialized_entries.Count; i++)
		{
			serialized_entries[i].Serialize(writer);
		}
	}

	private List<Entry> ReadEntries(IReader reader)
	{
		List<Entry> list = new List<Entry>();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Entry entry = Entry.Deserialize(reader);
			if (entry != null)
			{
				list.Add(entry);
			}
		}
		return list;
	}

	private static string TrimAssemblyInfo(string type_name)
	{
		int num = type_name.IndexOf("[[");
		if (num != -1)
		{
			return type_name.Substring(0, num);
		}
		return type_name;
	}

	private bool Restore(Entry entry, StateMachine.Instance smi)
	{
		if (entry.version != smi.GetStateMachine().version)
		{
			return false;
		}
		entryData.Position = entry.dataPos;
		if (Manager.HasDeserializationMapping(smi.GetType()))
		{
			Deserializer.DeserializeTypeless(smi, entryData);
		}
		if (!smi.GetStateMachine().serializable)
		{
			return false;
		}
		StateMachine.BaseState state = smi.GetStateMachine().GetState(entry.currentState);
		if (state == null)
		{
			return false;
		}
		StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
		int num = entryData.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = entryData.ReadInt32();
			int position = entryData.Position;
			string text = entryData.ReadKleiString();
			text = text.Replace("Version=4.0.0.0", "Version=2.0.0.0");
			string b = entryData.ReadKleiString();
			StateMachine.Parameter.Context[] array = parameterContexts;
			foreach (StateMachine.Parameter.Context context in array)
			{
				if (context.parameter.name == b && context.GetType().FullName == text)
				{
					context.Deserialize(entryData);
					break;
				}
			}
			entryData.SkipBytes(num2 - (entryData.Position - position));
		}
		smi.GoTo(state);
		return true;
	}

	public bool Restore(StateMachine.Instance instance)
	{
		if (entryData == null)
		{
			return false;
		}
		Type type = instance.GetType();
		for (int i = 0; i < entries.Count; i++)
		{
			Entry entry = entries[i];
			if (entry.type == type)
			{
				entries.RemoveAt(i);
				return Restore(entry, instance);
			}
		}
		return false;
	}
}
