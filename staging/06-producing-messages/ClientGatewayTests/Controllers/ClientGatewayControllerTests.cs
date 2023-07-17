namespace ClientGatewayTests.Controllers;

using Moq;
using Confluent.Kafka;
using ClientGateway;
using ClientGateway.Controllers;
using Microsoft.Extensions.Logging;
using ClientGateway.Domain;

public class ClientGatewayControllerTests
{
    private Logger<ClientGatewayController> _logger;
    private Mock<IProducer<string, Biometrics>> _mockProducer;
    private ClientGatewayController _controller;
    private Biometrics _biometrics;

    [SetUp]
    public void Setup()
    {
        _logger = new Logger<ClientGatewayController>(new LoggerFactory());
        _mockProducer = new Mock<IProducer<string, Biometrics>>();
        _controller = new ClientGatewayController(_logger, _mockProducer.Object);
        _biometrics = new Biometrics
        (
            Guid.NewGuid(),
            new List<HeartRate>(),
            new List<StepCount>(),
            10
        );
    }

    [Test]
    public async Task RecordMeasurements_ShouldProduceTheExpectedMessage()
    {
        var expectedTopic = "BiometricsImported";
        var expectedMessage = new Message<string, Biometrics>
        {
            Value = _biometrics,
            Key = _biometrics.DeviceId.ToString(),
        };

        _mockProducer.Setup(producer => producer.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, Biometrics>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new DeliveryResult<string, Biometrics>
            {
                Message = expectedMessage
            }));

        var result = await _controller.RecordMeasurements(expectedMessage.Value);

        _mockProducer.Verify(producer => producer.ProduceAsync(
            expectedTopic,
            It.Is<Message<string, Biometrics>>(msg => msg.Value == expectedMessage.Value),
            It.IsAny<CancellationToken>()));

        _mockProducer.Verify(producer => producer.Flush(It.IsAny<CancellationToken>()), Times.Once());

        Assert.That(result.Value, Is.EqualTo(expectedMessage.Value));
    }

    [Test]
    public void RecordMeasurements_ShouldReturnAFailure_IfTheMessageProducerFails()
    {
        var expectedTopic = "BiometricsImported";
        var expectedMessage = new Message<String, Biometrics>
        {
            Value = _biometrics
        };

        _mockProducer.Setup(producer => producer.ProduceAsync(It.IsAny<String>(), It.IsAny<Message<String, Biometrics>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromException<DeliveryResult<String, Biometrics>>(new Exception("Boom")));

        Assert.ThrowsAsync<Exception>(() => _controller.RecordMeasurements(expectedMessage.Value));

        _mockProducer.Verify(producer => producer.ProduceAsync(
            expectedTopic,
            It.Is<Message<String, Biometrics>>(msg => msg.Value == expectedMessage.Value),
            It.IsAny<CancellationToken>()));
    }
}

