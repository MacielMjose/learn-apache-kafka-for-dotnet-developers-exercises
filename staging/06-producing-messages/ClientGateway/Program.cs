using ClientGateway;
using ClientGateway.Controllers;
using ClientGateway.Extensions;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ClientGateway.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO: Load a config of type ProducerConfig from the "Kafka" section of the config:
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("Kafka"));

//Schema Registry configuration
builder.Services.Configure<SchemaRegistryConfig>(
    builder.Configuration.GetSection("SchemaRegistry")
);

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>().Value;

    return new CachedSchemaRegistryClient(config);
});

//TODO: Register an IProducer of type <String, String>
builder.Services.AddSingleton<IProducer<String, Biometrics>>(sp =>
{
    var config = sp.GetRequiredService<IOptions<ProducerConfig>>();
    var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();
    //var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfigs);    
    var serializer = new JsonSerializer<Biometrics>(schemaRegistry);

    return new ProducerBuilder<string, Biometrics>(config.Value.ToKeyValuePairs())
        .SetValueSerializer(serializer)
        .Build();
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();