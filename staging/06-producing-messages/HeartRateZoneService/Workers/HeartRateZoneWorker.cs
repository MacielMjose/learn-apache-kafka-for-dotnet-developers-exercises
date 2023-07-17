using Confluent.Kafka;
using static Confluent.Kafka.ConfigPropertyNames;
using HeartRateZoneService.Domain;

namespace HeartRateZoneService.Workers;

public class HeartRateZoneWorker : BackgroundService
{
    private readonly ILogger<HeartRateZoneWorker> _logger;
    private readonly IConsumer<String, Biometrics> _consunmer;
    private const string BiometricsImportedTopicName = "BiometricsImported";

    public HeartRateZoneWorker(ILogger<HeartRateZoneWorker> logger, IConsumer<String, Biometrics> consunmer)
    {
        _logger = logger;
        _consunmer = consunmer;
        logger.LogInformation("HeartRateZoneWorker is Active.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consunmer.Subscribe(BiometricsImportedTopicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = _consunmer.Consume(stoppingToken);
            await HandleMessage(consumeResult.Message.Value, stoppingToken);
        }

        _consunmer.Close();

        await Task.Delay(1000);
    }   

    protected virtual async Task HandleMessage(Biometrics biometrics, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Received: "+ biometrics.DeviceId);

        await Task.CompletedTask;
    }
}
