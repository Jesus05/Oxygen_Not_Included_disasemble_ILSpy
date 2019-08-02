using STRINGS;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementWidget : KMonoBehaviour
{
	private Color color_dark_red = new Color(0.282352954f, 0.160784319f, 0.149019614f);

	private Color color_gold = new Color(1f, 0.635294139f, 0.286274523f);

	private Color color_dark_grey = new Color(0.215686277f, 0.215686277f, 0.215686277f);

	private Color color_grey = new Color(0.6901961f, 0.6901961f, 0.6901961f);

	[SerializeField]
	private RectTransform sheenTransform;

	public AnimationCurve flourish_iconScaleCurve;

	public AnimationCurve flourish_sheenPositionCurve;

	public KBatchedAnimController[] sparks;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void Update()
	{
	}

	public void ActivateNewlyAchievedFlourish(float delay = 1f)
	{
		StartCoroutine(Flourish(delay));
	}

	private IEnumerator Flourish(float startDelay)
	{
		SetNeverAchieved();
		if ((Object)GetComponent<Canvas>() == (Object)null)
		{
			Canvas canvas = base.gameObject.AddComponent<Canvas>();
			canvas.sortingOrder = 1;
		}
		GetComponent<Canvas>().overrideSorting = true;
		yield return (object)new WaitForSecondsRealtime(startDelay);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void SetAchievedNow()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(1);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_red;
		component2.GetReference<Image>("iconBorder").color = color_gold;
		component2.GetReference<Image>("icon").color = color_gold;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = Color.white;
		}
		GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
	}

	public void SetAchievedBefore()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(1);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_red;
		component2.GetReference<Image>("iconBorder").color = color_gold;
		component2.GetReference<Image>("icon").color = color_gold;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = Color.white;
		}
		GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
	}

	public void SetNeverAchieved()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_grey;
		component2.GetReference<Image>("iconBorder").color = color_grey;
		component2.GetReference<Image>("icon").color = color_grey;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			LocText locText2 = locText;
			Color color = locText.color;
			float r = color.r;
			Color color2 = locText.color;
			float g = color2.g;
			Color color3 = locText.color;
			locText2.color = new Color(r, g, color3.b, 0.6f);
		}
		GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
	}

	public void SetNotAchieved()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_grey;
		component2.GetReference<Image>("iconBorder").color = color_grey;
		component2.GetReference<Image>("icon").color = color_grey;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			LocText locText2 = locText;
			Color color = locText.color;
			float r = color.r;
			Color color2 = locText.color;
			float g = color2.g;
			Color color3 = locText.color;
			locText2.color = new Color(r, g, color3.b, 0.6f);
		}
		GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
	}
}
