using System;
using UnityEngine;

public class TreeFilterableSideScreenElement : KMonoBehaviour
{
	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private KToggle checkBox;

	[SerializeField]
	private KImage elementImg;

	private KImage checkBoxImg;

	private Tag elementTag;

	private TreeFilterableSideScreen parent;

	private bool initialized = false;

	public bool IsSelected => checkBox.isOn;

	public TreeFilterableSideScreen Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	public event Action<Tag, bool> OnSelectionChanged;

	public Tag GetElementTag()
	{
		return elementTag;
	}

	public KToggle GetCheckboxToggle()
	{
		return checkBox;
	}

	private void Initialize()
	{
		if (!initialized)
		{
			checkBoxImg = checkBox.gameObject.GetComponentInChildrenOnly<KImage>();
			checkBox.onClick += CheckBoxClicked;
			initialized = true;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Initialize();
	}

	public Sprite GetStorageObjectSprite(Tag t)
	{
		Sprite result = null;
		GameObject prefab = Assets.GetPrefab(t);
		if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
		{
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				result = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false);
			}
		}
		return result;
	}

	public void SetSprite(Tag t)
	{
		Element element = ElementLoader.GetElement(t);
		Sprite sprite = (element == null) ? GetStorageObjectSprite(t) : Def.GetUISpriteFromMultiObjectAnim(element.substance.anim, "ui", false);
		elementImg.sprite = sprite;
		elementImg.enabled = ((UnityEngine.Object)sprite != (UnityEngine.Object)null);
	}

	public void SetTag(Tag newTag)
	{
		Initialize();
		elementTag = newTag;
		string text = elementTag.ProperName();
		if (parent.IsStorage)
		{
			float amountInStorage = parent.GetAmountInStorage(elementTag);
			text = text + ": " + GameUtil.GetFormattedMass(amountInStorage, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		}
		elementName.text = text;
	}

	private void CheckBoxClicked()
	{
		SetCheckBox(!parent.IsTagAllowed(GetElementTag()));
	}

	public void SetCheckBox(bool checkBoxState)
	{
		checkBox.isOn = checkBoxState;
		checkBoxImg.enabled = checkBoxState;
		if (this.OnSelectionChanged != null)
		{
			this.OnSelectionChanged(GetElementTag(), checkBoxState);
		}
	}
}
