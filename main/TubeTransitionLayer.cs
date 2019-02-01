using UnityEngine;

public class TubeTransitionLayer : TransitionDriver.OverrideLayer
{
	private TubeTraveller.Instance tube_traveller;

	private TravelTubeEntrance entrance;

	public TubeTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		tube_traveller = navigator.GetSMI<TubeTraveller.Instance>();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		tube_traveller.OnPathAdvanced(null);
		if (transition.start != NavType.Tube && transition.end == NavType.Tube)
		{
			int cell = Grid.PosToCell(navigator);
			entrance = GetEntrance(cell);
		}
		else
		{
			entrance = null;
		}
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		if (transition.start != NavType.Tube && transition.end == NavType.Tube && (bool)entrance)
		{
			entrance.ConsumeCharge(navigator.gameObject);
			entrance = null;
		}
		tube_traveller.OnTubeTransition(transition.end == NavType.Tube);
	}

	private TravelTubeEntrance GetEntrance(int cell)
	{
		if (Grid.HasUsableTubeEntrance(cell, tube_traveller.prefabInstanceID))
		{
			GameObject gameObject = Grid.Objects[cell, 1];
			if ((Object)gameObject != (Object)null)
			{
				TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
				if ((Object)component != (Object)null && component.isSpawned)
				{
					return component;
				}
			}
			return null;
		}
		return null;
	}
}
