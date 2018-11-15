namespace YamlDotNet.Serialization
{
	public interface ITrackingRegistrationLocationSelectionSyntax<TBaseRegistrationType>
	{
		void InsteadOf<TRegistrationType>() where TRegistrationType : TBaseRegistrationType;
	}
}
