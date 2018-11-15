using System;

namespace FMODUnity
{
	public class EventNotFoundException : Exception
	{
		public Guid Guid;

		public string Path;

		public EventNotFoundException(string path)
			: base("FMOD Studio event not found '" + path + "'")
		{
			Path = path;
		}

		public EventNotFoundException(Guid guid)
			: base("FMOD Studio event not found " + guid.ToString("b") + string.Empty)
		{
			Guid = guid;
		}
	}
}
