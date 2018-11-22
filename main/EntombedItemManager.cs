using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class EntombedItemManager : KMonoBehaviour, ISim33ms
{
	private struct Item
	{
		public int cell;

		public int elementId;

		public float mass;

		public float temperature;

		public byte diseaseIdx;

		public int diseaseCount;
	}

	[Serialize]
	private List<int> cells = new List<int>();

	[Serialize]
	private List<int> elementIds = new List<int>();

	[Serialize]
	private List<float> masses = new List<float>();

	[Serialize]
	private List<float> temperatures = new List<float>();

	[Serialize]
	private List<byte> diseaseIndices = new List<byte>();

	[Serialize]
	private List<int> diseaseCounts = new List<int>();

	private List<Pickupable> pickupables = new List<Pickupable>();

	[OnDeserialized]
	private void OnDeserialized()
	{
		SpawnUncoveredObjects();
		AddMassToWorlIfPossible();
		PopulateEntombedItemVisualizers();
	}

	public static bool CanEntomb(Pickupable pickupable)
	{
		if (!((Object)pickupable == (Object)null))
		{
			if (!((Object)pickupable.storage != (Object)null))
			{
				int num = Grid.PosToCell(pickupable);
				if (Grid.IsValidCell(num))
				{
					if (Grid.Solid[num])
					{
						if (!((Object)Grid.Objects[num, 9] != (Object)null))
						{
							PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
							if (component.Element.IsSolid)
							{
								ElementChunk component2 = pickupable.GetComponent<ElementChunk>();
								if ((Object)component2 != (Object)null)
								{
									return true;
								}
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	public void Add(Pickupable pickupable)
	{
		pickupables.Add(pickupable);
	}

	public void Sim33ms(float dt)
	{
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		HashSetPool<Pickupable, EntombedItemManager>.PooledHashSet pooledHashSet = HashSetPool<Pickupable, EntombedItemManager>.Allocate();
		foreach (Pickupable pickupable in pickupables)
		{
			if (CanEntomb(pickupable))
			{
				pooledHashSet.Add(pickupable);
			}
		}
		pickupables.Clear();
		foreach (Pickupable item in pooledHashSet)
		{
			int num = Grid.PosToCell(item);
			PrimaryElement component2 = item.GetComponent<PrimaryElement>();
			SimHashes elementID = component2.ElementID;
			float mass = component2.Mass;
			float temperature = component2.Temperature;
			byte diseaseIdx = component2.DiseaseIdx;
			int diseaseCount = component2.DiseaseCount;
			Element element = Grid.Element[num];
			if (elementID == element.id && mass > 0.0100000007f && Grid.Mass[num] + mass < element.maxMass)
			{
				SimMessages.AddRemoveSubstance(num, ElementLoader.FindElementByHash(elementID).idx, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature, diseaseIdx, diseaseCount, true, -1);
			}
			else
			{
				component.AddItem(num);
				cells.Add(num);
				elementIds.Add((int)elementID);
				masses.Add(mass);
				temperatures.Add(temperature);
				diseaseIndices.Add(diseaseIdx);
				diseaseCounts.Add(diseaseCount);
			}
			Util.KDestroyGameObject(item.gameObject);
		}
		pooledHashSet.Recycle();
	}

	public void OnSolidChanged(List<int> solid_changed_cells)
	{
		ListPool<int, EntombedItemManager>.PooledList pooledList = ListPool<int, EntombedItemManager>.Allocate();
		foreach (int solid_changed_cell in solid_changed_cells)
		{
			if (!Grid.Solid[solid_changed_cell])
			{
				pooledList.Add(solid_changed_cell);
			}
		}
		ListPool<int, EntombedItemManager>.PooledList pooledList2 = ListPool<int, EntombedItemManager>.Allocate();
		for (int i = 0; i < cells.Count; i++)
		{
			int num = cells[i];
			foreach (int item in pooledList)
			{
				if (num == item)
				{
					pooledList2.Add(i);
					break;
				}
			}
		}
		pooledList.Recycle();
		SpawnObjects(pooledList2);
		pooledList2.Recycle();
	}

	private void SpawnUncoveredObjects()
	{
		ListPool<int, EntombedItemManager>.PooledList pooledList = ListPool<int, EntombedItemManager>.Allocate();
		for (int i = 0; i < cells.Count; i++)
		{
			int i2 = cells[i];
			if (!Grid.Solid[i2])
			{
				pooledList.Add(i);
			}
		}
		SpawnObjects(pooledList);
		pooledList.Recycle();
	}

	private void AddMassToWorlIfPossible()
	{
		ListPool<int, EntombedItemManager>.PooledList pooledList = ListPool<int, EntombedItemManager>.Allocate();
		for (int i = 0; i < cells.Count; i++)
		{
			int num = cells[i];
			if (Grid.Solid[num] && Grid.Element[num].id == (SimHashes)elementIds[i])
			{
				pooledList.Add(i);
			}
		}
		pooledList.Sort();
		pooledList.Reverse();
		foreach (int item2 in pooledList)
		{
			Item item = GetItem(item2);
			RemoveItem(item2);
			SimMessages.AddRemoveSubstance(item.cell, ElementLoader.FindElementByHash((SimHashes)item.elementId).idx, CellEventLogger.Instance.ElementConsumerSimUpdate, item.mass, item.temperature, item.diseaseIdx, item.diseaseCount, true, -1);
		}
		pooledList.Recycle();
	}

	private void RemoveItem(int item_idx)
	{
		cells.RemoveAt(item_idx);
		elementIds.RemoveAt(item_idx);
		masses.RemoveAt(item_idx);
		temperatures.RemoveAt(item_idx);
		diseaseIndices.RemoveAt(item_idx);
		diseaseCounts.RemoveAt(item_idx);
	}

	private Item GetItem(int item_idx)
	{
		Item result = default(Item);
		result.cell = cells[item_idx];
		result.elementId = elementIds[item_idx];
		result.mass = masses[item_idx];
		result.temperature = temperatures[item_idx];
		result.diseaseIdx = diseaseIndices[item_idx];
		result.diseaseCount = diseaseCounts[item_idx];
		return result;
	}

	private void SpawnObjects(List<int> uncovered_item_indices)
	{
		uncovered_item_indices.Sort();
		uncovered_item_indices.Reverse();
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		foreach (int uncovered_item_index in uncovered_item_indices)
		{
			Item item = GetItem(uncovered_item_index);
			component.RemoveItem(item.cell);
			RemoveItem(uncovered_item_index);
			SimHashes elementId = (SimHashes)item.elementId;
			ElementLoader.FindElementByHash(elementId)?.substance.SpawnResource(Grid.CellToPosCCC(item.cell, Grid.SceneLayer.Ore), item.mass, item.temperature, item.diseaseIdx, item.diseaseCount, false, false);
		}
	}

	private void PopulateEntombedItemVisualizers()
	{
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		foreach (int cell in cells)
		{
			component.AddItem(cell);
		}
	}
}
