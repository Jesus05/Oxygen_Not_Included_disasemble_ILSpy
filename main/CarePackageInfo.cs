using UnityEngine;

public class CarePackageInfo : ITelepadDeliverable
{
	public readonly string id;

	public readonly float quantity;

	public readonly int cycleRequirement;

	public CarePackageInfo(string ID, float amount, int cycleRequirement)
	{
		id = ID;
		quantity = amount;
		this.cycleRequirement = cycleRequirement;
	}

	public GameObject Deliver(Vector3 location)
	{
		location += Vector3.right / 2f;
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(CarePackageConfig.ID), location);
		gameObject.SetActive(true);
		gameObject.GetComponent<CarePackage>().SetInfo(this);
		return gameObject;
	}
}
