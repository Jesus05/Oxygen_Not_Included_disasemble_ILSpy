using System;
using UnityEngine;

public class KCompBuilder : MonoBehaviour
{
	[Serializable]
	public struct BodyData
	{
		public HashedString headShape;

		public HashedString mouth;

		public HashedString neck;

		public HashedString eyes;

		public HashedString hair;

		public HashedString body;

		public HashedString arms;

		public HashedString hat;
	}
}
