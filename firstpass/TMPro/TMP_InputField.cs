using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI/TextMeshPro - Input Field", 11)]
	public class TMP_InputField : Selectable, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, IScrollHandler, IEventSystemHandler
	{
		public enum ContentType
		{
			Standard,
			Autocorrected,
			IntegerNumber,
			DecimalNumber,
			Alphanumeric,
			Name,
			EmailAddress,
			Password,
			Pin,
			Custom
		}

		public enum InputType
		{
			Standard,
			AutoCorrect,
			Password
		}

		public enum CharacterValidation
		{
			None,
			Digit,
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			Regex,
			EmailAddress,
			CustomValidator
		}

		public enum LineType
		{
			SingleLine,
			MultiLineSubmit,
			MultiLineNewline
		}

		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class SelectionEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class TextSelectionEvent : UnityEvent<string, int, int>
		{
		}

		protected enum EditState
		{
			Continue,
			Finish
		}

		public System.Action onFocus;

		protected TouchScreenKeyboard m_Keyboard;

		private static readonly char[] kSeparators = new char[6]
		{
			' ',
			'.',
			',',
			'\t',
			'\r',
			'\n'
		};

		[SerializeField]
		protected RectTransform m_TextViewport;

		[SerializeField]
		protected TMP_Text m_TextComponent;

		protected RectTransform m_TextComponentRectTransform;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		protected Scrollbar m_VerticalScrollbar;

		[SerializeField]
		protected TMP_ScrollbarEventHandler m_VerticalScrollbarEventHandler;

		private float m_ScrollPosition;

		[SerializeField]
		protected float m_ScrollSensitivity = 1f;

		[SerializeField]
		private ContentType m_ContentType = ContentType.Standard;

		[SerializeField]
		private InputType m_InputType = InputType.Standard;

		[SerializeField]
		private char m_AsteriskChar = '*';

		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;

		[SerializeField]
		private LineType m_LineType = LineType.SingleLine;

		[SerializeField]
		private bool m_HideMobileInput = false;

		[SerializeField]
		private CharacterValidation m_CharacterValidation = CharacterValidation.None;

		[SerializeField]
		private string m_RegexValue = string.Empty;

		[SerializeField]
		private float m_GlobalPointSize = 14f;

		[SerializeField]
		private int m_CharacterLimit = 0;

		[SerializeField]
		private SubmitEvent m_OnEndEdit = new SubmitEvent();

		[SerializeField]
		private SubmitEvent m_OnSubmit = new SubmitEvent();

		[SerializeField]
		private SelectionEvent m_OnSelect = new SelectionEvent();

		[SerializeField]
		private SelectionEvent m_OnDeselect = new SelectionEvent();

		[SerializeField]
		private TextSelectionEvent m_OnTextSelection = new TextSelectionEvent();

		[SerializeField]
		private TextSelectionEvent m_OnEndTextSelection = new TextSelectionEvent();

		[SerializeField]
		private OnChangeEvent m_OnValueChanged = new OnChangeEvent();

		[SerializeField]
		private OnValidateInput m_OnValidateInput;

		[SerializeField]
		private Color m_CaretColor = new Color(0.196078435f, 0.196078435f, 0.196078435f, 1f);

		[SerializeField]
		private bool m_CustomCaretColor = false;

		[SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.807843149f, 1f, 0.7529412f);

		[SerializeField]
		protected string m_Text = string.Empty;

		[SerializeField]
		[Range(0f, 4f)]
		private float m_CaretBlinkRate = 0.85f;

		[SerializeField]
		[Range(1f, 5f)]
		private int m_CaretWidth = 1;

		[SerializeField]
		private bool m_ReadOnly = false;

		[SerializeField]
		private bool m_RichText = true;

		protected int m_StringPosition = 0;

		protected int m_StringSelectPosition = 0;

		protected int m_CaretPosition = 0;

		protected int m_CaretSelectPosition = 0;

		private RectTransform caretRectTrans = null;

		protected UIVertex[] m_CursorVerts = null;

		private CanvasRenderer m_CachedInputRenderer;

		private Vector2 m_DefaultTransformPosition;

		private Vector2 m_LastPosition;

		[NonSerialized]
		protected Mesh m_Mesh;

		private bool m_AllowInput = false;

		private bool m_ShouldActivateNextUpdate = false;

		private bool m_UpdateDrag = false;

		private bool m_DragPositionOutOfBounds = false;

		private const float kHScrollSpeed = 0.05f;

		private const float kVScrollSpeed = 0.1f;

		protected bool m_CaretVisible;

		private Coroutine m_BlinkCoroutine = null;

		private float m_BlinkStartTime = 0f;

		private Coroutine m_DragCoroutine = null;

		private string m_OriginalText = "";

		private bool m_WasCanceled = false;

		private bool m_HasDoneFocusTransition = false;

		private bool m_IsScrollbarUpdateRequired = false;

		private bool m_IsUpdatingScrollbarValues = false;

		private bool m_isLastKeyBackspace = false;

		private float m_ClickStartTime;

		private float m_DoubleClickDelay = 0.5f;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		[SerializeField]
		protected TMP_FontAsset m_GlobalFontAsset;

		[SerializeField]
		protected bool m_OnFocusSelectAll = true;

		protected bool m_isSelectAll;

		[SerializeField]
		protected bool m_ResetOnDeActivation = true;

		[SerializeField]
		private bool m_RestoreOriginalTextOnEscape = true;

		[SerializeField]
		protected bool m_isRichTextEditingAllowed = true;

		[SerializeField]
		protected TMP_InputValidator m_InputValidator = null;

		private bool m_isSelected;

		private bool isStringPositionDirty;

		private bool m_forceRectTransformAdjustment;

		private Event m_ProcessingEvent = new Event();

		protected Mesh mesh
		{
			get
			{
				if ((UnityEngine.Object)m_Mesh == (UnityEngine.Object)null)
				{
					m_Mesh = new Mesh();
				}
				return m_Mesh;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform != RuntimePlatform.Android && platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.tvOS)
				{
					return true;
				}
				return m_HideMobileInput;
			}
			set
			{
				SetPropertyUtility.SetStruct(ref m_HideMobileInput, value);
			}
		}

		public string text
		{
			get
			{
				return m_Text;
			}
			set
			{
				if (!(text == value))
				{
					if (value == null)
					{
						value = string.Empty;
					}
					m_Text = value;
					if (m_Keyboard != null)
					{
						m_Keyboard.text = m_Text;
					}
					if (m_StringPosition > m_Text.Length)
					{
						m_StringPosition = (m_StringSelectPosition = m_Text.Length);
					}
					AdjustTextPositionRelativeToViewport(0f);
					m_forceRectTransformAdjustment = true;
					SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		public bool isFocused => m_AllowInput;

		public float caretBlinkRate
		{
			get
			{
				return m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CaretBlinkRate, value) && m_AllowInput)
				{
					SetCaretActive();
				}
			}
		}

		public int caretWidth
		{
			get
			{
				return m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CaretWidth, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public RectTransform textViewport
		{
			get
			{
				return m_TextViewport;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_TextViewport, value);
			}
		}

		public TMP_Text textComponent
		{
			get
			{
				return m_TextComponent;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_TextComponent, value);
			}
		}

		public Graphic placeholder
		{
			get
			{
				return m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_Placeholder, value);
			}
		}

		public Scrollbar verticalScrollbar
		{
			get
			{
				return m_VerticalScrollbar;
			}
			set
			{
				if ((UnityEngine.Object)m_VerticalScrollbar != (UnityEngine.Object)null)
				{
					m_VerticalScrollbar.onValueChanged.RemoveListener(OnScrollbarValueChange);
				}
				SetPropertyUtility.SetClass(ref m_VerticalScrollbar, value);
				if ((bool)m_VerticalScrollbar)
				{
					m_VerticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChange);
				}
			}
		}

		public float scrollSensitivity
		{
			get
			{
				return m_ScrollSensitivity;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_ScrollSensitivity, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public Color caretColor
		{
			get
			{
				return (!customCaretColor) ? textComponent.color : m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref m_CaretColor, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public bool customCaretColor
		{
			get
			{
				return m_CustomCaretColor;
			}
			set
			{
				if (m_CustomCaretColor != value)
				{
					m_CustomCaretColor = value;
					MarkGeometryAsDirty();
				}
			}
		}

		public Color selectionColor
		{
			get
			{
				return m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref m_SelectionColor, value))
				{
					MarkGeometryAsDirty();
				}
			}
		}

		public SubmitEvent onEndEdit
		{
			get
			{
				return m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnEndEdit, value);
			}
		}

		public SubmitEvent onSubmit
		{
			get
			{
				return m_OnSubmit;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnSubmit, value);
			}
		}

		public SelectionEvent onSelect
		{
			get
			{
				return m_OnSelect;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnSelect, value);
			}
		}

		public SelectionEvent onDeselect
		{
			get
			{
				return m_OnDeselect;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnDeselect, value);
			}
		}

		public TextSelectionEvent onTextSelection
		{
			get
			{
				return m_OnTextSelection;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnTextSelection, value);
			}
		}

		public TextSelectionEvent onEndTextSelection
		{
			get
			{
				return m_OnEndTextSelection;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnEndTextSelection, value);
			}
		}

		public OnChangeEvent onValueChanged
		{
			get
			{
				return m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnValueChanged, value);
			}
		}

		public OnValidateInput onValidateInput
		{
			get
			{
				return m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass(ref m_OnValidateInput, value);
			}
		}

		public int characterLimit
		{
			get
			{
				return m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CharacterLimit, Math.Max(0, value)))
				{
					UpdateLabel();
				}
			}
		}

		public float pointSize
		{
			get
			{
				return m_GlobalPointSize;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_GlobalPointSize, Math.Max(0f, value)))
				{
					SetGlobalPointSize(m_GlobalPointSize);
					UpdateLabel();
				}
			}
		}

		public TMP_FontAsset fontAsset
		{
			get
			{
				return m_GlobalFontAsset;
			}
			set
			{
				if (SetPropertyUtility.SetClass(ref m_GlobalFontAsset, value))
				{
					SetGlobalFontAsset(m_GlobalFontAsset);
					UpdateLabel();
				}
			}
		}

		public bool onFocusSelectAll
		{
			get
			{
				return m_OnFocusSelectAll;
			}
			set
			{
				m_OnFocusSelectAll = value;
			}
		}

		public bool resetOnDeActivation
		{
			get
			{
				return m_ResetOnDeActivation;
			}
			set
			{
				m_ResetOnDeActivation = value;
			}
		}

		public bool restoreOriginalTextOnEscape
		{
			get
			{
				return m_RestoreOriginalTextOnEscape;
			}
			set
			{
				m_RestoreOriginalTextOnEscape = value;
			}
		}

		public bool isRichTextEditingAllowed
		{
			get
			{
				return m_isRichTextEditingAllowed;
			}
			set
			{
				m_isRichTextEditingAllowed = value;
			}
		}

		public ContentType contentType
		{
			get
			{
				return m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_ContentType, value))
				{
					EnforceContentType();
				}
			}
		}

		public LineType lineType
		{
			get
			{
				return m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_LineType, value))
				{
					SetTextComponentWrapMode();
				}
				SetToCustomIfContentTypeIsNot(ContentType.Standard, ContentType.Autocorrected);
			}
		}

		public InputType inputType
		{
			get
			{
				return m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_InputType, value))
				{
					SetToCustom();
				}
			}
		}

		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return m_KeyboardType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_KeyboardType, value))
				{
					SetToCustom();
				}
			}
		}

		public CharacterValidation characterValidation
		{
			get
			{
				return m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_CharacterValidation, value))
				{
					SetToCustom();
				}
			}
		}

		public TMP_InputValidator inputValidator
		{
			get
			{
				return m_InputValidator;
			}
			set
			{
				if (SetPropertyUtility.SetClass(ref m_InputValidator, value))
				{
					SetToCustom(CharacterValidation.CustomValidator);
				}
			}
		}

		public bool readOnly
		{
			get
			{
				return m_ReadOnly;
			}
			set
			{
				m_ReadOnly = value;
			}
		}

		public bool richText
		{
			get
			{
				return m_RichText;
			}
			set
			{
				m_RichText = value;
				SetTextComponentRichTextMode();
			}
		}

		public bool multiLine => m_LineType == LineType.MultiLineNewline || lineType == LineType.MultiLineSubmit;

		public char asteriskChar
		{
			get
			{
				return m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_AsteriskChar, value))
				{
					UpdateLabel();
				}
			}
		}

		public bool wasCanceled => m_WasCanceled;

		protected int caretPositionInternal
		{
			get
			{
				return m_CaretPosition + Input.compositionString.Length;
			}
			set
			{
				m_CaretPosition = value;
				ClampCaretPos(ref m_CaretPosition);
			}
		}

		protected int stringPositionInternal
		{
			get
			{
				return m_StringPosition + Input.compositionString.Length;
			}
			set
			{
				m_StringPosition = value;
				ClampStringPos(ref m_StringPosition);
			}
		}

		protected int caretSelectPositionInternal
		{
			get
			{
				return m_CaretSelectPosition + Input.compositionString.Length;
			}
			set
			{
				m_CaretSelectPosition = value;
				ClampCaretPos(ref m_CaretSelectPosition);
			}
		}

		protected int stringSelectPositionInternal
		{
			get
			{
				return m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				m_StringSelectPosition = value;
				ClampStringPos(ref m_StringSelectPosition);
			}
		}

		private bool hasSelection => stringPositionInternal != stringSelectPositionInternal;

		public int caretPosition
		{
			get
			{
				return caretSelectPositionInternal;
			}
			set
			{
				selectionAnchorPosition = value;
				selectionFocusPosition = value;
				isStringPositionDirty = true;
			}
		}

		public int selectionAnchorPosition
		{
			get
			{
				return caretPositionInternal;
			}
			set
			{
				if (Input.compositionString.Length == 0)
				{
					caretPositionInternal = value;
					isStringPositionDirty = true;
				}
			}
		}

		public int selectionFocusPosition
		{
			get
			{
				return caretSelectPositionInternal;
			}
			set
			{
				if (Input.compositionString.Length == 0)
				{
					caretSelectPositionInternal = value;
					isStringPositionDirty = true;
				}
			}
		}

		public int stringPosition
		{
			get
			{
				return stringSelectPositionInternal;
			}
			set
			{
				selectionStringAnchorPosition = value;
				selectionStringFocusPosition = value;
			}
		}

		public int selectionStringAnchorPosition
		{
			get
			{
				return stringPositionInternal;
			}
			set
			{
				if (Input.compositionString.Length == 0)
				{
					stringPositionInternal = value;
				}
			}
		}

		public int selectionStringFocusPosition
		{
			get
			{
				return stringSelectPositionInternal;
			}
			set
			{
				if (Input.compositionString.Length == 0)
				{
					stringSelectPositionInternal = value;
				}
			}
		}

		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		protected TMP_InputField()
		{
		}

		protected void ClampStringPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > text.Length)
			{
				pos = text.Length;
			}
		}

		protected void ClampCaretPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > m_TextComponent.textInfo.characterCount - 1)
			{
				pos = m_TextComponent.textInfo.characterCount - 1;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (m_Text == null)
			{
				m_Text = string.Empty;
			}
			if (Application.isPlaying && (UnityEngine.Object)m_CachedInputRenderer == (UnityEngine.Object)null && (UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null)
			{
				GameObject gameObject = new GameObject(base.transform.name + " Input Caret", typeof(RectTransform));
				TMP_SelectionCaret tMP_SelectionCaret = gameObject.AddComponent<TMP_SelectionCaret>();
				tMP_SelectionCaret.raycastTarget = false;
				tMP_SelectionCaret.color = Color.clear;
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(m_TextComponent.transform.parent);
				gameObject.transform.SetAsFirstSibling();
				gameObject.layer = base.gameObject.layer;
				caretRectTrans = gameObject.GetComponent<RectTransform>();
				m_CachedInputRenderer = gameObject.GetComponent<CanvasRenderer>();
				m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				AssignPositioningIfNeeded();
			}
			if ((UnityEngine.Object)m_CachedInputRenderer != (UnityEngine.Object)null)
			{
				m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
			}
			if ((UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null)
			{
				m_TextComponent.RegisterDirtyVerticesCallback(MarkGeometryAsDirty);
				m_TextComponent.RegisterDirtyVerticesCallback(UpdateLabel);
				m_TextComponent.ignoreRectMaskCulling = true;
				m_DefaultTransformPosition = m_TextComponent.rectTransform.localPosition;
				if ((UnityEngine.Object)m_VerticalScrollbar != (UnityEngine.Object)null)
				{
					m_VerticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChange);
				}
				UpdateLabel();
			}
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
		}

		protected override void OnDisable()
		{
			m_BlinkCoroutine = null;
			DeactivateInputField();
			if ((UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null)
			{
				m_TextComponent.UnregisterDirtyVerticesCallback(MarkGeometryAsDirty);
				m_TextComponent.UnregisterDirtyVerticesCallback(UpdateLabel);
				if ((UnityEngine.Object)m_VerticalScrollbar != (UnityEngine.Object)null)
				{
					m_VerticalScrollbar.onValueChanged.RemoveListener(OnScrollbarValueChange);
				}
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if ((UnityEngine.Object)m_CachedInputRenderer != (UnityEngine.Object)null)
			{
				m_CachedInputRenderer.Clear();
			}
			if ((UnityEngine.Object)m_Mesh != (UnityEngine.Object)null)
			{
				UnityEngine.Object.DestroyImmediate(m_Mesh);
			}
			m_Mesh = null;
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
			base.OnDisable();
		}

		private void ON_TEXT_CHANGED(UnityEngine.Object obj)
		{
			if (obj == (UnityEngine.Object)m_TextComponent && Application.isPlaying)
			{
				caretPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal);
				caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
			}
		}

		private IEnumerator CaretBlink()
		{
			m_CaretVisible = true;
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private void SetCaretVisible()
		{
			if (m_AllowInput)
			{
				m_CaretVisible = true;
				m_BlinkStartTime = Time.unscaledTime;
				SetCaretActive();
			}
		}

		private void SetCaretActive()
		{
			if (m_AllowInput)
			{
				if (m_CaretBlinkRate > 0f)
				{
					if (m_BlinkCoroutine == null)
					{
						m_BlinkCoroutine = StartCoroutine(CaretBlink());
					}
				}
				else
				{
					m_CaretVisible = true;
				}
			}
		}

		protected void OnFocus()
		{
			if (m_OnFocusSelectAll)
			{
				SelectAll();
			}
			if (onFocus != null)
			{
				onFocus();
			}
		}

		protected void SelectAll()
		{
			m_isSelectAll = true;
			stringPositionInternal = text.Length;
			stringSelectPositionInternal = 0;
		}

		public void MoveTextEnd(bool shift)
		{
			if (m_isRichTextEditingAllowed)
			{
				int length = text.Length;
				if (shift)
				{
					stringSelectPositionInternal = length;
				}
				else
				{
					stringPositionInternal = length;
					stringSelectPositionInternal = stringPositionInternal;
				}
			}
			else
			{
				int num = m_TextComponent.textInfo.characterCount - 1;
				if (!shift)
				{
					int num4 = caretPositionInternal = (caretSelectPositionInternal = num);
					num4 = (stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(num)));
				}
				else
				{
					caretSelectPositionInternal = num;
					stringSelectPositionInternal = GetStringIndexFromCaretPosition(num);
				}
			}
			UpdateLabel();
		}

		public void MoveTextStart(bool shift)
		{
			if (m_isRichTextEditingAllowed)
			{
				int num = 0;
				if (shift)
				{
					stringSelectPositionInternal = num;
				}
				else
				{
					stringPositionInternal = num;
					stringSelectPositionInternal = stringPositionInternal;
				}
			}
			else
			{
				int num2 = 0;
				if (!shift)
				{
					int num5 = caretPositionInternal = (caretSelectPositionInternal = num2);
					num5 = (stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(num2)));
				}
				else
				{
					caretSelectPositionInternal = num2;
					stringSelectPositionInternal = GetStringIndexFromCaretPosition(num2);
				}
			}
			UpdateLabel();
		}

		public void MoveToEndOfLine(bool shift, bool ctrl)
		{
			int lineNumber = m_TextComponent.textInfo.characterInfo[caretPositionInternal].lineNumber;
			int caretPosition = (!ctrl) ? m_TextComponent.textInfo.lineInfo[lineNumber].lastCharacterIndex : (m_TextComponent.textInfo.characterCount - 1);
			caretPosition = GetStringIndexFromCaretPosition(caretPosition);
			if (shift)
			{
				stringSelectPositionInternal = caretPosition;
			}
			else
			{
				stringPositionInternal = caretPosition;
				stringSelectPositionInternal = stringPositionInternal;
			}
			UpdateLabel();
		}

		public void MoveToStartOfLine(bool shift, bool ctrl)
		{
			int lineNumber = m_TextComponent.textInfo.characterInfo[caretPositionInternal].lineNumber;
			int caretPosition = (!ctrl) ? m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex : 0;
			caretPosition = GetStringIndexFromCaretPosition(caretPosition);
			if (shift)
			{
				stringSelectPositionInternal = caretPosition;
			}
			else
			{
				stringPositionInternal = caretPosition;
				stringSelectPositionInternal = stringPositionInternal;
			}
			UpdateLabel();
		}

		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported;
		}

		protected virtual void LateUpdate()
		{
			if (m_ShouldActivateNextUpdate)
			{
				if (!isFocused)
				{
					ActivateInputFieldInternal();
					m_ShouldActivateNextUpdate = false;
					return;
				}
				m_ShouldActivateNextUpdate = false;
			}
			if (m_IsScrollbarUpdateRequired)
			{
				UpdateScrollbar();
				m_IsScrollbarUpdateRequired = false;
			}
			if (!InPlaceEditing() && isFocused)
			{
				AssignPositioningIfNeeded();
				if (m_Keyboard == null || !m_Keyboard.active)
				{
					if (m_Keyboard != null)
					{
						if (!m_ReadOnly)
						{
							this.text = m_Keyboard.text;
						}
						if (m_Keyboard.wasCanceled)
						{
							m_WasCanceled = true;
						}
						if (m_Keyboard.done)
						{
							OnSubmit(null);
						}
					}
					OnDeselect(null);
				}
				else
				{
					string text = m_Keyboard.text;
					if (m_Text != text)
					{
						if (m_ReadOnly)
						{
							m_Keyboard.text = m_Text;
						}
						else
						{
							m_Text = "";
							for (int i = 0; i < text.Length; i++)
							{
								char c = text[i];
								if (c == '\r' || c == '\u0003')
								{
									c = '\n';
								}
								if (onValidateInput != null)
								{
									c = onValidateInput(m_Text, m_Text.Length, c);
								}
								else if (characterValidation != 0)
								{
									c = Validate(m_Text, m_Text.Length, c);
								}
								if (lineType == LineType.MultiLineSubmit && c == '\n')
								{
									m_Keyboard.text = m_Text;
									OnSubmit(null);
									OnDeselect(null);
									return;
								}
								if (c != 0)
								{
									m_Text += c;
								}
							}
							if (characterLimit > 0 && m_Text.Length > characterLimit)
							{
								m_Text = m_Text.Substring(0, characterLimit);
							}
							int num2 = stringPositionInternal = (stringSelectPositionInternal = m_Text.Length);
							if (m_Text != text)
							{
								m_Keyboard.text = m_Text;
							}
							SendOnValueChangedAndUpdateLabel();
						}
					}
					if (m_Keyboard.done)
					{
						if (m_Keyboard.wasCanceled)
						{
							m_WasCanceled = true;
						}
						OnDeselect(null);
					}
				}
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left && (UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null && m_Keyboard == null;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				m_UpdateDrag = true;
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				CaretPosition cursor;
				int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, eventData.position, eventData.pressEventCamera, out cursor);
				switch (cursor)
				{
				case CaretPosition.Left:
					stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition);
					break;
				case CaretPosition.Right:
					stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1;
					break;
				}
				caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
				MarkGeometryAsDirty();
				m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(textViewport, eventData.position, eventData.pressEventCamera);
				if (m_DragPositionOutOfBounds && m_DragCoroutine == null)
				{
					m_DragCoroutine = StartCoroutine(MouseDragOutsideRect(eventData));
				}
				eventData.Use();
			}
		}

		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			if (m_UpdateDrag && m_DragPositionOutOfBounds)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(textViewport, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos);
				Rect rect = textViewport.rect;
				if (multiLine)
				{
					if (localMousePos.y > rect.yMax)
					{
						MoveUp(true, true);
					}
					else if (localMousePos.y < rect.yMin)
					{
						MoveDown(true, true);
					}
				}
				else if (localMousePos.x < rect.xMin)
				{
					MoveLeft(true, false);
				}
				else if (localMousePos.x > rect.xMax)
				{
					MoveRight(true, false);
				}
				UpdateLabel();
				float delay = (!multiLine) ? 0.05f : 0.1f;
				yield return (object)new WaitForSeconds(delay);
				/*Error: Unable to find new state assignment for yield return*/;
			}
			m_DragCoroutine = null;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				m_UpdateDrag = false;
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (MayDrag(eventData))
			{
				UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
				bool allowInput = m_AllowInput;
				base.OnPointerDown(eventData);
				if (!InPlaceEditing() && (m_Keyboard == null || !m_Keyboard.active))
				{
					OnSelect(eventData);
				}
				else
				{
					bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
					bool flag2 = false;
					float unscaledTime = Time.unscaledTime;
					if (m_ClickStartTime + m_DoubleClickDelay > unscaledTime)
					{
						flag2 = true;
					}
					m_ClickStartTime = unscaledTime;
					if (allowInput || !m_OnFocusSelectAll)
					{
						CaretPosition cursor;
						int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(m_TextComponent, eventData.position, eventData.pressEventCamera, out cursor);
						if (!flag)
						{
							switch (cursor)
							{
							case CaretPosition.Left:
							{
								int num3 = stringPositionInternal = (stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition));
								break;
							}
							case CaretPosition.Right:
							{
								int num3 = stringPositionInternal = (stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1);
								break;
							}
							}
						}
						else
						{
							switch (cursor)
							{
							case CaretPosition.Left:
								stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition);
								break;
							case CaretPosition.Right:
								stringSelectPositionInternal = GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1;
								break;
							}
						}
						if (!flag2)
						{
							int num3 = caretPositionInternal = (caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal));
						}
						else
						{
							int num6 = TMP_TextUtilities.FindIntersectingWord(m_TextComponent, eventData.position, eventData.pressEventCamera);
							if (num6 != -1)
							{
								caretPositionInternal = m_TextComponent.textInfo.wordInfo[num6].firstCharacterIndex;
								caretSelectPositionInternal = m_TextComponent.textInfo.wordInfo[num6].lastCharacterIndex + 1;
								stringPositionInternal = GetStringIndexFromCaretPosition(caretPositionInternal);
								stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
							}
							else
							{
								caretPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal);
								stringSelectPositionInternal++;
								caretSelectPositionInternal = caretPositionInternal + 1;
								caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
							}
						}
					}
					UpdateLabel();
					eventData.Use();
				}
			}
		}

		protected EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			RuntimePlatform platform = Application.platform;
			bool flag = (platform != 0 && platform != RuntimePlatform.OSXPlayer) ? ((modifiers & EventModifiers.Control) != EventModifiers.None) : ((modifiers & EventModifiers.Command) != EventModifiers.None);
			bool flag2 = (modifiers & EventModifiers.Shift) != EventModifiers.None;
			bool flag3 = (modifiers & EventModifiers.Alt) != EventModifiers.None;
			bool flag4 = flag && !flag3 && !flag2;
			switch (evt.keyCode)
			{
			case KeyCode.Backspace:
				Backspace();
				return EditState.Continue;
			case KeyCode.Delete:
				ForwardSpace();
				return EditState.Continue;
			case KeyCode.Home:
				MoveToStartOfLine(flag2, flag);
				return EditState.Continue;
			case KeyCode.End:
				MoveToEndOfLine(flag2, flag);
				return EditState.Continue;
			case KeyCode.A:
				if (flag4)
				{
					SelectAll();
					return EditState.Continue;
				}
				break;
			case KeyCode.C:
				if (flag4)
				{
					if (inputType != InputType.Password)
					{
						clipboard = GetSelectedString();
					}
					else
					{
						clipboard = "";
					}
					return EditState.Continue;
				}
				break;
			case KeyCode.V:
				if (flag4)
				{
					Append(clipboard);
					return EditState.Continue;
				}
				break;
			case KeyCode.X:
				if (flag4)
				{
					if (inputType != InputType.Password)
					{
						clipboard = GetSelectedString();
					}
					else
					{
						clipboard = "";
					}
					Delete();
					SendOnValueChangedAndUpdateLabel();
					return EditState.Continue;
				}
				break;
			case KeyCode.LeftArrow:
				MoveLeft(flag2, flag);
				return EditState.Continue;
			case KeyCode.RightArrow:
				MoveRight(flag2, flag);
				return EditState.Continue;
			case KeyCode.UpArrow:
				MoveUp(flag2);
				return EditState.Continue;
			case KeyCode.DownArrow:
				MoveDown(flag2);
				return EditState.Continue;
			case KeyCode.PageUp:
				MovePageUp(flag2);
				return EditState.Continue;
			case KeyCode.PageDown:
				MovePageDown(flag2);
				return EditState.Continue;
			case KeyCode.Return:
			case KeyCode.KeypadEnter:
				if (lineType != LineType.MultiLineNewline)
				{
					return EditState.Finish;
				}
				break;
			case KeyCode.Escape:
				m_WasCanceled = true;
				return EditState.Finish;
			}
			char c = evt.character;
			if (!multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (IsValidChar(c))
			{
				Append(c);
			}
			if (c == '\0' && Input.compositionString.Length > 0)
			{
				UpdateLabel();
			}
			return EditState.Continue;
		}

		private bool IsValidChar(char c)
		{
			switch (c)
			{
			case '\u007f':
				return false;
			case '\t':
			case '\n':
				return true;
			default:
				return m_TextComponent.font.HasCharacter(c, true);
			}
		}

		public void ProcessEvent(Event e)
		{
			KeyPressed(e);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (isFocused)
			{
				bool flag = false;
				while (Event.PopEvent(m_ProcessingEvent))
				{
					if (m_ProcessingEvent.rawType == EventType.KeyDown)
					{
						flag = true;
						EditState editState = KeyPressed(m_ProcessingEvent);
						if (editState == EditState.Finish)
						{
							DeactivateInputField();
							break;
						}
					}
					EventType type = m_ProcessingEvent.type;
					if (type == EventType.ValidateCommand || type == EventType.ExecuteCommand)
					{
						string commandName = m_ProcessingEvent.commandName;
						if (commandName != null && commandName == "SelectAll")
						{
							SelectAll();
							flag = true;
						}
					}
				}
				if (flag)
				{
					UpdateLabel();
				}
				eventData.Use();
			}
		}

		public virtual void OnScroll(PointerEventData eventData)
		{
			if (!(m_TextComponent.preferredHeight < m_TextViewport.rect.height))
			{
				Vector2 scrollDelta = eventData.scrollDelta;
				float num = 0f - scrollDelta.y;
				m_ScrollPosition += 1f / (float)m_TextComponent.textInfo.lineCount * num * m_ScrollSensitivity;
				m_ScrollPosition = Mathf.Clamp01(m_ScrollPosition);
				AdjustTextPositionRelativeToViewport(m_ScrollPosition);
				m_AllowInput = false;
				if ((bool)m_VerticalScrollbar)
				{
					m_IsUpdatingScrollbarValues = true;
					m_VerticalScrollbar.value = m_ScrollPosition;
				}
			}
		}

		private string GetSelectedString()
		{
			if (hasSelection)
			{
				int num = stringPositionInternal;
				int num2 = stringSelectPositionInternal;
				if (num > num2)
				{
					int num3 = num;
					num = num2;
					num2 = num3;
				}
				return text.Substring(num, num2 - num);
			}
			return "";
		}

		private int FindtNextWordBegin()
		{
			if (stringSelectPositionInternal + 1 < text.Length)
			{
				int num = text.IndexOfAny(kSeparators, stringSelectPositionInternal + 1);
				return (num != -1) ? (num + 1) : text.Length;
			}
			return text.Length;
		}

		private void MoveRight(bool shift, bool ctrl)
		{
			if (!hasSelection || shift)
			{
				int num = ctrl ? FindtNextWordBegin() : ((!m_isRichTextEditingAllowed) ? GetStringIndexFromCaretPosition(caretSelectPositionInternal + 1) : (stringSelectPositionInternal + 1));
				if (shift)
				{
					stringSelectPositionInternal = num;
					caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
				}
				else
				{
					int num4 = stringSelectPositionInternal = (stringPositionInternal = num);
					num4 = (caretSelectPositionInternal = (caretPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal)));
				}
			}
			else
			{
				int num4 = stringPositionInternal = (stringSelectPositionInternal = Mathf.Max(stringPositionInternal, stringSelectPositionInternal));
				num4 = (caretPositionInternal = (caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal)));
			}
		}

		private int FindtPrevWordBegin()
		{
			if (stringSelectPositionInternal - 2 >= 0)
			{
				int num = text.LastIndexOfAny(kSeparators, stringSelectPositionInternal - 2);
				return (num != -1) ? (num + 1) : 0;
			}
			return 0;
		}

		private void MoveLeft(bool shift, bool ctrl)
		{
			if (!hasSelection || shift)
			{
				int num = ctrl ? FindtPrevWordBegin() : ((!m_isRichTextEditingAllowed) ? GetStringIndexFromCaretPosition(caretSelectPositionInternal - 1) : (stringSelectPositionInternal - 1));
				if (shift)
				{
					stringSelectPositionInternal = num;
					caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal);
				}
				else
				{
					int num4 = stringSelectPositionInternal = (stringPositionInternal = num);
					num4 = (caretSelectPositionInternal = (caretPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal)));
				}
			}
			else
			{
				int num4 = stringPositionInternal = (stringSelectPositionInternal = Mathf.Min(stringPositionInternal, stringSelectPositionInternal));
				num4 = (caretPositionInternal = (caretSelectPositionInternal = GetCaretPositionFromStringIndex(stringSelectPositionInternal)));
			}
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tMP_CharacterInfo.lineNumber;
			if (lineNumber - 1 >= 0)
			{
				int num = m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex - 1;
				int num2 = -1;
				float num3 = 32767f;
				float num4 = 0f;
				for (int i = m_TextComponent.textInfo.lineInfo[lineNumber - 1].firstCharacterIndex; i < num; i++)
				{
					TMP_CharacterInfo tMP_CharacterInfo2 = m_TextComponent.textInfo.characterInfo[i];
					float num5 = tMP_CharacterInfo.origin - tMP_CharacterInfo2.origin;
					float num6 = num5 / (tMP_CharacterInfo2.xAdvance - tMP_CharacterInfo2.origin);
					if (num6 >= 0f && num6 <= 1f)
					{
						if (!(num6 < 0.5f))
						{
							return i + 1;
						}
						return i;
					}
					num5 = Mathf.Abs(num5);
					if (num5 < num3)
					{
						num2 = i;
						num3 = num5;
						num4 = num6;
					}
				}
				if (num2 != -1)
				{
					if (!(num4 < 0.5f))
					{
						return num2 + 1;
					}
					return num2;
				}
				return num;
			}
			return (!goToFirstChar) ? originalPos : 0;
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos < m_TextComponent.textInfo.characterCount)
			{
				TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
				int lineNumber = tMP_CharacterInfo.lineNumber;
				if (lineNumber + 1 < m_TextComponent.textInfo.lineCount)
				{
					int lastCharacterIndex = m_TextComponent.textInfo.lineInfo[lineNumber + 1].lastCharacterIndex;
					int num = -1;
					float num2 = 32767f;
					float num3 = 0f;
					for (int i = m_TextComponent.textInfo.lineInfo[lineNumber + 1].firstCharacterIndex; i < lastCharacterIndex; i++)
					{
						TMP_CharacterInfo tMP_CharacterInfo2 = m_TextComponent.textInfo.characterInfo[i];
						float num4 = tMP_CharacterInfo.origin - tMP_CharacterInfo2.origin;
						float num5 = num4 / (tMP_CharacterInfo2.xAdvance - tMP_CharacterInfo2.origin);
						if (num5 >= 0f && num5 <= 1f)
						{
							if (!(num5 < 0.5f))
							{
								return i + 1;
							}
							return i;
						}
						num4 = Mathf.Abs(num4);
						if (num4 < num2)
						{
							num = i;
							num2 = num4;
							num3 = num5;
						}
					}
					if (num != -1)
					{
						if (!(num3 < 0.5f))
						{
							return num + 1;
						}
						return num;
					}
					return lastCharacterIndex;
				}
				return (!goToLastChar) ? originalPos : (m_TextComponent.textInfo.characterCount - 1);
			}
			return m_TextComponent.textInfo.characterCount - 1;
		}

		private int PageUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tMP_CharacterInfo.lineNumber;
			if (lineNumber - 1 >= 0)
			{
				float height = m_TextViewport.rect.height;
				int num = lineNumber - 1;
				while (num > 0 && !(m_TextComponent.textInfo.lineInfo[num].baseline > m_TextComponent.textInfo.lineInfo[lineNumber].baseline + height))
				{
					num--;
				}
				int lastCharacterIndex = m_TextComponent.textInfo.lineInfo[num].lastCharacterIndex;
				int num2 = -1;
				float num3 = 32767f;
				float num4 = 0f;
				for (int i = m_TextComponent.textInfo.lineInfo[num].firstCharacterIndex; i < lastCharacterIndex; i++)
				{
					TMP_CharacterInfo tMP_CharacterInfo2 = m_TextComponent.textInfo.characterInfo[i];
					float num5 = tMP_CharacterInfo.origin - tMP_CharacterInfo2.origin;
					float num6 = num5 / (tMP_CharacterInfo2.xAdvance - tMP_CharacterInfo2.origin);
					if (num6 >= 0f && num6 <= 1f)
					{
						if (!(num6 < 0.5f))
						{
							return i + 1;
						}
						return i;
					}
					num5 = Mathf.Abs(num5);
					if (num5 < num3)
					{
						num2 = i;
						num3 = num5;
						num4 = num6;
					}
				}
				if (num2 != -1)
				{
					if (!(num4 < 0.5f))
					{
						return num2 + 1;
					}
					return num2;
				}
				return lastCharacterIndex;
			}
			return (!goToFirstChar) ? originalPos : 0;
		}

		private int PageDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos < m_TextComponent.textInfo.characterCount)
			{
				TMP_CharacterInfo tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[originalPos];
				int lineNumber = tMP_CharacterInfo.lineNumber;
				if (lineNumber + 1 < m_TextComponent.textInfo.lineCount)
				{
					float height = m_TextViewport.rect.height;
					int i;
					for (i = lineNumber + 1; i < m_TextComponent.textInfo.lineCount - 1 && !(m_TextComponent.textInfo.lineInfo[i].baseline < m_TextComponent.textInfo.lineInfo[lineNumber].baseline - height); i++)
					{
					}
					int lastCharacterIndex = m_TextComponent.textInfo.lineInfo[i].lastCharacterIndex;
					int num = -1;
					float num2 = 32767f;
					float num3 = 0f;
					for (int j = m_TextComponent.textInfo.lineInfo[i].firstCharacterIndex; j < lastCharacterIndex; j++)
					{
						TMP_CharacterInfo tMP_CharacterInfo2 = m_TextComponent.textInfo.characterInfo[j];
						float num4 = tMP_CharacterInfo.origin - tMP_CharacterInfo2.origin;
						float num5 = num4 / (tMP_CharacterInfo2.xAdvance - tMP_CharacterInfo2.origin);
						if (num5 >= 0f && num5 <= 1f)
						{
							if (!(num5 < 0.5f))
							{
								return j + 1;
							}
							return j;
						}
						num4 = Mathf.Abs(num4);
						if (num4 < num2)
						{
							num = j;
							num2 = num4;
							num3 = num5;
						}
					}
					if (num != -1)
					{
						if (!(num3 < 0.5f))
						{
							return num + 1;
						}
						return num;
					}
					return lastCharacterIndex;
				}
				return (!goToLastChar) ? originalPos : (m_TextComponent.textInfo.characterCount - 1);
			}
			return m_TextComponent.textInfo.characterCount - 1;
		}

		private void MoveDown(bool shift)
		{
			MoveDown(shift, true);
		}

		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (hasSelection && !shift)
			{
				int num3 = caretPositionInternal = (caretSelectPositionInternal = Mathf.Max(caretPositionInternal, caretSelectPositionInternal));
			}
			int num4 = (!multiLine) ? (m_TextComponent.textInfo.characterCount - 1) : LineDownCharacterPosition(caretSelectPositionInternal, goToLastChar);
			if (shift)
			{
				caretSelectPositionInternal = num4;
				stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
			}
			else
			{
				int num3 = caretSelectPositionInternal = (caretPositionInternal = num4);
				num3 = (stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal)));
			}
		}

		private void MoveUp(bool shift)
		{
			MoveUp(shift, true);
		}

		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (hasSelection && !shift)
			{
				int num3 = caretPositionInternal = (caretSelectPositionInternal = Mathf.Min(caretPositionInternal, caretSelectPositionInternal));
			}
			int num4 = multiLine ? LineUpCharacterPosition(caretSelectPositionInternal, goToFirstChar) : 0;
			if (shift)
			{
				caretSelectPositionInternal = num4;
				stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
			}
			else
			{
				int num3 = caretSelectPositionInternal = (caretPositionInternal = num4);
				num3 = (stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal)));
			}
		}

		private void MovePageUp(bool shift)
		{
			MovePageUp(shift, true);
		}

		private void MovePageUp(bool shift, bool goToFirstChar)
		{
			if (hasSelection && !shift)
			{
				int num3 = caretPositionInternal = (caretSelectPositionInternal = Mathf.Min(caretPositionInternal, caretSelectPositionInternal));
			}
			int num4 = multiLine ? PageUpCharacterPosition(caretSelectPositionInternal, goToFirstChar) : 0;
			if (!shift)
			{
				int num3 = caretSelectPositionInternal = (caretPositionInternal = num4);
				num3 = (stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal)));
			}
			else
			{
				caretSelectPositionInternal = num4;
				stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
			}
			if (m_LineType != 0)
			{
				float height = m_TextViewport.rect.height;
				Vector3 position = m_TextComponent.rectTransform.position;
				float y = position.y;
				Vector3 max = m_TextComponent.textBounds.max;
				float num8 = y + max.y;
				Vector3 position2 = m_TextViewport.position;
				float num9 = position2.y + m_TextViewport.rect.yMax;
				height = ((!(num9 > num8 + height)) ? (num9 - num8) : height);
				m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, height);
				AssignPositioningIfNeeded();
				m_IsScrollbarUpdateRequired = true;
			}
		}

		private void MovePageDown(bool shift)
		{
			MovePageDown(shift, true);
		}

		private void MovePageDown(bool shift, bool goToLastChar)
		{
			if (hasSelection && !shift)
			{
				int num3 = caretPositionInternal = (caretSelectPositionInternal = Mathf.Max(caretPositionInternal, caretSelectPositionInternal));
			}
			int num4 = (!multiLine) ? (m_TextComponent.textInfo.characterCount - 1) : PageDownCharacterPosition(caretSelectPositionInternal, goToLastChar);
			if (!shift)
			{
				int num3 = caretSelectPositionInternal = (caretPositionInternal = num4);
				num3 = (stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal)));
			}
			else
			{
				caretSelectPositionInternal = num4;
				stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
			}
			if (m_LineType != 0)
			{
				float height = m_TextViewport.rect.height;
				Vector3 position = m_TextComponent.rectTransform.position;
				float y = position.y;
				Vector3 min = m_TextComponent.textBounds.min;
				float num8 = y + min.y;
				Vector3 position2 = m_TextViewport.position;
				float num9 = position2.y + m_TextViewport.rect.yMin;
				height = ((!(num9 > num8 + height)) ? (num9 - num8) : height);
				m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, height);
				AssignPositioningIfNeeded();
				m_IsScrollbarUpdateRequired = true;
			}
		}

		private void Delete()
		{
			if (!m_ReadOnly && stringPositionInternal != this.stringSelectPositionInternal)
			{
				if (m_isRichTextEditingAllowed || m_isSelectAll)
				{
					string text = m_Text;
					int stringSelectPositionInternal = this.stringSelectPositionInternal;
					try
					{
						if (stringPositionInternal < this.stringSelectPositionInternal)
						{
							m_Text = this.text.Substring(0, stringPositionInternal) + this.text.Substring(this.stringSelectPositionInternal, this.text.Length - this.stringSelectPositionInternal);
							this.stringSelectPositionInternal = stringPositionInternal;
						}
						else
						{
							m_Text = this.text.Substring(0, this.stringSelectPositionInternal) + this.text.Substring(stringPositionInternal, this.text.Length - stringPositionInternal);
							stringPositionInternal = this.stringSelectPositionInternal;
						}
					}
					catch (Exception obj)
					{
						Debug.LogWarning(obj, null);
						Debug.LogWarning("m_text=" + text, null);
						Debug.LogWarning("stringSelectPositionInternal=" + stringSelectPositionInternal.ToString(), null);
						m_Text = "";
						stringPositionInternal = this.stringSelectPositionInternal;
					}
					m_isSelectAll = false;
				}
				else
				{
					try
					{
						stringPositionInternal = GetStringIndexFromCaretPosition(caretPositionInternal);
						this.stringSelectPositionInternal = GetStringIndexFromCaretPosition(caretSelectPositionInternal);
						if (caretPositionInternal < caretSelectPositionInternal)
						{
							m_Text = this.text.Substring(0, stringPositionInternal) + this.text.Substring(this.stringSelectPositionInternal, this.text.Length - this.stringSelectPositionInternal);
							this.stringSelectPositionInternal = stringPositionInternal;
							caretSelectPositionInternal = caretPositionInternal;
						}
						else
						{
							m_Text = this.text.Substring(0, this.stringSelectPositionInternal) + this.text.Substring(stringPositionInternal, this.text.Length - stringPositionInternal);
							stringPositionInternal = this.stringSelectPositionInternal;
							stringPositionInternal = this.stringSelectPositionInternal;
							caretPositionInternal = caretSelectPositionInternal;
						}
					}
					catch (Exception obj2)
					{
						Debug.LogWarning(stringPositionInternal, null);
						Debug.LogWarning(this.stringSelectPositionInternal, null);
						Debug.LogWarning(caretPositionInternal, null);
						Debug.LogWarning(caretSelectPositionInternal, null);
						Debug.LogWarning(this.text.Length, null);
						Debug.LogError(obj2, null);
					}
				}
			}
		}

		private void ForwardSpace()
		{
			if (!m_ReadOnly)
			{
				if (hasSelection)
				{
					Delete();
					SendOnValueChangedAndUpdateLabel();
				}
				else if (m_isRichTextEditingAllowed)
				{
					if (stringPositionInternal < text.Length)
					{
						m_Text = text.Remove(stringPositionInternal, 1);
						SendOnValueChangedAndUpdateLabel();
					}
				}
				else if (caretPositionInternal < m_TextComponent.textInfo.characterCount - 1)
				{
					int num2 = stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(caretPositionInternal));
					m_Text = text.Remove(stringPositionInternal, 1);
					SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		private void Backspace()
		{
			if (!m_ReadOnly)
			{
				if (hasSelection)
				{
					Delete();
					SendOnValueChangedAndUpdateLabel();
				}
				else if (m_isRichTextEditingAllowed)
				{
					if (stringPositionInternal > 0)
					{
						stringPositionInternal = Math.Min(stringPositionInternal, m_Text.Length);
						if (m_Text.Length > 0)
						{
							m_Text = text.Remove(stringPositionInternal - 1, 1);
							stringSelectPositionInternal = --stringPositionInternal;
						}
						m_isLastKeyBackspace = true;
						SendOnValueChangedAndUpdateLabel();
					}
				}
				else
				{
					if (caretPositionInternal > 0)
					{
						m_Text = text.Remove(GetStringIndexFromCaretPosition(caretPositionInternal - 1), 1);
						caretSelectPositionInternal = --caretPositionInternal;
						int num2 = stringSelectPositionInternal = (stringPositionInternal = GetStringIndexFromCaretPosition(caretPositionInternal));
					}
					m_isLastKeyBackspace = true;
					SendOnValueChangedAndUpdateLabel();
				}
			}
		}

		protected virtual void Append(string input)
		{
			if (!m_ReadOnly && InPlaceEditing())
			{
				int i = 0;
				for (int length = input.Length; i < length; i++)
				{
					char c = input[i];
					if (c < ' ')
					{
						switch (c)
						{
						case '\t':
						case '\n':
						case '\r':
							break;
						default:
							continue;
						}
					}
					Append(c);
				}
			}
		}

		protected virtual void Append(char input)
		{
			if (!m_ReadOnly && InPlaceEditing())
			{
				if (onValidateInput != null)
				{
					input = onValidateInput(text, stringPositionInternal, input);
				}
				else
				{
					if (characterValidation == CharacterValidation.CustomValidator)
					{
						input = Validate(text, stringPositionInternal, input);
						if (input != 0)
						{
							SendOnValueChanged();
							UpdateLabel();
						}
						return;
					}
					if (characterValidation != 0)
					{
						input = Validate(text, stringPositionInternal, input);
					}
				}
				if (input != 0)
				{
					Insert(input);
				}
			}
		}

		private void Insert(char c)
		{
			if (!m_ReadOnly)
			{
				string text = c.ToString();
				Delete();
				if (characterLimit <= 0 || this.text.Length < characterLimit)
				{
					m_Text = this.text.Insert(m_StringPosition, text);
					stringSelectPositionInternal = (stringPositionInternal += text.Length);
					SendOnValueChanged();
				}
			}
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			SendOnValueChanged();
			UpdateLabel();
		}

		private void SendOnValueChanged()
		{
			if (onValueChanged != null)
			{
				onValueChanged.Invoke(text);
			}
		}

		protected void SendOnEndEdit()
		{
			if (onEndEdit != null)
			{
				onEndEdit.Invoke(m_Text);
			}
		}

		protected void SendOnSubmit()
		{
			if (onSubmit != null)
			{
				onSubmit.Invoke(m_Text);
			}
		}

		protected void SendOnFocus()
		{
			if (onSelect != null)
			{
				onSelect.Invoke(m_Text);
			}
		}

		protected void SendOnFocusLost()
		{
			if (onDeselect != null)
			{
				onDeselect.Invoke(m_Text);
			}
		}

		protected void SendOnTextSelection()
		{
			m_isSelected = true;
			if (onTextSelection != null)
			{
				onTextSelection.Invoke(m_Text, stringPositionInternal, stringSelectPositionInternal);
			}
		}

		protected void SendOnEndTextSelection()
		{
			if (m_isSelected)
			{
				if (onEndTextSelection != null)
				{
					onEndTextSelection.Invoke(m_Text, stringPositionInternal, stringSelectPositionInternal);
				}
				m_isSelected = false;
			}
		}

		protected void UpdateLabel()
		{
			if ((UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null && (UnityEngine.Object)m_TextComponent.font != (UnityEngine.Object)null)
			{
				string text = (Input.compositionString.Length <= 0) ? this.text : (this.text.Substring(0, m_StringPosition) + Input.compositionString + this.text.Substring(m_StringPosition));
				string str = (inputType != InputType.Password) ? text : new string(asteriskChar, text.Length);
				bool flag = string.IsNullOrEmpty(text);
				if ((UnityEngine.Object)m_Placeholder != (UnityEngine.Object)null)
				{
					m_Placeholder.enabled = flag;
				}
				if (!flag)
				{
					SetCaretVisible();
				}
				m_TextComponent.text = str + "\u200b";
				MarkGeometryAsDirty();
				m_IsScrollbarUpdateRequired = true;
			}
		}

		private void UpdateScrollbar()
		{
			if ((bool)m_VerticalScrollbar)
			{
				float size = m_TextViewport.rect.height / m_TextComponent.preferredHeight;
				m_IsUpdatingScrollbarValues = true;
				m_VerticalScrollbar.size = size;
				Vector2 anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
				float num = anchoredPosition.y / (m_TextComponent.preferredHeight - m_TextViewport.rect.height);
				m_VerticalScrollbar.value = num;
				m_ScrollPosition = num;
			}
		}

		private void OnScrollbarValueChange(float value)
		{
			if (m_IsUpdatingScrollbarValues)
			{
				m_IsUpdatingScrollbarValues = false;
			}
			else if (!(value < 0f) && !(value > 1f))
			{
				AdjustTextPositionRelativeToViewport(value);
				m_ScrollPosition = value;
			}
		}

		private void AdjustTextPositionRelativeToViewport(float relativePosition)
		{
			TMP_TextInfo textInfo = m_TextComponent.textInfo;
			if (textInfo != null && textInfo.lineInfo != null && textInfo.lineCount != 0 && textInfo.lineCount <= textInfo.lineInfo.Length)
			{
				RectTransform rectTransform = m_TextComponent.rectTransform;
				Vector2 anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
				rectTransform.anchoredPosition = new Vector2(anchoredPosition.x, (m_TextComponent.preferredHeight - m_TextViewport.rect.height) * relativePosition);
				AssignPositioningIfNeeded();
			}
		}

		private int GetCaretPositionFromStringIndex(int stringIndex)
		{
			int characterCount = m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if (m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
				{
					return i;
				}
			}
			return characterCount;
		}

		private int GetStringIndexFromCaretPosition(int caretPosition)
		{
			ClampCaretPos(ref caretPosition);
			return m_TextComponent.textInfo.characterInfo[caretPosition].index;
		}

		public void ForceLabelUpdate()
		{
			UpdateLabel();
		}

		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				UpdateGeometry();
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void UpdateGeometry()
		{
			if (shouldHideMobileInput && !((UnityEngine.Object)m_CachedInputRenderer == (UnityEngine.Object)null))
			{
				OnFillVBO(mesh);
				m_CachedInputRenderer.SetMesh(mesh);
			}
		}

		private void AssignPositioningIfNeeded()
		{
			if ((UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null && (UnityEngine.Object)caretRectTrans != (UnityEngine.Object)null && (caretRectTrans.localPosition != m_TextComponent.rectTransform.localPosition || caretRectTrans.localRotation != m_TextComponent.rectTransform.localRotation || caretRectTrans.localScale != m_TextComponent.rectTransform.localScale || caretRectTrans.anchorMin != m_TextComponent.rectTransform.anchorMin || caretRectTrans.anchorMax != m_TextComponent.rectTransform.anchorMax || caretRectTrans.anchoredPosition != m_TextComponent.rectTransform.anchoredPosition || caretRectTrans.sizeDelta != m_TextComponent.rectTransform.sizeDelta || caretRectTrans.pivot != m_TextComponent.rectTransform.pivot))
			{
				caretRectTrans.localPosition = m_TextComponent.rectTransform.localPosition;
				caretRectTrans.localRotation = m_TextComponent.rectTransform.localRotation;
				caretRectTrans.localScale = m_TextComponent.rectTransform.localScale;
				caretRectTrans.anchorMin = m_TextComponent.rectTransform.anchorMin;
				caretRectTrans.anchorMax = m_TextComponent.rectTransform.anchorMax;
				caretRectTrans.anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
				caretRectTrans.sizeDelta = m_TextComponent.rectTransform.sizeDelta;
				caretRectTrans.pivot = m_TextComponent.rectTransform.pivot;
			}
		}

		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!isFocused && m_ResetOnDeActivation)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					if (isStringPositionDirty)
					{
						stringPositionInternal = GetStringIndexFromCaretPosition(m_CaretPosition);
						stringSelectPositionInternal = GetStringIndexFromCaretPosition(m_CaretSelectPosition);
						isStringPositionDirty = false;
					}
					if (!hasSelection)
					{
						GenerateCaret(vertexHelper, Vector2.zero);
						SendOnEndTextSelection();
					}
					else
					{
						GenerateHightlight(vertexHelper, Vector2.zero);
						SendOnTextSelection();
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (m_CaretVisible)
			{
				if (m_CursorVerts == null)
				{
					CreateCursorVerts();
				}
				float num = (float)m_CaretWidth;
				int characterCount = m_TextComponent.textInfo.characterCount;
				Vector2 vector = Vector2.zero;
				float num2 = 0f;
				caretPositionInternal = GetCaretPositionFromStringIndex(stringPositionInternal);
				TMP_CharacterInfo tMP_CharacterInfo;
				if (caretPositionInternal == 0)
				{
					tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[0];
					vector = new Vector2(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
					num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
				}
				else if (caretPositionInternal < characterCount)
				{
					tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[caretPositionInternal];
					vector = new Vector2(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
					num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
				}
				else
				{
					tMP_CharacterInfo = m_TextComponent.textInfo.characterInfo[characterCount - 1];
					vector = new Vector2(tMP_CharacterInfo.xAdvance, tMP_CharacterInfo.descender);
					num2 = tMP_CharacterInfo.ascender - tMP_CharacterInfo.descender;
				}
				if ((isFocused && vector != m_LastPosition) || m_forceRectTransformAdjustment)
				{
					AdjustRectTransformRelativeToViewport(vector, num2, tMP_CharacterInfo.isVisible);
				}
				m_LastPosition = vector;
				float num3 = vector.y + num2;
				float y = num3 - num2;
				m_CursorVerts[0].position = new Vector3(vector.x, y, 0f);
				m_CursorVerts[1].position = new Vector3(vector.x, num3, 0f);
				m_CursorVerts[2].position = new Vector3(vector.x + num, num3, 0f);
				m_CursorVerts[3].position = new Vector3(vector.x + num, y, 0f);
				m_CursorVerts[0].color = caretColor;
				m_CursorVerts[1].color = caretColor;
				m_CursorVerts[2].color = caretColor;
				m_CursorVerts[3].color = caretColor;
				vbo.AddUIVertexQuad(m_CursorVerts);
				int height = Screen.height;
				vector.y = (float)height - vector.y;
				Input.compositionCursorPos = vector;
			}
		}

		private void CreateCursorVerts()
		{
			m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < m_CursorVerts.Length; i++)
			{
				m_CursorVerts[i] = UIVertex.simpleVert;
				m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

		private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
		{
			TMP_TextInfo textInfo = m_TextComponent.textInfo;
			caretPositionInternal = (m_CaretPosition = GetCaretPositionFromStringIndex(stringPositionInternal));
			caretSelectPositionInternal = (m_CaretSelectPosition = GetCaretPositionFromStringIndex(stringSelectPositionInternal));
			float num = 0f;
			Vector2 startPosition;
			if (caretSelectPositionInternal < textInfo.characterCount)
			{
				int num2 = Mathf.Min(textInfo.characterInfo.Length - 1, caretSelectPositionInternal);
				TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[num2];
				startPosition = new Vector2(tMP_CharacterInfo.origin, tMP_CharacterInfo.descender);
				num = textInfo.characterInfo[caretSelectPositionInternal].ascender - textInfo.characterInfo[caretSelectPositionInternal].descender;
			}
			else
			{
				int num3 = Mathf.Min(textInfo.characterInfo.Length - 1, caretSelectPositionInternal - 1);
				TMP_CharacterInfo tMP_CharacterInfo2 = textInfo.characterInfo[num3];
				startPosition = new Vector2(tMP_CharacterInfo2.xAdvance, tMP_CharacterInfo2.descender);
				try
				{
					num = textInfo.characterInfo[caretSelectPositionInternal - 1].ascender - textInfo.characterInfo[caretSelectPositionInternal - 1].descender;
				}
				catch (Exception obj)
				{
					Debug.LogWarning(obj, null);
					string text = "";
					Transform transform = base.transform;
					while ((UnityEngine.Object)transform != (UnityEngine.Object)null)
					{
						text = transform.name + "." + text;
						transform = transform.parent;
					}
					Debug.LogWarning(text, null);
					num = 0f;
				}
			}
			AdjustRectTransformRelativeToViewport(startPosition, num, true);
			int num4 = Mathf.Max(0, caretPositionInternal);
			int num5 = Mathf.Max(0, caretSelectPositionInternal);
			if (num4 > num5)
			{
				int num6 = num4;
				num4 = num5;
				num5 = num6;
			}
			num5--;
			int num7 = textInfo.characterInfo[Math.Min(textInfo.characterInfo.Length - 1, num4)].lineNumber;
			int lastCharacterIndex = textInfo.lineInfo[num7].lastCharacterIndex;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.zero;
			simpleVert.color = selectionColor;
			for (int i = num4; i <= num5 && i < textInfo.characterCount; i++)
			{
				if (i == lastCharacterIndex || i == num5)
				{
					TMP_CharacterInfo tMP_CharacterInfo3 = textInfo.characterInfo[num4];
					TMP_CharacterInfo tMP_CharacterInfo4 = textInfo.characterInfo[i];
					if (i > 0 && tMP_CharacterInfo4.character == '\n' && textInfo.characterInfo[i - 1].character == '\r')
					{
						tMP_CharacterInfo4 = textInfo.characterInfo[i - 1];
					}
					Vector2 vector = new Vector2(tMP_CharacterInfo3.origin, textInfo.lineInfo[num7].ascender);
					Vector2 vector2 = new Vector2(tMP_CharacterInfo4.xAdvance, textInfo.lineInfo[num7].descender);
					int currentVertCount = vbo.currentVertCount;
					simpleVert.position = new Vector3(vector.x, vector2.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector2.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector.x, vector.y, 0f);
					vbo.AddVert(simpleVert);
					vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
					vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
					num4 = i + 1;
					num7++;
					if (num7 < textInfo.lineCount)
					{
						lastCharacterIndex = textInfo.lineInfo[num7].lastCharacterIndex;
					}
				}
			}
			m_IsScrollbarUpdateRequired = true;
		}

		private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
		{
			float xMin = m_TextViewport.rect.xMin;
			float xMax = m_TextViewport.rect.xMax;
			float num = xMax;
			Vector2 anchoredPosition = m_TextComponent.rectTransform.anchoredPosition;
			float num2 = anchoredPosition.x + startPosition.x;
			Vector4 margin = m_TextComponent.margin;
			float num3 = num - (num2 + margin.z + (float)m_CaretWidth);
			if (num3 < 0f && (!multiLine || (multiLine && isCharVisible)))
			{
				m_TextComponent.rectTransform.anchoredPosition += new Vector2(num3, 0f);
				AssignPositioningIfNeeded();
			}
			Vector2 anchoredPosition2 = m_TextComponent.rectTransform.anchoredPosition;
			float num4 = anchoredPosition2.x + startPosition.x;
			Vector4 margin2 = m_TextComponent.margin;
			float num5 = num4 - margin2.x - xMin;
			if (num5 < 0f)
			{
				m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f - num5, 0f);
				AssignPositioningIfNeeded();
			}
			if (m_LineType != 0)
			{
				float yMax = m_TextViewport.rect.yMax;
				Vector2 anchoredPosition3 = m_TextComponent.rectTransform.anchoredPosition;
				float num6 = yMax - (anchoredPosition3.y + startPosition.y + height);
				if (num6 < -0.0001f)
				{
					m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, num6);
					AssignPositioningIfNeeded();
					m_IsScrollbarUpdateRequired = true;
				}
				Vector2 anchoredPosition4 = m_TextComponent.rectTransform.anchoredPosition;
				float num7 = anchoredPosition4.y + startPosition.y - m_TextViewport.rect.yMin;
				if (num7 < 0f)
				{
					m_TextComponent.rectTransform.anchoredPosition -= new Vector2(0f, num7);
					AssignPositioningIfNeeded();
					m_IsScrollbarUpdateRequired = true;
				}
			}
			if (m_isLastKeyBackspace)
			{
				Vector2 anchoredPosition5 = m_TextComponent.rectTransform.anchoredPosition;
				float num8 = anchoredPosition5.x + m_TextComponent.textInfo.characterInfo[0].origin;
				Vector4 margin3 = m_TextComponent.margin;
				float num9 = num8 - margin3.x;
				Vector2 anchoredPosition6 = m_TextComponent.rectTransform.anchoredPosition;
				float num10 = anchoredPosition6.x + m_TextComponent.textInfo.characterInfo[m_TextComponent.textInfo.characterCount - 1].origin;
				Vector4 margin4 = m_TextComponent.margin;
				float num11 = num10 + margin4.z;
				Vector2 anchoredPosition7 = m_TextComponent.rectTransform.anchoredPosition;
				if (anchoredPosition7.x + startPosition.x <= xMin + 0.0001f)
				{
					if (num9 < xMin)
					{
						float x = Mathf.Min((xMax - xMin) / 2f, xMin - num9);
						m_TextComponent.rectTransform.anchoredPosition += new Vector2(x, 0f);
						AssignPositioningIfNeeded();
					}
				}
				else if (num11 < xMax && num9 < xMin)
				{
					float x2 = Mathf.Min(xMax - num11, xMin - num9);
					m_TextComponent.rectTransform.anchoredPosition += new Vector2(x2, 0f);
					AssignPositioningIfNeeded();
				}
				m_isLastKeyBackspace = false;
			}
			m_forceRectTransformAdjustment = false;
		}

		protected char Validate(string text, int pos, char ch)
		{
			if (characterValidation != 0 && base.enabled)
			{
				if (characterValidation == CharacterValidation.Integer || characterValidation == CharacterValidation.Decimal)
				{
					bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
					bool flag2 = stringPositionInternal == 0 || stringSelectPositionInternal == 0;
					if (!flag)
					{
						if (ch >= '0' && ch <= '9')
						{
							return ch;
						}
						if (ch == '-' && (pos == 0 || flag2))
						{
							return ch;
						}
						if (ch == '.' && characterValidation == CharacterValidation.Decimal && !text.Contains("."))
						{
							return ch;
						}
					}
				}
				else if (characterValidation == CharacterValidation.Digit)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
				}
				else if (characterValidation == CharacterValidation.Alphanumeric)
				{
					if (ch >= 'A' && ch <= 'Z')
					{
						return ch;
					}
					if (ch >= 'a' && ch <= 'z')
					{
						return ch;
					}
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
				}
				else if (characterValidation == CharacterValidation.Name)
				{
					char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
					char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
					if (char.IsLetter(ch))
					{
						if (char.IsLower(ch) && c == ' ')
						{
							return char.ToUpper(ch);
						}
						if (char.IsUpper(ch) && c != ' ' && c != '\'')
						{
							return char.ToLower(ch);
						}
						return ch;
					}
					switch (ch)
					{
					case '\'':
						if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
						{
							return ch;
						}
						break;
					case ' ':
						if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
						{
							return ch;
						}
						break;
					}
				}
				else if (characterValidation == CharacterValidation.EmailAddress)
				{
					if (ch >= 'A' && ch <= 'Z')
					{
						return ch;
					}
					if (ch >= 'a' && ch <= 'z')
					{
						return ch;
					}
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '@' && text.IndexOf('@') == -1)
					{
						return ch;
					}
					if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
					{
						return ch;
					}
					if (ch == '.')
					{
						char c3 = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
						char c4 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
						if (c3 != '.' && c4 != '.')
						{
							return ch;
						}
					}
				}
				else if (characterValidation == CharacterValidation.Regex)
				{
					if (Regex.IsMatch(ch.ToString(), m_RegexValue))
					{
						return ch;
					}
				}
				else if (characterValidation == CharacterValidation.CustomValidator && (UnityEngine.Object)m_InputValidator != (UnityEngine.Object)null)
				{
					char result = m_InputValidator.Validate(ref text, ref pos, ch);
					m_Text = text;
					int num3 = stringSelectPositionInternal = (stringPositionInternal = pos);
					return result;
				}
				return '\0';
			}
			return ch;
		}

		public void ActivateInputField()
		{
			if (!((UnityEngine.Object)m_TextComponent == (UnityEngine.Object)null) && !((UnityEngine.Object)m_TextComponent.font == (UnityEngine.Object)null) && IsActive() && IsInteractable())
			{
				if (isFocused && m_Keyboard != null && !m_Keyboard.active)
				{
					m_Keyboard.active = true;
					m_Keyboard.text = m_Text;
				}
				m_ShouldActivateNextUpdate = true;
			}
		}

		private void ActivateInputFieldInternal()
		{
			if (!((UnityEngine.Object)UnityEngine.EventSystems.EventSystem.current == (UnityEngine.Object)null))
			{
				if ((UnityEngine.Object)UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != (UnityEngine.Object)base.gameObject)
				{
					UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(base.gameObject);
				}
				if (TouchScreenKeyboard.isSupported)
				{
					if (Input.touchSupported)
					{
						TouchScreenKeyboard.hideInput = shouldHideMobileInput;
					}
					m_Keyboard = ((inputType != InputType.Password) ? TouchScreenKeyboard.Open(m_Text, keyboardType, inputType == InputType.AutoCorrect, multiLine) : TouchScreenKeyboard.Open(m_Text, keyboardType, false, multiLine, true));
					MoveTextEnd(false);
				}
				else
				{
					Input.imeCompositionMode = IMECompositionMode.On;
					OnFocus();
				}
				m_AllowInput = true;
				m_OriginalText = text;
				m_WasCanceled = false;
				SetCaretVisible();
				UpdateLabel();
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			SendOnFocus();
			ActivateInputField();
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				ActivateInputField();
			}
		}

		public void OnControlClick()
		{
		}

		public void DeactivateInputField()
		{
			if (m_AllowInput)
			{
				m_HasDoneFocusTransition = false;
				m_AllowInput = false;
				if ((UnityEngine.Object)m_Placeholder != (UnityEngine.Object)null)
				{
					m_Placeholder.enabled = string.IsNullOrEmpty(m_Text);
				}
				if ((UnityEngine.Object)m_TextComponent != (UnityEngine.Object)null && IsInteractable())
				{
					if (m_WasCanceled && m_RestoreOriginalTextOnEscape)
					{
						text = m_OriginalText;
					}
					if (m_Keyboard != null)
					{
						m_Keyboard.active = false;
						m_Keyboard = null;
					}
					if (m_ResetOnDeActivation)
					{
						m_StringPosition = (m_StringSelectPosition = 0);
						m_CaretPosition = (m_CaretSelectPosition = 0);
						m_TextComponent.rectTransform.localPosition = m_DefaultTransformPosition;
						if ((UnityEngine.Object)caretRectTrans != (UnityEngine.Object)null)
						{
							caretRectTrans.localPosition = Vector3.zero;
						}
					}
					SendOnEndEdit();
					SendOnEndTextSelection();
					Input.imeCompositionMode = IMECompositionMode.Auto;
				}
				MarkGeometryAsDirty();
				m_IsScrollbarUpdateRequired = true;
			}
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			DeactivateInputField();
			base.OnDeselect(eventData);
			SendOnFocusLost();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (IsActive() && IsInteractable())
			{
				if (!isFocused)
				{
					m_ShouldActivateNextUpdate = true;
				}
				SendOnSubmit();
			}
		}

		private void EnforceContentType()
		{
			switch (contentType)
			{
			case ContentType.Standard:
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.None;
				break;
			case ContentType.Autocorrected:
				m_InputType = InputType.AutoCorrect;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.None;
				break;
			case ContentType.IntegerNumber:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				m_CharacterValidation = CharacterValidation.Integer;
				break;
			case ContentType.DecimalNumber:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				m_CharacterValidation = CharacterValidation.Decimal;
				break;
			case ContentType.Alphanumeric:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				m_CharacterValidation = CharacterValidation.Alphanumeric;
				break;
			case ContentType.Name:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.Name;
				break;
			case ContentType.EmailAddress:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Standard;
				m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				m_CharacterValidation = CharacterValidation.EmailAddress;
				break;
			case ContentType.Password:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Password;
				m_KeyboardType = TouchScreenKeyboardType.Default;
				m_CharacterValidation = CharacterValidation.None;
				break;
			case ContentType.Pin:
				m_LineType = LineType.SingleLine;
				m_TextComponent.enableWordWrapping = false;
				m_InputType = InputType.Password;
				m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				m_CharacterValidation = CharacterValidation.Digit;
				break;
			}
		}

		private void SetTextComponentWrapMode()
		{
			if (!((UnityEngine.Object)m_TextComponent == (UnityEngine.Object)null))
			{
				if (m_LineType == LineType.SingleLine)
				{
					m_TextComponent.enableWordWrapping = false;
				}
				else
				{
					m_TextComponent.enableWordWrapping = true;
				}
			}
		}

		private void SetTextComponentRichTextMode()
		{
			if (!((UnityEngine.Object)m_TextComponent == (UnityEngine.Object)null))
			{
				m_TextComponent.richText = m_RichText;
			}
		}

		private void SetToCustomIfContentTypeIsNot(params ContentType[] allowedContentTypes)
		{
			if (contentType != ContentType.Custom)
			{
				for (int i = 0; i < allowedContentTypes.Length; i++)
				{
					if (contentType == allowedContentTypes[i])
					{
						return;
					}
				}
				contentType = ContentType.Custom;
			}
		}

		private void SetToCustom()
		{
			if (contentType != ContentType.Custom)
			{
				contentType = ContentType.Custom;
			}
		}

		private void SetToCustom(CharacterValidation characterValidation)
		{
			if (contentType == ContentType.Custom)
			{
				characterValidation = CharacterValidation.CustomValidator;
			}
			else
			{
				contentType = ContentType.Custom;
				characterValidation = CharacterValidation.CustomValidator;
			}
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			if (m_HasDoneFocusTransition)
			{
				state = SelectionState.Highlighted;
			}
			else if (state == SelectionState.Pressed)
			{
				m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

		public void SetGlobalPointSize(float pointSize)
		{
			TMP_Text tMP_Text = m_Placeholder as TMP_Text;
			if ((UnityEngine.Object)tMP_Text != (UnityEngine.Object)null)
			{
				tMP_Text.fontSize = pointSize;
			}
			textComponent.fontSize = pointSize;
		}

		public void SetGlobalFontAsset(TMP_FontAsset fontAsset)
		{
			TMP_Text tMP_Text = m_Placeholder as TMP_Text;
			if ((UnityEngine.Object)tMP_Text != (UnityEngine.Object)null)
			{
				tMP_Text.font = fontAsset;
			}
			textComponent.font = fontAsset;
		}

		Transform get_transform()
		{
			return base.transform;
		}

		Transform ICanvasElement.get_transform()
		{
			//ILSpy generated this explicit interface implementation from .override directive in get_transform
			return this.get_transform();
		}

		bool ICanvasElement.IsDestroyed()
		{
			return IsDestroyed();
		}
	}
}
