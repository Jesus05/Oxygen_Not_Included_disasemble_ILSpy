using System.Collections.Generic;
using UnityEngine;

public class AccessorySlot : Resource
{
	private KAnimFile file;

	public KAnimHashedString targetSymbolId
	{
		get;
		private set;
	}

	public List<Accessory> accessories
	{
		get;
		private set;
	}

	public AccessorySlot(string id, ResourceSet parent, KAnimFile swap_build, string build_symbol_override = null)
		: base(id, parent, null)
	{
		if ((Object)swap_build == (Object)null)
		{
			Debug.LogErrorFormat("AccessorySlot {0} missing swap_build", id);
		}
		targetSymbolId = new KAnimHashedString("snapTo_" + id.ToLower());
		accessories = new List<Accessory>();
		file = swap_build;
	}

	public void AddAccessories(KAnimFile default_build, ResourceSet parent)
	{
		KAnim.Build build = file.GetData().build;
		KAnim.Build.Symbol symbol = default_build.GetData().build.GetSymbol(targetSymbolId);
		string text = Id.ToLower();
		if (symbol != null)
		{
			string id = text + "_DEFAULT";
			Accessory accessory = new Accessory(id, parent, this, default_build.batchTag, symbol);
			accessories.Add(accessory);
			HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);
		}
		for (int i = 0; i < build.symbols.Length; i++)
		{
			string text2 = HashCache.Get().Get(build.symbols[i].hash);
			if (text2.StartsWith(text))
			{
				Accessory accessory2 = new Accessory(text2, parent, this, file.batchTag, build.symbols[i]);
				accessories.Add(accessory2);
				HashCache.Get().Add(accessory2.IdHash.HashValue, accessory2.Id);
			}
		}
	}

	public Accessory Lookup(string id)
	{
		return Lookup(new HashedString(id));
	}

	public Accessory Lookup(HashedString full_id)
	{
		return accessories.Find((Accessory a) => a.IdHash == full_id);
	}
}
