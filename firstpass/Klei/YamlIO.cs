using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;

namespace Klei
{
	public static class YamlIO
	{
		public struct Error
		{
			public enum Severity
			{
				Fatal,
				Recoverable
			}

			public FileHandle file;

			public string message;

			public Exception inner_exception;

			public string text;

			public Severity severity;
		}

		public delegate void ErrorHandler(Error error, bool force_log_as_warning);

		private delegate void ErrorLogger(string format, params object[] args);

		private const bool verbose_errors = false;

		[CompilerGenerated]
		private static ErrorLogger _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static ErrorLogger _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ErrorHandler _003C_003Ef__mg_0024cache2;

		public static void Save<T>(T some_object, string filename, List<Tuple<string, Type>> tagMappings = null)
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
				serializer.Serialize(writer, some_object);
			}
		}

		public static T LoadFile<T>(string filename, ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			return Parse<T>(FileSystem.ConvertToText(FileSystem.ReadBytes(filename)), filename, handle_error, tagMappings);
		}

		public static void LogError(Error error, bool force_log_as_warning)
		{
			ErrorLogger errorLogger = (!force_log_as_warning && error.severity != Error.Severity.Recoverable) ? new ErrorLogger(Debug.LogErrorFormat) : new ErrorLogger(Debug.LogWarningFormat);
			if (error.inner_exception == null)
			{
				errorLogger("{0} parse error in {1}\n{2}", error.severity, error.file.full_path, error.message);
			}
			else
			{
				errorLogger("{0} parse error in {1}\n{2}\n{3}", error.severity, error.file.full_path, error.message, error.inner_exception.Message);
			}
		}

		public static T Parse<T>(string readText, string debugFilename, ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			try
			{
				if (handle_error == null)
				{
					handle_error = LogError;
				}
				readText = readText.Replace("\t", "    ");
				Action<string> unmatchedLogFn = delegate(string error)
				{
					handle_error(new Error
					{
						file = new FileHandle
						{
							full_path = debugFilename
						},
						text = readText,
						message = error,
						severity = Error.Severity.Recoverable
					}, false);
				};
				DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
				deserializerBuilder.IgnoreUnmatchedProperties(unmatchedLogFn);
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tagMapping in tagMappings)
					{
						deserializerBuilder = deserializerBuilder.WithTagMapping(tagMapping.first, tagMapping.second);
					}
				}
				Deserializer deserializer = deserializerBuilder.Build();
				StringReader input = new StringReader(readText);
				return deserializer.Deserialize<T>(input);
			}
			catch (Exception ex)
			{
				handle_error(new Error
				{
					file = new FileHandle
					{
						full_path = debugFilename
					},
					text = readText,
					message = ex.Message,
					inner_exception = ex.InnerException,
					severity = Error.Severity.Fatal
				}, false);
			}
			return default(T);
		}
	}
}
