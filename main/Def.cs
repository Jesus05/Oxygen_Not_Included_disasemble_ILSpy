using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Def : ScriptableObject
{
	public string PrefabID;

	public Tag Tag;

	private static Dictionary<Tuple<KAnimFile, string, bool>, Sprite> knownUISprites = new Dictionary<Tuple<KAnimFile, string, bool>, Sprite>();

	public virtual string Name => null;

	public virtual void InitDef()
	{
		Tag = TagManager.Create(PrefabID);
	}

	public static Tuple<Sprite, Color> GetUISprite(object item, string animName = "ui", bool centered = false)
	{
		if (item is Substance)
		{
			return GetUISprite(ElementLoader.FindElementByHash((item as Substance).elementID), animName, centered);
		}
		if (item is Element)
		{
			if ((item as Element).IsSolid)
			{
				return new Tuple<Sprite, Color>(GetUISpriteFromMultiObjectAnim((item as Element).substance.anim, animName, centered), Color.white);
			}
			if ((item as Element).IsLiquid)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_liquid"), (item as Element).substance.debugColour);
			}
			if ((item as Element).IsGas)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_gas"), (item as Element).substance.debugColour);
			}
			return new Tuple<Sprite, Color>(null, Color.clear);
		}
		if (item is GameObject)
		{
			GameObject gameObject = item as GameObject;
			if (ElementLoader.GetElement(gameObject.PrefabID()) != null)
			{
				return GetUISprite(ElementLoader.GetElement(gameObject.PrefabID()), animName, centered);
			}
			CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				animName = component.symbolPrefix + "ui";
			}
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			if ((bool)component2)
			{
				Sprite uISpriteFromMultiObjectAnim = GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], animName, centered);
				return new Tuple<Sprite, Color>(uISpriteFromMultiObjectAnim, (!((UnityEngine.Object)uISpriteFromMultiObjectAnim != (UnityEngine.Object)null)) ? Color.clear : Color.white);
			}
			if ((UnityEngine.Object)gameObject.GetComponent<Building>() != (UnityEngine.Object)null)
			{
				Sprite uISprite = gameObject.GetComponent<Building>().Def.GetUISprite(animName, centered);
				return new Tuple<Sprite, Color>(uISprite, (!((UnityEngine.Object)uISprite != (UnityEngine.Object)null)) ? Color.clear : Color.white);
			}
			Debug.LogWarningFormat("Can't get sprite for type {0} (no KBatchedAnimController)", item.ToString());
			return null;
		}
		if (item is string)
		{
			if (Db.Get().Amounts.Exists(item as string))
			{
				Amount amount = Db.Get().Amounts.Get(item as string);
				return new Tuple<Sprite, Color>(Assets.GetSprite(amount.uiSprite), Color.white);
			}
			if (Db.Get().Attributes.Exists(item as string))
			{
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(item as string);
				return new Tuple<Sprite, Color>(Assets.GetSprite(attribute.uiSprite), Color.white);
			}
			return GetUISprite((item as string).ToTag(), animName, centered);
		}
		if (item is Tag)
		{
			if (ElementLoader.GetElement((Tag)item) != null)
			{
				return GetUISprite(ElementLoader.GetElement((Tag)item), animName, centered);
			}
			if ((UnityEngine.Object)Assets.GetPrefab((Tag)item) != (UnityEngine.Object)null)
			{
				return GetUISprite(Assets.GetPrefab((Tag)item), animName, centered);
			}
			if ((UnityEngine.Object)Assets.GetSprite(((Tag)item).Name) != (UnityEngine.Object)null)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite(((Tag)item).Name), Color.white);
			}
		}
		Debug.LogErrorFormat("Can't get sprite for type {0}", item.ToString());
		return null;
	}

	public static Sprite GetUISpriteFromMultiObjectAnim(KAnimFile animFile, string animName = "ui", bool centered = false)
	{
		Tuple<KAnimFile, string, bool> key = new Tuple<KAnimFile, string, bool>(animFile, animName, centered);
		if (knownUISprites.ContainsKey(key))
		{
			return knownUISprites[key];
		}
		if ((UnityEngine.Object)animFile == (UnityEngine.Object)null)
		{
			Output.LogWarning(animName, "missing Anim File");
			return null;
		}
		KAnimFileData data = animFile.GetData();
		if (data == null)
		{
			Output.LogWarning(animName, "KAnimFileData is null");
			return null;
		}
		if (data.build == null)
		{
			return null;
		}
		KAnim.Anim.Frame frame = KAnim.Anim.Frame.InvalidFrame;
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.name == animName)
			{
				frame = anim.GetFrame(data.batchTag, 0);
			}
		}
		if (!frame.IsValid())
		{
			Output.LogWarning($"missing '{animName}' anim in '{animFile}'");
			return null;
		}
		if (data.elementCount == 0)
		{
			return null;
		}
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		KAnimHashedString symbolName = new KAnimHashedString(animName);
		frameElement = data.FindAnimFrameElement(symbolName);
		KAnim.Build.Symbol symbol = data.build.GetSymbol(frameElement.symbol);
		if (symbol == null)
		{
			Output.LogWarning(animFile.name, animName, "placeSymbol [", frameElement.symbol, "] is missing");
			return null;
		}
		KAnim.Build.SymbolFrameInstance frame2 = symbol.GetFrame(frameElement.frame);
		KAnim.Build.SymbolFrame symbolFrame = frame2.symbolFrame;
		if (symbolFrame == null)
		{
			Output.LogWarning(animName, "SymbolFrame [", frameElement.frame, "] is missing");
			return null;
		}
		Texture2D texture = data.build.GetTexture(0);
		float x = symbolFrame.uvMin.x;
		float x2 = symbolFrame.uvMax.x;
		float y = symbolFrame.uvMax.y;
		float y2 = symbolFrame.uvMin.y;
		int num = (int)((float)texture.width * Mathf.Abs(x2 - x));
		int num2 = (int)((float)texture.height * Mathf.Abs(y2 - y));
		float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
		Rect rect = default(Rect);
		rect.width = (float)num;
		rect.height = (float)num2;
		rect.x = (float)(int)((float)texture.width * x);
		rect.y = (float)(int)((float)texture.height * y);
		float pixelsPerUnit = 100f;
		if (num != 0)
		{
			pixelsPerUnit = 100f / (num3 / (float)num);
		}
		Sprite sprite = Sprite.Create(texture, rect, (!centered) ? Vector2.zero : new Vector2(0.5f, 0.5f), pixelsPerUnit, 0u, SpriteMeshType.FullRect);
		sprite.name = $"{texture.name}:{animName}:{frameElement.frame.ToString()}:{centered}";
		knownUISprites[key] = sprite;
		return sprite;
	}
}
