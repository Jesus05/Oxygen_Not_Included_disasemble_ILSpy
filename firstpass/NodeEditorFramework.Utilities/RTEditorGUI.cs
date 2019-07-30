using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class RTEditorGUI
	{
		public static float labelWidth = 150f;

		public static float fieldWidth = 50f;

		public static float indent = 0f;

		private static GUIStyle seperator;

		private static Stack<bool> changeStack = new Stack<bool>();

		private static int activeFloatField = -1;

		private static float activeFloatFieldLastValue = 0f;

		private static string activeFloatFieldString = string.Empty;

		private static Material texVizMat;

		private static Material lineMaterial;

		private static Texture2D lineTexture;

		private static float textFieldHeight => GUI.skin.textField.CalcHeight(new GUIContent("i"), 10f);

		public static Rect PrefixLabel(Rect totalPos, GUIContent label, GUIStyle style)
		{
			if (label == GUIContent.none)
			{
				return totalPos;
			}
			Rect position = new Rect(totalPos.x + indent, totalPos.y, Mathf.Min(getLabelWidth() - indent, totalPos.width / 2f), totalPos.height);
			GUI.Label(position, label, style);
			return new Rect(totalPos.x + getLabelWidth(), totalPos.y, totalPos.width - getLabelWidth(), totalPos.height);
		}

		public static Rect PrefixLabel(Rect totalPos, float percentage, GUIContent label, GUIStyle style)
		{
			if (label == GUIContent.none)
			{
				return totalPos;
			}
			Rect position = new Rect(totalPos.x + indent, totalPos.y, totalPos.width * percentage, totalPos.height);
			GUI.Label(position, label, style);
			return new Rect(totalPos.x + totalPos.width * percentage, totalPos.y, totalPos.width * (1f - percentage), totalPos.height);
		}

		private static Rect IndentedRect(Rect source)
		{
			return new Rect(source.x + indent, source.y, source.width - indent, source.height);
		}

		private static float getLabelWidth()
		{
			if (labelWidth == 0f)
			{
				return 150f;
			}
			return labelWidth;
		}

		private static float getFieldWidth()
		{
			if (fieldWidth == 0f)
			{
				return 50f;
			}
			return fieldWidth;
		}

		private static Rect GetFieldRect(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			float minWidth = 0f;
			float maxWidth = 0f;
			if (label != GUIContent.none)
			{
				style.CalcMinMaxWidth(label, out minWidth, out maxWidth);
			}
			return GUILayoutUtility.GetRect(getFieldWidth() + minWidth + 5f, getFieldWidth() + maxWidth + 5f, textFieldHeight, textFieldHeight, options);
		}

		private static Rect GetSliderRect(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			float minWidth = 0f;
			float maxWidth = 0f;
			if (label != GUIContent.none)
			{
				style.CalcMinMaxWidth(label, out minWidth, out maxWidth);
			}
			return GUILayoutUtility.GetRect(getFieldWidth() + minWidth + 5f, getFieldWidth() + maxWidth + 5f + 100f, textFieldHeight, textFieldHeight, options);
		}

		private static Rect GetSliderRect(Rect sliderRect)
		{
			return new Rect(sliderRect.x, sliderRect.y, sliderRect.width - getFieldWidth() - 5f, sliderRect.height);
		}

		private static Rect GetSliderFieldRect(Rect sliderRect)
		{
			return new Rect(sliderRect.x + sliderRect.width - getFieldWidth(), sliderRect.y, getFieldWidth(), sliderRect.height);
		}

		public static void Space()
		{
			Space(6f);
		}

		public static void Space(float pixels)
		{
			GUILayoutUtility.GetRect(pixels, pixels);
		}

		public static void Seperator()
		{
			setupSeperator();
			GUILayout.Box(GUIContent.none, seperator, GUILayout.Height(1f));
		}

		public static void Seperator(Rect rect)
		{
			setupSeperator();
			GUI.Box(new Rect(rect.x, rect.y, rect.width, 1f), GUIContent.none, seperator);
		}

		private static void setupSeperator()
		{
			if (seperator == null)
			{
				seperator = new GUIStyle();
				seperator.normal.background = ColorToTex(1, new Color(0.6f, 0.6f, 0.6f));
				seperator.stretchWidth = true;
				seperator.margin = new RectOffset(0, 0, 7, 7);
			}
		}

		public static void BeginChangeCheck()
		{
			changeStack.Push(GUI.changed);
			GUI.changed = false;
		}

		public static bool EndChangeCheck()
		{
			bool changed = GUI.changed;
			if (changeStack.Count > 0)
			{
				GUI.changed = changeStack.Pop();
				if (changed && changeStack.Count > 0 && !changeStack.Peek())
				{
					changeStack.Pop();
					changeStack.Push(changed);
				}
			}
			else
			{
				Debug.LogWarning("Requesting more EndChangeChecks than issuing BeginChangeChecks!");
			}
			return changed;
		}

		public static bool Foldout(bool foldout, string content, params GUILayoutOption[] options)
		{
			return Foldout(foldout, new GUIContent(content), options);
		}

		public static bool Foldout(bool foldout, string content, GUIStyle style, params GUILayoutOption[] options)
		{
			return Foldout(foldout, new GUIContent(content), style, options);
		}

		public static bool Foldout(bool foldout, GUIContent content, params GUILayoutOption[] options)
		{
			return Foldout(foldout, content, GUI.skin.toggle, options);
		}

		public static bool Foldout(bool foldout, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			return GUILayout.Toggle(foldout, content, style, options);
		}

		public static bool Toggle(bool toggle, string content, params GUILayoutOption[] options)
		{
			return Toggle(toggle, new GUIContent(content), options);
		}

		public static bool Toggle(bool toggle, string content, GUIStyle style, params GUILayoutOption[] options)
		{
			return Toggle(toggle, new GUIContent(content), style, options);
		}

		public static bool Toggle(bool toggle, GUIContent content, params GUILayoutOption[] options)
		{
			return Toggle(toggle, content, GUI.skin.toggle, options);
		}

		public static bool Toggle(bool toggle, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			return GUILayout.Toggle(toggle, content, style, options);
		}

		public static string TextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			if (style == null)
			{
				style = GUI.skin.textField;
			}
			Rect fieldRect = GetFieldRect(label, style, options);
			Rect position = PrefixLabel(fieldRect, 0.5f, label, style);
			text = GUI.TextField(position, text);
			return text;
		}

		public static int OptionSlider(GUIContent label, int selected, string[] selectableOptions, params GUILayoutOption[] options)
		{
			return OptionSlider(label, selected, selectableOptions, GUI.skin.label, options);
		}

		public static int OptionSlider(GUIContent label, int selected, string[] selectableOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			if (style == null)
			{
				style = GUI.skin.textField;
			}
			Rect sliderRect = GetSliderRect(label, style, options);
			Rect sliderRect2 = PrefixLabel(sliderRect, 0.5f, label, style);
			selected = Mathf.RoundToInt(GUI.HorizontalSlider(GetSliderRect(sliderRect2), (float)selected, 0f, (float)(selectableOptions.Length - 1)));
			GUI.Label(GetSliderFieldRect(sliderRect2), selectableOptions[selected]);
			return selected;
		}

		public static int MathPowerSlider(GUIContent label, int baseValue, int value, int minPow, int maxPow, params GUILayoutOption[] options)
		{
			int power = (int)Math.Floor(Math.Log((double)value) / Math.Log((double)baseValue));
			power = MathPowerSliderRaw(label, baseValue, power, minPow, maxPow, options);
			return (int)Math.Pow((double)baseValue, (double)power);
		}

		public static int MathPowerSliderRaw(GUIContent label, int baseValue, int power, int minPow, int maxPow, params GUILayoutOption[] options)
		{
			Rect sliderRect = GetSliderRect(label, GUI.skin.label, options);
			Rect sliderRect2 = PrefixLabel(sliderRect, 0.5f, label, GUI.skin.label);
			power = Mathf.RoundToInt(GUI.HorizontalSlider(GetSliderRect(sliderRect2), (float)power, (float)minPow, (float)maxPow));
			GUI.Label(GetSliderFieldRect(sliderRect2), Mathf.Pow((float)baseValue, (float)power).ToString());
			return power;
		}

		public static int IntSlider(string label, int value, int minValue, int maxValue, params GUILayoutOption[] options)
		{
			return (int)Slider(new GUIContent(label), (float)value, (float)minValue, (float)maxValue, options);
		}

		public static int IntSlider(GUIContent label, int value, int minValue, int maxValue, params GUILayoutOption[] options)
		{
			return (int)Slider(label, (float)value, (float)minValue, (float)maxValue, options);
		}

		public static int IntSlider(int value, int minValue, int maxValue, params GUILayoutOption[] options)
		{
			return (int)Slider(GUIContent.none, (float)value, (float)minValue, (float)maxValue, options);
		}

		public static int IntField(string label, int value, params GUILayoutOption[] options)
		{
			return (int)FloatField(new GUIContent(label), (float)value, options);
		}

		public static int IntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			return (int)FloatField(label, (float)value, options);
		}

		public static int IntField(int value, params GUILayoutOption[] options)
		{
			return (int)FloatField((float)value, options);
		}

		public static float Slider(float value, float minValue, float maxValue, params GUILayoutOption[] options)
		{
			return Slider(GUIContent.none, value, minValue, maxValue, options);
		}

		public static float Slider(string label, float value, float minValue, float maxValue, params GUILayoutOption[] options)
		{
			return Slider(new GUIContent(label), value, minValue, maxValue, options);
		}

		public static float Slider(GUIContent label, float value, float minValue, float maxValue, params GUILayoutOption[] options)
		{
			Rect sliderRect = GetSliderRect(label, GUI.skin.label, options);
			Rect sliderRect2 = PrefixLabel(sliderRect, 0.5f, label, GUI.skin.label);
			value = GUI.HorizontalSlider(GetSliderRect(sliderRect2), value, minValue, maxValue);
			value = Mathf.Min(maxValue, Mathf.Max(minValue, FloatField(GetSliderFieldRect(sliderRect2), value, GUILayout.Width(60f))));
			return value;
		}

		public static float FloatField(string label, float value, params GUILayoutOption[] fieldOptions)
		{
			return FloatField(new GUIContent(label), value, fieldOptions);
		}

		public static float FloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			Rect fieldRect = GetFieldRect(label, GUI.skin.label, options);
			Rect pos = PrefixLabel(fieldRect, 0.5f, label, GUI.skin.label);
			return FloatField(pos, value, options);
		}

		public static float FloatField(float value, params GUILayoutOption[] options)
		{
			Rect fieldRect = GetFieldRect(GUIContent.none, null, options);
			return FloatField(fieldRect, value, options);
		}

		public static float FloatField(Rect pos, float value, params GUILayoutOption[] options)
		{
			int num = GUIUtility.GetControlID("FloatField".GetHashCode(), FocusType.Keyboard, pos) + 1;
			if (num == 0)
			{
				return value;
			}
			bool flag = activeFloatField == num;
			bool flag2 = num == GUIUtility.keyboardControl;
			if (flag2 && flag && activeFloatFieldLastValue != value)
			{
				activeFloatFieldLastValue = value;
				activeFloatFieldString = value.ToString();
			}
			string text = (!flag) ? value.ToString() : activeFloatFieldString;
			string text2 = GUI.TextField(pos, text);
			if (flag)
			{
				activeFloatFieldString = text2;
			}
			bool flag3 = true;
			if (text2 == string.Empty)
			{
				value = (activeFloatFieldLastValue = 0f);
			}
			else if (text2 != value.ToString())
			{
				flag3 = float.TryParse(text2, out float result);
				if (flag3)
				{
					value = (activeFloatFieldLastValue = result);
				}
			}
			if (flag2 && !flag)
			{
				activeFloatField = num;
				activeFloatFieldString = text2;
				activeFloatFieldLastValue = value;
			}
			else if (!flag2 && flag)
			{
				activeFloatField = -1;
				if (!flag3)
				{
					value = text2.ForceParse();
				}
			}
			return value;
		}

		public static float ForceParse(this string str)
		{
			if (float.TryParse(str, out float result))
			{
				return result;
			}
			bool flag = false;
			List<char> list = new List<char>(str);
			for (int i = 0; i < list.Count; i++)
			{
				UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(str[i]);
				if (unicodeCategory != UnicodeCategory.DecimalDigitNumber)
				{
					list.RemoveRange(i, list.Count - i);
					break;
				}
				if (str[i] == '.')
				{
					if (flag)
					{
						list.RemoveRange(i, list.Count - i);
						break;
					}
					flag = true;
				}
			}
			if (list.Count == 0)
			{
				return 0f;
			}
			str = new string(list.ToArray());
			if (!float.TryParse(str, out result))
			{
				Debug.LogError("Could not parse " + str);
			}
			return result;
		}

		public static T ObjectField<T>(T obj, bool allowSceneObjects) where T : UnityEngine.Object
		{
			return ObjectField(GUIContent.none, obj, allowSceneObjects);
		}

		public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : UnityEngine.Object
		{
			return ObjectField(new GUIContent(label), obj, allowSceneObjects);
		}

		public static T ObjectField<T>(GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : UnityEngine.Object
		{
			bool flag = false;
			if (obj.GetType() == typeof(Texture2D))
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(label);
				flag = GUILayout.Button(obj as Texture2D, GUILayout.MaxWidth(64f), GUILayout.MaxHeight(64f));
				GUILayout.EndHorizontal();
			}
			else
			{
				GUIStyle style = new GUIStyle(GUI.skin.box);
				flag = GUILayout.Button(label, style);
			}
			if (!flag)
			{
				goto IL_0094;
			}
			goto IL_0094;
			IL_0094:
			return obj;
		}

		public static Enum EnumPopup(Enum selected)
		{
			return EnumPopup(GUIContent.none, selected);
		}

		public static Enum EnumPopup(string label, Enum selected)
		{
			return EnumPopup(new GUIContent(label), selected);
		}

		public static Enum EnumPopup(GUIContent label, Enum selected)
		{
			label.text = label.text + ": " + selected.ToString();
			GUILayout.Label(label);
			return selected;
		}

		public static int Popup(GUIContent label, int selected, string[] displayedOptions)
		{
			GUILayout.BeginHorizontal();
			label.text = label.text + ": " + selected.ToString();
			GUILayout.Label(label);
			GUILayout.EndHorizontal();
			return selected;
		}

		public static int Popup(string label, int selected, string[] displayedOptions)
		{
			GUILayout.Label(label + ": " + selected.ToString());
			return selected;
		}

		public static int Popup(int selected, string[] displayedOptions)
		{
			return Popup(string.Empty, selected, displayedOptions);
		}

		public static void DrawTexture(Texture texture, int texSize, GUIStyle style, params GUILayoutOption[] options)
		{
			DrawTexture(texture, texSize, style, 1, 2, 3, 4, options);
		}

		public static void DrawTexture(Texture texture, int texSize, GUIStyle style, int shuffleRed, int shuffleGreen, int shuffleBlue, int shuffleAlpha, params GUILayoutOption[] options)
		{
			if ((UnityEngine.Object)texVizMat == (UnityEngine.Object)null)
			{
				texVizMat = new Material(Shader.Find("Hidden/GUITextureClip_ChannelControl"));
			}
			texVizMat.SetInt("shuffleRed", shuffleRed);
			texVizMat.SetInt("shuffleGreen", shuffleGreen);
			texVizMat.SetInt("shuffleBlue", shuffleBlue);
			texVizMat.SetInt("shuffleAlpha", shuffleAlpha);
			if (options == null || options.Length == 0)
			{
				options = new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				};
			}
			Rect screenRect = (style != null) ? GUILayoutUtility.GetRect((float)texSize, (float)texSize, style, options) : GUILayoutUtility.GetRect((float)texSize, (float)texSize, options);
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(screenRect, texture, texVizMat);
			}
		}

		private static void SetupLineMat(Texture tex, Color col)
		{
			if ((UnityEngine.Object)lineMaterial == (UnityEngine.Object)null)
			{
				lineMaterial = new Material(Shader.Find("Hidden/LineShader"));
			}
			if ((UnityEngine.Object)tex == (UnityEngine.Object)null)
			{
				tex = (((UnityEngine.Object)lineTexture != (UnityEngine.Object)null) ? lineTexture : (lineTexture = ResourceManager.LoadTexture("Textures/AALine.png")));
			}
			lineMaterial.SetTexture("_LineTexture", tex);
			lineMaterial.SetColor("_LineColor", col);
			lineMaterial.SetPass(0);
		}

		public static void DrawBezier(Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, float width = 1f)
		{
			if (Event.current.type == EventType.Repaint)
			{
				int segmentCount = CalculateBezierSegmentCount(startPos, endPos, startTan, endTan);
				DrawBezier(startPos, endPos, startTan, endTan, col, tex, segmentCount, width);
			}
		}

		public static void DrawBezier(Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, int segmentCount, float width)
		{
			if (Event.current.type == EventType.Repaint || Event.current.type == EventType.KeyDown)
			{
				Vector2[] array = new Vector2[segmentCount + 1];
				for (int i = 0; i <= segmentCount; i++)
				{
					array[i] = GetBezierPoint((float)i / (float)segmentCount, startPos, endPos, startTan, endTan);
				}
				DrawPolygonLine(array, col, tex, width);
			}
		}

		public static void DrawBezier(Rect clippingRect, Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan, Color col, Texture2D tex, int segmentCount, float width)
		{
			if (Event.current.type == EventType.Repaint || Event.current.type == EventType.KeyDown)
			{
				Vector2[] array = new Vector2[segmentCount + 1];
				for (int i = 0; i <= segmentCount; i++)
				{
					array[i] = GetBezierPoint((float)i / (float)segmentCount, startPos, endPos, startTan, endTan);
				}
				DrawPolygonLine(clippingRect, array, col, tex, width);
			}
		}

		public static void DrawPolygonLine(Vector2[] points, Color col, Texture2D tex, float width = 1f)
		{
			DrawPolygonLine(GUIScaleUtility.getTopRect, points, col, tex, width);
		}

		public static void DrawPolygonLine(Rect clippingRect, Vector2[] points, Color col, Texture2D tex, float width = 1f)
		{
			if ((Event.current.type == EventType.Repaint || Event.current.type == EventType.KeyDown) && points.Length != 1)
			{
				if (points.Length == 2)
				{
					DrawLine(points[0], points[1], col, tex, width);
				}
				SetupLineMat(tex, col);
				GL.Begin(5);
				GL.Color(Color.white);
				float num3 = clippingRect.x = (clippingRect.y = 0f);
				Vector2 p = points[0];
				for (int i = 1; i < points.Length; i++)
				{
					Vector2 p2 = points[i];
					Vector2 vector = p;
					Vector2 vector2 = p2;
					if (SegmentRectIntersection(clippingRect, ref p, ref p2, out bool clippedP, out bool clippedP2))
					{
						Vector2 a = (i >= points.Length - 1) ? CalculateLinePerpendicular(vector, vector2) : CalculatePointPerpendicular(vector, vector2, points[i + 1]);
						if (clippedP)
						{
							GL.End();
							GL.Begin(5);
							DrawLineSegment(p, a * width / 2f);
						}
						if (i == 1)
						{
							DrawLineSegment(p, CalculateLinePerpendicular(p, p2) * width / 2f);
						}
						DrawLineSegment(p2, a * width / 2f);
					}
					else if (clippedP2)
					{
						GL.End();
						GL.Begin(5);
					}
					p = vector2;
				}
				GL.End();
			}
		}

		private static int CalculateBezierSegmentCount(Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan)
		{
			float num = Vector2.Angle(startTan - startPos, endPos - startPos) * Vector2.Angle(endTan - endPos, startPos - endPos) * (endTan.magnitude + startTan.magnitude);
			num = 2f + Mathf.Pow(num / 400f, 0.125f);
			float f = 1f + (startPos - endPos).magnitude;
			f = Mathf.Pow(f, 0.25f);
			return 4 + (int)(num * f);
		}

		private static Vector2 CalculateLinePerpendicular(Vector2 startPos, Vector2 endPos)
		{
			return new Vector2(endPos.y - startPos.y, startPos.x - endPos.x).normalized;
		}

		private static Vector2 CalculatePointPerpendicular(Vector2 prevPos, Vector2 pointPos, Vector2 nextPos)
		{
			return CalculateLinePerpendicular(pointPos, pointPos + (nextPos - prevPos));
		}

		private static Vector2 GetBezierPoint(float t, Vector2 startPos, Vector2 endPos, Vector2 startTan, Vector2 endTan)
		{
			float num = 1f - t;
			float d = num * t;
			return startPos * num * num * num + startTan * 3f * num * d + endTan * 3f * d * t + endPos * t * t * t;
		}

		private static void DrawLineSegment(Vector2 point, Vector2 perpendicular)
		{
			GL.TexCoord2(0f, 0f);
			GL.Vertex(point - perpendicular);
			GL.TexCoord2(0f, 1f);
			GL.Vertex(point + perpendicular);
		}

		public static void DrawLine(Vector2 startPos, Vector2 endPos, Color col, Texture2D tex, float width = 1f)
		{
			if (Event.current.type == EventType.Repaint)
			{
				DrawLine(GUIScaleUtility.getTopRect, startPos, endPos, col, tex, width);
			}
		}

		public static void DrawLine(Rect clippingRect, Vector2 startPos, Vector2 endPos, Color col, Texture2D tex, float width = 1f)
		{
			SetupLineMat(tex, col);
			GL.Begin(5);
			GL.Color(Color.white);
			float num3 = clippingRect.x = (clippingRect.y = 0f);
			if (SegmentRectIntersection(clippingRect, ref startPos, ref endPos))
			{
				Vector2 perpendicular = CalculateLinePerpendicular(startPos, endPos) * width / 2f;
				DrawLineSegment(startPos, perpendicular);
				DrawLineSegment(endPos, perpendicular);
			}
			GL.End();
		}

		public static List<Vector2> GetLine(Rect clippingRect, Vector2 startPos, Vector2 endPos, float width = 1f, bool noClip = false)
		{
			List<Vector2> list = new List<Vector2>();
			if (noClip || SegmentRectIntersection(clippingRect, ref startPos, ref endPos))
			{
				Vector2 b = CalculateLinePerpendicular(startPos, endPos) * width / 2f;
				list.Add(startPos - b);
				list.Add(endPos + b);
			}
			return list;
		}

		private static bool SegmentRectIntersection(Rect bounds, ref Vector2 p0, ref Vector2 p1)
		{
			bool clippedP;
			bool clippedP2;
			return SegmentRectIntersection(bounds, ref p0, ref p1, out clippedP, out clippedP2);
		}

		private static bool SegmentRectIntersection(Rect bounds, ref Vector2 p0, ref Vector2 p1, out bool clippedP0, out bool clippedP1)
		{
			float t = 0f;
			float t2 = 1f;
			float num = p1.x - p0.x;
			float num2 = p1.y - p0.y;
			if (ClipTest(0f - num, p0.x - bounds.xMin, ref t, ref t2) && ClipTest(num, bounds.xMax - p0.x, ref t, ref t2) && ClipTest(0f - num2, p0.y - bounds.yMin, ref t, ref t2) && ClipTest(num2, bounds.yMax - p0.y, ref t, ref t2))
			{
				clippedP0 = (t > 0f);
				clippedP1 = (t2 < 1f);
				if (clippedP1)
				{
					p1.x = p0.x + t2 * num;
					p1.y = p0.y + t2 * num2;
				}
				if (clippedP0)
				{
					p0.x += t * num;
					p0.y += t * num2;
				}
				return true;
			}
			clippedP1 = (clippedP0 = true);
			return false;
		}

		private static bool ClipTest(float p, float q, ref float t0, ref float t1)
		{
			float num = q / p;
			if (p < 0f)
			{
				if (num > t1)
				{
					return false;
				}
				if (num > t0)
				{
					t0 = num;
				}
			}
			else if (p > 0f)
			{
				if (num < t0)
				{
					return false;
				}
				if (num < t1)
				{
					t1 = num;
				}
			}
			else if (q < 0f)
			{
				return false;
			}
			return true;
		}

		public static Texture2D ColorToTex(int pxSize, Color col)
		{
			Texture2D texture2D = new Texture2D(pxSize, pxSize);
			texture2D.name = "RTEditorGUI";
			for (int i = 0; i < pxSize; i++)
			{
				for (int j = 0; j < pxSize; j++)
				{
					texture2D.SetPixel(i, j, col);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D Tint(Texture2D tex, Color color)
		{
			Texture2D texture2D = UnityEngine.Object.Instantiate(tex);
			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					texture2D.SetPixel(i, j, tex.GetPixel(i, j) * color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D RotateTextureCCW(Texture2D tex, int quarterSteps)
		{
			if ((UnityEngine.Object)tex == (UnityEngine.Object)null)
			{
				return null;
			}
			tex = UnityEngine.Object.Instantiate(tex);
			int width = tex.width;
			int height = tex.height;
			Color[] pixels = tex.GetPixels();
			Color[] array = new Color[width * height];
			for (int i = 0; i < quarterSteps; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < height; k++)
					{
						array[j * width + k] = pixels[(width - k - 1) * width + j];
					}
				}
				array.CopyTo(pixels, 0);
			}
			tex.SetPixels(pixels);
			tex.Apply();
			return tex;
		}
	}
}
