using ProcGen;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CellSelectionObject : KMonoBehaviour
{
	[HideInInspector]
	public CellSelectionObject alternateSelectionObject;

	private float zDepth = -0.5f;

	private float zDepthSelected;

	private KBoxCollider2D mCollider;

	private KSelectable mSelectable;

	private Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

	public GameObject SelectedDisplaySprite;

	public Sprite Sprite_Selected;

	public Sprite Sprite_Hover;

	public int mouseCell;

	private int selectedCell;

	public string ElementName;

	public Element element;

	public Element.State state;

	public float Mass;

	public float temperature;

	public Tag tags;

	public byte diseaseIdx;

	public int diseaseCount;

	private float updateTimer;

	private Dictionary<SimViewMode, Func<bool>> overlayFilterMap = new Dictionary<SimViewMode, Func<bool>>();

	private bool isAppFocused = true;

	public int SelectedCell => selectedCell;

	public float FlowRate => Grid.AccumulatedFlow[selectedCell] / 3f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		mCollider = GetComponent<KBoxCollider2D>();
		mCollider.size = new Vector2(1.1f, 1.1f);
		mSelectable = GetComponent<KSelectable>();
		SelectedDisplaySprite.transform.localScale = Vector3.one * 0.390625f;
		SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = Sprite_Hover;
		Subscribe(Game.Instance.gameObject, 493375141, ForceRefreshUserMenu);
		overlayFilterMap.Add(SimViewMode.OxygenMap, () => Grid.Element[mouseCell].IsGas);
		overlayFilterMap.Add(SimViewMode.GasVentMap, () => Grid.Element[mouseCell].IsGas);
		overlayFilterMap.Add(SimViewMode.LiquidVentMap, () => Grid.Element[mouseCell].IsLiquid);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		isAppFocused = focusStatus;
	}

	private void Update()
	{
		if (isAppFocused && !((UnityEngine.Object)SelectTool.Instance == (UnityEngine.Object)null) && !((UnityEngine.Object)Game.Instance == (UnityEngine.Object)null) && Game.Instance.GameStarted())
		{
			SelectedDisplaySprite.SetActive(PlayerController.Instance.IsUsingDefaultTool() && !DebugHandler.HideUI);
			if ((UnityEngine.Object)SelectTool.Instance.selected != (UnityEngine.Object)mSelectable)
			{
				mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
				if (Grid.IsValidCell(mouseCell) && Grid.IsVisible(mouseCell))
				{
					bool flag = true;
					foreach (KeyValuePair<SimViewMode, Func<bool>> item in overlayFilterMap)
					{
						if (item.Value == null)
						{
							Debug.LogWarning("Filter value is null", null);
						}
						else if ((UnityEngine.Object)OverlayScreen.Instance == (UnityEngine.Object)null)
						{
							Debug.LogWarning("Overlay screen Instance is null", null);
						}
						else if (OverlayScreen.Instance.GetMode() == item.Key)
						{
							flag = false;
							if (base.gameObject.layer != LayerMask.NameToLayer("MaskedOverlay"))
							{
								base.gameObject.layer = LayerMask.NameToLayer("MaskedOverlay");
							}
							if (item.Value())
							{
								break;
							}
							SelectedDisplaySprite.SetActive(false);
							return;
						}
					}
					if (flag && base.gameObject.layer != LayerMask.NameToLayer("Default"))
					{
						base.gameObject.layer = LayerMask.NameToLayer("Default");
					}
					Vector3 position = Grid.CellToPos(mouseCell, 0f, 0f, 0f) + offset;
					position.z = zDepth;
					base.transform.SetPosition(position);
					mSelectable.SetName(Grid.Element[mouseCell].name);
				}
				if ((UnityEngine.Object)SelectTool.Instance.hover != (UnityEngine.Object)mSelectable)
				{
					SelectedDisplaySprite.SetActive(false);
				}
			}
			updateTimer += Time.deltaTime;
			if (updateTimer >= 0.5f)
			{
				updateTimer = 0f;
				if ((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)mSelectable)
				{
					UpdateValues();
				}
			}
		}
	}

	public void UpdateValues()
	{
		if (Grid.IsValidCell(selectedCell))
		{
			Mass = Grid.Mass[selectedCell];
			element = Grid.Element[selectedCell];
			ElementName = element.name;
			state = element.state;
			tags = element.GetMaterialCategoryTag();
			temperature = Grid.Temperature[selectedCell];
			diseaseIdx = Grid.DiseaseIdx[selectedCell];
			diseaseCount = Grid.DiseaseCount[selectedCell];
			mSelectable.SetName(Grid.Element[selectedCell].name);
			DetailsScreen.Instance.Trigger(-1514841199, null);
			UpdateStatusItem();
			if (element.id == SimHashes.OxyRock)
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.OxyRockEmitting, this);
				if (FlowRate <= 0f)
				{
					mSelectable.AddStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, this);
				}
				else
				{
					mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, false);
				}
			}
			else
			{
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockEmitting, false);
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.OxyRockBlocked, false);
			}
			if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(selectedCell))
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BuriedItem, this);
			}
			else
			{
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.BuriedItem, true);
			}
			bool on = IsExposedToSpace(selectedCell);
			mSelectable.ToggleStatusItem(Db.Get().MiscStatusItems.Space, on, null);
		}
	}

	public static bool IsExposedToSpace(int cell)
	{
		return Game.Instance.world.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.Space && (UnityEngine.Object)Grid.Objects[cell, 2] == (UnityEngine.Object)null;
	}

	private void UpdateStatusItem()
	{
		if (element.id == SimHashes.Vacuum || element.id == SimHashes.Void)
		{
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalCategory, true);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, true);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalMass, true);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalDisease, true);
		}
		else
		{
			if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalCategory))
			{
				Func<Element> data = () => element;
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, data);
			}
			if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalTemperature))
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, this);
			}
			if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalMass))
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalMass, this);
			}
			if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalDisease))
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalDisease, this);
			}
		}
	}

	public void OnObjectSelected(object o)
	{
		SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = Sprite_Hover;
		UpdateStatusItem();
		if ((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)mSelectable)
		{
			selectedCell = Grid.PosToCell(base.gameObject);
			UpdateValues();
			Vector3 position = Grid.CellToPos(selectedCell, 0f, 0f, 0f) + offset;
			position.z = zDepthSelected;
			base.transform.SetPosition(position);
			SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = Sprite_Selected;
		}
	}

	public string MassString()
	{
		return $"{Mass:0.00}";
	}

	private void ForceRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.Refresh(base.gameObject);
	}
}
