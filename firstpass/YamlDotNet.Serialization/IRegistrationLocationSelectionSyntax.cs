namespace YamlDotNet.Serialization
{
	public interface IRegistrationLocationSelectionSyntax<TBaseRegistrationType>
	{
		void InsteadOf<TRegistrationType>() where TRegistrationType : TBaseRegistrationType;

		void Before<TRegistrationType>() where TRegistrationType : TBaseRegistrationType;

		void After<TRegistrationType>() where TRegistrationType : TBaseRegistrationType;

		void OnTop();

		void OnBottom();
	}
}
