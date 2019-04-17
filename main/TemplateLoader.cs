using Klei.AI;
using System;
using TemplateClasses;
using UnityEngine;

public static class TemplateLoader
{
	private static TemplateContainer template;

	public static void Stamp(TemplateContainer template, Vector2 rootLocation, System.Action on_complete_callback)
	{
		TemplateLoader.template = template;
		BuildPhase1((int)rootLocation.x, (int)rootLocation.y, delegate
		{
			BuildPhase2((int)rootLocation.x, (int)rootLocation.y, delegate
			{
				BuildPhase3((int)rootLocation.x, (int)rootLocation.y, delegate
				{
					BuildPhase4((int)rootLocation.x, (int)rootLocation.y, on_complete_callback);
				});
			});
		});
	}

	private static void BuildPhase1(int baseX, int baseY, System.Action callback)
	{
		if (Grid.WidthInCells >= 16)
		{
			CellOffset[] array = new CellOffset[template.cells.Count];
			for (int i = 0; i < template.cells.Count; i++)
			{
				array[i] = new CellOffset(template.cells[i].location_x, template.cells[i].location_y);
			}
			ClearPickups(baseX, baseY, array);
			if (template.cells.Count > 0)
			{
				ApplyGridProperties(baseX, baseY, template);
				PlaceCells(baseX, baseY, template, callback);
				ClearEntities<Crop>(baseX, baseY, array);
				ClearEntities<Health>(baseX, baseY, array);
				ClearEntities<Geyser>(baseX, baseY, array);
			}
			else
			{
				callback();
			}
		}
	}

