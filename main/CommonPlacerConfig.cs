using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CommonPlacerConfig
{
	[Serializable]
	public class CommonPlacerAssets
	{
		public Mesh mesh;
	}

	public GameObject CreatePrefab(string id, string name, Material default_material)
	{
		GameObject gameObject = EntityTemplates.CreateEntity(id, name, true);
		gameObject.layer = LayerMask.NameToLayer("PlaceWithDepth");
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<StateMachineController>();
		Prioritizable prioritizable = gameObject.AddOrGet<Prioritizable>();
		prioritizable.iconOffset = new Vector2(0.3f, 0.32f);
		KBoxCollider2D kBoxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kBoxCollider2D.offset = new Vector2(0f, 0.5f);
		kBoxCollider2D.size = new Vector2(1f, 1f);
		GameObject gameObject2 = new GameObject("Mask");
		gameObject2.layer = LayerMask.NameToLayer("PlaceWithDepth");
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.SetLocalPosition(new Vector3(0f, 0.5f, -3.537f));
		gameObject2.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = Assets.instance.commonPlacerAssets.mesh;
		MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
		meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		meshRenderer.sharedMaterial = default_material;
		EasingAnimations easingAnimations = gameObject2.AddComponent<EasingAnimations>();
		EasingAnimations.AnimationScales animationScales = default(EasingAnimations.AnimationScales);
		animationScales.name = "ScaleUp";
		animationScales.startScale = 0f;
		animationScales.endScale = 1f;
		animationScales.type = EasingAnimations.AnimationScales.AnimationType.EaseInOutBack;
		animationScales.easingMultiplier = 5f;
		EasingAnimations.AnimationScales animationScales2 = default(EasingAnimations.AnimationScales);
		animationScales2.name = "ScaleDown";
		animationScales2.startScale = 1f;
		animationScales2.endScale = 0f;
		animationScales2.type = EasingAnimations.AnimationScales.AnimationType.EaseOutBack;
		animationScales2.easingMultiplier = 1f;
		easingAnimations.scales = new EasingAnimations.AnimationScales[2]
		{
			animationScales,
			animationScales2
		};
		return gameObject;
	}
}
