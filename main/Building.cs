using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public class Building : KMonoBehaviour, IEffectDescriptor, IUniformGridObject, IApproachable
{
	public BuildingDef Def;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private StateMachineController stateMachineController;

	private int[] placementCells;

	private Extents extents;

	private HandleVector<int>.Handle scenePartitionerEntry;

	public Orientation Orientation => ((UnityEngine.Object)rotatable != (UnityEngine.Object)null) ? rotatable.GetOrientation() : Orientation.Neutral;

	public int[] PlacementCells
	{
		get
		{
			if (placementCells == null)
			{
				RefreshCells();
			}
			return placementCells;
		}
	}

	public Extents GetExtents()
	{
		if (extents.width == 0 || extents.height == 0)
		{
			RefreshCells();
		}
		return extents;
	}

	public Extents GetValidPlacementExtents()
	{
		Extents result = GetExtents();
		result.x--;
		result.y--;
		result.width += 2;
		result.height += 2;
		return result;
	}

	public void RefreshCells()
	{
		placementCells = new int[Def.PlacementOffsets.Length];
		int cell = Grid.PosToCell(this);
		Orientation orientation = Orientation;
		for (int i = 0; i < Def.PlacementOffsets.Length; i++)
		{
			CellOffset offset = Def.PlacementOffsets[i];
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			placementCells[i] = num;
		}
		int x = 0;
		int y = 0;
		Grid.CellToXY(placementCells[0], out x, out y);
		int num2 = x;
		int num3 = y;
		int[] array = placementCells;
		foreach (int cell2 in array)
		{
			int x2 = 0;
			int y2 = 0;
			Grid.CellToXY(cell2, out x2, out y2);
			x = Math.Min(x, x2);
			y = Math.Min(y, y2);
			num2 = Math.Max(num2, x2);
			num3 = Math.Max(num3, y2);
		}
		extents.x = x;
		extents.y = y;
		extents.width = num2 - x + 1;
		extents.height = num3 - y + 1;
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.Temperature == 0f)
		{
			if (component.Element == null)
			{
				DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(base.name + " primary element has no element.", base.gameObject);
			}
			else if (!(this is BuildingUnderConstruction))
			{
				DeserializeWarnings.Instance.BuildingTemeperatureIsZeroKelvin.Warn(base.name + " is at zero degrees kelvin. Resetting temperature.", null);
				component.Temperature = component.Element.defaultValues.temperature;
			}
		}
	}

	protected override void OnSpawn()
	{
		if ((UnityEngine.Object)Def == (UnityEngine.Object)null)
		{
			Debug.LogError("Missing building definition on object " + base.name, null);
		}
		KSelectable component = GetComponent<KSelectable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetName(Def.Name);
			component.SetStatusIndicatorOffset(new Vector3(0f, -0.35f, 0f));
		}
		Prioritizable component2 = GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			component2.iconOffset.y = 0.3f;
		}
		KPrefabID component3 = GetComponent<KPrefabID>();
		if (component3.HasTag(RoomConstraints.ConstraintTags.IndustrialMachinery))
		{
			scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.name, base.gameObject, GetExtents(), GameScenePartitioner.Instance.industrialBuildings, null);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
		base.OnCleanUp();
	}

	protected void RegisterBlockTileRenderer()
	{
		if ((UnityEngine.Object)Def.BlockTileAtlas != (UnityEngine.Object)null)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				SimHashes visualizationElementID = GetVisualizationElementID(component);
				World.Instance.blockTileRenderer.AddBlock(base.gameObject.layer, Def, visualizationElementID, Grid.PosToCell(base.transform.GetPosition()));
			}
		}
	}

	public CellOffset GetRotatedOffset(CellOffset offset)
	{
		return (!((UnityEngine.Object)rotatable != (UnityEngine.Object)null)) ? offset : rotatable.GetRotatedCellOffset(offset);
	}

	private int GetBottomLeftCell()
	{
		Vector3 position = base.transform.GetPosition();
		return Grid.PosToCell(position);
	}

	public int GetPowerInputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.PowerInputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetPowerOutputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.PowerOutputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetUtilityInputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.UtilityInputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public int GetUtilityOutputCell()
	{
		CellOffset rotatedOffset = GetRotatedOffset(Def.UtilityOutputOffset);
		return Grid.OffsetCell(GetBottomLeftCell(), rotatedOffset);
	}

	public CellOffset GetUtilityInputOffset()
	{
		return GetRotatedOffset(Def.UtilityInputOffset);
	}

	public CellOffset GetUtilityOutputOffset()
	{
		return GetRotatedOffset(Def.UtilityOutputOffset);
	}

	protected void UnregisterBlockTileRenderer()
	{
		if ((UnityEngine.Object)Def.BlockTileAtlas != (UnityEngine.Object)null)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				SimHashes visualizationElementID = GetVisualizationElementID(component);
				World.Instance.blockTileRenderer.RemoveBlock(Def, visualizationElementID, Grid.PosToCell(base.transform.GetPosition()));
			}
		}
	}

	private SimHashes GetVisualizationElementID(PrimaryElement pe)
	{
		return (!(this is BuildingComplete)) ? SimHashes.Void : pe.ElementID;
	}

	public void RunOnArea(Action<int> callback)
	{
		Def.RunOnArea(Grid.PosToCell(this), Orientation, callback);
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (def.RequiresPowerInput)
		{
			float wattsNeededWhenActive = GetComponent<IEnergyConsumer>().WattsNeededWhenActive;
			if (wattsNeededWhenActive > 0f)
			{
				string formattedWattage = GameUtil.GetFormattedWattage(wattsNeededWhenActive, GameUtil.WattageFormatterUnit.Automatic);
				Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESPOWER, formattedWattage), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWER, formattedWattage), Descriptor.DescriptorType.Requirement, false);
				list.Add(item);
			}
		}
		if (def.InputConduitType == ConduitType.Liquid)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESLIQUIDINPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDINPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item2);
		}
		else if (def.InputConduitType == ConduitType.Gas)
		{
			Descriptor item3 = default(Descriptor);
			item3.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESGASINPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESGASINPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item3);
		}
		if (def.OutputConduitType == ConduitType.Liquid)
		{
			Descriptor item4 = default(Descriptor);
			item4.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESLIQUIDOUTPUT, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESLIQUIDOUTPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item4);
		}
		else if (def.OutputConduitType == ConduitType.Gas)
		{
			Descriptor item5 = default(Descriptor);
			item5.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT, UI.BUILDINGEFFECTS.REQUIRESGASOUTPUT, Descriptor.DescriptorType.Requirement);
			list.Add(item5);
		}
		if (component.isManuallyOperated)
		{
			Descriptor item6 = default(Descriptor);
			item6.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION, Descriptor.DescriptorType.Requirement);
			list.Add(item6);
		}
		if (component.isArtable)
		{
			Descriptor item7 = default(Descriptor);
			item7.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESCREATIVITY, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESCREATIVITY, Descriptor.DescriptorType.Requirement);
			list.Add(item7);
		}
		if ((UnityEngine.Object)def.BuildingUnderConstruction != (UnityEngine.Object)null)
		{
			Constructable component2 = def.BuildingUnderConstruction.GetComponent<Constructable>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.requiredRolePerk != HashedString.Invalid)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<RoleConfig> rolesWithPerk = Game.Instance.roleManager.GetRolesWithPerk(component2.requiredRolePerk);
				for (int i = 0; i < rolesWithPerk.Count; i++)
				{
					RoleConfig roleConfig = rolesWithPerk[i];
					stringBuilder.Append(roleConfig.GetProperName());
					if (i != rolesWithPerk.Count - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				string replacement = stringBuilder.ToString();
				list.Add(new Descriptor(UI.BUILD_REQUIRES_ROLE.Replace("{ROLE}", replacement), UI.BUILD_REQUIRES_ROLE_TOOLTIP.Replace("{ROLE}", replacement), Descriptor.DescriptorType.Requirement, false));
			}
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (def.EffectDescription != null)
		{
			list.AddRange(def.EffectDescription);
		}
		if (def.GeneratorWattageRating > 0f && (UnityEngine.Object)GetComponent<Battery>() == (UnityEngine.Object)null)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating, GameUtil.WattageFormatterUnit.Automatic)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ENERGYGENERATED, GameUtil.GetFormattedWattage(def.GeneratorWattageRating, GameUtil.WattageFormatterUnit.Automatic)), Descriptor.DescriptorType.Effect);
			list.Add(item);
		}
		if (def.ExhaustKilowattsWhenActive > 0f || def.SelfHeatKilowattsWhenActive > 0f)
		{
			Descriptor item2 = default(Descriptor);
			string formattedHeatEnergy = GameUtil.GetFormattedHeatEnergy((def.ExhaustKilowattsWhenActive + def.SelfHeatKilowattsWhenActive) * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic);
			item2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATGENERATED, formattedHeatEnergy), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, formattedHeatEnergy), Descriptor.DescriptorType.Effect);
			list.Add(item2);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors(def))
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in EffectDescriptors(def))
		{
			list.Add(item2);
		}
		return list;
	}

	public override Vector2 PosMin()
	{
		Extents extents = GetExtents();
		return new Vector2((float)extents.x, (float)extents.y);
	}

	public override Vector2 PosMax()
	{
		Extents extents = GetExtents();
		return new Vector2((float)(extents.x + extents.width), (float)(extents.y + extents.height));
	}

	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Use;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	Transform get_transform()
	{
		return base.transform;
	}

	Transform IApproachable.get_transform()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_transform
		return this.get_transform();
	}
}
