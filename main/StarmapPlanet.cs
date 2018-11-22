using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarmapPlanet : KMonoBehaviour
{
	public List<StarmapPlanetVisualizer> visualizers;

	public void SetSprite(Sprite sprite, Color color)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.image.sprite = sprite;
			visualizer.image.color = color;
		}
	}

	public void SetFillAmount(float amount)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.image.fillAmount = amount;
		}
	}

	public void SetUnknownBGActive(bool active, Color color)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.unknownBG.gameObject.SetActive(active);
			visualizer.unknownBG.color = color;
		}
	}

	public void SetSelectionActive(bool active)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.selection.gameObject.SetActive(active);
		}
	}

	public void SetAnalysisActive(bool active)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.analysisSelection.SetActive(active);
		}
	}

	public void SetLabel(string text)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.label.text = text;
			ShowLabel(false);
		}
	}

	public void ShowLabel(bool show)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.label.gameObject.SetActive(show);
		}
	}

	public void SetOnClick(System.Action del)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.button.onClick = del;
		}
	}

	public void SetOnEnter(System.Action del)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.button.onEnter = del;
		}
	}

	public void SetOnExit(System.Action del)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.button.onExit = del;
		}
	}

	public void AnimateSelector(float time)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			visualizer.selection.anchoredPosition = new Vector2(0f, 25f + Mathf.Sin(time * 4f) * 5f);
		}
	}

	public void ShowAsCurrentRocketDestination(bool show)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			RectTransform rectTransform = visualizer.rocketIconContainer.rectTransform();
			if (rectTransform.childCount > 0)
			{
				HierarchyReferences component = rectTransform.GetChild(rectTransform.childCount - 1).GetComponent<HierarchyReferences>();
				component.GetReference<Image>("fg").color = ((!show) ? Color.white : new Color(0.117647059f, 0.8627451f, 0.3137255f));
			}
		}
	}

	public void SetRocketIcons(int numRockets, GameObject iconPrefab)
	{
		foreach (StarmapPlanetVisualizer visualizer in visualizers)
		{
			RectTransform rectTransform = visualizer.rocketIconContainer.rectTransform();
			for (int i = rectTransform.childCount; i < numRockets; i++)
			{
				Util.KInstantiateUI(iconPrefab, visualizer.rocketIconContainer, true);
			}
			for (int num = rectTransform.childCount; num > numRockets; num--)
			{
				UnityEngine.Object.Destroy(rectTransform.GetChild(num - 1).gameObject);
			}
			int num2 = 0;
			IEnumerator enumerator2 = rectTransform.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					RectTransform rectTransform2 = (RectTransform)enumerator2.Current;
					rectTransform2.anchoredPosition = new Vector2((float)num2 * -10f, 0f);
					num2++;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator2 as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
