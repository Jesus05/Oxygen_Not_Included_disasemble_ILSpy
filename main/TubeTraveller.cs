using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TubeTraveller : GameStateMachine<TubeTraveller, TubeTraveller.Instance>, OxygenBreather.IGasProvider
{
	public new class Instance : GameInstance
	{
		private List<TravelTubeEntrance> reservations = new List<TravelTubeEntrance>();

		private bool inTube;

		private bool hadSuitTank;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void OnPathAdvanced(object data)
		{
			UnreserveEntrances();
			ReserveEntrances();
		}

		public void ReserveEntrances()
		{
			PathFinder.Path path = GetComponent<Navigator>().path;
			if (path.nodes != null)
			{
				for (int i = 0; i < path.nodes.Count - 1; i++)
				{
					PathFinder.Path.Node node = path.nodes[i];
					if (node.navType == NavType.Floor)
					{
						PathFinder.Path.Node node2 = path.nodes[i + 1];
						if (node2.navType == NavType.Tube)
						{
							PathFinder.Path.Node node3 = path.nodes[i];
							int cell = node3.cell;
							if (Grid.HasTubeEntrance[cell])
							{
								GameObject gameObject = Grid.Objects[cell, 1];
								if ((bool)gameObject)
								{
									TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
									if ((bool)component)
									{
										component.Reserve(this);
										reservations.Add(component);
									}
								}
							}
						}
					}
				}
			}
		}

		public void UnreserveEntrances()
		{
			foreach (TravelTubeEntrance reservation in reservations)
			{
				if (!((Object)reservation == (Object)null))
				{
					reservation.Unreserve(this);
				}
			}
			reservations.Clear();
		}

		public void OnTubeTransition(bool nowInTube)
		{
			if (nowInTube != inTube)
			{
				inTube = nowInTube;
				Effects component = GetComponent<Effects>();
				Attributes attributes = base.gameObject.GetAttributes();
				if (nowInTube)
				{
					hadSuitTank = HasSuitTank();
					if (!hadSuitTank)
					{
						GetComponent<OxygenBreather>().SetGasProvider(base.sm);
					}
					foreach (Effect immunity in base.sm.immunities)
					{
						component.AddImmunity(immunity);
					}
					foreach (AttributeModifier modifier in base.sm.modifiers)
					{
						attributes.Add(modifier);
					}
				}
				else
				{
					if (!hadSuitTank)
					{
						GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
					}
					foreach (Effect immunity2 in base.sm.immunities)
					{
						component.RemoveImmunity(immunity2);
					}
					foreach (AttributeModifier modifier2 in base.sm.modifiers)
					{
						attributes.Remove(modifier2);
					}
				}
				CreatureSimTemperatureTransfer component2 = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
				if ((Object)component2 != (Object)null)
				{
					component2.RefreshRegistration();
				}
			}
		}

		private bool HasSuitTank()
		{
			Equipment component = GetComponent<Equipment>();
			AssignableSlotInstance slot = component.GetSlot(Db.Get().AssignableSlots.Suit);
			if (slot != null && (Object)slot.assignable != (Object)null)
			{
				SuitTank component2 = slot.assignable.GetComponent<SuitTank>();
				return (Object)component2 != (Object)null;
			}
			return false;
		}
	}

	private List<Effect> immunities = new List<Effect>();

	private List<AttributeModifier> modifiers = new List<AttributeModifier>();

	public void InitModifiers()
	{
		modifiers.Add(new AttributeModifier(Db.Get().Attributes.Insulation.Id, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		modifiers.Add(new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		modifiers.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		modifiers.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		immunities.Add(Db.Get().effects.Get("SoakingWet"));
		immunities.Add(Db.Get().effects.Get("WetFeet"));
		immunities.Add(Db.Get().effects.Get("PoppedEarDrums"));
		immunities.Add(Db.Get().effects.Get("Unclean"));
	}

	public override void InitializeStates(out BaseState default_state)
	{
		InitModifiers();
		default_state = root;
		root.DoNothing();
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
	{
		return true;
	}

	public bool ShouldEmitCO2()
	{
		return false;
	}
}
