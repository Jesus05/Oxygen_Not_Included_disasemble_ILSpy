using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : KMonoBehaviour
{
	private enum BoundaryTag : uint
	{
		Component = 3735928559u,
		Prefab = 3131961357u,
		Complete = 3735929054u
	}

	public const int SAVE_MAJOR_VERSION_LAST_UNDOCUMENTED = 7;

	public const int SAVE_MAJOR_VERSION = 7;

	public const int SAVE_MINOR_VERSION_EXPLICIT_VALUE_TYPES = 4;

	public const int SAVE_MINOR_VERSION_LAST_UNDOCUMENTED = 7;

	public const int SAVE_MINOR_VERSION_MOD_IDENTIFIER = 8;

	public const int SAVE_MINOR_VERSION = 8;

	private Dictionary<Tag, GameObject> prefabMap = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, List<SaveLoadRoot>> sceneObjects = new Dictionary<Tag, List<SaveLoadRoot>>();

	private static readonly char[] SAVE_HEADER = new char[4]
	{
		'K',
		'S',
		'A',
		'V'
	};

	private List<Tag> orderedKeys = new List<Tag>();

	public event Action<SaveLoadRoot> onRegister;

	public event Action<SaveLoadRoot> onUnregister;

	protected override void OnPrefabInit()
	{
		Assets.RegisterOnAddPrefab(OnAddPrefab);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Assets.UnregisterOnAddPrefab(OnAddPrefab);
	}

	private void OnAddPrefab(KPrefabID prefab)
	{
		if (!((UnityEngine.Object)prefab == (UnityEngine.Object)null))
		{
			Tag saveLoadTag = prefab.GetSaveLoadTag();
			prefabMap[saveLoadTag] = prefab.gameObject;
		}
	}

	public Dictionary<Tag, List<SaveLoadRoot>> GetLists()
	{
		return sceneObjects;
	}

	private List<SaveLoadRoot> GetSaveLoadRootList(SaveLoadRoot saver)
	{
		KPrefabID component = saver.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			DebugUtil.LogErrorArgs(saver.gameObject, "All savers must also have a KPrefabID on them but", saver.gameObject.name, "does not have one.");
			return null;
		}
		if (!sceneObjects.TryGetValue(component.GetSaveLoadTag(), out List<SaveLoadRoot> value))
		{
			value = new List<SaveLoadRoot>();
			sceneObjects[component.GetSaveLoadTag()] = value;
		}
		return value;
	}

	public void Register(SaveLoadRoot root)
	{
		List<SaveLoadRoot> saveLoadRootList = GetSaveLoadRootList(root);
		if (saveLoadRootList != null)
		{
			saveLoadRootList.Add(root);
			if (this.onRegister != null)
			{
				this.onRegister(root);
			}
		}
	}

	public void Unregister(SaveLoadRoot root)
	{
		if (this.onRegister != null)
		{
			this.onUnregister(root);
		}
		GetSaveLoadRootList(root)?.Remove(root);
	}

	public GameObject GetPrefab(Tag tag)
	{
		GameObject value = null;
		if (prefabMap.TryGetValue(tag, out value))
		{
			return value;
		}
		DebugUtil.LogArgs("Item not found in prefabMap", "[" + tag.Name + "]");
		return null;
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(SAVE_HEADER);
		writer.Write(7);
		writer.Write(8);
		int num = 0;
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> sceneObject in sceneObjects)
		{
			if (sceneObject.Value.Count > 0)
			{
				num++;
			}
		}
		writer.Write(num);
		orderedKeys.Clear();
		orderedKeys.AddRange(sceneObjects.Keys);
		orderedKeys.Remove(SaveGame.Instance.PrefabID());
		orderedKeys = (from a in orderedKeys
		orderby a.Name.Contains("UnderConstruction")
		select a).ToList();
		Write(SaveGame.Instance.PrefabID(), new List<SaveLoadRoot>(new SaveLoadRoot[1]
		{
			SaveGame.Instance.GetComponent<SaveLoadRoot>()
		}), writer);
		foreach (Tag orderedKey in orderedKeys)
		{
			List<SaveLoadRoot> list = sceneObjects[orderedKey];
			int count = list.Count;
			if (count > 0)
			{
				foreach (SaveLoadRoot item in list)
				{
					if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && (UnityEngine.Object)item.GetComponent<SimCellOccupier>() != (UnityEngine.Object)null)
					{
						Write(orderedKey, list, writer);
						break;
					}
				}
			}
		}
		foreach (Tag orderedKey2 in orderedKeys)
		{
			List<SaveLoadRoot> list2 = sceneObjects[orderedKey2];
			int count2 = list2.Count;
			if (count2 > 0)
			{
				foreach (SaveLoadRoot item2 in list2)
				{
					if (!((UnityEngine.Object)item2 == (UnityEngine.Object)null) && (UnityEngine.Object)item2.GetComponent<SimCellOccupier>() == (UnityEngine.Object)null)
					{
						Write(orderedKey2, list2, writer);
						break;
					}
				}
			}
		}
	}

	private void Write(Tag key, List<SaveLoadRoot> value, BinaryWriter writer)
	{
		int count = value.Count;
		Tag tag = key;
		writer.WriteKleiString(tag.Name);
		writer.Write(count);
		long position = writer.BaseStream.Position;
		int value2 = -1;
		writer.Write(value2);
		long position2 = writer.BaseStream.Position;
		foreach (SaveLoadRoot item in value)
		{
			if ((UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				item.Save(writer);
			}
			else
			{
				DebugUtil.LogWarningArgs("Null game object when saving");
			}
		}
		long position3 = writer.BaseStream.Position;
		long num = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num);
		writer.BaseStream.Position = position3;
	}

	public bool Load(IReader reader)
	{
		char[] array = reader.ReadChars(SAVE_HEADER.Length);
		if (array == null || array.Length != SAVE_HEADER.Length)
		{
			return false;
		}
		for (int i = 0; i < SAVE_HEADER.Length; i++)
		{
			if (array[i] != SAVE_HEADER[i])
			{
				return false;
			}
		}
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		if (num != 7 || num2 > 8)
		{
			DebugUtil.LogWarningArgs($"SAVE FILE VERSION MISMATCH! Expected {7}.{8} but got {num}.{num2}");
			return false;
		}
		ClearScene();
		try
		{
			int num3 = reader.ReadInt32();
			for (int j = 0; j < num3; j++)
			{
				string text = reader.ReadKleiString();
				int num4 = reader.ReadInt32();
				int num5 = 0;
				num5 = reader.ReadInt32();
				Tag key = TagManager.Create(text);
				if (!prefabMap.TryGetValue(key, out GameObject value))
				{
					DebugUtil.LogWarningArgs("Could not find prefab '" + text + "'");
					reader.SkipBytes(num5);
				}
				else
				{
					List<SaveLoadRoot> value2 = new List<SaveLoadRoot>(num4);
					sceneObjects[key] = value2;
					for (int k = 0; k < num4; k++)
					{
						SaveLoadRoot x = SaveLoadRoot.Load(value, reader);
						if ((UnityEngine.Object)x == (UnityEngine.Object)null)
						{
							Debug.LogError("Error loading data [" + text + "]");
							return false;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			DebugUtil.LogErrorArgs("Error deserializing prefabs\n\n", ex.ToString());
			throw ex;
		}
		return true;
	}

	private void ClearScene()
	{
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> sceneObject in sceneObjects)
		{
			foreach (SaveLoadRoot item in sceneObject.Value)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		sceneObjects.Clear();
	}
}
