using UnityEngine;
using UnityEngine.EventSystems;

public class SelectablePanel : MonoBehaviour, IDeselectHandler, IEventSystemHandler
{
	public void OnDeselect(BaseEventData evt)
	{
		base.gameObject.SetActive(false);
	}
}
