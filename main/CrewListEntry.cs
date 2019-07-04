using Klei.AI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrewListEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	protected MinionIdentity identity;

	protected CrewPortrait portrait;

	public CrewPortrait PortraitPrefab;

	public GameObject crewPortraitParent;

	protected bool mouseOver = false;

	public Image BorderHighlight;

	public Image BGImage;

	public float lastClickTime;

	public MinionIdentity Identity => identity;

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseOver = true;
		BGImage.enabled = true;
		BorderHighlight.color = new Color(0.65882355f, 0.2901961f, 0.4745098f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseOver = false;
		BGImage.enabled = false;
		BorderHighlight.color = new Color(0.8f, 0.8f, 0.8f);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool focus = Time.unscaledTime - lastClickTime < 0.3f;
		SelectCrewMember(focus);
		lastClickTime = Time.unscaledTime;
	}

	public virtual void Populate(MinionIdentity _identity)
	{
		identity = _identity;
		if ((Object)portrait == (Object)null)
		{
			GameObject parent = (!((Object)crewPortraitParent != (Object)null)) ? base.gameObject : crewPortraitParent;
			portrait = Util.KInstantiateUI<CrewPortrait>(PortraitPrefab.gameObject, parent, false);
			if ((Object)crewPortraitParent == (Object)null)
			{
				portrait.transform.SetSiblingIndex(2);
			}
		}
		portrait.SetIdentityObject(_identity, true);
	}

	public virtual void Refresh()
	{
	}

	public void RefreshCrewPortraitContent()
	{
		if ((Object)portrait != (Object)null)
		{
			portrait.ForceRefresh();
		}
	}

	private string seniorityString()
	{
		Attributes attributes = identity.GetAttributes();
		return attributes.GetProfessionString(true);
	}

	public void SelectCrewMember(bool focus)
	{
		if (focus)
		{
			SelectTool.Instance.SelectAndFocus(identity.transform.GetPosition(), identity.GetComponent<KSelectable>(), new Vector3(8f, 0f, 0f));
		}
		else
		{
			SelectTool.Instance.Select(identity.GetComponent<KSelectable>(), false);
		}
	}
}
