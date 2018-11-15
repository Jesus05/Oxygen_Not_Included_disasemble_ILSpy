using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	[ExecuteInEditMode]
	public class TMP_SubMeshUI : MaskableGraphic, IClippable, IMaskable, IMaterialModifier
	{
		[SerializeField]
		private TMP_FontAsset m_fontAsset;

		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		private Material m_material;

		[SerializeField]
		private Material m_sharedMaterial;

		private Material m_fallbackMaterial;

		private Material m_fallbackSourceMaterial;

		[SerializeField]
		private bool m_isDefaultMaterial;

		[SerializeField]
		private float m_padding;

		[SerializeField]
		private CanvasRenderer m_canvasRenderer;

		private Mesh m_mesh;

		[SerializeField]
		private TextMeshProUGUI m_TextComponent;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		private bool m_materialDirty;

		[SerializeField]
		private int m_materialReferenceIndex;

		public TMP_FontAsset fontAsset
		{
			get
			{
				return m_fontAsset;
			}
			set
			{
				m_fontAsset = value;
			}
		}

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return m_spriteAsset;
			}
			set
			{
				m_spriteAsset = value;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if ((UnityEngine.Object)sharedMaterial != (UnityEngine.Object)null)
				{
					return sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex);
				}
				return null;
			}
		}

		public override Material material
		{
			get
			{
				return GetMaterial(m_sharedMaterial);
			}
			set
			{
				if (!((UnityEngine.Object)m_sharedMaterial != (UnityEngine.Object)null) || m_sharedMaterial.GetInstanceID() != value.GetInstanceID())
				{
					m_sharedMaterial = (m_material = value);
					m_padding = GetPaddingForMaterial();
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Material sharedMaterial
		{
			get
			{
				return m_sharedMaterial;
			}
			set
			{
				SetSharedMaterial(value);
			}
		}

		public Material fallbackMaterial
		{
			get
			{
				return m_fallbackMaterial;
			}
			set
			{
				if (!((UnityEngine.Object)m_fallbackMaterial == (UnityEngine.Object)value))
				{
					if ((UnityEngine.Object)m_fallbackMaterial != (UnityEngine.Object)null && (UnityEngine.Object)m_fallbackMaterial != (UnityEngine.Object)value)
					{
						TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
					}
					m_fallbackMaterial = value;
					TMP_MaterialManager.AddFallbackMaterialReference(m_fallbackMaterial);
					SetSharedMaterial(m_fallbackMaterial);
				}
			}
		}

		public Material fallbackSourceMaterial
		{
			get
			{
				return m_fallbackSourceMaterial;
			}
			set
			{
				m_fallbackSourceMaterial = value;
			}
		}

		public override Material materialForRendering => TMP_MaterialManager.GetMaterialForRendering(this, m_sharedMaterial);

		public bool isDefaultMaterial
		{
			get
			{
				return m_isDefaultMaterial;
			}
			set
			{
				m_isDefaultMaterial = value;
			}
		}

		public float padding
		{
			get
			{
				return m_padding;
			}
			set
			{
				m_padding = value;
			}
		}

		public new CanvasRenderer canvasRenderer
		{
			get
			{
				if ((UnityEngine.Object)m_canvasRenderer == (UnityEngine.Object)null)
				{
					m_canvasRenderer = GetComponent<CanvasRenderer>();
				}
				return m_canvasRenderer;
			}
		}

		public Mesh mesh
		{
			get
			{
				if ((UnityEngine.Object)m_mesh == (UnityEngine.Object)null)
				{
					m_mesh = new Mesh();
					m_mesh.hideFlags = HideFlags.HideAndDontSave;
				}
				return m_mesh;
			}
			set
			{
				m_mesh = value;
			}
		}

		public static TMP_SubMeshUI AddSubTextObject(TextMeshProUGUI textComponent, MaterialReference materialReference)
		{
			GameObject gameObject = new GameObject("TMP UI SubObject [" + materialReference.material.name + "]", typeof(RectTransform));
			gameObject.transform.SetParent(textComponent.transform, false);
			gameObject.layer = textComponent.gameObject.layer;
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.sizeDelta = Vector2.zero;
			component.pivot = textComponent.rectTransform.pivot;
			TMP_SubMeshUI tMP_SubMeshUI = gameObject.AddComponent<TMP_SubMeshUI>();
			tMP_SubMeshUI.m_canvasRenderer = tMP_SubMeshUI.canvasRenderer;
			tMP_SubMeshUI.m_TextComponent = textComponent;
			tMP_SubMeshUI.m_materialReferenceIndex = materialReference.index;
			tMP_SubMeshUI.m_fontAsset = materialReference.fontAsset;
			tMP_SubMeshUI.m_spriteAsset = materialReference.spriteAsset;
			tMP_SubMeshUI.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			tMP_SubMeshUI.SetSharedMaterial(materialReference.material);
			return tMP_SubMeshUI;
		}

		protected override void OnEnable()
		{
			if (!m_isRegisteredForEvents)
			{
				m_isRegisteredForEvents = true;
			}
			m_ShouldRecalculateStencil = true;
			RecalculateClipping();
			RecalculateMasking();
		}

		protected override void OnDisable()
		{
			TMP_UpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if ((UnityEngine.Object)m_MaskMaterial != (UnityEngine.Object)null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(m_MaskMaterial);
				m_MaskMaterial = null;
			}
			if ((UnityEngine.Object)m_fallbackMaterial != (UnityEngine.Object)null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				m_fallbackMaterial = null;
			}
			base.OnDisable();
		}

		protected override void OnDestroy()
		{
			if ((UnityEngine.Object)m_mesh != (UnityEngine.Object)null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
			if ((UnityEngine.Object)m_MaskMaterial != (UnityEngine.Object)null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(m_MaskMaterial);
			}
			if ((UnityEngine.Object)m_fallbackMaterial != (UnityEngine.Object)null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				m_fallbackMaterial = null;
			}
			m_isRegisteredForEvents = false;
			RecalculateClipping();
		}

		protected override void OnTransformParentChanged()
		{
			if (IsActive())
			{
				m_ShouldRecalculateStencil = true;
				RecalculateClipping();
				RecalculateMasking();
			}
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (m_ShouldRecalculateStencil)
			{
				m_StencilValue = TMP_MaterialManager.GetStencilID(base.gameObject);
				m_ShouldRecalculateStencil = false;
			}
			if (m_StencilValue > 0)
			{
				material = TMP_MaterialManager.GetStencilMaterial(baseMaterial, m_StencilValue);
				if ((UnityEngine.Object)m_MaskMaterial != (UnityEngine.Object)null)
				{
					TMP_MaterialManager.ReleaseStencilMaterial(m_MaskMaterial);
				}
				m_MaskMaterial = material;
			}
			return material;
		}

		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(m_sharedMaterial, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
		}

		public float GetPaddingForMaterial(Material mat)
		{
			return ShaderUtilities.GetPadding(mat, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
		}

		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		public override void SetAllDirty()
		{
		}

		public override void SetVerticesDirty()
		{
			if (IsActive() && (UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null)
			{
				m_TextComponent.havePropertiesChanged = true;
				m_TextComponent.SetVerticesDirty();
			}
		}

		public override void SetLayoutDirty()
		{
		}

		public override void SetMaterialDirty()
		{
			m_materialDirty = true;
			UpdateMaterial();
			if (m_OnDirtyMaterialCallback != null)
			{
				m_OnDirtyMaterialCallback();
			}
		}

		public void SetPivotDirty()
		{
			if (IsActive())
			{
				base.rectTransform.pivot = m_TextComponent.rectTransform.pivot;
			}
		}

		public override void Cull(Rect clipRect, bool validRect)
		{
			if (!m_TextComponent.ignoreRectMaskCulling)
			{
				base.Cull(clipRect, validRect);
			}
		}

		protected override void UpdateGeometry()
		{
			Debug.Log("UpdateGeometry()", null);
		}

		public override void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.PreRender && m_materialDirty)
			{
				UpdateMaterial();
				m_materialDirty = false;
			}
		}

		public void RefreshMaterial()
		{
			UpdateMaterial();
		}

		protected override void UpdateMaterial()
		{
			if ((UnityEngine.Object)m_canvasRenderer == (UnityEngine.Object)null)
			{
				m_canvasRenderer = canvasRenderer;
			}
			m_canvasRenderer.materialCount = 1;
			m_canvasRenderer.SetMaterial(materialForRendering, 0);
			m_canvasRenderer.SetTexture(mainTexture);
		}

		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		public override void RecalculateMasking()
		{
			m_ShouldRecalculateStencil = true;
			SetMaterialDirty();
		}

		private Material GetMaterial()
		{
			return m_sharedMaterial;
		}

		private Material GetMaterial(Material mat)
		{
			if ((UnityEngine.Object)m_material == (UnityEngine.Object)null || m_material.GetInstanceID() != mat.GetInstanceID())
			{
				m_material = CreateMaterialInstance(mat);
			}
			m_sharedMaterial = m_material;
			m_padding = GetPaddingForMaterial();
			SetVerticesDirty();
			SetMaterialDirty();
			return m_sharedMaterial;
		}

		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			Material material2 = material;
			material2.name += " (Instance)";
			return material;
		}

		private Material GetSharedMaterial()
		{
			if ((UnityEngine.Object)m_canvasRenderer == (UnityEngine.Object)null)
			{
				m_canvasRenderer = GetComponent<CanvasRenderer>();
			}
			return m_canvasRenderer.GetMaterial();
		}

		private void SetSharedMaterial(Material mat)
		{
			m_sharedMaterial = mat;
			m_Material = m_sharedMaterial;
			m_padding = GetPaddingForMaterial();
			SetMaterialDirty();
		}
	}
}
