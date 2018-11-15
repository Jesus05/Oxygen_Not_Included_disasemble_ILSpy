using UnityEngine;

[SkipSaveFileSerialization]
public class TravelTubeBridge : KMonoBehaviour, ITravelTubePiece
{
	private static readonly EventSystem.IntraObjectHandler<TravelTubeBridge> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<TravelTubeBridge>(delegate(TravelTubeBridge component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<TravelTubeBridge> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<TravelTubeBridge>(delegate(TravelTubeBridge component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	public Vector3 Position => base.transform.GetPosition();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Grid.HasTube[Grid.PosToCell(this)] = true;
		Components.ITravelTubePieces.Add(this);
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(774203113, OnBuildingBrokenDelegate, false);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate, false);
		Grid.HasTube[Grid.PosToCell(this)] = false;
		Components.ITravelTubePieces.Remove(this);
		base.OnCleanUp();
	}

	private void OnBuildingBroken(object data)
	{
	}

	private void OnBuildingFullyRepaired(object data)
	{
	}
}
