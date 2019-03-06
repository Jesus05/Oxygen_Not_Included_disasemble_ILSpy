using UnityEngine;
using UnityEngine.UI;

public class RestartWarning : MonoBehaviour
{
	public static bool ShouldWarn;

	public LocText text;

	public Image image;

	private void Update()
	{
		if (ShouldWarn)
		{
			text.enabled = true;
			image.enabled = true;
		}
	}
}
