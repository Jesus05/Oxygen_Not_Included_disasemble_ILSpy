using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class SubstanceSource : KMonoBehaviour
{
	private bool enableRefresh;

	private static readonly float MaxPickupTime = 8f;

	[MyCmpReq]
	public Pickupable pickupable;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	protected override void OnPrefabInit()
	{
		pickupable.SetWorkTime(MaxPickupTime);
	}

	protected override void OnSpawn()
	{
		pickupable.SetWorkTime(10f);
	}

	protected abstract CellOffset[] GetOffsetGroup();

	protected abstract IChunkManager GetChunkManager();

	public SimHashes GetElementID()
	{
		return primaryElement.ElementID;
	}

	public Tag GetElementTag()
	{
		Tag result = Tag.Invalid;
		if ((Object)base.gameObject != (Object)null && (Object)primaryElement != (Object)null && primaryElement.Element != null)
		{
			result = primaryElement.Element.tag;
		}
		return result;
	}

	public Tag GetMaterialCategoryTag()
	{
		Tag result = Tag.Invalid;
		if ((Object)base.gameObject != (Object)null && (Object)primaryElement != (Object)null && primaryElement.Element != null)
		{
			result = primaryElement.Element.GetMaterialCategoryTag();
		}
		return result;
	}
}