	private static void BuildPhase2(int baseX, int baseY, System.Action callback)
	{
		int num = Grid.OffsetCell(0, baseX, baseY);
		if (template == null)
		{
			Debug.LogError("No stamp template");
		}
		if (template.buildings != null)
		{
			for (int i = 0; i < template.buildings.Count; i++)
			{
				PlaceBuilding(template.buildings[i], num);
			}
		}
		HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(callback, false));
		SimMessages.ReplaceElement(num, ElementLoader.elements[Grid.ElementIdx[num]].id, CellEventLogger.Instance.TemplateLoader, Grid.Mass[num], Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num], handle.index);
		handle.index = -1;
	}

	public static GameObject PlaceBuilding(Prefab prefab, int root_cell)
	{
		if (prefab == null || prefab.id == string.Empty)
		{
			return null;
		}
		BuildingDef buildingDef = Assets.GetBuildingDef(prefab.id);
		if ((UnityEngine.Object)buildingDef == (UnityEngine.Object)null)
		{
			return null;
		}
		int num = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, num, location_y)))
		{
			return null;
		}
		int widthInCells = Assets.GetBuildingDef(prefab.id).WidthInCells;
		if (widthInCells >= 3)
		{
			num--;
		}
		GameObject gameObject = Scenario.PlaceBuilding(root_cell, num, location_y, prefab.id, prefab.element);
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Null prefab for " + prefab.id);
			return gameObject;
		}
		BuildingComplete component = gameObject.GetComponent<BuildingComplete>();
		Rotatable component2 = gameObject.GetComponent<Rotatable>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			component2.SetOrientation(prefab.rotationOrientation);
		}
		PrimaryElement component3 = component.GetComponent<PrimaryElement>();
		if (prefab.temperature > 0f)
		{
			component3.Temperature = prefab.temperature;
		}
		component3.AddDisease(Db.Get().Diseases.GetIndex(prefab.diseaseName), prefab.diseaseCount, "TemplateLoader.PlaceBuilding");
		if (prefab.id == "Door")
		{
			for (int i = 0; i < component.PlacementCells.Length; i++)
			{
				SimMessages.ReplaceElement(component.PlacementCells[i], SimHashes.Vacuum, CellEventLogger.Instance.TemplateLoader, 0f, 0f, byte.MaxValue, 0, -1);
			}
		}
		if (prefab.amounts != null)
		{
			Prefab.template_amount_value[] amounts = prefab.amounts;
			foreach (Prefab.template_amount_value template_amount_value in amounts)
			{
				try
				{
					if (Db.Get().Amounts.Get(template_amount_value.id) != null)
					{
						gameObject.GetAmounts().SetValue(template_amount_value.id, template_amount_value.value);
					}
				}
				catch
				{
					Debug.LogWarning($"Building does not have amount with ID {template_amount_value.id}");
				}
			}
		}
		if (prefab.other_values != null)
		{
			Prefab.template_amount_value[] other_values = prefab.other_values;
			foreach (Prefab.template_amount_value template_amount_value2 in other_values)
			{
				string id = template_amount_value2.id;
				switch (id)
				{
				case "joulesAvailable":
				{
					Battery component4 = gameObject.GetComponent<Battery>();
					if ((bool)component4)
					{
						component4.AddEnergy(template_amount_value2.value);
					}
					break;
				}
				case "sealedDoorDirection":
				{
					Unsealable component5 = gameObject.GetComponent<Unsealable>();
					if ((bool)component5)
					{
						component5.facingRight = ((template_amount_value2.value != 0f) ? true : false);
					}
					break;
				}
				case "switchSetting":
				{
					_003CPlaceBuilding_003Ec__AnonStorey1 _003CPlaceBuilding_003Ec__AnonStorey = default(_003CPlaceBuilding_003Ec__AnonStorey1);
					_003CPlaceBuilding_003Ec__AnonStorey.s = gameObject.GetComponent<LogicSwitch>();
					if ((bool)_003CPlaceBuilding_003Ec__AnonStorey.s && ((_003CPlaceBuilding_003Ec__AnonStorey.s.IsSwitchedOn && template_amount_value2.value == 0f) || (!_003CPlaceBuilding_003Ec__AnonStorey.s.IsSwitchedOn && template_amount_value2.value == 1f)))
					{
						_003CPlaceBuilding_003Ec__AnonStorey.s.SetFirstFrameCallback(delegate
						{
							_003CPlaceBuilding_003Ec__AnonStorey.s.HandleToggle();
						});
					}
					break;
				}
				}
			}
		}
		if (prefab.storage != null && prefab.storage.Count > 0)
		{
			Storage component6 = component.gameObject.GetComponent<Storage>();
			if ((UnityEngine.Object)component6 == (UnityEngine.Object)null)
			{
				Debug.LogWarning("No storage component on stampTemplate building " + prefab.id + ". Saved storage contents will be ignored.");
			}
			for (int l = 0; l < prefab.storage.Count; l++)
			{
				StorageItem storageItem = prefab.storage[l];
				string id2 = storageItem.id;
				GameObject gameObject2;
				if (storageItem.isOre)
				{
					Substance substance = ElementLoader.FindElementByHash(storageItem.element).substance;
					gameObject2 = substance.SpawnResource(Vector3.zero, storageItem.units, storageItem.temperature, Db.Get().Diseases.GetIndex(storageItem.diseaseName), storageItem.diseaseCount, false, false, false);
				}
				else
				{
					gameObject2 = Scenario.SpawnPrefab(root_cell, 0, 0, id2, Grid.SceneLayer.Ore);
					if ((UnityEngine.Object)gameObject2 == (UnityEngine.Object)null)
					{
						Debug.LogWarning("Null prefab for " + id2);
						continue;
					}
					gameObject2.SetActive(true);
					PrimaryElement component7 = gameObject2.GetComponent<PrimaryElement>();
					component7.Units = storageItem.units;
					component7.Temperature = storageItem.temperature;
					component7.AddDisease(Db.Get().Diseases.GetIndex(storageItem.diseaseName), storageItem.diseaseCount, "TemplateLoader.PlaceBuilding");
					Rottable.Instance sMI = gameObject2.GetSMI<Rottable.Instance>();
					if (sMI != null)
					{
						sMI.RotValue = storageItem.rottable.rotAmount;
					}
				}
				GameObject gameObject3 = component6.Store(gameObject2, true, true, true, false);
				if ((UnityEngine.Object)gameObject3 != (UnityEngine.Object)null)
				{
					gameObject3.GetComponent<Pickupable>().OnStore(component6);
				}
			}
		}
		if (prefab.connections != 0)
		{
			PlaceUtilityConnection(gameObject, prefab, root_cell);
		}
		return gameObject;
	}

	public static void PlaceUtilityConnection(GameObject spawned, Prefab bc, int root_cell)
	{
		int cell = Grid.OffsetCell(root_cell, bc.location_x, bc.location_y);
		UtilityConnections connection = (UtilityConnections)bc.connections;
		switch (bc.id)
		{
		case "Wire":
		case "InsulatedWire":
		case "HighWattageWire":
			spawned.GetComponent<Wire>().SetFirstFrameCallback(delegate
			{
				Game.Instance.electricalConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.Refresh();
				}
			});
			break;
		case "GasConduit":
		case "InsulatedGasConduit":
			spawned.GetComponent<Conduit>().SetFirstFrameCallback(delegate
			{
				Game.Instance.gasConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component2 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					component2.Refresh();
				}
			});
			break;
		case "LiquidConduit":
		case "InsulatedLiquidConduit":
			spawned.GetComponent<Conduit>().SetFirstFrameCallback(delegate
			{
				Game.Instance.liquidConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component3 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
				{
					component3.Refresh();
				}
			});
			break;
		case "SolidConduit":
			spawned.GetComponent<SolidConduit>().SetFirstFrameCallback(delegate
			{
				Game.Instance.solidConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component4 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component4 != (UnityEngine.Object)null)
				{
					component4.Refresh();
				}
			});
			break;
		case "LogicWire":
			spawned.GetComponent<LogicWire>().SetFirstFrameCallback(delegate
			{
				Game.Instance.logicCircuitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component5 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component5 != (UnityEngine.Object)null)
				{
					component5.Refresh();
				}
			});
			break;
		case "TravelTube":
			spawned.GetComponent<TravelTube>().SetFirstFrameCallback(delegate
			{
				Game.Instance.travelTubeSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component6 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component6 != (UnityEngine.Object)null)
				{
					component6.Refresh();
				}
			});
			break;
		}
	}

	public static GameObject PlacePickupables(Prefab prefab, int root_cell)
	{
		int location_x = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, location_x, location_y)))
		{
			return null;
		}
		GameObject gameObject = Scenario.SpawnPrefab(root_cell, location_x, location_y, prefab.id, Grid.SceneLayer.Ore);
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Null prefab for " + prefab.id);
			return null;
		}
		gameObject.SetActive(true);
		if (prefab.units != 0f)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Units = prefab.units;
			component.Temperature = ((!(prefab.temperature > 0f)) ? component.Element.defaultValues.temperature : prefab.temperature);
			component.AddDisease(Db.Get().Diseases.GetIndex(prefab.diseaseName), prefab.diseaseCount, "TemplateLoader.PlacePickupables");
		}
		Rottable.Instance sMI = gameObject.GetSMI<Rottable.Instance>();
		if (sMI != null)
		{
			sMI.RotValue = prefab.rottable.rotAmount;
		}
		return gameObject;
	}

	public static GameObject PlaceOtherEntities(Prefab prefab, int root_cell)
	{
		int location_x = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, location_x, location_y)))
		{
			return null;
		}
		GameObject prefab2 = Assets.GetPrefab(new Tag(prefab.id));
		if ((UnityEngine.Object)prefab2 == (UnityEngine.Object)null)
		{
			return null;
		}
		Grid.SceneLayer scene_layer = Grid.SceneLayer.Front;
		KBatchedAnimController component = prefab2.GetComponent<KBatchedAnimController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			scene_layer = component.sceneLayer;
		}
		GameObject gameObject = Scenario.SpawnPrefab(root_cell, location_x, location_y, prefab.id, scene_layer);
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			Debug.LogWarning("Null prefab for " + prefab.id);
			return null;
		}
		gameObject.SetActive(true);
		if (prefab.amounts != null)
		{
			Prefab.template_amount_value[] amounts = prefab.amounts;
			foreach (Prefab.template_amount_value template_amount_value in amounts)
			{
				try
				{
					gameObject.GetAmounts().SetValue(template_amount_value.id, template_amount_value.value);
				}
				catch
				{
					Debug.LogWarning($"Entity {gameObject.GetProperName()} does not have amount with ID {template_amount_value.id}");
				}
			}
		}
		return gameObject;
	}

	public static GameObject PlaceElementalOres(Prefab prefab, int root_cell)
	{
		int location_x = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, location_x, location_y)))
		{
			return null;
		}
		Substance substance = ElementLoader.FindElementByHash(prefab.element).substance;
		int cell = Grid.OffsetCell(root_cell, location_x, location_y);
		Vector3 position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore);
		byte index = Db.Get().Diseases.GetIndex(prefab.diseaseName);
		if (prefab.temperature <= 0f)
		{
			Debug.LogWarning("Template trying to spawn zero temperature substance!");
			prefab.temperature = 300f;
		}
		return substance.SpawnResource(position, prefab.units, prefab.temperature, index, prefab.diseaseCount, false, false, false);
	}

	private static void BuildPhase3(int baseX, int baseY, System.Action callback)
	{
		if (template != null)
		{
			int root_cell = Grid.OffsetCell(0, baseX, baseY);
			foreach (BuildingComplete item in Components.BuildingCompletes.Items)
			{
				KAnimGraphTileVisualizer component = item.GetComponent<KAnimGraphTileVisualizer>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.Refresh();
				}
			}
			for (int i = 0; i < template.pickupables.Count; i++)
			{
				if (template.pickupables[i] != null && !(template.pickupables[i].id == string.Empty))
				{
					PlacePickupables(template.pickupables[i], root_cell);
				}
			}
			for (int j = 0; j < template.elementalOres.Count; j++)
			{
				if (template.elementalOres[j] != null && !(template.elementalOres[j].id == string.Empty))
				{
					PlaceElementalOres(template.elementalOres[j], root_cell);
				}
			}
		}
		callback?.Invoke();
	}

	private static void BuildPhase4(int baseX, int baseY, System.Action callback)
	{
		if (template != null)
		{
			int root_cell = Grid.OffsetCell(0, baseX, baseY);
			for (int i = 0; i < template.otherEntities.Count; i++)
			{
				if (template.otherEntities[i] != null && !(template.otherEntities[i].id == string.Empty))
				{
					PlaceOtherEntities(template.otherEntities[i], root_cell);
				}
			}
			template = null;
		}
		callback?.Invoke();
	}

	private static void ClearPickups(int baseX, int baseY, CellOffset[] template_as_offsets)
	{
		if ((UnityEngine.Object)SaveGame.Instance.worldGenSpawner != (UnityEngine.Object)null)
		{
			SaveGame.Instance.worldGenSpawner.ClearSpawnersInArea(new Vector2((float)baseX, (float)baseY), template_as_offsets);
		}
		foreach (Pickupable item in Components.Pickupables.Items)
		{
			if (Grid.IsCellOffsetOf(Grid.XYToCell(baseX, baseY), item.gameObject, template_as_offsets))
			{
				Util.KDestroyGameObject(item.gameObject);
			}
		}
	}

	private static void ClearEntities<T>(int rootX, int rootY, CellOffset[] TemplateOffsets) where T : KMonoBehaviour
	{
		T[] array = (T[])UnityEngine.Object.FindObjectsOfType(typeof(T));
		T[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			T val = array2[i];
			if (Grid.IsCellOffsetOf(Grid.PosToCell(val.gameObject), Grid.XYToCell(rootX, rootY), TemplateOffsets))
			{
				Util.KDestroyGameObject(val.gameObject);
			}
		}
	}

	private static void PlaceCells(int baseX, int baseY, TemplateContainer template, System.Action callback)
	{
		HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(callback, false));
		if (template == null)
		{
			Debug.LogError("Template Loader does not have template.");
		}
		for (int i = 0; i < template.cells.Count; i++)
		{
			int num = Grid.XYToCell(template.cells[i].location_x + baseX, template.cells[i].location_y + baseY);
			if (!Grid.IsValidCell(num))
			{
				Debug.LogError($"Trying to replace invalid cells cell{num} root{baseX}:{baseY} offset{template.cells[i].location_x}:{template.cells[i].location_y}");
			}
			SimHashes element = template.cells[i].element;
			float mass = template.cells[i].mass;
			float temperature = template.cells[i].temperature;
			byte index = Db.Get().Diseases.GetIndex(template.cells[i].diseaseName);
			int diseaseCount = template.cells[i].diseaseCount;
			SimMessages.ReplaceElement(num, element, CellEventLogger.Instance.TemplateLoader, mass, temperature, index, diseaseCount, handle.index);
			handle.index = -1;
		}
	}

	public static void ApplyGridProperties(int baseX, int baseY, TemplateContainer template)
	{
		for (int i = 0; i < template.cells.Count; i++)
		{
			int num = Grid.XYToCell(template.cells[i].location_x + baseX, template.cells[i].location_y + baseY);
			if (Grid.IsValidCell(num) && template.cells[i].preventFoWReveal)
			{
				Grid.PreventFogOfWarReveal[num] = true;
				Grid.Visible[num] = 0;
			}
		}
	}
}
