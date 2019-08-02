using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidDescriptorPanel : KMonoBehaviour
{
	[SerializeField]
	private GameObject customLabelPrefab;

	private List<GameObject> labels = new List<GameObject>();

	public bool HasDescriptors()
	{
		return labels.Count > 0;
	}

	public void SetDescriptors(IList<AsteroidDescriptor> descriptors)
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
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			LocText reference = component.GetReference<LocText>("Label");
			AsteroidDescriptor asteroidDescriptor = descriptors[i];
			reference.text = asteroidDescriptor.text;
			ToolTip reference2 = component.GetReference<ToolTip>("ToolTip");
			AsteroidDescriptor asteroidDescriptor2 = descriptors[i];
			reference2.toolTip = asteroidDescriptor2.tooltip;
			AsteroidDescriptor asteroidDescriptor3 = descriptors[i];
			if (asteroidDescriptor3.bands != null)
			{
				Transform reference3 = component.GetReference<Transform>("BandContainer");
				Transform reference4 = component.GetReference<Transform>("BarBitPrefab");
				int j = 0;
				while (true)
				{
					int num = j;
					AsteroidDescriptor asteroidDescriptor4 = descriptors[i];
					if (num >= asteroidDescriptor4.bands.Count)
					{
						break;
					}
					Transform transform = (j < reference3.childCount) ? reference3.GetChild(j) : Util.KInstantiateUI<Transform>(reference4.gameObject, reference3.gameObject, false);
					Image component2 = transform.GetComponent<Image>();
					LayoutElement component3 = transform.GetComponent<LayoutElement>();
					Image image = component2;
					AsteroidDescriptor asteroidDescriptor5 = descriptors[i];
					image.color = asteroidDescriptor5.bands[j].second;
					LayoutElement layoutElement = component3;
					AsteroidDescriptor asteroidDescriptor6 = descriptors[i];
					layoutElement.flexibleWidth = asteroidDescriptor6.bands[j].third;
					ToolTip component4 = transform.GetComponent<ToolTip>();
					AsteroidDescriptor asteroidDescriptor7 = descriptors[i];
					component4.toolTip = asteroidDescriptor7.bands[j].first;
					transform.gameObject.SetActive(true);
					j++;
				}
				for (; j < reference3.childCount; j++)
				{
					reference3.GetChild(j).gameObject.SetActive(false);
				}
			}
			gameObject.SetActive(true);
		}
		for (; i < labels.Count; i++)
		{
			labels[i].SetActive(false);
		}
	}
}
