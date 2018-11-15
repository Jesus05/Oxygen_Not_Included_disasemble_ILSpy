using System;
using System.Globalization;

namespace YamlDotNet
{
	internal sealed class CultureInfoAdapter : CultureInfo
	{
		private readonly IFormatProvider _provider;

		public CultureInfoAdapter(CultureInfo baseCulture, IFormatProvider provider)
			: base(baseCulture.LCID)
		{
			_provider = provider;
		}

		public override object GetFormat(Type formatType)
		{
			return _provider.GetFormat(formatType);
		}
	}
}
