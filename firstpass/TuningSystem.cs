using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class TuningSystem
{
	private static JsonSerializerSettings _SerializationSettings;

	private static Dictionary<Type, object> _TuningValues;

	private static string _TuningPath;

	static TuningSystem()
	{
		_SerializationSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented
		};
		_TuningPath = Path.Combine(Application.streamingAssetsPath, "Tuning.json");
		InitializeTuning();
		ListenForFileChanges();
		Load();
	}

	public static void Init()
	{
	}

	private static void ListenForFileChanges()
	{
		string directoryName = Path.GetDirectoryName(_TuningPath);
		try
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
			fileSystemWatcher.Changed += OnFileChanged;
			fileSystemWatcher.Path = directoryName;
			fileSystemWatcher.Filter = Path.GetFileName(_TuningPath);
			fileSystemWatcher.EnableRaisingEvents = true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Error when attempting to monitor path: " + directoryName + "\n" + ex.ToString());
		}
	}

	private static void OnFileChanged(object source, FileSystemEventArgs e)
	{
		Load();
	}

	private static void InitializeTuning()
	{
		_TuningValues = new Dictionary<Type, object>();
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			Type baseType = currentDomainType.BaseType;
			if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(TuningData<>))
			{
				Type type = baseType.GetGenericArguments()[0];
				object obj = Activator.CreateInstance(type);
				_TuningValues[obj.GetType()] = obj;
				FieldInfo field = baseType.GetField("_TuningData", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				field.SetValue(null, obj);
			}
		}
	}

	private static void Save()
	{
		if (_TuningValues != null)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(_SerializationSettings);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<Type, object> tuningValue in _TuningValues)
			{
				dictionary[tuningValue.Value.GetType().FullName] = tuningValue.Value;
			}
			using (StreamWriter textWriter = File.CreateText(_TuningPath))
			{
				jsonSerializer.Serialize(textWriter, dictionary);
			}
		}
	}

	private static void Load()
	{
		string[] array = new string[2]
		{
			_TuningPath,
			string.Empty
		};
		if (Thread.CurrentThread == KProfiler.main_thread)
		{
			array[1] = Path.Combine(Application.dataPath, "Tuning.json");
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text) && FileSystem.FileExists(text))
			{
				string value = FileSystem.ConvertToText(FileSystem.ReadBytes(text));
				Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					Type type = null;
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly assembly in assemblies)
					{
						type = assembly.GetType(item.Key);
						if (type != null)
						{
							break;
						}
					}
					if (type != null)
					{
						if (_TuningValues.ContainsKey(type))
						{
							JsonConvert.PopulateObject(item.Value.ToString(), _TuningValues[type]);
						}
						else
						{
							object value2 = JsonConvert.DeserializeObject(item.Value.ToString(), type);
							_TuningValues[type] = value2;
						}
					}
				}
			}
		}
	}

	private static bool IsLoaded()
	{
		return _TuningValues != null;
	}

	public static Dictionary<Type, object> GetAllTuningValues()
	{
		return _TuningValues;
	}
}
