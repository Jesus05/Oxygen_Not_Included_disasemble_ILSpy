using System;
using UnityEngine;
using UnityEngine.UI;

public class DestinationAsteroid2 : KMonoBehaviour
{
	[SerializeField]
	private Image asteroidImage;

	[SerializeField]
	private KButton button;

	private ColonyDestinationAsteroidData asteroidData;

	public event Action<ColonyDestinationAsteroidData> OnClicked;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		button.onClick += OnClickInternal;
	}

	public void SetAsteroid(ColonyDestinationAsteroidData newAsteroidData)
	{
		if (newAsteroidData != asteroidData)
		{
			asteroidData = newAsteroidData;
			asteroidImage.sprite = Assets.GetSprite(asteroidData.sprite);
		}
	}

	private void OnClickInternal()
	{
		DebugUtil.LogArgs("Clicked asteroid", asteroidData.worldPath);
		this.OnClicked(asteroidData);
	}
}
