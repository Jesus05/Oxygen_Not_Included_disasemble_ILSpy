using STRINGS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

	public GameObject foreground;

	public GameObject scrollContent;

	public GameObject pointDisplayCountPrefab;

	public GameObject pointDisplayContainer;

	private Dictionary<string, LocText> pointDisplayMap;

	private Dictionary<Tech, ResearchEntry> entryMap;

	[SerializeField]
	private TMP_InputField filterField;

	[SerializeField]
	private KButton filterClearButton;

	private Tech currentResearch;

	public KButton CloseButton;

	public bool IsBeingResearched(Tech tech)
	{
		return Research.Instance.IsBeingResearched(tech);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
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
			Vector2 center = y.center;
			ref float y3 = ref center.y;
			Vector2 center2 = x.center;
			return y3.CompareTo(center2.y);
		});
		List<Vector2> list = new List<Vector2>();
		float x2 = 0f;
		float y2 = 0f;
		Vector2 b = new Vector2(x2, y2);
		for (int i = 0; i < resources.Count; i++)
		{
			ResearchEntry researchEntry = Util.KInstantiateUI<ResearchEntry>(entryPrefab.gameObject, scrollContent, false);
			Tech tech = resources[i];
			researchEntry.name = tech.Name + " Panel";
			Vector3 v = tech.center + b;
			researchEntry.transform.rectTransform().anchoredPosition = v;
			researchEntry.transform.rectTransform().sizeDelta = new Vector2(tech.width, tech.height);
			entryMap.Add(tech, researchEntry);
			if (tech.edges.Count > 0)
			{
				for (int j = 0; j < tech.edges.Count; j++)
				{
					ResourceTreeNode.Edge edge = tech.edges[j];
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
							for (int k = 1; k < edge.path.Count; k++)
							{
								list.Add(edge.path[k - 1]);
								list.Add(edge.path[k]);
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
		for (int l = 0; l < list.Count; l++)
		{
			List<Vector2> list2 = list;
			int index = l;
			Vector2 vector = list[l];
			float x3 = vector.x;
			Vector2 vector2 = list[l];
			list2[index] = new Vector2(x3, vector2.y + foreground.transform.rectTransform().rect.height);
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
		foreground.GetComponent<KScrollRect>().allowHorizontalScrollWheel = false;
		base.OnSpawn();
		Show(false);
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
		Debug.LogError("The Tech provided was not present in the dictionary", null);
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
			Debug.LogError("The Tech provided was not present in the dictionary", null);
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
		filterField.text = "";
		OnFilterChanged("");
		UpdateProgressBars();
		UpdatePointDisplay();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && (e.TryConsume(Action.MouseRight) || e.TryConsume(Action.Escape)))
		{
			ManagementMenu.Instance.CloseAll();
		}
		else
		{
			base.OnKeyDown(e);
		}
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
