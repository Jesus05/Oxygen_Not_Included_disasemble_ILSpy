using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : KGameObjectComponentManager<WhiteBoard.Data>, IKComponentManager
{
	public struct Data
	{
		public Dictionary<HashedString, object> keyValueStore;
	}

	public HandleVector<int>.Handle Add(GameObject go)
	{
		Data cmp = default(Data);
		cmp.keyValueStore = new Dictionary<HashedString, object>();
		return Add(go, cmp);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		Data data = GetData(h);
		data.keyValueStore.Clear();
		data.keyValueStore = null;
		SetData(h, data);
	}

	public bool HasValue(HandleVector<int>.Handle h, HashedString key)
	{
		if (!h.IsValid())
		{
			return false;
		}
		Data data = GetData(h);
		return data.keyValueStore.ContainsKey(key);
	}

	public object GetValue(HandleVector<int>.Handle h, HashedString key)
	{
		Data data = GetData(h);
		return data.keyValueStore[key];
	}

	public void SetValue(HandleVector<int>.Handle h, HashedString key, object value)
	{
		if (h.IsValid())
		{
			Data data = GetData(h);
			data.keyValueStore[key] = value;
			SetData(h, data);
		}
	}

	public void RemoveValue(HandleVector<int>.Handle h, HashedString key)
	{
		if (h.IsValid())
		{
			Data data = GetData(h);
			data.keyValueStore.Remove(key);
			SetData(h, data);
		}
	}
}
