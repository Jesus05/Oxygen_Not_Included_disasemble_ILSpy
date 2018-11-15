namespace YamlDotNet.Core
{
	internal interface ILookAheadBuffer
	{
		bool EndOfInput
		{
			get;
		}

		char Peek(int offset);

		void Skip(int length);
	}
}
