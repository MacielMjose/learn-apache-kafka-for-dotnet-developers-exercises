using System.Diagnostics.Metrics;
using System.Net;
using System.Runtime.CompilerServices;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using ClientGateway.Domain;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ClientGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientGatewayController : ControllerBase
{
    private readonly ILogger<ClientGatewayController> _logger;
    private readonly string BiometricsImportedTopicName = "BiometricsImported";
    private readonly IProducer<string, Biometrics> _producer;

    public ClientGatewayController(ILogger<ClientGatewayController> logger, IProducer<string, Biometrics> producer)
    {
        _logger = logger;
        _producer = producer;
        logger.LogInformation("ClientGatewayController is Active.");
    }

    [HttpGet("Hello")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public string Hello()
    {
        _logger.LogInformation("Hello World");
        return "Hello World";
    }

    [HttpPost("Biometrics")]
    [ProducesResponseType(typeof(Biometrics), (int)HttpStatusCode.Accepted)]
    public async Task<AcceptedResult> RecordMeasurements(Biometrics metrics)
    {
        _logger.LogInformation("Accepted biometrics");

        var message = new Message<string, Biometrics>
        {
            Key = metrics.DeviceId.ToString(),
            Value = metrics
        };

        var deliverResult = await _producer.ProduceAsync(BiometricsImportedTopicName, message);

        _producer.Flush();

        return Accepted(string.Empty, metrics);
    }
}



