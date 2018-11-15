using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[DisallowMultipleComponent]
	public class TMP_SpriteAnimator : MonoBehaviour
	{
		private Dictionary<int, bool> m_animations = new Dictionary<int, bool>(16);

		private TMP_Text m_TextComponent;

		private void Awake()
		{
			m_TextComponent = GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void StopAllAnimations()
		{
			StopAllCoroutines();
			m_animations.Clear();
		}

		public void DoSpriteAnimation(int currentCharacter, TMP_SpriteAsset spriteAsset, int start, int end, int framerate)
		{
			bool value = false;
			if (!m_animations.TryGetValue(currentCharacter, out value))
			{
				StartCoroutine(DoSpriteAnimationInternal(currentCharacter, spriteAsset, start, end, framerate));
				m_animations.Add(currentCharacter, true);
			}
		}

		private IEnumerator DoSpriteAnimationInternal(int currentCharacter, TMP_SpriteAsset spriteAsset, int start, int end, int framerate)
		{
			if (!((Object)m_TextComponent == (Object)null))
			{
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}
	}
}
