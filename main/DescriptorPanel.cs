using System.Collections.Generic;
using UnityEngine;

public class DescriptorPanel : KMonoBehaviour
{
	[SerializeField]
	private GameObject customLabelPrefab;

	private List<GameObject> labels = new List<GameObject>();

	public bool HasDescriptors()
	{
		return labels.Count > 0;
	}

	public void SetDescriptors(IList<Descriptor> descriptors)
	{
		int i;
		for (i = 0; i < descriptors.Count; i++)
		{
			GameObject gameObject = null;
			if (i >= labels.Count)
			{
				GameObject original = (!((Object)customLabelPrefab != (Object)null)) ? ScreenPrefabs.Instance.DescriptionLabel : customLabelPrefab;
				gameObject = Util.KInstantiate(original, base.gameObject, null);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				labels.Add(gameObject);
			}
			else
			{
				gameObject = labels[i];
			}
			gameObject.GetComponent<LocText>().text = descriptors[i].IndentedText();
			ToolTip component = gameObject.GetComponent<ToolTip>();
			Descriptor descriptor = descriptors[i];
			component.toolTip = descriptor.tooltipText;
			gameObject.SetActive(true);
		}
		for (; i < labels.Count; i++)
		{
			labels[i].SetActive(false);
		}
	}
}
