namespace MaterializedViewProcessor.Triggers;

public class CosmosDbChangeFeedTrigger(ILoggerFactory loggerFactory)
{
    private readonly Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<CosmosDbChangeFeedTrigger>();

    [Function(nameof(CosmosDbChangeFeedTrigger))]
    public async Task ExecuteCosmosDbChangeFeedAsync(
                            [CosmosDBTrigger(
                                databaseName: "db",
                                containerName: "Orders",
                                Connection = "CosmosDBConnection",
                                LeaseContainerName = "Orders-Leases",
                                CreateLeaseContainerIfNotExists = true)] IReadOnlyList<CreateOrderRequest> input,
                            [DurableClient] DurableTaskClient starter,
                            FunctionContext executionContext)
    {
        if (input is not null && input.Count > 0)
        {
            foreach (var item in input)
            {
                string instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(nameof(MaterializedViewProcessorOrchestrator), item, executionContext.CancellationToken);
                logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            }

            logger.LogInformation("Documents modified: " + input.Count);
        }
    }
}

