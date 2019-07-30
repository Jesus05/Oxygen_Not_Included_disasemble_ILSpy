using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DupeGreetingManager : KMonoBehaviour, ISim200ms
{
	public class Tuning : TuningData<Tuning>
	{
		public float cyclesBeforeFirstGreeting;

		public float greetingDelayMultiplier;
	}

	private class GreetingUnit
	{
		public MinionIdentity minion;

		public Reactable reactable;

		public GreetingUnit(MinionIdentity minion, Reactable reactable)
		{
			this.minion = minion;
			this.reactable = reactable;
		}
	}

	private class GreetingSetup
	{
		public int cell;

		public GreetingUnit A;

		public GreetingUnit B;
	}

	private const float COOLDOWN_TIME = 720f;

	private Dictionary<int, MinionIdentity> candidateCells;

	private List<GreetingSetup> activeSetups;

	private Dictionary<MinionIdentity, float> cooldowns;

	private static readonly List<string> waveAnims = new List<string>
	{
		"anim_react_wave_kanim",
		"anim_react_wave_shy_kanim",
		"anim_react_fingerguns_kanim"
	};

	protected override void OnPrefabInit()
	{
		candidateCells = new Dictionary<int, MinionIdentity>();
		activeSetups = new List<GreetingSetup>();
		cooldowns = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		if (!(GameClock.Instance.GetTime() / 600f < TuningData<Tuning>.Get().cyclesBeforeFirstGreeting))
		{
			for (int num = activeSetups.Count - 1; num >= 0; num--)
			{
				GreetingSetup greetingSetup = activeSetups[num];
				if (!ValidNavigatingMinion(greetingSetup.A.minion) || !ValidOppositionalMinion(greetingSetup.A.minion, greetingSetup.B.minion))
				{
					greetingSetup.A.reactable.Cleanup();
					greetingSetup.B.reactable.Cleanup();
					activeSetups.RemoveAt(num);
				}
			}
			candidateCells.Clear();
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				if ((!cooldowns.ContainsKey(item) || !(GameClock.Instance.GetTime() - cooldowns[item] < 720f * TuningData<Tuning>.Get().greetingDelayMultiplier)) && ValidNavigatingMinion(item))
				{
					for (int i = 0; i <= 2; i++)
					{
						int offsetCell = GetOffsetCell(item, i);
						if (candidateCells.ContainsKey(offsetCell) && ValidOppositionalMinion(item, candidateCells[offsetCell]))
						{
							BeginNewGreeting(item, candidateCells[offsetCell], offsetCell);
							break;
						}
						candidateCells[offsetCell] = item;
					}
				}
			}
		}
	}

	private int GetOffsetCell(MinionIdentity minion, int offset)
	{
		Facing component = minion.GetComponent<Facing>();
		return (!component.GetFacing()) ? Grid.OffsetCell(Grid.PosToCell(minion), offset, 0) : Grid.OffsetCell(Grid.PosToCell(minion), -offset, 0);
	}

	private bool ValidNavigatingMinion(MinionIdentity minion)
	{
		if ((UnityEngine.Object)minion == (UnityEngine.Object)null)
		{
			return false;
		}
		Navigator component = minion.GetComponent<Navigator>();
		return (UnityEngine.Object)component != (UnityEngine.Object)null && component.IsMoving() && component.CurrentNavType == NavType.Floor;
	}

	private bool ValidOppositionalMinion(MinionIdentity reference_minion, MinionIdentity minion)
	{
		if ((UnityEngine.Object)reference_minion == (UnityEngine.Object)null)
		{
			return false;
		}
		if ((UnityEngine.Object)minion == (UnityEngine.Object)null)
		{
			return false;
		}
		Facing component = minion.GetComponent<Facing>();
		Facing component2 = reference_minion.GetComponent<Facing>();
		return ValidNavigatingMinion(minion) && (UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)component2 != (UnityEngine.Object)null && component.GetFacing() != component2.GetFacing();
	}

	private void BeginNewGreeting(MinionIdentity minion_a, MinionIdentity minion_b, int cell)
	{
		GreetingSetup greetingSetup = new GreetingSetup();
		greetingSetup.cell = cell;
		greetingSetup.A = new GreetingUnit(minion_a, GetReactable(minion_a));
		greetingSetup.B = new GreetingUnit(minion_b, GetReactable(minion_b));
		activeSetups.Add(greetingSetup);
	}

	private Reactable GetReactable(MinionIdentity minion)
	{
		return new SelfEmoteReactable(minion.gameObject, "NavigatorPassingGreeting", Db.Get().ChoreTypes.Emote, waveAnims[UnityEngine.Random.Range(0, waveAnims.Count)], 1000f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
		{
			anim = (HashedString)"react",
			startcb = new Action<GameObject>(BeginReacting)
		}).AddThought(Db.Get().Thoughts.Chatty);
	}

	private void BeginReacting(GameObject minionGO)
	{
		MinionIdentity component = minionGO.GetComponent<MinionIdentity>();
		Vector3 vector = Vector3.zero;
		foreach (GreetingSetup activeSetup in activeSetups)
		{
			if ((UnityEngine.Object)activeSetup.A.minion == (UnityEngine.Object)component)
			{
				vector = activeSetup.B.minion.transform.GetPosition();
				break;
			}
			if ((UnityEngine.Object)activeSetup.B.minion == (UnityEngine.Object)component)
			{
				vector = activeSetup.A.minion.transform.GetPosition();
				break;
			}
		}
		Facing component2 = minionGO.GetComponent<Facing>();
		Facing facing = component2;
		float x = vector.x;
		Vector3 position = minionGO.transform.GetPosition();
		facing.SetFacing(x < position.x);
		Effects component3 = minionGO.GetComponent<Effects>();
		component3.Add("Greeting", true);
		cooldowns[component] = GameClock.Instance.GetTime();
	}
}
