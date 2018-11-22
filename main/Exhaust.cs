using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Exhaust : KMonoBehaviour, ISim200ms
{
	[MyCmpGet]
	private Vent vent;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private ConduitConsumer consumer;

	[MyCmpGet]
	private PrimaryElement exhaustPE;

	private static readonly Operational.Flag canExhaust = new Operational.Flag("canExhaust", Operational.Flag.Type.Requirement);

	private bool isAnimating;

	private bool recentlyExhausted = false;

	private const float MinSwitchTime = 1f;

	private float elapsedSwitchTime = 0f;

	private static readonly EventSystem.IntraObjectHandler<Exhaust> OnConduitStateChangedDelegate = new EventSystem.IntraObjectHandler<Exhaust>(delegate(Exhaust component, object data)
	{
		component.OnConduitStateChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-592767678, OnConduitStateChangedDelegate);
		Subscribe(-111137758, OnConduitStateChangedDelegate);
		GetComponent<RequireInputs>().visualizeRequirements = false;
	}

	protected override void OnSpawn()
	{
		OnConduitStateChanged(null);
	}

	private void OnConduitStateChanged(object data)
	{
		operational.SetActive(operational.IsOperational && !vent.IsBlocked, false);
	}

	private void CalculateDiseaseTransfer(PrimaryElement item1, PrimaryElement item2, float transfer_rate, out int disease_to_item1, out int disease_to_item2)
	{
		disease_to_item1 = (int)((float)item2.DiseaseCount * transfer_rate);
		disease_to_item2 = (int)((float)item1.DiseaseCount * transfer_rate);
	}

	public void Sim200ms(float dt)
	{
		operational.SetFlag(canExhaust, !vent.IsBlocked);
		if (!operational.IsOperational)
		{
			if (isAnimating)
			{
				isAnimating = false;
				recentlyExhausted = false;
				Trigger(-793429877, null);
			}
		}
		else
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (!Grid.Solid[num] && consumer.ConsumptionRate != 0f)
			{
				List<GameObject> items = storage.items;
				if (items.Count > 0)
				{
					switch (consumer.TypeOfConduit)
					{
					case ConduitType.Liquid:
					{
						int num2 = Grid.CellBelow(num);
						bool flag = Grid.IsValidCell(num2) && !Grid.Solid[num2];
						for (int j = 0; j < items.Count; j++)
						{
							PrimaryElement component2 = items[j].GetComponent<PrimaryElement>();
							if (component2.Mass > 0f && component2.Element.IsLiquid)
							{
								CalculateDiseaseTransfer(exhaustPE, component2, 0.05f, out int disease_to_item3, out int disease_to_item4);
								component2.ModifyDiseaseCount(-disease_to_item3, "Exhaust transfer");
								component2.AddDisease(exhaustPE.DiseaseIdx, disease_to_item4, "Exhaust transfer");
								exhaustPE.ModifyDiseaseCount(-disease_to_item4, "Exhaust transfer");
								exhaustPE.AddDisease(component2.DiseaseIdx, disease_to_item3, "Exhaust transfer");
								if (flag)
								{
									byte elementIdx = (byte)ElementLoader.elements.IndexOf(component2.Element);
									FallingWater.instance.AddParticle(num, elementIdx, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, true, false, true, false);
								}
								else
								{
									SimMessages.AddRemoveSubstance(num, component2.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, true, -1);
								}
								component2.KeepZeroMassObject = true;
								component2.Mass = 0f;
								component2.ModifyDiseaseCount(-2147483648, "Exhaust.SimUpdate");
								recentlyExhausted = true;
								break;
							}
						}
						break;
					}
					case ConduitType.Gas:
						for (int i = 0; i < items.Count; i++)
						{
							PrimaryElement component = items[i].GetComponent<PrimaryElement>();
							if (component.Mass > 0f && component.Element.IsGas)
							{
								CalculateDiseaseTransfer(exhaustPE, component, 0.05f, out int disease_to_item, out int disease_to_item2);
								component.ModifyDiseaseCount(-disease_to_item, "Exhaust transfer");
								component.AddDisease(exhaustPE.DiseaseIdx, disease_to_item2, "Exhaust transfer");
								exhaustPE.ModifyDiseaseCount(-disease_to_item2, "Exhaust transfer");
								exhaustPE.AddDisease(component.DiseaseIdx, disease_to_item, "Exhaust transfer");
								SimMessages.AddRemoveSubstance(num, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
								component.KeepZeroMassObject = true;
								component.Mass = 0f;
								component.ModifyDiseaseCount(-2147483648, "Exhaust.SimUpdate");
								recentlyExhausted = true;
								break;
							}
						}
						break;
					}
				}
			}
			elapsedSwitchTime -= dt;
			if (elapsedSwitchTime <= 0f)
			{
				elapsedSwitchTime = 1f;
				if (recentlyExhausted != isAnimating)
				{
					isAnimating = recentlyExhausted;
					Trigger(-793429877, null);
				}
				recentlyExhausted = false;
			}
		}
	}

	public bool IsAnimating()
	{
		return isAnimating;
	}
}
