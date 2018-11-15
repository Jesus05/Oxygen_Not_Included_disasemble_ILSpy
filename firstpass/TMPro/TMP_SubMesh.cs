using System;
using UnityEngine;

namespace TMPro
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class TMP_SubMesh : MonoBehaviour
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
		private Renderer m_renderer;

		[SerializeField]
		private MeshFilter m_meshFilter;

		private Mesh m_mesh;

		[SerializeField]
		private BoxCollider m_boxCollider;

		[SerializeField]
		private TextMeshPro m_TextComponent;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

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

		public Material material
		{
			get
			{
				return GetMaterial(m_sharedMaterial);
			}
			set
			{
				if (m_sharedMaterial.GetInstanceID() != value.GetInstanceID())
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

		public Renderer renderer
		{
			get
			{
				if ((UnityEngine.Object)m_renderer == (UnityEngine.Object)null)
				{
					m_renderer = GetComponent<Renderer>();
				}
				return m_renderer;
			}
		}

		public MeshFilter meshFilter
		{
			get
			{
				if ((UnityEngine.Object)m_meshFilter == (UnityEngine.Object)null)
				{
					m_meshFilter = GetComponent<MeshFilter>();
				}
				return m_meshFilter;
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
					meshFilter.mesh = m_mesh;
				}
				return m_mesh;
			}
			set
			{
				m_mesh = value;
			}
		}

		public BoxCollider boxCollider
		{
			get
			{
				if ((UnityEngine.Object)m_boxCollider == (UnityEngine.Object)null)
				{
					m_boxCollider = GetComponent<BoxCollider>();
					if ((UnityEngine.Object)m_boxCollider == (UnityEngine.Object)null)
					{
						m_boxCollider = base.gameObject.AddComponent<BoxCollider>();
						base.gameObject.AddComponent<Rigidbody>();
					}
				}
				return m_boxCollider;
			}
		}

		private void OnEnable()
		{
			if (!m_isRegisteredForEvents)
			{
				m_isRegisteredForEvents = true;
			}
			meshFilter.sharedMesh = mesh;
			if ((UnityEngine.Object)m_sharedMaterial != (UnityEngine.Object)null)
			{
				m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, new Vector4(-32767f, -32767f, 32767f, 32767f));
			}
		}

		private void OnDisable()
		{
			m_meshFilter.sharedMesh = null;
			if ((UnityEngine.Object)m_fallbackMaterial != (UnityEngine.Object)null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				m_fallbackMaterial = null;
			}
		}

		private void OnDestroy()
		{
			if ((UnityEngine.Object)m_mesh != (UnityEngine.Object)null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
			if ((UnityEngine.Object)m_fallbackMaterial != (UnityEngine.Object)null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				m_fallbackMaterial = null;
			}
			m_isRegisteredForEvents = false;
		}

		public static TMP_SubMesh AddSubTextObject(TextMeshPro textComponent, MaterialReference materialReference)
		{
			GameObject gameObject = new GameObject("TMP SubMesh [" + materialReference.material.name + "]", typeof(TMP_SubMesh));
			TMP_SubMesh component = gameObject.GetComponent<TMP_SubMesh>();
			gameObject.transform.SetParent(textComponent.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = textComponent.gameObject.layer;
			component.m_meshFilter = gameObject.GetComponent<MeshFilter>();
			component.m_TextComponent = textComponent;
			component.m_fontAsset = materialReference.fontAsset;
			component.m_spriteAsset = materialReference.spriteAsset;
			component.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			component.SetSharedMaterial(materialReference.material);
			component.renderer.sortingLayerID = textComponent.renderer.sortingLayerID;
			component.renderer.sortingOrder = textComponent.renderer.sortingOrder;
			return component;
		}

		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject, 1f);
		}

		private Material GetMaterial(Material mat)
		{
			if ((UnityEngine.Object)m_renderer == (UnityEngine.Object)null)
			{
				m_renderer = GetComponent<Renderer>();
			}
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
			if ((UnityEngine.Object)m_renderer == (UnityEngine.Object)null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			return m_renderer.sharedMaterial;
		}

		private void SetSharedMaterial(Material mat)
		{
			m_sharedMaterial = mat;
			m_padding = GetPaddingForMaterial();
			SetMaterialDirty();
		}

		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(m_sharedMaterial, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
		}

		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		public void SetVerticesDirty()
		{
			if (base.enabled && (UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null)
			{
				m_TextComponent.havePropertiesChanged = true;
				m_TextComponent.SetVerticesDirty();
			}
		}

		public void SetMaterialDirty()
		{
			UpdateMaterial();
		}

		protected void UpdateMaterial()
		{
			if ((UnityEngine.Object)m_renderer == (UnityEngine.Object)null)
			{
				m_renderer = renderer;
			}
			m_renderer.sharedMaterial = m_sharedMaterial;
		}

		public void UpdateColliders(int vertexCount)
		{
			if (!((UnityEngine.Object)boxCollider == (UnityEngine.Object)null))
			{
				Vector2 mAX_16BIT = TMP_Math.MAX_16BIT;
				Vector2 mIN_16BIT = TMP_Math.MIN_16BIT;
				for (int i = 0; i < vertexCount; i++)
				{
					mAX_16BIT.x = Mathf.Min(mAX_16BIT.x, m_mesh.vertices[i].x);
					mAX_16BIT.y = Mathf.Min(mAX_16BIT.y, m_mesh.vertices[i].y);
					mIN_16BIT.x = Mathf.Max(mIN_16BIT.x, m_mesh.vertices[i].x);
					mIN_16BIT.y = Mathf.Max(mIN_16BIT.y, m_mesh.vertices[i].y);
				}
				Vector3 center = (mAX_16BIT + mIN_16BIT) / 2f;
				Vector3 size = mIN_16BIT - mAX_16BIT;
				size.z = 0.1f;
				boxCollider.center = center;
				boxCollider.size = size;
			}
		}
	}
}
