using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityConfigManager : KMonoBehaviour
{
	private struct ConfigEntry
	{
		public Type type;

		public int sortOrder;
	}

	public static EntityConfigManager Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	private static int GetSortOrder(Type type)
	{
		object[] customAttributes = type.GetCustomAttributes(true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			Attribute attribute = (Attribute)customAttributes[i];
			if (attribute.GetType() == typeof(EntityConfigOrder))
			{
				return (attribute as EntityConfigOrder).sortOrder;
			}
		}
		return 0;
	}

	public void LoadGeneratedEntities(List<Type> types)
	{
		Type typeFromHandle = typeof(IEntityConfig);
		Type typeFromHandle2 = typeof(IMultiEntityConfig);
		List<ConfigEntry> list = new List<ConfigEntry>();
		foreach (Type type in types)
		{
			if ((typeFromHandle.IsAssignableFrom(type) || typeFromHandle2.IsAssignableFrom(type)) && !type.IsAbstract && !type.IsInterface)
			{
				int sortOrder = GetSortOrder(type);
				ConfigEntry configEntry = default(ConfigEntry);
				configEntry.type = type;
				configEntry.sortOrder = sortOrder;
				ConfigEntry item = configEntry;
				list.Add(item);
			}
		}
		list.Sort((ConfigEntry x, ConfigEntry y) => x.sortOrder.CompareTo(y.sortOrder));
		foreach (ConfigEntry item2 in list)
		{
			ConfigEntry current2 = item2;
			object obj = Activator.CreateInstance(current2.type);
			if (obj is IEntityConfig)
			{
				RegisterEntity(obj as IEntityConfig);
			}
			if (obj is IMultiEntityConfig)
			{
				RegisterEntities(obj as IMultiEntityConfig);
			}
		}
	}

	public void RegisterEntity(IEntityConfig config)
	{
		GameObject gameObject = config.CreatePrefab();
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += config.OnPrefabInit;
		component.prefabSpawnFn += config.OnSpawn;
		Assets.AddPrefab(component);
	}

	public void RegisterEntities(IMultiEntityConfig config)
	{
		List<GameObject> list = config.CreatePrefabs();
		foreach (GameObject item in list)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			component.prefabInitFn += config.OnPrefabInit;
			component.prefabSpawnFn += config.OnSpawn;
			Assets.AddPrefab(component);
		}
	}
}
