using System;
using UnityEngine;

public interface IStateMachineTarget
{
	GameObject gameObject
	{
		get;
	}

	Transform transform
	{
		get;
	}

	string name
	{
		get;
	}

	bool isNull
	{
		get;
	}

	int Subscribe(int hash, Action<object> handler);

	void Unsubscribe(int hash, Action<object> handler);

	void Unsubscribe(int id);

	void Trigger(int hash, object data = null);

	ComponentType GetComponent<ComponentType>();
}
