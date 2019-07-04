using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComplete : Building
{
	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpGet]
	public Assignable assignable;

	[MyCmpGet]
	public KPrefabID prefabid;

	public bool isManuallyOperated;

	public bool isArtable;

	private bool hasSpawnedKComponents;

	private bool wasReplaced = false;

	public List<AttributeModifier> regionModifiers = new List<AttributeModifier>();

	private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<BuildingComplete>(delegate(BuildingComplete component, object data)
	{
		component.OnObjectReplaced(data);
	});

	private HandleVector<int>.Handle scenePartitionerEntry;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Def.SceneLayer);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		Attributes attributes = this.GetAttributes();
		foreach (Attribute attribute2 in Def.attributes)
		{
			attributes.Add(attribute2);
		}
		foreach (AttributeModifier attributeModifier in Def.attributeModifiers)
		{
			Attribute attribute = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
			if (attributes.Get(attribute) == null)
			{
				attributes.Add(attribute);
			}
			attributes.Add(attributeModifier);
		}
		foreach (AttributeInstance item2 in attributes)
		{
			AttributeModifier item = new AttributeModifier(item2.Id, item2.GetTotalValue(), null, false, false, true);
			regionModifiers.Add(item);
		}
		if (Def.UseStructureTemperature)
		{
			GameComps.StructureTemperatures.Add(base.gameObject);
		}
		Subscribe(1606648047, OnObjectReplacedDelegate);
	}

	private void OnObjectReplaced(object data)
	{
		wasReplaced = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Rotatable component2 = GetComponent<Rotatable>();
		if ((Object)component != (Object)null && (Object)component2 == (Object)null)
		{
			component.Offset = Def.GetVisualizerOffset() + Def.placementPivot;
		}
		KBoxCollider2D component3 = GetComponent<KBoxCollider2D>();
		if ((Object)component3 != (Object)null)
		{
			Vector3 visualizerOffset = Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		int cell = Grid.PosToCell(base.transform.GetPosition());
		if (Def.IsFoundation)
		{
			int[] placementCells = base.PlacementCells;
			foreach (int num in placementCells)
			{
				Grid.Foundation[num] = true;
				Game.Instance.roomProber.SolidChangedEvent(num, false);
			}
		}
		Vector3 position = Grid.CellToPosCBC(cell, Def.SceneLayer);
		base.transform.SetPosition(position);
		PrimaryElement component4 = GetComponent<PrimaryElement>();
		if ((Object)component4 != (Object)null && component4.Mass == 0f)
		{
			component4.Mass = Def.Mass[0];
		}
		Def.MarkArea(cell, base.Orientation, Def.ObjectLayer, base.gameObject);
		if (Def.IsTilePiece)
		{
			Def.MarkArea(cell, base.Orientation, Def.TileLayer, base.gameObject);
			Def.RunOnArea(cell, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
			});
		}
		RegisterBlockTileRenderer();
		if (Def.PreventIdleTraversalPastBuilding)
		{
			for (int j = 0; j < base.PlacementCells.Length; j++)
			{
				Grid.PreventIdleTraversal[base.PlacementCells[j]] = true;
			}
		}
		KSelectable component5 = GetComponent<KSelectable>();
		if ((Object)component5 != (Object)null)
		{
			component5.SetStatusIndicatorOffset(Def.placementPivot);
		}
		Components.BuildingCompletes.Add(this);
		BuildingConfigManager.Instance.AddBuildingCompleteKComponents(base.gameObject, Def.Tag);
		hasSpawnedKComponents = true;
		scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.name, this, GetExtents(), GameScenePartitioner.Instance.completeBuildings, null);
		Attributes attributes = this.GetAttributes();
		if (attributes != null)
		{
			Deconstructable component6 = GetComponent<Deconstructable>();
			if ((Object)component6 != (Object)null)
			{
				for (int k = 1; k < component6.constructionElements.Length; k++)
				{
					Tag tag = component6.constructionElements[k];
					Element element = ElementLoader.GetElement(tag);
					if (element != null)
					{
						foreach (AttributeModifier attributeModifier in element.attributeModifiers)
						{
							attributes.Add(attributeModifier);
						}
					}
					else
					{
						GameObject gameObject = Assets.TryGetPrefab(tag);
						if ((Object)gameObject != (Object)null)
						{
							PrefabAttributeModifiers component7 = gameObject.GetComponent<PrefabAttributeModifiers>();
							if ((Object)component7 != (Object)null)
							{
								foreach (AttributeModifier descriptor in component7.descriptors)
								{
									attributes.Add(descriptor);
								}
							}
						}
					}
				}
			}
		}
	}

	private string GetInspectSound()
	{
		string name = "AI_Inspect_" + GetComponent<KPrefabID>().PrefabTag.Name;
		return GlobalAssets.GetSound(name, false);
	}

	protected override void OnCleanUp()
	{
		if (!Game.quitting)
		{
			GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
			if (hasSpawnedKComponents)
			{
				BuildingConfigManager.Instance.DestroyBuildingCompleteKComponents(base.gameObject, Def.Tag);
			}
			if (Def.UseStructureTemperature)
			{
				GameComps.StructureTemperatures.Remove(base.gameObject);
			}
			base.OnCleanUp();
			if (!wasReplaced)
			{
				int cell = Grid.PosToCell(this);
				Def.UnmarkArea(cell, base.Orientation, Def.ObjectLayer, base.gameObject);
				if (Def.IsTilePiece)
				{
					Def.UnmarkArea(cell, base.Orientation, Def.TileLayer, base.gameObject);
					Def.RunOnArea(cell, base.Orientation, delegate(int c)
					{
						TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
					});
				}
				if (Def.IsFoundation)
				{
					int[] placementCells = base.PlacementCells;
					foreach (int num in placementCells)
					{
						Grid.Foundation[num] = false;
						Game.Instance.roomProber.SolidChangedEvent(num, false);
					}
				}
				if (Def.PreventIdleTraversalPastBuilding)
				{
					for (int j = 0; j < base.PlacementCells.Length; j++)
					{
						Grid.PreventIdleTraversal[base.PlacementCells[j]] = false;
					}
				}
			}
			Components.BuildingCompletes.Remove(this);
			UnregisterBlockTileRenderer();
			Trigger(-21016276, this);
		}
	}
}
