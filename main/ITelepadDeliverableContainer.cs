using UnityEngine;

public interface ITelepadDeliverableContainer
{
	void SelectDeliverable();

	void DeselectDeliverable();

	GameObject GetGameObject();
}
