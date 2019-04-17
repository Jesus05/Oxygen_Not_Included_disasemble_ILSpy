using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Klei
{
	public class YamlIO<T>
	{
		public void Save(string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				SerializerBuilder serializerBuilder = new SerializerBuilder();
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tagMapping in tagMappings)
					{
						serializerBuilder = serializerBuilder.WithTagMapping(tagMapping.first, tagMapping.second);
					}
				}
				Serializer serializer = serializerBuilder.Build();
				serializer.Serialize(writer, this);
			}
		}

		public static T LoadFile(string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			string readText = (LayeredFileSystem.instance == null) ? File.ReadAllText(filename) : LayeredFileSystem.instance.ReadText(filename);
			T val = Parse(readText, tagMappings);
			if (val == null)
			{
				Debug.LogWarning("Exception while loading yaml file [" + filename + "]");
			}
			return val;
		}

		public static T Parse(string readText, List<Tuple<string, Type>> tagMappings = null)
		{
			try
			{
				readText = readText.Replace("\t", "    ");
				DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
				deserializerBuilder.IgnoreUnmatchedProperties();
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tagMapping in tagMappings)
					{
						deserializerBuilder = deserializerBuilder.WithTagMapping(tagMapping.first, tagMapping.second);
					}
				}
				Deserializer deserializer = deserializerBuilder.Build();
				StringReader input = new StringReader(readText);
				return deserializer.Deserialize<T>((TextReader)input);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				DebugUtil.DevLogError("Exception while loading yaml data: " + message + "\n YAML FILE:\n" + readText);
			}
			return default(T);
		}
	}
}
