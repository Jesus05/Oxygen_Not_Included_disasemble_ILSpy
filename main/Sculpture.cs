using UnityEngine;

public class Sculpture : Artable
{
	private static KAnimFile[] sculptureOverrides;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (sculptureOverrides == null)
		{
			sculptureOverrides = new KAnimFile[1]
			{
				Assets.GetAnim("anim_interacts_sculpture_kanim")
			};
		}
		overrideAnims = sculptureOverrides;
		synchronizeAnims = false;
	}

	public override void SetStage(string stage_id, bool skip_effect)
	{
		base.SetStage(stage_id, skip_effect);
		if (!skip_effect && base.CurrentStage != "Default")
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("sculpture_fx_kanim", base.transform.GetPosition(), base.transform, false, Grid.SceneLayer.Front, false);
			kBatchedAnimController.destroyOnAnimComplete = true;
			kBatchedAnimController.transform.SetLocalPosition(Vector3.zero);
			kBatchedAnimController.Play("poof", KAnim.PlayMode.Once, 1f, 0f);
		}
	}
}
