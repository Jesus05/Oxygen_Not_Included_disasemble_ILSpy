using Klei.AI;
using System;
using System.Collections.Generic;
using TemplateClasses;
using TMPro;
using UnityEngine;

public class DebugBaseTemplateButton : KScreen
{
	private bool SaveAllBuildings;

	private bool SaveAllPickups;

	public KButton saveBaseButton;

	public KButton clearButton;

	private TemplateContainer pasteAndSelectAsset;

	public KButton AddSelectionButton;

	public KButton RemoveSelectionButton;

	public KButton clearSelectionButton;

	public KButton DestroyButton;

	public KButton DeconstructButton;

	public KButton MoveButton;

	public TemplateContainer moveAsset;

	public TMP_InputField nameField;

	private bool editing;

	private string SaveName = "enter_template_name";

	public GameObject Placer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public List<int> SelectedCells = new List<int>();

	public static DebugBaseTemplateButton Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		base.gameObject.SetActive(false);
		SetupLocText();
		ConsumeMouseScroll = true;
		TMP_InputField tMP_InputField = nameField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, (System.Action)delegate
		{
			editing = true;
		});
		nameField.onEndEdit.AddListener(delegate
		{
			editing = false;
		});
		nameField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(nameField, true);
		});
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		ConsumeMouseScroll = true;
	}

	public override float GetSortKey()
	{
		return 10f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (editing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((UnityEngine.Object)saveBaseButton != (UnityEngine.Object)null)
		{
			saveBaseButton.onClick -= OnClickSaveBase;
			saveBaseButton.onClick += OnClickSaveBase;
		}
		if ((UnityEngine.Object)clearButton != (UnityEngine.Object)null)
		{
			clearButton.onClick -= OnClickClear;
			clearButton.onClick += OnClickClear;
		}
		if ((UnityEngine.Object)AddSelectionButton != (UnityEngine.Object)null)
		{
			AddSelectionButton.onClick -= OnClickAddSelection;
			AddSelectionButton.onClick += OnClickAddSelection;
		}
		if ((UnityEngine.Object)RemoveSelectionButton != (UnityEngine.Object)null)
		{
			RemoveSelectionButton.onClick -= OnClickRemoveSelection;
			RemoveSelectionButton.onClick += OnClickRemoveSelection;
		}
		if ((UnityEngine.Object)clearSelectionButton != (UnityEngine.Object)null)
		{
			clearSelectionButton.onClick -= OnClickClearSelection;
			clearSelectionButton.onClick += OnClickClearSelection;
		}
		if ((UnityEngine.Object)MoveButton != (UnityEngine.Object)null)
		{
			MoveButton.onClick -= OnClickMove;
			MoveButton.onClick += OnClickMove;
		}
		if ((UnityEngine.Object)DestroyButton != (UnityEngine.Object)null)
		{
			DestroyButton.onClick -= OnClickDestroySelection;
			DestroyButton.onClick += OnClickDestroySelection;
		}
		if ((UnityEngine.Object)DeconstructButton != (UnityEngine.Object)null)
		{
			DeconstructButton.onClick -= OnClickDeconstructSelection;
			DeconstructButton.onClick += OnClickDeconstructSelection;
		}
	}

	private void SetupLocText()
	{
	}

	private void OnClickDestroySelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Destroy);
	}

	private void OnClickDeconstructSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Deconstruct);
	}

	private void OnClickMove()
	{
		DebugTool.Instance.DeactivateTool(null);
		moveAsset = GetSelectionAsAsset();
		StampTool.Instance.Activate(moveAsset, false, false);
	}

	private void OnClickAddSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.AddSelection);
	}

	private void OnClickRemoveSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.RemoveSelection);
	}

	private void OnClickClearSelection()
	{
		ClearSelection();
		nameField.text = string.Empty;
	}

	private void OnClickClear()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Clear);
	}

	protected override void OnDeactivate()
	{
		if ((UnityEngine.Object)DebugTool.Instance != (UnityEngine.Object)null)
		{
			DebugTool.Instance.DeactivateTool(null);
		}
		base.OnDeactivate();
	}

	private void OnDisable()
	{
		if ((UnityEngine.Object)DebugTool.Instance != (UnityEngine.Object)null)
		{
			DebugTool.Instance.DeactivateTool(null);
		}
	}

	private TemplateContainer GetSelectionAsAsset()
	{
		List<Cell> list = new List<Cell>();
		List<Prefab> list2 = new List<Prefab>();
		List<Prefab> list3 = new List<Prefab>();
		List<Prefab> _primaryElementOres = new List<Prefab>();
		List<Prefab> _otherEntities = new List<Prefab>();
		HashSet<GameObject> _excludeEntities = new HashSet<GameObject>();
		float num = 0f;
		float num2 = 0f;
		foreach (int selectedCell in SelectedCells)
		{
			float num3 = num;
			Vector2I vector2I = Grid.CellToXY(selectedCell);
			num = num3 + (float)vector2I.x;
			float num4 = num2;
			Vector2I vector2I2 = Grid.CellToXY(selectedCell);
			num2 = num4 + (float)vector2I2.y;
		}
		float x = num / (float)SelectedCells.Count;
		float y = num2 /= (float)SelectedCells.Count;
		int cell = Grid.PosToCell(new Vector3(x, y, 0f));
		Grid.CellToXY(cell, out int x2, out int y2);
		for (int i = 0; i < SelectedCells.Count; i++)
		{
			int i2 = SelectedCells[i];
			Grid.CellToXY(SelectedCells[i], out int x3, out int y3);
			Element element = ElementLoader.elements[Grid.ElementIdx[i2]];
			string diseaseName = (Grid.DiseaseIdx[i2] == 255) ? null : Db.Get().Diseases[Grid.DiseaseIdx[i2]].Id;
			list.Add(new Cell(x3 - x2, y3 - y2, element.id, Grid.Temperature[i2], Grid.Mass[i2], diseaseName, Grid.DiseaseCount[i2], Grid.PreventFogOfWarReveal[SelectedCells[i]]));
		}
		for (int j = 0; j < Components.BuildingCompletes.Count; j++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[j];
			if (!_excludeEntities.Contains(buildingComplete.gameObject))
			{
				Grid.CellToXY(Grid.PosToCell(buildingComplete), out int x4, out int y4);
				if (SaveAllBuildings || SelectedCells.Contains(Grid.PosToCell(buildingComplete)))
				{
					int[] placementCells = buildingComplete.PlacementCells;
					string diseaseName2;
					foreach (int num5 in placementCells)
					{
						Grid.CellToXY(num5, out int x5, out int y5);
						diseaseName2 = ((Grid.DiseaseIdx[num5] == 255) ? null : Db.Get().Diseases[Grid.DiseaseIdx[num5]].Id);
						list.Add(new Cell(x5 - x2, y5 - y2, Grid.Element[num5].id, Grid.Temperature[num5], Grid.Mass[num5], diseaseName2, Grid.DiseaseCount[num5], false));
					}
					Orientation rotation = Orientation.Neutral;
					Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						rotation = component.GetOrientation();
					}
					SimHashes element2 = SimHashes.Void;
					float value = 280f;
					diseaseName2 = null;
					int disease_count = 0;
					PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
					{
						element2 = component2.ElementID;
						value = component2.Temperature;
						diseaseName2 = ((component2.DiseaseIdx == 255) ? null : Db.Get().Diseases[component2.DiseaseIdx].Id);
						disease_count = component2.DiseaseCount;
					}
					List<Prefab.template_amount_value> list4 = new List<Prefab.template_amount_value>();
					List<Prefab.template_amount_value> list5 = new List<Prefab.template_amount_value>();
					foreach (AmountInstance amount in buildingComplete.gameObject.GetAmounts())
					{
						list4.Add(new Prefab.template_amount_value(amount.amount.Id, amount.value));
					}
					float num6 = 0f;
					Battery component3 = buildingComplete.GetComponent<Battery>();
					if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
					{
						num6 = component3.JoulesAvailable;
						list5.Add(new Prefab.template_amount_value("joulesAvailable", num6));
					}
					float num7 = 0f;
					Unsealable component4 = buildingComplete.GetComponent<Unsealable>();
					if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
					{
						num7 = (float)(component4.facingRight ? 1 : 0);
						list5.Add(new Prefab.template_amount_value("sealedDoorDirection", num7));
					}
					float num8 = 0f;
					LogicSwitch component5 = buildingComplete.GetComponent<LogicSwitch>();
					if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
					{
						num8 = (float)(component5.IsSwitchedOn ? 1 : 0);
						list5.Add(new Prefab.template_amount_value("switchSetting", num8));
					}
					x4 -= x2;
					y4 -= y2;
					value = Mathf.Clamp(value, 1f, 99999f);
					Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, x4, y4, element2, value, 0f, diseaseName2, disease_count, rotation, list4.ToArray(), list5.ToArray(), 0);
					Storage component6 = buildingComplete.gameObject.GetComponent<Storage>();
					if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
					{
						foreach (GameObject item2 in component6.items)
						{
							float units = 0f;
							SimHashes element3 = SimHashes.Vacuum;
							float temp = 280f;
							string disease = null;
							int disease_count2 = 0;
							bool isOre = false;
							PrimaryElement component7 = item2.GetComponent<PrimaryElement>();
							if ((UnityEngine.Object)component7 != (UnityEngine.Object)null)
							{
								units = component7.Units;
								element3 = component7.ElementID;
								temp = component7.Temperature;
								disease = ((component7.DiseaseIdx == 255) ? null : Db.Get().Diseases[component7.DiseaseIdx].Id);
								disease_count2 = component7.DiseaseCount;
							}
							float rotAmount = 0f;
							Rottable.Instance sMI = item2.gameObject.GetSMI<Rottable.Instance>();
							if (sMI != null)
							{
								rotAmount = sMI.RotValue;
							}
							ElementChunk component8 = item2.GetComponent<ElementChunk>();
							if ((UnityEngine.Object)component8 != (UnityEngine.Object)null)
							{
								isOre = true;
							}
							StorageItem storageItem = new StorageItem(item2.PrefabID().Name, units, temp, element3, disease, disease_count2, isOre);
							if (sMI != null)
							{
								storageItem.rottable.rotAmount = rotAmount;
							}
							prefab.AssignStorage(storageItem);
							_excludeEntities.Add(item2);
						}
					}
					list2.Add(prefab);
					_excludeEntities.Add(buildingComplete.gameObject);
				}
			}
		}
		for (int l = 0; l < list2.Count; l++)
		{
			Prefab prefab2 = list2[l];
			int x6 = prefab2.location_x + x2;
			int y6 = prefab2.location_y + y2;
			int cell2 = Grid.XYToCell(x6, y6);
			switch (prefab2.id)
			{
			default:
				prefab2.connections = 0;
				break;
			case "Wire":
			case "InsulatedWire":
			case "HighWattageWire":
				prefab2.connections = (int)Game.Instance.electricalConduitSystem.GetConnections(cell2, true);
				break;
			case "GasConduit":
			case "InsulatedGasConduit":
				prefab2.connections = (int)Game.Instance.gasConduitSystem.GetConnections(cell2, true);
				break;
			case "LiquidConduit":
			case "InsulatedLiquidConduit":
				prefab2.connections = (int)Game.Instance.liquidConduitSystem.GetConnections(cell2, true);
				break;
			case "LogicWire":
				prefab2.connections = (int)Game.Instance.logicCircuitSystem.GetConnections(cell2, true);
				break;
			}
		}
		for (int m = 0; m < Components.Pickupables.Count; m++)
		{
			if (Components.Pickupables[m].gameObject.activeSelf)
			{
				Pickupable pickupable = Components.Pickupables[m];
				if (!_excludeEntities.Contains(pickupable.gameObject))
				{
					int num9 = Grid.PosToCell(pickupable);
					if ((SaveAllPickups || SelectedCells.Contains(num9)) && !(bool)Components.Pickupables[m].gameObject.GetComponent<MinionBrain>())
					{
						Grid.CellToXY(num9, out int x7, out int y7);
						x7 -= x2;
						y7 -= y2;
						SimHashes element4 = SimHashes.Void;
						float temperature = 280f;
						float units2 = 1f;
						string disease2 = null;
						int disease_count3 = 0;
						float rotAmount2 = 0f;
						Rottable.Instance sMI2 = pickupable.gameObject.GetSMI<Rottable.Instance>();
						if (sMI2 != null)
						{
							rotAmount2 = sMI2.RotValue;
						}
						PrimaryElement component9 = pickupable.gameObject.GetComponent<PrimaryElement>();
						if ((UnityEngine.Object)component9 != (UnityEngine.Object)null)
						{
							element4 = component9.ElementID;
							units2 = component9.Units;
							temperature = component9.Temperature;
							disease2 = ((component9.DiseaseIdx == 255) ? null : Db.Get().Diseases[component9.DiseaseIdx].Id);
							disease_count3 = component9.DiseaseCount;
						}
						ElementChunk component10 = pickupable.gameObject.GetComponent<ElementChunk>();
						if ((UnityEngine.Object)component10 != (UnityEngine.Object)null)
						{
							Prefab item = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, x7, y7, element4, temperature, units2, disease2, disease_count3, Orientation.Neutral, null, null, 0);
							_primaryElementOres.Add(item);
						}
						else
						{
							Prefab item = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, x7, y7, element4, temperature, units2, disease2, disease_count3, Orientation.Neutral, null, null, 0);
							item.rottable = new TemplateClasses.Rottable();
							item.rottable.rotAmount = rotAmount2;
							list3.Add(item);
						}
						_excludeEntities.Add(pickupable.gameObject);
					}
				}
			}
		}
		GetEntities(Components.Crops.Items, x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities(Components.Health.Items, x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities(Components.Harvestables.Items, x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities(Components.Edibles.Items, x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities<Geyser>(x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities<OccupyArea>(x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities<FogOfWarMask>(x2, y2, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		TemplateContainer templateContainer = new TemplateContainer();
		templateContainer.Init(list, list2, list3, _primaryElementOres, _otherEntities);
		return templateContainer;
	}

	private void GetEntities<T>(int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		object[] component_collection = UnityEngine.Object.FindObjectsOfType(typeof(T));
		GetEntities(component_collection, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
	}

	private void GetEntities<T>(IEnumerable<T> component_collection, int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		foreach (T item2 in component_collection)
		{
			if (!_excludeEntities.Contains((item2 as KMonoBehaviour).gameObject) && (item2 as KMonoBehaviour).gameObject.activeSelf)
			{
				int num = Grid.PosToCell(item2 as KMonoBehaviour);
				if (SelectedCells.Contains(num) && !(bool)(item2 as KMonoBehaviour).gameObject.GetComponent<MinionBrain>())
				{
					Grid.CellToXY(num, out int x, out int y);
					x -= rootX;
					y -= rootY;
					SimHashes simHashes = SimHashes.Void;
					float num2 = 280f;
					float num3 = 1f;
					string text = null;
					int num4 = 0;
					PrimaryElement component = (item2 as KMonoBehaviour).gameObject.GetComponent<PrimaryElement>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						simHashes = component.ElementID;
						num3 = component.Units;
						num2 = component.Temperature;
						text = ((component.DiseaseIdx == 255) ? null : ((ResourceSet<Disease>)Db.Get().Diseases)[(int)component.DiseaseIdx].Id);
						num4 = component.DiseaseCount;
					}
					List<Prefab.template_amount_value> list = new List<Prefab.template_amount_value>();
					if ((item2 as KMonoBehaviour).gameObject.GetAmounts() != null)
					{
						foreach (AmountInstance item3 in (Modifications<Amount, AmountInstance>)(item2 as KMonoBehaviour).gameObject.GetAmounts())
						{
							list.Add(new Prefab.template_amount_value(item3.amount.Id, item3.value));
						}
					}
					ElementChunk component2 = (item2 as KMonoBehaviour).gameObject.GetComponent<ElementChunk>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
					{
						string name = (item2 as KMonoBehaviour).PrefabID().Name;
						Prefab.Type type = Prefab.Type.Ore;
						int loc_x = x;
						int loc_y = y;
						SimHashes element = simHashes;
						float temperature = num2;
						float units = num3;
						string disease = text;
						int disease_count = num4;
						Prefab.template_amount_value[] amount_values = list.ToArray();
						Prefab item = new Prefab(name, type, loc_x, loc_y, element, temperature, units, disease, disease_count, Orientation.Neutral, amount_values, null, 0);
						_primaryElementOres.Add(item);
						_excludeEntities.Add((item2 as KMonoBehaviour).gameObject);
					}
					else
					{
						string disease = (item2 as KMonoBehaviour).PrefabID().Name;
						Prefab.Type type = Prefab.Type.Other;
						int disease_count = x;
						int loc_y = y;
						SimHashes element = simHashes;
						float units = num2;
						float temperature = num3;
						string name = text;
						int loc_x = num4;
						Prefab.template_amount_value[] amount_values = list.ToArray();
						Prefab item = new Prefab(disease, type, disease_count, loc_y, element, units, temperature, name, loc_x, Orientation.Neutral, amount_values, null, 0);
						_otherEntities.Add(item);
						_excludeEntities.Add((item2 as KMonoBehaviour).gameObject);
					}
				}
			}
		}
	}

	private void OnClickSaveBase()
	{
		TemplateContainer selectionAsAsset = GetSelectionAsAsset();
		if (SelectedCells.Count <= 0)
		{
			Debug.LogWarning("No cells selected. Use buttons above to select the area you want to save.");
		}
		else
		{
			SaveName = nameField.text;
			if (SaveName == null || SaveName == string.Empty)
			{
				Debug.LogWarning("Invalid save name. Please enter a name in the input field.");
			}
			else
			{
				selectionAsAsset.SaveToYaml(SaveName);
				PasteBaseTemplateScreen.Instance.RefreshStampButtons();
			}
		}
	}

	public void ClearSelection()
	{
		for (int num = SelectedCells.Count - 1; num >= 0; num--)
		{
			RemoveFromSelection(SelectedCells[num]);
		}
	}

	public void DestroySelection()
	{
	}

	public void DeconstructSelection()
	{
	}

	public void AddToSelection(int cell)
	{
		if (!SelectedCells.Contains(cell))
		{
			GameObject gameObject2 = Grid.Objects[cell, 7] = Util.KInstantiate(Placer, null, null);
			Vector3 position = Grid.CellToPosCBC(cell, visualizerLayer);
			float num = -0.15f;
			position.z += num;
			gameObject2.transform.SetPosition(position);
			SelectedCells.Add(cell);
		}
	}

	public void RemoveFromSelection(int cell)
	{
		if (SelectedCells.Contains(cell))
		{
			GameObject gameObject = Grid.Objects[cell, 7];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				gameObject.DeleteObject();
			}
			SelectedCells.Remove(cell);
		}
	}
}
