using System;
using System.Collections.Generic;

public class SnapOn : KMonoBehaviour
{
	[Serializable]
	public class SnapPoint
	{
		public string pointName;

		public bool automatic = true;

		public HashedString context;

		public KAnimFile buildFile;

		public HashedString overrideSymbol;
	}

	public class OverrideEntry
	{
		public KAnimFile buildFile;

		public string symbolName;
	}

	private KAnimControllerBase kanimController;

	public List<SnapPoint> snapPoints = new List<SnapPoint>();

	private Dictionary<string, OverrideEntry> overrideMap = new Dictionary<string, OverrideEntry>();

	protected override void OnPrefabInit()
	{
		kanimController = GetComponent<KAnimControllerBase>();
	}

	protected override void OnSpawn()
	{
		foreach (SnapPoint snapPoint in snapPoints)
		{
			if (snapPoint.automatic)
			{
				DoAttachSnapOn(snapPoint);
			}
		}
	}

	public void AttachSnapOnByName(string name)
	{
		foreach (SnapPoint snapPoint in snapPoints)
		{
			if (snapPoint.pointName == name)
			{
				HashedString context = GetComponent<AnimEventHandler>().GetContext();
				if (!context.IsValid || !snapPoint.context.IsValid || context == snapPoint.context)
				{
					DoAttachSnapOn(snapPoint);
				}
			}
		}
	}

	public void DetachSnapOnByName(string name)
	{
		foreach (SnapPoint snapPoint in snapPoints)
		{
			if (snapPoint.pointName == name)
			{
				HashedString context = GetComponent<AnimEventHandler>().GetContext();
				if (!context.IsValid || !snapPoint.context.IsValid || context == snapPoint.context)
				{
					SymbolOverrideController component = GetComponent<SymbolOverrideController>();
					component.RemoveSymbolOverride(snapPoint.overrideSymbol, 5);
					kanimController.SetSymbolVisiblity(snapPoint.overrideSymbol, false);
					break;
				}
			}
		}
	}

	private void DoAttachSnapOn(SnapPoint point)
	{
		OverrideEntry value = null;
		KAnimFile buildFile = point.buildFile;
		string symbol_name = string.Empty;
		if (overrideMap.TryGetValue(point.pointName, out value))
		{
			buildFile = value.buildFile;
			symbol_name = value.symbolName;
		}
		KAnim.Build.Symbol symbol = GetSymbol(buildFile, symbol_name);
		SymbolOverrideController component = GetComponent<SymbolOverrideController>();
		component.AddSymbolOverride(point.overrideSymbol, symbol, 5);
		kanimController.SetSymbolVisiblity(point.overrideSymbol, true);
	}

	private static KAnim.Build.Symbol GetSymbol(KAnimFile anim_file, string symbol_name)
	{
		KAnim.Build.Symbol result = anim_file.GetData().build.symbols[0];
		KAnimHashedString y = new KAnimHashedString(symbol_name);
		KAnim.Build.Symbol[] symbols = anim_file.GetData().build.symbols;
		foreach (KAnim.Build.Symbol symbol in symbols)
		{
			if (symbol.hash == y)
			{
				result = symbol;
				break;
			}
		}
		return result;
	}

	public void AddOverride(string point_name, KAnimFile build_override, string symbol_name)
	{
		overrideMap[point_name] = new OverrideEntry
		{
			buildFile = build_override,
			symbolName = symbol_name
		};
	}

	public void RemoveOverride(string point_name)
	{
		overrideMap.Remove(point_name);
	}
}
