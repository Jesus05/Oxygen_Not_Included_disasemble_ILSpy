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
	private RectTransform scaleOffsetAnchor;

	[SerializeField]
	private RectTransform contentPositionDummy;

	[SerializeField]
	private KButton zoomOutButton;

	[SerializeField]
	private KButton zoomInButton;

	private Tech currentResearch;

	public KButton CloseButton;

	private float targetContentScale = 1f;

	private GraphicRaycaster m_Raycaster;

	private PointerEventData m_PointerEventData;

	private UnityEngine.EventSystems.EventSystem m_EventSystem;

	private Vector3 currentScrollPosition;

	private float keyboardScrollSpeed = 1500f;

	public float contentPositionLerpSpeed = 10f;

	private bool zoomingOut = false;

	private bool zoomingIn = false;

	private bool rightMouseDown = false;

	private bool isDragging = false;

	private Vector3 dragStartPosition;

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
		m_EventSystem = GetComponent<UnityEngine.EventSystems.EventSystem>();
		contentPositionDummy.SetLocalPosition(new Vector3(1000f, -2500f, 0f));
	}

	private IEnumerator ZoomOut()
	{
		KCanvasScaler kCanvasScaler = Object.FindObjectOfType<KCanvasScaler>();
		zoomingOut = true;
		contentPositionDummy.transform.SetParent(scaleOffsetAnchor.transform);
		float zoomAmount = Mathf.Clamp(0.45f * kCanvasScaler.GetCanvasScale(), 0.1f, 1f);
		Vector3 localScale = scaleOffsetAnchor.transform.localScale;
		if (localScale.x > zoomAmount)
		{
			scaleOffsetAnchor.transform.localScale *= 1f - Mathf.Clamp(Time.unscaledDeltaTime * 10f, 0f, 1f);
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		scaleOffsetAnchor.transform.localScale = Vector3.one * zoomAmount;
		contentPositionDummy.transform.SetParent(scaleOffsetAnchor.transform.parent);
		zoomingOut = false;
	}

	private IEnumerator ZoomIn()
	{
		KCanvasScaler kCanvasScaler = Object.FindObjectOfType<KCanvasScaler>();
		zoomingIn = true;
		contentPositionDummy.transform.SetParent(scaleOffsetAnchor.transform);
		float zoomAmount = Mathf.Clamp(1f * kCanvasScaler.GetCanvasScale(), 1f, 1.6f);
		Vector3 localScale = scaleOffsetAnchor.transform.localScale;
		if (localScale.x < zoomAmount)
		{
			scaleOffsetAnchor.transform.localScale *= 1f + Mathf.Clamp(Time.unscaledDeltaTime * 10f, 0f, 1f);
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		scaleOffsetAnchor.transform.localScale = Vector3.one * zoomAmount;
		contentPositionDummy.transform.SetParent(scaleOffsetAnchor.transform.parent);
		zoomingIn = false;
	}

	private void Update()
	{
		if (!isDragging && rightMouseDown && Vector3.Distance(dragStartPosition, Input.mousePosition) > 1f)
		{
			isDragging = true;
		}
		if (!zoomingIn && !zoomingOut)
		{
			scaleOffsetAnchor.SetPosition(Input.mousePosition);
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				contentPositionDummy.transform.position -= Vector3.up * Time.unscaledDeltaTime * keyboardScrollSpeed;
			}
			else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				contentPositionDummy.transform.position += Vector3.up * Time.unscaledDeltaTime * keyboardScrollSpeed;
			}
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				contentPositionDummy.transform.position += Vector3.right * Time.unscaledDeltaTime * keyboardScrollSpeed;
			}
			else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				contentPositionDummy.transform.position -= Vector3.right * Time.unscaledDeltaTime * keyboardScrollSpeed;
			}
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			if (mouseScrollDelta.y > 0f)
			{
				StartCoroutine(ZoomIn());
			}
			else
			{
				Vector2 mouseScrollDelta2 = Input.mouseScrollDelta;
				if (mouseScrollDelta2.y < 0f)
				{
					StartCoroutine(ZoomOut());
				}
				else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
				{
					if ((Object)contentPositionDummy.transform.parent != (Object)scaleOffsetAnchor.transform)
					{
						contentPositionDummy.transform.SetParent(scaleOffsetAnchor.transform);
					}
				}
				else if ((Object)contentPositionDummy.transform.parent != (Object)scaleOffsetAnchor.transform.parent)
				{
					contentPositionDummy.transform.SetParent(scaleOffsetAnchor.transform.parent);
				}
			}
		}
		contentPositionDummy.position = ClampScrollToContent();
		Vector3 position = Vector3.Lerp(scrollContent.transform.position, contentPositionDummy.transform.position, Time.unscaledDeltaTime * contentPositionLerpSpeed);
		scrollContent.transform.SetPosition(position);
		scrollContent.transform.localScale = Vector3.Lerp(scrollContent.transform.localScale, contentPositionDummy.lossyScale, Time.unscaledDeltaTime * contentPositionLerpSpeed);
	}

	private Vector3 ClampScrollToContent()
	{
		Vector3 position = contentPositionDummy.position;
		if (!zoomingIn && !zoomingOut)
		{
			Vector3 vector = foreground.rectTransform().InverseTransformPoint(scrollContent.rectTransform().position);
			float num = 512f;
			Vector2 sizeDelta = scrollContent.rectTransform().sizeDelta;
			float num2 = sizeDelta.x / 2f;
			Vector3 localScale = scrollContent.transform.localScale;
			float num3 = num2 * localScale.x - foreground.rectTransform().rect.width / 2f + num;
			if (vector.x > num3)
			{
				position.x -= vector.x - num3;
			}
			Vector2 sizeDelta2 = scrollContent.rectTransform().sizeDelta;
			float num4 = sizeDelta2.x / 2f;
			Vector3 localScale2 = scrollContent.transform.localScale;
			float num5 = 0f - (num4 * localScale2.x - foreground.rectTransform().rect.width / 2f + num);
			if (vector.x < num5)
			{
				position.x -= vector.x - num5;
			}
			Vector2 sizeDelta3 = scrollContent.rectTransform().sizeDelta;
			float num6 = sizeDelta3.y / 2f;
			Vector3 localScale3 = scrollContent.transform.localScale;
			float num7 = num6 * localScale3.y - foreground.rectTransform().rect.height / 2f + num;
			if (vector.y > num7)
			{
				position.y -= vector.y - num7;
			}
			Vector2 sizeDelta4 = scrollContent.rectTransform().sizeDelta;
			float num8 = sizeDelta4.y / 2f;
			Vector3 localScale4 = scrollContent.transform.localScale;
			float num9 = 0f - (num8 * localScale4.y - foreground.rectTransform().rect.height / 2f + num);
			if (vector.y < num9)
			{
				position.y -= vector.y - num9;
			}
			Vector3 localScale5 = scrollContent.transform.localScale;
			if (localScale5.x < 0.7f)
			{
				float width = foreground.rectTransform().rect.width;
				float width2 = scrollContent.rectTransform().rect.width;
				Vector3 localScale6 = scrollContent.transform.localScale;
				if (width > width2 * localScale6.x)
				{
					position.x = foreground.rectTransform().rect.width / 2f;
				}
			}
		}
		return position;
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
			filterField.text = "";
			OnFilterChanged("");
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
			StartCoroutine(ZoomOut());
		};
		zoomInButton.onClick += delegate
		{
			StartCoroutine(ZoomIn());
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
		if (entryMap.ContainsKey(tech))
		{
			return entryMap[tech].transform.GetPosition();
		}
		Debug.LogError("The Tech provided was not present in the dictionary");
		return Vector3.zero;
	}

	public ResearchEntry GetEntry(Tech tech)
	{
		if (entryMap != null)
		{
			if (entryMap.ContainsKey(tech))
			{
				return entryMap[tech];
			}
			Debug.LogError("The Tech provided was not present in the dictionary");
			return null;
		}
		return null;
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
		filterField.text = "";
		OnFilterChanged("");
		UpdateProgressBars();
		UpdatePointDisplay();
		zoomingIn = false;
		zoomingOut = false;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed && e.IsAction(Action.MouseRight))
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
		base.OnKeyUp(e);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				dragStartPosition = Input.mousePosition;
				rightMouseDown = true;
				return;
			}
			if (e.TryConsume(Action.ZoomIn))
			{
				targetContentScale = Mathf.Clamp(targetContentScale * (1f + Time.unscaledDeltaTime * 2.5f), 0.5f, 1f);
				return;
			}
			if (e.TryConsume(Action.ZoomOut))
			{
				targetContentScale = Mathf.Clamp(targetContentScale * (1f - Time.unscaledDeltaTime * 2.5f), 0.5f, 1f);
				return;
			}
			if (e.TryConsume(Action.Escape))
			{
				ManagementMenu.Instance.CloseAll();
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
