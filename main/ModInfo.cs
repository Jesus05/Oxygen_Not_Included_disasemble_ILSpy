using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

[Serializable]
public struct ModInfo
{
	public enum Source
	{
		Local,
		Steam,
		Rail
	}

	public enum ModType
	{
		WorldGen,
		Scenario,
		Mod
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public Source source;

	[JsonConverter(typeof(StringEnumConverter))]
	public ModType type;

	public string assetID;

	public string assetPath;

	public bool enabled;

	public bool markedForDelete;

	public bool markedForUpdate;

	public string description;

	public ulong lastModifiedTime;
}
