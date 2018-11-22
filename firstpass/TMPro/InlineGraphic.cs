using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public class InlineGraphic : MaskableGraphic
	{
		public Texture texture;

		private InlineGraphicManager m_manager;

		private RectTransform m_RectTransform;

		private RectTransform m_ParentRectTransform;

		public override Texture mainTexture
		{
			get
			{
				if (!((Object)texture == (Object)null))
				{
					return texture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		protected override void Awake()
		{
			m_manager = GetComponentInParent<InlineGraphicManager>();
		}

		protected override void OnEnable()
		{
			if ((Object)m_RectTransform == (Object)null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			if ((Object)m_manager != (Object)null && (Object)m_manager.spriteAsset != (Object)null)
			{
				texture = m_manager.spriteAsset.spriteSheet;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnTransformParentChanged()
		{
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if ((Object)m_RectTransform == (Object)null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			if ((Object)m_ParentRectTransform == (Object)null)
			{
				m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();
			}
			if (m_RectTransform.pivot != m_ParentRectTransform.pivot)
			{
				m_RectTransform.pivot = m_ParentRectTransform.pivot;
			}
		}

		public new void UpdateMaterial()
		{
			base.UpdateMaterial();
		}

		protected override void UpdateGeometry()
		{
		}
	}
}
