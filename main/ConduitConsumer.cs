using STRINGS;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class ConduitConsumer : KMonoBehaviour
{
	public enum WrongElementResult
	{
		Destroy,
		Dump,
		Store
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public bool ignoreMinMassCheck;

	[SerializeField]
	public Tag capacityTag = GameTags.Any;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public bool forceAlwaysSatisfied;

	[SerializeField]
	public bool alwaysConsume;

	[SerializeField]
	public bool keepZeroMassObject = true;

	[SerializeField]
	public bool useSecondaryInput;

	[NonSerialized]
	public bool isConsuming = true;

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	public Storage storage;

	private int utilityCell = -1;

	public float consumptionRate = float.PositiveInfinity;

	public SimHashes lastConsumedElement = SimHashes.Vacuum;

	public static readonly Operational.Flag elementRequirementFlag = new Operational.Flag("elementRequired", Operational.Flag.Type.Requirement);

	private HandleVector<int>.Handle partitionerEntry;

	private bool satisfied;

	public WrongElementResult wrongElementResult;

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[utilityCell, (conduitType != ConduitType.Gas) ? 16 : 12];
			return (UnityEngine.Object)gameObject != (UnityEngine.Object)null && (UnityEngine.Object)gameObject.GetComponent<BuildingComplete>() != (UnityEngine.Object)null;
		}
	}

	public bool CanConsume
	{
		get
		{
			bool result = false;
			if (IsConnected)
			{
				ConduitFlow conduitManager = GetConduitManager();
				result = (conduitManager.GetContents(utilityCell).mass > 0f);
			}
			return result;
		}
	}

	public float stored_mass => ((UnityEngine.Object)storage == (UnityEngine.Object)null) ? 0f : ((!(capacityTag != GameTags.Any)) ? storage.MassStored() : storage.GetMassAvailable(capacityTag));

	public float space_remaining_kg
	{
		get
		{
			float num = capacityKG - stored_mass;
			return (!((UnityEngine.Object)storage == (UnityEngine.Object)null)) ? Mathf.Min(storage.RemainingCapacity(), num) : num;
		}
	}

	public ConduitType TypeOfConduit => conduitType;

	public bool IsAlmostEmpty => !ignoreMinMassCheck && MassAvailable < ConsumptionRate * 30f;

	public bool IsEmpty => !ignoreMinMassCheck && (MassAvailable == 0f || MassAvailable < ConsumptionRate);

	public float ConsumptionRate => consumptionRate;

	public bool IsSatisfied
	{
		get
		{
			return satisfied || !isConsuming;
		}
		set
		{
			satisfied = (value || forceAlwaysSatisfied);
		}
	}

	public float MassAvailable
	{
		get
		{
			int inputCell = GetInputCell();
			ConduitFlow conduitManager = GetConduitManager();
			return conduitManager.GetContents(inputCell).mass;
		}
	}

	public void SetConduitData(ConduitType type)
	{
		conduitType = type;
	}

	private ConduitFlow GetConduitManager()
	{
		switch (conduitType)
		{
		case ConduitType.Gas:
			return Game.Instance.gasConduitFlow;
		case ConduitType.Liquid:
			return Game.Instance.liquidConduitFlow;
		default:
			return null;
		}
	}

	private int GetInputCell()
	{
		if (useSecondaryInput)
		{
			ISecondaryInput component = GetComponent<ISecondaryInput>();
			return Grid.OffsetCell(building.NaturalBuildingCell(), component.GetSecondaryConduitOffset());
		}
		return building.GetUtilityInputCell();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		utilityCell = GetInputCell();
		ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(conduitType != ConduitType.Gas) ? 16 : 12];
		partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, utilityCell, layer, OnConduitConnectionChanged);
		GetConduitManager().AddConduitUpdater(ConduitUpdate, ConduitFlowPriority.Default);
		OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		GetConduitManager().RemoveConduitUpdater(ConduitUpdate);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		Trigger(-2094018600, IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		if (isConsuming)
		{
			ConduitFlow conduitManager = GetConduitManager();
			Consume(dt, conduitManager);
		}
	}

	private void Consume(float dt, ConduitFlow conduit_mgr)
	{
		IsSatisfied = false;
		if (building.Def.CanMove)
		{
			utilityCell = GetInputCell();
		}
		if (IsConnected)
		{
			ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(utilityCell);
			if (!(contents.mass <= 0f))
			{
				IsSatisfied = true;
				if (alwaysConsume || operational.IsOperational)
				{
					float a = ConsumptionRate * dt;
					a = Mathf.Min(a, space_remaining_kg);
					float num = 0f;
					if (a > 0f)
					{
						ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(utilityCell, a);
						num = conduitContents.mass;
						lastConsumedElement = conduitContents.element;
					}
					Element element = ElementLoader.FindElementByHash(contents.element);
					bool flag = element.HasTag(capacityTag);
					if (num > 0f && capacityTag != GameTags.Any && !flag)
					{
						Trigger(-794517298, new BuildingHP.DamageSourceInfo
						{
							damage = 1,
							source = (string)BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
							popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
						});
					}
					if (flag || wrongElementResult == WrongElementResult.Store || contents.element == SimHashes.Vacuum || capacityTag == GameTags.Any)
					{
						if (num > 0f)
						{
							int disease_count = (int)((float)contents.diseaseCount * (num / contents.mass));
							Element element2 = ElementLoader.FindElementByHash(contents.element);
							switch (conduitType)
							{
							case ConduitType.Liquid:
								if (element2.IsLiquid)
								{
									storage.AddLiquid(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, keepZeroMassObject, false);
								}
								else
								{
									Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id.ToString());
								}
								break;
							case ConduitType.Gas:
								if (element2.IsGas)
								{
									storage.AddGasChunk(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, keepZeroMassObject, false);
								}
								else
								{
									Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id.ToString());
								}
								break;
							}
						}
					}
					else if (num > 0f && wrongElementResult == WrongElementResult.Dump)
					{
						int disease_count2 = (int)((float)contents.diseaseCount * (num / contents.mass));
						int gameCell = Grid.PosToCell(base.transform.GetPosition());
						SimMessages.AddRemoveSubstance(gameCell, contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num, contents.temperature, contents.diseaseIdx, disease_count2, true, -1);
					}
				}
			}
		}
	}
}
