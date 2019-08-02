using System.Collections.Generic;

public class FaceGraph : KMonoBehaviour
{
	private List<Expression> expressions = new List<Expression>();

	private static KAnimHashedString HASH_SNAPTO_EYES = "snapto_eyes";

	private static KAnimHashedString HASH_NEUTRAL = "neutral";

	private static int FIRST_SIDEWAYS_FRAME = 29;

	public Expression overrideExpression
	{
		get;
		private set;
	}

	public Expression currentExpression
	{
		get;
		private set;
	}

	public IEnumerator<Expression> GetEnumerator()
	{
		return expressions.GetEnumerator();
	}

	public void AddExpression(Expression expression)
	{
		if (!expressions.Contains(expression))
		{
			expressions.Add(expression);
			UpdateFace();
		}
	}

	public void RemoveExpression(Expression expression)
	{
		if (expressions.Remove(expression))
		{
			UpdateFace();
		}
	}

	public void SetOverrideExpression(Expression expression)
	{
		if (expression != overrideExpression)
		{
			overrideExpression = expression;
			UpdateFace();
		}
	}

	public void ApplyShape()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Accessorizer component2 = GetComponent<Accessorizer>();
		KAnimFile anim = Assets.GetAnim("head_master_swap_kanim");
		bool should_use_sideways_symbol = ShouldUseSidewaysSymbol(component);
		BlinkMonitor.Instance sMI = component2.GetSMI<BlinkMonitor.Instance>();
		if (sMI.IsNullOrStopped() || !sMI.IsBlinking())
		{
			Accessory accessory = component2.GetAccessory(Db.Get().AccessorySlots.Eyes);
			KAnim.Build.Symbol symbol = accessory.symbol;
			ApplyShape(symbol, component, anim, "snapto_eyes", should_use_sideways_symbol);
		}
		SpeechMonitor.Instance sMI2 = component2.GetSMI<SpeechMonitor.Instance>();
		if (sMI2.IsNullOrStopped() || !sMI2.IsPlayingSpeech())
		{
			Accessory accessory2 = component2.GetAccessory(Db.Get().AccessorySlots.Mouth);
			KAnim.Build.Symbol symbol2 = accessory2.symbol;
			ApplyShape(symbol2, component, anim, "snapto_mouth", should_use_sideways_symbol);
		}
		else
		{
			sMI2.DrawMouth();
		}
	}

	private bool ShouldUseSidewaysSymbol(KBatchedAnimController controller)
	{
		KAnim.Anim currentAnim = controller.GetCurrentAnim();
		if (currentAnim == null)
		{
			return false;
		}
		int currentFrameIndex = controller.GetCurrentFrameIndex();
		if (currentFrameIndex <= 0)
		{
			return false;
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(currentAnim.animFile.animBatchTag);
		KAnim.Anim.Frame frame = batchGroupData.GetFrame(currentFrameIndex);
		for (int i = 0; i < frame.numElements; i++)
		{
			KAnim.Anim.FrameElement frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + i);
			if (frameElement.symbol == HASH_SNAPTO_EYES && frameElement.frame >= FIRST_SIDEWAYS_FRAME)
			{
				return true;
			}
		}
		return false;
	}

	private void ApplyShape(KAnim.Build.Symbol variation_symbol, KBatchedAnimController controller, KAnimFile shapes_file, HashedString symbol_name_in_shape_file, bool should_use_sideways_symbol)
	{
		HashedString hashedString = HASH_NEUTRAL;
		if (currentExpression != null)
		{
			hashedString = currentExpression.face.hash;
		}
		KAnim.Anim anim = null;
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < shapes_file.GetData().animCount; i++)
		{
			if (flag)
			{
				break;
			}
			KAnim.Anim anim2 = shapes_file.GetData().GetAnim(i);
			if (anim2.hash == hashedString)
			{
				anim = anim2;
				KAnim.Anim.Frame frame = anim.GetFrame(shapes_file.GetData().build.batchTag, 0);
				for (int j = 0; j < frame.numElements; j++)
				{
					KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(shapes_file.GetData().animBatchTag);
					frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + j);
					if (!(frameElement.symbol != symbol_name_in_shape_file))
					{
						if (flag2 || !should_use_sideways_symbol)
						{
							flag = true;
						}
						flag2 = true;
						break;
					}
				}
			}
		}
		if (anim == null)
		{
			DebugUtil.Assert(false, "Could not find shape for expression: " + HashCache.Get().Get(hashedString));
		}
		if (!flag2)
		{
			DebugUtil.Assert(false, "Could not find shape element for shape:" + HashCache.Get().Get(variation_symbol.hash));
		}
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(controller.batchGroupID).GetSymbol(symbol_name_in_shape_file);
		KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(variation_symbol.build.batchTag);
		KAnim.Build.SymbolFrameInstance symbol_frame_instance = batchGroupData2.symbolFrameInstances[variation_symbol.firstFrameIdx + frameElement.frame];
		symbol_frame_instance.buildImageIdx = GetComponent<SymbolOverrideController>().GetAtlasIdx(variation_symbol.build.GetTexture(0));
		controller.SetSymbolOverride(symbol.firstFrameIdx, symbol_frame_instance);
	}

	private void UpdateFace()
	{
		Expression expression = null;
		if (overrideExpression != null)
		{
			expression = overrideExpression;
		}
		else if (expressions.Count > 0)
		{
			expressions.Sort((Expression a, Expression b) => b.priority.CompareTo(a.priority));
			expression = expressions[0];
		}
		if (expression != currentExpression || expression == null)
		{
			currentExpression = expression;
			GetComponent<SymbolOverrideController>().MarkDirty();
		}
	}

	public Expression GetCurrentExpression()
	{
		return currentExpression;
	}
}
