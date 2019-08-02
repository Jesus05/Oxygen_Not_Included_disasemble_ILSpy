using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Accessorizer : KMonoBehaviour
{
	[Serialize]
	private List<ResourceRef<Accessory>> accessories = new List<ResourceRef<Accessory>>();

	[MyCmpReq]
	private KAnimControllerBase animController;

	public List<ResourceRef<Accessory>> GetAccessories()
	{
		return accessories;
	}

	public void SetAccessories(List<ResourceRef<Accessory>> data)
	{
		accessories = data;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		ApplyAccessories();
	}

	public void AddAccessory(Accessory accessory)
	{
		if (accessory != null)
		{
			SymbolOverrideController component = animController.GetComponent<SymbolOverrideController>();
			component.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol, 0);
			if (!HasAccessory(accessory))
			{
				ResourceRef<Accessory> resourceRef = new ResourceRef<Accessory>(accessory);
				if (resourceRef != null)
				{
					accessories.Add(resourceRef);
				}
			}
		}
	}

	public void RemoveAccessory(Accessory accessory)
	{
		accessories.RemoveAll((ResourceRef<Accessory> x) => x.Get() == accessory);
		SymbolOverrideController component = animController.GetComponent<SymbolOverrideController>();
		component.TryRemoveSymbolOverride(accessory.slot.targetSymbolId, 0);
	}

	public void ApplyAccessories()
	{
		foreach (ResourceRef<Accessory> accessory2 in accessories)
		{
			Accessory accessory = accessory2.Get();
			if (accessory != null)
			{
				AddAccessory(accessory);
			}
		}
	}

	public bool HasAccessory(Accessory accessory)
	{
		return accessories.Exists((ResourceRef<Accessory> x) => x.Get() == accessory);
	}

	public Accessory GetAccessory(AccessorySlot slot)
	{
		for (int i = 0; i < accessories.Count; i++)
		{
			if (accessories[i].Get() != null && accessories[i].Get().slot == slot)
			{
				return accessories[i].Get();
			}
		}
		return null;
	}

	public void GetBodySlots(ref KCompBuilder.BodyData fd)
	{
		fd.eyes = HashedString.Invalid;
		fd.hair = HashedString.Invalid;
		fd.headShape = HashedString.Invalid;
		fd.mouth = HashedString.Invalid;
		fd.neck = HashedString.Invalid;
		fd.body = HashedString.Invalid;
		fd.arms = HashedString.Invalid;
		fd.hat = HashedString.Invalid;
		for (int i = 0; i < accessories.Count; i++)
		{
			Accessory accessory = accessories[i].Get();
			if (accessory != null)
			{
				if (accessory.slot.Id == "Eyes")
				{
					fd.eyes = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Hair")
				{
					fd.hair = accessory.IdHash;
				}
				else if (accessory.slot.Id == "HeadShape")
				{
					fd.headShape = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Mouth")
				{
					fd.mouth = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Neck")
				{
					fd.neck = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Body")
				{
					fd.body = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Arm")
				{
					fd.arms = accessory.IdHash;
				}
				else if (accessory.slot.Id == "Hat")
				{
					fd.hat = HashedString.Invalid;
				}
			}
		}
	}
}
