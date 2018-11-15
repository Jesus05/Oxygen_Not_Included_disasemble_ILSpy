using UnityEngine;
using UnityEngine.UI;

public class TitleBarPortrait : KMonoBehaviour
{
	public GameObject FaceObject;

	public GameObject ImageObject;

	public GameObject PortraitShadow;

	public GameObject AnimControllerObject;

	public Material DefaultMaterial;

	public Material DesatMaterial;

	public void SetSaturation(bool saturated)
	{
		ImageObject.GetComponent<Image>().material = ((!saturated) ? DesatMaterial : DefaultMaterial);
	}

	public void SetPortrait(GameObject selectedTarget)
	{
		MinionIdentity component = selectedTarget.GetComponent<MinionIdentity>();
		if ((Object)component != (Object)null)
		{
			SetPortrait(component);
		}
		else
		{
			Building component2 = selectedTarget.GetComponent<Building>();
			if ((Object)component2 != (Object)null)
			{
				SetPortrait(component2.Def.GetUISprite("ui", false));
			}
			else
			{
				MeshRenderer componentInChildren = selectedTarget.GetComponentInChildren<MeshRenderer>();
				if ((bool)componentInChildren)
				{
					SetPortrait(Sprite.Create((Texture2D)componentInChildren.material.mainTexture, new Rect(0f, 0f, (float)componentInChildren.material.mainTexture.width, (float)componentInChildren.material.mainTexture.height), new Vector2(0.5f, 0.5f)));
				}
			}
		}
	}

	public void SetPortrait(Sprite image)
	{
		if ((bool)PortraitShadow)
		{
			PortraitShadow.SetActive(true);
		}
		if ((bool)FaceObject)
		{
			FaceObject.SetActive(false);
		}
		if ((bool)ImageObject)
		{
			ImageObject.SetActive(true);
		}
		if ((bool)AnimControllerObject)
		{
			AnimControllerObject.SetActive(false);
		}
		if ((Object)image == (Object)null)
		{
			ClearPortrait();
		}
		else
		{
			ImageObject.GetComponent<Image>().sprite = image;
		}
	}

	private void SetPortrait(MinionIdentity identity)
	{
		if ((bool)PortraitShadow)
		{
			PortraitShadow.SetActive(true);
		}
		if ((bool)FaceObject)
		{
			FaceObject.SetActive(false);
		}
		if ((bool)ImageObject)
		{
			ImageObject.SetActive(false);
		}
		CrewPortrait component = GetComponent<CrewPortrait>();
		if ((Object)component != (Object)null)
		{
			component.SetIdentityObject(identity, true);
		}
		else if ((bool)AnimControllerObject)
		{
			AnimControllerObject.SetActive(true);
			CrewPortrait.SetPortraitData(identity, AnimControllerObject.GetComponent<KBatchedAnimController>(), true);
		}
	}

	public void ClearPortrait()
	{
		if ((bool)PortraitShadow)
		{
			PortraitShadow.SetActive(false);
		}
		if ((bool)FaceObject)
		{
			FaceObject.SetActive(false);
		}
		if ((bool)ImageObject)
		{
			ImageObject.SetActive(false);
		}
		if ((bool)AnimControllerObject)
		{
			AnimControllerObject.SetActive(false);
		}
	}
}
