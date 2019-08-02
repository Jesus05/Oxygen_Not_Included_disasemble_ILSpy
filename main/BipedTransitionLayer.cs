using Klei.AI;
using TUNING;
using UnityEngine;

public class BipedTransitionLayer : TransitionDriver.OverrideLayer
{
	private bool isWalking;

	private float floorSpeed;

	private float ladderSpeed;

	private float startTime;

	private float jetPackSpeed;

	private const float tubeSpeed = 18f;

	private const float downPoleSpeed = 15f;

	private const float WATER_SPEED_PENALTY = 0.5f;

	private AttributeConverterInstance movementSpeed;

	private AttributeLevels attributeLevels;

	public BipedTransitionLayer(Navigator navigator, float floor_speed, float ladder_speed)
		: base(navigator)
	{
		navigator.Subscribe(1773898642, delegate
		{
			isWalking = true;
		});
		navigator.Subscribe(1597112836, delegate
		{
			isWalking = false;
		});
		floorSpeed = floor_speed;
		ladderSpeed = ladder_speed;
		jetPackSpeed = floor_speed;
		movementSpeed = Db.Get().AttributeConverters.MovementSpeed.Lookup(navigator.gameObject);
		attributeLevels = navigator.GetComponent<AttributeLevels>();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		float num = 1f;
		bool flag = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
		bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
		bool flag3 = transition.start == NavType.Hover || transition.end == NavType.Hover;
		if (!flag && !flag2 && !flag3)
		{
			if (isWalking)
			{
				return;
			}
			num = GetMovementSpeedMultiplier(navigator);
		}
		int cell = Grid.PosToCell(navigator);
		float num2 = 1f;
		bool flag4 = (navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != PathFinder.PotentialPath.Flags.None;
		if ((navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) == PathFinder.PotentialPath.Flags.None && !flag4 && Grid.IsSubstantialLiquid(cell, 0.35f))
		{
			num2 = 0.5f;
		}
		num *= num2;
		if (transition.x == 0 && (transition.start == NavType.Ladder || transition.start == NavType.Pole) && transition.start == transition.end)
		{
			if (flag)
			{
				transition.speed = 15f * num2;
			}
			else
			{
				transition.speed = ladderSpeed * num;
				GameObject gameObject = Grid.Objects[cell, 1];
				if ((Object)gameObject != (Object)null)
				{
					Ladder component = gameObject.GetComponent<Ladder>();
					if ((Object)component != (Object)null)
					{
						float num3 = component.upwardsMovementSpeedMultiplier;
						if (transition.y < 0)
						{
							num3 = component.downwardsMovementSpeedMultiplier;
						}
						transition.speed *= num3;
						transition.animSpeed *= num3;
					}
				}
			}
		}
		else if (flag2)
		{
			transition.speed = 18f;
		}
		else if (flag3)
		{
			transition.speed = jetPackSpeed;
		}
		else
		{
			transition.speed = floorSpeed * num;
		}
		float num4 = num - 1f;
		transition.animSpeed += transition.animSpeed * num4 / 2f;
		if (transition.start == NavType.Floor && transition.end == NavType.Floor)
		{
			int num5 = Grid.CellBelow(cell);
			if (Grid.Foundation[num5])
			{
				GameObject gameObject2 = Grid.Objects[num5, 1];
				if ((Object)gameObject2 != (Object)null)
				{
					SimCellOccupier component2 = gameObject2.GetComponent<SimCellOccupier>();
					if ((Object)component2 != (Object)null)
					{
						transition.speed *= component2.movementSpeedMultiplier;
						transition.animSpeed *= component2.movementSpeedMultiplier;
					}
				}
			}
		}
		startTime = Time.time;
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		bool flag = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
		bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
		if (!isWalking && !flag && !flag2 && (Object)attributeLevels != (Object)null)
		{
			attributeLevels.AddExperience(Db.Get().Attributes.Athletics.Id, Time.time - startTime, DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE);
		}
	}

	public float GetMovementSpeedMultiplier(Navigator navigator)
	{
		float num = 1f;
		if (movementSpeed != null)
		{
			num += movementSpeed.Evaluate();
		}
		return Mathf.Max(0.1f, num);
	}
}
