using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KUISelectable : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private GameObject target;

	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((Object)target != (Object)null)
		{
			SelectTool.Instance.SetHoverOverride(target.GetComponent<KSelectable>());
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SelectTool.Instance.SetHoverOverride(null);
	}

	private void OnClick()
	{
		if ((Object)target != (Object)null)
		{
			SelectTool.Instance.Select(target.GetComponent<KSelectable>(), false);
		}
	}

	protected override void OnCmpDisable()
	{
		if ((Object)SelectTool.Instance != (Object)null)
		{
			SelectTool.Instance.SetHoverOverride(null);
		}
	}
}
