using ProcGen;
using System;
using System.Diagnostics;

[Serializable]
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

	public void Mod(WorldTrait.ElementBandModifier mod)
	{
		Debug.Assert(mod.element == base.content);
		base.bandSize *= mod.bandMultiplier;
		if (overrides == null)
		{
			overrides = new SampleDescriber.Override();
		}
		overrides.ModMultiplyMass(mod.massMultiplier);
	}
}
