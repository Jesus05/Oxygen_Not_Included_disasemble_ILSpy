using System;

namespace FMODUnity
{
	[Serializable]
	public class EmitterRef
	{
		public StudioEventEmitter Target;

		public ParamRef[] Params;
	}
}
