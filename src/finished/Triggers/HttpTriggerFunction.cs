namespace MaterializedViewProcessor.Triggers;

public class HttpTriggerFunction(CosmosClient client, ILoggerFactory loggerFactory)
{
    private readonly CosmosClient client = client; 
    private readonly Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<HttpTriggerFunction>();

    [Function("CreateOrder")]
    public async Task<HttpResponseData> ExecuteCreateOrderAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        try
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");
            // Function input comes from the request content. 
            var createOrderRequest = await req.GetJsonBody<CreateOrderRequest>();

            if (createOrderRequest is null)
            {
                return await req.ToBadRequestResponseAsync("Invalid request body");
            }

            Database database = await client.CreateDatabaseIfNotExistsAsync("db");
            Container container = await database.CreateContainerIfNotExistsAsync("Orders", "/customerId", 400);

            var createdItem =  await container.CreateItemAsync<CreateOrderRequest>(createOrderRequest);
            logger.LogInformation($"RU Used: {createdItem.RequestCharge:0.0}");

            return await req.ToOkResponseAsync("true");
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Exception: {ex}");
            return await req.ToBadRequestResponseAsync(ex.Message);
        }
    }
}
