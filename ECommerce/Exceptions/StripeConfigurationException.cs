namespace ECommerce.Exceptions
{
    public class StripeConfigurationException : Exception
    {
        public string ConfigKey { get; }

        public StripeConfigurationException(string configKey)
            : base($"Stripe configuration key '{configKey}' is missing or invalid")
        {
            ConfigKey = configKey;
        }
    }
}
