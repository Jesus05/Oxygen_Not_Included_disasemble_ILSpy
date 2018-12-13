using System;

namespace UnityEngine.UI.Extensions
{
	public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
	{
		[SerializeField]
		private Sprite m_Sprite;

		[NonSerialized]
		private Sprite m_OverrideSprite;

		internal float m_EventAlphaThreshold = 1f;

		public Sprite sprite
		{
			get
			{
				return m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass(ref m_Sprite, value))
				{
					SetAllDirty();
				}
			}
		}

		public Sprite overrideSprite
		{
			get
			{
				return (!((Object)m_OverrideSprite == (Object)null)) ? m_OverrideSprite : sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value))
				{
					SetAllDirty();
				}
			}
		}

		public float eventAlphaThreshold
		{
			get
			{
				return m_EventAlphaThreshold;
			}
			set
			{
				m_EventAlphaThreshold = value;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if ((Object)overrideSprite == (Object)null)
				{
					if ((Object)material != (Object)null && (Object)material.mainTexture != (Object)null)
					{
						return material.mainTexture;
					}
					return Graphic.s_WhiteTexture;
				}
				return overrideSprite.texture;
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if ((bool)sprite)
				{
					num = sprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if ((bool)base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		public virtual float minWidth => 0f;

		public virtual float preferredWidth
		{
			get
			{
				if ((Object)overrideSprite == (Object)null)
				{
					return 0f;
				}
				Vector2 size = overrideSprite.rect.size;
				return size.x / pixelsPerUnit;
			}
		}

		public virtual float flexibleWidth => -1f;

		public virtual float minHeight => 0f;

		public virtual float preferredHeight
		{
			get
			{
				if ((Object)overrideSprite == (Object)null)
				{
					return 0f;
				}
				Vector2 size = overrideSprite.rect.size;
				return size.y / pixelsPerUnit;
			}
		}

		public virtual float flexibleHeight => -1f;

		public virtual int layoutPriority => 0;

		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (m_EventAlphaThreshold >= 1f)
			{
				return true;
			}
			Sprite overrideSprite = this.overrideSprite;
			if (!((Object)overrideSprite == (Object)null))
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out Vector2 localPoint);
				Rect pixelAdjustedRect = GetPixelAdjustedRect();
				float x = localPoint.x;
				Vector2 pivot = base.rectTransform.pivot;
				localPoint.x = x + pivot.x * pixelAdjustedRect.width;
				float y = localPoint.y;
				Vector2 pivot2 = base.rectTransform.pivot;
				localPoint.y = y + pivot2.y * pixelAdjustedRect.height;
				localPoint = MapCoordinate(localPoint, pixelAdjustedRect);
				Rect textureRect = overrideSprite.textureRect;
				Vector2 vector = new Vector2(localPoint.x / textureRect.width, localPoint.y / textureRect.height);
				float x2 = Mathf.Lerp(textureRect.x, textureRect.xMax, vector.x) / (float)overrideSprite.texture.width;
				float y2 = Mathf.Lerp(textureRect.y, textureRect.yMax, vector.y) / (float)overrideSprite.texture.height;
				try
				{
					Color pixelBilinear = overrideSprite.texture.GetPixelBilinear(x2, y2);
					return pixelBilinear.a >= m_EventAlphaThreshold;
				}
				catch (UnityException ex)
				{
					Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
					return true;
				}
			}
			return true;
		}

		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			Rect rect2 = sprite.rect;
			return new Vector2(local.x * rect2.width / rect.width, local.y * rect2.height / rect.height);
		}

		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if (rect.size[i] < num && num != 0f)
				{
					float num2 = rect.size[i] / num;
					int index;
					ref Vector4 reference;
					((Vector4)(ref reference = ref border))[index = i] = reference[index] * num2;
					int index2;
					((Vector4)(ref reference = ref border))[index2 = i + 2] = reference[index2] * num2;
				}
			}
			return border;
		}
	}
}
