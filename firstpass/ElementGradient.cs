using ProcGen;
using System.Diagnostics;

[DebuggerDisplay("{content} {bandSize} {maxValue}")]
public class ElementGradient : Gradient<string>
{
	public SampleDescriber.Override overrides
	{
		get;
		set;
	}

	public ElementGradient()
		: base((string)null, 0f)
	{
	}

	public ElementGradient(string content, float bandSize, SampleDescriber.Override overrides)
		: base(content, bandSize)
	{
		this.overrides = overrides;
	}
}
