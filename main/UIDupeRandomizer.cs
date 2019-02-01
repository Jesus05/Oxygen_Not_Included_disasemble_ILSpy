using Database;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDupeRandomizer : MonoBehaviour
{
	[Serializable]
	public struct AnimChoice
	{
		public string anim_name;

		public KBatchedAnimController minon;

		public float minSecondsBetweenAction;

		public float maxSecondsBetweenAction;

		public float lastWaitTime;

		public KAnimFile curBody;

		public KAnimFile target_minion_anim;

		public bool overrideSet;
	}

	public KAnimFile head_default_anim;

	public KAnimFile head_swap_anim;

	public KAnimFile body_swap_anim;

	public bool applyHat = true;

	public AnimChoice[] anims;

	private AccessorySlots slots = null;

	protected virtual void Start()
	{
		slots = new AccessorySlots(null, head_default_anim, head_swap_anim, body_swap_anim);
		for (int i = 0; i < anims.Length; i++)
		{
			anims[i].curBody = null;
			anims[i].overrideSet = false;
			GetNewBody(i);
		}
	}

	protected void GetNewBody(int minion_idx)
	{
		Apply(anims[minion_idx].minon, ref anims[minion_idx]);
	}

	private void Apply(KBatchedAnimController dupe, ref AnimChoice anim)
	{
		int idx = UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
		Personality p = Db.Get().Personalities[idx];
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(p);
		SymbolOverrideController component = dupe.GetComponent<SymbolOverrideController>();
		component.RemoveAllSymbolOverrides(0);
		AddAccessory(dupe, slots.Hair.Lookup(bodyData.hair));
		AddAccessory(dupe, slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)));
		AddAccessory(dupe, slots.Eyes.Lookup(bodyData.eyes));
		AddAccessory(dupe, slots.HeadShape.Lookup(bodyData.headShape));
		AddAccessory(dupe, slots.Mouth.Lookup(bodyData.mouth));
		AddAccessory(dupe, slots.Body.Lookup(bodyData.body));
		AddAccessory(dupe, slots.Arm.Lookup(bodyData.arms));
		if (UnityEngine.Random.value < 0.15f)
		{
			component.AddBuildOverride(Assets.GetAnim("body_oxygen_kanim").GetData(), 6);
			component.AddBuildOverride(Assets.GetAnim("helm_oxygen_kanim").GetData(), 6);
			dupe.SetSymbolVisiblity("snapto_neck", true);
		}
		else
		{
			dupe.SetSymbolVisiblity("snapto_neck", false);
		}
		if (applyHat && UnityEngine.Random.value < 0.5f)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> item in RoleManager.roleHatIndex)
			{
				list.Add(item.Value);
			}
			string id = list[UnityEngine.Random.Range(0, list.Count)];
			AddAccessory(dupe, slots.Hat.Lookup(id));
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
		}
		else
		{
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
			dupe.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, false);
		}
		if (!anim.overrideSet)
		{
			dupe.AddAnimOverrides(anim.target_minion_anim, 0f);
			anim.overrideSet = true;
		}
	}

	public static KAnimHashedString AddAccessory(KBatchedAnimController minion, Accessory accessory)
	{
		if (accessory == null)
		{
			return HashedString.Invalid;
		}
		SymbolOverrideController component = minion.GetComponent<SymbolOverrideController>();
		DebugUtil.Assert((UnityEngine.Object)component != (UnityEngine.Object)null, minion.name + " is missing symbol override controller");
		component.TryRemoveSymbolOverride(accessory.slot.targetSymbolId, 0);
		component.AddSymbolOverride(accessory.slot.targetSymbolId, accessory.symbol, 0);
		minion.SetSymbolVisiblity(accessory.slot.targetSymbolId, true);
		return accessory.slot.targetSymbolId;
	}

	public KAnimHashedString AddRandomAccessory(KBatchedAnimController minion, List<Accessory> choices)
	{
		Accessory accessory = choices[UnityEngine.Random.Range(1, choices.Count)];
		return AddAccessory(minion, accessory);
	}

	protected virtual void Update()
	{
	}
}
