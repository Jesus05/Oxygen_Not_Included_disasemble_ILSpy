using UnityEngine;
using UnityEngine.UI;

public class Vignette : MonoBehaviour
{
	[SerializeField]
	private Image image;

	private Color defaultColor;

	public static Vignette Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void Awake()
	{
		Instance = this;
		defaultColor = image.color;
	}

	public void SetColor(Color color)
	{
		image.color = color;
	}

	public void Reset()
	{
		SetColor(defaultColor);
	}
}
