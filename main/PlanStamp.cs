using System;
using UnityEngine;
using UnityEngine.UI;

public class PlanStamp : KMonoBehaviour
{
	[Serializable]
	public struct StampArt
	{
		public Sprite UnderConstruction;

		public Sprite NeedsResearch;

		public Sprite SelectResource;

		public Sprite NeedsRepair;

		public Sprite NeedsPower;

		public Sprite NeedsResource;

		public Sprite NeedsGasPipe;

		public Sprite NeedsLiquidPipe;
	}

	public StampArt stampSprites;

	[SerializeField]
	private Image StampImage;

	[SerializeField]
	private Text StampText;

	public void SetStamp(Sprite sprite, string Text)
	{
		StampImage.sprite = sprite;
		StampText.text = Text.ToUpper();
	}
}
