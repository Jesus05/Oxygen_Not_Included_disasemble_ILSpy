using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class KMonoBehaviourExtensions
{
	public static int Subscribe(this GameObject go, int hash, Action<object> handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		return component.Subscribe(hash, handler);
	}

	public static void Subscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		component.Subscribe(target, hash, handler);
	}

	public static void Unsubscribe(this GameObject go, int hash, Action<object> handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.Unsubscribe(hash, handler);
		}
	}

	public static void Unsubscribe(this GameObject go, int id)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.Unsubscribe(id);
		}
	}

	public static void Unsubscribe(this GameObject go, GameObject target, int hash, Action<object> handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.Unsubscribe(target, hash, handler);
		}
	}

	public static T GetComponentInChildrenOnly<T>(this GameObject go) where T : Component
	{
		T[] componentsInChildren = go.GetComponentsInChildren<T>();
		T[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			T result = array[i];
			if ((UnityEngine.Object)result.gameObject != (UnityEngine.Object)go)
			{
				return result;
			}
		}
		return (T)null;
	}

	public static T[] GetComponentsInChildrenOnly<T>(this GameObject go) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange((IEnumerable<T>)go.GetComponentsInChildren<T>());
		list.RemoveAll((Predicate<T>)((T t) => (UnityEngine.Object)t.gameObject == (UnityEngine.Object)go));
		return list.ToArray();
	}

	public static void SetAlpha(this Image img, float alpha)
	{
		Color color = img.color;
		color.a = alpha;
		img.color = color;
	}

	public static void SetAlpha(this Text txt, float alpha)
	{
		Color color = txt.color;
		color.a = alpha;
		txt.color = color;
	}
}
