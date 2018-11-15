using UnityEngine;

namespace Database
{
	public class AccessorySlots : ResourceSet<AccessorySlot>
	{
		public AccessorySlot Eyes;

		public AccessorySlot Hair;

		public AccessorySlot HeadShape;

		public AccessorySlot Mouth;

		public AccessorySlot Body;

		public AccessorySlot Arm;

		public AccessorySlot Hat;

		public AccessorySlot HatHair;

		public AccessorySlot HairAlways;

		public AccessorySlots(ResourceSet parent, KAnimFile default_build = null, KAnimFile swap_build = null, KAnimFile torso_swap_build = null)
			: base("AccessorySlots", parent)
		{
			if ((Object)swap_build == (Object)null)
			{
				swap_build = Assets.GetAnim("head_swap_kanim");
				parent = Db.Get().Accessories;
			}
			if ((Object)default_build == (Object)null)
			{
				default_build = Assets.GetAnim("body_comp_default_kanim");
			}
			if ((Object)torso_swap_build == (Object)null)
			{
				torso_swap_build = Assets.GetAnim("body_swap_kanim");
			}
			Eyes = new AccessorySlot("Eyes", this, swap_build, null);
			Hair = new AccessorySlot("Hair", this, swap_build, null);
			HeadShape = new AccessorySlot("HeadShape", this, swap_build, null);
			Mouth = new AccessorySlot("Mouth", this, swap_build, null);
			Hat = new AccessorySlot("Hat", this, swap_build, null);
			HatHair = new AccessorySlot("Hat_Hair", this, swap_build, null);
			HairAlways = new AccessorySlot("Hair_Always", this, swap_build, "hair");
			Body = new AccessorySlot("Body", this, torso_swap_build, null);
			Arm = new AccessorySlot("Arm", this, torso_swap_build, null);
			foreach (AccessorySlot resource in resources)
			{
				resource.AddAccessories(default_build, parent);
			}
		}
	}
}
