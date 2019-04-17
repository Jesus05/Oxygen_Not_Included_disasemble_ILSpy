using UnityEngine;

namespace Klei.AI
{
	public class CustomSickEffectSickness : Sickness.SicknessComponent
	{
		private string kanim;

		private string animName;

		public CustomSickEffectSickness(string effect_kanim, string effect_anim_name)
		{
			kanim = effect_kanim;
			animName = effect_anim_name;
		}

		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect(kanim, go.transform.GetPosition() + new Vector3(0f, 0f, -0.1f), go.transform, true, Grid.SceneLayer.Front, false);
			kBatchedAnimController.Play(animName, KAnim.PlayMode.Loop, 1f, 0f);
			return kBatchedAnimController;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			KAnimControllerBase kAnimControllerBase = (KAnimControllerBase)instance_data;
			kAnimControllerBase.gameObject.DeleteObject();
		}
	}
}
