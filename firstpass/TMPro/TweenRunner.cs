using System.Collections;
using UnityEngine;

namespace TMPro
{
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;

		protected IEnumerator m_Tween;

		private static IEnumerator Start(T tweenInfo)
		{
			if (tweenInfo.ValidTarget())
			{
				float elapsedTime2 = 0f;
				if (elapsedTime2 < tweenInfo.duration)
				{
					elapsedTime2 += ((!tweenInfo.ignoreTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime);
					float percentage = Mathf.Clamp01(elapsedTime2 / tweenInfo.duration);
					tweenInfo.TweenValue(percentage);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				tweenInfo.TweenValue(1f);
			}
		}

		public void Init(MonoBehaviour coroutineContainer)
		{
			m_CoroutineContainer = coroutineContainer;
		}

		public void StartTween(T info)
		{
			if ((Object)m_CoroutineContainer == (Object)null)
			{
				Debug.LogWarning("Coroutine container not configured... did you forget to call Init?", null);
			}
			else
			{
				StopTween();
				if (!m_CoroutineContainer.gameObject.activeInHierarchy)
				{
					info.TweenValue(1f);
				}
				else
				{
					m_Tween = Start(info);
					m_CoroutineContainer.StartCoroutine(m_Tween);
				}
			}
		}

		public void StopTween()
		{
			if (m_Tween != null)
			{
				m_CoroutineContainer.StopCoroutine(m_Tween);
				m_Tween = null;
			}
		}
	}
}
