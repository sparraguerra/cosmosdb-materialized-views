namespace MaterializedViewProcessor.Activities;

[DurableTask(nameof(UpdateOrdersByQuarterMaterializedViewActivity))]
public class UpdateOrdersByQuarterMaterializedViewActivity
                    (CosmosClient client, ILogger<UpdateOrdersByQuarterMaterializedViewActivity> logger)
                           : TaskActivity<CreateOrderRequest, bool>
{
    private readonly CosmosClient client = client;
    private readonly ILogger<UpdateOrdersByQuarterMaterializedViewActivity> logger = logger;

    public override async Task<bool> RunAsync(TaskActivityContext context, CreateOrderRequest input)
    {
        if (input is null)
        {
            return false;
        }

        Database database = await client.CreateDatabaseIfNotExistsAsync("db");
        Container container = await database.CreateContainerIfNotExistsAsync("OrdersByQuarter", "/qtr", 400);

        // find in container if the customerId exists
        var query = new QueryDefinition($"SELECT * FROM c WHERE c.qtr = @qtr AND c.customerId = @customerId")
                        .WithParameter("@qtr", GetQuarter(input.OrderDate))
                        .WithParameter("@customerId", input.CustomerId);
        var requestOptions = new QueryRequestOptions()
        {
            MaxItemCount = 1
        };
        var iterator = container.GetItemQueryIterator<OrdersByQuarter>(query, null, requestOptions);
        var result = await iterator.ReadNextAsync();

        if (result?.FirstOrDefault() is null)
        {
            OrdersByQuarter view = new()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = input.CustomerId,
                Customer = input.Customer,
                Qtr = GetQuarter(input.OrderDate),
                NumberOfOrders = 1,
                Total = input.OrderItems.Sum(x => x.Total)
            };

            // add to container 
            await container.CreateItemAsync(view, new PartitionKey(view.Qtr));
        }
        else
        {
            OrdersByQuarter view = result.First();
            view.NumberOfOrders += 1;
            view.Total += input.OrderItems.Sum(x => x.Total);

            // update in container  
            await container.ReplaceItemAsync(view, view.Id, new PartitionKey(view.Qtr));
        }

        return true;
    }

    // method to calculate Quarter given a DateTime
    private static string GetQuarter(DateTime? date) => date is not null ? 
                                                            $"{date.Value.Year}-Q{((date.Value.Month - 1) / 3) + 1}" : 
                                                            string.Empty;
}
