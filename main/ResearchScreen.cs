using STRINGS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchScreen : KModalScreen
{
	public enum ResearchState
	{
		Available,
		ActiveResearch,
		ResearchComplete,
		MissingPrerequisites,
		StateCount
	}

	[SerializeField]
	private Image BG;

	public ResearchEntry entryPrefab;

	public ResearchTreeTitle researchTreeTitlePrefab;

	public GameObject foreground;

	public GameObject scrollContent;

	public GameObject treeTitles;

	public GameObject pointDisplayCountPrefab;

	public GameObject pointDisplayContainer;

	private Dictionary<string, LocText> pointDisplayMap;

	private Dictionary<Tech, ResearchEntry> entryMap;

	[SerializeField]
	private TMP_InputField filterField;

	[SerializeField]
	private KButton filterClearButton;

	[SerializeField]
	private KButton zoomOutButton;

	[SerializeField]
	private KButton zoomInButton;

	private Tech currentResearch;

	public KButton CloseButton;

	private GraphicRaycaster m_Raycaster;

	private PointerEventData m_PointerEventData;

	private Vector3 currentScrollPosition;

	private bool panUp;

	private bool panDown;

	private bool panLeft;

	private bool panRight;

	private bool zoomingOut;

	private bool zoomingIn;

	private bool rightMouseDown;

	private bool isDragging;

	private Vector3 dragStartPosition;

	private Vector3 dragLastPosition;

	private float targetZoom = 1f;

	private float currentZoom = 1f;

	private bool zoomCenterLock;

	private Vector3 keyPanDelta = Vector3.zero;

	[SerializeField]
	private float effectiveZoomSpeed = 5f;

	[SerializeField]
	private float zoomAmountPerScroll = 0.05f;

	[SerializeField]
	private float zoomAmountPerButton = 0.5f;

	[SerializeField]
	private float minZoom = 0.15f;

	[SerializeField]
	private float maxZoom = 1f;

	[SerializeField]
	private float keyboardScrollSpeed = 200f;

	[SerializeField]
	private float keyPanEasing = 1f;

	[SerializeField]
	private float edgeClampFactor = 0.5f;

	public bool IsBeingResearched(Tech tech)
	{
		return Research.Instance.IsBeingResearched(tech);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		Transform transform = base.transform;
		while ((Object)m_Raycaster == (Object)null)
		{
			m_Raycaster = transform.GetComponent<GraphicRaycaster>();
			if ((Object)m_Raycaster == (Object)null)
			{
				transform = transform.parent;
			}
		}
	}

	private void ZoomOut()
	{
		targetZoom = Mathf.Clamp(targetZoom - zoomAmountPerButton, minZoom, maxZoom);
		zoomCenterLock = true;
	}

	private void ZoomIn()
	{
		targetZoom = Mathf.Clamp(targetZoom + zoomAmountPerButton, minZoom, maxZoom);
		zoomCenterLock = true;
	}

	private void Update()
	{
		RectTransform component = scrollContent.GetComponent<RectTransform>();
		RectTransform component2 = scrollContent.transform.parent.GetComponent<RectTransform>();
		if (!isDragging && rightMouseDown && Vector3.Distance(dragStartPosition, KInputManager.GetMousePos()) > 1f)
		{
			isDragging = true;
		}
		Vector3 position = component.GetPosition();
		float t = Mathf.Min(effectiveZoomSpeed * Time.unscaledDeltaTime, 0.9f);
		currentZoom = Mathf.Lerp(currentZoom, targetZoom, t);
		Vector3 zero = Vector3.zero;
		Vector3 mousePos = KInputManager.GetMousePos();
		Vector3 b = (!zoomCenterLock) ? (component.InverseTransformPoint(mousePos) * currentZoom) : (component.InverseTransformPoint(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f)) * currentZoom);
		component.localScale = new Vector3(currentZoom, currentZoom, 1f);
		Vector3 a = (!zoomCenterLock) ? (component.InverseTransformPoint(mousePos) * currentZoom) : (component.InverseTransformPoint(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f)) * currentZoom);
		zero = a - b;
		float d = keyboardScrollSpeed;
		if (panUp)
		{
			keyPanDelta -= Vector3.up * Time.unscaledDeltaTime * d;
		}
		else if (panDown)
		{
			keyPanDelta += Vector3.up * Time.unscaledDeltaTime * d;
		}
		if (panLeft)
		{
			keyPanDelta += Vector3.right * Time.unscaledDeltaTime * d;
		}
		else if (panRight)
		{
			keyPanDelta -= Vector3.right * Time.unscaledDeltaTime * d;
		}
		Vector3 vector = new Vector3(Mathf.Lerp(0f, keyPanDelta.x, Time.unscaledDeltaTime * keyPanEasing), Mathf.Lerp(0f, keyPanDelta.y, Time.unscaledDeltaTime * keyPanEasing), 0f);
		keyPanDelta -= vector;
		Vector3 vector2 = Vector3.zero;
		if (isDragging)
		{
			Vector3 vector3 = component.InverseTransformPoint(mousePos) * currentZoom;
			Vector3 b2 = KInputManager.GetMousePos() - dragLastPosition;
			vector2 += b2;
			dragLastPosition = KInputManager.GetMousePos();
		}
		Vector3 vector4 = position + zero + keyPanDelta + vector2;
		if (!isDragging)
		{
			Vector2 vector5 = component.rect.min * currentZoom + component2.rect.size * 0.5f;
			Vector2 vector6 = component.rect.max * currentZoom + component2.rect.size * 0.5f;
			Vector3 a2 = new Vector3(Mathf.Clamp(vector4.x, vector5.x, vector6.x), Mathf.Clamp(vector4.y, vector5.y, vector6.y), 0f);
			Vector3 vector7 = a2 - vector4;
			if (!panLeft && !panRight && !panUp && !panDown)
			{
				vector4 += vector7 * edgeClampFactor * Time.unscaledDeltaTime;
			}
			else
			{
				vector4 += vector7;
				if (vector7.x < 0f)
				{
					keyPanDelta.x = Mathf.Min(0f, keyPanDelta.x);
				}
				if (vector7.x > 0f)
				{
					keyPanDelta.x = Mathf.Max(0f, keyPanDelta.x);
				}
				if (vector7.y < 0f)
				{
					keyPanDelta.y = Mathf.Min(0f, keyPanDelta.y);
				}
				if (vector7.y > 0f)
				{
					keyPanDelta.y = Mathf.Max(0f, keyPanDelta.y);
				}
			}
		}
		component.SetPosition(vector4);
	}

	protected override void OnSpawn()
	{
		Subscribe(Research.Instance.gameObject, -1914338957, OnActiveResearchChanged);
		Subscribe(Game.Instance.gameObject, -107300940, OnResearchComplete);
		Subscribe(Game.Instance.gameObject, -1974454597, delegate
		{
			Show(false);
		});
		filterField.placeholder.GetComponent<TextMeshProUGUI>().text = UI.FILTER;
		filterField.onValueChanged.AddListener(OnFilterChanged);
		filterClearButton.onClick += delegate
		{
			filterField.text = string.Empty;
			OnFilterChanged(string.Empty);
		};
		pointDisplayMap = new Dictionary<string, LocText>();
		foreach (ResearchType type in Research.Instance.researchTypes.Types)
		{
			pointDisplayMap[type.id] = Util.KInstantiateUI(pointDisplayCountPrefab, pointDisplayContainer, true).GetComponentInChildren<LocText>();
			pointDisplayMap[type.id].text = Research.Instance.globalPointInventory.PointsByTypeID[type.id].ToString();
			pointDisplayMap[type.id].transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(type.description);
			pointDisplayMap[type.id].transform.parent.GetComponentInChildren<Image>().sprite = type.sprite;
		}
		pointDisplayContainer.transform.parent.gameObject.SetActive(Research.Instance.UseGlobalPointInventory);
		entryMap = new Dictionary<Tech, ResearchEntry>();
		List<Tech> resources = Db.Get().Techs.resources;
		resources.Sort(delegate(Tech x, Tech y)
		{
			Vector2 center3 = y.center;
			ref float y5 = ref center3.y;
			Vector2 center4 = x.center;
			return y5.CompareTo(center4.y);
		});
		List<TechTreeTitle> resources2 = Db.Get().TechTreeTitles.resources;
		resources2.Sort(delegate(TechTreeTitle x, TechTreeTitle y)
		{
			Vector2 center = y.center;
			ref float y4 = ref center.y;
			Vector2 center2 = x.center;
			return y4.CompareTo(center2.y);
		});
		float x2 = 0f;
		float y2 = 125f;
		Vector2 b = new Vector2(x2, y2);
		for (int i = 0; i < resources2.Count; i++)
		{
			ResearchTreeTitle researchTreeTitle = Util.KInstantiateUI<ResearchTreeTitle>(researchTreeTitlePrefab.gameObject, treeTitles, false);
			TechTreeTitle techTreeTitle = resources2[i];
			researchTreeTitle.name = techTreeTitle.Name + " Title";
			Vector3 v = techTreeTitle.center + b;
			researchTreeTitle.transform.rectTransform().anchoredPosition = v;
			float height = techTreeTitle.height;
			if (i + 1 < resources2.Count)
			{
				TechTreeTitle techTreeTitle2 = resources2[i + 1];
				Vector3 vector = techTreeTitle2.center + b;
				height += v.y - (vector.y + techTreeTitle2.height);
			}
			else
			{
				height += 600f;
			}
			researchTreeTitle.transform.rectTransform().sizeDelta = new Vector2(techTreeTitle.width, height);
			researchTreeTitle.SetLabel(techTreeTitle.Name);
			researchTreeTitle.SetColor(i);
		}
		List<Vector2> list = new List<Vector2>();
		float x3 = 0f;
		float y3 = 0f;
		Vector2 b2 = new Vector2(x3, y3);
		for (int j = 0; j < resources.Count; j++)
		{
			ResearchEntry researchEntry = Util.KInstantiateUI<ResearchEntry>(entryPrefab.gameObject, scrollContent, false);
			Tech tech = resources[j];
			researchEntry.name = tech.Name + " Panel";
			Vector3 v2 = tech.center + b2;
			researchEntry.transform.rectTransform().anchoredPosition = v2;
			researchEntry.transform.rectTransform().sizeDelta = new Vector2(tech.width, tech.height);
			entryMap.Add(tech, researchEntry);
			if (tech.edges.Count > 0)
			{
				for (int k = 0; k < tech.edges.Count; k++)
				{
					ResourceTreeNode.Edge edge = tech.edges[k];
					if (edge.path == null)
					{
						list.AddRange(edge.SrcTarget);
					}
					else
					{
						switch (edge.edgeType)
						{
						case ResourceTreeNode.Edge.EdgeType.PolyLineEdge:
						case ResourceTreeNode.Edge.EdgeType.QuadCurveEdge:
						case ResourceTreeNode.Edge.EdgeType.BezierEdge:
						case ResourceTreeNode.Edge.EdgeType.GenericEdge:
							list.Add(edge.SrcTarget[0]);
							list.Add(edge.path[0]);
							for (int l = 1; l < edge.path.Count; l++)
							{
								list.Add(edge.path[l - 1]);
								list.Add(edge.path[l]);
							}
							list.Add(edge.path[edge.path.Count - 1]);
							list.Add(edge.SrcTarget[1]);
							break;
						default:
							list.AddRange(edge.path);
							break;
						}
					}
				}
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			List<Vector2> list2 = list;
			int index = m;
			Vector2 vector2 = list[m];
			float x4 = vector2.x;
			Vector2 vector3 = list[m];
			list2[index] = new Vector2(x4, vector3.y + foreground.transform.rectTransform().rect.height);
		}
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.SetTech(item.Key);
		}
		CloseButton.soundPlayer.Enabled = false;
		CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		StartCoroutine(WaitAndSetActiveResearch());
		ManagementMenu.Instance.AddResearchScreen(this);
		base.OnSpawn();
		Show(false);
		zoomOutButton.onClick += delegate
		{
			ZoomOut();
		};
		zoomInButton.onClick += delegate
		{
			ZoomIn();
		};
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Unsubscribe(Game.Instance.gameObject, -1974454597, delegate
		{
			Deactivate();
		});
	}

	private IEnumerator WaitAndSetActiveResearch()
	{
		yield return (object)new WaitForEndOfFrame();
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public Vector3 GetEntryPosition(Tech tech)
	{
		if (!entryMap.ContainsKey(tech))
		{
			Debug.LogError("The Tech provided was not present in the dictionary");
			return Vector3.zero;
		}
		return entryMap[tech].transform.GetPosition();
	}

	public ResearchEntry GetEntry(Tech tech)
	{
		if (entryMap == null)
		{
			return null;
		}
		if (!entryMap.ContainsKey(tech))
		{
			Debug.LogError("The Tech provided was not present in the dictionary");
			return null;
		}
		return entryMap[tech];
	}

	public void SetEntryPercentage(Tech tech, float percent)
	{
		ResearchEntry entry = GetEntry(tech);
		if ((Object)entry != (Object)null)
		{
			entry.SetPercentage(percent);
		}
	}

	public void TurnEverythingOff()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.SetEverythingOff();
		}
	}

	public void TurnEverythingOn()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.SetEverythingOn();
		}
	}

	private void SelectAllEntries(Tech tech, bool isSelected)
	{
		ResearchEntry entry = GetEntry(tech);
		if ((Object)entry != (Object)null)
		{
			entry.QueueStateChanged(isSelected);
		}
		foreach (Tech item in tech.requiredTech)
		{
			SelectAllEntries(item, isSelected);
		}
	}

	private void OnResearchComplete(object data)
	{
		Tech tech = (Tech)data;
		ResearchEntry entry = GetEntry(tech);
		if ((Object)entry != (Object)null)
		{
			entry.ResearchCompleted(true);
		}
		UpdateProgressBars();
		UpdatePointDisplay();
	}

	private void UpdatePointDisplay()
	{
		foreach (ResearchType type in Research.Instance.researchTypes.Types)
		{
			pointDisplayMap[type.id].text = $"{Research.Instance.researchTypes.GetResearchType(type.id).name}: {Research.Instance.globalPointInventory.PointsByTypeID[type.id].ToString()}";
		}
	}

	private void OnActiveResearchChanged(object data)
	{
		List<TechInstance> list = (List<TechInstance>)data;
		foreach (TechInstance item in list)
		{
			ResearchEntry entry = GetEntry(item.tech);
			if ((Object)entry != (Object)null)
			{
				entry.QueueStateChanged(true);
			}
		}
		UpdateProgressBars();
		UpdatePointDisplay();
		if (list.Count > 0)
		{
			currentResearch = list[list.Count - 1].tech;
		}
	}

	private void UpdateProgressBars()
	{
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			item.Value.UpdateProgressBars();
		}
	}

	public void CancelResearch()
	{
		List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
		foreach (TechInstance item in researchQueue)
		{
			ResearchEntry entry = GetEntry(item.tech);
			if ((Object)entry != (Object)null)
			{
				entry.QueueStateChanged(false);
			}
		}
		researchQueue.Clear();
	}

	private void SetActiveResearch(Tech newResearch)
	{
		if (newResearch != currentResearch && currentResearch != null)
		{
			SelectAllEntries(currentResearch, false);
		}
		currentResearch = newResearch;
		if (currentResearch != null)
		{
			SelectAllEntries(currentResearch, true);
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			DetailsScreen.Instance.gameObject.SetActive(false);
		}
		else if ((Object)SelectTool.Instance.selected != (Object)null)
		{
			DetailsScreen.Instance.gameObject.SetActive(true);
			DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
		}
		filterField.text = string.Empty;
		OnFilterChanged(string.Empty);
		UpdateProgressBars();
		UpdatePointDisplay();
		zoomingIn = false;
		zoomingOut = false;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.IsAction(Action.MouseRight))
			{
				if (!isDragging && e.TryConsume(Action.MouseRight))
				{
					isDragging = false;
					rightMouseDown = false;
					ManagementMenu.Instance.CloseAll();
					return;
				}
				isDragging = false;
				rightMouseDown = false;
			}
			if (panUp && e.TryConsume(Action.PanUp))
			{
				panUp = false;
				return;
			}
			if (panDown && e.TryConsume(Action.PanDown))
			{
				panDown = false;
				return;
			}
			if (panRight && e.TryConsume(Action.PanRight))
			{
				panRight = false;
				return;
			}
			if (panLeft && e.TryConsume(Action.PanLeft))
			{
				panLeft = false;
				return;
			}
		}
		base.OnKeyUp(e);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				dragStartPosition = KInputManager.GetMousePos();
				dragLastPosition = KInputManager.GetMousePos();
				rightMouseDown = true;
				return;
			}
			if (e.TryConsume(Action.ZoomIn))
			{
				targetZoom = Mathf.Clamp(targetZoom + zoomAmountPerScroll, minZoom, maxZoom);
				zoomCenterLock = false;
				return;
			}
			if (e.TryConsume(Action.ZoomOut))
			{
				targetZoom = Mathf.Clamp(targetZoom - zoomAmountPerScroll, minZoom, maxZoom);
				zoomCenterLock = false;
				return;
			}
			if (e.TryConsume(Action.Escape))
			{
				ManagementMenu.Instance.CloseAll();
				return;
			}
			if (e.TryConsume(Action.PanLeft))
			{
				panLeft = true;
				return;
			}
			if (e.TryConsume(Action.PanRight))
			{
				panRight = true;
				return;
			}
			if (e.TryConsume(Action.PanUp))
			{
				panUp = true;
				return;
			}
			if (e.TryConsume(Action.PanDown))
			{
				panDown = true;
				return;
			}
		}
		base.OnKeyDown(e);
	}

	private void OnFilterChanged(string filter_text)
	{
		filter_text = filter_text.ToLower();
		foreach (KeyValuePair<Tech, ResearchEntry> item in entryMap)
		{
			ResearchEntry value = item.Value;
			value.UpdateFilterState(filter_text);
		}
	}
}
