using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

[Serializable]
public struct ModInfo : IEquatable<ModInfo>
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

	public ModInfo(Source source, ModType type, string asset_id, string description, string asset_path, ulong last_modified_time = 0uL)
	{
		this.source = source;
		this.type = type;
		assetID = asset_id;
		this.description = description;
		assetPath = asset_path;
		enabled = false;
		markedForDelete = false;
		markedForUpdate = false;
		lastModifiedTime = last_modified_time;
	}

	public bool Equals(ModInfo other)
	{
		return source == other.source && assetID == other.assetID;
	}

	public override int GetHashCode()
	{
		return source.GetHashCode() ^ assetID.GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (other is ModInfo)
		{
			ModInfo other2 = (ModInfo)other;
			return Equals(other2);
		}
		return false;
	}
}
