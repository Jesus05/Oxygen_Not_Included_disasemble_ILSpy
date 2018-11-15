using UnityEngine;

public class SwapUIAnimationController : MonoBehaviour
{
	public GameObject AnimationControllerObject_Primary;

	public GameObject AnimationControllerObject_Alternate;

	public void SetState(bool Primary)
	{
		AnimationControllerObject_Primary.SetActive(Primary);
		if (!Primary)
		{
			AnimationControllerObject_Alternate.GetComponent<KAnimControllerBase>().TintColour = new Color(1f, 1f, 1f, 0.5f);
			AnimationControllerObject_Primary.GetComponent<KAnimControllerBase>().TintColour = Color.clear;
		}
		AnimationControllerObject_Alternate.SetActive(!Primary);
		if (Primary)
		{
			AnimationControllerObject_Primary.GetComponent<KAnimControllerBase>().TintColour = Color.white;
			AnimationControllerObject_Alternate.GetComponent<KAnimControllerBase>().TintColour = Color.clear;
		}
	}
}
