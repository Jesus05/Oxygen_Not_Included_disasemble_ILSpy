using Klei.AI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DiseaseSourceVisualizer : KMonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private GameObject visualizer;

	private bool visible = false;

	public string alwaysShowDisease = null;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateVisibility();
		Components.DiseaseSourceVisualizers.Add(this);
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(OnViewModeChanged));
		base.OnCleanUp();
		Components.DiseaseSourceVisualizers.Remove(this);
		if ((UnityEngine.Object)visualizer != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(visualizer);
			visualizer = null;
		}
	}

	private void CreateVisualizer()
	{
		if (!((UnityEngine.Object)visualizer != (UnityEngine.Object)null) && !((UnityEngine.Object)GameScreenManager.Instance.worldSpaceCanvas == (UnityEngine.Object)null))
		{
			visualizer = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas, null);
		}
	}

	public void UpdateVisibility()
	{
		CreateVisualizer();
		if (string.IsNullOrEmpty(alwaysShowDisease))
		{
			visible = false;
		}
		else
		{
			Disease disease = Db.Get().Diseases.Get(alwaysShowDisease);
			if (disease != null)
			{
				SetVisibleDisease(disease);
			}
		}
		if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null)
		{
			Show(OverlayScreen.Instance.GetMode());
		}
	}

	private void SetVisibleDisease(Disease disease)
	{
		Sprite overlaySprite = Assets.instance.DiseaseVisualization.overlaySprite;
		Color32 overlayColour = disease.overlayColour;
		Transform child = visualizer.transform.GetChild(0);
		Image component = child.GetComponent<Image>();
		component.sprite = overlaySprite;
		component.color = overlayColour;
		visible = true;
	}

	private void Update()
	{
		if (!((UnityEngine.Object)visualizer == (UnityEngine.Object)null))
		{
			visualizer.transform.SetPosition(base.transform.GetPosition() + offset);
		}
	}

	private void OnViewModeChanged(HashedString mode)
	{
		Show(mode);
	}

	public void Show(HashedString mode)
	{
		base.enabled = (visible && mode == OverlayModes.Disease.ID);
		if ((UnityEngine.Object)visualizer != (UnityEngine.Object)null)
		{
			visualizer.SetActive(base.enabled);
		}
	}
}
