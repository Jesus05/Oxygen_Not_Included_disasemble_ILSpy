using UnityEngine;
using UnityEngine.UI;

public class KScrollbarVisibility : MonoBehaviour
{
	[SerializeField]
	private ScrollRect content;

	[SerializeField]
	private RectTransform parent;

	[SerializeField]
	private bool checkWidth = true;

	[SerializeField]
	private bool checkHeight = true;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private GameObject[] others;

	private void Start()
	{
		Update();
	}

	private void Update()
	{
		if (!((Object)content.content == (Object)null))
		{
			bool flag = false;
			Vector2 vector = new Vector2(parent.rect.width, parent.rect.height);
			Vector2 sizeDelta = content.content.GetComponent<RectTransform>().sizeDelta;
			if ((sizeDelta.x >= vector.x && checkWidth) || (sizeDelta.y >= vector.y && checkHeight))
			{
				flag = true;
			}
			if (scrollbar.gameObject.activeSelf != flag)
			{
				scrollbar.gameObject.SetActive(flag);
				if (others != null)
				{
					GameObject[] array = others;
					foreach (GameObject gameObject in array)
					{
						gameObject.SetActive(flag);
					}
				}
			}
		}
	}
}
