using UnityEngine;

public interface IKComponentManager
{
	HandleVector<int>.Handle Add(GameObject go);

	void Remove(GameObject go);
}
