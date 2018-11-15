using UnityEngine;
using UnityEngine.UI;

public class DetailScreenTabHeader : KTabMenuHeader
{
	public float SelectedHeight = 36f;

	public float UnselectedHeight = 30f;

	public override void ActivateTabArtwork(int tabIdx)
	{
		base.ActivateTabArtwork(tabIdx);
		if (tabIdx < base.transform.childCount)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				LayoutElement component = base.transform.GetChild(i).GetComponent<LayoutElement>();
				if ((Object)component != (Object)null)
				{
					if (i == tabIdx)
					{
						component.preferredHeight = SelectedHeight;
						component.transform.Find("Icon").GetComponent<Image>().color = new Color(0.145098045f, 0.164705887f, 0.23137255f);
					}
					else
					{
						component.preferredHeight = UnselectedHeight;
						component.transform.Find("Icon").GetComponent<Image>().color = new Color(0.356862754f, 0.372549027f, 0.4509804f);
					}
				}
			}
		}
	}
}
