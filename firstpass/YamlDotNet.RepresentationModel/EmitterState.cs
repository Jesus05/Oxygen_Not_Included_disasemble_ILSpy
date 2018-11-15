using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	internal class EmitterState
	{
		private readonly HashSet<string> emittedAnchors = new HashSet<string>();

		public HashSet<string> EmittedAnchors => emittedAnchors;
	}
}
