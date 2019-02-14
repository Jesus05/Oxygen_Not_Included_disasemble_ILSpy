using Klei;
using Klei.AI;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class BuildingDef : Def
{
	public float EnergyConsumptionWhenActive;

	public float GeneratorWattageRating;

	public float GeneratorBaseCapacity;

	public float MassForTemperatureModification;

	public float ExhaustKilowattsWhenActive;

	public float SelfHeatKilowattsWhenActive;

	public float BaseMeltingPoint;

	public float ConstructionTime;

	public float WorkTime;

	public float ThermalConductivity = 1f;

	public int WidthInCells;

	public int HeightInCells;

	public int HitPoints;

	public bool RequiresPowerInput;

	public bool RequiresPowerOutput;

	public bool UseWhitePowerOutputConnectorColour = false;

	public CellOffset ElectricalArrowOffset;

	public ConduitType InputConduitType = ConduitType.None;

	public ConduitType OutputConduitType = ConduitType.None;

	public bool ModifiesTemperature;

	public bool Floodable = true;

	public bool Disinfectable = true;

	public bool Entombable = true;

	public bool Replaceable = true;

	public bool Invincible = false;

	public bool Overheatable = true;

	public bool Repairable = true;

	public float OverheatTemperature = 348.15f;

	public float FatalHot = 533.15f;

	public bool Breakable;

	public bool ContinuouslyCheckFoundation;

	public bool IsFoundation;

	public bool DragBuild = false;

	public bool UseStructureTemperature = true;

	public Action HotKey = Action.NumActions;

	public CellOffset attachablePosition = new CellOffset(0, 0);

	public bool CanMove = false;

	public List<Tag> ReplacementTags;

	[NonSerialized]
	[HashedEnum]
	public HashedString ViewMode = OverlayModes.None.ID;

	public BuildLocationRule BuildLocationRule;

	public ObjectLayer ObjectLayer = ObjectLayer.Building;

	public ObjectLayer TileLayer = ObjectLayer.NumLayers;

	public ObjectLayer ReplacementLayer = ObjectLayer.NumLayers;

	public Vector3 placementPivot;

	public string DiseaseCellVisName;

	public string[] MaterialCategory;

	public string AudioCategory = "Metal";

	public string AudioSize = "medium";

	public float[] Mass;

	public bool Upgradeable;

	public float BaseTimeUntilRepair = 600f;

	public bool ShowInBuildMenu = true;

	public PermittedRotations PermittedRotations;

	public bool Deprecated;

	public CellOffset PowerInputOffset;

	public CellOffset PowerOutputOffset;

	public CellOffset UtilityInputOffset = new CellOffset(0, 1);

	public CellOffset UtilityOutputOffset = new CellOffset(1, 0);

	public Grid.SceneLayer SceneLayer = Grid.SceneLayer.Building;

	public Grid.SceneLayer ForegroundLayer = Grid.SceneLayer.BuildingFront;

	public string RequiredAttribute = "";

	public int RequiredAttributeLevel;

	public List<Descriptor> EffectDescription;

	public float MassTier;

	public float HeatTier;

	public float ConstructionTimeTier;

	public string PrimaryUse;

	public string SecondaryUse;

	public string PrimarySideEffect;

	public string SecondarySideEffect;

	public Recipe CraftRecipe;

	public Sprite UISprite;

	public bool isKAnimTile;

	public bool isUtility;

	public bool isSolidTile;

	public KAnimFile[] AnimFiles;

	public string DefaultAnimState = "off";

	public bool BlockTileIsTransparent = false;

	public TextureAtlas BlockTileAtlas;

	public TextureAtlas BlockTilePlaceAtlas;

	public TextureAtlas BlockTileShineAtlas;

	public Material BlockTileMaterial;

	public BlockTileDecorInfo DecorBlockTileInfo;

	public BlockTileDecorInfo DecorPlaceBlockTileInfo;

	public List<Klei.AI.Attribute> attributes = new List<Klei.AI.Attribute>();

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public Tag AttachmentSlotTag;

	public bool PreventIdleTraversalPastBuilding;

	public GameObject BuildingComplete;

	public GameObject BuildingPreview;

	public GameObject BuildingUnderConstruction;

	public CellOffset[] PlacementOffsets;

	public CellOffset[] ConstructionOffsetFilter;

	public static CellOffset[] ConstructionOffsetFilter_OneDown = new CellOffset[1]
	{
		new CellOffset(0, -1)
	};

	public float BaseDecor;

	public float BaseDecorRadius;

	public int BaseNoisePollution;

	public int BaseNoisePollutionRadius;

	public BuildingDef[] Enables = null;

	private static Dictionary<CellOffset, CellOffset[]> placementOffsetsCache = new Dictionary<CellOffset, CellOffset[]>();

	public override string Name => Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".NAME");

	public string Desc => Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".DESC");

	public string Flavor => "\"" + Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".FLAVOR") + "\"";

	public string Effect => Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".EFFECT");

	public bool IsTilePiece => TileLayer != ObjectLayer.NumLayers;

	public bool CanReplace(GameObject go)
	{
		if (ReplacementTags != null)
		{
			foreach (Tag replacementTag in ReplacementTags)
			{
				if (go.GetComponent<KPrefabID>().HasTag(replacementTag))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public GameObject Create(Vector3 pos, Storage resource_storage, IList<Tag> selected_elements, Recipe recipe, float temperature, GameObject obj)
	{
		SimUtil.DiseaseInfo a = SimUtil.DiseaseInfo.Invalid;
		if ((UnityEngine.Object)resource_storage != (UnityEngine.Object)null)
		{
			Recipe.Ingredient[] allIngredients = recipe.GetAllIngredients(selected_elements);
			if (allIngredients != null)
			{
				Recipe.Ingredient[] array = allIngredients;
				foreach (Recipe.Ingredient ingredient in array)
				{
					resource_storage.ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out SimUtil.DiseaseInfo disease_info, out float _);
					a = SimUtil.CalculateFinalDiseaseInfo(a, disease_info);
				}
			}
		}
		GameObject gameObject = GameUtil.KInstantiate(obj, pos, SceneLayer, null, 0);
		Element element = ElementLoader.GetElement(selected_elements[0]);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ElementID = element.id;
		component.Temperature = temperature;
		component.AddDisease(a.idx, a.count, "BuildingDef.Create");
		gameObject.name = obj.name;
		gameObject.SetActive(true);
		return gameObject;
	}

	public GameObject Build(int cell, Orientation orientation, Storage resource_storage, IList<Tag> selected_elements, float temperature, bool playsound = true)
	{
		Vector3 pos = Grid.CellToPosCBC(cell, SceneLayer);
		GameObject gameObject = Create(pos, resource_storage, selected_elements, CraftRecipe, temperature, BuildingComplete);
		Rotatable component = gameObject.GetComponent<Rotatable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetOrientation(orientation);
		}
		MarkArea(cell, orientation, ObjectLayer, gameObject);
		if (IsTilePiece)
		{
			MarkArea(cell, orientation, TileLayer, gameObject);
			RunOnArea(cell, orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, TileLayer, ReplacementLayer);
			});
		}
		string sound = GlobalAssets.GetSound("Finish_Building_" + AudioSize, false);
		if (playsound && sound != null)
		{
			KMonoBehaviour.PlaySound3DAtLocation(sound, gameObject.transform.GetPosition());
		}
		Deconstructable component2 = gameObject.GetComponent<Deconstructable>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			component2.constructionElements = new Tag[selected_elements.Count];
			for (int i = 0; i < selected_elements.Count; i++)
			{
				component2.constructionElements[i] = selected_elements[i];
			}
		}
		Game.Instance.Trigger(-1661515756, gameObject);
		return gameObject;
	}

	public GameObject TryPlace(GameObject src_go, Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		GameObject result = null;
		if (IsValidPlaceLocation(src_go, pos, orientation, false, out string _))
		{
			result = Instantiate(pos, orientation, selected_elements, layer);
		}
		return result;
	}

	public GameObject TryReplaceTile(GameObject src_go, Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		GameObject result = null;
		if (IsValidPlaceLocation(src_go, pos, orientation, true, out string _))
		{
			Constructable component = BuildingUnderConstruction.GetComponent<Constructable>();
			component.IsReplacementTile = true;
			result = Instantiate(pos, orientation, selected_elements, layer);
			component.IsReplacementTile = false;
		}
		return result;
	}

	public GameObject Instantiate(Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		float num = -0.15f;
		pos.z += num;
		GameObject buildingUnderConstruction = BuildingUnderConstruction;
		Vector3 position = pos;
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		GameObject gameObject = GameUtil.KInstantiate(buildingUnderConstruction, position, sceneLayer, null, layer);
		Element element = ElementLoader.GetElement(selected_elements[0]);
		gameObject.GetComponent<PrimaryElement>().ElementID = element.id;
		gameObject.GetComponent<Constructable>().SelectedElementsTags = selected_elements;
		gameObject.SetActive(true);
		return gameObject;
	}

	private bool IsAreaClear(GameObject source_go, int cell, Orientation orientation, ObjectLayer layer, ObjectLayer tile_layer, bool replace_tile, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset offset = PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			if (!Grid.IsCellOffsetValid(cell, rotatedCellOffset))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			if (Grid.Element[num].id == SimHashes.Unobtanium)
			{
				fail_reason = null;
				flag = false;
				break;
			}
			bool flag2 = BuildLocationRule == BuildLocationRule.LogicBridge || BuildLocationRule == BuildLocationRule.Conduit || BuildLocationRule == BuildLocationRule.WireBridge;
			if (!replace_tile && !flag2)
			{
				GameObject gameObject = Grid.Objects[num, (int)layer];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					if ((UnityEngine.Object)gameObject.GetComponent<Wire>() == (UnityEngine.Object)null || (UnityEngine.Object)BuildingComplete.GetComponent<Wire>() == (UnityEngine.Object)null)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
						flag = false;
					}
					break;
				}
				if (tile_layer != ObjectLayer.NumLayers && (UnityEngine.Object)Grid.Objects[num, (int)tile_layer] != (UnityEngine.Object)null && (UnityEngine.Object)Grid.Objects[num, (int)tile_layer].GetComponent<BuildingPreview>() == (UnityEngine.Object)null)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
					flag = false;
					break;
				}
			}
			if (layer == ObjectLayer.Building && AttachmentSlotTag != GameTags.Rocket)
			{
				GameObject x = Grid.Objects[num, 38];
				if ((UnityEngine.Object)x != (UnityEngine.Object)null)
				{
					if ((UnityEngine.Object)BuildingComplete.GetComponent<Wire>() == (UnityEngine.Object)null)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
						flag = false;
					}
					break;
				}
			}
			if (layer == ObjectLayer.Gantry)
			{
				bool flag3 = false;
				for (int j = 0; j < Gantry.TileOffsets.Length; j++)
				{
					CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(Gantry.TileOffsets[j], orientation);
					flag3 |= (rotatedCellOffset2 == rotatedCellOffset);
				}
				if (flag3 && !IsValidTileLocation(source_go, num, orientation, layer, ref fail_reason))
				{
					flag = false;
					break;
				}
				GameObject gameObject2 = Grid.Objects[num, 1];
				if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null && (UnityEngine.Object)gameObject2.GetComponent<BuildingPreview>() == (UnityEngine.Object)null)
				{
					Building component = gameObject2.GetComponent<Building>();
					if (flag3 || (UnityEngine.Object)component == (UnityEngine.Object)null || component.Def.AttachmentSlotTag != GameTags.Rocket)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
						flag = false;
						break;
					}
				}
			}
			if (BuildLocationRule == BuildLocationRule.Tile)
			{
				if (!replace_tile && !IsValidTileLocation(source_go, num, orientation, layer, ref fail_reason))
				{
					flag = false;
					break;
				}
			}
			else if (BuildLocationRule == BuildLocationRule.OnFloorOverSpace && World.Instance.zoneRenderData.GetSubWorldZoneType(num) != SubWorld.ZoneType.Space)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
				flag = false;
				break;
			}
		}
		if (flag)
		{
			switch (BuildLocationRule)
			{
			case BuildLocationRule.WireBridge:
				return IsValidWireBridgeLocation(source_go, cell, orientation, out fail_reason);
			case BuildLocationRule.HighWattBridgeTile:
				flag = ((replace_tile || IsValidTileLocation(source_go, cell, orientation, layer, ref fail_reason)) && IsValidHighWattBridgeLocation(source_go, cell, orientation, out fail_reason));
				break;
			case BuildLocationRule.BuildingAttachPoint:
				flag = false;
				for (int k = 0; k < Components.BuildingAttachPoints.Count; k++)
				{
					if (flag)
					{
						break;
					}
					for (int l = 0; l < Components.BuildingAttachPoints[k].points.Length; l++)
					{
						BuildingAttachPoint buildingAttachPoint = Components.BuildingAttachPoints[k];
						if (buildingAttachPoint.AcceptsAttachment(AttachmentSlotTag, Grid.OffsetCell(cell, attachablePosition)))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, AttachmentSlotTag);
				}
				break;
			case BuildLocationRule.NotInTiles:
			{
				GameObject x2 = Grid.Objects[cell, 9];
				if ((UnityEngine.Object)x2 != (UnityEngine.Object)null && (UnityEngine.Object)x2 != (UnityEngine.Object)source_go)
				{
					flag = false;
				}
				else if (Grid.HasDoor[cell])
				{
					flag = false;
				}
				else
				{
					GameObject gameObject3 = Grid.Objects[cell, (int)ObjectLayer];
					if ((UnityEngine.Object)gameObject3 != (UnityEngine.Object)null)
					{
						if (ReplacementLayer == ObjectLayer.NumLayers)
						{
							if ((UnityEngine.Object)gameObject3 != (UnityEngine.Object)source_go)
							{
								flag = false;
							}
						}
						else
						{
							Building component2 = gameObject3.GetComponent<Building>();
							if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.Def.ReplacementLayer != ReplacementLayer)
							{
								flag = false;
							}
						}
					}
				}
				if (!flag)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
				}
				break;
			}
			}
			return flag && ArePowerPortsInValidPositions(source_go, cell, orientation, out fail_reason) && AreConduitPortsInValidPositions(source_go, cell, orientation, out fail_reason) && AreLogicPortsInValidPositions(source_go, cell, out fail_reason);
		}
		return false;
	}

	private bool IsValidTileLocation(GameObject source_go, int cell, Orientation orientation, ObjectLayer layer, ref string fail_reason)
	{
		GameObject gameObject = Grid.Objects[cell, 27];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject != (UnityEngine.Object)source_go)
		{
			Building component = gameObject.GetComponent<Building>();
			if (component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
				return false;
			}
		}
		gameObject = Grid.Objects[cell, 29];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject != (UnityEngine.Object)source_go)
		{
			Building component2 = gameObject.GetComponent<Building>();
			if (component2.Def.BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
				return false;
			}
		}
		gameObject = Grid.Objects[cell, 2];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject != (UnityEngine.Object)source_go)
		{
			Building component3 = gameObject.GetComponent<Building>();
			if ((UnityEngine.Object)component3 != (UnityEngine.Object)null && component3.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_BACK_WALL;
				return false;
			}
		}
		return true;
	}

	public void RunOnArea(int cell, Orientation orientation, Action<int> callback)
	{
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset offset = PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			int obj = Grid.OffsetCell(cell, rotatedCellOffset);
			callback(obj);
		}
	}

	public void MarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		if (BuildLocationRule != BuildLocationRule.Conduit && BuildLocationRule != BuildLocationRule.WireBridge && BuildLocationRule != BuildLocationRule.LogicBridge)
		{
			for (int i = 0; i < PlacementOffsets.Length; i++)
			{
				CellOffset offset = PlacementOffsets[i];
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
				int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
				Grid.Objects[cell2, (int)layer] = go;
			}
		}
		if (InputConduitType != 0)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(UtilityInputOffset, orientation);
			int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(InputConduitType);
			MarkOverlappingPorts(Grid.Objects[cell3, (int)objectLayerForConduitType], go);
			Grid.Objects[cell3, (int)objectLayerForConduitType] = go;
		}
		if (OutputConduitType != 0)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(UtilityOutputOffset, orientation);
			int cell4 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(OutputConduitType);
			MarkOverlappingPorts(Grid.Objects[cell4, (int)objectLayerForConduitType2], go);
			Grid.Objects[cell4, (int)objectLayerForConduitType2] = go;
		}
		if (RequiresPowerInput)
		{
			CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(PowerInputOffset, orientation);
			int cell5 = Grid.OffsetCell(cell, rotatedCellOffset4);
			MarkOverlappingPorts(Grid.Objects[cell5, 29], go);
			Grid.Objects[cell5, 29] = go;
		}
		if (RequiresPowerOutput || GeneratorWattageRating > 0f)
		{
			CellOffset rotatedCellOffset5 = Rotatable.GetRotatedCellOffset(PowerOutputOffset, orientation);
			int cell6 = Grid.OffsetCell(cell, rotatedCellOffset5);
			MarkOverlappingPorts(Grid.Objects[cell6, 29], go);
			Grid.Objects[cell6, 29] = go;
		}
		if (BuildLocationRule == BuildLocationRule.WireBridge || BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			UtilityNetworkLink component = go.GetComponent<UtilityNetworkLink>();
			component.GetCells(cell, orientation, out int linked_cell, out int linked_cell2);
			MarkOverlappingPorts(Grid.Objects[linked_cell, 29], go);
			MarkOverlappingPorts(Grid.Objects[linked_cell2, 29], go);
			Grid.Objects[linked_cell, 29] = go;
			Grid.Objects[linked_cell2, 29] = go;
		}
		if (BuildLocationRule == BuildLocationRule.LogicBridge)
		{
			LogicPorts component2 = go.GetComponent<LogicPorts>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.inputPortInfo != null)
			{
				LogicPorts.Port[] inputPortInfo = component2.inputPortInfo;
				for (int j = 0; j < inputPortInfo.Length; j++)
				{
					LogicPorts.Port port = inputPortInfo[j];
					CellOffset rotatedCellOffset6 = Rotatable.GetRotatedCellOffset(port.cellOffset, orientation);
					int cell7 = Grid.OffsetCell(cell, rotatedCellOffset6);
					MarkOverlappingPorts(Grid.Objects[cell7, (int)layer], go);
					Grid.Objects[cell7, (int)layer] = go;
				}
			}
		}
		ISecondaryInput component3 = BuildingComplete.GetComponent<ISecondaryInput>();
		if (component3 != null)
		{
			ConduitType secondaryConduitType = component3.GetSecondaryConduitType();
			ObjectLayer objectLayerForConduitType3 = Grid.GetObjectLayerForConduitType(secondaryConduitType);
			CellOffset rotatedCellOffset7 = Rotatable.GetRotatedCellOffset(component3.GetSecondaryConduitOffset(), orientation);
			int cell8 = Grid.OffsetCell(cell, rotatedCellOffset7);
			MarkOverlappingPorts(Grid.Objects[cell8, (int)objectLayerForConduitType3], go);
			Grid.Objects[cell8, (int)objectLayerForConduitType3] = go;
		}
		ISecondaryOutput component4 = BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component4 != null)
		{
			ConduitType secondaryConduitType2 = component4.GetSecondaryConduitType();
			ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(secondaryConduitType2);
			CellOffset rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(component4.GetSecondaryConduitOffset(), orientation);
			int cell9 = Grid.OffsetCell(cell, rotatedCellOffset8);
			MarkOverlappingPorts(Grid.Objects[cell9, (int)objectLayerForConduitType4], go);
			Grid.Objects[cell9, (int)objectLayerForConduitType4] = go;
		}
	}

	public void MarkOverlappingPorts(GameObject existing, GameObject replaced)
	{
		if (!((UnityEngine.Object)existing == (UnityEngine.Object)null) && (UnityEngine.Object)existing != (UnityEngine.Object)replaced)
		{
			existing.AddTag(GameTags.HasInvalidPorts);
		}
	}

	public void UnmarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset offset = PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
			if ((UnityEngine.Object)Grid.Objects[cell2, (int)layer] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell2, (int)layer] = null;
			}
		}
		if (InputConduitType != 0)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(UtilityInputOffset, orientation);
			int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(InputConduitType);
			if ((UnityEngine.Object)Grid.Objects[cell3, (int)objectLayerForConduitType] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell3, (int)objectLayerForConduitType] = null;
			}
		}
		if (OutputConduitType != 0)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(UtilityOutputOffset, orientation);
			int cell4 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(OutputConduitType);
			if ((UnityEngine.Object)Grid.Objects[cell4, (int)objectLayerForConduitType2] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell4, (int)objectLayerForConduitType2] = null;
			}
		}
		if (RequiresPowerInput)
		{
			CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(PowerInputOffset, orientation);
			int cell5 = Grid.OffsetCell(cell, rotatedCellOffset4);
			if ((UnityEngine.Object)Grid.Objects[cell5, 29] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell5, 29] = null;
			}
		}
		if (RequiresPowerOutput || GeneratorWattageRating > 0f)
		{
			CellOffset rotatedCellOffset5 = Rotatable.GetRotatedCellOffset(PowerOutputOffset, orientation);
			int cell6 = Grid.OffsetCell(cell, rotatedCellOffset5);
			if ((UnityEngine.Object)Grid.Objects[cell6, 29] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell6, 29] = null;
			}
		}
		if (BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			UtilityNetworkLink component = go.GetComponent<UtilityNetworkLink>();
			component.GetCells(cell, orientation, out int linked_cell, out int linked_cell2);
			if ((UnityEngine.Object)Grid.Objects[linked_cell, 29] == (UnityEngine.Object)go)
			{
				Grid.Objects[linked_cell, 29] = null;
			}
			if ((UnityEngine.Object)Grid.Objects[linked_cell2, 29] == (UnityEngine.Object)go)
			{
				Grid.Objects[linked_cell2, 29] = null;
			}
		}
		ISecondaryInput component2 = BuildingComplete.GetComponent<ISecondaryInput>();
		if (component2 != null)
		{
			ConduitType secondaryConduitType = component2.GetSecondaryConduitType();
			ObjectLayer objectLayerForConduitType3 = Grid.GetObjectLayerForConduitType(secondaryConduitType);
			CellOffset rotatedCellOffset6 = Rotatable.GetRotatedCellOffset(component2.GetSecondaryConduitOffset(), orientation);
			int cell7 = Grid.OffsetCell(cell, rotatedCellOffset6);
			if ((UnityEngine.Object)Grid.Objects[cell7, (int)objectLayerForConduitType3] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell7, (int)objectLayerForConduitType3] = null;
			}
		}
		ISecondaryOutput component3 = BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component3 != null)
		{
			ConduitType secondaryConduitType2 = component3.GetSecondaryConduitType();
			ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(secondaryConduitType2);
			CellOffset rotatedCellOffset7 = Rotatable.GetRotatedCellOffset(component3.GetSecondaryConduitOffset(), orientation);
			int cell8 = Grid.OffsetCell(cell, rotatedCellOffset7);
			if ((UnityEngine.Object)Grid.Objects[cell8, (int)objectLayerForConduitType4] == (UnityEngine.Object)go)
			{
				Grid.Objects[cell8, (int)objectLayerForConduitType4] = null;
			}
		}
	}

	public int GetBuildingCell(int cell)
	{
		return cell + (WidthInCells - 1) / 2;
	}

	public Vector3 GetVisualizerOffset()
	{
		return Vector3.right * (0.5f * (float)((WidthInCells + 1) % 2));
	}

	public bool IsValidPlaceLocation(GameObject go, Vector3 pos, Orientation orientation, out string fail_reason)
	{
		int cell = Grid.PosToCell(pos);
		return IsValidPlaceLocation(go, cell, orientation, false, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject go, Vector3 pos, Orientation orientation, bool replace_tile, out string fail_reason)
	{
		int cell = Grid.PosToCell(pos);
		return IsValidPlaceLocation(go, cell, orientation, replace_tile, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject go, int cell, Orientation orientation, out string fail_reason)
	{
		if (Grid.IsValidBuildingCell(cell))
		{
			if (BuildLocationRule == BuildLocationRule.OnWall)
			{
				if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
					return false;
				}
			}
			else if (BuildLocationRule == BuildLocationRule.InCorner && !CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
				return false;
			}
			return IsAreaClear(go, cell, orientation, ObjectLayer, TileLayer, false, out fail_reason);
		}
		fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
		return false;
	}

	public bool IsValidPlaceLocation(GameObject go, int cell, Orientation orientation, bool replace_tile, out string fail_reason)
	{
		if (Grid.IsValidBuildingCell(cell))
		{
			if (BuildLocationRule == BuildLocationRule.OnWall)
			{
				if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
					return false;
				}
			}
			else if (BuildLocationRule == BuildLocationRule.InCorner && !CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
				return false;
			}
			return IsAreaClear(go, cell, orientation, ObjectLayer, TileLayer, replace_tile, out fail_reason);
		}
		fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
		return false;
	}

	public bool IsValidReplaceLocation(Vector3 pos, Orientation orientation, ObjectLayer replace_layer, ObjectLayer obj_layer)
	{
		if (replace_layer != ObjectLayer.NumLayers)
		{
			bool result = true;
			int cell = Grid.PosToCell(pos);
			for (int i = 0; i < PlacementOffsets.Length; i++)
			{
				CellOffset offset = PlacementOffsets[i];
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
				int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
				if (!Grid.IsValidBuildingCell(cell2))
				{
					return false;
				}
				if ((UnityEngine.Object)Grid.Objects[cell2, (int)obj_layer] == (UnityEngine.Object)null || (UnityEngine.Object)Grid.Objects[cell2, (int)replace_layer] != (UnityEngine.Object)null)
				{
					result = false;
					break;
				}
			}
			return result;
		}
		return false;
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation)
	{
		string reason = "";
		return IsValidBuildLocation(source_go, pos, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation, out string reason)
	{
		int cell = Grid.PosToCell(pos);
		return IsValidBuildLocation(source_go, cell, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		if (Grid.IsValidBuildingCell(cell))
		{
			if (IsAreaValid(cell, orientation, out fail_reason))
			{
				bool flag = true;
				fail_reason = null;
				switch (BuildLocationRule)
				{
				case BuildLocationRule.OnFloor:
				case BuildLocationRule.OnCeiling:
				case BuildLocationRule.OnFoundationRotatable:
					if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
					}
					break;
				case BuildLocationRule.OnWall:
					if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
					}
					break;
				case BuildLocationRule.InCorner:
					if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
					}
					break;
				case BuildLocationRule.OnFloorOverSpace:
					if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
					}
					else if (!AreAllCellsValid(cell, orientation, WidthInCells, HeightInCells, (int check_cell) => World.Instance.zoneRenderData.GetSubWorldZoneType(check_cell) == SubWorld.ZoneType.Space))
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
					}
					break;
				case BuildLocationRule.NotInTiles:
				{
					GameObject x = Grid.Objects[cell, 9];
					flag = (((UnityEngine.Object)x == (UnityEngine.Object)null || (UnityEngine.Object)x == (UnityEngine.Object)source_go) && !Grid.HasDoor[cell]);
					if (flag)
					{
						GameObject gameObject = Grid.Objects[cell, (int)ObjectLayer];
						if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
						{
							if (ReplacementLayer == ObjectLayer.NumLayers)
							{
								flag = (flag && ((UnityEngine.Object)gameObject == (UnityEngine.Object)null || (UnityEngine.Object)gameObject == (UnityEngine.Object)source_go));
							}
							else
							{
								Building component = gameObject.GetComponent<Building>();
								flag = ((UnityEngine.Object)component == (UnityEngine.Object)null || component.Def.ReplacementLayer == ReplacementLayer);
							}
						}
					}
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
					break;
				}
				case BuildLocationRule.Tile:
				{
					flag = true;
					GameObject gameObject2 = Grid.Objects[cell, 27];
					if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
					{
						Building component2 = gameObject2.GetComponent<Building>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
						{
							flag = false;
						}
					}
					gameObject2 = Grid.Objects[cell, 2];
					if ((UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
					{
						Building component3 = gameObject2.GetComponent<Building>();
						if ((UnityEngine.Object)component3 != (UnityEngine.Object)null && component3.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
						{
							flag = false;
						}
					}
					break;
				}
				case BuildLocationRule.BuildingAttachPoint:
					flag = false;
					for (int k = 0; k < Components.BuildingAttachPoints.Count; k++)
					{
						if (flag)
						{
							break;
						}
						for (int l = 0; l < Components.BuildingAttachPoints[k].points.Length; l++)
						{
							BuildingAttachPoint buildingAttachPoint2 = Components.BuildingAttachPoints[k];
							if (buildingAttachPoint2.AcceptsAttachment(AttachmentSlotTag, Grid.OffsetCell(cell, attachablePosition)))
							{
								flag = true;
								break;
							}
						}
					}
					fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, AttachmentSlotTag);
					break;
				case BuildLocationRule.OnFloorOrBuildingAttachPoint:
					flag = false;
					if (!CheckFoundation(cell, orientation, BuildLocationRule.OnFloor, WidthInCells, HeightInCells))
					{
						flag = false;
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR_OR_ATTACHPOINT;
						if (!flag)
						{
							for (int i = 0; i < Components.BuildingAttachPoints.Count; i++)
							{
								if (flag)
								{
									break;
								}
								for (int j = 0; j < Components.BuildingAttachPoints[i].points.Length; j++)
								{
									BuildingAttachPoint buildingAttachPoint = Components.BuildingAttachPoints[i];
									if (buildingAttachPoint.AcceptsAttachment(AttachmentSlotTag, Grid.OffsetCell(cell, attachablePosition)))
									{
										flag = true;
										break;
									}
								}
							}
							fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR_OR_ATTACHPOINT, AttachmentSlotTag);
						}
					}
					else
					{
						flag = true;
					}
					break;
				case BuildLocationRule.Anywhere:
				case BuildLocationRule.Conduit:
					flag = true;
					break;
				}
				return flag && ArePowerPortsInValidPositions(source_go, cell, orientation, out fail_reason) && AreConduitPortsInValidPositions(source_go, cell, orientation, out fail_reason);
			}
			return false;
		}
		fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
		return false;
	}

	private bool IsAreaValid(int cell, Orientation orientation, out string fail_reason)
	{
		bool result = true;
		fail_reason = null;
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset offset = PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			if (!Grid.IsCellOffsetValid(cell, rotatedCellOffset))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				result = false;
				break;
			}
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				result = false;
				break;
			}
			if (Grid.Element[num].id == SimHashes.Unobtanium)
			{
				fail_reason = null;
				result = false;
				break;
			}
		}
		return result;
	}

	private bool ArePowerPortsInValidPositions(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		fail_reason = null;
		if (!((UnityEngine.Object)source_go == (UnityEngine.Object)null))
		{
			if (RequiresPowerInput)
			{
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PowerInputOffset, orientation);
				int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
				GameObject x = Grid.Objects[cell2, 29];
				if ((UnityEngine.Object)x != (UnityEngine.Object)null && (UnityEngine.Object)x != (UnityEngine.Object)source_go)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
					return false;
				}
			}
			if (RequiresPowerOutput || GeneratorWattageRating > 0f)
			{
				CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(PowerOutputOffset, orientation);
				int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
				GameObject x2 = Grid.Objects[cell3, 29];
				if ((UnityEngine.Object)x2 != (UnityEngine.Object)null && (UnityEngine.Object)x2 != (UnityEngine.Object)source_go)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
					return false;
				}
			}
			return true;
		}
		return true;
	}

	private bool AreConduitPortsInValidPositions(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		fail_reason = null;
		if (!((UnityEngine.Object)source_go == (UnityEngine.Object)null))
		{
			bool flag = true;
			if (InputConduitType != 0)
			{
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(UtilityInputOffset, orientation);
				int utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);
				flag = IsValidConduitConnection(source_go, InputConduitType, utility_cell, ref fail_reason);
			}
			if (flag && OutputConduitType != 0)
			{
				CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(UtilityOutputOffset, orientation);
				int utility_cell2 = Grid.OffsetCell(cell, rotatedCellOffset2);
				flag = IsValidConduitConnection(source_go, OutputConduitType, utility_cell2, ref fail_reason);
			}
			Building component = source_go.GetComponent<Building>();
			if (flag && (bool)component)
			{
				ISecondaryInput component2 = component.Def.BuildingComplete.GetComponent<ISecondaryInput>();
				if (component2 != null)
				{
					ConduitType secondaryConduitType = component2.GetSecondaryConduitType();
					CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(component2.GetSecondaryConduitOffset(), orientation);
					int utility_cell3 = Grid.OffsetCell(cell, rotatedCellOffset3);
					flag = IsValidConduitConnection(source_go, secondaryConduitType, utility_cell3, ref fail_reason);
				}
			}
			if (flag)
			{
				ISecondaryOutput component3 = component.Def.BuildingComplete.GetComponent<ISecondaryOutput>();
				if (component3 != null)
				{
					ConduitType secondaryConduitType2 = component3.GetSecondaryConduitType();
					CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(component3.GetSecondaryConduitOffset(), orientation);
					int utility_cell4 = Grid.OffsetCell(cell, rotatedCellOffset4);
					flag = IsValidConduitConnection(source_go, secondaryConduitType2, utility_cell4, ref fail_reason);
				}
			}
			return flag;
		}
		return true;
	}

	private bool IsValidWireBridgeLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		UtilityNetworkLink component = source_go.GetComponent<UtilityNetworkLink>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.GetCells(out int linked_cell, out int linked_cell2);
			if ((UnityEngine.Object)Grid.Objects[linked_cell, 29] != (UnityEngine.Object)null || (UnityEngine.Object)Grid.Objects[linked_cell2, 29] != (UnityEngine.Object)null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		fail_reason = null;
		return true;
	}

	private bool IsValidHighWattBridgeLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		UtilityNetworkLink component = source_go.GetComponent<UtilityNetworkLink>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			if (!component.AreCellsValid(cell, orientation))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				return false;
			}
			component.GetCells(out int linked_cell, out int linked_cell2);
			if ((UnityEngine.Object)Grid.Objects[linked_cell, 29] != (UnityEngine.Object)null || (UnityEngine.Object)Grid.Objects[linked_cell2, 29] != (UnityEngine.Object)null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
			if ((UnityEngine.Object)Grid.Objects[linked_cell, 9] != (UnityEngine.Object)null || (UnityEngine.Object)Grid.Objects[linked_cell2, 9] != (UnityEngine.Object)null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
				return false;
			}
			if (Grid.HasDoor[linked_cell] || Grid.HasDoor[linked_cell2])
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
				return false;
			}
			GameObject gameObject = Grid.Objects[linked_cell, 1];
			GameObject gameObject2 = Grid.Objects[linked_cell2, 1];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null || (UnityEngine.Object)gameObject2 != (UnityEngine.Object)null)
			{
				BuildingUnderConstruction buildingUnderConstruction = (!(bool)gameObject) ? null : gameObject.GetComponent<BuildingUnderConstruction>();
				BuildingUnderConstruction buildingUnderConstruction2 = (!(bool)gameObject2) ? null : gameObject2.GetComponent<BuildingUnderConstruction>();
				if (((bool)buildingUnderConstruction && (bool)buildingUnderConstruction.Def.BuildingComplete.GetComponent<Door>()) || ((bool)buildingUnderConstruction2 && (bool)buildingUnderConstruction2.Def.BuildingComplete.GetComponent<Door>()))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
					return false;
				}
			}
		}
		fail_reason = null;
		return true;
	}

	private bool AreLogicPortsInValidPositions(GameObject source_go, int cell, out string fail_reason)
	{
		fail_reason = null;
		if (!((UnityEngine.Object)source_go == (UnityEngine.Object)null))
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			ReadOnlyCollection<ILogicUIElement> visElements = logicCircuitManager.GetVisElements();
			LogicPorts component = source_go.GetComponent<LogicPorts>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.HackRefreshVisualizers();
				if (DoLogicPortsConflict(component.inputPorts, visElements) || DoLogicPortsConflict(component.outputPorts, visElements))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
					return false;
				}
			}
			else
			{
				LogicGateBase component2 = source_go.GetComponent<LogicGateBase>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && (IsLogicPortObstructed(component2.InputCellOne, visElements) || IsLogicPortObstructed(component2.OutputCell, visElements) || (component2.RequiresTwoInputs && IsLogicPortObstructed(component2.InputCellTwo, visElements))))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
					return false;
				}
			}
			return true;
		}
		return true;
	}

	private bool DoLogicPortsConflict(IList<ILogicUIElement> ports_a, IList<ILogicUIElement> ports_b)
	{
		if (ports_a != null && ports_b != null)
		{
			foreach (ILogicUIElement item in ports_a)
			{
				int logicUICell = item.GetLogicUICell();
				foreach (ILogicUIElement item2 in ports_b)
				{
					if (item != item2 && logicUICell == item2.GetLogicUICell())
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	private bool IsLogicPortObstructed(int cell, IList<ILogicUIElement> ports)
	{
		int num = 0;
		foreach (ILogicUIElement port in ports)
		{
			if (port.GetLogicUICell() == cell)
			{
				num++;
			}
		}
		return num > 0;
	}

	private bool IsValidConduitConnection(GameObject source_go, ConduitType conduit_type, int utility_cell, ref string fail_reason)
	{
		bool result = true;
		switch (conduit_type)
		{
		case ConduitType.Gas:
		{
			GameObject x3 = Grid.Objects[utility_cell, 15];
			if ((UnityEngine.Object)x3 != (UnityEngine.Object)null && (UnityEngine.Object)x3 != (UnityEngine.Object)source_go)
			{
				result = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_GASPORTS_OVERLAP;
			}
			break;
		}
		case ConduitType.Liquid:
		{
			GameObject x2 = Grid.Objects[utility_cell, 19];
			if ((UnityEngine.Object)x2 != (UnityEngine.Object)null && (UnityEngine.Object)x2 != (UnityEngine.Object)source_go)
			{
				result = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LIQUIDPORTS_OVERLAP;
			}
			break;
		}
		case ConduitType.Solid:
		{
			GameObject x = Grid.Objects[utility_cell, 23];
			if ((UnityEngine.Object)x != (UnityEngine.Object)null && (UnityEngine.Object)x != (UnityEngine.Object)source_go)
			{
				result = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SOLIDPORTS_OVERLAP;
			}
			break;
		}
		}
		return result;
	}

	public static int GetXOffset(int width)
	{
		return -(width - 1) / 2;
	}

	public static bool CheckFoundation(int cell, Orientation orientation, BuildLocationRule location_rule, int width, int height)
	{
		switch (location_rule)
		{
		case BuildLocationRule.OnWall:
			return CheckWallFoundation(cell, width, height, true) || CheckWallFoundation(cell, width, height, false);
		case BuildLocationRule.InCorner:
			return CheckBaseFoundation(cell, orientation, BuildLocationRule.OnCeiling, width, height) && CheckWallFoundation(cell, width, height, orientation != Orientation.FlipH);
		default:
			return CheckBaseFoundation(cell, orientation, location_rule, width, height);
		}
	}

	public static bool CheckBaseFoundation(int cell, Orientation orientation, BuildLocationRule location_rule, int width, int height)
	{
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		for (int i = num; i <= num2; i++)
		{
			CellOffset offset = (location_rule != BuildLocationRule.OnCeiling) ? new CellOffset(i, -1) : new CellOffset(i, height);
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num3) || !Grid.Solid[num3])
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckWallFoundation(int cell, int width, int height, bool leftWall)
	{
		int num = 0;
		for (int i = num; i <= height; i++)
		{
			CellOffset offset = new CellOffset((!leftWall) ? (width / 2 + 1) : (-(width - 1) / 2 - 1), i);
			int num2 = Grid.OffsetCell(cell, offset);
			if (!Grid.IsValidBuildingCell(num2) || !Grid.Solid[num2])
			{
				return false;
			}
		}
		return true;
	}

	public static bool AreAllCellsValid(int base_cell, Orientation orientation, int width, int height, Func<int, bool> valid_cell_check)
	{
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		if (orientation == Orientation.FlipH)
		{
			int num3 = num;
			num = -num2;
			num2 = -num3;
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				int arg = Grid.OffsetCell(base_cell, j, i);
				if (!valid_cell_check(arg))
				{
					return false;
				}
			}
		}
		return true;
	}

	public Sprite GetUISprite(string animName = "ui", bool centered = false)
	{
		return Def.GetUISpriteFromMultiObjectAnim(AnimFiles[0], animName, centered);
	}

	public void GenerateOffsets()
	{
		GenerateOffsets(WidthInCells, HeightInCells);
	}

	public void GenerateOffsets(int width, int height)
	{
		if (!placementOffsetsCache.TryGetValue(new CellOffset(width, height), out PlacementOffsets))
		{
			int num = width / 2;
			int num2 = num - width + 1;
			PlacementOffsets = new CellOffset[width * height];
			for (int i = 0; i != height; i++)
			{
				int num3 = i * width;
				for (int j = 0; j != width; j++)
				{
					int num4 = num3 + j;
					PlacementOffsets[num4].x = j + num2;
					PlacementOffsets[num4].y = i;
				}
			}
			placementOffsetsCache.Add(new CellOffset(width, height), PlacementOffsets);
		}
	}

	public void PostProcess()
	{
		string name = BuildingComplete.PrefabID().Name;
		string name2 = Name;
		CraftRecipe = new Recipe(name, 1f, (SimHashes)0, name2, null, 0);
		CraftRecipe.Icon = UISprite;
		for (int i = 0; i < MaterialCategory.Length; i++)
		{
			Recipe.Ingredient item = new Recipe.Ingredient(MaterialCategory[i], (float)(int)Mass[i]);
			CraftRecipe.Ingredients.Add(item);
		}
		if ((UnityEngine.Object)DecorBlockTileInfo != (UnityEngine.Object)null)
		{
			DecorBlockTileInfo.PostProcess();
		}
		if ((UnityEngine.Object)DecorPlaceBlockTileInfo != (UnityEngine.Object)null)
		{
			DecorPlaceBlockTileInfo.PostProcess();
		}
		if (!Deprecated)
		{
			Db.Get().TechItems.AddTechItem(PrefabID, Name, Effect, GetUISprite);
		}
	}

	public bool MaterialsAvailable(IList<Tag> selected_elements)
	{
		bool result = true;
		Recipe.Ingredient[] allIngredients = CraftRecipe.GetAllIngredients(selected_elements);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			float amount = WorldInventory.Instance.GetAmount(ingredient.tag);
			if (amount < ingredient.amount)
			{
				result = false;
				break;
			}
		}
		return result;
	}
}
