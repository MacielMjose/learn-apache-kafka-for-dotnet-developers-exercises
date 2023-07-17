namespace ClientGateway.Extensions
{
    public static class ObjectSettingsExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this ProducerConfig settings)
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>();

            // Adicione aqui as propriedades específicas do objeto de configuração que deseja transformar em pares chave-valor
          
            keyValuePairs.Add(new KeyValuePair<string, string>("bootstrap.servers", settings.BootstrapServers));
            keyValuePairs.Add(new KeyValuePair<string, string>("client.id", settings.ClientId));
            keyValuePairs.Add(new KeyValuePair<string, string>("security.protocol", settings.SecurityProtocol));
            keyValuePairs.Add(new KeyValuePair<string, string>("sasl.mechanism", settings.SaslMechanism));
            keyValuePairs.Add(new KeyValuePair<string, string>("sasl.username", settings.SaslUsername));
            keyValuePairs.Add(new KeyValuePair<string, string>("sasl.password", settings.SaslPassword));
            // Adicione mais linhas para outras propriedades...

            return keyValuePairs;
        }
    }
}
