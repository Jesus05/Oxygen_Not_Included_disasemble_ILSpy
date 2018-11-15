using Klei;
using UnityEngine;

public class LadderDiseaseTransitionLayer : TransitionDriver.OverrideLayer
{
	public LadderDiseaseTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		if (transition.end == NavType.Ladder)
		{
			int cell = Grid.PosToCell(navigator);
			GameObject gameObject = Grid.Objects[cell, 1];
			if ((Object)gameObject != (Object)null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if ((Object)component != (Object)null)
				{
					PrimaryElement component2 = navigator.GetComponent<PrimaryElement>();
					if ((Object)component2 != (Object)null)
					{
						SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
						invalid.idx = component2.DiseaseIdx;
						invalid.count = (int)((float)component2.DiseaseCount * 0.005f);
						SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
						invalid2.idx = component.DiseaseIdx;
						invalid2.count = (int)((float)component.DiseaseCount * 0.005f);
						component2.ModifyDiseaseCount(-invalid.count, "Navigator.EndTransition");
						component.ModifyDiseaseCount(-invalid2.count, "Navigator.EndTransition");
						if (invalid.count > 0)
						{
							component.AddDisease(invalid.idx, invalid.count, "TransitionDriver.EndTransition");
						}
						if (invalid2.count > 0)
						{
							component2.AddDisease(invalid2.idx, invalid2.count, "TransitionDriver.EndTransition");
						}
					}
				}
			}
		}
	}
}
