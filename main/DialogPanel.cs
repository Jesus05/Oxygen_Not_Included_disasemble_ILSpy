using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPanel : MonoBehaviour, IDeselectHandler, IEventSystemHandler
{
	public bool destroyOnDeselect = true;

	public void OnDeselect(BaseEventData eventData)
	{
		if (destroyOnDeselect)
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					Util.KDestroyGameObject(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
		base.gameObject.SetActive(false);
	}
}
