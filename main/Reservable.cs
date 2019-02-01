using UnityEngine;

public class Reservable : KMonoBehaviour
{
	private GameObject reservedBy;

	public GameObject ReservedBy => reservedBy;

	public bool isReserved => !((Object)reservedBy == (Object)null);

	public bool Reserve(GameObject reserver)
	{
		if (!((Object)reservedBy == (Object)null))
		{
			return false;
		}
		reservedBy = reserver;
		return true;
	}

	public void ClearReservation(GameObject reserver)
	{
		if ((Object)reservedBy == (Object)reserver)
		{
			reservedBy = null;
		}
	}
}
