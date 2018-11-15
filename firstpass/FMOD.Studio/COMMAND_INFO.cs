namespace FMOD.Studio
{
	public struct COMMAND_INFO
	{
		private StringWrapper commandname;

		public int parentcommandindex;

		public int framenumber;

		public float frametime;

		public INSTANCETYPE instancetype;

		public INSTANCETYPE outputtype;

		public uint instancehandle;

		public uint outputhandle;
	}
}
