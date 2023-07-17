public class ProducerConfig
{
    public string BootstrapServers { get; set; }
    public string ClientId { get; set; }
    public string SecurityProtocol { get; set; }
    public string SaslMechanism { get; set; }
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }
}